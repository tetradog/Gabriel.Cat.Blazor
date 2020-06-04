using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gabriel.Cat.S.Extension
{
    public static class ExtensionNavigateTo
    {
        public static void NavigateTo<T>(this NavigationManager navigator)
        {
            navigator.NavigateTo($"/{typeof(T).Name}");
        }
    }
}
