using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using Models;
using Models.Interfaces;
using Moq;
using UploadFilesServer.Controllers;
using FileManager = Models.Impl.FileManager;

namespace FileManager.UnitTest
{
    [TestClass]
    public class FileMangerUnitTest
    {
        public Mock<IFileManager> _mockFileManagear = new Mock<IFileManager>();
        public Mock<ICacheService> _mockCacheService = new Mock<ICacheService>();
        

        [TestMethod]
        // [ExpectedException(typeof(NotImplementedException))]
        public void GetFiles_HandleEmptyList()
        {
            _mockFileManagear.Setup(p => p.GetFiles()).Returns(() => { return new List<MyFileInfo>(); });
            FileController fileController = new FileController(_mockFileManagear.Object);
            var test = fileController.GetFiles().Result;
            Assert.IsTrue(test != null);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void UploadFile_HandleFileSizeException()
        {
            // setup
            _mockCacheService.Setup(c => c.Get<MyFileInfo>("")).Returns(() => null);
            _mockCacheService.Setup(c => c.Set("", ""));

            // excute
            Models.Impl.FileManager fmgr = new Models.Impl.FileManager(_mockCacheService.Object);
            fmgr.UploadFile(new MyFileInfo()
            {
                ContentType = "image/png",
                FileExtention = ".png",
                FileName = "test-1234.png",
                FileSize = 0,
                UploadDate = DateTime.Now
            }, null);
            
            // assert (cache ArgumentOutOfRangeException exception)
        }
    }
}