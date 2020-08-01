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
        {//Gracias a la ayuda de Hazuky de es.StackOverFlow.com por su ayuda en el binding https://es.stackoverflow.com/questions/364370/problema-con-binding-en-blazor
            Value = (T)e.Value;

            return ValueChanged.InvokeAsync(Value);
        }

    }
}
