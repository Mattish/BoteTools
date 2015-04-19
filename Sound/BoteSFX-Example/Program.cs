using System;
using BoteSFX.Wwise;

namespace BoteSFX_Example
{
    class Program
    {
        static void Main()
        {
            IBnkFile bnkFile = BnkFile.Create(@"Content\WOWS_UI.bnk");
            bnkFile.ReplaceWemFile(955869274, @"Content\poiLoud_12E58A64.wem");
            bnkFile.ReplaceWemFile(676333621, @"Content\PoiIntro_12E58A64.wem");
            bnkFile.WriteToFile("WOWS_UI_NEW3.bnk");
            Console.ReadKey();
        }
    }
}
