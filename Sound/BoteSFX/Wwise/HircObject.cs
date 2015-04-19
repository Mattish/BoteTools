using System;

namespace BoteSFX.Wwise
{
    internal interface IHircObject
    {
        byte Type { get; }
        UInt32 IdAndDataLength { get; }
        UInt32 Id { get; }
        IHircSectionInner SectionInner { get; }
        uint AbsoluteIndexOfData { get; }
    }

    class HircObject : IWritable, IHircObject
    {
        public byte Type { get; set; }
        public UInt32 IdAndDataLength { get; set; }
        public UInt32 Id { get; set; }
        public byte[] Data { get; set; }
        public HircSectionInner Inner { get; set; }
        public uint AbsoluteIndexOfData { get; set; }

        public IHircSectionInner SectionInner { get { return Inner; } }

        public override string ToString()
        {
            return string.Format("HircObject - Type:{0} - Id:{1} - IdAndDataLength:{2}", Type, Id, IdAndDataLength);
        }

        public byte[] ToBytes()
        {
            byte[] tmpArray = new byte[9 + Data.Length];
            tmpArray[0] = Type;
            Array.Copy(BitConverter.GetBytes(IdAndDataLength), 0, tmpArray, 1, 4);
            Array.Copy(BitConverter.GetBytes(Id), 0, tmpArray, 5, 4);
            if (Type == 0x02 &&
                Inner != null &&
                Inner.IsSoundbankOrStreamed == 0x00 &&
                Inner.LengthInBytesInSoundBank > 0)
            {
                Array.Copy(new byte[4], 0, tmpArray, 9, 4);
                //4 empty bytes

                tmpArray[13] = Inner.IsSoundbankOrStreamed;
                Array.Copy(BitConverter.GetBytes(Inner.AudioFileId), 0, tmpArray, 14, 4);
                Array.Copy(BitConverter.GetBytes(Inner.IdOfSource), 0, tmpArray, 18, 4);
                Array.Copy(BitConverter.GetBytes(Inner.OffsetToPosInSoundbank), 0, tmpArray, 22, 4);
                Array.Copy(BitConverter.GetBytes(Inner.LengthInBytesInSoundBank), 0, tmpArray, 26, 4);
            }
            else
            {
                Array.Copy(Data, 0, tmpArray, 9, Data.Length);
            }

            return tmpArray;
        }
    }
}