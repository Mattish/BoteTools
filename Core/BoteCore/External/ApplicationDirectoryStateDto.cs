namespace BoteCore.External
{
    public class ApplicationDirectoryStateDto
    {
        public string RelativePath { get; set; }
        public ApplicationDirectoryStateDto[] ChildDirectories { get; set; }
        public ApplicationFileStateDto[] Files { get; set; }
    }
}