/*
 * Custom Made interpreter for Console Explorer
 * I wanted so that the user can insert their own commands
 * in Console Explorer, So I am making this Interpreter.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Console
{

    internal class Interpret
    {
        string SFile; //Source File
        List<string> Lines = new List<string>();
        Dictionary<string, object> Variable = new Dictionary<string, object>(); //<varname>=<value:int or string>
        List<Delegate> cmds;
        private List<string> varin = new List<string>();

        public Interpret(string File,List<Delegate> func)
        {
            SFile = File;
            cmds = new List<Delegate>(func);
            
            if (!BeginRead())
            {
                MessageBox.Show("file cant be emptY!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if(Path.GetExtension(File) != ".csc")
            {
                print($"File: {File} is not a console script!", true);
                return;
            }

        }


        ~Interpret()
        {
            Variable.Clear();
            cmds.Clear();
            Lines.Clear();
            SFile = string.Empty;
        }

           
        public async Task RunAsync()
        {
            await Evaluate();
        }

        private async Task<string> InputAsync()
        {
            Func<Task<string>> wait = (Func<Task<string>>)cmds[1];
            return await wait();
        }

        private bool BeginRead()
        {
            if(SFile == string.Empty)
            {
                return false;
            }

            foreach(string lines in File.ReadAllLines(SFile))
            {
                if (string.IsNullOrWhiteSpace(lines))
                {
                    continue;
                }

                Lines.Add(lines);
            }

            return true;
        }

        private void debug(string msg)
        {
            MessageBox.Show(msg, "debug", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void print(string msg,bool isError = false,bool isWarning = false)
        {
            ((Action<string, bool, bool>)cmds[0])(msg, isError, isWarning);
        }


        private bool hasVariable(string input)
        {
 
            bool found = false;

            if (Variable.Count > 0)
            {
  
                foreach (string token in varin)
                {

                    if (Variable.ContainsKey(token))
                    {
                        found = !found;
                       
                        break;
                    }
 

                }
            }

            return found;
        }

        

        private async Task Evaluate() // to evaluate line by line.
        {
            if(Lines.Count < 1) // if there is no lines at all we dont evaluate. just in case
            {
                return;
            }
            string[] cmds; //list of syntaxes in script
            
            foreach(string line in Lines)
            {
                cmds = Utility.TokenizeString(line);

                if (cmds[0].ToLower().Contains("print")) //output
                {
                    string tmp = cmds[0].Remove(0,6).Trim(')');

                    if (hasVariable(tmp))
                    {
                        tmp = Utility.ReplaceWords(tmp, varin.ToArray(), Variable, '$');
                    }

                    print(tmp);
                    continue;
                }else if (cmds[0].ToLower().Contains("input")) //input
                {
                    string varName = cmds[0].Remove(0, 6).Trim(')');

                    Variable[varName] = await InputAsync();

                    continue;
                } else if (cmds[0].ToLower().Contains("var"))
                {
                    string[] vars = cmds[1].Split('=');
                    varin.Add(vars[0]);
                    if (vars[1] == "null") // so that we can have empty variables to be used in input
                    {
                       
                        Variable.Add(vars[0], string.Empty);
                        continue;
                    }
                    Variable.Add(vars[0], vars[1]);
                    continue;
                }
                else
                {
                    print($"Syntax: {cmds[0]} is not valid!",true);
                }
            }

        }
    }
}
