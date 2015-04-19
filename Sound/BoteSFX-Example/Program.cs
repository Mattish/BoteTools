using System;
using BoteSFX.Wwise;

namespace BoteSFX_Example
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 3)
            {
                IBnkFile bnkFile = BnkFile.Create(args[0]);
                var numberToReplace = uint.Parse(args[1]);
                bnkFile.ReplaceWemFile(numberToReplace, args[2]);
                bnkFile.WriteToFile("NEW_FILE.bnk");
                Console.ReadKey();
                Console.WriteLine("Done.");
            }
            else
            {
                Console.WriteLine("Usage: BoteSFX-Example <Original .BNK> <File Number To Replace> <.WEM To replace with>");
                Console.WriteLine("Example: BoteSFX-Example WOWS_UI.BNK 12345678 something.wem");
            }
        }
    }
}
