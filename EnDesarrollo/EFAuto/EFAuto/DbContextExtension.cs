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
            SortedList<string, Type> dicTipos;
            PropiedadTipo[] propiedades = context.GetType().GetPropiedadesTipo()
                                                            .Where(p => p.Tipo.IsGenericType && p.Tipo.GetGenericTypeDefinition().Equals(typeof(DbSet<>)))
                                                            .ToArray();
            Type[] propiedadesAux= propiedades.Select(p => p.Tipo.GetGenericArguments()[0])
                                              .Where(p => !p.FullName.Contains(nameof(Microsoft)))
                                              .ToArray();
            dicTipos = new SortedList<string, Type>();

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
            return dicTipos.GetValues();


        }
    }
}
