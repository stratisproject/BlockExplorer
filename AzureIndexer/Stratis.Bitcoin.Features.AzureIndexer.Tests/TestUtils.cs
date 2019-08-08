using System.IO;

namespace Stratis.Bitcoin.Features.AzureIndexer.Tests
{
    internal class TestUtils
    {
        internal static void EnsureNew(string folderName)
        {
            if (Directory.Exists(folderName))
                Directory.Delete(folderName, true);

            while (true)
            {
                try
                {
                    Directory.CreateDirectory(folderName);
                    break;
                }
                catch
                {
                }
            }
        }
    }
}
