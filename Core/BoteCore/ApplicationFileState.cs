using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace BoteCore
{
    public class ApplicationFileState
    {
        public string RelativePath { get; private set; }
        public ApplicationFileType Type { get; private set; }
        public string Hash { get; private set; }

        internal ApplicationFileState(string relativePath, ApplicationFileType type, string hash)
        {
            RelativePath = relativePath;
            Type = type;
            Hash = hash;
        }

        public static ApplicationFileState Create(string filePath, string applicationPath)
        {
            Console.WriteLine("ApplicationFileState.Create: {0}", filePath);
            if (File.Exists(filePath))
            {
                var sha = SHA1.Create();
                var file = new FileInfo(filePath);
                var extension = file.Extension.Remove(0, 1);
                using (var fs = new FileStream(filePath, FileMode.Open))
                {
                    var hashResult = sha.ComputeHash(fs);
                    var results = BitConverter.ToString(hashResult).Replace("-", string.Empty);
                    return new ApplicationFileState(filePath.Replace(applicationPath, ""), extension.FromString(), results);
                }
            }
            else
            {
                throw new FileLoadException(string.Format("File not found {0}", filePath));
            }
        }

        public static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

        public override string ToString()
        {
            return RelativePath;
        }
    }
}