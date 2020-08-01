using Gabriel.Cat.S.Utilitats;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Gabriel.Cat.S.Extension
{
    public static class ExtensionColor
    {
        public static string ToHtml(this Color color)
        {//rgba
            return (Hex)new byte[] {color.R,color.G,color.B,color.A };
        }
    }
}
