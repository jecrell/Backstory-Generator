using System.Xml.Serialization;

namespace Backstory_Generator
{
    public class TraitEntry
    {
        [XmlIgnore]
        public string label { get; set; }

        public string defName { get; set; }
        public int degree { get; set; }
    }
}
