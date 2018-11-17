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
        NewDef
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

            comboBoxBodyTypeGlobal.DataSource = Enum.GetValues(typeof(BodyType));
            comboBoxBodyTypeMale.DataSource = Enum.GetValues(typeof(BodyType));
            comboBoxBodyTypeFemale.DataSource = Enum.GetValues(typeof(BodyType));
            comboBoxRequiredWorkTypes.DataSource = Enum.GetValues(typeof(WorkTags));
            comboBoxWorkTypesDisabled.DataSource = Enum.GetValues(typeof(WorkTags));
            comboBoxSkills.DataSource = Enum.GetValues(typeof(SkillDef));
        }

        public void UpdateForm(UpdateEvent eventType)
        {
            switch (eventType)
            {
                case UpdateEvent.LoadFile:
                    ShowFileControls(true);
                    UpdateListBoxes(formController);
                    radioButtonAlienRace.Checked = formController.LoadedBackstoryFile.IsAlienRaceBackstory;
                    ToggleAllControls(true);
                    break;
                case UpdateEvent.NewDef:
                    UpdateListBoxes(formController);
                    break;
                case UpdateEvent.CloseFile:
                    ShowFileControls(false);
                    ToggleAllControls(false);
                    break;
            }
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

        private void UpdateListBoxes(FormController formController)
        {
            var currentIndex = listBox1.SelectedIndex;
            BindingList<string> defNames = new BindingList<string>();
            foreach (var def in formController.LoadedBackstoryFile.Backstories)
                defNames.Add(def.defName);
            listBox1.DataSource = defNames;

            //listBox1.SelectedIndex = currentIndex % defNames.ToArray().Length;
            TryUpdatingBackstoryListBoxIndex(listBox1.SelectedIndex);

            if (!TryInitializingTraitsComboboxes(formController))
            {
                throw new Exception("Failed to initialize trait comboboxes.");
            }
            if (!TryInitializingTraitsDataGridViews(formController))
            {
                throw new Exception("Failed to initialize trait data grid views.");
            }

            currentState = State.Viewing;
        }

        private bool TryInitializingTraitsDataGridViews(FormController formController)
        {
            if (formController.LoadedBackstoryFile.SelectedBackstory is Backstory backstory)
            {
                dataGridViewTraitsDisabled.DataSource = backstory.disallowedTraits;
                dataGridViewTraitsForced.DataSource = backstory.forcedTraits;
                return true;
            }
            return false;
        }

        private bool TryInitializingTraitsComboboxes(FormController formController)
        {
            if (formController.CurrentlyLoadedTraitEntries is List<TraitEntry> entries)
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
            dataGridViewTraitsDisabled.DataSource = null;
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
            dataGridViewTraitsDisabled.Enabled = enabled;
            dataGridViewTraitsForced.Enabled = enabled;
        }
        
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        { 
            if (currentState != State.Updating)
            {
                currentState = State.Updating;
                UpdateListBoxes(formController);
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
            GridViewUtility.UpdateView(dataGridViewSkills, bs?.skillGains);
            GridViewUtility.UpdateView(dataGridViewTraitsForced, bs?.forcedTraits, 
                bs != null ? new int[] { 75, 25, 25 } : null,
                bs != null ? new List<string> { "label" } : null);
            GridViewUtility.UpdateView(dataGridViewTraitsDisabled, bs?.disallowedTraits, 
                bs != null ? new int[] { 75, 25, 25 } : null,
                bs != null ? new List<string> { "label" } : null);

            richTextBoxRequiredWorkTypes.Text = bs?.requiredWorkTags.ToString() ?? "";
            richTextBoxDisabledWorkTypes.Text = bs?.workDisables.ToString() ?? "";


            richTextBoxSpawnCategories.Text = 
                bs?.spawnCategories?.Count > 0 ?
                    richTextBoxSpawnCategories.Text = string.Join(", ", bs.spawnCategories) 
                    :
                    "";
        }


        private void radioButtonAdulthood_CheckedChanged(object sender, EventArgs e)
        {
            radioButtonAdulthood.Checked = formController.Notify_RadioButtonAdulthood();
            radioButtonChildhood.Checked = !radioButtonAdulthood.Checked;
        }

        private void radioButtonChildhood_CheckedChanged(object sender, EventArgs e)
        {
            radioButtonChildhood.Checked = formController.Notify_RadioButtonChildhood();
            radioButtonAdulthood.Checked = !radioButtonChildhood.Checked;
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
            //if (GetSelectedIndex() >= 0 && e.ColumnIndex == 2)
            //{
                //GridViewUtility.DeleteRow(dataGridViewSkills, LoadedBackstory.skillGains, e);
                //var curIndex = listBox1.SelectedIndex;
                //ChangeDefIndex(curIndex, true);
            //}
        }

        private int GetSelectedIndex()
        {
            return listBox1.SelectedIndex;
        }

        //private void buttonSave_Click(object sender, EventArgs e)
        //{
        //    if (LoadedBackstoryFile == null) return;

        //    if (listBox1.SelectedIndex < 0) return;
        //    Backstory curBackstory = LoadedBackstory;

        //    curBackstory.defName = textBoxDefName.Text;
        //    curBackstory.title = textBoxTitle.Text;
        //    curBackstory.baseDescription = richTextBoxDescription.Text;
        //    curBackstory.slot = radioButtonAdulthood.Checked ? Slot.Adulthood : Slot.Childhood;

        //    if (!Updating && !newFiling)
        //    {
        //        Updating = true;
        //        UpdateListBoxes(LoadedBackstoryFile);

        //    }

        //}

        private void buttonAddSkill_Click(object sender, EventArgs e)
        {
            if (!formController.TryAddSkillGain(comboBoxSkills))
            {
                MessageBox.Show("Failed to add new skill gain setting. Perhaps skill already exists?");
            }

        }

        private void buttonOpenFile_Click(object sender, EventArgs e)
        {
            formController.OpenBackstoryFileDialog();
        }

        private void buttonCreateFile_Click(object sender, EventArgs e)
        {
            formController.SaveBackstoryDialog(radioButtonAlienRace.Checked ? BackstoryUtility.ErdsPrefix : BackstoryUtility.JecsPrefix, true);
            
        }
        
        private void buttonAddDefName_Click(object sender, EventArgs e)
        {
            formController.AddNewBackstoryDef(textBoxAddDefName);
        }

        private void buttonTraitsForcedAdd_Click(object sender, EventArgs e)
        {
            formController.TryAddForcedTrait(comboBoxTraitsForced);
        }

        private void buttonTraitsDisabledAdd_Click(object sender, EventArgs e)
        {
            formController.TryAddDisallowedTrait(comboBoxTraitsDisabled);
        }

        private void buttonRequiredWorkTypes_Click(object sender, EventArgs e)
        {

            if (LoadedBackstoryFile == null) return;
            var box = comboBoxRequiredWorkTypes;
            var selectedItem = (WorkTags)box.SelectedItem;

            LoadedBackstory.requiredWorkTags = LoadedBackstory.requiredWorkTags | selectedItem;
            UpdateAllDataViews();
        }

        private void buttonClearRequiredWorkTags_Click(object sender, EventArgs e)
        {
            LoadedBackstory.requiredWorkTags = WorkTags.None;
            UpdateAllDataViews();
        }

        private void buttonClearDisallowedWorkType_Click(object sender, EventArgs e)
        {
            LoadedBackstory.workDisables = WorkTags.None;
            UpdateAllDataViews();
        }

        private void buttonAddWorkTypesDisabled_Click(object sender, EventArgs e)
        {

            var box = comboBoxWorkTypesDisabled;
            var selectedItem = (WorkTags)box.SelectedItem;

            LoadedBackstory.workDisables = LoadedBackstory.workDisables | selectedItem;
            UpdateAllDataViews();
        }


        private void textBoxDefName_TextChanged(object sender, EventArgs e)
        {

            if (LoadedBackstoryFile == null) return;
            LoadedBackstory.defName = textBoxDefName.Text;
        }

        private void textBoxTitle_TextChanged(object sender, EventArgs e)
        {

            if (LoadedBackstoryFile == null) return;
            LoadedBackstory.title = textBoxTitle.Text;
        }

        private void richTextBoxDescription_TextChanged(object sender, EventArgs e)
        {

            if (LoadedBackstoryFile == null) return;
            LoadedBackstory.baseDescription = richTextBoxDescription.Text;
        }
        private void richTextBoxSpawnCategories_TextChanged(object sender, EventArgs e)
        {

            if (LoadedBackstoryFile == null) return;
            if (richTextBoxSpawnCategories.Text == "") return;

            // This is a comma seperated value list. Warn the user.
            if (richTextBoxSpawnCategories.Text.FirstOrDefault(x => x == ' ') != default(char) && 
                !richTextBoxSpawnCategories.Text.Contains(','))
            {
                MessageBox.Show("Use commas to seperate spawn categories.\n\tE.g. RangedShooting, Tribal, Giant");
                richTextBoxSpawnCategories.Text = richTextBoxSpawnCategories.Text.Replace(' ', ',');
            }

            
            LoadedBackstory.spawnCategories = new BindingList<string>(richTextBoxSpawnCategories.Text.Replace(" ", "").Split(','));
        }
        



        private void comboBoxBodyTypeGlobal_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (LoadedBackstoryFile == null) return;

            var box = comboBoxBodyTypeGlobal;
            var selectedItem = (BodyType)box.Items[box.SelectedIndex];

            LoadedBackstory.bodyTypeGlobal = selectedItem;

        }

        private void comboBoxBodyTypeMale_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (LoadedBackstoryFile == null) return;
            var box = comboBoxBodyTypeMale;
            var selectedItem = (BodyType)box.Items[box.SelectedIndex];

            LoadedBackstory.bodyTypeMale = selectedItem;
        }

        private void comboBoxBodyTypeFemale_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (LoadedBackstoryFile == null) return;
            var box = comboBoxBodyTypeFemale;
            var selectedItem = (BodyType)box.Items[box.SelectedIndex];

            LoadedBackstory.bodyTypeFemale = selectedItem;
        }
    }
}
