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
        public TraitEntryFile LoadedTraitEntryFile { get; set; }

        public FormController(FormViewer newFormViewer)
        {
            formViewer = newFormViewer;
        }

        public bool TryLoadTraitEntries()
        {
            if (LoadedTraitEntryFile == null)
            {
                LoadedTraitEntryFile = TraitEntryFile.Load(Settings.Default["RimWorldPath"].ToString() + @"\Mods\Core\Defs\TraitDefs");
            }
            if (LoadedTraitEntryFile != null)
            {
                return true;
            }
            return false;
        }

        public void OpenBackstoryFileDialog()
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
                LoadedBackstoryFile = LoadBackstoryFile(openFileDialog1.FileName);
                LoadedTraitEntryFile.UpdateLabelsFor(LoadedBackstoryFile);
            }
            formViewer.UpdateForm(UpdateEvent.LoadFile);
        }

        public BackstoryFile SaveBackstoryDialog(bool AlienRace = false, bool newFile = false)
        {
            if (!TryLoadTraitEntries()) return null;

            string prefix = AlienRace ? BackstoryUtility.ErdsPrefix : BackstoryUtility.JecsPrefix;

            BackstoryFile saveFile = null;

            // Displays a SaveFileDialog so the user can save the XML 
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "XML File|*.xml";
            saveFileDialog1.Title = "Save an XML File";
            saveFileDialog1.ShowDialog();

            // If the file name is not an empty string open it for saving.  
            if (saveFileDialog1.FileName != "")
            {
                saveFile = new BackstoryFile(saveFileDialog1.FileName);
                
                // Saves the Image via a FileStream created by the OpenFile method.  
                saveFile.Save(prefix, !newFile);
                //MessageBox.Show("Created successfully");
                LoadedBackstoryFile = LoadBackstoryFile(saveFileDialog1.FileName);
                LoadedTraitEntryFile.UpdateLabelsFor(LoadedBackstoryFile);
            }
            formViewer.UpdateForm(UpdateEvent.CreateFile);
            return saveFile;
        }

        private BackstoryFile LoadBackstoryFile(string fileName, bool newFile = false)
        {
            var result = BackstoryFile.Load(fileName);
            return result;
        }

        internal bool TryGettingBackstory(out Backstory selectedBackstory, bool showMessage = true)
        {
            selectedBackstory = null;
            if (LoadedBackstoryFile != null)
            {
                //No message returned here. The files must be loading.
                if (LoadedBackstoryFile.CurrentIndex == -1)
                    return false;
                
                if (LoadedBackstoryFile.SelectedBackstory != null)
                {
                    selectedBackstory = LoadedBackstoryFile.SelectedBackstory;
                    return true;
                }
                if (showMessage) MessageBox.Show("No backstory selected." + "\n\n" + Environment.StackTrace.ToString() );
                return false;
            }
            if (showMessage) MessageBox.Show("Failed to load backstory file." + "\n\n" + Environment.StackTrace.ToString() );
            return false;
        }

        internal bool Notify_RadioButtonChildhood()
        {
            try
            {
                if (!TryGettingBackstory(out Backstory selectedBackstory)) return false;
                if (selectedBackstory.slot == Slot.Childhood)
                {
                    return true;
                }
                selectedBackstory.slot = Slot.Childhood;
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
                if (!TryGettingBackstory(out Backstory selectedBackstory)) return false;
                if (selectedBackstory.slot == Slot.Adulthood)
                {
                    return true;
                }
                selectedBackstory.slot = Slot.Adulthood;
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString() + "\n" + e.StackTrace.ToString());
            }
            return false;
        }

        internal void SelectNewIndex(int selectedIndex)
        {
            if (LoadedBackstoryFile == null) return;

            LoadedBackstoryFile.CurrentIndex = selectedIndex;
            if (selectedIndex < 0)
                formViewer.UpdateForm(UpdateEvent.DeselectDef);
            else
                formViewer.UpdateForm(UpdateEvent.SelectDef);
        }

        internal bool TryAddSkillGain(ComboBox comboBoxSkills, DataGridView dataGridView)
        {
            try
            {
                if (!TryGettingBackstory(out Backstory selectedBackstory)) return false;
                Enum.TryParse<SkillDef>(comboBoxSkills.Items[comboBoxSkills.SelectedIndex].ToString(), out var skill);
                
                
                bool newList = false;
                if (selectedBackstory.skillGains == null || selectedBackstory.skillGains.Count() == 0)
                {
                    newList = true;
                    selectedBackstory.skillGains = new BindingList<SkillGain>();
                }
                else
                {
                    if (selectedBackstory.skillGains.FirstOrDefault(x => x.defName == skill) != null)
                    {
                        MessageBox.Show("Skill gain entry \"" + skill + "\" already exists.");
                        return false;
                    }
                }

                var newSkill = new SkillGain() { defName = skill, amount = 1 };

                selectedBackstory.skillGains.Add(newSkill);
                if (newList)
                    GridViewUtility.UpdateView(dataGridView, selectedBackstory.skillGains,
                        selectedBackstory != null ? new int[] { 125, 25, 25 } : null);

                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString() + "\n" + e.StackTrace.ToString());
            }
            return false;
        }

        internal bool TryAddNewBackstoryDef(TextBox textBoxAddDefName)
        {
            var newDefName = textBoxAddDefName.Text;
            if (newDefName != string.Empty)
            {
                if (LoadedBackstoryFile.Backstories.Any(x => x.defName == newDefName))
                {
                    MessageBox.Show("Backstory with same defName already exists. Try another defName.");
                    return false;
                }
                var newBackstory = new Backstory { originalDefName = newDefName, defName = newDefName };
                LoadedBackstoryFile.Backstories.Add(newBackstory);
                MessageBox.Show("Added Backstory: " + newDefName);
                textBoxAddDefName.Text = "";
            }

            formViewer.UpdateForm(UpdateEvent.NewDef);
            return true;
        }

        internal bool TryAddForcedTrait(ComboBox comboBoxTraitsForced, DataGridView dataGridView)
        {
            if (!TryGettingBackstory(out Backstory selectedBackstory)) return false;

            var selectedItem = (TraitEntry)comboBoxTraitsForced.Items[comboBoxTraitsForced.SelectedIndex];



            bool newList = false;
            if (selectedBackstory.forcedTraits == null || selectedBackstory.forcedTraits.Count() == 0)
            {
                newList = true;
                selectedBackstory.forcedTraits = new BindingList<TraitEntry>();
            }

            if (selectedBackstory?.forcedTraits?.FirstOrDefault(x => x.defName == selectedItem.defName) != null)
            {
                MessageBox.Show("Failed to add new forced trait entry. Trait entry already exists.");
                return false;
            }
            
            selectedBackstory.forcedTraits.Add(selectedItem);

            if (dataGridView.DataSource != selectedBackstory.forcedTraits)
                dataGridView.DataSource = selectedBackstory.forcedTraits;


            if (newList)
                GridViewUtility.UpdateView(dataGridView, selectedBackstory?.forcedTraits,
                    selectedBackstory != null ? new int[] { 135, 25 } : null,
                    selectedBackstory != null ? new List<string> { "defName", "degree" } : null);
            return true;
        }

        internal bool TryAddDisallowedTrait(ComboBox comboBoxTraitsDisabled, DataGridView dataGridView)
        {
            if (!TryGettingBackstory(out Backstory selectedBackstory)) return false;

            var selectedItem = (TraitEntry)comboBoxTraitsDisabled.Items[comboBoxTraitsDisabled.SelectedIndex];

            bool newList = false;
            if (selectedBackstory.disallowedTraits == null || selectedBackstory.disallowedTraits.Count() == 0)
            {
                newList = true;
                selectedBackstory.disallowedTraits = new BindingList<TraitEntry>();
            }

            if (selectedBackstory.disallowedTraits.Any(x => x.defName == selectedItem.defName))
            {
                MessageBox.Show("Failed to add new disabled trait entry. Trait entry already exists.");
                return false;
            }

            selectedBackstory.disallowedTraits.Add(selectedItem);

            if (dataGridView.DataSource != selectedBackstory.disallowedTraits)
                dataGridView.DataSource = selectedBackstory.disallowedTraits;

            if (newList)
                GridViewUtility.UpdateView(dataGridView, selectedBackstory.disallowedTraits,
                    selectedBackstory != null ? new int[] { 135, 25 } : null,
                    selectedBackstory != null ? new List<string> { "defName", "degree" } : null);

            return true;
        }

        internal bool TryAddRequiredWorkType(ComboBox comboBoxRequiredWorkTypes, RichTextBox richTextBox)
        {
            if (!TryGettingBackstory(out Backstory selectedBackstory)) return false;

            var box = comboBoxRequiredWorkTypes;
            var selectedItem = (WorkTags)box.SelectedItem;

            bool newList = false;
            if (selectedBackstory.requiredWorkTags == null || selectedBackstory.requiredWorkTags.Count == 0)
            {
                newList = true;
                selectedBackstory.requiredWorkTags = new BindingList<WorkTags>();
            }

            if (selectedBackstory.requiredWorkTags.Contains(selectedItem))
                return false;
            selectedBackstory.requiredWorkTags.Add(selectedItem);
            
                richTextBox.Text = selectedBackstory?.requiredWorkTags?.FirstOrDefault() == null ? "" : string.Join(", ", selectedBackstory.requiredWorkTags); //bs?.requiredWorkTags.ToString() ?? "";
            return true;
        }


        internal bool TryAddDisabledWorkType(ComboBox comboBoxWorkTypesDisabled, RichTextBox richTextBox)
        {
            if (!TryGettingBackstory(out Backstory selectedBackstory)) return false;

            var box = comboBoxWorkTypesDisabled;
            var selectedItem = (WorkTags)box.SelectedItem;

            bool newList = false;
            if (selectedBackstory.workDisables == null || selectedBackstory.workDisables.Count == 0)
            {
                newList = true;
                selectedBackstory.workDisables = new BindingList<WorkTags>();
            }

            if (selectedBackstory.workDisables.Contains(selectedItem))
                return false;
            selectedBackstory.workDisables.Add(selectedItem);

            
                richTextBox.Text = selectedBackstory?.workDisables?.FirstOrDefault() == null ? "" : string.Join(", ", selectedBackstory.workDisables);//bs?.workDisables.ToString() ?? "";
            


            return true; ;
        }
        internal bool AdjustDefName(TextBox textBoxDefName)
        {
            if (!TryGettingBackstory(out Backstory selectedBackstory)) return false;
            selectedBackstory.defName = textBoxDefName.Text = BackstoryUtility.RemoveSpecialCharacters(textBoxDefName.Text);
            return true;
        }

        internal bool AdjustTitle(TextBox textBoxTitle)
        {
            if (!TryGettingBackstory(out Backstory selectedBackstory)) return false;
            selectedBackstory.title = textBoxTitle.Text;
            return true;
        }

        internal bool AdjustDescription(RichTextBox richTextBoxDescription)
        {
            if (!TryGettingBackstory(out Backstory selectedBackstory)) return false;
            selectedBackstory.baseDescription = richTextBoxDescription.Text;
            return true;
        }

        internal bool AdjustSpawnCategories(RichTextBox richTextBoxSpawnCategories)
        {
            if (!TryGettingBackstory(out Backstory selectedBackstory)) return false;

            if (richTextBoxSpawnCategories.Text.Trim() == string.Empty) return false;

            // This is a comma seperated value list. Warn the user.
            if (richTextBoxSpawnCategories.Text.FirstOrDefault(x => x == ' ') != default(char) &&
                !richTextBoxSpawnCategories.Text.Contains(','))
            {
                MessageBox.Show("Use commas to seperate spawn categories.\n\tE.g. RangedShooting, Tribal, Giant");
                richTextBoxSpawnCategories.Text = richTextBoxSpawnCategories.Text.Replace(' ', ',');
            }
            
            selectedBackstory.spawnCategories = new BindingList<string>(richTextBoxSpawnCategories.Text.Replace(" ", "").Split(','));
            return true;
        }

        internal bool AdjustBodyTypeGlobal(ComboBox comboBoxBodyTypeGlobal)
        {
            if (!TryGettingBackstory(out Backstory selectedBackstory, false)) return false;

            var box = comboBoxBodyTypeGlobal;
            var selectedItem = (BodyType)box.Items[box.SelectedIndex];

            selectedBackstory.bodyTypeGlobal = selectedItem;
            return true;
        }

        internal bool AdjustBodyTypeMale(ComboBox comboBoxBodyTypeMale)
        {
            if (!TryGettingBackstory(out Backstory selectedBackstory, false)) return false;

            var box = comboBoxBodyTypeMale;
            var selectedItem = (BodyType)box.Items[box.SelectedIndex];

            selectedBackstory.bodyTypeMale = selectedItem;
            return true;
        }

        internal bool AdjustBodyTypeFemale(ComboBox comboBoxBodyTypeFemale)
        {
            if (!TryGettingBackstory(out Backstory selectedBackstory, false)) return false;

            var box = comboBoxBodyTypeFemale;
            var selectedItem = (BodyType)box.Items[box.SelectedIndex];

            selectedBackstory.bodyTypeFemale = selectedItem;
            return true;
        }

        internal void CloseFile()
        {
            if (LoadedBackstoryFile != null)
                LoadedBackstoryFile = null;
            formViewer.UpdateForm(UpdateEvent.CloseFile);
        }

        internal void SaveFile(string prefix)
        {
            if (File.Exists(LoadedBackstoryFile.FilePathFileName))
                LoadedBackstoryFile.Save(prefix);
            else
            {
                var result = MessageBox.Show("Error! File at " + LoadedBackstoryFile.FilePathFileName + " is missing. Create new file at this location?", "File Path Not Found", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                    LoadedBackstoryFile.Save(prefix);
            }
        }

        internal void TitleOptionsDialog()
        {
            TitleDialog result = new TitleDialog(LoadedBackstoryFile.SelectedBackstory);
            result.ShowDialog();
        }


        internal bool DeleteSkill(DataGridView dataGridViewSkills, DataGridViewCellEventArgs e)
        {
            if (!TryGettingBackstory(out Backstory selectedBackstory)) return false;

            // Retrieve Skill Name
            string skillDefNameToRemove = dataGridViewSkills[0, e.RowIndex].Value.ToString();
            MessageBox.Show(skillDefNameToRemove);

            var toRemove = selectedBackstory.skillGains.FirstOrDefault(x => x.defName.ToString() == skillDefNameToRemove);
            MessageBox.Show(toRemove.defName.ToString());

            if (toRemove != null)
                selectedBackstory.skillGains.Remove(toRemove);

            return true;
        }
        
        internal bool DeleteForcedTrait(DataGridView viewer, DataGridViewCellEventArgs e)
        {
            MessageBox.Show(e.ColumnIndex.ToString(), e.RowIndex.ToString());
            if (!TryGettingBackstory(out Backstory selectedBackstory)) return false;

            // Retrieve Skill Name
            var forcedTraitDefNameToRemove = ((TraitEntry)viewer[0, e.RowIndex].Value).defName.ToString();
            MessageBox.Show(forcedTraitDefNameToRemove);

            var toRemove = selectedBackstory.forcedTraits.FirstOrDefault(x => x.defName.ToString() == forcedTraitDefNameToRemove);
            MessageBox.Show(toRemove.defName.ToString());

            if (toRemove != null)
                selectedBackstory.forcedTraits.Remove(toRemove);

            return true;
        }

        internal bool DeleteDisallowedTrait(DataGridView viewer, DataGridViewCellEventArgs e)
        {
            if (!TryGettingBackstory(out Backstory selectedBackstory)) return false;

            // Retrieve Skill Name
            string disallowedTraitDefNameToRemove = ((TraitEntry)viewer[0, e.RowIndex].Value).defName.ToString();
            //MessageBox.Show(disallowedTraitDefNameToRemove);

            var toRemove = selectedBackstory.disallowedTraits.FirstOrDefault(x => x.defName.ToString() == disallowedTraitDefNameToRemove);
            //MessageBox.Show(toRemove.defName.ToString());

            if (toRemove != null)
                selectedBackstory.disallowedTraits.Remove(toRemove);

            return true;
        }

        internal bool ClearRequiredWorkTags(RichTextBox richTextBox)
        {
            if (!TryGettingBackstory(out Backstory selectedBackstory)) return false;
            
            selectedBackstory.requiredWorkTags.Clear();
            richTextBox.Text = "";
            return true;
        }

        internal bool ClearDisallowedWorkTypes(RichTextBox richTextBox)
        {
            if (!TryGettingBackstory(out Backstory selectedBackstory)) return false;
            
            selectedBackstory.workDisables.Clear();
            richTextBox.Text = "";
            return true;
        }

    }
}
