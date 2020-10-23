using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Devoir_2_Analyse
{
    class Data
    {
        public string firstIden;
        public List<Variable> variables;
        public string lastIden;

        public Data()
        {
            firstIden = "";
            lastIden = "";
            variables = new List<Variable>();
        }
        public Variable ContainsVariableName(string name)
        {
            foreach(Variable var in variables)
            {
                if (var.name.Equals(name))
                {
                    return var;
                }
            }
            return null;
        }
    }

    class Variable
    {
        public string name;
        public string type;
        public string value;

        public Variable(string name, string type)
        {
            this.name = name;
            this.type = type;
        }
    }


    class Analyser
    {
        private List<Error> errors;
        private string code;
        private List<string> lines;
        private List<string> keywords = new List<string>() { "Procedure", "declare", "entier", "reel", "Fin_Procedure" };
        private Data data = new Data();
            
        public Analyser(string code)
        {
            code = code.Replace("\r\n", " ");
            this.code = code.Replace('\n', ' ');
            lines = new List<string>();
            errors = new List<Error>();
        }

        public List<Error> CheckCode()
        {
            VerifyProgram();

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

        private string GetCodeBetweenTwoKeywords(string kw, string nextKw)
        {
            code = code.Substring(kw.Length, code.Length - (kw.Length));
            int idx = code.IndexOf(nextKw);

            string codeBetween = code.Substring(0, idx);
            code = code.Substring(codeBetween.Length, code.Length - codeBetween.Length);
            return codeBetween;
        }


        private void VerifyProgram()
        {
            bool lfProcedure = true;
            bool foundBeginning = false;
            bool lfnom = false;
            bool lfdeclare = false;
            bool foundDeclare = false;
            bool lfvariabletype = false;
            bool lfterm = false;
            bool lfFinProc = false;

            string word = "";
            foreach(char c in code)
            {
                switch (true)
                {
                    case true when lfProcedure:
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
                            if (c == ' ')
                            {
                                lfnom = false;
                                lfdeclare = true;
                                foundBeginning = false;
                                Verify(word, "identificator");
                                //lines.Add(word);
                                word = "";
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
                                if (c != 'd')
                                {
                                    lfdeclare = false;
                                    lfterm = true;
                                    word += c;
                                    break;
                                }
                                
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

                            if(word.Length == "declare".Length)
                            {
                                if (word == "declare")
                                {
                                    lfdeclare = false;
                                    lfvariabletype = true;
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
                    case true when lfvariabletype:
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
                                lfvariabletype = false;
                                lfdeclare = true;
                                foundBeginning = false;
                                Verify(word + ";", "declaration");
                                //lines.Add(word + ";");
                                word = "";
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
                            if(word.Contains("Fin_"))
                            {
                                string line = word.Replace("Fin_", "");
                                Verify(line, "assignation", true);
                                lines.Add(line);
                                word = "Fin_";
                                lfFinProc = true;
                                lfterm = false;
                            }
                            if(c == ';')
                            {
                                foundBeginning = false;
                                lfterm = true;
                                Verify(word + ";", "assignation");
                                lines.Add(word + ";");
                                word = "";
                                break;
                            }
                            if (c == '=')
                            {
                                word += c;
                                lfterm = true;
                                foundBeginning = false;
                            }
                            else
                            {
                                if (c == '+' || c == '-' || c == '*' || c == '/')
                                {
                                    lfterm = true;
                                    foundBeginning = false;
                                    word += c;
                                }
                                else
                                    word += c;
                            }
                        }
                        break;

                    case true when lfFinProc:
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
                            word += c;
                        }

                        if (word.Length == "Fin_Procedure".Length)
                        {
                            if (word == "Fin_Procedure")
                            {
                                lfFinProc = false;
                                lfnom = true;
                                foundBeginning = false;
                                word = "";
                            }
                            else
                            {
                                errors.Add(new Error(ErrorType.ExpectedKeyword, word, "Fin_Procedure"));
                                word = "";
                                lfFinProc = false;
                            }
                        }

                        break;
                }
            }
            //lines.Add(word);
            Verify(word, "identification", true);
            word = "";
        }

        private void Verify(string word, string type, bool isLast = false)
        {
            switch (true)
            {
                case true when type == "identificator":

                    if (!Regex.IsMatch(word, "([A-Za-z]{1}[a-zA-Z0-9]{0,7})$"))
                        errors.Add(new Error(ErrorType.WrongIdenFormat, word));
                    if (keywords.Contains(word))
                        errors.Add(new Error(ErrorType.CantUseReservedKeywords, word));

                    if (isLast)
                    {
                        if (data.firstIden.Equals("word"))
                            data.lastIden = word;
                        else
                            errors.Add(new Error(ErrorType.WrongIdenFormat, word));
                    }
                    else
                    {
                        data.firstIden = word;
                    }
                    break;
                case true when type == "declaration":
                    if (Regex.IsMatch(word, "( *[A-Za-z]+ *:{1} *reel *; *| *[A-Za-z]+ *:{1} *entier *; *)$"))
                    {
                        string variable = word.Trim().Substring(0, word.IndexOf(":")).Trim();
                        string varType = word.Trim().Substring(word.IndexOf(":") +1 , (word.Length - (word.Length - word.IndexOf(";")))-2 ).Trim();
                        //string varType = word.Trim().Substring(word.IndexOf(":", word.Length - word.IndexOf(":"))).Trim();

                        Variable var = new Variable(variable, varType);

                        if (data.variables.Contains(var))
                        {
                            errors.Add(new Error(ErrorType.AlreadyDeclaredVariable, word));
                        }
                        else
                        {
                            data.variables.Add(var);
                        }
                    }
                    else
                    {
                        errors.Add(new Error(ErrorType.WrongDeclarationFormat, word));
                    }
                    break;
                case true when type == "assignation":
                    if(word.Split('(').Length == word.Split(')').Length)
                    {
                        word = word.Replace(" ", "");

                        BinaryTree tree = new BinaryTree();

                        BuildTree(tree, word);

                        double result = Evaluate(tree.getHead());

                        string name = word.Split('=')[0];

                        Variable assignedVar = data.ContainsVariableName(name);

                        int parsedResult;

                        if(assignedVar.type == "entier")
                        {
                            if(int.TryParse(result.ToString(), out parsedResult))
                            {
                                assignedVar.value = parsedResult.ToString();
                            }
                            else
                            {
                                errors.Add(new Error(ErrorType.WrongType, assignedVar.name, "reel"));
                            }
                        }
                        else
                        {
                            assignedVar.value = result.ToString();
                        }

                        // Vérifier dans le programme.txt assignations à chiffre à virgule
                        // ; à la dernière assignation

                        int i = 0;


                        /*
                         
                        Retirer les espaces avant
                   
                         A=A+A;

                        ([A-Za-z]{1}[a-zA-Z0-9]{0,7}=[A-Za-z]{1}[a-zA-Z0-9]{0,7}[+/-]{1}[A-Za-z]{1}[a-zA-Z0-9]{0,7};)

                        A=A+10;

                        

                        A=10+a;

                        

                        a=10;

                        


                         */
                    }
                    else
                    {
                        errors.Add(new Error(ErrorType.WrongAssignationFortmat, word, "( ou )"));
                    }
                    break;
            }
        }

        private void BuildTree(BinaryTree tree, string expression)
        {
            expression = expression.Substring(expression.IndexOf('=') + 1, expression.Length - expression.IndexOf('=') -1);

            string[] operators = { "-", "+", "*", "/" };

            int nbOp = expression.ToCharArray().Count(c => c == '/' || c == '*' || c == '+' || c == '-');
            int nbFoundOp = 0;
            MatchCollection mc = Regex.Matches(expression.Replace(";", ""), @"([*+/\-)(])|([0-9A-Za-z]+)");

            foreach (var i in mc)
            {
                if (operators.Contains(i.ToString()))
                {
                    tree.setCurrentValue(i.ToString());
                    tree.InsertRight("");
                    nbFoundOp++;
                }
                else if (!operators.Contains(i.ToString()) && i.ToString() != "(" && i.ToString() != ")")
                {
                    if(nbFoundOp != nbOp)
                    {
                        tree.InsertLeft(i.ToString());
                    }
                    else
                    {
                        tree.setCurrentValue(i.ToString());
                    }
                }
            }
        } 
        private double Evaluate(Node node)
        {
            if(node.leftChild != null && node.rightChild != null)
            {
                switch (true)
                {
                    case true when node.value == "+":
                        return Evaluate(node.leftChild) + Evaluate(node.rightChild);
                    case true when node.value == "-":
                        return Evaluate(node.leftChild) - Evaluate(node.rightChild);
                    case true when node.value == "*":
                        return Evaluate(node.leftChild) * Evaluate(node.rightChild);
                    case true when node.value == "/":
                        return Evaluate(node.leftChild) / Evaluate(node.rightChild);
                }
                return -999999;
            }
            else
            {
                Variable temp = data.ContainsVariableName(node.value.ToString());
                if (temp != null)
                {
                    return double.Parse(temp.value);
                }
                else
                {
                    return double.Parse(node.value);
                }
            }    
        }
    }
}
