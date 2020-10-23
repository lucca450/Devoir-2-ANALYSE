using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Devoir_2_Analyse
{
    public static class MyConsole
    {
        public static string AskFileName()
        {
            string fileName = "", path;
            bool done = false;
            while (!done)
            {
                Console.WriteLine("Entrez le nom du fichier: ");
                fileName = Console.ReadLine().Trim();                        //  Lecture du nom du fichier
                path = MyFileManager.FormatPath(fileName);                      //  Formatage du chemin du fichier

                done = MyFileManager.FileExists(path);                                   //  Vérification de l'existance du fichier
                         
                if(!done) 
                    Console.WriteLine("Aucun fichier trouvée.\n");

            }
            return fileName;
        }

        public static void DisplayError(Error error)
        {
            Console.WriteLine(error.GetErrorText());

            if (error.errorOrigin != "")
            {
                Console.Write("L'erreur viens de : " + error.errorOrigin);
                if(error.expected != "")
                {
                    Console.WriteLine(" | Expectation : " + error.expected);
                }
            }
        }

        public static void NoErrorFound()
        {
            Console.WriteLine("Aucune erreur n'a été trouvée");
        }
    }
}
