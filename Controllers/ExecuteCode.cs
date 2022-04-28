using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Soluvision_NetCore
{
    [ApiController]
    [Route("[controller]")]
    public class ExecuteCodeController : ControllerBase
    {
        private readonly IWebHostEnvironment _webHostenvironment;

        public ExecuteCodeController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostenvironment = webHostEnvironment;
        }

        [HttpPost("[action]")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult UploadFiles(IFormFile file)
        {
            if (file == null)
                return BadRequest();
            string directoryPath = Path.Combine(_webHostenvironment.ContentRootPath, "UploadedFiles");

            string filePath = Path.Combine(directoryPath, file.FileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            return Ok("Upload Successful");
        }

        [HttpPost("[action]")]
        public IActionResult GetResultFromPythonCode(string message)
        {
            if (message == null)
                return BadRequest();

            PythonInstance py = new(@"
from datetime import datetime

class PyClass:
    def __init__(self):
        pass

    def getCurrentDatetime(self):
        return datetime.today().strftime('%Y-%m-%d %H:%M:%S.%f')

    def readTextFromImage(self, data):
        return data + ' ' + self.getCurrentDatetime()

", "PyClass");

            string result = py.CallFunction("readTextFromImage", message);

            return Ok(result);
        }
    }
}
