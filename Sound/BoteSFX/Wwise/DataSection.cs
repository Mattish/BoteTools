using System;
using System.Linq;

namespace BoteSFX.Wwise
{
    public interface IDataSection
    {
        string SectionAsString { get; }
        IWemFile[] WemFiles { get; }
    }

    class DataSection : GeneralSection, IDataSection
    {
        private readonly DidxItem[] _items;
        public WemFile[] Files;

        public IWemFile[] WemFiles
        {
            get { return Files; }
        }

        public DataSection(GeneralSection section, DidxItem[] items)
        {
            _items = items;
            Section = section.Section;
            Data = section.Data;
            LengthOfSectionData = section.LengthOfSectionData;
        }

        public override void Process()
        {
            Files = new WemFile[_items.Length];
            uint previousOffsetFromStartOfDataSection = 0;
            uint previousDataLength = 0;
            for (int i = 0; i < _items.Length; i++)
            {
                Files[i] = new WemFile
                {
                    Data = new byte[_items[i].DataLength],
                    Id = _items[i].FileId
                };
                Array.Copy(Data, _items[i].OffsetFromStartOfDataSection, Files[i].Data, 0, _items[i].DataLength);
                if ((previousOffsetFromStartOfDataSection + previousDataLength) !=
                    _items[i].OffsetFromStartOfDataSection)
                {
                    Files[i - 1].EndPaddingLength = _items[i].OffsetFromStartOfDataSection - (previousOffsetFromStartOfDataSection + previousDataLength);
                }
                previousDataLength = _items[i].DataLength;
                previousOffsetFromStartOfDataSection = _items[i].OffsetFromStartOfDataSection;
            }
        }

        public override byte[] ToBytes()
        {
            var total = Files.Sum(file => file.Data.Length + file.EndPaddingLength);
            byte[] tmpArray = new byte[8 + total];
            Array.Copy(Section, 0, tmpArray, 0, 4);
            Array.Copy(BitConverter.GetBytes(total), 0, tmpArray, 4, 4);
            var index = 8;
            for (int i = 0; i < Files.Length; i++)
            {
                var wemFileBytes = Files[i].ToBytes();
                Array.Copy(wemFileBytes, 0, tmpArray, index, wemFileBytes.Length);

                for (int j = 0; j < Files[i].Data.Length; j++)
                {
                    if (wemFileBytes[j] != Files[i].Data[j])
                        throw new Exception("wtf");
                }

                index += wemFileBytes.Length;
            }
            return tmpArray;
        }
    }
}