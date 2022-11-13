using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParameterParsing
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("converting");
            ConvertiLogicFormSpec.ConvertiLogicXaml();
            Console.ReadLine();
        }
    }
}
