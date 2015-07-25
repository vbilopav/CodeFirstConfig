using System.Threading.Tasks;

namespace CodeFirstConfig
{
    public abstract class ConfigManager<TModel> where TModel : class, new()
    {
        protected static volatile TModel Model;
        protected static ModelConfigurator<TModel> ModelConfigurator { get; set; }
                
        public static TModel Config 
        { 
            get 
            {
                if (Model != null) return Model;
                lock (Configurator.ConfigLock)
                {
                    if (Model != null) return Model;
                    return Model = ModelConfigurator.ConfigureModel();
                }
            } 
            internal set
            {
                lock (Configurator.ConfigLock)
                {
                    Model = value;
                    ConfigObjects.Set(ModelConfigurator<TModel>.GetNamespace(typeof(TModel)), Model);
                }
            }
        }

        public static TModel Reconfigure()
        {
            lock (Configurator.ConfigLock)
            {
                return Model = ModelConfigurator.ConfigureModel();
            }
        }

        public static async Task<TModel> ReconfigureAsync() { return await Task.Run(() => Reconfigure()); }

        static ConfigManager()
        {
            ModelConfigurator = new ModelConfigurator<TModel>();
            Model = null;            
        }
    }
}