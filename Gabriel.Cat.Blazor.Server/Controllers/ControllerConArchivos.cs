using BazorPeliculas.Server.Helpers;
using BazorPeliculas.Shared.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BazorPeliculas.Server.Controllers
{

    public class ControllerConArchivos<T> : Controller<T> where T:IElementoConId,IElementoConArchivos,new()
    {
        IAlmacenamientoDeArchivos Almacenamiento { get; set; }
        /// <summary>
        /// Name of Container default:Type.Name
        /// </summary>
        protected string Folder { get; set; }
        
        public ControllerConArchivos(ApplicationDBContext context, IAlmacenamientoDeArchivos almacenadorDeArchivos) : base(context)
        {
            Almacenamiento = almacenadorDeArchivos;
            Folder = typeof(T).Name;
        }

        public override async Task<ActionResult<int>> Post(T itemConArchivos)
        {
            IList<Archivo> archivos = itemConArchivos.Archivos;
            for(int i=0;i<archivos.Count;i++)
            if (archivos[i].HaveData)
            {
                    archivos[i].SetUrl( await Almacenamiento.GuardarArchivo(archivos[i].Data, archivos[i].Extension,Folder));
            }
            return await base.Post(itemConArchivos);
        }

    }
}
