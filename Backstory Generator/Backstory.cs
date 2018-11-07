using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Backstory_Generator
{
    public enum Slot
    {
        Childhood = 0,
        Adulthood
    }

    public enum SkillDef
    {
        Animals,
        Artistic,
        Construction,
        Cooking,
        Crafting,
        Intellectual,
        Medicine,
        Melee,
        Mining,
        Plants,
        Shooting,
        Social
    }

    public class SkillGain
    {
        public SkillDef defName { get; set; }
        public int amount { get; set; }
    }

    public class Backstory
    {
        public string defName { get; set; }
        public string title { get; set; }
        public string baseDescription { get; set; }
        public Slot slot { get; set; }

        [XmlArray("skillGains")]
        [XmlArrayItem("li")]
        public List<SkillGain> skillGains { get; set; }

    }
}
