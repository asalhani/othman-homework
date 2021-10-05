using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Models.Interfaces
{
    public interface IFileManager
    {
        public bool UploadFile(MyFileInfo fileInfo, IFormFile fileForm);
        public List<MyFileInfo> GetFiles();
    }
}
