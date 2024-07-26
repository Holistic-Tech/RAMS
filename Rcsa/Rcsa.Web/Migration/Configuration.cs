using System.Web.Security;
using Rcsa.Web.App_Start;
using Rcsa.Web.Models;
using System.Data.Entity.Migrations;

namespace Rcsa.Web.Migration
{
    internal sealed class Configuration : DbMigrationsConfiguration<RcsaDb>
    {
        public Configuration()
        {
          AutomaticMigrationsEnabled = true;
          AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(RcsaDb context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //

          SeedMembership();
        }

        private void SeedMembership()
        {
          DataBaseConfig.RegisterDatabaseConnection();
        }
    }
}
