using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace BoteCore.External
{
    public class ApplicationDirectoryStateDto
    {
        public string RelativePath { get; set; }
        public ApplicationDirectoryStateDto[] ChildDirectories { get; set; }
        public ApplicationFileStateDto[] Files { get; set; }
    }

    public class ShouldSerializeContractResolver : DefaultContractResolver
    {
        public static readonly ShouldSerializeContractResolver Instance = new ShouldSerializeContractResolver();

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);

            if (property.PropertyType == typeof(ApplicationFileStateDto[]) && property.PropertyName == "Files")
            {

                property.ShouldSerialize = instance =>
                {
                    if (instance is ApplicationDirectoryStateDto)
                        return ((ApplicationDirectoryStateDto)instance).Files.Length > 0;
                    return true;
                };
            }

            return property;
        }
    }
}