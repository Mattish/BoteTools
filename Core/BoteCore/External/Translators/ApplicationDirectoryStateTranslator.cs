using System;
using System.Collections.Generic;
using System.Linq;

namespace BoteCore.External.Translators
{
    public static class ApplicationDirectoryStateTranslator
    {
        public static ApplicationDirectoryStateDto ToDto(ApplicationDirectoryState state)
        {
            return new ApplicationDirectoryStateDto
            {
                Files = state.Files.Count > 0 ? state.Files.Select(ApplicationFileStateTranslator.ToDto).ToArray() : new ApplicationFileStateDto[0],
                ChildDirectories = state.ChildDirectories.Count > 0 ? ToDtos(state.ChildDirectories) : new ApplicationDirectoryStateDto[0],
                RelativePath = state.RelativePath
            };
        }

        public static ApplicationDirectoryStateDto[] ToDtos(IEnumerable<ApplicationDirectoryState> states)
        {
            return states.Select(ToDto).ToArray();
        }

        public static ApplicationDirectoryState ToModel(ApplicationDirectoryStateDto rootDirectory, out List<ApplicationFileState> files)
        {
            files = new List<ApplicationFileState>();
            var root = new ApplicationDirectoryState(rootDirectory.RelativePath, null);
            var stack = new Stack<Tuple<ApplicationDirectoryStateDto, ApplicationDirectoryState>>();

            var parent = root;

            foreach (var dir in rootDirectory.ChildDirectories) stack.Push(new Tuple<ApplicationDirectoryStateDto, ApplicationDirectoryState>(dir, parent));

            while (stack.Count > 0)
            {
                var working = stack.Pop();
                var workingState = new ApplicationDirectoryState(working.Item1.RelativePath, working.Item2);
                working.Item2.AddDirectory(workingState);

                parent = workingState; // set new parent for children
                var dirs = working.Item1.ChildDirectories; // get children
                foreach (var dir in dirs) stack.Push(new Tuple<ApplicationDirectoryStateDto, ApplicationDirectoryState>(dir, parent));

                foreach (var file in working.Item1.Files)
                {
                    var applicationFileState = ApplicationFileStateTranslator.ToModel(file);
                    workingState.AddFile(applicationFileState);
                    files.Add(applicationFileState);
                }
            }
            return root;
        }
    }
}