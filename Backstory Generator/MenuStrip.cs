using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Backstory_Generator
{
    public partial class Form1 : Form
    {

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog();
            MessageBox.Show("Created file");
        }


        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog();
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
                Save(CurrentlyLoadedFile);
            else
            {
                var result = MessageBox.Show("Error! File at " + CurrentlyLoadedFile + " is missing. Create new file?", "File Path Not Found", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                    Save(CurrentlyLoadedFile);
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();

        }

    }
}
