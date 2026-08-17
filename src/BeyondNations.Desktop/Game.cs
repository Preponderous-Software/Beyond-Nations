using System;
using System.Diagnostics;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using beyondnations;
using beyondnations.desktop.input;
using beyondnations.desktop.render;

namespace beyondnations.desktop {

    /**
    * The host: a window, a GL context and the loop that drives the simulation.
    *
    * Unity gave the project Update() and FixedUpdate() for free, and that split
    * is worth keeping: behaviour, metabolism and world generation are all
    * tick-driven and should not run faster on a faster machine. The loop below
    * accumulates real elapsed time and advances the simulation a whole number of
    * fixed steps, so the tick rate is stable and independent of frame rate.
    *
    * Drawing is delegated to PrimitiveRenderer, which turns the world snapshot
    * into a few instanced draw calls (#218), viewed through the camera and
    * filtered by the culler that came with #219; the UI arrives with #221.
    */
    public class Game : IDisposable {
        private readonly GameOptions options;
        private readonly GameConfig gameConfig;
        private readonly ScreenState screens = new ScreenState();

        private IWindow window;
        private GL gl;
        private IInputContext input;
        private InputService inputService;
        private readonly PlayerInputController playerInputController = new PlayerInputController();

        private Simulation simulation;
        private readonly WorldSnapshot snapshot = new WorldSnapshot();

        // --- #218 instanced renderer ---
        private PrimitiveRenderer renderer;
        private RenderStatsRecorder renderStats;
        // --- end #218 ---

        // --- #219 camera and culling ---
        private readonly PlayerCamera camera = new PlayerCamera();
        private readonly RenderCuller culler = new RenderCuller();
        private int lastLoggedRenderDistance;
        // --- end #219 ---

        // Fixed-step accumulator
        private readonly double fixedTimeStep;
        private double accumulator;

        private int framesRendered;
        private bool smokeResizeDone;
        private int fixedStepsRun;
        private readonly Stopwatch clock = new Stopwatch();

        // Screenshot capture (#224). The key binding itself is minimal and
        // temporary -- #217 owns the real binding table -- but the capture
        // path (glReadPixels -> PNG) lives here so it can be exercised
        // headlessly via --screenshot-after-frames.
        private bool screenshotKeyWasDown;
        private bool screenshotAfterFramesDone;
        private string lastScreenshotPath;

        public Game(GameOptions options) {
            this.options = options;
            this.gameConfig = new GameConfig();
            if (options.Seed != 0) {
                gameConfig.setWorldSeed(options.Seed);
            }
            this.fixedTimeStep = 1.0 / options.TicksPerSecond;
        }

        public int getFramesRendered() {
            return framesRendered;
        }

        public int getFixedStepsRun() {
            return fixedStepsRun;
        }

        public ScreenState getScreens() {
            return screens;
        }

        public Simulation getSimulation() {
            return simulation;
        }

        public string getLastScreenshotPath() {
            return lastScreenshotPath;
        }

        /**
        * Reads the current framebuffer and writes it as a PNG under
        * AppDataPaths.getScreenshotsDirectory(). Public so it can be
        * invoked both from the (minimal, #217-owned) key binding below and
        * from --screenshot-after-frames for headless verification.
        */
        public string takeScreenshot() {
            Vector2D<int> size = window.FramebufferSize;
            string path = ScreenshotCapture.capture(gl, size.X, size.Y, AppDataPaths.getScreenshotsDirectory());
            lastScreenshotPath = path;
            return path;
        }

        public void run() {
            WindowOptions windowOptions = WindowOptions.Default;
            windowOptions.Size = new Vector2D<int>(options.Width, options.Height);
            windowOptions.Title = "Beyond Nations";
            windowOptions.VSync = options.VSync;

            window = Window.Create(windowOptions);
            window.Load += onLoad;
            window.Update += onUpdate;
            window.Render += onRender;
            window.FramebufferResize += onFramebufferResize;
            window.Closing += onClosing;

            window.Run();
        }

        private void onLoad() {
            gl = GL.GetApi(window);
            input = window.CreateInput();
            inputService = new InputService(new SilkInputSource(input));

            Log.info("GL vendor:   " + gl.GetStringS(StringName.Vendor));
            Log.info("GL renderer: " + gl.GetStringS(StringName.Renderer));
            Log.info("GL version:  " + gl.GetStringS(StringName.Version));

            gl.Enable(EnableCap.DepthTest);
            gl.Enable(EnableCap.CullFace);
            gl.CullFace(TriangleFace.Back);
            gl.ClearColor(0.45f, 0.65f, 0.90f, 1.0f);

            // --- #218 instanced renderer ---
            renderer = new PrimitiveRenderer(gl);
            if (options.RenderStats) {
                renderStats = new RenderStatsRecorder();
            }
            // --- end #218 ---

            onFramebufferResize(window.FramebufferSize);

            // Straight into the world for now. The title and menu screens are
            // reachable through the state machine but have nothing to draw until
            // the ImGui port in #221, so starting on the title screen would show
            // an empty window and look like a failure.
            simulation = new Simulation(gameConfig);
            // --- #219 camera and culling ---
            if (options.RenderDistance > 0) {
                applyStartingRenderDistance(simulation.getPlayer(), options.RenderDistance);
                Log.info("starting render distance: " + simulation.getPlayer().getRenderDistance());
            }
            culler.setEnabled(!options.NoCulling);
            // --- end #219 ---
            // The binding table itself arrives with #217; the default key is N.
            simulation.getPlayer().getStatus().update("Press N to create a nation.");
            screens.goTo(ScreenType.WORLD);

            clock.Start();
            Log.info("host ready: " + options.TicksPerSecond + " ticks per second");
        }

        /**
        * Variable rate. Runs once per frame, then advances the simulation by as
        * many whole fixed steps as the elapsed time allows.
        */
        private void onUpdate(double deltaTime) {
            readInput();

            if (!screens.shouldAdvanceSimulation() || simulation == null) {
                return;
            }

            accumulator += deltaTime;

            // Clamp the backlog so that a long stall -- a breakpoint, a resize,
            // a paused window -- cannot make the next frame run thousands of
            // steps trying to catch up.
            double maxBacklog = fixedTimeStep * options.MaxCatchUpSteps;
            if (accumulator > maxBacklog) {
                accumulator = maxBacklog;
            }

            while (accumulator >= fixedTimeStep) {
                simulation.getTickCounter().increment();
                simulation.fixedUpdate((float) fixedTimeStep);
                accumulator -= fixedTimeStep;
                fixedStepsRun++;
            }

            if (options.SmokeResize && !smokeResizeDone && framesRendered > 10) {
                smokeResizeDone = true;
                Vector2D<int> resized = new Vector2D<int>(options.Width / 2, options.Height / 2);
                Log.info("smoke: resizing window to " + resized.X + "x" + resized.Y);
                window.Size = resized;
            }

            if (options.ScreenshotAfterFrames > 0 && !screenshotAfterFramesDone && framesRendered >= options.ScreenshotAfterFrames) {
                screenshotAfterFramesDone = true;
                takeScreenshot();
            }

            if (options.ExitAfterFrames > 0 && framesRendered >= options.ExitAfterFrames) {
                window.Close();
            }
        }

        /**
        * The binding table lives in KeyBindings and is applied by
        * PlayerInputController (src/BeyondNations.Desktop/input); see #217.
        * Escape is the one binding that stays here, since toggling screens can
        * close the window, which only the host owns.
        */
        private void readInput() {
            if (inputService == null) {
                return;
            }

            inputService.update();

            if (inputService.wasPressedThisFrame(KeyBindings.Pause)) {
                if (!screens.escapePressed()) {
                    window.Close();
                }
            }

            // Screenshot key, preserved from the original KeyBindings.takeScreenshot
            // (F12). This is deliberately minimal -- #217 owns the real binding
            // table -- it just proves the capture path still works from a keypress.
            bool screenshotKeyIsDown = false;
            foreach (IKeyboard keyboard in input.Keyboards) {
                if (keyboard.IsKeyPressed(Key.F12)) {
                    screenshotKeyIsDown = true;
                }
            }
            if (screenshotKeyIsDown && !screenshotKeyWasDown) {
                takeScreenshot();
            }
            screenshotKeyWasDown = screenshotKeyIsDown;

            if (simulation == null || !screens.isWorldActive()) {
                return;
            }

            playerInputController.update(simulation, inputService);
        }

        /**
        * Variable rate. Draws whatever the simulation currently is; it never
        * changes it.
        */
        private void onRender(double deltaTime) {
            renderStats?.beginFrame();

            gl.Clear((uint) (ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit));

            if (simulation != null && screens.isWorldActive()) {
                // The snapshot is rebuilt into the same buffers every frame and
                // is what the renderer draws from. Capturing it here keeps the
                // seam honest: the host reads the simulation, the renderer reads
                // the snapshot, and neither reaches past the other.
                snapshot.capture(simulation.getEntityRepository(), simulation.getEnvironment());

                // --- #219 camera and culling ---
                // The camera trails the player exactly as the parented Unity
                // one did, its far plane is the render distance Page Up and
                // Page Down move, and the culler drops everything out of the
                // view volume or past that distance before an instance buffer
                // is touched.
                Player player = simulation.getPlayer();
                MouseLook mouseLook = playerInputController.getMouseLook();
                camera.setRenderDistance(player.getRenderDistance());
                camera.follow(player.getPosition(), player.getYaw(), mouseLook.getYawDegrees(), mouseLook.getPitchDegrees());
                culler.beginFrame(camera);
                renderer.render(snapshot, camera.getViewMatrix(), camera.getProjectionMatrix(), culler);
                reportRenderDistance(player.getRenderDistance());
                // --- end #219 ---
            }

            framesRendered++;
            // --- #219 camera and culling ---
            renderStats?.recordCamera(
                simulation != null ? simulation.getPlayer().getRenderDistance() : 0,
                renderer != null ? renderer.getInstancesCulled() : 0);
            // --- end #219 ---
            renderStats?.endFrame(
                renderer != null ? renderer.getDrawCallCount() : 0,
                renderer != null ? renderer.getInstancesDrawn() : 0);
        }

        // --- #219 camera and culling ---
        /**
        * The debug overlay is #221's; until it exists, this is where the render
        * distance can be seen changing under Page Up and Page Down. Only
        * changes are logged, so a run is not drowned in one line per frame.
        */
        private void reportRenderDistance(int renderDistance) {
            if (!options.RenderStats || renderDistance == lastLoggedRenderDistance) {
                return;
            }
            lastLoggedRenderDistance = renderDistance;
            Log.info("render distance: " + renderDistance);
        }

        public PlayerCamera getCamera() {
            return camera;
        }

        public RenderCuller getCuller() {
            return culler;
        }

        /**
        * Steps the render distance towards a target using the same increase and
        * decrease the Page Up and Page Down bindings call, so a value this flag
        * cannot be reached by a player is not reachable here either. Stops as
        * soon as stepping stops changing anything, which is what the clamp on
        * Player does at either bound.
        */
        private static void applyStartingRenderDistance(Player player, int target) {
            int previous = -1;
            while (player.getRenderDistance() != previous) {
                previous = player.getRenderDistance();
                if (previous + 10 <= target) {
                    player.increaseRenderDistance();
                } else if (previous - 10 >= target) {
                    player.decreaseRenderDistance();
                } else {
                    return;
                }
            }
        }
        // --- end #219 ---

        private void onFramebufferResize(Vector2D<int> size) {
            if (gl == null) {
                return;
            }
            // A minimised window reports zero, which is not a valid viewport.
            int width = Math.Max(1, size.X);
            int height = Math.Max(1, size.Y);
            gl.Viewport(0, 0, (uint) width, (uint) height);
            camera.setAspectRatio(width, height);
            Log.info("framebuffer resized to " + size.X + "x" + size.Y + ", viewport set to " + width + "x" + height);
        }

        private void onClosing() {
            clock.Stop();
            double seconds = clock.Elapsed.TotalSeconds;
            Log.info(string.Format(
                "shutting down after {0} frames and {1} fixed steps in {2:F2}s ({3:F1} fps, {4:F1} tps)",
                framesRendered, fixedStepsRun, seconds,
                seconds > 0 ? framesRendered / seconds : 0,
                seconds > 0 ? fixedStepsRun / seconds : 0));

            // --- #218 instanced renderer ---
            if (renderStats != null && renderStats.hasData()) {
                Log.info(renderStats.summarize());
            }
            // Buffers and programs have to be deleted while the context that
            // owns them still exists, which by Dispose() time it does not.
            renderer?.Dispose();
            renderer = null;
            // --- end #218 ---
        }

        public WorldSnapshot getSnapshot() {
            return snapshot;
        }

        public PrimitiveRenderer getRenderer() {
            return renderer;
        }

        public void Dispose() {
            renderer?.Dispose();
            input?.Dispose();
            gl?.Dispose();
            window?.Dispose();
        }
    }
}
