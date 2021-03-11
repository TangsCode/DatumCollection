using DatumCollection.Data;
using DatumCollection.Data.SqlServer;
using DatumCollection.Data.MySql;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace DatumCollection.Web
{
    public static class ServiceCollectionExtension
    {
        #region Data Storage (supporting Sql Server, MySql,PostgreSql & SQLite)
        public static IServiceCollection AddDataStorage(this IServiceCollection services,
            Action<DataStorageServiceBuilder> action = null
            )
        {
            services.AddSingleton<IDataStorage, SqlServerStorage>();
            var builder = new DataStorageServiceBuilder(services);
            action?.Invoke(builder);

            return services;
        }

        public static DataStorageServiceBuilder UseSqlServer(this DataStorageServiceBuilder builder)
        {
            builder.Services.AddSingleton<IDataStorage, SqlServerStorage>();
            return builder;
        }

        public static DataStorageServiceBuilder UseMySql(this DataStorageServiceBuilder builder)
        {
            builder.Services.AddSingleton<IDataStorage, MySqlStorage>();
            return builder;
        }
        #endregion
    }
}
