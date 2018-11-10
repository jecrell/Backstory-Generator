using Backstory_Generator.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml.XPath;

namespace Backstory_Generator
{
    public partial class MainDialog : Form
    {

        private bool TryLoadTraitEntries()
        {
            List<TraitEntry> newTraitEntries = new List<TraitEntry>();
            string traitsPath = Settings.Default["RimWorldPath"].ToString() + @"\Mods\Core\Defs\TraitDefs";

            List<string> xmlFiles = new List<string>() {
                traitsPath + @"\" + "Traits_Singular.xml",
                traitsPath + @"\" + "Traits_Spectrum.xml" };

            if (!Directory.Exists(traitsPath))
            {
                var result = MessageBox.Show("No RimWorld Path", "No directory path to RimWorld set. Set RimWorld path now?", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    SettingsDialog form2 = new SettingsDialog();
                    form2.Show();
                }
                return false;
            }

            foreach (string xmlFile in xmlFiles)
            {
                if (!File.Exists(xmlFile))
                {
                    MessageBox.Show("Cannot find file at \"" + xmlFile +
                        "\" Was it removed or deleted? " +
                        "Check RimWorld path in the settings menu.");
                    continue;
                }

                XDocument doc = XDocument.Load(xmlFile);


                var defRoot = "/Defs/";
                var defString = "TraitDef";
                var defCount = doc.XPathSelectElements(defRoot + defString).Count();


                for (int i = 1; i <= defCount; i++)
                {

                    string defName =
                        doc.XPathSelectElement(defRoot + defString + "[" + i + "]/defName").Value;
                    
                    var degreeCount = doc.XPathSelectElements(defRoot + defString + "[" + i + "]/degreeDatas/li").Count();


                    for (int j = 1; j <= degreeCount; j++)
                    {


                        string newLabel = doc.XPathSelectElement(
                            defRoot + defString + "[" + i + "]/degreeDatas/li[" + j + "]/label").Value;

                        XElement newDegreeElement = doc.XPathSelectElement(
                            defRoot + defString + "[" + i + "]/degreeDatas/li[" + j + "]/degree");
                        int newDegree = default(int);
                        if (newDegreeElement != null)
                            Int32.TryParse(newDegreeElement.Value, out newDegree);

                        newTraitEntries.Add(new TraitEntry() { label = newLabel.FirstCharToUpper(), def = defName, degree = newDegree });
                        //MessageBox.Show(newLabel + " " + defName + " " + newDegree);

                    }
                }
                CurrentlyLoadedTraitEntries = newTraitEntries;

            }

            return true;
        }

        private void OpenFileDialog()
        {
            if (!TryLoadTraitEntries()) return;

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
