using BazorPeliculas.Shared.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BazorPeliculas.Server.Controllers
{
    public class Controller<T>:ControllerBase where T : IElementoConId
    {
        public Controller(ApplicationDBContext context)
        {
            Context = context;
        }

        protected ApplicationDBContext Context { get; private set; }

        [HttpPost]
        public virtual async Task<ActionResult<int>> Post(T item)
        {
            Context.Add(item);
            await Context.SaveChangesAsync();
            return item.Id;
        }
    }
}
