using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;


namespace Console
{
    public partial class Form1 : Form
    {

        Process shell;
        public Form1()
        {
            InitializeComponent();
            lb_path.Text = currentDirectory.FullName;

            var psi = new ProcessStartInfo
            {
                FileName = "linuxify.exe",
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            shell = new Process();
            shell.StartInfo = psi;

            shell.OutputDataReceived += (s, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                    Invoke(new Action(() => print(e.Data)));
            };

            shell.ErrorDataReceived += (s, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                    Invoke(new Action(() => print(e.Data)));
            };

            shell.Start();
            shell.BeginOutputReadLine();
            shell.BeginErrorReadLine();
        }

        string input = string.Empty;
        DirectoryInfo currentDirectory = new DirectoryInfo(Environment.GetEnvironmentVariable("USERPROFILE"));


        void print(string text)
        {
            rtb_output.Text += text + Environment.NewLine;
        }

        private void btn_submit_Click(object sender, EventArgs e)
        {
            shell.StandardInput.WriteLine(input);
            input = tb_input1.Text;
            mainLoop();
            tb_input1.Text = string.Empty;
        }

        private void help()
        {
            print("Available Commands:");
                print("echo <text> - Prints the text to the console.");
                print("clear - Clears the console output.");
                print("current - Displays the current directory.");
                print("change <directory-path> - Changes the current directory to the specified path.");
        }

        private void mainLoop()
        {
            string[] cmd = input.Split(' ');
            if (cmd[0].ToLower() == "echo")
            {
                print(string.Join(" ", cmd.Skip(1)));
            } 
            else if (cmd[0].ToLower() == "clear") {
                rtb_output.Text = string.Empty;
            }
            else if (cmd[0].ToLower() == "current")
            {
                print(currentDirectory.FullName);
            }
            else if (cmd[0].ToLower() == "change")
            {
                if(cmd.Length < 2)
                {
                    print("Usage: change <directory-path>");
                    return;
                }

                string newPath;

                if (Path.IsPathRooted(cmd[1])) //if its an Absolute path
                {
     
                    newPath = cmd[1];
                }
                else // Relative path
                {
                   
                    newPath = Path.Combine(currentDirectory.FullName, cmd[1]);
                }

                var newDir = new DirectoryInfo(newPath);

                if (!newDir.Exists)
                {
                    print("Directory does not exist: " + newPath);
                    return;
                }

                currentDirectory = newDir;
                lb_path.Text = currentDirectory.FullName;
            } else if(cmd[0].ToLower() == "help")
            {
                help();
            }
            else if (cmd[0].ToLower() == "./")
            {
                string exeName = cmd[0].Substring(2);

                if (cmd[0].Length == 0)
                {
                    print("Usage: ./<exe-file>");
                    return;
                }

                var exePath = Path.Combine(currentDirectory.FullName, exeName);

                if(!File.Exists(exePath))
                {
                    print("Executable not found: " + exePath);
                    return;
                }


                
                
            }
            else
            {
                print("Unknown Command: " + cmd[0]);
                shell.StandardInput.WriteLine(input);
            }
        }
    }
}
