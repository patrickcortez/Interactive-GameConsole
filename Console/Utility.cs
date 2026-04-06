using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Console
{
    class Explorer //For future use
    {
        List<Explorer> NewExplore = new List<Explorer>();
        string dir_name;
        List<string> files = new List<string>();
        Explorer(string current, string[] Files)
        {
            dir_name = current;
            files = Files.ToList();
        }

        public void CreateNewInstance(string nextDir, string[] Files)
        {
            Explorer tmp = new Explorer(nextDir, Files);

            NewExplore.Add(tmp);
        }
    }

    internal static class Utility
    {
        /*
         * This class is purely for the utility of
         * COnsole Explorer which is why all methods are static in this class.
         * So the main stays clean.
         */


        public static void CopyDirectory(string source, string destination, bool ranOnce = true) // Recursive Copy of Directories
        {
            try
            {
                if (ranOnce)
                {
                    DialogResult res = MessageBox.Show("Are you sure you want to proceed?", "Info", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                    if (res == DialogResult.No)
                    {
                        return;
                    }
                }

                if (!Directory.Exists(source)) //if the source doesnt exist then we warn the user.
                {
                    return;
                }

                if (!Directory.Exists(destination)) //we automatically make every subdirectory in dest thats in source.
                {
                    Directory.CreateDirectory(destination);
                }




                var dirs = new DirectoryInfo(source); // we grab all the info in the source directory 

                foreach (FileInfo file in dirs.GetFiles()) // then copy all files and dir to dest
                {
                    string tarpath = Path.Combine(destination, file.Name);
                    file.CopyTo(tarpath, true);
                }

                foreach (DirectoryInfo subdir in dirs.GetDirectories())
                {
                    string ndir = Path.Combine(source, subdir.Name);
                    string ndes = Path.Combine(destination, subdir.Name);
                    CopyDirectory(ndir, ndes, false); //repeat, with the ranOnce flag turned to false =D
                }

            } catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public static string[] TokenizeString(string input) // this is our new tokenizer, that handles qoutes by using a boolean to check each character for qoutes
        {
            List<string> Tokens = new List<string>(); // we store each token we find in the tokenization loop
            StringBuilder word = new StringBuilder();
            bool isQoutes = false;

            foreach (char c in input) // Our Tokenization loop
            {
                if (c == '"') // if we encounter a qoute we trigger our isQoutes by reversing its value then if we reach an end qoute it beautifully sets it to false =D recording the entire string
                {
                    isQoutes = !isQoutes;
                    continue;
                }

                if (c == ' ' && !isQoutes) // once we reach an empty space that isnt a qoute we finally add the 'word' to our list and clear the string builder
                { //this is also extensible for later
                    if (word.Length > 0) // our guard clause 
                    {
                        Tokens.Add(word.ToString());
                        word.Clear();
                    }

                }
                else // extensible logic here
                {
                    word.Append(c);

                }

            }

            if (word.Length > 0) // final check for the last token
            {
                Tokens.Add(word.ToString());
            }

            return Tokens.ToArray(); //Finally return the entire list
        }

        public static char GetFlag(string option, params char[] prefix)
        {
            char flag = option.Trim(prefix)[0];

            return flag;
        }

        public static bool hasBraces(char inp)
        {
            char[] braces = { '{', '(', '[' };
            bool found = false;

            foreach (char c in braces)
            {
                if (inp == c)
                {
                    found = true;
                    break;
                }
            }

            return found;
        }

        public static char? matchBrace(char input)
        {
            Dictionary<char, char> Braces = new Dictionary<char, char>();
            Braces.Add('(', ')');
            Braces.Add('{', '}');
            Braces.Add('[', ']');


            if (!Braces.ContainsKey(input)) {
                return null;
            }

            return Braces[input];
        }

        public static bool hasUnwanted(string data)
        {
            if (data.Contains("NTUSER") == true || data.Contains("ntuser") == true)
            {
                return true;
            }

            return false;
        }

        public static string ReplaceWords(string data,string[] oldWords,Dictionary<string,object> newWords,char prefix)
        {
            List<string> nOldWords = new List<string>();

            foreach(var word in oldWords)
            {
                nOldWords.Add(string.Concat(prefix, word));
            }
            
            StringBuilder nString = new StringBuilder(data);

            foreach(var word in nOldWords)
            {
                if (nString.ToString().Contains(word))
                {
                    nString.Replace(word, newWords[word.Remove(0,1)].ToString());
                }
            }

            return nString.ToString();

        }

        


    }
}
