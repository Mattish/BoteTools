namespace BoteSFX.Wwise
{
    public interface IHircSectionInner
    {
        byte IsSoundbankOrStreamed { get; }
        uint AudioFileId { get; }
        uint IdOfSource { get; }
        uint OffsetToPosInSoundbank { get; }
        uint LengthInBytesInSoundBank { get; }
    }

    class HircSectionInner : IHircSectionInner
    {
        public byte IsSoundbankOrStreamed { get; set; }
        public uint AudioFileId { get; set; }
        public uint IdOfSource { get; set; }
        public uint OffsetToPosInSoundbank { get; set; }
        public uint LengthInBytesInSoundBank { get; set; }
    }
}