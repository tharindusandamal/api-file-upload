using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileUpload.API.Infrastructure;
using FileUpload.API.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;

namespace FileUpload.API.Controllers.API
{
    [Produces("application/json")]
    [Route("api/documents")]
    public class DocumentsController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IMyDirectoryHandler _myDirectoryHandler;
        private readonly IStreamHandler _streamHandler;
        private readonly string _documentRootName = "TemporyDocuments";

        public DocumentsController(
            IHostingEnvironment hostingEnvironment, 
            IMyDirectoryHandler myDirectoryHandler,
            IStreamHandler streamHandler)
        {
            _hostingEnvironment = hostingEnvironment;
            _myDirectoryHandler = myDirectoryHandler;
            _streamHandler = streamHandler;

            // create document 
            _documentRootName = _myDirectoryHandler.CreateRootDirectory(_documentRootName);
        }

        [HttpPost]
        public async Task<IActionResult> Post()
        {
            string _requestId = Guid.NewGuid().ToString();

            if (!MultipartRequestHandler.IsMultipartContentType(Request.ContentType))
            {
                return BadRequest($"Expected a multipart request, but got {Request.ContentType}");
            }

            var boundary = MultipartRequestHandler.GetBoundary(MediaTypeHeaderValue.Parse(Request.ContentType), (int)Request.ContentLength);
            var reader = new MultipartReader(boundary, HttpContext.Request.Body);
            var section = await reader.ReadNextSectionAsync();
            List<FileContent> fileList = new List<FileContent>();
            List<string> jsonList = new List<string>();
            string jsonBody = string.Empty;

            while (section != null)
            {
                var hasContentDispositionHeader = ContentDispositionHeaderValue.TryParse(section.ContentDisposition, out ContentDispositionHeaderValue contentDisposition);
                if (hasContentDispositionHeader)
                {
                    // if context has file desposition
                    if (MultipartRequestHandler.HasFileContentDisposition(contentDisposition))
                    {
                        System.Net.Mime.ContentDisposition cd = new System.Net.Mime.ContentDisposition(section.ContentDisposition);
                        string filename = cd.FileName;
                        var fileContent = new FileContent
                        {
                            ContentType = section.ContentType,
                            FileName = filename,
                            ContentDisposition = section.ContentDisposition,
                            Stream = _streamHandler.ReadFully(section.Body)
                        };
                        fileList.Add(fileContent);
                    } // if conext has data desposition
                    else if (MultipartRequestHandler.HasFormDataContentDisposition(contentDisposition))
                    {
                        using (var streamReader = new StreamReader(
                            section.Body,
                            Encoding.UTF8,
                            detectEncodingFromByteOrderMarks: true,
                            bufferSize: 1024,
                            leaveOpen: true))
                        {
                            // The value length limit is enforced by MultipartBodyLengthLimit
                            jsonBody = await streamReader.ReadToEndAsync();
                            jsonList.Add(jsonBody);
                        }
                    }
                }

                // Drains any remaining section body that has not been consumed and
                // reads the headers for the next section.
                section = await reader.ReadNextSectionAsync();
            }

            // save data to sarver path
            if(jsonList.Count > 0 || fileList.Count > 0)
            {
                // create new directory
                var path = _myDirectoryHandler.CreateDirectory(_documentRootName, _requestId);

                // save files
                foreach (var item in fileList)
                {
                    await _streamHandler.SaveFile(Path.Combine(new string[] { path, item.FileName }), item.Stream);
                }

                // save JSONs
                foreach (var item in jsonList)
                {
                    await _streamHandler.SaveFile(Path.Combine(new string[] { path, _requestId + ".json" }), Encoding.UTF8.GetBytes(item));
                }
            }

            return await Task.FromResult(Ok(new { id = _requestId }));
        }
    }
}