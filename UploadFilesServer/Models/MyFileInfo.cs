using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Models
{
   public class MyFileInfo
    {
        public string FileName { get; set; }
        public string FileExtention { get; set; }
        public long FileSize { get; set; }
        public string ContentType { get; set; }
        public DateTime UploadDate { get; set; } = DateTime.Now;

    }
}
