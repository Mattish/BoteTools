using System;

namespace BoteSFX.Wwise
{
    public interface IDidxItem
    {
        UInt32 FileId { get; }
        UInt32 OffsetFromStartOfDataSection { get; }
        UInt32 DataLength { get; }
    }

    class DidxItem : IWritable, IDidxItem
    {
        public UInt32 FileId { get; set; }
        public UInt32 OffsetFromStartOfDataSection { get; set; }
        public UInt32 DataLength { get; set; }

        public override string ToString()
        {
            return string.Format("DidxItem - FileId:{0} - OffsetFromStartOfDataSection:{1} - DataLength:{2}",
                FileId, OffsetFromStartOfDataSection, DataLength);
        }

        public byte[] ToBytes()
        {
            byte[] tmpArray = new byte[12];
            Array.Copy(BitConverter.GetBytes(FileId), 0, tmpArray, 0, 4);
            Array.Copy(BitConverter.GetBytes(OffsetFromStartOfDataSection), 0, tmpArray, 4, 4);
            Array.Copy(BitConverter.GetBytes(DataLength), 0, tmpArray, 8, 4);
            return tmpArray;
        }
    }
}