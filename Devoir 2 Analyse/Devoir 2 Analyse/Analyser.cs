using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Devoir_2_Analyse
{
    struct Variable
    {
        string name;
        string type;

        public Variable(string name, string type)
        {
            this.name = name;
            this.type = type;
        }
    }


    class Analyser
    {
        private List<Error> errors;
        private List<Variable> variables;
        private string code;
        private List<string> lines;
        private List<string> keywords = new List<string>() { "Procedure", "declare", "entier", "reel", "Fin_Procedure" };
            
        public Analyser(string code)
        {
            code = code.Replace("\r\n", " ");
            this.code = code.Replace('\n', ' ');
            lines = new List<string>();
            errors = new List<Error>();
            variables = new List<Variable>();
        }

        public List<Error> CheckCode()
        {
            List<string> foundKeyWords = ReadProgram();

            return errors;
        }

        private void CheckKeywords(List<string> keywords)
        {
            if(keywords[0] != "Procedure")
            {
                errors.Add(new Error(ErrorType.ExpectedKeyword, keywords[0], "Procedure"));
            }
            if(keywords[keywords.Count()-1] != "Fin_Procedure")
            {
                errors.Add(new Error(ErrorType.ExpectedKeyword, keywords[keywords.Count() - 1], "Fin_Procedure"));

            }
        }

        private bool CheckDeclarationFormat(string codeBetween)
        {
            if (Regex.IsMatch(codeBetween, "(declare +[A-Za-z]+ *:{1} *reel *; +|declare +[A-Za-z]+ *:{1} *entier *; +)$"))
            {
                return true;
            }
            else
            {
                errors.Add(new Error(ErrorType.WrongDeclarationFormat, codeBetween));
                return false;
            }
        }

        private bool CheckIdenFormat(string identificator)
        {
            if (!Regex.IsMatch(identificator, "([A-Za-z]{1}[a-zA-Z0-9]{0,7})$"))
            {
                errors.Add(new Error(ErrorType.WrongIdenFormat, identificator));
                return false;
            }
            if (!keywords.Contains(identificator))
            {
                errors.Add(new Error(ErrorType.CantUseReservedKeywords, identificator));
                return false;
            }
            return true;
        }

        private string GetCodeBetweenTwoKeywords(string kw, string nextKw)
        {
            code = code.Substring(kw.Length, code.Length - (kw.Length));
            int idx = code.IndexOf(nextKw);

            string codeBetween = code.Substring(0, idx);
            code = code.Substring(codeBetween.Length, code.Length - codeBetween.Length);
            return codeBetween;
        }


        private List<string> ReadProgram()
        {
            int i = 0;
            string nomProcedure = "";


            bool lfProcedure = true;
            bool foundBeginning = false;
            bool lfnom = false;
            bool lfdeclare = false;
            bool foundDeclare = false;
            bool lfvariable = false;
            bool lfcolon = false;
            bool lftype = false;
            bool lfsemicolon = false;
            bool lfvariableaffectation = false;
            bool lfsymbole = false;
            bool lfterm = false;
            bool lfoperator = false;
            bool lfFinProc = false;

            string word = "";
            foreach(char c in code)
            {
                switch (true)
                {
                    case true when lfProcedure:
                        if (!foundBeginning)
                        {
                            if(c != ' ')
                            {
                                foundBeginning = true;
                                word += c;
                            }
                        }
                        else
                        {
                            word += c;
                        }

                        if (word.Length == "Procedure".Length)
                        {
                            if (word == "Procedure")
                            {
                                lfProcedure = false;
                                lfnom = true;
                                foundBeginning = false;
                                word = "";
                            }
                            else
                            {
                                errors.Add(new Error(ErrorType.ExpectedKeyword, word, "Procedure"));
                                word = "";
                                lfProcedure = false;
                                lfdeclare = true;
                            }
                        }

                        break;

                    case true when lfnom:
                        if (!foundBeginning)
                        {
                            if (c != ' ')
                            {
                                foundBeginning = true;
                                word += c;
                            }
                        }
                        else
                        {
                            if(c == ' ')
                            {
                                nomProcedure = word;
                                word = "";
                                lfnom = false;
                                lfdeclare = true;
                                foundBeginning = false;
                                lines.Add(nomProcedure);
                            }
                            else
                            {
                                word += c;
                            }
                        }
                        break;
                    case true when lfdeclare:
                        if (!foundBeginning)
                        {
                            if (c != ' ')
                            {
                                foundBeginning = true;
                                word += c;
                            }
                        }
                        else
                        {
                            if (c == ' ')
                            {
                                word = "";
                                lfdeclare = false;
                                foundBeginning = false;
                            }
                            else
                            {
                                word += c;
                            }

                            if (word.Length == "declare".Length)
                            {
                                if (word == "declare")
                                {
                                    lfdeclare = false;
                                    lfnom = true;
                                    foundBeginning = false;
                                    word = "";
                                    foundDeclare = true;
                                }
                                else
                                {
                                    if (!foundDeclare)
                                    {
                                        errors.Add(new Error(ErrorType.ExpectedKeyword, word, "declare"));
                                        word = "";
                                    }
                                    else
                                    {
                                        lfdeclare = false;
                                        lfterm = true;
                                    }
                                }
                            }
                        }
                        break;

                    case true when lfvariable:
                        if (!foundBeginning)
                        {
                            if (c != ' ')
                            {
                                foundBeginning = true;
                                word += c;
                            }
                        }
                        else
                        {
                            if (c == ':')
                            {
                                string var = word;
                                word = "";
                                lfvariable = false;
                                lftype = true;
                                foundBeginning = false;
                                lines.Add(var);
                            }
                            else
                            {
                                word += c;
                            }
                        }
                        break;
                    case true when lftype:
                        if (!foundBeginning)
                        {
                            if (c != ' ')
                            {
                                foundBeginning = true;
                                word += c;
                            }
                        }
                        else
                        {
                            if (c == ';')
                            {
                                string type = word;
                                word = "";
                                lftype = false;
                                lfdeclare = true;
                                foundBeginning = false;
                                lines.Add(type);
                            }
                            else
                            {
                                word += c;
                            }
                        }
                        break;
                    case true when lfterm:
                        if (!foundBeginning)
                        {
                            if (c != ' ')
                            {
                                foundBeginning = true;
                                word += c;
                            }
                        }
                        else
                        {
                            if (c == '=')
                            {
                                string variable = word;
                                word = "";
                                lfterm = true;                                      // lf term encore pour trouver le permier terme l'autre bord de du =
                                foundBeginning = false;
                                lines.Add(variable);
                            }
                            else
                            {
                                word += c;
                            }
                        }
                        break;

                }

            }
            return null;
        }

    }
}
