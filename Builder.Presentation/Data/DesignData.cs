using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Builder.Presentation.Data
{

    public static class DesignData
    {
        private const string DocumentsFolder = "C:\\Users\\bas_d\\Documents\\";

        private const string PortraitsFolder = "C:\\Users\\bas_d\\Documents\\5e Character Builder\\portraits\\";

        private static readonly Random Random = new Random();

        public static string PortraitFilename => "C:\\Users\\bas_d\\Documents\\5e Character Builder\\portraits\\changeling-male.png";

        public static string[] PortraitFilenames => new string[15]
        {
        "C:\\Users\\bas_d\\Documents\\5e Character Builder\\portraits\\wood elf female rogue.jpg", "C:\\Users\\bas_d\\Documents\\5e Character Builder\\portraits\\dragonborn female.jpg", "C:\\Users\\bas_d\\Documents\\5e Character Builder\\portraits\\human male monk.jpg", "C:\\Users\\bas_d\\Documents\\5e Character Builder\\portraits\\changeling-male.png", "C:\\Users\\bas_d\\Documents\\5e Character Builder\\portraits\\changeling-female.png", "C:\\Users\\bas_d\\Documents\\5e Character Builder\\portraits\\dwarf-female.png", "C:\\Users\\bas_d\\Documents\\5e Character Builder\\portraits\\dwarf-male-1.png", "C:\\Users\\bas_d\\Documents\\5e Character Builder\\portraits\\elf-male-4.png", "C:\\Users\\bas_d\\Documents\\5e Character Builder\\portraits\\yuan ti pureblood female 1.jpg", "C:\\Users\\bas_d\\Documents\\5e Character Builder\\portraits\\kenku-2.png",
        "C:\\Users\\bas_d\\Documents\\5e Character Builder\\portraits\\svirfneblin male.jpg", "C:\\Users\\bas_d\\Documents\\5e Character Builder\\portraits\\half orc female.jpg", "C:\\Users\\bas_d\\Documents\\5e Character Builder\\portraits\\warforged.png", "C:\\Users\\bas_d\\Documents\\5e Character Builder\\portraits\\kobold male 3.png", "C:\\Users\\bas_d\\Documents\\5e Character Builder\\portraits\\tabaxi.png"
        };

        public static string[] CompanionFilenames => new string[4] { "C:\\Users\\bas_d\\Documents\\5e Character Builder\\portraits\\beast.jpg", "C:\\Users\\bas_d\\Documents\\5e Character Builder\\portraits\\owl.jpg", "C:\\Users\\bas_d\\Documents\\5e Character Builder\\portraits\\snake.jpg", "C:\\Users\\bas_d\\Documents\\5e Character Builder\\portraits\\boar.jpg" };

        public static string[] SymbolFilenames => new string[4] { "C:\\Users\\bas_d\\Documents\\5e Character Builder\\portraits\\Symbol of Azuth.png", "C:\\Users\\bas_d\\Documents\\5e Character Builder\\portraits\\Symbol of Savras.png", "C:\\Users\\bas_d\\Documents\\5e Character Builder\\portraits\\Symbol of Silvanus.png", "C:\\Users\\bas_d\\Documents\\5e Character Builder\\portraits\\Symbol of Mask.png" };

        public static string[] CharacterNames => new string[8] { "Cotillion", "Flaem Firespitter", "Dr. Ustabil", "Beguil the Bard", "Richard Cane", "Kurald Emurlahn", "Aust", "Ivellios Stormborn" };

        public static string GetRandomCharacterName()
        {
            string[] characterNames = GetCharacterNames();
            return characterNames[Random.Next(0, characterNames.Length)];
        }

        public static string[] GetCharacterNames()
        {
            return new string[11]
            {
            "Xarriz", "Cotillion", "Dr. Ustabil", "Beguil the Bard", "Richard Cane", "Kurald Emurlahn", "Seiðr", "Ivellios Stormborn", "Binwin Bronzebottom", "Jim Darkmagic",
            "Omin Dran"
            };
        }
    }
}
