﻿using System.Threading.Tasks;

namespace CodeFirstConfig
{
    public abstract class ConfigManager<TModel> where TModel : class, new()
    {
        protected static volatile TModel Model;
        internal static ModelConfigurator<TModel> ModelConfigurator { get; }
                
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
                if (ConfigSettings.Instance.OnModelConfigured != null)
                    ConfigSettings.Instance.OnModelConfigured(new ModelConfiguredEventArgs(typeof(TModel), Model));
                return Model;               
            }             
        }

        public static TModel Reconfigure()
        {
            lock (Configurator.ConfigLock)
            {
                Model = ModelConfigurator.ConfigureModel();               
            }
            if (ConfigSettings.Instance.OnModelConfigured != null)
                ConfigSettings.Instance.OnModelConfigured(new ModelConfiguredEventArgs(typeof(TModel), Model));
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