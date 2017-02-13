using System;
using Microsoft.Extensions.Configuration;

namespace Auth.API.Initialization
{
    public static class DatabaseSettingsExtensions
    {
        public static bool IsMigrateDatabaseOnStartup(this IConfiguration config)
        {
            IConfigurationSection section = config.GetSection("DatabaseSettings:MigrateOnStartup");
            return Convert.ToBoolean(section.Value);
        }
    }
}