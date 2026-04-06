using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using System.Web.UI.WebControls;
using System.Windows.Forms;

namespace Console
{

    public partial class Form1 : Form
    {

        List<string> history = new List<string>();
        List<string> syntax = new List<string>();
        Dictionary<int, char> log = new Dictionary<int, char>();
        Dictionary<string, Action> commands = new Dictionary<string, Action>();
        string[] delgate;
        string username = Path.GetFileName(Environment.GetEnvironmentVariable("USERPROFILE"));
         
        int line = 1;
        bool correct = false;
        int traverse = 0;
        

        string GetHistoryPath() //to grab the history file, for our dynamic history look up using arrow keys
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
                print("Warning: " + "Asset Folder is Missing!",false,true);
                return string.Empty;
            }

            return assets;

            
            

        }

        private void init() // we initialize the db folder in APPDATA
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
                    print("Warning: Icon File is missing!",false,true);
                }
            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        bool isInSyntax(string input) // this is probably abit unneeded but we'll keep it for now, to accurately check if its a syntax in live logging
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

        private void ReadHistory() // Read from file then store the contents in a map
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
                print("Failed to read history",true);
            }
        }

        private bool isExtra(string inp)
        {
            var tmp = inp.Split(' ');
            foreach (var key in commands.Keys)
            {
                if (key == tmp[0])
                {
                    return true;
                }
            }

            return false;
        }

        string input = string.Empty;
        DirectoryInfo currentDirectory = new DirectoryInfo(Environment.GetEnvironmentVariable("USERPROFILE"));
        bool isUser = false;

        void print(string text,bool isError = false,bool isWarning = false) // so I dont have to  manually type that long ass code
        {
            rtb_output.SelectionStart = rtb_output.TextLength;
            rtb_output.SelectionLength = 0;

            if(isError == true)
            {
                rtb_output.SelectionColor = Color.Red;
            } else if(isWarning == true)
            {
                rtb_output.SelectionColor = Color.Yellow;
            }
            else
            {
                rtb_output.SelectionColor = rtb_output.ForeColor;
            }

            string uname;

            if (isUser && !isError)
            {
                uname = username;
                isUser = !isUser;
            }
            else
            {
                uname = "System";
            }


            rtb_output.AppendText($"{uname}> {text}" + Environment.NewLine);


        }

        private void WriteHistory(string command) // Store user input to the history file, so we have a functional history system
        {
            string historyPath = Path.Combine(GetHistoryPath(), "db", "history.txt");
            File.AppendAllText(historyPath, command + Environment.NewLine); // we always append new text other wise its all going to be stored in one line
        }

        private void btn_submit_Click(object sender, EventArgs e) //this is where it all happens, the main loop is called, then the input gets evaluated
        {
            input = tb_input1.Text;   
            WriteHistory(input);
            mainLoop();
            evaluate(input);
            tb_input1.Text = string.Empty;
        }

        private void help() //for user convenience
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
            print("view <file-path> - view any image in your FS");
            print("exit - Exits the application.");
        }

        private bool IsEnvVar(string input) //this is our env var checker to make sure that the said token is actually a valid env var
        {
            if (input.StartsWith("%") && input.EndsWith("%")) {
                string envar = input.Trim('%');
                return Environment.GetEnvironmentVariable(envar) != null;

            }

            return false;
        }

        private void evaluate(string input) //yep i know its simple but it works! =P
        {
            correct = isInSyntax(input) || isExtra(input);

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

        private void setArray(ref string[] array) // this is to set up users input, incase there is some env var
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (IsEnvVar(array[i]) == true)
                {
                    array[i] = Environment.ExpandEnvironmentVariables(array[i]);
                }
            }
        }

 

        List<Delegate> Acts() //for sending methods to Commands.cs so this file doesnt become a god file,
        {

         
            List<Delegate> tmp = new List<Delegate>();
            tmp.Add(new Action<string,bool,bool>((s,v,d) => print(s,v,d)));
            tmp.Add(new Action<string>(c => getInput())); 
            
            return tmp; 
        }


        private void MapInit(Commands comd) // Initializer for future commands
        { // This is where we put our future commands in, by adding them to the Dict
            //test commands
            commands.Add("test",comd.Test);
            commands.Add("input", comd.TestInput);
            commands.Add("view", comd.showImage);

        }

        private void mainMap(string inp) // Our command executor for other commands
        {

            bool isvalid = false;

            foreach(var keys in commands.Keys)
            {
                if (inp == keys)
                {
                    isvalid = true;
                    break;
                }
            }


            if(isvalid == false) // guard clause so exception wont trigger
            {
                return;
            }

            commands[inp]();
        }

        private string getInput()
        {
            return tb_input1.Text;
        }

        private string handleQoute(string[] arr)
        {
            string tmp = string.Empty;

            if (arr[0].Contains('"'))
            {
                tmp = string.Join(" ", arr.Skip(1));
                tmp = tmp.Trim('"');
            }

            return tmp;
        }

        private void mainLoop() //this is slowly becoming a god function, I must do something about it.
        {
            try
            {
                string[] cmd = Utility.TokenizeString(input); //our simple tokenizer

                setArray(ref cmd);
                // And this is the start of our long ass if else command Evaluator
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

                    if (cmd[1].Contains('"'))
                    {
                        handleQoute(cmd);
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
                        print("Directory does not exist: " + newPath,true);
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
                        print("Directory does not exist: " + path,true);
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

                    if (cmd[2].Contains('"'))
                    {
                        handleQoute(cmd);
                    }

                    var path = Path.Combine(currentDirectory.FullName, cmd[2]);


                    if (cmd.Length < 3)
                    {
                        print("Usage: create <file/directory> <name>");
                        correct = false;
                    }

                    if (File.Exists(path) || Directory.Exists(path))
                    {
                        print("File or Directory already exists: " + cmd[2],true);
                        correct = false;
                        return;
                    }

                    if (cmd[1].ToLower() == "file")
                    {
                        if (cmd.Length < 3)
                        {
                            print("File name cant be empty!",true);
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
                            print("Folder name cant be empty!",true);
                            correct = false;
                            return;
                        }
                        Directory.CreateDirectory(path);
                        print("Directory: " + cmd[2] + " created successfully.");
                    }
                    else
                    {
                        print(cmd[2] + " failed to be created.",true);
                        correct = false;
                    }
                }
                else if (cmd[0].ToLower() == "delete")
                {
                    if (cmd[2].Contains('"'))
                    {
                        handleQoute(cmd);
                    }

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

                            print("File: " + cmd[2] + " does not exist.",true);
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
                            print("Directory: " + cmd[2] + " does not exist.",true);
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

                    if (File.Exists(src) || Directory.Exists(src))
                    {
                        char[] pref = { '-' };
                        if (Utility.GetFlag(cmd[3], pref) == 'r')
                        {
                            Utility.CopyDirectory(src, dest);
                            return;
                        }

                        dest = Path.Combine(dest, Path.GetFileName(src));
                        File.Copy(src, dest);
                        print("Copy: " + cmd[1] + " to " + cmd[2] + " copy successful!");
                    }
                    else
                    {
                        print("File:" + cmd[1] + " does not exist.",true);
                        correct = false;
                    }
                }
                else if (cmd[0].ToLower() == "move")
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


                    if (File.Exists(src) || Directory.Exists(src))
                    {
                        char[] pref = { '-' };

                        if (Utility.GetFlag(cmd[3],pref) == 'r')
                        {
                            Directory.Move(src,dest);
                        }
                        dest = Path.Combine(dest, Path.GetFileName(src));
                        File.Move(src, dest);
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

                }else if (cmd[0] == "edit") // if user wants to edit a text file, we call our text editor 
                {
                    if(cmd.Length< 2) //if empty we'll just use the current path, and let the user manually "Save As" other wise, they can use the Save option
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
                    bool hasexec = false;
                    Commands comd = new Commands(Acts(),hasexec,cmd,currentDirectory.ToString());

                    commands.Clear();
                    MapInit(comd);
                    

                    mainMap(cmd[0]);

                    if (comd.getexec() == false)
                    {
                        print("Unknown Command: " + cmd[0],true);
                        correct = false;
                    }
                }
            }catch(Exception ex)
            {
                //MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                print("Error: " + ex.Message,true);
            }
        }

        private void tb_input1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true; // prevent the beep sound
                btn_submit.PerformClick(); // trigger the button click event
                isUser = true;
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

        

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        bool modified = false;
        private void tb_input1_TextChanged(object sender, EventArgs e)
        {

            try
            {
                if (tb_input1.Text == string.Empty)
                {
                    return;
                }

                foreach (char c in tb_input1.Text)
                {
                    if (Utility.hasBraces(c))
                    {
                        modified = !modified;
                        break;
                    }
                }

                if (modified)
                {
                    tb_input1.AppendText(Utility.matchBrace(tb_input1.Text.Last()).ToString());
                    modified = !modified;
                }
            }catch(Exception ex)
            {
                print(ex.Message, true);
            }

        }
    }
}
