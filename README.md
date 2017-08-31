# REDCap API Library for C#
The REDCap API (Application Programming Interface) for REDCap, lets you:
    export/import/delete data in REDCap
    export/import/delete project information (e.g., field names and types) in REDCap

__Usage__:

1. dotnet restore
2. Add reference to the library in your project
3. Add "using Redcap" namespace
4. instantiate a new instance of the redcapapi object

__Example__
```C# 

using System;
using Newtonsoft.Json;
using Redcap;
using Redcap.Models;

namespace RedcapApiDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Redcap Api Demo Started!");

            var redcap_api = new RedcapApi("3D57A7FA57C8A43F6C8803A84BB3957B", "http://localhost/redcap/api/");

            var result = redcap_api.GetRecordAsync("1", RedcapFormat.json, RedcapDataType.flat, ReturnFormat.json, null, null, null, null).Result;
            var data = JsonConvert.DeserializeObject(result);
            Console.WriteLine(data);
            Console.ReadLine();

        }
    }
}

```

__Install directly in Package Manager Console or Command Line Interface__

``` Install-Package RedcapAPI -Version 0.2.6-alpha-release ```
```dotnet add package RedcapAPI --version 0.2.6-alpha-release ```

__DEMO__

A console project has been included with the source code to get started.