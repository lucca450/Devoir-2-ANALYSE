using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Devoir_2_Analyse
{
    class Program
    {
        static void Main(string[] args)
        {
            string fileName = MyConsole.AskFileName();

            string code = MyFileManager.ReadFromFile(fileName);

            Analyser analyser = new Analyser(code);

            List<Error> errors = analyser.CheckCode();

            if(errors.Count != 0)
            {
                foreach(Error error in errors)
                {
                    error.DisplayError();
                }
            }
            else
            {
                MyConsole.NoErrorFound();
            }
            Console.ReadLine();
        }
    }
}
