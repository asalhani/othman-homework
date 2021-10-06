using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Models;
using Models.Impl;
using Models.Interfaces;
using UploadFilesServer.Models;

namespace UploadFilesServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly IFileManager _fileManager;

        public FileController(IFileManager fileManager)
        {
            _fileManager = fileManager;
        }
        
        [HttpPost, DisableRequestSizeLimit]
        [RequestFormLimits(ValueLengthLimit = int.MaxValue, MultipartBodyLengthLimit = int.MaxValue)]
        [Route("upload")]
        public async Task<IActionResult> Upload()
        {
            try
            {
                //var file = Request.Form.Files[0];
               var formCollection = await Request.ReadFormAsync();
                var file = formCollection.Files.First();

                var fileInfo = new MyFileInfo()
                {
                    FileName = file.FileName,
                    ContentType = file.ContentType,
                    FileSize= file.Length,
                    FileExtention= Path.GetExtension(file.FileName)
                };
               
                _fileManager.UploadFile(fileInfo, file);
                 
                return Ok(new SuccessResponse(){Message = "File uploaded"});
            }
            catch (Exception ex)
            {
             //  return BadRequest(new ErrorResponse() {ErrorMessage = "File upload error..."});
             return BadRequest(ex.ToString());
            }
        }

        [HttpGet, DisableRequestSizeLimit]
        [Route("GetFiles")]
        public async Task<IActionResult> GetFiles()
        {
            return Ok(_fileManager.GetFiles());
        }

        [HttpGet, DisableRequestSizeLimit]
        [Route("download")]
        public async Task<IActionResult> Download([FromQuery] string fileUrl)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), fileUrl);

            if (!System.IO.File.Exists(filePath))
                return NotFound();

            var memory = new MemoryStream();
            await using (var stream = new FileStream(filePath, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;

            return File(memory, GetContentType(filePath), filePath);
        }

        [HttpGet, DisableRequestSizeLimit]
        [Route("getPhotos")]
        public IActionResult GetPhotos()
        {
            try
            {
                var folderName = Path.Combine("Resources", "Images");
                var pathToRead = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                var photos = Directory.EnumerateFiles(pathToRead)
                    .Where(IsAPhotoFile)
                    .Select(fullPath => Path.Combine(folderName, Path.GetFileName(fullPath)));

                return Ok(new { photos });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

        [HttpGet]
        public IActionResult FilesInfo(string fileType)
        {
            var folderName = Path.Combine("Resources", "Images");
            var directoryPath = Path.Combine(Directory.GetCurrentDirectory(), folderName);
            List<FileDetails> fileDetailsList = new List<FileDetails>();
            string[] files = Directory.GetFiles(directoryPath);
            for (int i = 0; i < files.Length; i++)
            {
                FileInfo file = new FileInfo(files[i]);
                if (!string.IsNullOrWhiteSpace(fileType) && file.Extension.ToUpper() == "." + fileType.ToUpper())
                {
                    FileDetails f = new FileDetails();
                    f.fileName = file.Name;
                    f.fileSize = file.Length;
                    f.uploadDate = file.CreationTime;
                    fileDetailsList.Add(f);
                }
            }
            return Ok(files);
        }


        private bool IsAPhotoFile(string fileName)
        {
            return fileName.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase)
                   || fileName.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase)
                   || fileName.EndsWith(".png", StringComparison.OrdinalIgnoreCase);
        }

        private string GetContentType(string path)
        {
            var provider = new FileExtensionContentTypeProvider();
            string contentType;
            
            if (!provider.TryGetContentType(path, out contentType))
            {
                contentType = "application/octet-stream";
            }
            
            return contentType;
        }
    }
}
