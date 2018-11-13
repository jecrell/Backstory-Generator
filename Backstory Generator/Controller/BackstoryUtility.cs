using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backstory_Generator
{
    public static class BackstoryUtility
    {
        public static string JecsPrefix = "JecsTools.";
        public static string ErdsPrefix = "AlienRace.";


        public static bool IsAlienRaceBackstory(string fileName)
        {
            return File.ReadAllText(fileName).Contains(ErdsPrefix);
        }

        public static bool IsStandardBackstory(string fileName)
        {
            return File.ReadAllText(fileName).Contains(JecsPrefix);
        }
    }
}
