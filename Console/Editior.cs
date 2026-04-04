using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Console
{
    public partial class Editior : Form
    {
        private string fpath;
        bool currEmpty = false;
        
        public Editior(string tpath)
        {
            InitializeComponent();
            fpath = tpath;

            if (string.IsNullOrEmpty(fpath))
            {
                currEmpty = true;
            }
            else
            {

                rtb_edit.Text = File.ReadAllText(fpath);
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currEmpty == true)
            {
                MessageBox.Show("Cannot Save without File Name!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else
            {
                
                File.WriteAllText(fpath, rtb_edit.Text);
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
           
                
                using (SaveFileDialog save1 = new SaveFileDialog()) {
                    save1.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
                    save1.Title = "Save File As";

                    if(save1.ShowDialog() == DialogResult.OK)
                    {

                        File.WriteAllText(fpath, rtb_edit.Text);
                  
                    }

                    
                }
            
        }

        private void lightToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.BackColor = default;
            this.ForeColor = default;
            rtb_edit.ForeColor = default;
            rtb_edit.BackColor = default;
            mn_editor.BackColor = default;
            mn_editor.ForeColor = default;
        }

        private void darkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.BackColor = Color.Black;
            this.ForeColor = Color.Black;
            rtb_edit.ForeColor = Color.White;
            rtb_edit.BackColor = Color.Black;

        }

        private void aboutConsoleExplorerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("A C# Application made by Cortez," + Environment.NewLine + "A 3rd year Computer Engineering Student.", "About", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
