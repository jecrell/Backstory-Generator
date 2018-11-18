using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Backstory_Generator
{

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
        public string titleFemale { get; set; }
        public bool ShouldSerializetitleFemale() { return titleFemale != ""; }
        public string titleShort { get; set; }
        public bool ShouldSerializetitleShort() { return titleShort != ""; }
        public string titleShortFemale { get; set; }
        public bool ShouldSerializetitleShortFemale() { return titleShortFemale != ""; }

        public string baseDescription { get; set; }
        public BodyType bodyTypeGlobal { get; set; }
        public bool ShouldSerializebodyTypeGlobal() { return bodyTypeGlobal != BodyType.Any; }

        public BodyType bodyTypeMale { get; set; }
        public bool ShouldSerializebodyTypeMale() { return bodyTypeMale != BodyType.Any; }

        public BodyType bodyTypeFemale { get; set; }
        public bool ShouldSerializebodyTypeFemale() { return bodyTypeFemale != BodyType.Any; }

        public Slot slot { get; set; }

        [XmlArray("workDisables")]
        [XmlArrayItem("li")]
        public BindingList<WorkTags> workDisables { get; set; }

        [XmlArray("requiredWorkTags")]
        [XmlArrayItem("li")]
        public BindingList<WorkTags> requiredWorkTags { get; set; }
        
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
