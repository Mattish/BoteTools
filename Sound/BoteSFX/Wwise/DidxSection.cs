using System;

namespace BoteSFX.Wwise
{
    internal interface IDidxSection : IWritable
    {
        string SectionAsString { get; }
        IDidxItem[] DidxItems { get; }
    }

    class DidxSection : GeneralSection, IDidxSection
    {
        private readonly int _amount;

        public DidxItem[] Items { get; set; }

        public IDidxItem[] DidxItems
        {
            get { return Items; }
        }

        public DidxSection(GeneralSection section)
        {
            _amount = section.Data.Length / 12;
            Section = section.Section;
            Data = section.Data;
            LengthOfSectionData = section.LengthOfSectionData;
        }

        public override void Process()
        {
            Items = new DidxItem[_amount];
            for (int i = 0; i < _amount; i++)
            {
                Items[i] = new DidxItem();
                var startingIndex = (12 * i);
                Items[i].FileId = BitConverter.ToUInt32(Data, startingIndex);
                Items[i].OffsetFromStartOfDataSection = BitConverter.ToUInt32(Data, startingIndex + 4);
                Items[i].DataLength = BitConverter.ToUInt32(Data, startingIndex + 8);
            }
        }

        public override byte[] ToBytes()
        {
            byte[] tmpArray = new byte[4 + 4 + (Items.Length * 12)];
            Array.Copy(Section, 0, tmpArray, 0, 4);
            Array.Copy(BitConverter.GetBytes(LengthOfSectionData), 0, tmpArray, 4, 4);
            int j = 8;
            for (int i = 0; i < Items.Length; i++)
            {
                Array.Copy(Items[i].ToBytes(), 0, tmpArray, j, 12);
                j += 12;
            }
            return tmpArray;
        }
    }
}