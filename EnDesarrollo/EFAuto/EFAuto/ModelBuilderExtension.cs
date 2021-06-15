using Gabriel.Cat.S.Extension;
using Gabriel.Cat.S.Utilitats;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Dynamic;
using System.Linq;

using System.Text;
using System.Threading.Tasks;

namespace EFAuto
{
    public static class ModelBuilderExtension
    {
        //de momento no funciona bien porque EF no lo usa
        public static void ArreglaIds(this ModelBuilder modelBuilder, IEnumerable<Type> types)
        {
            const string ID = "Id";


            IEnumerable<PropiedadTipo> propiedades;
            string propertyNavigationName;
            ForeignKeyAttribute foreignKeyAttribute;
            //las clases que no tengan clase intermedia la crea
            //PersonaEstudiante se tendria que crear
            foreach (Type tipo in types)
            {

                propiedades = tipo.GetPropiedadesTipo();
                if (!propiedades.Any(p => p.Atributos.Any(attr => attr.GetType().FullName.Equals(typeof(KeyAttribute).FullName)) || p.Nombre == ID))
                {
                    //añado el campo Id
                    if (!modelBuilder.ExistProperty(tipo, ID))
                    {
                        try
                        {
                            modelBuilder.Entity(tipo).Metadata.AddProperty(ID, typeof(int));
                        }
                        catch { }
                    }

                }
                foreach (PropiedadTipo propiedad in propiedades)
                {
                    //los que sean clases miro si tienen un campo Id o hay uno con ForeingKey apuntando al campo
                    if (propiedad.Tipo.IsTypeValidOne())
                    {
                        foreignKeyAttribute = propiedad.Atributos.Where(attr => attr.Equals(typeof(ForeignKeyAttribute))).FirstOrDefault() as ForeignKeyAttribute;
                        propertyNavigationName = !Equals(foreignKeyAttribute, default(Attribute)) ? foreignKeyAttribute.Name : propiedad.Nombre + "Id";
                        if (!propiedades.Any(p => p.Nombre == propertyNavigationName) && !modelBuilder.ExistProperty(tipo, propertyNavigationName))
                        {
                            try
                            {
                                if (propiedad.Atributos.Any(attr => attr.Equals(typeof(RequiredAttribute))))
                                    modelBuilder.Entity(tipo).Metadata.AddProperty(propertyNavigationName, typeof(int));
                                else modelBuilder.Entity(tipo).Metadata.AddProperty(propertyNavigationName, typeof(int?));
                            }
                            catch
                            {
                            }
                        }
                    }
                }

            }


        }
        public static void AddNavigation(this ModelBuilder modelBuilder, IEnumerable<Type> types)
        {

            foreach (Type tipo in types)
            {

                foreach (PropiedadTipo propiedad in tipo.GetPropiedadesTipo())
                {
                    if (propiedad.Tipo.IsTypeValidOne())
                    {
                        modelBuilder.Entity(tipo.HeredaDirectoObj() ? tipo : tipo.GenFullType().GetType())
                                    .Navigation(propiedad.Nombre)
                                    .UsePropertyAccessMode(PropertyAccessMode.Property);
                    }
                }
            }
        }


        public static bool ExistProperty(this ModelBuilder modelBuilder, Type entity, string name)
        {
            return modelBuilder.Entity(entity).Metadata.GetDeclaredProperties().Any(p => p.Name.Equals(name)) || entity.GetPropiedadesTipo().Any(p => p.Nombre == name);
        }
        public static void ConfigureRelationTypes(this ModelBuilder modelBuilder, IList<Type> types)
        {
            Type tipo;
            PropiedadTipo[] propiedades;
            NavigationProperty propiedadDestino;
            dynamic builder;
            for (int i = 0; i < types.Count; i++)
            {
                if (!types[i].HeredaDirectoObj())
                {
                    throw new NotSupportedException($"El tipo '{types[i]}' no hereda directamente de '{typeof(object)}' permitida!");
                }
                tipo = types[i];
                //configuro las relaciones
                propiedades = tipo.GetPropiedadesTipo().ToArray();
                foreach (PropiedadTipo propiedad in propiedades.Where(p => p.Tipo.IsGenericType && !p.Tipo.GetGenericArguments()[0].FullName.StartsWith(nameof(System)) || !p.Tipo.FullName.StartsWith(nameof(System))))
                {
                    try
                    {
                        propiedadDestino = tipo.GetPropertyNavigation(propiedad);
                        if (propiedadDestino.IsValid)
                        {
                            if (propiedadDestino.IsFromMany)
                            {
                                builder = modelBuilder.Entity(tipo)
                                                      .HasMany(propiedad.Nombre);
                            }
                            else
                            {
                                builder = modelBuilder.Entity(tipo)
                                                   .HasOne(propiedad.Nombre);
                            }

                            if (propiedadDestino.IsToMany)
                            {
                                if (propiedadDestino.HasNavigationTo)
                                    builder.WithMany(propiedadDestino.Nombre);
                                else builder.WithMany();
                            }
                            else
                            {
                                if (propiedadDestino.HasNavigationTo)
                                    builder.WithOne(propiedadDestino.Nombre);
                                else builder.WithOne();
                            }
                        }

                    }
                    catch { }
                }
            }
        }
        public static NavigationProperty GetPropertyNavigation(this Type tipo, PropiedadTipo propiedadTipo)
        {
            NavigationProperty navigation = new NavigationProperty();
            //la propiedad del tipo tiene un Tipo y este puede o no tener navegación hacia esa propiedad,
            //contiene el nombre en caso de 1-n al revés ya es otro tema pero se podria mirar
            //si hay el nombre del tipo en la propiedad del n y si es así es probable que la otra sea el otro tipo
            //tener en cuenta el atributo ForeignKey para saber el nombre ya que puede ser que haya más de una lista y para no tener esa ambiguedad
            PropiedadTipo foreignKey = propiedadTipo.Tipo.GetPropiedadesTipo()
                                                             .Where(p => p.Atributos.Where(a => a is ForeignKeyAttribute).Any(a => (a as ForeignKeyAttribute).Name == propiedadTipo.Nombre))
                                                             .FirstOrDefault() as PropiedadTipo;
            navigation.IsFromMany = propiedadTipo.Tipo.IsGenericType;

            if (!Equals(foreignKey, default(PropiedadTipo)))
            {
                navigation.Nombre = foreignKey.Nombre;
                navigation.IsValid = true;
                navigation.IsToMany = foreignKey.Tipo.IsGenericType;

            }
            else
            {
                //será por convención o no será

            }
            




            return navigation;
        }
        public class NavigationProperty
        {
            public bool IsValid { get; set; }
            public string Nombre { get; set; }
            public bool IsFromMany { get; set; }
            public bool IsToMany { get; set; }
            public bool HasNavigationTo => !String.IsNullOrEmpty(Nombre);
        }
    }
}
