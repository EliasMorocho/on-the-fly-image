using IDK_API_IMAGE.IService;
using IDK_API_IMAGE.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace IDK_API_IMAGE.Controllers
{
    [Route("/")]
    [ApiController]
    public class ImageController : ControllerBase
    {

        private readonly IImageProcess imageProcess;
        public ImageController(IImageProcess imageProcess)
        {
            this.imageProcess = imageProcess;

        }     
        [HttpGet("{sku}")]
        public IActionResult Get(string sku)
        {
            if (string.IsNullOrEmpty(sku))
            {
                return null;
            }
            string path = imageProcess.Path(sku.Split(",")[0]);
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }
            byte[] b = System.IO.File.ReadAllBytes(path);
            return File(b, "image/jpeg");
        }
    }
}
