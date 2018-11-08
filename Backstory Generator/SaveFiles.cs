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
    public partial class Form1 : Form
    {

        private void SaveFileDialog()
        {
            // Displays a SaveFileDialog so the user can save the XML  
          
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "XML File|*.xml";
            saveFileDialog1.Title = "Save an XML File";
            saveFileDialog1.ShowDialog();

            // If the file name is not an empty string open it for saving.  
            if (saveFileDialog1.FileName != "")
            {
                // Saves the Image via a FileStream created by the OpenFile method.  
                Save(saveFileDialog1.FileName, true);
                MessageBox.Show("Created successfully");
                OpenFile(saveFileDialog1.FileName);
            }
        }


        private void Save(string filename, bool newFile = false)
        {
            XmlSerializer ser = new XmlSerializer(typeof(Defs));
            //XmlSerializer ser = new XmlSerializer(typeof(Backstory[]));
            TextWriter writer = new StreamWriter(filename);

            if (CurrentlyLoadedDefs == null || newFile)
            {
                Defs defs = new Defs();


                defs.Backstories = new BindingList<Backstory>();
                //var backstories = new Backstory[10];
                Random random = new Random();
                for (int i = 0; i < 5; i++)
                {
                    Backstory bs = new Backstory()
                    {
                        defName = "Derp" + random.Next(100, 999),
                        title = "Derper",
                        baseDescription = "Base derp derp",
                        slot = Slot.Adulthood,
                        skillGains = new List<SkillGain>() {
                        new SkillGain() { defName = SkillDef.Animals, amount = -100 }
                    }

                    };
                    defs.Backstories.Add(bs);
                    //backstories[i] = bs;
                }

                ser.Serialize(writer, defs);
            }
            else
                ser.Serialize(writer, CurrentlyLoadedDefs);
            //ser.Serialize(writer, backstories);
            writer.Close();

            //Add prefix for better backstory functionality
            string text = File.ReadAllText(filename);
            if (radioButtonAlienRace.Checked)
                text = text.Replace("Backstory", BackstoryUtility.ErdsPrefix + "BackstoryDef");
            else
                text = text.Replace("Backstory", BackstoryUtility.JecsPrefix + "BackstoryDef");
            text = text.Replace(" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"", "");
            File.WriteAllText(filename, text);

            MessageBox.Show("Saved successfully");

        }
    }
}
