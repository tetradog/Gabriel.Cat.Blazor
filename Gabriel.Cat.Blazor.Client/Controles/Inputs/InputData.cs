using Gabriel.Cat.S.Utilitats;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gabriel.Cat.Blazor.Client.Controles.Inputs
{
    internal class InputData<T> : ObjectBinding
    {
        private T data;

        public T Data { get => data; set { data = value; OnPropertyChanged(); } }
    }
}
