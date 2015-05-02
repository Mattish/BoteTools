using System;
using BoteCore;
using BoteCore.External.Translators;

namespace BoteCore_Example
{
    class Program
    {
        static void Main(string[] args)
        {
            var assem = ApplicationState.Create(@"C:\Games\WoWarships\");
            var translated = ApplicationStateTranslator.ToDto(assem);
            var retranslated = ApplicationStateTranslator.ToModel(translated);
            Console.ReadKey();
        }
    }
}
