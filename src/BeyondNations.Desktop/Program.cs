using System;
using beyondnations;

namespace beyondnations.desktop {

    public static class Program {

        public static int Main(string[] args) {
            // Core logs through a sink so that it needs no engine and no console
            // of its own. The host decides where the output goes.
            Log.setSink((level, message) => {
                if (level == LogLevel.Error) {
                    Console.Error.WriteLine("[error] " + message);
                } else if (level == LogLevel.Warning) {
                    Console.Error.WriteLine("[warn ] " + message);
                } else {
                    Console.WriteLine("[info ] " + message);
                }
            });

            GameOptions options = GameOptions.parse(args);

            try {
                using (Game game = new Game(options)) {
                    game.run();
                }
            } catch (Exception e) {
                Console.Error.WriteLine("[error] the host failed to start: " + e.Message);
                Console.Error.WriteLine(e);
                return 1;
            }

            return 0;
        }
    }
}
