using System;
using System.IO;
using System.IO.Compression;

namespace Rewst.RemoteAgent.Calvindd2f.Helpers
{
    public static class ZipArchiveExtensions
    {
        public static void ExtractToDirectory(
            this System.IO.Compression.ZipArchive archive,
            string destinationDirectoryName,
            bool overwriteFiles
        )
        {
            if (!overwriteFiles)
            {
                System.IO.Compression.ZipFileExtensions.ExtractToDirectory(
                    archive,
                    destinationDirectoryName
                );
                return;
            }
            foreach (System.IO.Compression.ZipArchiveEntry zipArchiveEntry in archive.Entries)
            {
                string text = System.IO.Path.Combine(
                    destinationDirectoryName,
                    zipArchiveEntry.FullName
                );
                string directoryName = System.IO.Path.GetDirectoryName(text);
                if (!System.IO.Directory.Exists(directoryName))
                {
                    System.IO.Directory.CreateDirectory(directoryName);
                }
                if (zipArchiveEntry.Name != "")
                {
                    System.IO.Compression.ZipFileExtensions.ExtractToFile(
                        zipArchiveEntry,
                        text,
                        true
                    );
                }
            }
        }
    }
}
