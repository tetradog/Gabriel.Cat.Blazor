using Gabriel.Cat.Blazor.Shared;
using Gabriel.Cat.Blazor.Shared.Entities;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace Gabriel.Cat.Blazor.Server
{
    public class DbContextWithFiles : DbContextWithOutFiles
    {
        public DbContextWithFiles([NotNull] DbContextOptions options) : base(options)
        {
        }
        public DbSet<Archivo> Archivos { get; set; }

    }
    public class DbContextWithOutFiles:DbContext
    {
        public DbContextWithOutFiles([NotNull] DbContextOptions options) : base(options)
        {
        }
        public virtual string GetPropertyName<T>()
        {
            return $"{typeof(T).Name}s";
        }
    }
}
