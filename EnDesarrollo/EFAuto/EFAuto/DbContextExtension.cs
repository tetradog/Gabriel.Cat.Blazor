using Gabriel.Cat.S.Extension;
using Gabriel.Cat.S.Utilitats;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFAuto
{
    public static class DbContextExtension
    {
        public static IEnumerable<Type> GetAllTypes(this DbContext context)
        {
            PropiedadTipo[] propiedades = context.GetType().GetPropiedadesTipos()
                                                            .Where(p => p.Tipo.IsGenericType && p.Tipo.GetGenericTypeDefinition().Equals(typeof(DbSet<>))).ToArray();
            Type[] propiedadesAux= propiedades.Select(p => p.Tipo.GetGenericArguments()[0]).ToArray();
            propiedadesAux = propiedadesAux.Where(p=>!p.FullName.Contains(nameof(Microsoft))).ToArray();
            SortedList<string, Type> dicTipos = new SortedList<string, Type>();

            foreach(Type propiedad in propiedadesAux)
            {
                foreach(Type tipo in propiedad.GetAllTypes())
                {
                    if (!dicTipos.ContainsKey(tipo.FullName))
                    {
                        dicTipos.Add(tipo.FullName, tipo);
                    }
                }
            }
            return dicTipos.GetValues().Select(t=>t.IsGenericType?t.GetGenericArguments()[0]:t);


        }
    }
}
