using System;
using System.Globalization;

namespace beyondnations.desktop {

    /**
    * How the host should run. Everything here has a working default, so
    * `dotnet run --project src/BeyondNations.Desktop` needs no arguments.
    */
    public class GameOptions {
        public int Width = 1280;
        public int Height = 720;
        public bool VSync = true;

        /**
        * Simulation rate, independent of frame rate. Unity's default fixed step
        * was 0.02 seconds, which is 50 a second; that cadence is preserved so
        * tick-driven behaviour keeps its current timing.
        */
        public int TicksPerSecond = 50;

        /**
        * Upper bound on how many fixed steps one frame may run to catch up after
        * a stall, so a long pause cannot spiral.
        */
        public int MaxCatchUpSteps = 5;

        /**
        * Run this many frames and exit. Zero means run until closed. This is what
        * makes the host checkable without a person watching a window.
        */
        public int ExitAfterFrames = 0;

        /**
        * World seed. Zero means pick one, matching GameConfig.
        */
        public int Seed = 0;

        /**
        * Resize the window part-way through the run. This exists so that resize
        * handling can be exercised without a person dragging a window corner.
        */
        public bool SmokeResize = false;

        /**
        * Take a screenshot once this many frames have rendered, then continue
        * running. Zero disables it. This is what makes the screenshot path
        * (glReadPixels -> PNG) checkable headlessly, without a person pressing
        * the screenshot key against a visible window. See #224.
        */
        public int ScreenshotAfterFrames = 0;

        /**
        * Record draw calls, frame times and render-loop allocation, and print
        * them on shutdown. The instanced renderer makes claims about scale and
        * about allocating nothing per object; this is how a run proves them.
        */
        public bool RenderStats = false;

        /**
        * Start at this render distance instead of the configured one, stepped
        * through the same increase and decrease Page Up and Page Down use and
        * so subject to the same bounds. Zero keeps the configured value. This
        * is what lets a run at fifty and a run at a thousand be compared
        * without anybody holding a key down. See #219.
        */
        public int RenderDistance = 0;

        /**
        * Submit everything, culling nothing. The counterfactual for the
        * culling claim: the same world drawn with the culler off says how much
        * work it was actually saving.
        */
        public bool NoCulling = false;

        public static GameOptions parse(string[] args) {
            GameOptions options = new GameOptions();
            for (int i = 0; i < args.Length; i++) {
                string arg = args[i];
                switch (arg) {
                    case "--width":            options.Width = intAfter(args, ref i, options.Width); break;
                    case "--height":           options.Height = intAfter(args, ref i, options.Height); break;
                    case "--ticks-per-second": options.TicksPerSecond = intAfter(args, ref i, options.TicksPerSecond); break;
                    case "--exit-after-frames":options.ExitAfterFrames = intAfter(args, ref i, options.ExitAfterFrames); break;
                    case "--screenshot-after-frames": options.ScreenshotAfterFrames = intAfter(args, ref i, options.ScreenshotAfterFrames); break;
                    case "--seed":             options.Seed = intAfter(args, ref i, options.Seed); break;
                    case "--no-vsync":         options.VSync = false; break;
                    case "--smoke-resize":     options.SmokeResize = true; break;
                    case "--render-stats":     options.RenderStats = true; break;
                    case "--render-distance":  options.RenderDistance = intAfter(args, ref i, options.RenderDistance); break;
                    case "--no-culling":       options.NoCulling = true; break;
                    case "--help":
                    case "-h":
                        printUsage();
                        System.Environment.Exit(0);
                        break;
                    default:
                        Console.Error.WriteLine("unknown argument: " + arg);
                        printUsage();
                        System.Environment.Exit(2);
                        break;
                }
            }
            return options;
        }

        private static int intAfter(string[] args, ref int i, int fallback) {
            if (i + 1 >= args.Length) {
                Console.Error.WriteLine("missing value after " + args[i]);
                System.Environment.Exit(2);
            }
            i++;
            int parsed;
            if (!int.TryParse(args[i], NumberStyles.Integer, CultureInfo.InvariantCulture, out parsed)) {
                Console.Error.WriteLine("not a number: " + args[i]);
                System.Environment.Exit(2);
            }
            return parsed;
        }

        private static void printUsage() {
            Console.WriteLine("Beyond Nations");
            Console.WriteLine();
            Console.WriteLine("  --width N               window width          (default 1280)");
            Console.WriteLine("  --height N              window height         (default 720)");
            Console.WriteLine("  --ticks-per-second N    simulation rate       (default 50)");
            Console.WriteLine("  --exit-after-frames N   render N frames then exit; 0 runs until closed");
            Console.WriteLine("  --screenshot-after-frames N  take a screenshot once N frames have rendered; 0 disables");
            Console.WriteLine("  --seed N                world seed; 0 picks one");
            Console.WriteLine("  --no-vsync              do not wait for vertical sync");
            Console.WriteLine("  --smoke-resize          resize part-way through, to exercise resize handling");
            Console.WriteLine("  --render-stats          report draw calls, frame times and allocation on exit");
            Console.WriteLine("  --render-distance N     start at this render distance; 0 keeps the configured one");
            Console.WriteLine("  --no-culling            submit every primitive, culling nothing");
        }
    }
}
