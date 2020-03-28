using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Reader = Blazor.FileReader;
namespace Gabriel.Cat.Extension
{
    public static class IFileReferenceExtension
    {
        public static async Task<byte[]> Read(this Reader.IFileReference fileReader, int buffer = 4 * 1024)
        {
            MemoryStream ms = null;
            byte[] bytesFile = null;
            try
            {
                ms = await fileReader.CreateMemoryStreamAsync(buffer);

                bytesFile = new byte[ms.Length];
                ms.Read(bytesFile, 0, (int)ms.Length);
            }
            finally
            {
                if (ms != null)
                    ms.Close();

            }
            return bytesFile;
        }
    }
}
