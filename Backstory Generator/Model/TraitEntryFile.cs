using Backstory_Generator.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Backstory_Generator
{
    public class TraitEntryFile
    {

        public List<TraitEntry> entries { get; set; }

        public TraitEntryFile(List<TraitEntry> newEntries)
        {
            entries = newEntries;
        }

        public void UpdateLabelsFor(BackstoryFile backstoryFile)
        {
            foreach (Backstory bs in backstoryFile.Backstories)
            {
                if (bs.forcedTraits != null && bs.forcedTraits.Count() > 0)
                {
                    foreach (var trait in bs.forcedTraits)
                    {
                        trait.label = entries.FirstOrDefault(x => x.defName == trait.defName && x.degree == trait.degree).label ?? "";
                    }
                }
                if (bs.disallowedTraits != null && bs.disallowedTraits.Count() > 0)
                {
                    foreach (var trait in bs.disallowedTraits)
                    {
                        trait.label = entries.FirstOrDefault(x => x.defName == trait.defName && x.degree == trait.degree).label ?? "";
                    }
                }
            }
        }

        public static TraitEntryFile Load(string fileName)
        {
            List<TraitEntry> newTraitEntries = new List<TraitEntry>();
            string traitsPath = fileName;

            List<string> xmlFiles = new List<string>() {
                traitsPath + @"\" + "Traits_Singular.xml",
                traitsPath + @"\" + "Traits_Spectrum.xml" };

            if (!Directory.Exists(traitsPath))
            {
                var result = MessageBox.Show("No RimWorld Path", "No directory path to RimWorld set. Set RimWorld path now?", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    SettingsDialog form2 = new SettingsDialog();
                    form2.Show();
                }
                return null;
            }

            foreach (string xmlFile in xmlFiles)
            {
                if (!File.Exists(xmlFile))
                {
                    MessageBox.Show("Cannot find file at \"" + xmlFile +
                        "\" Was it removed or deleted? " +
                        "Check RimWorld path in the settings menu.");
                    continue;
                }

                XDocument doc = XDocument.Load(xmlFile);


                var defRoot = "/Defs/";
                var defString = "TraitDef";
                var defCount = doc.XPathSelectElements(defRoot + defString).Count();


                for (int i = 1; i <= defCount; i++)
                {

                    string defName =
                        doc.XPathSelectElement(defRoot + defString + "[" + i + "]/defName").Value;

                    var degreeCount = doc.XPathSelectElements(defRoot + defString + "[" + i + "]/degreeDatas/li").Count();


                    for (int j = 1; j <= degreeCount; j++)
                    {


                        string newLabel = doc.XPathSelectElement(
                            defRoot + defString + "[" + i + "]/degreeDatas/li[" + j + "]/label").Value;

                        XElement newDegreeElement = doc.XPathSelectElement(
                            defRoot + defString + "[" + i + "]/degreeDatas/li[" + j + "]/degree");
                        int newDegree = default(int);
                        if (newDegreeElement != null)
                            Int32.TryParse(newDegreeElement.Value, out newDegree);

                        newTraitEntries.Add(new TraitEntry() { label = newLabel.FirstCharToUpper(), defName = defName, degree = newDegree });
                        //MessageBox.Show(newLabel + " " + defName + " " + newDegree);

                    }
                }

            }
            return new TraitEntryFile(newTraitEntries);
        }

        internal string GetDefNameByLabel(string label)
        {
            return entries.FirstOrDefault(x => x.label == label)?.defName ?? "";
        }
    }
}