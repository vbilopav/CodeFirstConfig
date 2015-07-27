using System;
using System.Data.Common;
using System.Data.SqlServerCe;

namespace CodeFirstConfig.SqlServerCe
{
    public sealed class SqlCeConfigOptions : DbConfigOptions
    {
        public SqlCeConfigOptions()
            : base(() => new SqlCeConnection())
        {
            Settings.SelectCommandText = "SELECT [Key], Value FROM ConfigValues WHERE (ApplicationId IS NULL OR ApplicationId = @ApplicationId) AND (InstanceId IS NULL OR InstanceId = @InstanceId)";
            Settings.InsertInstanceCommandText = "INSERT INTO ConfigInstances (TimeStamp, ApplicationId, InstanceId, Configuration) VALUES (@TimeStamp,@ApplicationId,@InstanceId,@Configuration)";
            Settings.CreateCommandText = null;
            Settings.RunCreateCommand = true;
        }

        public SqlCeConfigOptions(string nameOrConnectionString)
            : this()
        {
            Settings.NameOrConnectionString = nameOrConnectionString;
        }

        protected override DbCommand CreateCreateCommand(DbConnection connection)
        {
            throw new ArgumentException();
        }

        protected override bool CreateConfiguration(DbConnection connection)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT 1 AS Expr1 FROM Information_Schema.Tables WHERE (TABLE_NAME = 'ConfigValues')";
                var result = command.ExecuteScalar();
                if (result != null) return false;
                command.CommandText = "CREATE TABLE ConfigValues([Key] [nvarchar](128) NOT NULL, [ApplicationId] [nvarchar](128) NULL, [InstanceId] [nvarchar](128) NULL, [Value] ntext NULL)";
                command.ExecuteNonQuery();
                command.CommandText = "CREATE UNIQUE INDEX idxConfigValues ON ConfigValues([Key]);";
                command.ExecuteNonQuery();
                command.CommandText = "CREATE TABLE ConfigInstances([TimeStamp] datetime NOT NULL, [ApplicationId] [nvarchar](128) NOT NULL, [InstanceId] [nvarchar](128) NULL, Configuration ntext NULL)";
                command.ExecuteNonQuery();
                command.CommandText = "CREATE UNIQUE INDEX idxConfigInstances ON ConfigInstances([TimeStamp], [ApplicationId]);";
                command.ExecuteNonQuery();

                return true;
            }
        }
    }
}
