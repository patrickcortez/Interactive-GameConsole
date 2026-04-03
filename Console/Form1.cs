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


namespace Console
{
    public partial class Form1 : Form
    {

        List<string> history = new List<string>();
        List<string> syntax = new List<string>();
        Dictionary<int, char> log = new Dictionary<int, char>();
        
        int line = 1;
        bool correct = false;
        int traverse = 0;
        

        string GetHistoryPath()
        {
            string tmp = Environment.GetEnvironmentVariable("APPDATA");

            if (!string.IsNullOrEmpty(tmp))
            {
                return tmp;
            }

            return AppDomain.CurrentDomain.BaseDirectory;
        }


        string GetAssetsFolder()
        {
            string assets = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets");
            if (!Directory.Exists(assets))
            {
                MessageBox.Show("Assets Folder Missing!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return string.Empty;
            }

            return assets;
            

        }

        private void init()
        {
            if (Directory.Exists(Path.Combine(GetHistoryPath(),"db")))
            {
                if(File.Exists(Path.Combine(GetHistoryPath(), "db", "history.txt")))
                {
                    return;
                }
                File.Create(Path.Combine(GetHistoryPath(), "db", "history.txt")).Close();
            }
            else
            {

                Directory.CreateDirectory(Path.Combine(GetHistoryPath(), "db"));
                File.Create(Path.Combine(GetHistoryPath(), "db", "history.txt")).Close();
            }
        }

        public Form1()
        {
            try
            {
                InitializeComponent();
                init();
                ReadHistory();
                lb_path.Text = currentDirectory.FullName;
                rtb_log.Text += "CLI" + Environment.NewLine + "Logs:" + Environment.NewLine;
                rtb_output.Text = "Welcome to Console Explorer" + Environment.NewLine;
                rtb_output.Text += "Type 'help' to begin exploring!" + Environment.NewLine + Environment.NewLine;
                string[] cmds = { "echo", "exit", "copy", "create", "move", "export", "delete", "clear", "change", "list", "current", "help" };
                syntax.AddRange(cmds);

                var assets = GetAssetsFolder();

                if (!string.IsNullOrEmpty(assets))
                {
                   
                    this.Icon = new Icon(Path.Combine(assets, "terminal.ico"));
                }
                else
                {
                    MessageBox.Show("Icon file missing, Reverting to default...", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        bool isInSyntax(string input)
        {
            var tmp = input.Split(' ');

            foreach(var cmd in syntax)
            {
                if(cmd == tmp[0])
                {
                    return true;
                }
            }

            return false;
        }

        private void ReadHistory()
        {
            string historyPath = Path.Combine(GetHistoryPath(), "db", "history.txt");
            if (File.Exists(historyPath))
            {
                foreach(var line in File.ReadAllLines(historyPath))
                {
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        history.Add(line);
                    }
                }
            }
            else
            {
                print("Failed to read history");
            }
        }

        string input = string.Empty;
        DirectoryInfo currentDirectory = new DirectoryInfo(Environment.GetEnvironmentVariable("USERPROFILE"));


        void print(string text)
        {
            rtb_output.Text += text + Environment.NewLine;
        }

        private void WriteHistory(string command)
        {
            string historyPath = Path.Combine(GetHistoryPath(), "db", "history.txt");
            File.AppendAllText(historyPath, command + Environment.NewLine);
        }

        private void btn_submit_Click(object sender, EventArgs e)
        {
            input = tb_input1.Text;   
            WriteHistory(input);
            mainLoop();
            evaluate(input);
            tb_input1.Text = string.Empty;
        }

        private void help()
        {
            print("Available Commands:");
            print("echo <text> - Prints the text to the console.");
            print("clear - Clears the console output.");
            print("current - Displays the current directory.");
            print("change <directory-path> - Changes the current directory to the specified path.");
            print("list <directory-path> - Lists all the files in the directory");
            print("create <file/directory> <name> - Creates a file or directory.");
            print("delete <file/directory> <name> - Deletes a file or directory.");
            print("copy <src> <dest> - copies a file/folder to a destination");
            print("move <src> <dest> - moves a file/folder to a destination");
            print("export <var>=<value> - exports env var.");
            print("edit <file-path> - edit any text files.");
            print("exit - Exits the application.");
        }

        private bool IsEnvVar(string input)
        {
            if (input.StartsWith("%") && input.EndsWith("%")) {
                string envar = input.Trim('%');
                return Environment.GetEnvironmentVariable(envar) != null;

            }

            return false;
        }

        private void evaluate(string input)
        {
            correct = isInSyntax(input);

            if(correct == true)
            {
                log.Add(line, 'O');
                
            }
            else
            {
                log.Add(line, 'X');
                correct = true;
            }

            rtb_log.Text +=  line + " " + log[line] + Environment.NewLine;
            line++;
        }

        private void setArray(ref string[] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (IsEnvVar(array[i]) == true)
                {
                    array[i] = Environment.ExpandEnvironmentVariables(array[i]);
                }
            }
        }

        private void mainLoop()
        {
            try
            {
                string[] cmd = input.Split(' ');

                setArray(ref cmd);
                if (cmd[0].ToLower() == "echo")
                {
                    string output = string.Join(" ", cmd.Skip(1));
                    print(output);
                }
                else if (cmd[0].ToLower() == "clear")
                {
                    rtb_output.Text = string.Empty;
                }
                else if (cmd[0].ToLower() == "current")
                {
                    print(currentDirectory.FullName);
                }
                else if (cmd[0].ToLower() == "change")
                {
                    if (cmd.Length < 2)
                    {
                        print("Usage: change <directory-path>");
                        correct = false;
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
                        correct = false;
                        return;
                    }

                    currentDirectory = newDir;
                    lb_path.Text = currentDirectory.FullName;
                }
                else if (cmd[0].ToLower() == "help")
                {
                    help();
                }
                else if (cmd[0].ToLower() == "list")
                {

                    if (cmd.Length < 2)
                    {
                        var tpath = currentDirectory.FullName;

                        var tlistf = Directory.GetFiles(tpath);
                        var tlistd = Directory.GetDirectories(tpath);

                        foreach (var dir in tlistd)
                        {
                            print("[DIR] " + Path.GetFileName(dir));
                        }

                        foreach (var file in tlistf)
                        {
                            print("[FILE] " + Path.GetFileName(file));
                        }

                        return;
                    }

                    var path = Path.Combine(currentDirectory.FullName, cmd[1]);

                    if (!Directory.Exists(path))
                    {
                        print("Directory does not exist: " + path);
                        correct = false;
                        return;
                    }

                    var listf = Directory.GetFiles(path);
                    var listd = Directory.GetDirectories(path);

                    foreach (var dir in listd)
                    {
                        print("[DIR] " + Path.GetFileName(dir));
                    }

                    print(Environment.NewLine);

                    foreach (var file in listf)
                    {
                        print("[FILE] " + Path.GetFileName(file));
                    }

                }
                else if (cmd[0].ToLower() == "create")
                {

                    var path = Path.Combine(currentDirectory.FullName, cmd[2]);


                    if (cmd.Length < 3)
                    {
                        print("Usage: create <file/directory> <name>");
                        correct = false;
                    }

                    if (File.Exists(path) || Directory.Exists(path))
                    {
                        print("File or Directory already exists: " + cmd[2]);
                        correct = false;
                        return;
                    }

                    if (cmd[1].ToLower() == "file")
                    {
                        if (cmd.Length < 3)
                        {
                            print("File name cant be empty!");
                            correct = false;
                            return;
                        }
                        File.Create(path).Close();
                        print("File: " + cmd[2] + " created successfully.");
                    }
                    else if (cmd[1].ToLower() == "directory")
                    {
                        if (cmd.Length < 3)
                        {
                            print("Folder name cant be empty!");
                            correct = false;
                            return;
                        }
                        Directory.CreateDirectory(path);
                        print("Directory: " + cmd[2] + " created successfully.");
                    }
                    else
                    {
                        print(cmd[2] + " failed to be created.");
                        correct = false;
                    }
                }
                else if (cmd[0].ToLower() == "delete")
                {
                    var path = Path.Combine(currentDirectory.FullName, cmd[2]);

                    if (cmd.Length < 3)
                    {
                        print("Usage: " + "delete <file/directory> <name>");
                        correct = false;
                    }

                    if (cmd[1].ToLower() == "file")
                    {
                        if (File.Exists(path))
                        {
                            File.Delete(path);
                            print("File: " + cmd[2] + " deleted successfully.");
                        }
                        else
                        {

                            print("File: " + cmd[2] + " does not exist.");
                            correct = false;
                        }
                    }
                    else if (cmd[1].ToLower() == "directory")
                    {
                        if (Directory.Exists(path))
                        {
                            Directory.Delete(path, true);
                            print("Directory: " + cmd[2] + " deleted successfully.");
                        }
                        else
                        {
                            print("Directory: " + cmd[2] + " does not exist.");
                            correct = false;
                        }
                    }

                }
                else if (cmd[0].ToLower() == "copy")
                {


                    string src;
                    string dest;

                    if (Path.IsPathRooted(cmd[1]))
                    {
                        src = cmd[1];
                    }
                    else
                    {
                        src = Path.Combine(currentDirectory.FullName, cmd[1]);
                    }

                    if (Path.IsPathRooted(cmd[2]))
                    {
                        dest = cmd[2];
                    }
                    else
                    {
                        dest = Path.Combine(currentDirectory.FullName, cmd[2]);
                    }

                    if (File.Exists(src))
                    {
                        File.Copy(src, dest);
                        print("Copy: " + cmd[1] + " to " + cmd[2] + " copy successful!");
                    }
                    else
                    {
                        print("File:" + cmd[1] + " does not exist.");
                        correct = false;
                    }
                }
                else if (cmd[0].ToLower() == "move")
                {
                    string src;
                    string desc;

                    if (Path.IsPathRooted(cmd[1]))
                    {
                        src = cmd[1];
                    }
                    else
                    {
                        src = Path.Combine(currentDirectory.FullName, cmd[1]);
                    }

                    if (Path.IsPathRooted(cmd[2]))
                    {
                        desc = cmd[2];
                    }
                    else
                    {
                        desc = Path.Combine(currentDirectory.FullName, cmd[2]);
                    }


                    if (File.Exists(src))
                    {
                        File.Move(src, desc);
                        print("Move: " + cmd[1] + " to " + cmd[2] + " move successful!");

                    }
                    else
                    {
                        print("File:" + cmd[1] + " does not exist.");
                        correct = false;
                    }
                }
                else if (cmd[0].ToLower() == "exit")
                {
                    Application.Exit();
                }
                else if (cmd[0].ToLower() == "export")
                {
                    if (cmd.Length < 2)
                    {
                        print("Usage: export <varname>=<value>");
                        correct = false;
                        return;
                    }

                    var parts = cmd[1].Split('=');

                    if (parts.Length < 2)
                    {
                        print("Value cant be empty!");
                        correct = false;
                        return;
                    }

                    Environment.SetEnvironmentVariable(parts[0], parts[1]);

                    print("Environment variable set!");

                }else if (cmd[0] == "edit")
                {
                    if(cmd.Length< 2)
                    {
                        Editior ed = new Editior(string.Empty);
                        ed.Show();
                        return;
                    }

                    string npath;

                    if (Path.IsPathRooted(cmd[1]))
                    {
                        npath = cmd[1];
                    }
                    else
                    {
                        npath = Path.Combine(currentDirectory.FullName, cmd[1]);
                    }

                    if (File.Exists(npath))
                    {

                        Editior ed = new Editior(npath);
                        ed.Show();
                    }

                }
                else
                {
                    print("Unknown Command: " + cmd[0]);
                    correct = false;
                }
            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void tb_input1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true; // prevent the beep sound
                btn_submit.PerformClick(); // trigger the button click event
            }

            if (e.KeyCode == Keys.Up)
            {
                if (history.Count > 0)
                {
                    if (traverse < history.Count)
                    {
                        traverse++;
                        tb_input1.Text = history[history.Count - traverse];
                        tb_input1.SelectionStart = tb_input1.Text.Length; // move cursor to end
                    }
                }
            }
            else if (e.KeyCode == Keys.Down)
            {
                if (history.Count > 0)
                {
                    if (traverse > 1)
                    {
                        traverse--;
                        tb_input1.Text = history[history.Count - traverse];
                        tb_input1.SelectionStart = tb_input1.Text.Length; // move cursor to end
                    }
                    else
                    {
                        traverse = 0;
                        tb_input1.Text = string.Empty;
                    }
                }
            }

            else if(e.KeyCode == Keys.ControlKey && e.KeyCode == Keys.C)
            {
                if (!string.IsNullOrEmpty(rtb_output.Text))
                {
                    Clipboard.SetText(rtb_output.SelectedText);
                }
            }


            else if(e.KeyCode == Keys.ControlKey && e.KeyCode == Keys.V)
            {
                if (!string.IsNullOrEmpty(Clipboard.GetText()))
                {
                    tb_input1.Text += Clipboard.GetText();
                }
            }
        }


    }
}
