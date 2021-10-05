using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Models.Interfaces
{
    public interface IFileManager
    {
        bool UploadFile(IFormFile file);
    }
}
