using System.Data.Entity;
using System.Data.Entity.Infrastructure.Interception;
using System.Data.Entity.SqlServer;

namespace ContosoUniversity.DAL
{
    /// <summary>
    /// 实体框架将自动运行的代码在派生类中找到DbConfiguration。 
    /// 可以使用DbConfiguration类，以执行在中否则所执行的操作的代码中的配置任务Web.config文件。
    /// </summary>
    public class SchoolConfiguration : DbConfiguration
    {
        public SchoolConfiguration()
        {
            SetExecutionStrategy("System.Data.SqlClient", () => new SqlAzureExecutionStrategy());
            DbInterception.Add(new SchoolInterceptorTransientErrors());
            DbInterception.Add(new SchoolInterceptorLogging());
        }
    }
}