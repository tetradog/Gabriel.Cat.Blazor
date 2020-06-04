using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Gabriel.Cat.Blazor.Shared
{
    public class Archivo
    {
        public Archivo() { }
        public Archivo(byte[] data,string extension)
        {
            SetData(data);
            Extension = extension;
        }
        public int Id { get; set; }
        public string Url { get; set; }
        [NotMapped]
        public string DataBase64 { get; set; }
        public string Extension { get; set; }
        public bool NeedUpdate => !string.IsNullOrEmpty(Url) && !string.IsNullOrEmpty(DataBase64);
        public bool NeedSave => string.IsNullOrEmpty(Url) && !string.IsNullOrEmpty(DataBase64);

        public byte[] GetData()
        {
            return Convert.FromBase64String(DataBase64);
        }
        public void SetData(byte[] data)
        {
            if (data == default)
                throw new ArgumentNullException("data");

            DataBase64 = Convert.ToBase64String(data);
        }
    }
}
