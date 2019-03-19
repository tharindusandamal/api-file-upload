using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FileUpload.API.Infrastructure
{
    public interface IMyDirectoryHandler
    {
        string CreateRootDirectory(string name);
        string CreateDirectory(string path, string name);
    }

    public class MyDirectoryHandler : IMyDirectoryHandler
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly string _rootPath;

        public MyDirectoryHandler(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
            _rootPath = _hostingEnvironment.ContentRootPath;
        }

        public string CreateRootDirectory(string name)
        {
            string path = Path.Combine(new string[] { _rootPath, name });
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            return path;
        }

        public string CreateDirectory(string path, string name)
        {
            path = Path.Combine(new string[] { path, name });
            Directory.CreateDirectory(path);
            return path;
        }
    }
}
