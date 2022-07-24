using System.Data;
using System.Data.SqlClient;

namespace eCommerce.API.Utils
{
    public static class Utilidades
    {
        public static IDbConnection RetornaSqlConnection()
        {
            return new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=eCommerce;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
        }
    }
}
