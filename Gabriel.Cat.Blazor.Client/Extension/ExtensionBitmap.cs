using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Gabriel.Cat.S.Extension;
using System.Threading.Tasks;

namespace Gabriel.Cat.Extension
{
    public static class ExtensionBitmap
    {
        public static async ValueTask<string> GetUrl(this Bitmap bmp)
        {
            return await bmp.GetBytes().GetUrl();
        }
        public static string ToSrcImg(this Bitmap bmp)
        {
            return "data:image/png;base64, " + Convert.ToBase64String(bmp.ToStream(System.Drawing.Imaging.ImageFormat.Png).ToArray());
        }
    }
}
