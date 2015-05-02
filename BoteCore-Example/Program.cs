using System;
using System.IO;
using BoteCore;
using BoteCore.External;
using BoteCore.External.Translators;
using Newtonsoft.Json;

namespace BoteCore_Example
{
    class Program
    {
        static void Main(string[] args)
        {
            var assem = ApplicationState.Create(@"C:\Games\WoWarships\");
            var translated = ApplicationStateTranslator.ToDto(assem);
            var serializeObject = JsonConvert.SerializeObject(translated, new JsonSerializerSettings()
            {
                ContractResolver = ShouldSerializeContractResolver.Instance,
                NullValueHandling = NullValueHandling.Ignore
            });
            File.WriteAllText("ApplicationState.json", serializeObject);
            var obj = JsonConvert.DeserializeObject<ApplicationStateDto>(serializeObject);
            var itsBack = ApplicationStateTranslator.ToModel(obj);
            Console.WriteLine("Done");

            Console.ReadKey();
        }
    }
}
