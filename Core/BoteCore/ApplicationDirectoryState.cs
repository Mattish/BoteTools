using System.Collections.Generic;

namespace BoteCore
{
    public class ApplicationDirectoryState
    {
        public string RelativePath { get; private set; }
        public ApplicationDirectoryState Parent { get; private set; }
        public List<ApplicationDirectoryState> ChildDirectories { get; private set; }
        public List<ApplicationFileState> Files { get; private set; }

        internal ApplicationDirectoryState(string relativePath, ApplicationDirectoryState parent)
        {
            RelativePath = relativePath;
            ChildDirectories = new List<ApplicationDirectoryState>();
            Files = new List<ApplicationFileState>();
        }

        internal void AddDirectory(ApplicationDirectoryState state)
        {
            ChildDirectories.Add(state);
        }

        internal void AddFile(ApplicationFileState file)
        {
            Files.Add(file);
        }

        internal void AddFiles(ApplicationFileState[] files)
        {
            Files.AddRange(files);
        }

        public override string ToString()
        {
            return RelativePath;
        }
    }
}