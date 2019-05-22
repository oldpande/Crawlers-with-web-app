using CarBase.Business;
using System.Data.Entity;
using System.Linq;

namespace CarBase.WebUI
{
    public partial class EntityDataModel : DbContext
    {
        public EntityDataModel()
            : base("name=CarsConnectionName")
        {
            Configuration.LazyLoadingEnabled = false;
        }

        public virtual DbSet<Brand> Brands { get; set; }
        public virtual DbSet<Model> Models { get; set; }
        public virtual DbSet<CarVersion> Versions { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Brand>().ToTable("Brand");
            modelBuilder.Entity<Model>().ToTable("Model");
            modelBuilder.Entity<CarVersion>().ToTable("Version");
        }
    }
}
