using Backstory_Generator.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Backstory_Generator
{
    public class FormController
    {
        private FormViewer formViewer;
        public BackstoryFile LoadedBackstoryFile { get; set; }
        public List<TraitEntry> CurrentlyLoadedTraitEntries { get; set; }

        public FormController(FormViewer newFormViewer)
        {
            formViewer = newFormViewer;
        }

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

        public BackstoryFile OpenBackstoryFileDialog()
        {
            if (!TryLoadTraitEntries()) return null;

            BackstoryFile result = null;

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
                result = LoadBackstoryFile(openFileDialog1.FileName);
            }
            return result;
        }

        public BackstoryFile SaveBackstoryDialog(string prefix, bool newFile = false)
        {
            //if (!TryLoadTraitEntries()) return null;

            BackstoryFile saveFile = null;

            // Displays a SaveFileDialog so the user can save the XML 
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "XML File|*.xml";
            saveFileDialog1.Title = "Save an XML File";
            saveFileDialog1.ShowDialog();

            // If the file name is not an empty string open it for saving.  
            if (saveFileDialog1.FileName != "")
            {
                if (!File.Exists(saveFileDialog1.FileName))
                    saveFile = new BackstoryFile(saveFileDialog1.FileName);

                // Saves the Image via a FileStream created by the OpenFile method.  
                saveFile.Serialize(prefix, !newFile);
                MessageBox.Show("Created successfully");
                LoadBackstoryFile(saveFileDialog1.FileName);

            }
            return saveFile;
        }

        private BackstoryFile LoadBackstoryFile(string fileName, bool newFile = false)
        {
            var result = BackstoryFile.Load(fileName);

            formViewer.UpdateForm(UpdateEvent.LoadFile);


            return result;
        }

        internal bool Notify_RadioButtonChildhood()
        {
            try
            {
                if (LoadedBackstoryFile.SelectedBackstory.slot == Slot.Childhood)
                {
                    return true;
                }
                LoadedBackstoryFile.SelectedBackstory.slot = Slot.Childhood;
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString() + "\n" + e.StackTrace.ToString());
            }
            return false;
        }

        internal bool Notify_RadioButtonAdulthood()
        {
            try
            {
                if (LoadedBackstoryFile.SelectedBackstory.slot == Slot.Adulthood)
                {
                    return true;
                }
                LoadedBackstoryFile.SelectedBackstory.slot = Slot.Adulthood;
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString() + "\n" + e.StackTrace.ToString());
            }
            return false;
        }

        internal bool TryAddSkillGain(ComboBox comboBoxSkills)
        {
            try
            {
                Enum.TryParse<SkillDef>(comboBoxSkills.Items[comboBoxSkills.SelectedIndex].ToString(), out var skill);
                var newSkill = new SkillGain() { defName = skill, amount = 1 };
                formViewer.TryUpdatingBackstoryListBoxIndex(LoadedBackstoryFile.CurrentIndex, true);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString() + "\n" + e.StackTrace.ToString());
            }
            return false;
        }

        internal void AddNewBackstoryDef(TextBox textBoxAddDefName)
        {
            var newDefName = textBoxAddDefName.Text;
            if (newDefName != string.Empty)
            {
                if (LoadedBackstoryFile.Backstories.Any(x => x.defName == newDefName))
                {
                    MessageBox.Show("Backstory with same defName already exists. Try another defName.");
                    return;
                }
                var newBackstory = new Backstory { originalDefName = newDefName, defName = newDefName };
                LoadedBackstoryFile.Backstories.Add(newBackstory);
                MessageBox.Show("Added Backstory: " + newDefName);
                textBoxAddDefName.Text = "";
            }

            formViewer.UpdateForm(UpdateEvent.NewDef);
        }

        internal bool TryAddForcedTrait(ComboBox comboBoxTraitsForced)
        {
            var selectedItem = (TraitEntry)comboBoxTraitsForced.Items[comboBoxTraitsForced.SelectedIndex];

            Backstory selectedBackstory = LoadedBackstoryFile.SelectedBackstory;
            if (selectedBackstory.forcedTraits == null)
            {
                selectedBackstory.forcedTraits = new BindingList<TraitEntry>();
            }

            if (selectedBackstory.forcedTraits.Any(x => x.def == selectedItem.def)) return false;
            selectedBackstory.forcedTraits.Add(selectedItem);
            return true;
        }

        internal bool TryAddDisallowedTrait(ComboBox comboBoxTraitsDisabled)
        {
            var selectedItem = (TraitEntry)comboBoxTraitsDisabled.Items[comboBoxTraitsDisabled.SelectedIndex];

            Backstory selectedBackstory = LoadedBackstoryFile.SelectedBackstory;
            if (selectedBackstory.disallowedTraits == null)
            {
                selectedBackstory.disallowedTraits = new BindingList<TraitEntry>();
            }

            if (selectedBackstory.disallowedTraits.Any(x => x.def == selectedItem.def)) return false;
            selectedBackstory.disallowedTraits.Add(selectedItem);
            return true;
        }
      
    }
}
