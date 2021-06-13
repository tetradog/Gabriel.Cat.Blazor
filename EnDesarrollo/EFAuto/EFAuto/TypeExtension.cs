using Gabriel.Cat.S.Extension;
using Gabriel.Cat.S.Utilitats;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace EFAuto
{
    public static class ObjectExtension
    {
        public static List<object> GetParts(this object obj)
        {
            object aux;
            IEnumerable<Type> tipos = obj.GetType().GetAncestros().Select(a => a.CreateTypeOnlyPropType());
            Type tipoBasico = obj.GetType().CreateTypeOnlyPropType();
            List<object> parts = new List<object>();
            foreach(Type tipo in tipos)
            {
                aux = Activator.CreateInstance(tipo);
                aux.SetPropertiesFromOther(obj);
                parts.Add(aux);
            }
            aux = Activator.CreateInstance(tipoBasico);
            aux.SetPropertiesFromOther(obj);
            parts.Add(aux);
            return parts;
        }
        public static object SetPropertiesFromOther(this object obj,object source)
        {
            IEnumerable<PropiedadTipo> propiedadesSource = source.GetType().GetPropiedadesTipos();
            IEnumerable<PropiedadTipo> propiedades = obj.GetType().GetPropiedadesTipos().Where(p=>propiedadesSource.Any(s=>p.Nombre.Equals(s.Nombre)));
            foreach(PropiedadTipo propiedad in propiedades)
            {
                obj.SetProperty(propiedad.Nombre, source.GetProperty(propiedad.Nombre));
            }
            return obj;
        }
    }
    public static class TypeExtension
    {
        public static Type CreateTypeOnlyPropType(this Type tipo, bool addIdProp = true, bool addIdParentIfNotIsObject = true, Type parentIdType = default(Type))
        {//source: https://www.c-sharpcorner.com/UploadFile/87b416/dynamically-create-a-class-at-runtime/
            const string NAMESPACE = "Generated";
            const string ID = "Id";
            Type tipoId = typeof(int);
            string parentTypeName;
            SortedList<string, PropiedadTipo> dicProp = new SortedList<string, PropiedadTipo>();

            AssemblyName asemblyName = new AssemblyName($"{NAMESPACE}.{tipo.Name}");
            AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(asemblyName, AssemblyBuilderAccess.Run);
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("MainModule");
            TypeBuilder typeBuilder = moduleBuilder.DefineType(asemblyName.FullName
                                                            , TypeAttributes.Public |
                                                            TypeAttributes.Class |
                                                            TypeAttributes.AutoClass |
                                                            TypeAttributes.AnsiClass |
                                                            TypeAttributes.BeforeFieldInit |
                                                            TypeAttributes.AutoLayout
                                                            , null);
            typeBuilder.DefineDefaultConstructor(MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName);
            foreach (PropiedadTipo propiedad in tipo.GetPropiedadesTipo())
            {
                typeBuilder.AddProperty(propiedad.Nombre, propiedad.Tipo);
                dicProp.Add(propiedad.Nombre, propiedad);
            }
            if (!dicProp.ContainsKey(ID) && addIdProp)
            {
                typeBuilder.AddProperty(ID, tipoId);
            }
            if (addIdParentIfNotIsObject && !tipo.HeredaDirectoObj() )
            {
                parentTypeName = $"{tipo.BaseType.Name}{ID}";
                if (Equals(parentIdType, default(Type)))
                    parentIdType = tipoId;
                typeBuilder.AddProperty(parentTypeName, parentIdType);
            }

            return typeBuilder.CreateType();
        }
        public static bool HeredaDirectoObj(this Type tipo)
        {
            return tipo.BaseType.Equals(typeof(Object)) || tipo.BaseType.Equals(typeof(object));
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
        public static IEnumerable<PropiedadTipo> GetPropiedadesTipo(this Type tipo)
        {
            IEnumerable<PropiedadTipo> propiedadesParent = tipo.BaseType.GetPropiedadesTipos();
            return  tipo.GetPropiedadesTipos().Where(p=>!propiedadesParent.Any(pParent=>pParent.Nombre==p.Nombre));


        }
        public static List<Type> AddBaseTypes(this IEnumerable<Type> tipos)
        {
            List<Type> tiposLst =new List<Type>(GetBaseTypes(tipos));
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

        public static IEnumerable<Type> GetAllTypes(this Type tipo, string[] filtroNotIncludeFullName =default, string[] filtroExceptionFullName = default, bool aplicarFiltroALosGenericos = true) 
        {
            if(Equals(filtroNotIncludeFullName,default(string[])))
                filtroNotIncludeFullName=new string[]{ nameof(System),nameof(Microsoft)};
            if (Equals(filtroExceptionFullName, default(string[])))
                filtroExceptionFullName = new string[] { nameof(ICollection) };
            return tipo.IGetAllTypes(filtroNotIncludeFullName, filtroExceptionFullName).Where(t => !aplicarFiltroALosGenericos || !t.IsGenericType || filtroNotIncludeFullName.Any(f=> !t.GetGenericArguments()[0].FullName.Contains(f)));
        }
        public static bool IsTypeValid(this Type tipo)
        {
            return !tipo.FullName.Contains(nameof(System)) && !tipo.ImplementInterficie(typeof(ICollection<>));
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
