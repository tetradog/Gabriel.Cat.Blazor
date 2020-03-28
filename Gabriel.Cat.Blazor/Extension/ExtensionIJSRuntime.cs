using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Gabriel.Cat.Extension
{
    public static class IJSRuntimeExtension
    {
        public static async Task<bool> Confirm(this IJSRuntime js, string mensaje)
        {

            return await js.InvokeAsync<bool>("confirm", mensaje);

        }
    }
}
