using System.Collections.Generic;
using UnityEngine;

namespace osg {

    public static class NationNameGenerator {

        private static string[] prefixes = new string[] {
            "New",
            "Old",
            "Great",
            "Little",
            "Big",
            "Small",
            "Red",
            "Blue",
            "Green",
            "Yellow",
            "Black",
            "White",
            "North",
            "South",
            "East",
            "West",
            "Upper",
            "Lower",
            "Northern",
            "Southern",
            "Eastern",
            "Western",
            "Fort",
            "Fortress",
            "Castle",
            "City",
            "Town",
            "Village",
            "Hamlet",
            "Farm"
        };

        private static string[] suffixes = new string[] {
            "shire",
            "land",
            "ton",
            "ville",
            "town",
            "burg",
            "port",
            "ford",
            "ham",
            "field",
            "wood"
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