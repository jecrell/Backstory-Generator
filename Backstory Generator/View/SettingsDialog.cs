using Backstory_Generator.Properties;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Backstory_Generator
{
    public partial class SettingsDialog : Form
    {

        string rimWorldPath;
        StringCollection modPaths;

        private string newRimWorldPath;
        private StringCollection newModPaths;


        public SettingsDialog()
        {
            InitializeComponent();
            rimWorldPath = Settings.Default["RimWorldPath"].ToString();
            modPaths = (StringCollection)Settings.Default["LoadedModPaths"];
            newRimWorldPath = rimWorldPath;
            newModPaths = new StringCollection();
        }

        private void toolTip1_Popup(object sender, PopupEventArgs e)
        {

        }

        private void Form2_Load(object sender, EventArgs e)
        {
            this.textBoxRimWorldPath.Text = rimWorldPath;

            if (modPaths == null) modPaths = new StringCollection();
            if (modPaths?.Count > 0)
            {
                for (int i = 0; i < modPaths.Count; i++)
                {
                    this.listBoxIncludedModPaths.Items.Add(modPaths[i]);
                    newModPaths.Add(modPaths[i]);
                }
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Settings.Default["RimWorldPath"] = newRimWorldPath;
            Settings.Default["LoadedModPaths"] = newModPaths;
            Close();
        }

        private void textBoxRimWorldPath_TextChanged(object sender, EventArgs e)
        {
            newRimWorldPath = textBoxRimWorldPath.Text;
        }

        private void buttonGetModPath_Click(object sender, EventArgs e)
        {
            GetFilepathForTextBox(textBoxIncludedModPaths);
        }

        void GetFilepathForTextBox(TextBox tb)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    tb.Text = fbd.SelectedPath;
                }
            }
        }

        private void buttonSetRimWorldPath_Click(object sender, EventArgs e)
        {
            GetFilepathForTextBox(textBoxRimWorldPath);
        }

        private void buttonAddModPath_Click(object sender, EventArgs e)
        {
            string text = textBoxIncludedModPaths.Text;
            if (text != "")
            {
                listBoxIncludedModPaths.Items.Add(text);
                newModPaths.Add(text);
                textBoxIncludedModPaths.Text = "";
            }
        }

        private void buttonDel_Click(object sender, EventArgs e)
        {
            var i = listBoxIncludedModPaths.SelectedItem;
            if (i != null)
            {
                listBoxIncludedModPaths.Items.Remove(i);
                newModPaths.Remove(i.ToString());
            }
        }
    }
}
