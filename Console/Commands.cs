using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Console
{
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

        /*
         * Mental Map:
         *  methods index map:
         *  0: print() - for output in rtb_output
         * 
         */

        public Commands(List<Delegate> mtmps,bool exec, string[] inpargs) // Command Constructor, This is where we initialize our list of methods and input
        {
            //initiate our  string array and method list
            methods = mtmps; // for passing methods frim main to here incase some commands use private methods in main.
            hasexec = exec;
            cmds = inpargs; //tokenize our input for functions that need user input
        }


        public void Test()
        {
            ((Action<string>)methods[0])("Test");
            hasexec = true;
        }

        public void TestInput()
        {
            try
            {
                string output = string.Join(" ", cmds.Skip(1));
                ((Action<string>)methods[0])(output);
                hasexec = true;
            }catch(Exception ex)
            {
                ((Action<string>)methods[0])("Error: " + ex.Message);
            }
        }

        internal bool getexec()
        {
            return hasexec;
        }

        
      

    }
}
