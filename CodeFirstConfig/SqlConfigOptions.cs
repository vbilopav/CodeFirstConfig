using System.Data.SqlClient;

namespace CodeFirstConfig
{
    public sealed class SqlConfigOptions : DbConfigOptions
    {
        public SqlConfigOptions()
            : base(() => new SqlConnection())
        {
            CommandType = System.Data.CommandType.StoredProcedure;
            Settings.SelectCommandText = "Config.Values_Select";
            Settings.InsertInstanceCommandText = "Config.Instance_Insert";
            Settings.RunCreateCommand = true;

            Settings.CreateCommandText = @"                
if not exists(select 1 from sys.schemas where name = 'Config')
begin
	exec sp_executesql N'create schema Config authorization dbo'
	create table Config.[Values]
	(
		[Key] [varchar](128) NOT NULL,
		[ApplicationId] [varchar](128) NULL,
		[InstanceId] [varchar](128) NULL,
		[Value] [nvarchar](max) NULL,
		constraint [PK_Config] primary key clustered ( [Key] ASC )
	)
	if not exists(select 1 from sys.indexes where name = 'IX_ConfigValues' and object_id = object_id('Config'))
	begin
		create nonclustered index IX_ConfigValues ON Config.[Values] ( [ApplicationId] asc, [InstanceId] asc	)
	end
	create table Config.Instances
	(
		[TimeStamp] datetime not null,
		[ApplicationId] [varchar](128) NOT NULL,
		[InstanceId] [varchar](128) NULL,
		Configuration [nvarchar](max) NULL,
		constraint [PK_ConfigInstances] primary key clustered ([TimeStamp], [ApplicationId] )
	)
	exec sp_executesql N'create proc Config.Values_Select
		(
			@ApplicationId [varchar](128) = null,
			@InstanceId [varchar](128) = null
		)
		as
		select [Key], [Value] 
		from Config.[Values] 
		where   ( ApplicationId is null or ApplicationId = @ApplicationId ) and 
				( InstanceId is null or InstanceId = @InstanceId    )'				
	exec sp_executesql N'create proc Config.Instance_Insert
	(
		@ApplicationId [varchar](128),
		@InstanceId [varchar](128),
		@TimeStamp datetime,
		@Configuration [nvarchar](max)
	)
	as
	insert into Config.Instances ([TimeStamp], [ApplicationId], [InstanceId], Configuration)
	values  (	@TimeStamp, @ApplicationId, @InstanceId, @Configuration	)'
	select 1
end else select 0";     
        }

        public SqlConfigOptions(string nameOrConnectionString)
            : this()
        {
            Settings.NameOrConnectionString = nameOrConnectionString;
        }       
    }
}