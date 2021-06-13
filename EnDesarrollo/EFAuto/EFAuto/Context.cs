using Gabriel.Cat.S.Extension;
using Gabriel.Cat.S.Utilitats;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EFAuto
{
    public class Context : DbContextAuto
    {
        public Context([NotNull] DbContextOptions options) : base(options) { }
        public DbSet<Persona> Personas { get; set; }

        protected override void OnConfiguring( DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase("DbContext");

            base.OnConfiguring(optionsBuilder);
        }
    }
    public class DbContextAuto : DbContext
    {

        //metodos de extension bool para desactivarlos privados
        public SortedList<string,Type> Tipos { get; private set; }
        public DbContextAuto([NotNull] DbContextOptions options) : base(options)
        {
            IEnumerable<Type> allTableTypes;
            Tipos = new SortedList<string, Type>();
            allTableTypes = IncludeTableTypes(GetAllTypes());
            foreach (Type tipo in allTableTypes)
            {
                Tipos.Add(tipo.Name, tipo);
            }
        }
        public   List<EntityEntry> AddObj(object obj)
        {
            List<EntityEntry> entries=new List<EntityEntry>();
            Type tipo = obj.GetType();
            if (!tipo.HeredaDirectoObj())
            {
                //genero un array con los tipos de los padres 
                foreach(object part in obj.GetParts())
                {
                    if (Tipos.ContainsKey(part.GetType().Name))//las propiedades que hereden...tendrian que usar la dinamica...por hacer
                        entries.Add(Add(Activator.CreateInstance(Tipos[part.GetType().Name]).SetPropertiesFromOther(part)));
                    else entries.Add(Add(part));
                }    
            
            }
            else
            {
              entries.Add(Add(obj));
            }
            return entries;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            IList<Type> allTableTypes=Tipos.Values;


            base.OnModelCreating(modelBuilder);


            //ArreglaIds(modelBuilder, allTableTypes);
            AddParentTables(modelBuilder, allTableTypes);
            AddChildTables(modelBuilder, allTableTypes);
 
            //relaciones
            //AddRelations(modelBuilder,allTableTypes);
            //AddNavigation(modelBuilder, allTableTypes);


        }
        protected virtual void AddChildTables(ModelBuilder modelBuilder, IEnumerable<Type> allTableTypes)
        {
            modelBuilder.AddChildTables(allTableTypes);
        }
        protected virtual void AddParentTables(ModelBuilder modelBuilder, IEnumerable<Type> allTableTypes)
        {
            modelBuilder.AddParentTables(allTableTypes);
        }
        protected virtual IEnumerable<Type> GetAllTypes()
        {
            return DbContextExtension.GetAllTypes(this);
        }
        protected virtual IEnumerable<Type> IncludeTableTypes(IEnumerable<Type> types)
        {

            IList<PropiedadTipo> propiedades;
            List<Type> tipos = types.ToList();


            return tipos;
        }
        //protected virtual void ArreglaIds(ModelBuilder modelBuilder, IEnumerable<Type> types)
        //{
        //    modelBuilder.ArreglaIds(types);

        //}
        protected virtual void AddNavigation(ModelBuilder modelBuilder,IEnumerable<Type> types)
        {
            modelBuilder.AddNavigation(types);
        }




    }

    public class Persona:Aux3
    {
        public Ojo Izquierdo { get; set; } = new Ojo();
        public Ojo Derecho { get; set; } = new Ojo();
        public Boca Boca { get; set; } = new Boca();
        public ICollection<Estudiante> Estudiantes { get; set; }

    }
    public class Estudiante:Persona
    {

        public ICollection<Persona> Profesores { get; set; }

    }
    public class Aux1 
    {
        public bool PropAux1 { get; set; }
    }
    public class Aux2 : Aux1
    {
        public bool PropAux2 { get; set; }
    }
    public class Aux3 : Aux2 { public bool PropAux3 { get; set; } public bool PropAux32 { get; set; } }
    public class Ojo
    {
        public Persona Persona { get; set; }
        public string Color { get; set; }
    }
    public class Boca
    {
        public Persona Persona { get; set; }
        public ICollection<Diente> Dientes { get; set; }
    }
    public class Diente
    {
        [Key]
        public int Posicion { get; set; }
        public Boca Boca { get; set; }
    }
}
