# MySettingsReader

![Nuget version](https://img.shields.io/nuget/v/MySettingsReader?label=MySettingsReader&style=social)

```
dotnet add package MySettingsReader
```



```
  public class SettingsModel
  {
    [YamlProperty("ExternalBinance.SeqServiceUrl")]
    public string SeqServiceUrl { get; set; }

    [YamlProperty("ExternalBinance.ZipkinUrl")]
    public string ZipkinUrl { get; set; }
  }

  ...
  
  var settings = SettingsReader.GetSettings<SettingsModel>(SettingsFileName);
```  
  

