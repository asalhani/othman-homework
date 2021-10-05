using Microsoft.AspNetCore.Http;
using Models.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Models.Impl
{
    public class FileManager : IFileManager
    {
        private readonly ICacheService _cacheService;
        private const string _cache_key = "uploaded_files";
        public FileManager(ICacheService cacheService)
        {
            _cacheService = cacheService;
        }
        public bool UploadFile(MyFileInfo fileInfo, IFormFile fileForm)
        {
            var folderName = Path.Combine("Resources", "Images");
            var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);

            if (fileForm.Length > 0)
            {
                double sizeMB = Convert.ToDouble(fileForm.Length) / 1024 / 1024;
                if (sizeMB > 5)
                {
                    //return BadRequest();
                    return false;
                }
                var fileName = ContentDispositionHeaderValue.Parse(fileForm.ContentDisposition).FileName.Trim('"');
                var fullPath = Path.Combine(pathToSave, fileName);
                var dbPath = Path.Combine(folderName, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    fileForm.CopyTo(stream);
                }
                
                // update redis cache
                UpdateRedisCache(fileInfo);

                //return Ok(new { dbPath });
                return true;
            }
            else
            {
                return false;
                //return BadRequest();
            }
        }

        public List<MyFileInfo> GetFiles()
        {
            var files = _cacheService.Get<List<MyFileInfo>>(_cache_key);
            return files;
        }

        private void UpdateRedisCache(MyFileInfo fileInfo)
        {
            var items = _cacheService.Get<List<MyFileInfo>>(_cache_key);
            if (items == null)
                items = new List<MyFileInfo>();
            
            items.Add(fileInfo);

            _cacheService.Set(_cache_key, items);
        }
    }
}
