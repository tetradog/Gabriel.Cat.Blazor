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
using System.Dynamic;
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

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase("DbContext");

            base.OnConfiguring(optionsBuilder);
        }
    }
    public class DbContextAuto : DbContext
    {

        //metodos de extension bool para desactivarlos privados
        public SortedList<string, Type> Tipos { get; private set; }
        public DbContextAuto([NotNull] DbContextOptions options) : base(options)
        {
            Tipos = new SortedList<string, Type>();
    
            foreach (Type tipo in GetAllTypes())
            {
                if(!tipo.IsGenericType)
                   Tipos.Add(tipo.Name, tipo);
            }
        }
        public EntityEntry AddObj([NotNull] object obj)
        {

            Type tipo = obj.GetType();
            if (!tipo.HeredaDirectoObj())
            {
                throw new NotSupportedException($"No se puede añadir el tipo {tipo} porque no hereda directamente de {typeof(object)}");
            }
            return Add(obj);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            IList<Type> allTableTypes = Tipos.Values;


            base.OnModelCreating(modelBuilder);

            ////relaciones
            AddRelations(modelBuilder, allTableTypes);
            AddNavigation(modelBuilder, allTableTypes);


        }

        protected virtual void AddRelations(ModelBuilder modelBuilder, IList<Type> allTableTypes)
        {
            modelBuilder.ConfigureRelationTypes(allTableTypes);
        }

        protected virtual IEnumerable<Type> GetAllTypes()
        {
            return DbContextExtension.GetAllTypes(this);
        }

        protected virtual void AddNavigation(ModelBuilder modelBuilder, IEnumerable<Type> types)
        {
            modelBuilder.AddNavigation(types);
        }




    }

    public class Persona
    {
        public int Id { get; set; }
        public int EstudianteId { get; set; }
        public Estudiante Estudiante { get; set; }
        public int? IzquierdoId { get; set; }
        public Ojo Izquierdo { get; set; }
        public int? DerechoId { get; set; }
        public Ojo Derecho { get; set; }
        public int? BocaId { get; set; }
        public Boca Boca { get; set; }
        public ICollection<Estudiante> Estudiantes { get; set; }

    }
    public class Estudiante
    {
        public int Id { get; set; }
        public int PersonaId { get; set; }
        public Persona Persona { get; set; }
        public ICollection<Persona> Profesores { get; set; }

    }
    public class Ojo
    {
        public int Id { get; set; }
        public int PersonaId { get; set; }
        public Persona Persona { get; set; }
        public string Color { get; set; }
    }
    public class Boca
    {
        public int Id { get; set; }
        public int PersonaId { get; set; }
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
