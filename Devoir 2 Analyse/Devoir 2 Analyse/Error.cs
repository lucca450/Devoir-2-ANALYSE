using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Devoir_2_Analyse
{
    public class Error
    {
        public readonly string errorOrigin = "", expected = "";
        private readonly ErrorType errorType;
        public Error(ErrorType errorType, string errorOrigin, string expected = "")
        {
            this.errorType = errorType;
            this.errorOrigin = errorOrigin;
            this.expected = expected;
        }

        public void DisplayError()
        {
            MyConsole.DisplayError(this);
        }

        public string GetErrorText()
        {
            switch (true)
            {
                case true when errorType == ErrorType.MissingCaracter:
                    return "Un caractère est manquant";
                case true when errorType == ErrorType.UndeclaredVariable:
                    return "Une variable n'est pas declaree";
                case true when errorType == ErrorType.WrongIdenFormat:
                    return "Mauvais format d'identificateur";
                case true when errorType == ErrorType.WrongKeyword:
                    return "Mauvais mot clef";
                case true when errorType == ErrorType.WrongType:
                    return "Mauvais type de variable";
                case true when errorType == ErrorType.ExpectedKeyword:
                    return "Mot clef attendu";
                case true when errorType == ErrorType.DuplicateKeyword:
                    return "Mot clef dublique";
                case true when errorType == ErrorType.WrongDeclarationFormat:
                    return "Mauvais format de declaration";
                case true when errorType == ErrorType.CantUseReservedKeywords:
                    return "Vous ne pouvez utiliser de mots clefs reserves";
                case true when errorType == ErrorType.VariableNotDeclared:
                    return "Aucune variable déclarée";
            }
            return "";
        }
    }
}
