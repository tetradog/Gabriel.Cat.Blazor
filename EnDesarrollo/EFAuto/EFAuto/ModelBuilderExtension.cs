﻿using Gabriel.Cat.S.Extension;
using Gabriel.Cat.S.Utilitats;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Dynamic;
using System.Linq;
using System.Reflection;
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

                foreach (PropiedadTipo propiedad in tipo.GetPropiedadesTipo().Where(p => p.Tipo.IsGenericType ? !p.Tipo.GetGenericArguments()[0].FullName.Contains(nameof(System)) : !p.Tipo.FullName.Contains(nameof(System))))
                {

                    modelBuilder.Entity(tipo)
                                .Navigation(propiedad.Nombre)
                                .UsePropertyAccessMode(PropertyAccessMode.Property);

                }
            }
        }


        public static bool ExistProperty(this ModelBuilder modelBuilder, Type entity, string name)
        {
            return modelBuilder.Entity(entity).Metadata.GetDeclaredProperties().Any(p => p.Name.Equals(name)) || entity.GetPropiedadesTipo().Any(p => p.Nombre == name);
        }
        public static void ConfigureKeys(this ModelBuilder modelBuilder,IList<Type> types)
        {
            MethodInfo method;
            MethodInfo generic;

            method = typeof(ModelBuilderExtension).GetMethod(nameof(IConfigureKey), BindingFlags.Static | BindingFlags.NonPublic);
            for (int i = 0; i < types.Count; i++)
            {
                generic = method.MakeGenericMethod(types[i]);
                generic.Invoke(null, new object[] { modelBuilder });
            }
        }
        public static void ConfigureKey<TEntity>(this ModelBuilder modelBuilder) where TEntity : class, new()
        {
            modelBuilder.IConfigureKey<TEntity>();
        }
        private static void IConfigureKey<TEntity>(this ModelBuilder modelBuilder) where TEntity : class, new()
        {
            const string KEY = nameof(KeyAttribute);
            IList<PropiedadTipo> propiedades = typeof(TEntity).GetPropiedadesTipo();
            List<string> keys = new List<string>();
            for (int i = 0; i < propiedades.Count; i++)
            {
                if (propiedades[i].Atributos.Any(attr => attr.GetType().Name == KEY))
                    keys.Add(propiedades[i].Nombre);
            }
            if (keys.Count > 1)
                modelBuilder.Entity<TEntity>().HasKey(keys.ToArray());

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
                            //falta cuando hay más de uno pero no hay nombre porque no es un ICollection sino más de una propiedad de ese tipo
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
                                    builder = builder.WithMany(propiedadDestino.Nombre);
                                else builder = builder.WithMany();
                            }
                            else
                            {
                                if (propiedadDestino.HasNavigationTo)
                                    builder = builder.WithOne(propiedadDestino.Nombre);
                                else builder = builder.WithOne();
                            }
                            if (propiedadDestino.HasNavigationTo)
                            {
                                
                                     if(propiedadDestino.HasIdProperty)
                                      builder.HasForeignKey(propiedad.Tipo,propiedadDestino.NombreId);
                                    else builder.HasForeignKey(propiedadDestino.Nombre);


                            }
                        }

                    }
                    catch (Exception ex)
                    {

                        System.Diagnostics.Debugger.Break();
                    }
                }
            }
        }
        public static NavigationProperty GetPropertyNavigation(this Type tipo, PropiedadTipo propiedadFrom)
        {
            PropiedadTipo[] foreignKey;
            Type tipoFrom;
            string nombrePropiedadDetino;
            NavigationProperty navigation = new NavigationProperty();
            //la propiedad del tipo tiene un Tipo y este puede o no tener navegación hacia esa propiedad,
            //contiene el nombre en caso de 1-n al revés ya es otro tema pero se podria mirar
            //si hay el nombre del tipo en la propiedad del n y si es así es probable que la otra sea el otro tipo
            //tener en cuenta el atributo ForeignKey para saber el nombre ya que puede ser que haya más de una lista y para no tener esa ambiguedad


            navigation.IsFromMany = propiedadFrom.Tipo.IsGenericType;
            if (navigation.IsFromMany)
            {
                tipoFrom = propiedadFrom.Tipo.GetGenericArguments()[0];

            }
            else
            {
                tipoFrom = propiedadFrom.Tipo;

            }
            nombrePropiedadDetino = propiedadFrom.Atributos.Where(a => a is ForeignKeyAttribute).Select(a => (a as ForeignKeyAttribute).Name).FirstOrDefault();
            if (String.IsNullOrEmpty(nombrePropiedadDetino))
            {
                nombrePropiedadDetino = propiedadFrom.Nombre;
                foreignKey = tipoFrom.GetPropiedadesTipo()
                                                        .Where(p => p.Atributos.Where(a => a is ForeignKeyAttribute).Any(attrPropiedadDestino => (attrPropiedadDestino as ForeignKeyAttribute).Name == nombrePropiedadDetino))
                                                        .Where(p => p.Tipo.Equals(tipo))
                                                        .ToArray();

                if (foreignKey.Length == 0)
                {
                    foreignKey = tipoFrom.GetPropiedadesTipo()
                                        .Where(p => p.Nombre.Contains(tipo.Name) && (p.Tipo.IsGenericType ? !p.Tipo.GetGenericArguments()[0].FullName.Contains(nameof(System)) : !p.Tipo.FullName.Contains(nameof(System))))
                                        .Where(p => !p.Atributos.Any(a => a is ForeignKeyAttribute))
                                        .ToArray();

                }
            }
            else
            {
                foreignKey = tipoFrom.GetPropiedadesTipo().Where(p => p.Nombre.Equals(nombrePropiedadDetino)).ToArray();
            }



            if (foreignKey.Length != 0)
            {
                navigation.Nombre = foreignKey.Length == 1 ? foreignKey[0].Nombre : string.Empty;
                navigation.IsValid = true;
                navigation.IsToMany = foreignKey.Length == 1 ? foreignKey[0].Tipo.IsGenericType : true;
                if (navigation.HasNavigationTo)
                {
                    navigation.NombreId = tipoFrom.GetPropiedadesTipo().Where(p => p.Nombre.ToLower() == navigation.CalcNombreId).Select(p => p.Nombre).FirstOrDefault();
                }
            }
            //tener en cuenta en el IsValid    HasNavigationFrom osea que 



            return navigation;
        }
        public class NavigationProperty
        {
            public bool IsValid { get; set; }
            public string Nombre { get; set; }
            public string CalcNombreId => $"{Nombre}Id".ToLower();
            public string NombreId { get; set; }
            public bool IsFromMany { get; set; }
            public bool IsToMany { get; set; }
            public bool HasIdProperty => !string.IsNullOrEmpty(NombreId);
            public bool HasNavigationTo => !string.IsNullOrEmpty(Nombre);
        }
    }
}
