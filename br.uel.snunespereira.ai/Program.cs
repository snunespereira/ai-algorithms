using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using br.uel.snunespereira.ai.shared;
using br.uel.snunespereira.ai.algorithms.decisiontree;
using System.Text.RegularExpressions;

namespace br.uel.snunespereira.ai
{
    class Program
    {
        
        static void Main(string[] args)
        {
            StringBuilder presentation = new StringBuilder();
            string path = string.Empty;
            AlgorithmType method = AlgorithmType.Undefined;
            int value = 4;
            char start = 'N';
            string[] data;

            presentation.AppendLine("**********************************************************");
            presentation.AppendLine("**********************************************************");
            presentation.AppendLine("**             Inteligência Artificial para             **");
            presentation.AppendLine("**   identificação de atrasos em fábrica de software    **");
            presentation.AppendLine("**********************************************************");
            presentation.AppendLine("**********************************************************");
            presentation.AppendLine();
            presentation.AppendLine("----------------------------------------------------------");
            presentation.AppendLine("-- Sérgio Nunes Pereira, Helen Cristina de M. Senefonte --");
            presentation.AppendLine("----------------------------------------------------------");
            presentation.AppendLine();
            presentation.AppendLine("Departamento de Computação - Universidade Estadual de Londrina (UEL)");
            presentation.AppendLine("Caixa Postal 10.011 - CEP 86057-970 - Londrina - PR - Brasil");
            presentation.AppendLine("snunespereira@gmail.com, helen@uel.br");

            Console.WriteLine(presentation);

            // if there any arguments to the call
            if (args.Length == 2)
            {
                path = args[0];
                int.TryParse(args[1], out value);
            }

            // while path is empty
            while (path == string.Empty || !File.Exists(path))
            {
                Console.WriteLine("Choose the file path:");
                path = Console.ReadLine();

                // if the path is empty
                if (path.Trim() == string.Empty || !File.Exists(path))
                {
                    Console.WriteLine("Wrong path!");
                    Console.WriteLine();
                }
            }

            // while method is undefined
            while (value < 1 || value > 3)
            {
                Console.WriteLine();
                Console.WriteLine("Choose the learning method:");
                Console.WriteLine("1 - Decision Tree");
                Console.WriteLine("2 - Neural Network - Backpropagation");
                Console.WriteLine("3 - Support Vector Machine");

                int.TryParse(Console.ReadKey().KeyChar.ToString(), out value);

                // if the input is not 1, 2 or 3
                if (value < 1 || value > 3)
                {
                    Console.WriteLine();
                    Console.WriteLine("Wrong learning method!");
                    Console.WriteLine();
                }
            }
            
            // setting the selected algorithm
            method = (AlgorithmType)value;

            // show some details
            Console.WriteLine("Learning method: {0}", method.ToString());
            Console.WriteLine("File path: {0}", path);

            // reading and converting the data to a bi-dimentional object array
            data = File.ReadAllText(path, Encoding.GetEncoding("ISO-8859-1"))
                    .Split(new string[] { "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries);

            Console.WriteLine("Start execution? Y / N");
            start = Console.ReadKey().KeyChar;

            // if the user choose to start the process
            if (start == 'Y' || start == 'y')
            {
                switch (method)
                {
                    case AlgorithmType.DecisionTree:
                        Console.WriteLine(ID3.Execute(data));
                        break;
                }
            }

            Console.ReadKey();
        }
    }
}
