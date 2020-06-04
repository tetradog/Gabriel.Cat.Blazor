using AutoMapper;
using Gabriel.Cat.Blazor.Server;
using Gabriel.Cat.Blazor.Shared.Entities;
using Gabriel.Cat.S.Extension;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Gabriel.Cat.Blazor.Server.Controllers
{
    public abstract class Controller<TDbContext,T>:ControllerBase 
        where T :class, IElementoConId,new()
        where TDbContext:DbContextWithOutFiles
    {
        public Controller(TDbContext context,IMapper mapper)
        {
            Context = context;
            Mapper = mapper;
        }

        protected TDbContext Context { get; private set; }
        protected IMapper Mapper { get; private set; }
        
        public virtual async Task<ActionResult<int>> Post(T item)
        {
            Context.Add(item);
            await Context.SaveChangesAsync();
            return item.Id;
        }

        public virtual async Task<ActionResult<List<T>>> Get()
        {
            return await Task.FromResult(GetEnum().ToList());
        }

        public virtual async Task<ActionResult<List<T>>> Get(string textABuscar)
        {
            return await Task.FromResult(GetEnum().Filtra((item)=>string.IsNullOrEmpty(textABuscar)||item.ToString().ToLowerInvariant().Contains(textABuscar)).ToList());
        }

        public virtual async Task<ActionResult<T>> Get(int id)
        {
            ActionResult<T> result;
            T itemToReturn = GetItem(id);
            if (itemToReturn != default)
                result = await Task.FromResult(itemToReturn);
            else result = NotFound();
            return result;
        }

        protected T GetItem(int id)
        {
            T itemToReturn = default;
            GetEnum().WhileEach((item) =>
            {
                if (item.Id == id)
                    itemToReturn = item;
                return item.Id != id;
            });
            return itemToReturn;
        }

        protected IEnumerable<T> GetEnum()
        {
            return (IEnumerable<T>)Context.GetProperty(Context.GetPropertyName<T>());
        }

        public virtual async Task<ActionResult> Delete(int id)
        {
            ActionResult result;
            T itemEncontrado=default;
            bool exist = GetEnum().Any((item) => {

                if (item.Id == id)
                    itemEncontrado = item;

                return item.Id == id;
                    });
            if (exist)
            {
                Context.Remove(itemEncontrado);
                await Context.SaveChangesAsync();
                result = NoContent();
            }
            else
            {
                result = NotFound();
            }
            return result;
        }
 
        public virtual async Task<ActionResult> Put(T item)
        {
            ActionResult result;
            T tBD = GetItem(item.Id);

            if (!Equals(tBD, default))
            {
                Mapper.Map(item, tBD);
                await Context.SaveChangesAsync();
                result = NoContent();
            }
            else result = NotFound();
            return result;
        }
   
    }
}
