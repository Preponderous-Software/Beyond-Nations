using System;
using System.Diagnostics;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using beyondnations;
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
    * into a few instanced draw calls (#218). The camera it is given is the
    * placeholder #219 replaces; the UI arrives with #221.
    */
    public class Game : IDisposable {
        private readonly GameOptions options;
        private readonly GameConfig gameConfig;
        private readonly ScreenState screens = new ScreenState();

        private IWindow window;
        private GL gl;
        private IInputContext input;

        private Simulation simulation;
        private readonly WorldSnapshot snapshot = new WorldSnapshot();

        // --- #218 instanced renderer ---
        private PrimitiveRenderer renderer;
        private readonly PlaceholderCamera camera = new PlaceholderCamera();
        private RenderStatsRecorder renderStats;
        // --- end #218 ---

        // Fixed-step accumulator
        private readonly double fixedTimeStep;
        private double accumulator;

        private int framesRendered;
        private bool smokeResizeDone;
        private int fixedStepsRun;
        private readonly Stopwatch clock = new Stopwatch();

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

            if (options.ExitAfterFrames > 0 && framesRendered >= options.ExitAfterFrames) {
                window.Close();
            }
        }

        private void readInput() {
            if (input == null) {
                return;
            }

            foreach (IKeyboard keyboard in input.Keyboards) {
                if (keyboard.IsKeyPressed(Key.Escape)) {
                    if (!screens.escapePressed()) {
                        window.Close();
                    }
                }
            }

            if (simulation == null || !screens.isWorldActive()) {
                return;
            }

            // The full binding table arrives with #217. What is wired here is
            // only enough to prove the host feeds the simulation rather than the
            // simulation polling an engine.
            float horizontal = 0f;
            float vertical = 0f;
            bool sprinting = false;
            foreach (IKeyboard keyboard in input.Keyboards) {
                if (keyboard.IsKeyPressed(Key.A)) horizontal -= 1f;
                if (keyboard.IsKeyPressed(Key.D)) horizontal += 1f;
                if (keyboard.IsKeyPressed(Key.W)) vertical += 1f;
                if (keyboard.IsKeyPressed(Key.S)) vertical -= 1f;
                if (keyboard.IsKeyPressed(Key.ShiftLeft)) sprinting = true;
                if (keyboard.IsKeyPressed(Key.Space)) simulation.getPlayer().requestJump();
            }
            simulation.getPlayer().setMovementInput(horizontal, vertical);
            simulation.getPlayer().setSprinting(sprinting);
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

                // --- #218 instanced renderer ---
                // The camera is the placeholder #219 replaces; the renderer only
                // wants a view and a projection and does not care whose they are.
                camera.follow(simulation.getPlayer().getPosition());
                renderer.render(snapshot, camera.getViewMatrix(), camera.getProjectionMatrix());
                // --- end #218 ---
            }

            framesRendered++;
            renderStats?.endFrame(
                renderer != null ? renderer.getDrawCallCount() : 0,
                renderer != null ? renderer.getInstancesDrawn() : 0);
        }

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
