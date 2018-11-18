using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace Backstory_Generator
{
    public enum UpdateEvent
    {
        LoadFile,
        NewFile,
        CloseFile,
        NewDef,
        SelectDef,
        DeselectDef,
        CreateFile
    }

    public partial class FormViewer : Form
    {
        public enum State
        {
            Viewing,
            Updating
        };

        private FormController formController;
        private State currentState;

        public FormViewer()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            formController = new FormController(this);
            currentState = State.Viewing;

            ShowFileControls(false);
            ToggleAllControls(false);
        }

        public void UpdateForm(UpdateEvent eventType)
        {
            switch (eventType)
            {

                case UpdateEvent.CreateFile:
                case UpdateEvent.LoadFile:
                    ShowFileControls(true);
                    UpdateComboboxesAndDataViews(formController);
                    radioButtonAlienRace.Checked = formController.LoadedBackstoryFile.IsAlienRaceBackstory;
                    ToggleAllControls(false);
                    UpdateListBox(formController);
                    InitializeComboBoxes();
                    break;
                case UpdateEvent.NewDef:
                    UpdateListBox(formController);
                    UpdateComboboxesAndDataViews(formController);
                    break;
                case UpdateEvent.SelectDef:
                    UpdateComboboxesAndDataViews(formController);
                    ToggleAllControls(true);
                    break;
                case UpdateEvent.DeselectDef:
                    UpdateComboboxesAndDataViews(formController);
                    ToggleAllControls(false);
                    break;
                case UpdateEvent.CloseFile:
                    ShowFileControls(false);
                    ToggleAllControls(false);
                    listBox1.DataSource = null;
                    break;
            }
        }

        private void InitializeComboBoxes()
        {
            comboBoxBodyTypeGlobal.DataSource = Enum.GetValues(typeof(BodyType));
            comboBoxBodyTypeMale.DataSource = Enum.GetValues(typeof(BodyType));
            comboBoxBodyTypeFemale.DataSource = Enum.GetValues(typeof(BodyType));
            comboBoxRequiredWorkTypes.DataSource = Enum.GetValues(typeof(WorkTags));
            comboBoxWorkTypesDisabled.DataSource = Enum.GetValues(typeof(WorkTags));
            comboBoxSkills.DataSource = Enum.GetValues(typeof(SkillDef));
        }

        private void ShowFileControls(bool enable)
        {
            foreach (Control control in groupBox1.Controls)
                control.Visible = enable;
            saveToolStripMenuItem.Enabled = enable;
            saveAsToolStripMenuItem.Enabled = enable;
            closeToolStripMenuItem.Enabled = enable;
            buttonOpenFile.Visible = !enable;
            buttonCreateFile.Visible = !enable;
            radioButtonAlienRace.Enabled = enable;
            radioButtonVanilla.Enabled = enable;
            buttonAddDefName.Enabled = enable;
        }

        private void UpdateListBox(FormController formController)
        {
            BindingList<string> defNames = new BindingList<string>();
            foreach (var def in formController.LoadedBackstoryFile.Backstories)
                defNames.Add(def.defName);
            listBox1.DataSource = defNames;
        }

        private void UpdateComboboxesAndDataViews(FormController formController)
        {
            var currentIndex = listBox1.SelectedIndex;
            
            //var realIndex = currentIndex % defNames.ToArray().Length;
            if (listBox1.SelectedIndex >= 0)
            {
                TryUpdatingBackstoryListBoxIndex(listBox1.SelectedIndex);

                if (!TryInitializingTraitsComboboxes(formController))
                {
                    throw new Exception("Failed to initialize trait comboboxes.");
                }
                if (!TryInitializingTraitsDataGridViews(formController))
                {
                    MessageBox.Show("Failed to initialize trait data grid views." + "\n\n" + Environment.StackTrace.ToString());
                }
            }

            currentState = State.Viewing;
        }

        private bool TryInitializingTraitsDataGridViews(FormController formController)
        {
            if (formController?.LoadedBackstoryFile?.SelectedBackstory is Backstory backstory)
            {
                dataGridViewTraitsDisallowed.DataSource = backstory.disallowedTraits;
                dataGridViewTraitsForced.DataSource = backstory.forcedTraits;
                return true;
            }
            return false;
        }

        private bool TryInitializingTraitsComboboxes(FormController formController)
        {
            if (formController?.LoadedTraitEntryFile?.entries is List<TraitEntry> entries)
            {
                comboBoxTraitsForced.DataSource = new List<TraitEntry>(entries);
                comboBoxTraitsForced.DisplayMember = "label";
                comboBoxTraitsDisabled.DataSource = new List<TraitEntry>(entries);
                comboBoxTraitsDisabled.DisplayMember = "label";
                return true;
            }
            return false;
        }

        private void DisableAndClearAllControls()
        {
            ToggleAllControls(false);

            ClearAllControls();
        }

        private void ClearAllControls()
        {
            //Clear all

            //Clear text boxes
            textBoxDefName.Text = "";
            textBoxTitle.Text = "";
            richTextBoxDescription.Text = "";
            richTextBoxSpawnCategories.Text = "";

            //Clear backstory type
            radioButtonAdulthood.Checked = false;
            radioButtonChildhood.Checked = false;

            //Clear body type options
            //comboBoxBodyTypeGlobal.SelectedValue = BodyType.Any;
            //comboBoxBodyTypeMale.SelectedValue = BodyType.Any;
            //comboBoxBodyTypeFemale.SelectedValue = BodyType.Any;

            //Clear work types
            richTextBoxDisabledWorkTypes.Text = "";
            richTextBoxRequiredWorkTypes.Text = "";

            //Clear skills
            dataGridViewSkills.DataSource = null;

            //Clear traits
            dataGridViewTraitsDisallowed.DataSource = null;
            dataGridViewTraitsForced.DataSource = null;
        }

        private void ToggleAllControls(bool enabled = false)
        {
            //Disable all

            //Disable text boxes
            textBoxDefName.Enabled = enabled;
            textBoxTitle.Enabled = enabled;
            richTextBoxDescription.Enabled = enabled;
            richTextBoxSpawnCategories.Enabled = enabled;

            //Disable backstory type
            radioButtonAdulthood.Enabled = enabled;
            radioButtonChildhood.Enabled = enabled;

            //Disable body type options
            comboBoxBodyTypeGlobal.Enabled = enabled;
            comboBoxBodyTypeMale.Enabled = enabled;
            comboBoxBodyTypeFemale.Enabled = enabled;

            //Disable work types
            buttonAddWorkTypesDisabled.Enabled = enabled;
            buttonClearDisallowedWorkType.Enabled = enabled;
            buttonRequiredWorkTypes.Enabled = enabled;
            buttonClearRequiredWorkTags.Enabled = enabled;

            //Disable skill
            dataGridViewSkills.Enabled = enabled;

            //Disable traits
            dataGridViewTraitsDisallowed.Enabled = enabled;
            dataGridViewTraitsForced.Enabled = enabled;
        }
        
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //MessageBox.Show(listBox1.SelectedIndex.ToString() + "\n\n" + Environment.StackTrace.ToString());
            if (currentState != State.Updating)
            {
                currentState = State.Updating;
                formController.SelectNewIndex(listBox1.SelectedIndex);
            }
        }

        public void TryUpdatingBackstoryListBoxIndex(int index, bool forced = false)
        {
            var curBackstory = formController.LoadedBackstoryFile.SelectedBackstory;

            textBoxDefName.Text = curBackstory?.defName ?? "";
            textBoxTitle.Text = curBackstory?.title ?? "";
            richTextBoxDescription.Text = curBackstory?.baseDescription ?? "";
            radioButtonAdulthood.Checked = curBackstory?.slot == Slot.Adulthood;
            radioButtonChildhood.Checked = !radioButtonAdulthood.Checked;

            comboBoxBodyTypeGlobal.SelectedItem = curBackstory?.bodyTypeGlobal ?? BodyType.Any;
            comboBoxBodyTypeMale.SelectedItem = curBackstory?.bodyTypeMale ?? BodyType.Any;
            comboBoxBodyTypeFemale.SelectedItem = curBackstory?.bodyTypeFemale ?? BodyType.Any;
            
            UpdateAllDataViews(curBackstory);
        }

        private void UpdateAllDataViews(Backstory bs)
        {
            GridViewUtility.UpdateView(dataGridViewSkills, bs?.skillGains,
                bs != null ? new int[] { 125, 25, 25 } : null);
            GridViewUtility.UpdateView(dataGridViewTraitsForced, bs?.forcedTraits, 
                bs != null ? new int[] { 135, 25 } : null,
                bs != null ? new List<string> { "defName", "degree" } : null);
            GridViewUtility.UpdateView(dataGridViewTraitsDisallowed, bs?.disallowedTraits, 
                bs != null ? new int[] { 135, 25 } : null,
                bs != null ? new List<string> { "defName", "degree" } : null);
            
            richTextBoxRequiredWorkTypes.Text = bs?.requiredWorkTags?.FirstOrDefault() == null ? "" : string.Join(", ", bs.requiredWorkTags); //bs?.requiredWorkTags.ToString() ?? "";
            richTextBoxDisabledWorkTypes.Text = bs?.workDisables?.FirstOrDefault() == null ? "" : string.Join(", ", bs.workDisables);//bs?.workDisables.ToString() ?? "";


            richTextBoxSpawnCategories.Text = 
                bs?.spawnCategories?.Count > 0 ?
                    richTextBoxSpawnCategories.Text = string.Join(", ", bs.spawnCategories) 
                    :
                    "";
        }


        private void radioButtonAdulthood_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonAdulthood.Checked)
            {
                formController.Notify_RadioButtonAdulthood();
                radioButtonChildhood.Checked = false;
            }
        }

        private void radioButtonChildhood_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonChildhood.Checked)
            {
                formController.Notify_RadioButtonChildhood();
                radioButtonAdulthood.Checked = false;
            }
        }

        private void radioButtonVanilla_CheckedChanged(object sender, EventArgs e)
        {
            radioButtonAlienRace.Checked = !radioButtonVanilla.Checked;
        }

        private void radioButtonAlienRace_CheckedChanged(object sender, EventArgs e)
        {
            radioButtonVanilla.Checked = !radioButtonAlienRace.Checked;
        }

        private void dataGridViewSkills_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Ignore clicks that are not on button cells. 
            if (e.RowIndex < 0 || e.ColumnIndex !=
                dataGridViewSkills.Columns["dataGridViewDeleteButton"].Index) return;

            formController.DeleteSkill(dataGridViewSkills,e);

        }

        private int GetSelectedIndex()
        {
            return listBox1.SelectedIndex;
        }

        private void buttonAddSkill_Click(object sender, EventArgs e) => formController.TryAddSkillGain(comboBoxSkills, dataGridViewSkills);

        private void buttonOpenFile_Click(object sender, EventArgs e) => formController.OpenBackstoryFileDialog();

        private void buttonCreateFile_Click(object sender, EventArgs e) => formController.SaveBackstoryDialog(radioButtonAlienRace.Checked, true);            
        
        private void buttonAddDefName_Click(object sender, EventArgs e) => formController.TryAddNewBackstoryDef(textBoxAddDefName);

        private void buttonTraitsForcedAdd_Click(object sender, EventArgs e) => formController.TryAddForcedTrait(comboBoxTraitsForced, dataGridViewTraitsForced);

        private void buttonTraitsDisabledAdd_Click(object sender, EventArgs e) => formController.TryAddDisallowedTrait(comboBoxTraitsDisabled, dataGridViewTraitsDisallowed);
        
        private void buttonRequiredWorkTypes_Click(object sender, EventArgs e) => formController.TryAddRequiredWorkType(comboBoxRequiredWorkTypes, richTextBoxRequiredWorkTypes);
        
        private void buttonAddWorkTypesDisabled_Click(object sender, EventArgs e) => formController.TryAddDisabledWorkType(comboBoxWorkTypesDisabled, richTextBoxDisabledWorkTypes);

        private void buttonClearRequiredWorkTags_Click(object sender, EventArgs e)
        {
            formController.ClearRequiredWorkTags(richTextBoxRequiredWorkTypes);
        }

        private void buttonClearDisallowedWorkType_Click(object sender, EventArgs e)
        {
            formController.ClearDisallowedWorkTypes(richTextBoxDisabledWorkTypes);
        }


        private void textBoxDefName_TextChanged(object sender, EventArgs e)
        {
            formController.AdjustDefName(textBoxDefName);
        }

        private void textBoxTitle_TextChanged(object sender, EventArgs e)
        {
            formController.AdjustTitle(textBoxTitle);
        }

        private void richTextBoxDescription_TextChanged(object sender, EventArgs e)
        {
            formController.AdjustDescription(richTextBoxDescription);
        }
        private void richTextBoxSpawnCategories_TextChanged(object sender, EventArgs e)
        {
            formController.AdjustSpawnCategories(richTextBoxSpawnCategories);
        }
        
        private void comboBoxBodyTypeGlobal_SelectedIndexChanged(object sender, EventArgs e)
        {
            formController.AdjustBodyTypeGlobal(comboBoxBodyTypeGlobal);
        }

        private void comboBoxBodyTypeMale_SelectedIndexChanged(object sender, EventArgs e)
        {
            formController.AdjustBodyTypeMale(comboBoxBodyTypeMale);
        }

        private void comboBoxBodyTypeFemale_SelectedIndexChanged(object sender, EventArgs e)
        {
            formController.AdjustBodyTypeFemale(comboBoxBodyTypeFemale);
        }


        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            formController.SaveBackstoryDialog();
        }


        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
             formController.OpenBackstoryFileDialog();
        }


        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SettingsDialog settings = new SettingsDialog();
            settings.ShowDialog();
        }


        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            formController.CloseFile();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            formController.SaveFile(radioButtonAlienRace.Checked ? BackstoryUtility.ErdsPrefix : BackstoryUtility.JecsPrefix);

        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            formController.SaveBackstoryDialog();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();

        }

        private void buttonTitleOptions_Click(object sender, EventArgs e)
        {
            formController.TitleOptionsDialog();
        }

        private void dataGridViewTraitsForced_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            var viewer = dataGridViewTraitsForced;

            // Ignore clicks that are not on button cells. 
            if (e.RowIndex < 0 || e.ColumnIndex !=
                viewer.Columns["dataGridViewDeleteButton"].Index) return;

            formController.DeleteForcedTrait(viewer, e);

        }

        private void dataGridViewTraitsDisabled_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            var viewer = dataGridViewTraitsDisallowed;

            // Ignore clicks that are not on button cells. 
            if (e.RowIndex < 0 || e.ColumnIndex !=
                viewer.Columns["dataGridViewDeleteButton"].Index) return;

            formController.DeleteDisallowedTrait(viewer, e);
        }
    }
}
