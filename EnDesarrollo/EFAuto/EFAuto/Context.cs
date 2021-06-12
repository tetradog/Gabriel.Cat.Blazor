using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFAuto
{
    public class Context:DbContextAuto
    {
        public Context([NotNull] DbContextOptions options) : base(options) { }
        public DbSet<Persona> Personas { get; set; }
    }
    public class DbContextAuto:DbContext
    {

        //metodos de extension bool para desactivarlos privados

        public DbContextAuto([NotNull] DbContextOptions options) : base(options) {    
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            IEnumerable<Type> allTableTypes;


            base.OnModelCreating(modelBuilder);

            allTableTypes =GetAllTableTypes( GetAllTypes());


        }
        protected virtual IEnumerable<Type> GetAllTypes()
        {
            return DbContextExtension.GetAllTypes(this);
        }
        protected virtual IEnumerable<Type> GetAllTableTypes(IEnumerable<Type> types)
        {
            List<Type> tipos = types.ToList();

            //las clases que no tengan clase intermedia la crea
            //PersonaEstudiante se tendria que crear

            return tipos;
        }

    }
    public class Persona
    {
        public Ojo Izquierdo { get; set; }
        public Ojo Derecho { get; set; }
        public Boca Boca { get; set; }
        public ICollection<Estudiante> Estudiantes { get; set; }
    }
    public class Estudiante:Persona
    {
        public ICollection<Persona> Profesores { get; set; }
        
    }
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
        public Persona Persona { get; set; }
    }
}
