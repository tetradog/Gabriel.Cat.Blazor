using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gabriel.Cat.Blazor.Server.Helpers
{
    public interface IAlmacenamientoDeArchivos
    {
        async Task<string> EditarArchivo(byte[] contenido, string extension, string nombreContenedor, string rutaArchivo)
        {
            if (!string.IsNullOrEmpty(rutaArchivo))
            {
                await EliminarArchivo(rutaArchivo, nombreContenedor);
            }
            return await GuardarArchivo(contenido, extension, nombreContenedor);
        }
        Task EliminarArchivo(string ruta, string nombreContenedor);
        Task<string> GuardarArchivo(byte[] contenido, string extension, string nombreContenedor);
    }
}
