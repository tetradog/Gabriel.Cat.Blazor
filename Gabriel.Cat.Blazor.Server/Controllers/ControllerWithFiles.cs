using AutoMapper;
using BlazorPeliculas.Server.Helpers;
using BlazorPeliculas.Shared;
using BlazorPeliculas.Shared.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorPeliculas.Server.Controllers
{

    public abstract class ControllerWithFiles<TBDContext,T> : Controller<TBDContext,T>
        where T:class,IElementoConId,IArchivos,new()
        where TBDContext: DbContextWithFiles
    {
        protected IAlmacenamientoDeArchivos Almacenamiento { get; set; }
        /// <summary>
        /// Name of Container default:Type.Name
        /// </summary>
        protected string Folder { get; set; }
        
        public ControllerWithFiles(TBDContext context, IAlmacenamientoDeArchivos almacenadorDeArchivos, IMapper mapper) : base(context,mapper)
        {
            Almacenamiento = almacenadorDeArchivos;
            Folder = default;
        }

        public override async Task<ActionResult<int>> Post(T itemConArchivos)
        {
            IList<Archivo> archivos = itemConArchivos.Archivos;
            for(int i=0;i<archivos.Count;i++)
            if (archivos[i].NeedSave)
            {
                    archivos[i].Url= await Almacenamiento.GuardarArchivo<T>(archivos[i].GetData(), archivos[i].Extension,Folder);
                    archivos[i].DataBase64 = default;
            }
            return await base.Post(itemConArchivos);
        }

        public override async Task<ActionResult> Put(T itemConArchivos)
        {
            IList<Archivo> archivos =itemConArchivos.Archivos;

            for (int i = 0; i < archivos.Count; i++)
            {
                if (archivos[i].NeedUpdate)
                {
                    //se tiene que actualizar
                    archivos[i].Url=await Almacenamiento.EditarArchivo<T>(archivos[i].GetData(), archivos[i].Extension,archivos[i].Url,Folder);
                }
                else if (archivos[i].NeedSave)
                {
                    //es la primera vez que tiene un elemento
                    archivos[i].Url = await Almacenamiento.GuardarArchivo<T>(archivos[i].GetData(), archivos[i].Extension,Folder);
                }
                archivos[i].DataBase64 = default;
       
            }
            return await base.Put(itemConArchivos);
        }

    }
}
