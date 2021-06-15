using Gabriel.Cat.S.Extension;
using Gabriel.Cat.S.Utilitats;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;

namespace EFAuto
{
    public static class ObjectExtension
    {
        public static List<object> GetParts(this object obj)
        {
            object aux;
            IEnumerable<Type> tipos = obj.GetType().GetAncestros();
            Type tipoBasico =obj.GetType().GenType(obj.GetType().GetPropiedadesTipoObj());
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
            IEnumerable<PropiedadTipo> propiedadesSource = source.GetType().GetPropiedadesTipo();
            IEnumerable<PropiedadTipo> propiedades = obj.GetType().GetPropiedadesTipo().Where(p=>propiedadesSource.Any(s=>p.Nombre.Equals(s.Nombre)));
            foreach(PropiedadTipo propiedad in propiedades)
            {
                obj.SetProperty(propiedad.Nombre, source.GetProperty(propiedad.Nombre));
            }
            return obj;
        }
    }
}
