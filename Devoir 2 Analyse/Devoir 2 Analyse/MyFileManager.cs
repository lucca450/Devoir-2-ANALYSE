using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Devoir_2_Analyse
{
    static class MyFileManager
    {
        public static string FormatPath(string fileName)
        {
            return string.Format(@".\{0}.txt", fileName);                       //  Formatage du chemin du fichier 
        }
        public static bool FileExists(string path)
        {
            return File.Exists(path);                                           //  Si le fichier existe
        }

        public static string ReadFromFile(string fileName)                           //  Lecture de la grammaire du fichier
        {
            StreamReader sr = new StreamReader(FormatPath(fileName));
            
            string readToEnd = sr.ReadToEnd();

            sr.Close();

            return readToEnd;
        }
    }
}
