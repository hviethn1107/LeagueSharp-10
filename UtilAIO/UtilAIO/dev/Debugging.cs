using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilAIO.dev
{
    internal class Debugging
    {
        internal static void Log(string part, string message)
        {
            Console.WriteLine("------------ Start ------------");
            Console.WriteLine(DateTime.Today.TimeOfDay + " :: " + part + " :: " + message);
            Console.WriteLine("------------ End ------------");
        }
    }
}
