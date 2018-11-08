using System;
using System.Collections.Generic;
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

        private void OpenFileDialog()
        {
            // Displays an OpenFileDialog so the user can select a Cursor.  
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "XML Files|*.xml";
            openFileDialog1.Title = "Select an XML File";

            // Show the Dialog.  
            // If the user clicked OK in the dialog and  
            // a .XML file was selected, open it.  
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                // Assign the cursor in the Stream to the Form's Cursor property.  
                OpenFile(openFileDialog1.FileName);
            }
        }

        private void OpenFile(string fileName)
        {

            Defs defs = new Defs();

            XmlSerializer ser = new XmlSerializer(typeof(Defs));
            string file = File.ReadAllText(fileName);

            string data = file
                    .Replace(BackstoryUtility.ErdsPrefix + "BackstoryDef", "Backstory")
                    .Replace(BackstoryUtility.JecsPrefix + "BackstoryDef", "Backstory");
            
            byte[] bytes = Encoding.UTF8.GetBytes(data);
            Stream s = new MemoryStream(bytes);

            //using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            //{
            defs = ser.Deserialize(s) as Defs;
            //};

            s.Close();

            CurrentlyLoadedDefs = defs;
            CurrentlyLoadedFile = fileName;

            ShowFileControls(true);

            UpdateListBoxes(defs);

            if (BackstoryUtility.IsAlienRaceBackstory(fileName))
            {
                radioButtonAlienRace.Checked = true;
            }
            else
            {
                radioButtonVanilla.Checked = true;
            }
        }



    }
}
