using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;

namespace BoteCore
{
    public interface IApplicationState
    {
        string AssemblyVersion { get; }
        IEnumerable<ApplicationFileState> Files { get; }
        ApplicationDirectoryState RootDirectory { get; }
    }

    public class ApplicationState : IApplicationState
    {
        private readonly string _path;
        private readonly List<ApplicationFileState> _files;

        public string AssemblyVersion { get; private set; }

        public IEnumerable<ApplicationFileState> Files => _files;
        public ApplicationDirectoryState RootDirectory { get; private set; }

        private ApplicationState(string path)
        {
            _path = path;
            _files = new List<ApplicationFileState>();
        }

        internal ApplicationState(string version, List<ApplicationFileState> files, ApplicationDirectoryState rootDirectory)
        {
            AssemblyVersion = version;
            _files = files;
            RootDirectory = rootDirectory;
        }

        private void Load()
        {
            LoadAssembly();
            LoadDirectories();
        }

        private void LoadDirectories()
        {
            if (Directory.Exists(_path))
            {
                var stack = new Stack<Tuple<string, ApplicationDirectoryState>>();

                var root = new ApplicationDirectoryState("/", null);
                var dirs = Directory.GetDirectories(_path);

                var parent = root;

                foreach (var dir in dirs) stack.Push(new Tuple<string, ApplicationDirectoryState>(dir, parent));

                while (stack.Count > 0)
                {
                    var working = stack.Pop();
                    var workingState = new ApplicationDirectoryState(working.Item1.Replace(_path, ""), working.Item2);
                    working.Item2.AddDirectory(workingState);

                    parent = workingState; // set new parent for children
                    dirs = Directory.GetDirectories(working.Item1); // get children
                    foreach (var dir in dirs) stack.Push(new Tuple<string, ApplicationDirectoryState>(dir, parent));

                    var files = Directory.GetFiles(working.Item1);
                    foreach (var file in files)
                    {
                        if (file.EndsWith(".bnk", true, CultureInfo.CurrentCulture))
                        {
                            var applicationFileState = ApplicationFileState.Create(file, _path);
                            workingState.AddFile(applicationFileState);
                            _files.Add(applicationFileState);
                        }
                    }
                }
                RootDirectory = root;
            }
        }

        private void LoadAssembly()
        {
            if (File.Exists(_path + "WorldOfWarships.exe"))
                AssemblyVersion = FileVersionInfo.GetVersionInfo(_path + "WorldOfWarships.exe").ProductVersion;
            else
            {
                throw new FileNotFoundException("Unable to find WorldOfWarships.exe");
            }
        }

        public static ApplicationState Create(string path)
        {
            var applicationState = new ApplicationState(path);
            applicationState.Load();
            return applicationState;
        }
    }
}
