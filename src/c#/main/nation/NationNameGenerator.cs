using System.Collections.Generic;

namespace osg {

    public static class NationNameGenerator {

        private static string[] prefixes = new string[] {
            "Ald", "Alf", "Alv", "Arn", "As", "Bald", "Bj", "Borg", "Bri", "Bryn", "Dag", "Dalf", "Dj", "Dun", "Eg", "Eir", "Erl", "Fj", "Frey", "Frid", "Gard", "Gerd", "Gis", "Gj", "Gorm", "Grim", "Gud", "Gunn", "Gust", "Gutt", "Hal", "Har", "Hed", "Hel", "Her", "Hild", "Hjal", "Hjor", "Hol", "Ing", "Inge", "Ingj", "Iv", "Jalm", "Jarl", "Jof", "Jor", "Kalf", "Ket", "Kjell", "Knut", "Kol", "Koll", "Korm", "Kv", "Lamb", "Lif", "Lind", "Liv", "Magn", "Malm", "Mj", "Mun", "Nj", "Odd", "Ol", "Or", "Orn", "Ott", "Ragn", "Ran", "Rann", "Rik", "Sig", "Sigm", "Sigur", "Siv", "Skj", "Skjold", "Sv", "Svein", "Sven", "Tj", "Tor", "Torbj", "Tord", "Torg", "Torv", "Tyr", "Ulf", "Ulfr", "Vald", "Vall", "Varg", "Veg", "Vig", "Vigd", "Vil", "Vill", "Yng", "Yngv"
        };

        private static string[] suffixes = new string[] {
            "a", "bj", "bjorg", "bjorn", "borg", "d", "dis", "e", "frid", "g", "gard", "gerd", "gjerd", "gjert", "gjord", "gjurd", "gjut", "gjutt", "gjørd", "gjørt", "hild", "hildr", "hildur", "hildus", "hilduz", "hildyr", "hildyrs", "hildys", "hildyss", "hildyus", "hildyuz", "hildyr", "hildyrs", "hildys", "hildyss", "hildyus", "hildyuz", "hildyr", "hildyrs", "hildys", "hildyss", "hildyus", "hildyuz", "hildyr", "hildyrs", "hildys", "hildyss", "hildyus", "hildyuz", "hildyr", "hildyrs", "hildys", "hildyss", "hildyus", "hildyuz", "hildyr", "hildyrs", "hildys", "hildyss", "hildyus", "hildyuz", "hildyr", "hildyrs", "hildys", "hildyss", "hildyus", "hildyuz", "hildyr", "hildyrs", "hildys", "hildyss", "hildyus", "hildyuz", "hildyr", "hildyrs", "hildys", "hildyss", "hildyus", "hildyuz", "hildyr", "hildyrs", "hildys", "hildyss", "hildyus", "hildyuz", "hildyr", "hildyrs", "hildys", "hildyss", "hildyus", "hildyuz", "i", "ing", "inga", "ingr", "ingur",
        };

        // list of generated
        private static List<string> generated = new List<string>();

        public static string generate() {
            string prefix = prefixes[UnityEngine.Random.Range(0, prefixes.Length)];
            string suffix = suffixes[UnityEngine.Random.Range(0, suffixes.Length)];

            // check if name already generated
            string name = prefix + suffix;
            if (generated.Contains(name)) {
                return generate();
            }
            return name;
        }
    }
}