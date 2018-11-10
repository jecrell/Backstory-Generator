using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backstory_Generator
{
    //From RimWorld 1.0.2059
    public static class TraitDefOf
    {
        public static List<int?> Nudist;

        public static List<int?> Brawler;

        public static List<int?> Abrasive;

        public static List<int?> Cannibal;

        public static List<int?> Ascetic;

        public static List<int?> Psychopath;

        public static List<int?> Greedy;

        public static List<int?> Kind;

        public static List<int?> Gay;
        
        public static List<int?> DislikesMen;

        public static List<int?> DislikesWomen;

        public static List<int?> AnnoyingVoice;

        public static List<int?> CreepyBreathing;

        public static List<int?> Bloodlust;

        public static List<int?> Pyromaniac;

        public static List<int?> TooSmart;

        public static List<int?> Transhumanist;

        public static List<int?> BodyPurist;

        public static List<int?> Undergrounder;

        public static List<int?> GreatMemory;

        public static List<int?> Tough;


        //Spectrum

        public static List<int?> DrugDesire = new List<int?>() { -1, 1, 2 };

        public static List<int?> Industriousness = new List<int?>() { 2, 1, -1, -2 };

        public static List<int?> Immunity = new List<int?>() { 1, -1 };

        public static List<int?> Beauty = new List<int?>() { 2, 1, -1, -2 };

        public static List<int?> Nerves = new List<int?>() { 2, 1, -1, -2 };

        public static List<int?> Neurotic = new List<int?>() { 1, 2, };

        public static List<int?> NaturalMood = new List<int?>() { 2, 1, -1, -2 };

        public static List<int?> PsychicSensitivity = new List<int?>() { 2, 1, -1, -2 };

        public static List<int?> SpeedOffset = new List<int?>() { -1, 1, 2 };

        public static List<int?> ShootingAccuracy = new List<int?>() { 1, -1 };
    }
}
