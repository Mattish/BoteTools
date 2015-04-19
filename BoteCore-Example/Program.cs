using System;
using BoteCore;

namespace BoteCore_Example
{
    class Program
    {
        static void Main(string[] args)
        {
            var assem = BoteApplication.Create(@"C:\Games\WoWarships\");
            Console.ReadKey();
        }
    }
}
