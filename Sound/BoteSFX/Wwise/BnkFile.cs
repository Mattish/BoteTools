using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BoteSFX.Wwise
{
    /// <summary>
    /// Public interface for the BnkFile. Provides methods to manipulate the BnkFile in a safe fashion.
    /// </summary>
    public interface IBnkFile
    {
        IDidxSection DidxSection { get; }
        IDataSection DataSection { get; }
        IHircSection HircSection { get; }

        void WriteToFile(string filePath);
        void ReplaceWemFile(uint id, string filePath);
    }

    /// <summary>
    /// Represents a .bnk in memory. Able to manipulate the state and write back to disk.
    /// </summary>
    public class BnkFile : IBnkFile
    {
        private readonly string _filePath;

        private DidxSection _didx;
        private DataSection _data;
        private HircSection _hirc;
        private List<GeneralSection> _generalSections;
        private bool _hasLoadedBefore;

        public IDidxSection DidxSection => _didx;
        public IDataSection DataSection => _data;
        public IHircSection HircSection => _hirc;

        private BnkFile(string filePath)
        {
            _filePath = filePath;
        }

        private void Load()
        {
            if (_hasLoadedBefore)
                throw new FileLoadException("Already loaded this file.");
            _hasLoadedBefore = true;

            _generalSections = new List<GeneralSection>();

            var bytes = File.ReadAllBytes(_filePath);
            var nextIndex = 0L;
            while (nextIndex < bytes.Length)
            {
                GeneralSection section;
                nextIndex = GeneralSection.Create(bytes, nextIndex, out section);
                switch (section.SectionAsString)
                {
                    case "DIDX":
                        _didx = new DidxSection(section);
                        section = _didx;
                        break;
                    case "DATA":
                        _data = new DataSection(section, _didx.Items);
                        section = _data;
                        break;
                    case "HIRC":
                        _hirc = new HircSection(section);
                        section = _hirc;
                        break;
                }
                section.Process();
                _generalSections.Add(section);
            }
            var memoryStream = new MemoryStream();
            foreach (var generalSection in _generalSections)
            {
                var bytesToWrite = generalSection.ToBytes();
                memoryStream.Write(bytesToWrite, 0, bytesToWrite.Length);
            }
            var memoryStreamBuffer = memoryStream.ToArray();
            for (int i = 0; i < memoryStream.Length; i++)
            {
                if (memoryStreamBuffer[i] != bytes[i])
                    throw new Exception("Weeeep!");
            }
        }


        /// <summary>
        /// Write all sections, with changes(if any) to a file. Will throw on failure.
        /// </summary>
        /// <param name="filePath">Location to write the file to</param>
        public void WriteToFile(string filePath)
        {
            if (!_hasLoadedBefore)
                throw new Exception("Have not loaded the .bnk file.");

            var memoryStream = new MemoryStream();
            foreach (var generalSection in _generalSections)
            {
                var bytesToWrite = generalSection.ToBytes();
                memoryStream.Write(bytesToWrite, 0, bytesToWrite.Length);
            }
            var memoryStreamBuffer = memoryStream.ToArray();

            File.WriteAllBytes(filePath, memoryStreamBuffer);
        }

        /// <summary>
        /// Replaces a .wem file in the BnkFile and updates internal offsets.
        /// </summary>
        /// <param name="id">Id of the wem file in the .bnk</param>
        /// <param name="filePath">Path to the file to replace with</param>
        public void ReplaceWemFile(uint id, string filePath)
        {
            var newFile = File.ReadAllBytes(filePath);

            var didxObjectIndex = _didx.Items.TakeWhile(item => item.FileId != id).Count();
            var didxObject = _didx.Items[didxObjectIndex];
            var dataObject = _data.Files.First(dataObj => dataObj.Id == id);
            var hircObject = _hirc.Objects.First(o => o.Inner != null && o.Inner.AudioFileId == didxObject.FileId);
            var oldDidxObjectLength = didxObject.DataLength;
            var oldDataObjectEndPadding = dataObject.EndPaddingLength;

            didxObject.DataLength = (uint)newFile.Length; // Set DIDX object length
            hircObject.Inner.LengthInBytesInSoundBank = (uint)newFile.Length; // Set HIRC object length
            dataObject.Data = newFile; // Set New Object data
            dataObject.EndPaddingLength = (uint)(16 - (newFile.Length % 16));

            var brandNewFile = dataObject.ToBytes();


            for (int i = 0; i < newFile.Length; i++)
            {
                if (newFile[i] != brandNewFile[i])
                    throw new Exception("Weeeep! The new file hasnt created correctly!");
            }

            for (int i = didxObjectIndex + 1; i < _didx.Items.Length; i++) // For all didx objects after the replaced one
            {
                var item = _didx.Items[i];
                item.OffsetFromStartOfDataSection -= (oldDidxObjectLength + oldDataObjectEndPadding);
                item.OffsetFromStartOfDataSection += (didxObject.DataLength + dataObject.EndPaddingLength);

                if (_hirc.Objects.Any(o => o.Inner != null && o.Inner.AudioFileId == item.FileId)) // Can we find a HIRC object for the DIX object?
                {
                    var hircObjectt = _hirc.Objects.First(o => o.Inner != null && o.Inner.AudioFileId == item.FileId);
                    if (hircObjectt.Inner.LengthInBytesInSoundBank == item.DataLength) // If it's a 'real' HIRC object
                    {
                        hircObjectt.AbsoluteIndexOfData -= (oldDidxObjectLength + oldDataObjectEndPadding);
                        hircObjectt.AbsoluteIndexOfData += (didxObject.DataLength + dataObject.EndPaddingLength);
                    }
                    else
                    {
                        Console.WriteLine("Couldn't find matching HIRC for DIDX.Id:{0}", item.FileId);
                    }
                }
            }
        }

        /// <summary>
        /// Attempts to load a .bnk file. Will throw on failure.
        /// </summary>
        /// <param name="filePath">The file to load</param>
        /// <returns>If success, returns a loaded IBnkFile</returns>
        public static IBnkFile Create(string filePath)
        {
            var bnk = new BnkFile(filePath);
            bnk.Load();
            return bnk;
        }
    }
}