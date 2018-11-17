using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace Backstory_Generator
{
    [XmlRoot("Defs")]
    public class BackstoryFile
    {

        [XmlElement("Backstory")]
        public BindingList<Backstory> Backstories;

        [XmlIgnore]
        private string fileName;
        [XmlIgnore]
        private int selectedIndex;
        [XmlIgnore]
        private int prevSelectedIndex;

        public int CurrentIndex { get => selectedIndex;
            set
            {
                prevSelectedIndex = selectedIndex;
                selectedIndex = value;
            }
        }

        public Backstory SelectedBackstory => Backstories[selectedIndex];
        
        public bool IsAlienRaceBackstory => File.ReadAllText(fileName).Contains("AlienRace.");
        

        public BackstoryFile()
        {
            selectedIndex = -1;
        }

        public BackstoryFile(string newFileName)
        {
            fileName = newFileName;
            selectedIndex = -1;
        }
        

        public static BackstoryFile Load(string fileName)
        {
            XmlSerializer ser = new XmlSerializer(typeof(BackstoryFile));
            string file = File.ReadAllText(fileName);

            string data = file
                    .Replace(BackstoryUtility.ErdsPrefix + "BackstoryDef", "Backstory")
                    .Replace(BackstoryUtility.JecsPrefix + "BackstoryDef", "Backstory");

            byte[] bytes = Encoding.UTF8.GetBytes(data);
            Stream s = new MemoryStream(bytes);

            var newBackstoryFile = ser.Deserialize(s) as BackstoryFile;
            s.Close();

            return newBackstoryFile;
        }

        //Saves the file
        public void Serialize(string prefix, bool showMessage = false)
        {
                XmlSerializer ser = new XmlSerializer(typeof(BackstoryFile));
                TextWriter writer = new StreamWriter(fileName);

                if (Backstories == null || Backstories?.Count == 0)
                {
                    Backstories = new BindingList<Backstory>();
                    Random random = new Random();
                    Backstory bs = new Backstory()
                    {
                        defName = "MyFirstBackstory" + random.Next(100, 999),
                        title = "Backstory Maker",
                        baseDescription = "Typing at the computers during [PAWN_possessive] younger days," +
                                          " [PAWN_nameDef] helped out with creating amazing backstories." +
                                          " [PAWN_pronoun] made amazing mods and learned the basics of" +
                                          " backstory creation. Unsurprisingly, this made [PAWN_objective] " +
                                          "awesome as well.",
                        slot = Slot.Adulthood
                    };
                    Backstories.Add(bs);
                    ser.Serialize(writer, this);
                }
                else
                    ser.Serialize(writer, this);
                writer.Close();

                //Add prefix for better backstory functionality
                string text = File.ReadAllText(fileName);
                text = text.Replace("Backstory", prefix + "BackstoryDef");
                text = text.Replace(" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"", "");
                File.WriteAllText(fileName, text);

                if (showMessage)
                    MessageBox.Show("Saved successfully");
        }
    }
}
