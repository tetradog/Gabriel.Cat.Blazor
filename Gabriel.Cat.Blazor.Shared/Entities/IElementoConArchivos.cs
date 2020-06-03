using System;
using System.Collections.Generic;
using System.Text;
using Gabriel.Cat.S.Extension;
namespace Gabriel.Cat.Blazor.Shared.Entities
{
    public interface IElementoConArchivos
    {
        IList<Archivo> Archivos { get; }
    }
    public class Archivo
    {
        public Archivo(object obj,string propiedad,string extension)
        {
            Propiedad = propiedad;
            Objeto = obj;
            Extension = extension;
        }
        public string Extension { get; set; }
         string Propiedad { get; set; }
         object Objeto { get; set; }
        public byte[] Data =>  Convert.FromBase64String(DataBase64);
        public bool HaveData => DataBase64 != null;
        string DataBase64 => (string)Objeto.GetProperty(Propiedad);
        public void SetUrl(string url)
        {
            Objeto.SetProperty(Propiedad, url);
        }
    }
}
