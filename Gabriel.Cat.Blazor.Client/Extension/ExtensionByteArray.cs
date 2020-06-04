using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Gabriel.Cat.S.Extension
{
   public static class ExtensionByteArray
    {
        [Inject] static IJSRuntime JS { get; set; }
        public static async ValueTask<string> GetUrl(this byte[] data)
        {
            return await JS.InvokeAsync<string>("GetUri",new object[] { data });
        }
    }
}
