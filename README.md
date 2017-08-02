# redcap-api
Redcap Api Library for C#

__Usage__:

1. dotnet restore
2. Add reference to library in your project
3. Add "using Redcap.Interfaces" namespace (contains some enums for redcap optional params)
4. instantiate a new instance of the redcapapi object

```C# __var rc = new Redcap.RedcapApi("redcap token here", "your redcap api endpoint here")__```

```C# __var version = await rc.GetRedcapVersionAsync(RedcapFormat.json, RedcapDataType.flat);__```

__Install directly in Package Manager Console__

``` Install-Package RedcapAPI ```

__Demo__

https://github.com/cctrbic/redcap-api-demo