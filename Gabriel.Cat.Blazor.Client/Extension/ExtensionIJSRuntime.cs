using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Gabriel.Cat.Extension
{
    public static class IJSRuntimeExtension
    {
        public static async Task MostrarMensajeAsync(this IJSRuntime js, string mensaje)
        {
            await js.InvokeVoidAsync("alert", mensaje);
        }
        public static async Task DownloadFileStringAsync(this IJSRuntime js, string fileName, string data, string fileType, string charset = "utf-8")
        {

            await js.InvokeVoidAsync("StringSaveAsFile", fileName, data, fileType, charset);
        }

        public static async Task DownloadFileBinaryAsync(this IJSRuntime js, string fileName, byte[] data)
        {//no funciona
            await js.InvokeVoidAsync("SaveAsFile", fileName, data);
        }
        public static async Task SaveLocalStorageAsync(this IJSRuntime js, string id, string data)
        {
            await js.InvokeVoidAsync("SaveLocalStorage", id, data);
        }

        public static async Task<string> LoadLocalStorageAsync(this IJSRuntime js, string id)
        {
            return await js.InvokeAsync<string>("LoadLocalStorage", id);
        }
        public static async ValueTask<bool> PreguntaAsync(this IJSRuntime js, string mensaje)
        {
            return await js.InvokeAsync<bool>("confirm", mensaje);

        }
    }
}
