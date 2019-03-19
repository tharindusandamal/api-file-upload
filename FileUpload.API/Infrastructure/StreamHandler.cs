using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FileUpload.API.Infrastructure
{
    public interface IStreamHandler
    {
        byte[] ReadFully(Stream input);
        Task SaveFile(string path, byte[] data);
    }

    public class StreamHandler : IStreamHandler
    {
        public byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        public async Task SaveFile(string path, byte[] data)
        {
            using (var file = new FileStream(path, FileMode.CreateNew, FileAccess.ReadWrite))
            {
                await file.WriteAsync(data, 0, data.Length);
            }
        }
    }
}
