using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Backstory_Generator
{

    [XmlRoot("Defs")]
    public class Defs
    {
        [XmlElement("Backstory")]
        public BindingList<Backstory> Backstories;
    }
}
