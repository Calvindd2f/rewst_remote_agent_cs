namespace Rewst.RemoteAgent.Helpers
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
                string text = Path.Combine(destinationDirectoryName, zipArchiveEntry.FullName);
                string directoryName = Path.GetDirectoryName(text);
                if (!Directory.Exists(directoryName))
                {
                    _ = Directory.CreateDirectory(directoryName);
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
