using Gabriel.Cat.S.Extension;
using Gabriel.Cat.S.Utilitats;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace EFAuto
{
    public static class TypeExtension
    {
        static SortedList<string, dynamic> dicTipos = new SortedList<string, dynamic>();
        static SortedList<string, TypeBuilder> dicTipoBuilders = new SortedList<string, TypeBuilder>();
        public static bool IsTypeValidOne(this Type tipo)
        {
            return !tipo.FullName.Contains(nameof(System)) && !tipo.ImplementInterficie(typeof(ICollection<>));
        }
        public static IEnumerable<Type> ArreglaIds(this IEnumerable<Type> types)
        {
            return types.Select(t=>t.ArreglaId());
        }
        public static Type ArreglaId(this Type type)
        {
            const string ID = "Id";
            Type tipoGen;
            TypeBuilder builder = type.GetTypeBuilder();
            IEnumerable<PropiedadTipo> propiedades = type.GetPropiedadesTipo();

            if(!propiedades.Any(p=>p.Nombre==ID ||p.Nombre== p.Tipo.Name + ID))
            {
                builder.AddProperty(ID, typeof(int));
            }
            foreach(PropiedadTipo propiedad in propiedades.Where(p => p.Tipo.IsTypeValidOne()))
            {
                if (!propiedades.Any(p => p.Nombre == propiedad.Nombre + ID))
                {
                    if(propiedad.Atributos.Any(a=>a.Equals(typeof(RequiredAttribute))))
                    {
                        builder.AddProperty(propiedad.Nombre + ID, typeof(int));
                    }
                    else
                    {
                        builder.AddProperty(propiedad.Nombre + ID, typeof(int?));
                    }
                }
            }
            tipoGen= builder.CreateType();
            return tipoGen.GetPropiedadesTipo().Count() == propiedades.Count() ? type : tipoGen;
        }

        public static ExpandoObject GenFullType(this Type tipo, bool isRecursiveProperties = true)
        {

            IEnumerable<PropiedadTipo> propiedades = tipo.GetPropiedadesTipo();
            PropiedadTipo[] faltantes = propiedades.Where(p => !dicTipos.ContainsKey(p.Tipo.FullName)).ToArray();
            ExpandoObject tipoGen = tipo.GetGenOrEmpty();
            if (faltantes.Length > 0)
            {
                foreach (PropiedadTipo propiedad in faltantes.Where(p => !p.Tipo.FullName.Contains(nameof(System))))
                {
                    propiedad.Tipo.GenFullType();
                }

                foreach (PropiedadTipo propiedad in propiedades)
                {
                    if (!propiedad.Tipo.GetPropiedadesTipo().Any(p => p.Tipo.Equals(tipo)))
                        tipoGen.TryAdd(propiedad.Nombre, isRecursiveProperties ? propiedad.Tipo.GetGenOrEmpty() : propiedad.Tipo);
                }
                tipoGen.TryAdd("Type", tipo.GetGenName());


            }
            return tipoGen;
        }
        static string GetGenName(this Type tipo)
        {
            const string NAMESPACE = "Generated";
            return $"{NAMESPACE}.{tipo.Name}";
        }
        public static TypeBuilder GetTypeBuilder(this Type tipo,bool includeAllProperties=true)
        {
            //source: https://www.c-sharpcorner.com/UploadFile/87b416/dynamically-create-a-class-at-runtime/
            //recursividad infinita


            AssemblyName asemblyName;
            AssemblyBuilder assemblyBuilder;
            ModuleBuilder moduleBuilder;
            TypeBuilder typeBuilder;
          
            asemblyName = new AssemblyName(tipo.GetGenName());
            assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(asemblyName, AssemblyBuilderAccess.Run);
            moduleBuilder = assemblyBuilder.DefineDynamicModule("MainModule");
            typeBuilder = moduleBuilder.DefineType(asemblyName.FullName
                                                            , TypeAttributes.Public |
                                                            TypeAttributes.Class |
                                                            TypeAttributes.AutoClass |
                                                            TypeAttributes.AnsiClass |
                                                            TypeAttributes.BeforeFieldInit |
                                                            TypeAttributes.AutoLayout
                                                            , null);
            typeBuilder.DefineDefaultConstructor(MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName);
            return typeBuilder;
        }
        static ExpandoObject GetGenOrEmpty(this Type tipo)
        {
            if (!dicTipos.ContainsKey(tipo.FullName))
            {
                dicTipos.Add(tipo.FullName, new ExpandoObject());
            }
            return dicTipos[tipo.FullName];
        }



        public static bool HeredaDirectoObj(this Type tipo)
        {
            return Equals(tipo.BaseType, default) || tipo.BaseType.Equals(typeof(Object)) || tipo.BaseType.Equals(typeof(object));
        }

        public static List<Type> GetAncestros(this Type tipo)
        {
            List<Type> tAncestros = new List<Type>();
            if (!tipo.BaseType.Equals(typeof(Object)))
            {
                tAncestros.Add(tipo.BaseType);
                tAncestros.AddRange(tipo.BaseType.GetAncestros());
            }
            return tAncestros;
        }
        public static IEnumerable<PropiedadTipo> GetPropiedadesTipoAncestro(this Type tipo)
        {
            IEnumerable<PropiedadTipo> propiedadesParent = tipo.BaseType.GetPropiedadesTipo();
            return tipo.GetPropiedadesTipo().Where(p => !propiedadesParent.Any(pParent => pParent.Nombre == p.Nombre));


        }
        public static Type GenType(this Type tipo,[NotNull]IEnumerable<PropiedadTipo> propiedades)
        {
            TypeBuilder builder = tipo.GetTypeBuilder();
            foreach(PropiedadTipo propiedad in propiedades)
             builder.AddProperty(propiedad.Nombre, propiedad.Tipo);
            return builder.CreateType();
        }
        public static IEnumerable<PropiedadTipo> GetPropiedadesTipoObj(this Type tipo)
        {
            IEnumerable<PropiedadTipo> propiedades = tipo.GetPropiedadesTipo();
            return tipo.GetPropiedadesTipo().Where(p => !propiedades.Any(pObj => pObj.Nombre == p.Nombre));


        }
        public static List<Type> AddBaseTypes(this IEnumerable<Type> tipos)
        {
            List<Type> tiposLst = new List<Type>(GetBaseTypes(tipos));
            tiposLst.AddRange(tipos);
            return tiposLst;
        }
        public static IEnumerable<Type> GetBaseTypes(this IEnumerable<Type> tipos)
        {
            List<Type> tiposLst = new List<Type>();
            List<Type> ancestros;
            foreach (Type tipo in tipos)
            {
                ancestros = tipo.GetAncestros();
                tiposLst.AddRange(ancestros.Except(tiposLst));
            }
            return tiposLst.Except(tipos);
        }

        public static IEnumerable<Type> GetAllTypes(this Type tipo, string[] filtroNotIncludeFullName = default, string[] filtroExceptionFullName = default, bool aplicarFiltroALosGenericos = true)
        {
            if (Equals(filtroNotIncludeFullName, default(string[])))
                filtroNotIncludeFullName = new string[] { nameof(System), nameof(Microsoft) };
            if (Equals(filtroExceptionFullName, default(string[])))
                filtroExceptionFullName = new string[] { nameof(ICollection) };
            return tipo.IGetAllTypes(filtroNotIncludeFullName, filtroExceptionFullName).Where(t => !aplicarFiltroALosGenericos || !t.IsGenericType || filtroNotIncludeFullName.Any(f => !t.GetGenericArguments()[0].FullName.Contains(f)));
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
            if (!dicTipos.ContainsKey(dbSet.FullName))
            {
                dicTipos.Add(dbSet.FullName, dbSet);
                foreach (PropiedadTipo propiedadTipo in dbSet.GetPropiedadesTipo())
                {
                    IGetAllTypes(propiedadTipo.Tipo, dicTipos);
                }
            }
            return dicTipos.GetValues();


        }

    }
}
