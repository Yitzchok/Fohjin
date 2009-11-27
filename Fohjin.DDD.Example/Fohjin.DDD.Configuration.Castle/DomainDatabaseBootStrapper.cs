using System.Data.Common;
using System.IO;

namespace Fohjin.DDD.Configuration.Castle
{
    public class DomainDatabaseBootStrapper
    {
        public static void BootStrap()
        {
            new DomainDatabaseBootStrapper();
        }
    }
}