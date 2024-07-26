using System.Data.Entity;

namespace Rcsa.Web.Models
{
  public class RcsaDb : DbContext
  {
    public RcsaDb()
      : base("name=DefaultConnection")
    {

    }

    public DbSet<UserProfile> UserProfiles { get; set; }
    public DbSet<RiskMaster> RiskMasters { get; set; }
    public DbSet<RiskAssessmentMaster> RiskAssessmentMaster { get; set; }
    public DbSet<DepartmentMaster> DepartmentsMaster { get; set; }
    public DbSet<SubRiskMaster> SubRisksMaster { get; set; }
    public DbSet<MitigantMaster> MitigantsMaster { get; set; }
    public DbSet<CompanyMaster> CompaniesMaster { get; set; }
    public DbSet<CompanyUser> CompanyUsers { get; set; }
    public DbSet<RiskDetailsModel> RiskDetails { get; set; }
    public DbSet<UserDepartment> UserDepartments { get; set; }

    
  }
}