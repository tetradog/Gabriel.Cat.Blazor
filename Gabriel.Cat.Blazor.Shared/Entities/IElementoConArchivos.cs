﻿using System;
using System.Collections.Generic;
using System.Text;
using Gabriel.Cat.S.Extension;
namespace Gabriel.Cat.Blazor.Shared.Entities
{
    public interface IElementoConArchivos
    {
        IList<Archivo> Archivos { get; }
    }
}
