using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Gabriel.Cat.S.Extension;
namespace Gabriel.Cat.Extension
{
    public static class ExtensionBitmap
    {
        public static async ValueTask<string> GetUrl(this Bitmap bmp)
        {
            return bmp.GetBytes().GetUrl();
        }
    }
}
