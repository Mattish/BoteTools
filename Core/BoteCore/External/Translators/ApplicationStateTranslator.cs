using System.Collections.Generic;

namespace BoteCore.External.Translators
{
    public static class ApplicationStateTranslator
    {
        public static ApplicationStateDto ToDto(ApplicationState state)
        {
            return new ApplicationStateDto
            {
                AssemblyVersion = state.AssemblyVersion,
                RootDirectory = ApplicationDirectoryStateTranslator.ToDto(state.RootDirectory)
            };
        }

        public static ApplicationState ToModel(ApplicationStateDto dto)
        {
            List<ApplicationFileState> files;
            var ads = ApplicationDirectoryStateTranslator.ToModel(dto.RootDirectory, out files);
            return new ApplicationState(
                dto.AssemblyVersion,
                files,
                ads
                );
        }
    }
}