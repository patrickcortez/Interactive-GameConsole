using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Console
{

    class ImageForm : Form //We inherit the form class, so we can use this class as a ghost form
    {
        // This is our image handler.

        public ImageForm(string imgpath){ // Initiializer of our Magnificent ImageForm, which only exists when in this file, which is very convenient =D.
            try
            {

                PictureBox pbox = new PictureBox(); //instantiate our imagebox
                pbox.Dock = DockStyle.Fill; //Then set it all up to take over the entire window
                pbox.ImageLocation = imgpath;
                // then we set up our main ImageForm or our GhostForm.
                this.Controls.Add(pbox);
                this.ControlBox = false;

                Size newsize = new Size(876, 548);
                this.MinimumSize = newsize;
                this.MaximumSize = newsize;
                this.Size = newsize;
                this.KeyPreview = true;

                this.KeyDown += ImageFormKeyDown;
                this.Show();

            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
           

        }

        private void ImageFormKeyDown(Object sender,KeyEventArgs e) //The only way the user can exit... mwe hehehehe
        {
            if(e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
            else
            {
                MessageBox.Show("Press 'ESC' to Exit", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

    }

    class Commands
    {
        //All methods in this class has to be: PUBLIC to be passed in Main(Form1)
        //This is where all the extra commands come in:
        //We store all the other commands here instead of main

        /*
         * Reference:
         *  Main : Form1.cs
         *  cmds[] : user input
         *  methods : private methods being used by commands in main
         */

        
        private static List<Delegate> methods;
        TextBox input = new TextBox();
        string[] cmds;
        bool hasexec = false;
        DirectoryInfo currentdir;

        /*
         * Mental Map:
         *  methods index map:
         *  0: print() - for output in rtb_output
         * 
         */

        public Commands(List<Delegate> mtmps,bool exec, string[] inpargs,string CurrentDir) // Command Constructor, This is where we initialize our list of methods and input
        {
            //initiate our  string array and method list
            methods = mtmps; // for passing methods frim main to here incase some commands use private methods in main.
            hasexec = exec;
            cmds = inpargs; //tokenize our input for functions that need user input
            currentdir = new DirectoryInfo(CurrentDir);
        }


        public void Test()
        {
            ((Action<string>)methods[0])("Test");
            hasexec = true;
        }

        private void print(string msg,bool isError = false,bool isWarning = false)
        {
            ((Action<string,bool,bool>)methods[0])(msg,isError,isWarning);
        }

        public void TestInput()
        {
            try
            {
                string output = string.Join(" ", cmds.Skip(1));
                print(output);
                hasexec = true;
            }catch(Exception ex)
            {
                ((Action<string>)methods[0])("Error: " + ex.Message);
            }
        }

        public void showImage() // Image viewing command
        {
            try
            {
                string path = string.Join(" ", cmds.Skip(1));



                if (Path.IsPathRooted(path))
                {

                    if (!File.Exists(path))
                    {
                        print($"Image Doesnt Exist: {path}", true);
                        return;
                    }

                    ImageForm nform = new ImageForm(path);
                }
                else
                {



                    string nPath = Path.Combine(currentdir.ToString(), path);

                    if (!File.Exists(nPath))
                    {
                        print($"Image Doesnt Exist: {nPath}", true);
                        return;
                    }
                    ImageForm nform = new ImageForm(nPath);
                }
            }
            catch(Exception ex)
            {
                print($"Error: {ex.Message}",true);
            }

            hasexec = true;
        }

        internal bool getexec()
        {
            return hasexec;
        }

        
      

    }
}
