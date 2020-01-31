using System;
using System.Collections.Generic;
using System.Text;

namespace Gabriel.Cat.Extension
{
   public static class ExtensionByteArray
    {
        [inject] IJSRuntime js;
        public static async ValueTask<string> GetUrl(this byte[] data)
        {
            return  js.InvokeAsync<string>("GetUri", data);
        }
    }
}
