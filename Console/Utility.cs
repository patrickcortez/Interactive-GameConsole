using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console
{
    internal static class Utility
    {


        public static string[] TokenizeString(string input) // this is our new tokenizer, that handles qoutes by using a boolean to check each character for qoutes
        {
            List<string> Tokens = new List<string>(); // we store each token we find in the tokenization loop
            StringBuilder word = new StringBuilder();
            bool isQoutes = false;
            
            foreach(char c in input) // Our Tokenization loop
            {
                if(c == '"') // if we encounter a qoute we trigger our isQoutes by reversing its value then if we reach an end qoute it beautifully sets it to false =D recording the entire string
                {
                    isQoutes = !isQoutes;
                    continue;
                }
                
                if(c == ' ' && !isQoutes) // once we reach an empty space that isnt a qoute we finally add the 'word' to our list and clear the string builder
                { //this is also extensible for later
                    if(word.Length > 0) // our guard clause 
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

            if(word.Length > 0) // final check for the last token
            {
                Tokens.Add(word.ToString());
            }

            return Tokens.ToArray(); //Finally return the entire list
        }
    }
}
