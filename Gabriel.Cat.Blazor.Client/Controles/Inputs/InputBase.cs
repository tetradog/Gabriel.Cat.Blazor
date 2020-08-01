using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace Gabriel.Cat.Blazor.Client.Controles.Inputs
{
    public class InputBase<T> : ComponentBase
    {

        [Parameter]
        public T Value { get; set; }

    }
}
