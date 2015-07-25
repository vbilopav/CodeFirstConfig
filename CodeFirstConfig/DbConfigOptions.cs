using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CodeFirstConfig
{
    public abstract class DbConfigOptions
    {
        private static bool? _createResult = null;

        protected Func<DbConnection> CreateConnectionFunc = null;        
        protected virtual DbConfigSettings Settings { get; set; }
        protected virtual CommandType CommandType { get; set; }

        protected virtual DbCommand CreateSelectCommand(DbConnection connection)
        {
            var command = connection.CreateCommand();
            command.CommandType = CommandType;
            command.CommandText = Settings.SelectCommandText;
            DbParameter p1 = command.CreateParameter();
            p1.ParameterName = "ApplicationId";
            p1.Value = App.Config.Id;
            command.Parameters.Add(p1);
            DbParameter p2 = command.CreateParameter();
            p2.ParameterName = "InstanceId";
            p2.Value = App.Config.InstanceId;
            command.Parameters.Add(p2);
            return command;
        }

        protected virtual DbCommand CreateCreateCommand(DbConnection connection)
        {
            var command = connection.CreateCommand();
            command.CommandType = System.Data.CommandType.Text;
            command.CommandText = Settings.CreateCommandText;
            return command;
        }

        protected virtual IDictionary<string, string> ReadConfiguration(IDataReader reader, IDictionary<string, string> config)
        {
            bool any = false;
            while (reader.Read())
            {
                var key = reader[Settings.KeyField];
                var value = reader[Settings.ValueField];
                if (key == DBNull.Value || value == DBNull.Value) continue;
                any = true;
                config[key.ToString()] = value.ToString();
            }
            if (!any) return config;

            List<KeyValuePair<string, string>> list = config.ToList();
            list.Sort((firstPair, nextPair) => nextPair.Key.Length.CompareTo(firstPair.Key.Length));
            return list.ToDictionary(pair => pair.Key, pair => pair.Value);
        }

        protected virtual bool CreateConfiguration(DbConnection connection)
        {                                
            using (var command = CreateCreateCommand(connection))
            {
                var result = command.ExecuteScalar();
                if (result == null) return false;
                bool ret;
                if (bool.TryParse(result.ToString(), out ret))
                    return ret;
                return result.ToString() == "1";
            }
        }
        
        protected virtual DbConnection CreateConnection()            
        {
            DbConnection connection = CreateConnectionFunc();                      
            connection.ConnectionString =
                ConfigurationManager.ConnectionStrings[Settings.NameOrConnectionString] == null ? Settings.NameOrConnectionString :
                ConfigurationManager.ConnectionStrings[Settings.NameOrConnectionString].ConnectionString;
            return connection;
        }

        public virtual IDictionary<string, string> DbConfigure(IDictionary<string, string> dictionary) //protected by ConfigLocks.ConfigLock
        {
            using (var connection = CreateConnection())
            {
                connection.Open();
                if (Settings.RunCreateCommand && _createResult == null)
                {
                    _createResult = CreateConfiguration(connection);
                    if (_createResult == true) return dictionary;
                }                
                using (var command = CreateSelectCommand(connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        return ReadConfiguration(reader, dictionary);
                    }
                }
            }
        }

        public virtual async Task<IDictionary<string, string>> DbConfigureAsync(IDictionary<string, string> dictionary) //protected by ConfigLocks.ConfigLock
        {
            using (var connection = CreateConnection())            
            {
                await connection.OpenAsync();
                if (Settings.RunCreateCommand && _createResult == null)
                {
                    _createResult = CreateConfiguration(connection);
                    if (_createResult == true) return dictionary;
                }                
                using (var command = CreateSelectCommand(connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        return ReadConfiguration(reader, dictionary);
                    }
                }
            }
        }

        public virtual async Task InsertInstanceAsync(IDictionary<string, object> configuration, List<Exception> exceptions = null)
        {
            if (Settings.InsertInstanceCommandText == null || Settings.RunInsertInstanceCommand == false) return;
            using (var connection = CreateConnection())
            {
                await connection.OpenAsync();

                using (var command = connection.CreateCommand())
                {
                    command.CommandType = CommandType;
                    command.CommandText = Settings.InsertInstanceCommandText;

                    DbParameter p1 = command.CreateParameter();
                    p1.ParameterName = "TimeStamp";
                    p1.Value = DateTime.Now;
                    command.Parameters.Add(p1);

                    DbParameter p2 = command.CreateParameter();
                    p2.ParameterName = "ApplicationId";
                    p2.Value = App.Config.Id;
                    command.Parameters.Add(p2);

                    DbParameter p3 = command.CreateParameter();
                    p3.ParameterName = "InstanceId";
                    p3.Value = App.Config.InstanceId;
                    command.Parameters.Add(p3);

                    DbParameter p4 = command.CreateParameter();
                    p4.ParameterName = "Configuration";

                    using (TextWriter textWriter = new StringWriter())
                    {
                        ConfigObjects.ToWriter(textWriter, exceptions); 
                        p4.Value = textWriter.ToString();
                    }
                    command.Parameters.Add(p4);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        protected DbConfigOptions(Func<DbConnection> createConnection)
        {
            CommandType = System.Data.CommandType.Text;
            CreateConnectionFunc = createConnection;
            Settings = new DbConfigSettings();            
            if (ConfigValues.DatabaseOptions != null)
                throw new ArgumentException("Only one instance of DbConfigOptions is permited!");
            //ConfigValues.DatabaseOptions = this;
        }
    }
}