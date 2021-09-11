using IDK_API_IMAGE.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace IDK_API_IMAGE.IService
{
    public interface IImageProcess
    {
        string Overlay(Image backImage, Image topImage, LogoSetting logoSetting);
        Image Resize(string imagepath, int newWidth);
        string Path(string sku);
        bool Exist(string sku);
        LogoSetting Logo(string sku);

        string LogoPath(string sku);

        string BasePath(string sku);
    }
}
