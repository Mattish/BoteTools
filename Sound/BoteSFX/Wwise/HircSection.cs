using System;

namespace BoteSFX.Wwise
{
    internal interface IHircSection
    {
        string SectionAsString { get; }
        IHircObject[] HircObjects { get; }
    }

    internal class HircSection : GeneralSection, IHircSection
    {
        public UInt32 NumberOfObjects { get; set; }
        public HircObject[] Objects { get; set; }

        public IHircObject[] HircObjects { get { return Objects; } }

        public HircSection(GeneralSection section)
        {
            Section = section.Section;
            Data = section.Data;
            LengthOfSectionData = section.LengthOfSectionData;
        }

        public override void Process()
        {
            uint index = 0;
            NumberOfObjects = BitConverter.ToUInt32(Data, 0);
            Objects = new HircObject[NumberOfObjects];
            index += 4;

            for (int i = 0; i < NumberOfObjects; i++)
            {
                Objects[i] = new HircObject();
                Objects[i].Type = Data[index];
                index += 1;
                Objects[i].IdAndDataLength = BitConverter.ToUInt32(Data, (int)index);
                index += 4;
                Objects[i].Id = BitConverter.ToUInt32(Data, (int)index);
                index += 4;
                Objects[i].Data = new byte[Objects[i].IdAndDataLength - 4];
                Array.Copy(Data, index, Objects[i].Data, 0, Objects[i].Data.Length);


                if (Objects[i].Type == 0x02)
                {
                    Objects[i].Inner = new HircSectionInner();
                    var innerIndex = 4;
                    Objects[i].Inner.IsSoundbankOrStreamed = Objects[i].Data[innerIndex];
                    innerIndex += 1;
                    Objects[i].Inner.AudioFileId = BitConverter.ToUInt32(Objects[i].Data, innerIndex);
                    innerIndex += 4;
                    Objects[i].Inner.IdOfSource = BitConverter.ToUInt32(Objects[i].Data, innerIndex);
                    if (Objects[i].Inner.IsSoundbankOrStreamed == 0x00)
                    {
                        innerIndex += 4;
                        Objects[i].Inner.OffsetToPosInSoundbank = BitConverter.ToUInt32(Objects[i].Data, innerIndex);
                        innerIndex += 4;
                        Objects[i].Inner.LengthInBytesInSoundBank = BitConverter.ToUInt32(Objects[i].Data, innerIndex);
                    }
                }
                index += (uint)Objects[i].Data.Length;
            }
        }

        public override byte[] ToBytes()
        {
            byte[] tmpArray = new byte[8 + Data.Length];
            Array.Copy(Section, 0, tmpArray, 0, 4);
            Array.Copy(BitConverter.GetBytes(LengthOfSectionData), 0, tmpArray, 4, 4);
            Array.Copy(BitConverter.GetBytes(NumberOfObjects), 0, tmpArray, 8, 4);
            //Header End

            var j = 12;
            for (int i = 0; i < Objects.Length; i++)
            {
                var objectBytes = Objects[i].ToBytes();
                Array.Copy(objectBytes, 0, tmpArray, j, objectBytes.Length);
                j += objectBytes.Length;
            }

            Array.Copy(Data, 0, tmpArray, 8, Data.Length);
            return tmpArray;
        }
    }
}