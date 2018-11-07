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
            groupBox1.Visible = enable;
            saveToolStripMenuItem.Enabled = enable;
            saveAsToolStripMenuItem.Enabled = enable;
            closeToolStripMenuItem.Enabled = enable;
        }

        private static string GetDefaultFilePath()
        {
            return Environment.CurrentDirectory + @"\test.xml";
        }

        private void SerializeDataSet(string filename)
        {
            XmlSerializer ser = new XmlSerializer(typeof(Defs));
            //XmlSerializer ser = new XmlSerializer(typeof(Backstory[]));
            TextWriter writer = new StreamWriter(filename);

            if (CurrentlyLoadedDefs == null)
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
            
        }

        private void openFile(Stream openFileStream, string fileName)
        {

            Defs defs = new Defs();

            XmlSerializer ser = new XmlSerializer(typeof(Defs));
            
            defs = ser.Deserialize(openFileStream) as Defs;


            CurrentlyLoadedDefs = defs;
            CurrentlyLoadedFile = fileName;

            ShowFileControls(true);

            UpdateListBox(defs);

            openFileStream.Close();

        }

        private void UpdateListBox(Defs defs)
        {
            if (defs.Backstories == null || defs.Backstories.Count == 0)
                listBox1.DataSource = null;

            var currentIndex = listBox1.SelectedIndex;
            BindingList<string> defNames = new BindingList<string>();
            foreach (var def in defs.Backstories)
                defNames.Add(def.defName);
            listBox1.DataSource = defNames;
            listBox1.SelectedIndex = currentIndex % defNames.ToArray().Length;
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SerializeDataSet(GetDefaultFilePath());
            MessageBox.Show("Created file");
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
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
                openFile(openFileDialog1.OpenFile(), openFileDialog1.FileName);
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex < 0) return;

            Backstory curBackstory = GetCurrentbackstory();

            textBoxDefName.Text = curBackstory.defName;
            textBoxTitle.Text = curBackstory.title;
            richTextBoxDescription.Text = curBackstory.baseDescription;
            radioButtonAdulthood.Checked = curBackstory.slot == Slot.Adulthood;
            radioButtonChildhood.Checked = !radioButtonAdulthood.Checked;
            dataGridViewSkills.DataSource = curBackstory.skillGains;
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            if (CurrentlyLoadedDefs == null) return;

            if (listBox1.SelectedIndex < 0) return;
            Backstory curBackstory = GetCurrentbackstory();

            curBackstory.defName = textBoxDefName.Text;
            curBackstory.title = textBoxTitle.Text;
            curBackstory.baseDescription = richTextBoxDescription.Text;
            curBackstory.slot = radioButtonAdulthood.Checked ? Slot.Adulthood : Slot.Childhood;

            UpdateListBox(CurrentlyLoadedDefs);

        }

        private Backstory GetCurrentbackstory()
        {
            var curIndex = 0;
            if (listBox1.SelectedIndex > -1)
                curIndex = listBox1.SelectedIndex;
            return CurrentlyLoadedDefs.Backstories.ElementAt(curIndex);
        }

        private void radioButtonChildhood_CheckedChanged(object sender, EventArgs e)
        {
            radioButtonAdulthood.Checked = !radioButtonChildhood.Checked;
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CurrentlyLoadedDefs != null)
                CurrentlyLoadedDefs = null;
            if (CurrentlyLoadedFile != "")
                CurrentlyLoadedFile = "";
            ShowFileControls(false);
            listBox1.DataSource = null;
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (File.Exists(CurrentlyLoadedFile))
                SerializeDataSet(CurrentlyLoadedFile);
            else
            {
                var result = MessageBox.Show("Error! File at " + CurrentlyLoadedFile + " is missing. Create new file?", "File Path Not Found", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                    SerializeDataSet(CurrentlyLoadedFile);
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void buttonAddSkill_Click(object sender, EventArgs e)
        {
            Backstory curBackstory = GetCurrentbackstory();
            if (curBackstory.skillGains == null)
                curBackstory.skillGains = new List<SkillGain>();

            var item = comboBoxSkills.Items[comboBoxSkills.SelectedIndex];

            if (item == "Animals")
                curBackstory.skillGains.Add(new SkillGain() { defName = SkillDef.Animals, amount = 1 });
            if (item == "Artistic")
                curBackstory.skillGains.Add(new SkillGain() { defName = SkillDef.Artistic, amount = 1 });
            if (item == "Construction")
                curBackstory.skillGains.Add(new SkillGain() { defName = SkillDef.Construction, amount = 1 });
            if (item == "Cooking")
                curBackstory.skillGains.Add(new SkillGain() { defName = SkillDef.Cooking, amount = 1 });
            if (item == "Crafting")
                curBackstory.skillGains.Add(new SkillGain() { defName = SkillDef.Crafting, amount = 1 });
            if (item == "Intellectual")
                curBackstory.skillGains.Add(new SkillGain() { defName = SkillDef.Intellectual, amount = 1 });
            if (item == "Medicine")
                curBackstory.skillGains.Add(new SkillGain() { defName = SkillDef.Medicine, amount = 1 });
            if (item == "Melee")
                curBackstory.skillGains.Add(new SkillGain() { defName = SkillDef.Melee, amount = 1 });
            if (item == "Mining")
                curBackstory.skillGains.Add(new SkillGain() { defName = SkillDef.Mining, amount = 1 });
            if (item == "Plants")
                curBackstory.skillGains.Add(new SkillGain() { defName = SkillDef.Plants, amount = 1 });
            if (item == "Shooting")
                curBackstory.skillGains.Add(new SkillGain() { defName = SkillDef.Shooting, amount = 1 });
            if (item == "Social")
                curBackstory.skillGains.Add(new SkillGain() { defName = SkillDef.Social, amount = 1 });

            dataGridViewSkills.DataSource = null;
            dataGridViewSkills.DataSource = curBackstory.skillGains;
        }
    }
}
