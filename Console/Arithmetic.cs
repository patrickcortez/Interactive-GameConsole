using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Windows.Forms;

/*
 * This is the basic arithmetic hanlder of Console Explorer
 * So i can use it in the interpreter and so that the user can properly do math.
 * I mean they probably wont but why not =D.
 */

namespace Console
{

   
    internal class Arithmetic : IDisposable // The start of our beautiful math handler =D
    { // ngl I wish there was an auto arithmetic class in c#

        private int pos;
        private string pdata;
        List<Tokens> tokens = new List<Tokens>();
        enum MathTypes //we define all the types in math
        {
            Number,
            Plus,
            Minus,
            Multiply,
            Divide,
            LPar,
            RPar,
            End // YEP i am not doing Power!
        }

        public void Dispose()
        {
            tokens.Clear();
            pos = 0;
            pdata = string.Empty;
        }

        private struct Tokens // token definition: Type and value(if its a number)
        {
           public MathTypes type;
           public int value; //yep i dont care, I AM NOT DOING DECIMALS NAH!
            

            public Tokens(MathTypes ntype,int nvalue = 0) // for convinience
            {
                type = ntype;
                value = nvalue;
            }
        }

        private char current() // Our position tracker
        {
            if(pos >= pdata.Length) //if we are way passed the end we return a null value \0
            {
                return '\0';
            }

            return pdata[pos]; //else we return the current character in the string
        }

        private void advance() // we should only advance as long as its lesser than the size of the string
        {
            if(pos < pdata.Length) // if our current position is lesser than the size of the string
            {
                pos++; //then advance
            }
        }

        private int ReadNumbers() // read all numbers in the string only to stop if its not a number
        {
            int start = pos;

            while (char.IsDigit(current())) //advance until we hit any arithmetic operator
            {
                advance();
            }


            return Utility.Strint(pdata.Substring(start,pos - start)); //finally Convert then return =D
        }

        private void tokenize()
        {
            while (pos < pdata.Length)
            {
                if (char.IsDigit(pdata[pos])) //we read all of the digits and only advance if we hit a non digit
                {
                    tokens.Add(new Tokens(MathTypes.Number, ReadNumbers()));
                }
                else if (pdata[pos] == '+') //addition
                {
                    tokens.Add(new Tokens(MathTypes.Plus));
                    advance();
                }
                else if (pdata[pos] == '-')
                {

                    if (tokens[pos-1].type == MathTypes.LPar || // for negative numbers: (-n,n * -n,n + -n or n - -n)
                    tokens[pos-1].type == MathTypes.Minus ||
                    tokens[pos-1].type == MathTypes.Plus ||
                    tokens[pos-1].type == MathTypes.Minus)
                    {
                        tokens.Add(new Tokens(MathTypes.Number, ReadNumbers()));
                    }
                    else // if finally its not negative then we add the minus sign as a token.
                    {
                        tokens.Add(new Tokens(MathTypes.Minus));
                        advance();
                    }
                }
                else if (pdata[pos] == '*') // multiplication
                {
                    tokens.Add(new Tokens(MathTypes.Multiply));
                    advance();
                }
                else if (pdata[pos] == '/') //division
                {
                    tokens.Add(new Tokens(MathTypes.Divide));
                    advance();
                }else if (pdata[pos] == '(')
                {
                    tokens.Add(new Tokens(MathTypes.LPar));
                    advance();
                }
                else if (pdata[pos] == ')')
                {
                    tokens.Add(new Tokens(MathTypes.RPar));
                    advance();
                }
                else // strictly no letters or other special characters asides from: +,-,*,/,( or ).
                {
                    MessageBox.Show($"Unknown Token:{current()} ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); // if the current token is a letter or an unknown special character we stop execution
                    return;
                }
            }

            pos = 0; // reset our position after its done
            tokens.Add(new Tokens(MathTypes.End)); 
        }

        //parser

        private Tokens currentToken() { // our navigator, which is the main driver for our engine, at start in points to the first token in the list(tokens)
            return  tokens[pos];
        }
        private int Parse() // starts processing user expression and traversing our method ladder.
        {

            return ParseAdd();
        }

        private int ParseAdd() // Add/Sub handler, basically a ladder to parseBase()
        {
            int valueL = ParseMultiply();

            while(currentToken().type == MathTypes.Minus || currentToken().type == MathTypes.Plus) // we recursively add/sub here until we dont have any + - tokens
            {
                MathTypes ops = currentToken().type;
                advance();
                int valueR = ParseMultiply();

                if (ops == MathTypes.Plus) { 
                    valueL = valueL + valueR;
                }
                else
                {
                    valueR = valueL - valueR;
                }
            }

            return valueL;
        }

        private int ParseMultiply() // Multiplication handler based in PEMDAS, before passing onto Add, check if there are any * operators
        {
            int valueL = ParseBase();
            
            while(currentToken().type == MathTypes.Multiply || currentToken().type == MathTypes.Divide) //While its * or /, solve then advance until the 
            {
                MathTypes ops = currentToken().type;
                advance();
                int valueR = ParseBase();

                if(ops == MathTypes.Multiply)
                {
                    valueL = valueL * valueR;
                }
                else
                {
                    valueL = valueL / valueR;
                }
            }

            return valueL;
        }

        //Evaluator

        private int ParseBase() // the determiner if a number is under a parenthesis or its a negative or a non negative number by  analyzing each current token type
        {
            if(currentToken().type == MathTypes.Number)
            {
                int value = currentToken().value;
                advance();
                return value;
            }
            
            if (currentToken().type == MathTypes.LPar) // if the current token is a left parenthises, advance then we recurse
            {
                advance();
                int value = Parse();

                if(currentToken().type != MathTypes.RPar)
                {
                    MessageBox.Show("Expression is missing a )", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return 0;
                }

                advance(); //skip the right parenthesis then returb the value
                return value;
            }
            
            if(currentToken().type == MathTypes.Minus) // if its a minus token just return it as negative
            {
                advance();
                return -ParseBase();
            }

            MessageBox.Show("Unknown token in expression!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return 0;
        }

        public int Begin() // Start the math
        {
            int res = Parse();

            if (currentToken().type != MathTypes.End) // if the current token isnt an end like e.g: *, (, - or +. We throw an error, i mean we could ignore but thats lazy
            {
                MessageBox.Show($"Invalid token in end of Expression: {currentToken().value}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }

            return res;
        }

        public Arithmetic(string data)
        {
            pos = 0;
            pdata = data.Replace(" ","");
            tokenize();
        }
    }
}
