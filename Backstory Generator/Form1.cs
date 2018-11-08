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
    public partial class Form1 : Form
    {
        public string CurrentlyLoadedFile { get; set; }
        public Defs CurrentlyLoadedDefs { get; set; }
        public Backstory CurrentlyLoadedBackstory
        {
            get
            {
                var curIndex = 0;
                if (listBox1.SelectedIndex > -1)
                    curIndex = listBox1.SelectedIndex;
                return CurrentlyLoadedDefs.Backstories.ElementAt(curIndex);
            }
        }


        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ShowFileControls(false);
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
            
            comboBoxSkills.DataSource = Enum.GetValues(typeof(SkillDef));
        }



        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex < 0) return;

            Backstory curBackstory = CurrentlyLoadedBackstory;

            textBoxDefName.Text = curBackstory.defName;
            textBoxTitle.Text = curBackstory.title;
            richTextBoxDescription.Text = curBackstory.baseDescription;
            radioButtonAdulthood.Checked = curBackstory.slot == Slot.Adulthood;
            radioButtonChildhood.Checked = !radioButtonAdulthood.Checked;
            GridViewUtility.UpdateView(dataGridViewSkills, curBackstory.skillGains);
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

        private void radioButtonChildhood_CheckedChanged(object sender, EventArgs e)
        {
            radioButtonAdulthood.Checked = !radioButtonChildhood.Checked;
        }

        private void buttonAddSkill_Click(object sender, EventArgs e)
        {
            Enum.TryParse<SkillDef>(comboBoxSkills.Items[comboBoxSkills.SelectedIndex].ToString(), out var skill);
            var newSkill = new SkillGain() { defName = skill, amount = 1 };

            GridViewUtility.AddRow(dataGridViewSkills, CurrentlyLoadedBackstory.skillGains, newSkill, e);
            
        }

        private void buttonOpenFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog();
        }

        private void buttonCreateFile_Click(object sender, EventArgs e)
        {
            SaveFileDialog();
            
        }
        
        private void dataGridViewSkills_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            GridViewUtility.DeleteRow(dataGridViewSkills, CurrentlyLoadedBackstory.skillGains, e);
        }

        private void radioButtonVanilla_CheckedChanged(object sender, EventArgs e)
        {
            radioButtonAlienRace.Checked = !radioButtonVanilla.Checked;
        }

        private void radioButtonAlienRace_CheckedChanged(object sender, EventArgs e)
        {
            radioButtonVanilla.Checked = !radioButtonAlienRace.Checked;
        }
    }
}
