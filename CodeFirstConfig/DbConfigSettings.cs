namespace CodeFirstConfig
{
    public class DbConfigSettings
    {
        public string NameOrConnectionString { get; set; }
        public string SelectCommandText { get; set; }
        public string CreateCommandText { get; set; }
        public string InsertInstanceCommandText { get; set; }
        public bool RunCreateCommand { get; set; }
        public bool RunInsertInstanceCommand { get; set; }
        public string KeyField { get; set; }
        public string ValueField { get; set; }

        public bool Initialized { get; internal set; }

        public DbConfigSettings()
        {           
            NameOrConnectionString = "main";           
            KeyField = "Key";
            ValueField = "Value";            
            RunCreateCommand = true;
            RunInsertInstanceCommand = true;
            Initialized = false;
        }                        
    }
}