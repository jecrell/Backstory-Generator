using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace Backstory_Generator
{
    public partial class FormViewer : Form
    {

        private BackstoryFile SaveFileDialog(string prefix, bool newFile = false)
        {
            if (!TryLoadTraitEntries()) return null;
            
            BackstoryFile saveFile = null;

            // Displays a SaveFileDialog so the user can save the XML 
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "XML File|*.xml";
            saveFileDialog1.Title = "Save an XML File";
            saveFileDialog1.ShowDialog();

            // If the file name is not an empty string open it for saving.  
            if (saveFileDialog1.FileName != "")
            {
                if (!File.Exists(saveFileDialog1.FileName))
                    saveFile = new BackstoryFile(saveFileDialog1.FileName);

                // Saves the Image via a FileStream created by the OpenFile method.  
                saveFile.Serialize(prefix, !newFile);
                MessageBox.Show("Created successfully");
                OpenFile(saveFileDialog1.FileName, newFile);
                
            }
            return saveFile;
        }


    }
}
