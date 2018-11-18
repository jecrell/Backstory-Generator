using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Backstory_Generator
{
    public partial class TitleDialog : Form
    {
        private Backstory backstory;

        public TitleDialog(Backstory newBackstory)
        {
            InitializeComponent();
            backstory = newBackstory;
        }

        public TitleDialog()
        {
            InitializeComponent();
        }
        
        private void TitleDialog_Load(object sender, EventArgs e)
        {
            textBoxTitle.Text = backstory.title;
            textBoxTitleFemale.Text = backstory.titleFemale;
            textBoxTitleShort.Text = backstory.titleShort;
            textBoxTitleFemaleShort.Text = backstory.titleShortFemale;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (Form.ModifierKeys == Keys.None && keyData == Keys.Escape)
            {
                this.Close();
                return true;
            }
            return base.ProcessDialogKey(keyData);
        }

        private void buttonApply_Click(object sender, EventArgs e)
        {
            backstory.title = textBoxTitle.Text;
            backstory.titleFemale = textBoxTitleFemale.Text;
            backstory.titleShort = textBoxTitleShort.Text;
            backstory.titleShortFemale = textBoxTitleFemaleShort.Text;
            Close();
        }

        private void Form_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }
       

    }
}
