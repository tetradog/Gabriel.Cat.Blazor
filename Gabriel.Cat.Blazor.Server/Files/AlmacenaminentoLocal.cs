using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Gabriel.Cat.Blazor.Server.Helpers
{
    public class AlmacenaminentoLocal : IAlmacenamientoDeArchivos
    {
        IWebHostEnvironment Enviorment { get; set; }
        IHttpContextAccessor HttpContextAccessor { get; set; }
        public AlmacenaminentoLocal(IWebHostEnvironment enviorment,IHttpContextAccessor httpContextAccessor)
        {
            Enviorment = enviorment;
            HttpContextAccessor = httpContextAccessor;
        }


        public  Task EliminarArchivo(string ruta, string nombreContenedor)
        {
            string fileName=Path.Combine(Enviorment.WebRootPath,nombreContenedor,Path.GetFileName(ruta));
            if (File.Exists(fileName))
                File.Delete(fileName);
            return Task.FromResult(0);
        }

        public async Task<string> GuardarArchivo(byte[] contenido, string extension, string nombreContenedor)
        {
            string rutaActual = $"{HttpContextAccessor.HttpContext.Request.Scheme}://{HttpContextAccessor.HttpContext.Request.Host}";
            string filename = $"{Guid.NewGuid()}.{extension}";
            string folder = Path.Combine(Enviorment.WebRootPath, nombreContenedor);
            string rutaGuardado = Path.Combine(folder, filename);
            
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);
            
            File.WriteAllBytes(rutaGuardado, contenido);
            
            return Path.Combine(rutaActual,nombreContenedor,filename);
        }
    }
}
