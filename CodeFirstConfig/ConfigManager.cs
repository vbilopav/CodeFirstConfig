using System.Threading.Tasks;

namespace CodeFirstConfig
{
    public abstract class ConfigManager<TModel> where TModel : class, new()
    {
        protected static volatile TModel Model;
        internal static ModelConfigurator<TModel> ModelConfigurator { get; set; }
                
        public static TModel Config 
        { 
            get 
            {
                if (Model != null) return Model;
                lock (ModelConfigurator)
                {
                    if (Model != null) return Model;
                    Model = ModelConfigurator.ConfigureModel();
                }
                if (Configurator.OnModelConfigured != null)
                        Configurator.OnModelConfigured(new ModelConfiguredEventArgs(typeof(TModel)));
                return Model;               
            }             
        }

        public static TModel Reconfigure()
        {
            lock (Configurator.ConfigLock)
            {
                Model = ModelConfigurator.ConfigureModel();               
            }
            if (Configurator.OnModelConfigured != null)
                Configurator.OnModelConfigured(new ModelConfiguredEventArgs(typeof(TModel)));
            return Model;
        }

        public static async Task<TModel> ReconfigureAsync() => await Task.Run(() => Reconfigure()); 

        static ConfigManager()
        {
            ModelConfigurator = new ModelConfigurator<TModel>();
            Model = null;            
        }
    }
}