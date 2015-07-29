using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Data.SqlServerCe;
using CodeFirstConfig.SqlServerCe;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CodeFirstConfig.Tests
{
    [TestClass]
    public class DatabaseConfigTests
    {
        public class TestClass
        {
            public static TestClass Config => ConfigManager<TestClass>.Config;
            public string Value1 = "MyValueFromCode1";
            public string Value2 = "MyValueFromCode2";
            public string Value3 = "MyValueFromCode3";
            public string Value4 = "MyValueFromCode4";           
        }


        [TestMethod]
        public void TestSqlCeConfigOptions()
        {
            using (var connection = new SqlCeConnection(ConfigurationManager.ConnectionStrings["SqlServerCe"].ConnectionString))
            {
                //create structure in empty database, since db file is copied every time
                connection.Open();
                new SqlCeConfigOptions().CreateConfiguration(connection);

                using (var command = new SqlCeCommand())
                {
                    command.Connection = connection;                    
                    command.CommandText =
                        @"  insert into ConfigValues ([Key], ApplicationId, InstanceId, Value)
                            values ('Value2', null, null, 'DbValue2') ";
                    command.ExecuteNonQuery();


                    command.CommandText =
                        $@"  insert into ConfigValues ([Key], ApplicationId, InstanceId, Value)
                            values ('TestClass.Value3', '{App.Config.Id}', null, 'DbValue3') ";
                    command.ExecuteNonQuery();

                    command.CommandText =
                        $@"  insert into ConfigValues ([Key], ApplicationId, InstanceId, Value)
                            values ('Value4', '{App.Config.Id}', '{App.Config.InstanceId}', 'DbValue4') ";
                    command.ExecuteNonQuery();
                }                
                connection.Close();
            }

            
            Configurator.SetDatabaseOptions(new SqlCeConfigOptions("SqlServerCe"));

            Assert.AreEqual("MyValueFromCode1", TestClass.Config.Value1);
            Assert.AreEqual("DbValue2", TestClass.Config.Value2);
            Assert.AreEqual("DbValue3", TestClass.Config.Value3);
            Assert.AreEqual("DbValue4", TestClass.Config.Value4); 
        }

        [TestMethod]
        public void TestSqlConfigOptions()
        {
            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlServer"].ConnectionString))
            {
                //run create command
                connection.Open();
                new SqlConfigOptions().CreateConfiguration(connection);

                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText =
                        @"  insert into Config.[Values]  ([Key], ApplicationId, InstanceId, Value)
                            values ('Value2', null, null, 'DbValue2') ";
                    command.ExecuteNonQuery();


                    command.CommandText =
                        $@"  insert into Config.[Values]  ([Key], ApplicationId, InstanceId, Value)
                            values ('TestClass.Value3', '{App.Config.Id}', null, 'DbValue3') ";
                    command.ExecuteNonQuery();

                    command.CommandText =
                        $@"  insert into Config.[Values]  ([Key], ApplicationId, InstanceId, Value)
                            values ('Value4', '{App.Config.Id}', '{App.Config.InstanceId}', 'DbValue4') ";
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }


            Configurator.SetDatabaseOptions(new SqlConfigOptions("SqlServer"));

            Assert.AreEqual("MyValueFromCode1", TestClass.Config.Value1);
            Assert.AreEqual("DbValue2", TestClass.Config.Value2);
            Assert.AreEqual("DbValue3", TestClass.Config.Value3);
            Assert.AreEqual("DbValue4", TestClass.Config.Value4);
        }

        [TestMethod]
        public void TestConfigureSqlConfigOptions()
        {
            Configurator.OnBeforeSet = args => { };
            Configurator.OnAfterSet = args => { };

            Configurator.Configure(new SqlConfigOptions("SqlServer"));

            Assert.AreEqual("MyValueFromCode1", TestClass.Config.Value1);
            Assert.AreEqual("MyValueFromConfig2", TestClass.Config.Value2);
            Assert.AreEqual("MyValueFromConfig3", TestClass.Config.Value3);
            Assert.AreEqual("MyValueFromCode4", TestClass.Config.Value4); // different class name           
        }
    }
}
