using Gabriel.Cat.S.Extension;
using Gabriel.Cat.S.Utilitats;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace EFAuto
{
    public static class TypeExtension
    {
        public static IEnumerable<Type> GetAllTypes(this Type tipo, string[] filtroNotIncludeFullName =default, string[] filtroExceptionFullName = default, bool aplicarFiltroALosGenericos = true) 
        {
            if(Equals(filtroNotIncludeFullName,default(string[])))
                filtroNotIncludeFullName=new string[]{ nameof(System),nameof(Microsoft)};
            if (Equals(filtroExceptionFullName, default(string[])))
                filtroExceptionFullName = new string[] { nameof(ICollection) };
            return tipo.IGetAllTypes(filtroNotIncludeFullName, filtroExceptionFullName).Where(t => !aplicarFiltroALosGenericos || !t.IsGenericType || filtroNotIncludeFullName.Any(f=> !t.GetGenericArguments()[0].FullName.Contains(f)));
        }
         static Type[] IGetAllTypes(this Type tipo)
        {
            return IGetAllTypes(tipo, new SortedList<string, Type>());

        }
         static IEnumerable<Type> IGetAllTypes(this Type tipo, string[] notContainsOnFullName, string[] filtroExceptionFullName)
        {
            return IGetAllTypes(tipo).Where(t => !notContainsOnFullName.Any(f => t.FullName.Contains(f)) || filtroExceptionFullName.Any(f => t.FullName.Contains(f)));

        }
        static Type[] IGetAllTypes(Type dbSet, SortedList<string, Type> dicTipos)
        {
            IList<PropiedadTipo> propiedadesTipos;

            if (!dicTipos.ContainsKey(dbSet.FullName))
            {
                propiedadesTipos = dbSet.GetPropiedadesTipos();
                dicTipos.Add(dbSet.FullName, dbSet);
                for (int i = 0; i < propiedadesTipos.Count; i++)
                {
                    IGetAllTypes(propiedadesTipos[i].Tipo, dicTipos);
                }
            }
            return dicTipos.GetValues();


        }

    }
}
