using Blazor.FileReader;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;


namespace Gabriel.Cat.S.Extension
{
    public static class ExtensionIList
    {
        public static T Last<T>(this IList<T> lst)
        {
            return lst.Count>0? lst[lst.Count - 1]:default;
        }
    }
}
