using System;

namespace BoteCore
{
    public static class ApplicationFileTypeExtension
    {
        public static ApplicationFileType FromString(this string s)
        {
            if (s.Contains("pdf") || s.Contains("PDF"))
                return ApplicationFileType.Pdf;
            if (s.Contains("png") || s.Contains("PNG"))
                return ApplicationFileType.Png;
            if (s.Contains("bnk") || s.Contains("BNK"))
                return ApplicationFileType.Bnk;
            return ApplicationFileType.Unknown;
        }

        public static string ToString(this ApplicationFileType fileType)
        {
            switch (fileType)
            {
                case ApplicationFileType.Unknown:
                    return "Unknown";
                case ApplicationFileType.Bnk:
                    return "bnk";
                case ApplicationFileType.Png:
                    return "png";
                case ApplicationFileType.Pdf:
                    return "pdf";
                default:
                    throw new ArgumentOutOfRangeException("fileType");
            }
        }
    }
}