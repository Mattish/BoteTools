using System.Diagnostics;
using System.IO;

namespace BoteCore
{
    public interface IBoteApplication
    {
        FileVersionInfo AssemblyVersion { get; }
    }

    public class BoteApplication : IBoteApplication
    {
        private readonly string _path;

        private BoteApplication(string path)
        {
            _path = path;
        }

        private void Load()
        {
            if (File.Exists(_path))
            {
                AssemblyVersion = FileVersionInfo.GetVersionInfo(_path + "WorldOfWarships.exe");
            }
        }

        public static BoteApplication Create(string path)
        {
            var assembly = new BoteApplication(path);
            assembly.Load();
            return assembly;
        }

        public FileVersionInfo AssemblyVersion { get; set; }
    }
}
