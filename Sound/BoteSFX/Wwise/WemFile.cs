using System;
using System.Linq;

namespace BoteSFX.Wwise
{
    public interface IWemFile
    {
        byte[] Data { get; }
        uint Id { get; }
    }

    class WemFile : IWritable, IWemFile
    {
        public byte[] Data { get; set; }
        public uint Id { get; set; }
        public uint EndPaddingLength;

        public override string ToString()
        {
            var first4Bytes = new char[4];
            Array.Copy(Data, 0, first4Bytes, 0, 4);
            return string.Format("WemFile - Id:{0} - FileLength:{1} - First4Bytes:{2}", Id, Data.Length, new string(first4Bytes));
        }

        public byte[] ToBytes()
        {
            return Data.Concat(new byte[EndPaddingLength]).ToArray();
        }
    }
}