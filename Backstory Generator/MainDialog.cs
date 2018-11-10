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
    public partial class MainDialog : Form
    {
        public string CurrentlyLoadedFile { get; set; }
        public Defs CurrentlyLoadedDefs { get; set; }
        public Backstory CurrentlyLoadedBackstory
        {
            get
            {
                if (CurrentlyLoadedDefs == null) return null;
                var curIndex = 0;
                if (listBox1.SelectedIndex > -1)
                    curIndex = listBox1.SelectedIndex;
                return CurrentlyLoadedDefs.Backstories.ElementAt(curIndex);
            }
        }
        public int LastLoadedIndex { get; set; }
        public List<TraitEntry> CurrentlyLoadedTraitEntries { get; set; }


        public MainDialog()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ShowFileControls(false);

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

        private void UpdateListBoxes(Defs defs)
        {
            if (defs.Backstories == null || defs.Backstories.Count == 0)
                listBox1.DataSource = null;

            var currentIndex = listBox1.SelectedIndex;
            BindingList<string> defNames = new BindingList<string>();
            foreach (var def in defs.Backstories)
                defNames.Add(def.defName);
            listBox1.DataSource = defNames;
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

            if (CurrentlyLoadedBackstory != null)
            {
                dataGridViewTraitsDisabled.DataSource = CurrentlyLoadedBackstory.disallowedTraits;
                dataGridViewTraitsForced.DataSource = CurrentlyLoadedBackstory.forcedTraits;
            }

            Updating = false;
        }


        private static bool Updating = false;
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        { 
            if (!Updating)
            {
                Updating = true;
                UpdateListBoxes(CurrentlyLoadedDefs);
            }
        }

        private void ChangeDefIndex(int index, bool forced = false)
        {
            if (index < 0) return;
            if (!forced && index == LastLoadedIndex) return;

            if (CurrentlyLoadedBackstory.originalDefName != CurrentlyLoadedBackstory.defName)
            {
                int nameCount = 0;

                //Check for accidental double defNames
                for (int i = 0; i < listBox1.Items.Count; i++)
                {
                    if (listBox1.Items[i].ToString() == CurrentlyLoadedBackstory.defName)
                        nameCount++;
                    if (nameCount > 1)
                    {
                        MessageBox.Show("Multiple defs with the same defName detected. DefNames must be unique.");
                        listBox1.SelectedIndex = LastLoadedIndex;
                        textBoxDefName.SelectAll();
                        return;
                    }
                }
                
                CurrentlyLoadedBackstory.originalDefName = CurrentlyLoadedBackstory.defName;
            }

            LastLoadedIndex = index;


            Backstory curBackstory = CurrentlyLoadedBackstory;

            textBoxDefName.Text = curBackstory.defName;
            textBoxTitle.Text = curBackstory.title;
            richTextBoxDescription.Text = curBackstory.baseDescription;
            radioButtonAdulthood.Checked = curBackstory.slot == Slot.Adulthood;
            radioButtonChildhood.Checked = !radioButtonAdulthood.Checked;
            
            comboBoxBodyTypeGlobal.SelectedItem = CurrentlyLoadedBackstory.bodyTypeGlobal;
            comboBoxBodyTypeMale.SelectedItem = CurrentlyLoadedBackstory.bodyTypeMale;
            comboBoxBodyTypeFemale.SelectedItem = CurrentlyLoadedBackstory.bodyTypeFemale;

            UpdateAllDataViews(curBackstory);
        }

        private void UpdateAllDataViews(Backstory curBackstory)
        {
            GridViewUtility.UpdateView(dataGridViewSkills, curBackstory.skillGains);
            GridViewUtility.UpdateView(dataGridViewTraitsForced, curBackstory.forcedTraits, new int[] { 75, 25, 25 }, new List<string> { "label" });
            GridViewUtility.UpdateView(dataGridViewTraitsDisabled, curBackstory.disallowedTraits, new int[] { 75, 25, 25 }, new List<string> { "label" });

            richTextBoxRequiredWorkTypes.Text = curBackstory.requiredWorkTags.ToString();
            richTextBoxDisabledWorkTypes.Text = curBackstory.workDisables.ToString();

            if (curBackstory.spawnCategories != null && curBackstory.spawnCategories.Count > 0)
                richTextBoxSpawnCategories.Text = string.Join(", ", curBackstory.spawnCategories);
            else
                richTextBoxSpawnCategories.Text = "";
        }


        private void radioButtonChildhood_CheckedChanged(object sender, EventArgs e)
        {
            radioButtonAdulthood.Checked = !radioButtonChildhood.Checked;
            CurrentlyLoadedBackstory.slot = Slot.Childhood;
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
                //GridViewUtility.DeleteRow(dataGridViewSkills, CurrentlyLoadedBackstory.skillGains, e);
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
            if (CurrentlyLoadedDefs == null) return;

            if (listBox1.SelectedIndex < 0) return;
            Backstory curBackstory = CurrentlyLoadedBackstory;

            curBackstory.defName = textBoxDefName.Text;
            curBackstory.title = textBoxTitle.Text;
            curBackstory.baseDescription = richTextBoxDescription.Text;
            curBackstory.slot = radioButtonAdulthood.Checked ? Slot.Adulthood : Slot.Childhood;

            UpdateListBoxes(CurrentlyLoadedDefs);

        }

        private void buttonAddSkill_Click(object sender, EventArgs e)
        {
            Enum.TryParse<SkillDef>(comboBoxSkills.Items[comboBoxSkills.SelectedIndex].ToString(), out var skill);
            var newSkill = new SkillGain() { defName = skill, amount = 1 };

            GridViewUtility.AddRow(dataGridViewSkills, CurrentlyLoadedBackstory.skillGains, newSkill, e);
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
                if (CurrentlyLoadedDefs.Backstories.Any(x => x.defName == newDefName))
                {
                    MessageBox.Show("Backstory with same defName already exists. Try another defName.");
                    return;
                }
                var newBackstory = new Backstory { originalDefName = newDefName, defName = newDefName };
                CurrentlyLoadedDefs.Backstories.Add(newBackstory);
                MessageBox.Show("Added Backstory: " + newDefName);
                textBoxAddDefName.Text = "";
                UpdateListBoxes(CurrentlyLoadedDefs);
            }
        }

        private void buttonTraitsForcedAdd_Click(object sender, EventArgs e)
        {
            var selectedItem = (TraitEntry)comboBoxTraitsForced.Items[comboBoxTraitsForced.SelectedIndex];
            
            GridViewUtility.AddRow(dataGridViewTraitsForced, CurrentlyLoadedBackstory.forcedTraits, selectedItem, e);
            var curIndex = listBox1.SelectedIndex;
            ChangeDefIndex(curIndex, true);

        }

        private void buttonTraitsDisabledAdd_Click(object sender, EventArgs e)
        {

            var selectedItem = (TraitEntry)comboBoxTraitsDisabled.Items[comboBoxTraitsDisabled.SelectedIndex];

            GridViewUtility.AddRow(dataGridViewTraitsDisabled, CurrentlyLoadedBackstory.disallowedTraits, selectedItem, e);
            var curIndex = listBox1.SelectedIndex;
            ChangeDefIndex(curIndex, true);

        }

        private void buttonRequiredWorkTypes_Click(object sender, EventArgs e)
        {

            if (CurrentlyLoadedDefs == null) return;
            var box = comboBoxRequiredWorkTypes;
            var selectedItem = (WorkTags)box.SelectedItem;

            CurrentlyLoadedBackstory.requiredWorkTags = CurrentlyLoadedBackstory.requiredWorkTags | selectedItem;
            UpdateAllDataViews(CurrentlyLoadedBackstory);
        }

        private void buttonClearRequiredWorkTags_Click(object sender, EventArgs e)
        {
            CurrentlyLoadedBackstory.requiredWorkTags = WorkTags.None;
            UpdateAllDataViews(CurrentlyLoadedBackstory);
        }

        private void buttonClearDisallowedWorkType_Click(object sender, EventArgs e)
        {
            CurrentlyLoadedBackstory.workDisables = WorkTags.None;
            UpdateAllDataViews(CurrentlyLoadedBackstory);
        }

        private void buttonAddWorkTypesDisabled_Click(object sender, EventArgs e)
        {

            var box = comboBoxWorkTypesDisabled;
            var selectedItem = (WorkTags)box.SelectedItem;

            CurrentlyLoadedBackstory.workDisables = CurrentlyLoadedBackstory.workDisables | selectedItem;
            UpdateAllDataViews(CurrentlyLoadedBackstory);
        }


        private void textBoxDefName_TextChanged(object sender, EventArgs e)
        {

            if (CurrentlyLoadedDefs == null) return;
            CurrentlyLoadedBackstory.defName = textBoxDefName.Text;
        }

        private void textBoxTitle_TextChanged(object sender, EventArgs e)
        {

            if (CurrentlyLoadedDefs == null) return;
            CurrentlyLoadedBackstory.title = textBoxTitle.Text;
        }

        private void richTextBoxDescription_TextChanged(object sender, EventArgs e)
        {

            if (CurrentlyLoadedDefs == null) return;
            CurrentlyLoadedBackstory.baseDescription = richTextBoxDescription.Text;
        }
        private void richTextBoxSpawnCategories_TextChanged(object sender, EventArgs e)
        {

            if (CurrentlyLoadedDefs == null) return;
            if (richTextBoxSpawnCategories.Text == "") return;

            // This is a comma seperated value list. Warn the user.
            if (richTextBoxSpawnCategories.Text.FirstOrDefault(x => x == ' ') != default(char) && 
                !richTextBoxSpawnCategories.Text.Contains(','))
            {
                MessageBox.Show("Use commas to seperate spawn categories.\n\tE.g. RangedShooting, Tribal, Giant");
                richTextBoxSpawnCategories.Text = richTextBoxSpawnCategories.Text.Replace(' ', ',');
            }

            
            CurrentlyLoadedBackstory.spawnCategories = new BindingList<string>(richTextBoxSpawnCategories.Text.Replace(" ", "").Split(','));
        }
        



        private void comboBoxBodyTypeGlobal_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CurrentlyLoadedDefs == null) return;

            var box = comboBoxBodyTypeGlobal;
            var selectedItem = (BodyType)box.Items[box.SelectedIndex];

            CurrentlyLoadedBackstory.bodyTypeGlobal = selectedItem;

        }

        private void comboBoxBodyTypeMale_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (CurrentlyLoadedDefs == null) return;
            var box = comboBoxBodyTypeMale;
            var selectedItem = (BodyType)box.Items[box.SelectedIndex];

            CurrentlyLoadedBackstory.bodyTypeMale = selectedItem;
        }

        private void comboBoxBodyTypeFemale_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (CurrentlyLoadedDefs == null) return;
            var box = comboBoxBodyTypeFemale;
            var selectedItem = (BodyType)box.Items[box.SelectedIndex];

            CurrentlyLoadedBackstory.bodyTypeFemale = selectedItem;
        }

        private void radioButtonAdulthood_CheckedChanged(object sender, EventArgs e)
        {
            radioButtonChildhood.Checked = !radioButtonAdulthood.Checked;
            CurrentlyLoadedBackstory.slot = Slot.Adulthood;
        }
    }
}
