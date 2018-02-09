
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Serilog;

namespace Redcap.Utilities
{
    /// <summary>
    /// Utility class for extension methods
    /// </summary>
    public static class Utils
    {
        /// <summary>
        /// Extension method reads a stream and saves content to a local file.
        /// </summary>
        /// <param name="httpContent"></param>
        /// <param name="fileName"></param>
        /// <param name="path"></param>
        /// <param name="overwrite"></param>
        /// <returns>HttpContent</returns>
        public static Task ReadAsFileAsync(this HttpContent httpContent, string fileName, string path, bool overwrite)
        {

            if (!overwrite && File.Exists(Path.Combine(fileName, path)))
            {
                throw new InvalidOperationException($"File {fileName} already exists.");
            }
            FileStream filestream = null;
            try
            {
                fileName = fileName.Replace("\"", "");
                filestream = new FileStream(Path.Combine(path, fileName), FileMode.Create, FileAccess.Write, FileShare.None);
                return httpContent.CopyToAsync(filestream).ContinueWith(
                    (copyTask) =>
                    {
                        filestream.Flush();
                        filestream.Dispose();
                    }
                );
            }
            catch(Exception Ex)
            {
                Log.Error(Ex.Message);
                if(filestream != null)
                {
                    filestream.Flush();
                }
                throw new InvalidOperationException($"{Ex.Message}");
            }
        }
        /// https://stackoverflow.com/questions/8560106/isnullorempty-equivalent-for-array-c-sharp
        /// <summary>Indicates whether the specified array is null or has a length of zero.</summary>
        /// <param name="array">The array to test.</param>
        /// <returns>true if the array parameter is null or has a length of zero; otherwise, false.</returns>
        public static bool IsNullOrEmpty<T>(this T[] array)
        {
            return (array == null || array.Length == 0);
        }
    }
}
