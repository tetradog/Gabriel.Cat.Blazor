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
        [Parameter]
        public EventCallback<T> ValueChanged { get; set; }
        
        protected Task OnValueChanged(ChangeEventArgs e)
        {
            Value = (T)e.Value;

            return ValueChanged.InvokeAsync(Value);
        }

    }
}
