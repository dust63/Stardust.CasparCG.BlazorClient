using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Elsa.Activities.FileTransferProtocol.Extensions
{
    public static class FileExtensions
    {

        public static void EnsureDirectoryExists(this string path)
        {
            var directoryPath = Path.GetDirectoryName(path);
            if (Directory.Exists(directoryPath))
                return;

            Directory.CreateDirectory(directoryPath);
        }
    }
}
