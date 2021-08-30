using WebApiTest.Data.Entities;
using System.Data.Entity;
using SQLite.CodeFirst;

namespace WebApiTest.Data.DbContexts
{
    public class FinalDataContext : DbContext
    {
        //public FinalDataContext() : base() { }
        public FinalDataContext() : base("WebApiTestConnString") {}

        // Security
        public DbSet<User> User { get; set; }
        public DbSet<Address> Address { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasOptional(s => s.Address)
                .WithRequired(ad => ad.User);

            //Use this version if you want to create the DB only if not already exists
            var sqliteConnectionInitializer = new SqliteCreateDatabaseIfNotExists<FinalDataContext>(modelBuilder);
            
            //Use this if you want to recreate the database everytime project runs
            //var sqliteConnectionInitializer = new SqliteDropCreateDatabaseAlways<FinalDataContext>(modelBuilder);

            Database.SetInitializer(sqliteConnectionInitializer);
        }
    }
}
