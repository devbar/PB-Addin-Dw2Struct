using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnvDTE;
using System.IO;
using dw2structnv;
using System.Text.RegularExpressions;
using System.Xml;
using System.Windows.Forms;

namespace Dw2Struct
{
    public class PBHelper
    {
        /// <summary>
        /// retrieves the path of the selcected item
        /// </summary>
        /// <param name="item">selected item</param>
        /// <returns>file path</returns>
        private string GetSelectedFileName(SelectedItem item)
        {
            if (item != null)
            {
                if (item.ProjectItem != null)
                {
                    return item.ProjectItem.get_FileNames(1);
                }
            }

            return null;
        }

        /// <summary>
        /// checks the extension and indicates if it is a datawindow
        /// </summary>
        /// <param name="item">selected item</param>
        /// <returns>flag</returns>
        public bool IsDatawindow(SelectedItem item)
        {           
            string file = GetSelectedFileName(item);

            if (!String.IsNullOrEmpty(file))
            {
                if (file.Substring(file.Length - 4, 4).ToLower().Equals(".srd"))
                    return true;
            }
            
            return false;
        }

        /// <summary>
        /// generates a structure from the datawindow
        /// </summary>
        /// <param name="item"></param>
        public void GenerateStructure(SelectedItem item)
        {
            string file = GetSelectedFileName(item);            

            if (!String.IsNullOrEmpty(file))
            {
                bool fileNew = false;
                string syntax = null;
                string structure = null;                
                string structName = Regex.Replace(file.Substring(file.LastIndexOf("\\") +1).Replace(".srd", ""),"^d_", "s_");
                string structFile = file.Substring(0, file.LastIndexOf("\\") +1) + structName + ".srs";
                StreamReader reader = null;
                StreamWriter writer = null;
                n_dw2struct dw2struct = new n_dw2struct();

                reader = new StreamReader(new FileStream(file, FileMode.Open));
                syntax = reader.ReadToEnd();
                reader.Close();

                structure = dw2struct.GenerateStructure(syntax, structName);

                if (File.Exists(structFile))
                {
                    if (MessageBox.Show("The File " + structFile + " does already exist. Do you want to override it?", "Override", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.Cancel)
                        return;
                    else
                        File.Delete(structFile);
                }
                else
                {
                    fileNew = true;
                }


                writer = new StreamWriter(new FileStream(structFile, FileMode.CreateNew));
                writer.Write(structure);
                writer.Flush();
                writer.Close();

                if(fileNew)
                    AddToPbl(structFile, structName + ".srs");
            }
        }

        /// <summary>
        /// generates an nvo from the datawindow
        /// </summary>
        /// <param name="item"></param>
        public void GenerateNonVisualObject(SelectedItem item)
        {
            string file = GetSelectedFileName(item);  

            if (!String.IsNullOrEmpty(file))
            {
                bool fileNew = false;
                string syntax = null;
                string nonv = null;
                string nonvName = Regex.Replace(file.Substring(file.LastIndexOf("\\") + 1).Replace(".srd", ""), "^d_", "n_");
                string nonvFile = file.Substring(0, file.LastIndexOf("\\") + 1) + nonvName + ".sru";
                StreamReader reader = null;
                StreamWriter writer = null;
                n_dw2struct dw2struct = new n_dw2struct();

                reader = new StreamReader(new FileStream(file, FileMode.Open));
                syntax = reader.ReadToEnd();
                reader.Close();

                nonv = dw2struct.GenerateNonvisual(syntax, nonvName);

                if (File.Exists(nonvFile))
                {
                    if (MessageBox.Show("The File " + nonvFile + " does already exist. Do you want to override it?", "Override", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.Cancel)
                        return;
                    else
                        File.Delete(nonvFile);
                }
                else
                {
                    fileNew = true;
                }


                writer = new StreamWriter(new FileStream(nonvFile, FileMode.CreateNew));
                writer.Write(nonv);
                writer.Flush();
                writer.Close();

                if(fileNew)
                    AddToPbl(nonvFile, nonvName + ".sru");
            }
        }

        /// <summary>
        /// adds an new object to the (virtual) pbl
        /// </summary>
        /// <param name="objectfile"></param>
        /// <param name="name"></param>
        public void AddToPbl(string objectfile, string name)
        {
            string file = objectfile.Substring(0,objectfile.LastIndexOf(".pbl") + 4);

            string[] files = Directory.GetFiles(file, "*.pblx").ToArray();

            if (files.Length == 1)
            {
                XmlDocument xmlDoc = new XmlDocument();

                xmlDoc.Load(files[0]);

                XmlNodeList nodes = xmlDoc.GetElementsByTagName("Project");
                nodes[0].InnerXml += "<ItemGroup><Compile Include=\"" + name  + "\">" +
                                    "<SubType>Code</SubType>" +
                                    "</Compile></ItemGroup>";

                xmlDoc.Save(files[0]);
            }
            
        }

    }
}
