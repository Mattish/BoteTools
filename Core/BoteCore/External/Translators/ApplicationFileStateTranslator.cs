namespace BoteCore.External.Translators
{
    public static class ApplicationFileStateTranslator
    {
        public static ApplicationFileStateDto ToDto(ApplicationFileState state)
        {
            return new ApplicationFileStateDto
            {
                ApplicationFileType = state.Type.ToString().ToLower(),
                Hash = state.Hash,
                RelativePath = state.RelativePath
            };
        }

        public static ApplicationFileState ToModel(ApplicationFileStateDto dto)
        {
            return new ApplicationFileState(dto.RelativePath, dto.ApplicationFileType.FromString(), dto.Hash);
        }
    }
}