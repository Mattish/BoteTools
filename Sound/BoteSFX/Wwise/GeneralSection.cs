using System;
using System.Text;

namespace BoteSFX.Wwise
{
    internal interface IGeneralSection
    {
        string SectionAsString { get; }
    }

    class GeneralSection : IWritable, IGeneralSection
    {
        public byte[] Section;

        public string SectionAsString
        {
            get { return Encoding.ASCII.GetString(Section); }
        }

        public UInt32 LengthOfSectionData;
        public byte[] Data;

        public override string ToString()
        {
            return string.Format("Section:{0} - Length:{1}", Section, LengthOfSectionData);
        }

        virtual public byte[] ToBytes()
        {
            byte[] tmpArray = new byte[4 + 4 + Data.Length];
            Array.Copy(Section, 0, tmpArray, 0, 4);
            Array.Copy(BitConverter.GetBytes(LengthOfSectionData), 0, tmpArray, 4, 4);
            Array.Copy(Data, 0, tmpArray, 8, Data.Length);
            return tmpArray;
        }

        public virtual void Process()
        {

        }

        public static long Create(byte[] file, long index, out GeneralSection section)
        {
            section = new GeneralSection();
            section.Section = new byte[4];
            Array.Copy(file, index, section.Section, 0, 4);
            section.LengthOfSectionData = BitConverter.ToUInt32(file, (int)(index + 4));
            section.Data = new byte[section.LengthOfSectionData];
            Array.Copy(file, index + 8, section.Data, 0, section.LengthOfSectionData);
            return index + 8 + section.LengthOfSectionData;
        }
    }
}