
using IDK_API_IMAGE.IService;
using IDK_API_IMAGE.Models;
using LazZiya.ImageResize;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace IDK_API_IMAGE.Services
{
    public class ImageProcess : IImageProcess
    {
        private readonly IOptions<List<ProductSetting>> productConfigurations;
        private readonly IOptions<PathSetting> pathConfiguration;
        public ImageProcess(IOptions<List<ProductSetting>> productConfigurations, IOptions<PathSetting> pathConfiguration)
        {
            this.productConfigurations = productConfigurations;
            this.pathConfiguration = pathConfiguration;
        }
        public bool Exist(string path)
        {
            if (File.Exists(path))
            {
                return true;
            }
            return false;
        }

        public string Overlay(Image baseImage, Image logoImage, LogoSetting logoSetting)
        {
            try
            {
                string path = string.Format("{0}{1}.jpg", pathConfiguration.Value.Temp, Guid.NewGuid());
                Bitmap bmp = new Bitmap(baseImage.Width, baseImage.Height);
                int y = logoSetting.X;
                var image = logoImage.Scale(logoSetting.W, logoSetting.H);
                if (logoSetting.T == 1)
                {
                    y = logoSetting.Y + ((logoSetting.H - image.Height) / 2);
                }
                int x = logoSetting.X + ((logoSetting.W - image.Width) / 2);
                using (Graphics canvas = Graphics.FromImage(bmp))
                {
                    canvas.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    canvas.DrawImage(image: baseImage, destRect: new Rectangle(0, 0, bmp.Width, bmp.Height), srcRect: new Rectangle(0, 0, bmp.Width, bmp.Height), srcUnit: GraphicsUnit.Pixel);
                    canvas.DrawImage(image: image, destRect: new Rectangle(x, y, image.Width, image.Height), srcRect: new Rectangle(0, 0, image.Width, image.Height), srcUnit: GraphicsUnit.Pixel);
                }
                bmp.SaveAs(path, 25);
                return path;
            }
            catch (Exception ex)
            {
                Log.ILog(ex.Message);
            }
            return null;
        }

        public Image Resize(string imagepath, int newWidth)
        {
            try
            {
                Image img = Image.FromFile(imagepath);
                GraphicOptions graphicOptions = new GraphicOptions();
                graphicOptions.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.GammaCorrected;
                var image = img.ScaleByWidth(newWidth, graphicOptions);
                return image;
            }
            catch (Exception ex)
            {
                Log.ILog(ex.Message);
            }
            return null;
        }

        public string Path(string sku)
        {
            LogoSetting logoSetting = Logo(sku);
            string path = string.Format(@"{0}{1}.jpg", pathConfiguration.Value.Out, sku);
            if (Exist(path))
            {
                return path;
            }
            else
            {
                if (string.IsNullOrEmpty(logoSetting.BasePath) || string.IsNullOrEmpty(logoSetting.LogoPath))
                {
                    return "";
                }
                Image ImageBase = Image.FromFile(logoSetting.BasePath);
                Image ImageLogo = Image.FromFile(logoSetting.LogoPath);
                try
                {
                    string image_path = Overlay(ImageBase, ImageLogo, logoSetting);
                    Image _image = Resize(image_path, logoSetting.Width);
                    _image.SaveAs(path, 25);
                    return path;
                }
                catch (Exception ex)
                {
                    Log.ILog(ex.Message);
                }
                return "";
            }

        }

        public LogoSetting Logo(string sku)
        {
            try
            {
                LogoSetting logoConfiguration = new LogoSetting();
                string lib = sku.Substring(0, 1);
                string pro = sku.Substring(5, 3);
                string param = string.Format("{0}{1}", lib, pro);
                var pathConfiguration = productConfigurations.Value.Where(x => x.Name == param).FirstOrDefault();
                if (pathConfiguration != null)
                {
                    string[] setting = pathConfiguration.Value.Split("-"); ;
                    logoConfiguration.X = Convert.ToInt32(setting[0]);
                    logoConfiguration.Y = Convert.ToInt32(setting[1]);
                    logoConfiguration.W = Convert.ToInt32(setting[2]);
                    logoConfiguration.H = Convert.ToInt32(setting[3]);
                    logoConfiguration.T = Convert.ToInt32(setting[4]);
                    logoConfiguration.Width = Convert.ToInt32(sku.Substring(1, 4));
                    logoConfiguration.BasePath = BasePath(sku);
                    logoConfiguration.LogoPath = LogoPath(sku);
                }
                return logoConfiguration;
            }
            catch (Exception ex)
            {
                Log.ILog(ex.Message);
            }

            return null;
        }

        public string LogoPath(string sku)
        {
            try
            {

                string arte = sku.Substring(14, 21);
                string logo = arte.Substring(0, 7);
                string catg = arte.Substring(7, 2);
                int num = Convert.ToInt32(arte.Substring(0, 4));
                if (num <= 6403)
                    return string.Format("{0}art{4}{1}{4}{2}{4}A{4}{3}.png", pathConfiguration.Value.Root, catg, logo, arte, pathConfiguration.Value.Separator);
                else
                    return string.Format("{0}art{4}{1}{4}{2}{4}{3}.png", pathConfiguration.Value.Root, catg, logo, arte, pathConfiguration.Value.Separator);

            }
            catch (Exception ex)
            {
                Log.ILog(ex.Message);
            }
            return "";
        }

        public string BasePath(string sku)
        {
            try
            {
                string typeBase = sku.Substring(35, 2);
                string imageBase = sku.Substring(5, 9);
                return string.Format("{0}base{3}{1}{3}{2}.jpg", pathConfiguration.Value.Root, typeBase, imageBase, pathConfiguration.Value.Separator);

            }
            catch (Exception ex)
            {
                Log.ILog(ex.Message);
            }
            return "";
        }
    }
}
