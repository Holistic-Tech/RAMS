using WebMatrix.WebData;

namespace Rcsa.Web.App_Start
{
  public class DataBaseConfig
  {
    public static void RegisterDatabaseConnection()
    {
      //TODO : Move to register area
      WebSecurity.InitializeDatabaseConnection("DefaultConnection", "UserProfile", "UserId", "UserName", autoCreateTables: true);
    }
  }
}