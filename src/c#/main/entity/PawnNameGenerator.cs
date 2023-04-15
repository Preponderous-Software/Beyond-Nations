using System.Collections.Generic;

namespace osg {

    public static class PawnNameGenerator {

        public static string[] names = {
            "Aldred",
            "Baldric",
            "Cedric",
            "Dunstan",
            "Eadric",
            "Fulke",
            "Godric",
            "Haimirich",
            "Isembard",
            "Jocelin",
            "Kilian",
            "Lambert",
            "Milo",
            "Nigel",
            "Osmund",
            "Percival",
            "Radulf",
            "Sigeric",
            "Tancrède",
            "Ursion",
            "Vivien",
            "Wymarc",
            "Xavier",
            "Ymbert",
            "Zacharie",
        };
        
        private static string[] familyNames = {
            "Aubert",
            "Baudin",
            "Carpentier",
            "Dumont",
            "Evrard",
            "Fournier",
            "Girard",
            "Huet",
            "Jacquet",
            "Lambert",
            "Martin",
            "Noël",
            "Olivier",
            "Perrin",
            "Quentin",
            "Renaud",
            "Simon",
            "Tessier",
            "Urbain",
            "Vasseur",
            "Wagner",
            "Xavier",
            "Yvon",
            "Zimmermann",
        };

        // list of generated
        private static List<string> generated = new List<string>();

        public static string generate() {
            string name = names[UnityEngine.Random.Range(0, names.Length)];
            string familyName = familyNames[UnityEngine.Random.Range(0, familyNames.Length)];
            string fullName = name + " " + familyName;
            if (generated.Contains(fullName)) {
                return generate();
            }
            generated.Add(fullName);
            return fullName;
        }
    }
}