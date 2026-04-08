using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Windows.Forms;

/*
 * This is the arithmetic hanlder of Console Explorer
 * So i can use it in the interpreter and so that the user can properly do math.
 * I mean they probably wont but why not =D.
 * We dont have any decimals yet, but soon probably.
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
            Number, //1 - 9
            Plus, // addition
            Minus, //Sub or negative
            Multiply, // multiplication
            Divide, //division
            LPar, // left or starting parenthesis
            RPar, // right or ending parenthesis
            Power, // N^n power handling with Math
            End // signify an expression has a valid ending and doesnt end with other token types
        }

        public void Dispose() // to properly dispose of the lists we used
        {
            tokens.Clear();
            pos = 0;
            pdata = string.Empty;
        }

        private struct Tokens // token definition: Type and value(if its a number)
        {
           public MathTypes type;
           public int value; //We will use simple integers for now, no decimals yet.
            

            public Tokens(MathTypes ntype,int nvalue = 0) // constructor, so we can easily add to our list: List.Add(new Tokens(n,n))
            {
                type = ntype; // Token types
                value = nvalue; // value if token is number
            }
        }

        private char current() // Our position tracker
        {
            if(pos >= pdata.Length) //if we are way passed the end we return a null value \0
            {
                return '\0';
            }

            return pdata[pos]; //we return to keep tracking...
        }

        private void advance() // we should only advance as long as its lesser than the size of the string
        {
            if(pos < pdata.Length) // if our current position is lesser than the size of the string
            {
                pos++; //then advance position to next char
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
                    advance(); // always advance after its not a digit
                }
                else if (pdata[pos] == '-') // subtraction, removed unnecessary negative number handling, as we already handle it in parsebase() -_-
                {
                        tokens.Add(new Tokens(MathTypes.Minus));
                        advance();
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
                    tokens.Add(new Tokens(MathTypes.LPar)); // starting parenthesis
                    advance();
                }
                else if (pdata[pos] == ')')
                {
                    tokens.Add(new Tokens(MathTypes.RPar)); // closing
                    advance();
                }else if (pdata[pos] == '^')
                {
                    tokens.Add(new Tokens(MathTypes.Power));
                    advance();
                }
                else // strictly no letters or other special characters asides from: +,-,*,/,( or ).
                {
                    throw new Exception($"Unknown Token:{current()} "); // if the current token is a letter or an unknown special character we throw an error
                }
            }

            pos = 0; // reset our position after its done
            tokens.Add(new Tokens(MathTypes.End)); 
        }

        //parser

        private Tokens currentToken() { // our navigator, which is the main driver for our engine, at start in points to the first token in the list(tokens)
            return  tokens[pos];
        }
        private int Parse() // starts processing user expression and recursively parse
        {

            return ParseAdd();
        }

        private int ParseAdd() // Add/Sub handler, basically a ladder to parseBase() aka our recursing parser
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
                    valueL = valueL - valueR;
                }
            }

            return valueL;
        }

        private int ParseMultiply() // Multiplication handler based in PEMDAS, before passing onto Add, check if there are any * operators
        {
            int valueL = ParsePower();
            
            while(currentToken().type == MathTypes.Multiply || currentToken().type == MathTypes.Divide) //While its * or /, solve then advance until the 
            {
                MathTypes ops = currentToken().type;
                advance();
                int valueR = ParsePower();

                if(ops == MathTypes.Multiply)
                {
                    valueL = valueL * valueR;
                }
                else
                {
                    if(valueR == 0)
                    {
                        throw new Exception("Cannot be divided to 0!");
                    } 
                    valueL = valueL / valueR;
                }
            }

            return valueL;
        }

        private int ParsePower() // power handler
        {
            int valueL = ParseBase();

            while(currentToken().type == MathTypes.Power)
            {
                MathTypes ops = currentToken().type;
                advance();
                int valueR = ParsePower(); //exponent

                if(ops == MathTypes.Power)
                {
                    valueL = (int)Math.Pow(valueL, valueR);
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
                    throw new Exception("Expression is missing a )");
                }

                advance(); //skip the right parenthesis then returb the value
                return value;
            }
            
            if(currentToken().type == MathTypes.Minus) // if its a minus token just return it as negative
            {
                advance();
                return -ParseBase();
            }

            throw new Exception("Unknown token in expression!");
        }

        public int Begin() // Start the math
        {
            int res = Parse();

            if (currentToken().type != MathTypes.End) // if the current token isnt an end like e.g: *, (, - or +. We throw an error, i mean we could ignore but thats lazy
            {
                throw new Exception($"Invalid token in end of Expression: {currentToken().value}");
            }

            return res;
        }

        public Arithmetic(string data)
        {
            pos = 0;

            if(data == string.Empty)
            {
                throw new Exception("Expression cant be null");
            }

            pdata = data.Replace(" ","");
            tokenize();
        }
    }
}
