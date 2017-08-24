# redcap-api
redcap-api is a C# API (Application Programming Interface) for REDCap, that lets you:
    export/import/delete data in REDCap
    export/import/delete project information (e.g., field names and types) in REDCap

__Usage__:

1. dotnet restore
2. Add reference to library in your project
3. Add "using Redcap.Interfaces" namespace (contains some enums for redcap optional params)
4. instantiate a new instance of the redcapapi object

__Example__
```C# 

var rc = new Redcap.RedcapApi("redcap token here", "your redcap api endpoint here")

```

```C# 

var version = await rc.GetRedcapVersionAsync(RedcapFormat.json, RedcapDataType.flat);

```

__Install directly in Package Manager Console__

``` Install-Package RedcapAPI ```

__Demo__

https://github.com/cctrbic/redcap-api-demo