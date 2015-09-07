# `CodeFirstConfig` 
`CodeFirstConfig` will make your configuration pain go away!

Writing configurable code is sometimes boring, tedious and repeatable task. 
Not to mention that config files can sometimes grow out of control to complete and utter config hell!

You need to think about correct config keys, about parsing values correct way, about type checking, and so on, 
and usually end up generating lumps of code which is by no means prone to errors or exceptions...

Why not just write your code first the way you want, so you could have code completion, 
you can do painlees refactoring, default values, no type checking and so on...
and let someone else take care of that? 

`CodeFistConfig` lets you do just that!
## Simple example
```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>      
  <appSettings>
    <add key="Value1" value="Configured1" />
    <add key="Value2" value="Configured2" />
    <add key="Value3" value="4" />
  </appSettings>
</configuration>
```
```csharp
public class MyConfigurableModule : ConfigManager<MyConfigurableModule>
{
    public string Value1 { get; set; }
    public string Value2 { get; set; }
    public int Value3 { get; set; }

    public MyConfigurableModule()
    {
        this.Value1 = "Value1";
        this.Value2 = "Value2";
        this.Value3 = 3;
    }
}
...
WriteLine(MyConfigurableModule.Config.Value1); //outputs "Configured1"
WriteLine(MyConfigurableModule.Config.Value2); //outputs "Configured2"
WriteLine(MyConfigurableModule.Config.Value3); //outputs "4"
```
There you go. You just write your code as you would normally do, and declare class as configurable. 

You can do it by inhert `ConfigManager` class or why not directly via `ConfigManager<MyConfigurableModule>.Config`  or by class composition, or any way you like. 
`ConfigManager<MyConfigurableModule>.Config` will configure generic class on demand.

So, the whole idea is to just write code, don't worry about configuration files.

But, but, but wait, solution may have hundred, if not thousands configurable values,
how can I know what is configurable, and what is not? I cannot configure or reconfigure something if I 
even don't know that it can be configurend, now can I?

Sure you can. Just, insert one, single line to beginning of your application:
```csharp
Configurator.Configure();
```
or 
```csharp
Configurator.ConfigureAsync();
```
New configuration file will be generated for you, with following values:
```xml
...
<add key="WebExample.MyConfigurableModule.Value1" value="Configured1" />
<add key="WebExample.MyConfigurableModule.Value2" value="Configured2" />
<add key="WebExample.MyConfigurableModule.Value3" value="4" />
...
```
This file will include all of your configurable values in your application, 
as well as previous config keys from appSettings that you may have. 
It will show you exactly how many configurable values you have in your application with their respective values 
(either configured or default).

Now, why stop there. 
You can start using this newly created file as your main `appSettings` config file by referencing it from you main configuration: 
```xml
...
<appSettings configSource="CodeFirstAppSettings.config"></appSettings>
...
```
But, but, but, if I change that file it will not reset or restart my application as would main configuration file. 
Sure. Shut up. There is no need to. 

It will trigger asynchronous process that will reconfigure all of your configurable values, 
including ones not managed by `CodeFirstConfig ConfigManager` (plain `AppSettings` values) - without restart. 

And then, moment after, file will be recraeted again, so that it reflects real values. Of course, if you want to.


Cool. 

But, `CodeFirstConfig` can do more. 

How about, loading you configuration from database, asynchronous reconfigurations without nasty application resets, 
lazy configuration, complex data structures, config events and more... 

...

> **"So useful, my brain hurts"** ~ colleague from work.

> **"Wow, just wow"** ~ some other colleague from work.

> **"Very, very useful, saves me a lot of tedious coding. Now I can focus on real problems..."** ~ some developer dude.

> **"Never start project without it"** ~ me.

> **"Ubercool"** ~ somebody.


### Further Documentation

[Documentation can be found here.](https://github.com/vbilopav/CodeFirstConfig/wiki) 


### Nuget

```
PM> Install-Package CodeFirstConfig
```
