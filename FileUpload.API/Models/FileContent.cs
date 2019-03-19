using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FileUpload.API.Models
{
    public class FileContent
    {
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public byte[] Stream { get; set; }
        public string ContentDisposition { get; set; }
    }
}
