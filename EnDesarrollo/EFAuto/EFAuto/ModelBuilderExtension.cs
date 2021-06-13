using Gabriel.Cat.S.Extension;
using Gabriel.Cat.S.Utilitats;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

using System.Text;
using System.Threading.Tasks;

namespace EFAuto
{
    public static class ModelBuilderExtension
    {
        //de momento no funciona bien porque EF no lo usa
        //public static void ArreglaIds(this ModelBuilder modelBuilder, IEnumerable<Type> types)
        //{
        //    const string ID = "Id";


        //    IList<PropiedadTipo> propiedades;
        //    string propertyNavigationName;
        //    ForeignKeyAttribute foreignKeyAttribute;
        //    //las clases que no tengan clase intermedia la crea
        //    //PersonaEstudiante se tendria que crear
        //    foreach (Type tipo in types)
        //    {

        //        propiedades = tipo.GetPropiedadesTipos();
        //        if (!propiedades.Any(p => p.Atributos.Any(attr => attr.GetType().FullName.Equals(typeof(KeyAttribute).FullName)) || p.Nombre == ID))
        //        {
        //            //añado el campo Id
        //            if (!modelBuilder.ExistProperty(tipo, ID))
        //            {
        //                try
        //                {
        //                    modelBuilder.Entity(tipo).Metadata.AddProperty(ID, typeof(int));
        //                }
        //                catch { }
        //            }

        //        }
        //        for (int i = 0; i < propiedades.Count; i++)
        //        {
        //            //los que sean clases miro si tienen un campo Id o hay uno con ForeingKey apuntando al campo
        //            if (propiedades[i].Tipo.IsTypeValid())
        //            {
        //                foreignKeyAttribute = propiedades[i].Atributos.Where(attr => attr.Equals(typeof(ForeignKeyAttribute))).FirstOrDefault() as ForeignKeyAttribute;
        //                propertyNavigationName = !Equals(foreignKeyAttribute, default(Attribute)) ? foreignKeyAttribute.Name : propiedades[i].Nombre + "Id";
        //                if (!propiedades.Any(p => p.Nombre == propertyNavigationName) && !modelBuilder.ExistProperty(tipo,propertyNavigationName))
        //                {
        //                    try
        //                    {
        //                        if (propiedades[i].Atributos.Any(attr => attr.Equals(typeof(RequiredAttribute))))
        //                            modelBuilder.Entity(tipo).Metadata.AddProperty(propertyNavigationName, typeof(int));
        //                        else modelBuilder.Entity(tipo).Metadata.AddProperty(propertyNavigationName, typeof(int?));
        //                    }
        //                    catch {
        //                          }
        //                }
        //            }
        //        }

        //    }


        //}
        public static void AddNavigation(this ModelBuilder modelBuilder, IEnumerable<Type> types)
        {
            IList<PropiedadTipo> propiedades;
            foreach (Type tipo in types)
            {
                propiedades = tipo.GetPropiedadesTipos();
                for (int i = 0; i < propiedades.Count; i++)
                {
                    if (propiedades[i].Tipo.IsTypeValid())
                    {
                        modelBuilder.Entity(tipo)
                                    .Navigation(propiedades[i].Nombre)
                                    .UsePropertyAccessMode(PropertyAccessMode.Property);
                    }
                }
            }
        }
        public static void AddParentTables(this ModelBuilder modelBuilder, IEnumerable<Type> allTypesContext)
        {
            Type tipoAux;
            foreach (Type tipo in allTypesContext.GetBaseTypes())
            {

                tipoAux = tipo.CreateTypeOnlyPropType();
                modelBuilder.Model.AddEntityType(tipoAux);
            }


        }
        public static void AddChildTables(this ModelBuilder modelBuilder, IEnumerable<Type> allTypesContext)
        {
            Type tipoAux;
            foreach (Type tipo in allTypesContext)
            {
                if (!tipo.HeredaDirectoObj())
                {
                    tipoAux = tipo.CreateTypeOnlyPropType();
                    modelBuilder.Model.AddEntityType(tipoAux);
                }

          

            }


        }

        public static bool ExistProperty(this ModelBuilder modelBuilder, Type entity, string name)
        {
            return modelBuilder.Entity(entity).Metadata.GetDeclaredProperties().Any(p => p.Name.Equals(name)) || entity.GetPropiedadesTipos().Any(p => p.Nombre == name);
        }
    }
}
