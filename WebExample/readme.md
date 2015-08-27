## WebExample Web application

This is demonstration web app used primarily for experimentation purpose with 'CodeFirstConfig' library.

There is one configurable class `SampleClass` in `_code\SampleClass.cs` file. 
Application is set up to create new `CodeFirstAppSettings.config` configuration file (see `Global.asax.cs`).
Main configuration file is by default `Web.config`.

You may change this by uncommenting `Web.config` line:
```xml
<!--<appSettings configSource="CodeFirstAppSettings.config"></appSettings>-->
```
...and by commenting this section:
```xml
  <appSettings>
    <add key="webpages:Version" value="3.0.0.0" />
    <add key="webpages:Enabled" value="false" />
    <add key="ClientValidationEnabled" value="true" />
    <add key="UnobtrusiveJavaScriptEnabled" value="true" />
  </appSettings>
```

After save, application will restart and your main configuration file will be `CodeFirstAppSettings.config` 
which doesnt need restart to reconfigure and also contains all of your configurable classes.

Application contains four separate views where you can monitor different values in application :

### `SampleClass` view, first row on left

- Contains JSON representation of configurable class `SampleClass`

### `Log` view, first row on right

- Shows application log located in `App_Data\Log.txt` in reverse order (latest event on top). Every log is numbered.
Application will log following events:
    
    * Application start event, triggered in global `Application_Start` event.
    * Application end event, triggered in global `Application_Start` event.
    * Application error event, triggered in global `Application_Error` event.
    * Configurator error event, triggered in `Configurator.OnError` event.
    * Configurator model configred, triggered in Configurator.OnModelConfigured`.
    * Configurator configure finished.

### `CodeFirstConfig` view, second row

- Shows current `CodeFirstConfig` configuration dictionary seralized in `AppConfig` or `JSON` format. 
This is not content of `AppSetting` section in your configuration file, but representation of current `CodeFirstConfig` configuration dictionary.

### `ConfigurationManager.AppSettings` view, third row

- This is content of `AppSetting` section in your configuration file

