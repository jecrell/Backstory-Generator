using System;
using System.Collections;
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

    public enum BodyType
    {
        Male,
        Female,
        Fat,
        Hulk,
        Thin
    }

    public class SkillGain : IEqualityComparer
    {
        public SkillDef defName { get; set; }
        public int amount { get; set; }

        public new bool Equals(object x, object y)
        {
            if (x is SkillGain sx &&
                y is SkillGain sy &&
                sx.defName == sy.defName)
            {
                return true;
            }
            return false;
        }

        public int GetHashCode(object obj)
        {
            return (defName + amount).GetHashCode();
        }
    }

    [Flags]
    public enum WorkTags
    {
        None = 0,
        ManualDumb = 2,
        ManualSkilled = 4,
        Violent = 8,
        Caring = 16,
        Social = 32,
        Intellectual = 64,
        Animals = 128,
        Artistic = 256,
        Crafting = 512,
        Cooking = 1024,
        Firefighting = 2048,
        Cleaning = 4096,
        Hauling = 8192,
        PlantWork = 16384,
        Mining = 32768
    }


    public class TraitEntry
    {
        public string def { get; set; }
        public int degree { get; set; }
    }

    public class Backstory
    {
        [XmlAttribute]
        public string ParentName { get; set; }
        [XmlAttribute]
        public string Abstract { get; set; }
        
        public string defName { get; set; }
        public string title { get; set; }
        public string baseDescription { get; set; }
        public BodyType bodyTypeGlobal { get; set; }
        public BodyType bodyTypeMale { get; set; }
        public BodyType bodyTypeFemale { get; set; }
        public Slot slot { get; set; }
        public WorkTags workDisables { get; set; }
        public WorkTags requiredWorkTags { get; set; }

        [XmlArray("forcedTraits")]
        [XmlArrayItem("li")]
        public List<TraitEntry> forcedTraits { get; set; }

        [XmlArray("disallowedTraits")]
        [XmlArrayItem("li")]
        public List<TraitEntry> disallowedTraits { get; set; }

        [XmlArray("skillGains")]
        [XmlArrayItem("li")]
        public List<SkillGain> skillGains { get; set; }

        [XmlArray("spawnCategories")]
        [XmlArrayItem("li")]
        public List<string> spawnCategories { get; set; }

    }
}
