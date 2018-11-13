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
        CloseFile
    }

    public partial class FormViewer : Form
    {
        private FormController formController;

        public FormViewer()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            formController = new FormController(this);

            ShowFileControls(false);

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

                    if (BackstoryUtility.IsAlienRaceBackstory(fileName))
                    {
                        radioButtonAlienRace.Checked = true;
                    }
                    else
                    {
                        radioButtonVanilla.Checked = true;
                    }
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

        private void UpdateListBoxes()
        {
            var currentIndex = listBox1.SelectedIndex;
            MessageBox.Show(currentIndex.ToString());
            BindingList<string> defNames = new BindingList<string>();
            foreach (var def in formController.LoadedBackstoryFile.Backstories)
                defNames.Add(def.defName);
            listBox1.DataSource = defNames;
            
            ToggleAllControls(true);

            listBox1.SelectedIndex = currentIndex % defNames.ToArray().Length;
            ChangeDefIndex(listBox1.SelectedIndex);

            dataGridViewTraitsDisabled.DataSource = null;
            dataGridViewTraitsForced.DataSource = null;


            if (CurrentlyLoadedTraitEntries != null)
            {
                comboBoxTraitsForced.DataSource = new List<TraitEntry>(CurrentlyLoadedTraitEntries);
                comboBoxTraitsForced.DisplayMember = "label";
                comboBoxTraitsDisabled.DataSource = new List<TraitEntry>(CurrentlyLoadedTraitEntries);
                comboBoxTraitsDisabled.DisplayMember = "label";
            }

            if (LoadedBackstory != null)
            {
                dataGridViewTraitsDisabled.DataSource = LoadedBackstory.disallowedTraits;
                dataGridViewTraitsForced.DataSource = LoadedBackstory.forcedTraits;
            }

            Updating = false;
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

        private static bool Updating = false;
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        { 
            if (!Updating && !newFiling)
            {
                Updating = true;
                UpdateListBoxes(LoadedBackstoryFile);
                
            }
            if (newFiling)
                newFiling = false;
        }

        private void ChangeDefIndex(int index, bool forced = false)
        {
            if (index < 0) return;
            if (listBox1.Items.Count > 1)
            {
                if (!forced && index == LastLoadedIndex) return;
            }

            if (LoadedBackstory.originalDefName != LoadedBackstory.defName)
            {
                int nameCount = 0;

                //Check for accidental double defNames
                for (int i = 0; i < listBox1.Items.Count; i++)
                {
                    if (listBox1.Items[i].ToString() == LoadedBackstory.defName)
                        nameCount++;
                    if (nameCount > 1)
                    {
                        MessageBox.Show("Multiple defs with the same defName detected. DefNames must be unique.");
                        listBox1.SelectedIndex = LastLoadedIndex;
                        textBoxDefName.SelectAll();
                        return;
                    }
                }
                
                LoadedBackstory.originalDefName = LoadedBackstory.defName;
            }

            LastLoadedIndex = index;

            

            textBoxDefName.Text = LoadedBackstory.defName;
            textBoxTitle.Text = LoadedBackstory.title;
            richTextBoxDescription.Text = LoadedBackstory.baseDescription;
            radioButtonAdulthood.Checked = LoadedBackstory.slot == Slot.Adulthood;
            radioButtonChildhood.Checked = !radioButtonAdulthood.Checked;
            
            comboBoxBodyTypeGlobal.SelectedItem = LoadedBackstory.bodyTypeGlobal;
            comboBoxBodyTypeMale.SelectedItem = LoadedBackstory.bodyTypeMale;
            comboBoxBodyTypeFemale.SelectedItem = LoadedBackstory.bodyTypeFemale;
            

            UpdateAllDataViews();
        }

        private void UpdateAllDataViews()
        {
            GridViewUtility.UpdateView(dataGridViewSkills, LoadedBackstory.skillGains);
            GridViewUtility.UpdateView(dataGridViewTraitsForced, LoadedBackstory.forcedTraits, new int[] { 75, 25, 25 }, new List<string> { "label" });
            GridViewUtility.UpdateView(dataGridViewTraitsDisabled, LoadedBackstory.disallowedTraits, new int[] { 75, 25, 25 }, new List<string> { "label" });

            richTextBoxRequiredWorkTypes.Text = LoadedBackstory.requiredWorkTags.ToString();
            richTextBoxDisabledWorkTypes.Text = LoadedBackstory.workDisables.ToString();

            if (LoadedBackstory.spawnCategories != null && LoadedBackstory.spawnCategories.Count > 0)
                richTextBoxSpawnCategories.Text = string.Join(", ", LoadedBackstory.spawnCategories);
            else
                richTextBoxSpawnCategories.Text = "";
        }


        private void radioButtonChildhood_CheckedChanged(object sender, EventArgs e)
        {
            radioButtonAdulthood.Checked = !radioButtonChildhood.Checked;
            LoadedBackstory.slot = Slot.Childhood;
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

        private void buttonSave_Click(object sender, EventArgs e)
        {
            if (LoadedBackstoryFile == null) return;

            if (listBox1.SelectedIndex < 0) return;
            Backstory curBackstory = LoadedBackstory;

            curBackstory.defName = textBoxDefName.Text;
            curBackstory.title = textBoxTitle.Text;
            curBackstory.baseDescription = richTextBoxDescription.Text;
            curBackstory.slot = radioButtonAdulthood.Checked ? Slot.Adulthood : Slot.Childhood;

            if (!Updating && !newFiling)
            {
                Updating = true;
                UpdateListBoxes(LoadedBackstoryFile);

            }

        }

        private void buttonAddSkill_Click(object sender, EventArgs e)
        {
            Enum.TryParse<SkillDef>(comboBoxSkills.Items[comboBoxSkills.SelectedIndex].ToString(), out var skill);
            var newSkill = new SkillGain() { defName = skill, amount = 1 };

            GridViewUtility.AddRow(dataGridViewSkills, LoadedBackstory.skillGains, newSkill, e);
            var curIndex = listBox1.SelectedIndex;
            ChangeDefIndex(curIndex, true);

        }

        private void buttonOpenFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog();
        }

        private void buttonCreateFile_Click(object sender, EventArgs e)
        {
            SaveFileDialog();
            
        }
        
        private void buttonAddDefName_Click(object sender, EventArgs e)
        {
            string newDefName = textBoxAddDefName.Text;
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


                if (!Updating && !newFiling)
                {
                    Updating = true;
                    UpdateListBoxes(LoadedBackstoryFile);

                }
            }
        }

        private void buttonTraitsForcedAdd_Click(object sender, EventArgs e)
        {
            var selectedItem = (TraitEntry)comboBoxTraitsForced.Items[comboBoxTraitsForced.SelectedIndex];
            
            GridViewUtility.AddRow(dataGridViewTraitsForced, LoadedBackstory.forcedTraits, selectedItem, e);
            var curIndex = listBox1.SelectedIndex;
            ChangeDefIndex(curIndex, true);

        }

        private void buttonTraitsDisabledAdd_Click(object sender, EventArgs e)
        {

            var selectedItem = (TraitEntry)comboBoxTraitsDisabled.Items[comboBoxTraitsDisabled.SelectedIndex];

            GridViewUtility.AddRow(dataGridViewTraitsDisabled, LoadedBackstory.disallowedTraits, selectedItem, e);
            var curIndex = listBox1.SelectedIndex;
            ChangeDefIndex(curIndex, true);

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

        private void radioButtonAdulthood_CheckedChanged(object sender, EventArgs e)
        {
            radioButtonChildhood.Checked = !radioButtonAdulthood.Checked;
            LoadedBackstory.slot = Slot.Adulthood;
        }
    }
}
