using System;
using System.Collections.Generic;
using System.Text;

namespace Gabriel.Cat.Blazor.Shared
{
    public interface IArchivos
    {
        IList<Archivo> Archivos { get; }
    }
}
