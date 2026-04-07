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
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Console
{

   internal class BenchMark //tried to make my own benchmarking but turns out there is already a StopWatch feature in Diagnostics!
    { // Imma save this for future use.

        Timer time;
        private int seconds =0;

        public BenchMark()
        {
            time = new Timer();

            time.Interval = 1000;
            time.Tick += countSeconds;

            time.Start();
        }



        private void countSeconds(Object sender,EventArgs e) //Event for counting how many seconds this class instance ran
        {
            seconds++;
        }

        public int Stop() // stop benchmarking and return how many seconds has elapsed
        {
            time.Stop();
            return seconds;
        }
    } // For now its unused.

    internal class Interpret : IDisposable
    {
        string SFile; //Source File
        List<string> Lines = new List<string>();
        Dictionary<string, object> Variable = new Dictionary<string, object>(); //<varname>=<value:int or string>
        List<Delegate> cmds;
        private List<string> varin = new List<string>();
        private List<String> syntax;
        private bool success = false;
        int seconds = 0;



        public Interpret(string File,List<Delegate> func)
        {
            SFile = File;
            cmds = new List<Delegate>(func);
            syntax = new List<string>();
            string[] syntaxes = { "print", "input", "var" };
            syntax.AddRange(syntaxes);
            
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


         public void Dispose() // Clear all once the instace is done running.
        {
            if (!success)
            {
                print($"Script: {SFile} failed to run!", true);
            }

            print($"Your script ran for {seconds} ms");

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

        private string whichSyntax(string line)
        {
            foreach(string code in syntax)
            {
                if (line.Contains(code))
                {
                    return code;
                }
            }

            return string.Empty;
        }


        private async Task Evaluate() // to evaluate line by line.
        {
            if(Lines.Count < 1) // if there is no lines at all we dont evaluate. just in case
            {
                return;
            }
            string[] cmds; //list of syntaxes in script
            Stopwatch sW = new Stopwatch();
            sW.Start();

                foreach (string line in Lines)
                {
                    cmds = Utility.TokenizeString(line); // we first tokenize before we evaluate
                    string synt = whichSyntax(cmds[0]); // grab the syntax in the line.


                    if (synt.ToLower() == "print") //output
                    {
                        string tmp = cmds[0].Remove(0, 6).Trim(')');

                        if (hasVariable(tmp))
                        {
                            tmp = Utility.ReplaceWords(tmp, varin.ToArray(), Variable, '$');
                        }

                        print(tmp);
                        continue;
                    }
                    else if (synt.ToLower() == "input") //input
                    {
                        string varName = cmds[0].Remove(0, 6).Trim(')');

                        Variable[varName] = await InputAsync();

                        continue;
                    }
                    else if (synt.ToLower() == "var")
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
                    else //if anything is wrong by ONE bit, we break evaluation.
                    {
                        print($"Syntax: {cmds[0]} is not valid!", true);
                        sW.Stop();
                        seconds = (int)sW.ElapsedMilliseconds;
                        return;
                    }
                }

            sW.Stop();
            seconds = (int)sW.ElapsedMilliseconds;
            success = true; // Finally we only return true once the script is done evaluating

        }
    }
}
