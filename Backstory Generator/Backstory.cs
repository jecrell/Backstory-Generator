using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
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
        Any,
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
        [XmlIgnore]
        public string label { get; set; }

        public string def { get; set; }
        public int degree { get; set; }
    }

    public class Backstory
    {
        [XmlAttribute]
        public string ParentName { get; set; }
        [XmlAttribute]
        public string Abstract { get; set; }
        
        [XmlIgnore]
        public string originalDefName { get; set; }

        public string defName { get; set; }
        public string title { get; set; }
        public string baseDescription { get; set; }
        public BodyType bodyTypeGlobal { get; set; }
        public bool ShouldSerializebodyTypeGlobal() { return bodyTypeGlobal != BodyType.Any; }

        public BodyType bodyTypeMale { get; set; }
        public bool ShouldSerializebodyTypeMale() { return bodyTypeMale != BodyType.Any; }

        public BodyType bodyTypeFemale { get; set; }
        public bool ShouldSerializebodyTypeFemale() { return bodyTypeFemale != BodyType.Any; }

        public Slot slot { get; set; }

        //RimWorld XML processes [Flag] enums with commas
        [XmlIgnore]
        public WorkTags workDisables { get; set; }
        [XmlElement("workDisables")]
        public string workDisablesString
        {
            get { return workDisables.ToString(); }
            set {

                //Makes this compatible with a previous test release
                //where the enums were spaced but not comma-seperated
                var processedValue = value;
                if (!processedValue.Contains(","))
                    processedValue = value.Replace(" ", ", ");
                workDisables = (WorkTags)Enum.Parse(typeof(WorkTags),value);
            }
        }


        //RimWorld XML processes [Flag] enums with commas
        [XmlIgnore]
        public WorkTags requiredWorkTags { get; set; }
        [XmlElement("requiredWorkTags")]
        public string requiredWorkTagsString
        {
            get { return requiredWorkTags.ToString(); }

            set {

                //Makes this compatible with a previous test release
                //where the enums were spaced but not comma-seperated
                var processedValue = value;
                if (!processedValue.Contains(","))
                    processedValue = value.Replace(" ", ", ");
                requiredWorkTags = (WorkTags)Enum.Parse(typeof(WorkTags), processedValue);
            }
        }



        [XmlArray("forcedTraits")]
        [XmlArrayItem("li")]
        public BindingList<TraitEntry> forcedTraits { get; set; }

        [XmlArray("disallowedTraits")]
        [XmlArrayItem("li")]
        public BindingList<TraitEntry> disallowedTraits { get; set; }

        [XmlArray("skillGains")]
        [XmlArrayItem("li")]
        public BindingList<SkillGain> skillGains { get; set; }

        [XmlArray("spawnCategories")]
        [XmlArrayItem("li")]
        public BindingList<string> spawnCategories { get; set; }

    }
}
