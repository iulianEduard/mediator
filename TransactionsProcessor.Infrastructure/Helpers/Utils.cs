using System.IO;

namespace TransactionsProcessor.Infrastructure.Helpers
{
    public static class Utils
    {
        public static string GetFileNameFromFTP(string ftpLocation)
        {
            var bits = ftpLocation.Split('/');
            var bitsCount = bits.Length;

            if (bitsCount > 1)
            {
                return bits[bitsCount - 1];
            }
            else
            {
                return bits[0];
            }
        }

        public static void ValidateFolderLocation(string folderPath)
        {
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
        }
    }
}
