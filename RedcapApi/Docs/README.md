[![Build Status](https://dev.azure.com/cctrbic/redcap-api/_apis/build/status/redcap-api?branchName=master)](https://dev.azure.com/cctrbic/redcap-api/_build/latest?definitionId=122&branchName=master)

[![NuGet](https://img.shields.io/nuget/dt/RedcapApi.svg?style=for-the-badge)](https://www.nuget.org/packages/RedcapAPI) 
[![license](https://img.shields.io/github/license/mashape/apistatus.svg?style=for-the-badge)](https://github.com/cctrbic/redcap-api/blob/master/LICENSE.md)

# REDCap API Library for .NET
The REDCap Api Library provides the ability to interact with REDCap programmatically using various .NET languages(C#,F#,VB.NET);

__Prerequisites__
1.  Local redcap instance installed (visit https://project-redcap.org) if you need to download files(assuming you have access)
2.  Create a new project with "Demographics" for the template; this gives you a basic project to work with.
3.  Create an api token for yourself, replace that with the tokens you see on the "RedcapApiTests.cs" files, and others
4.  You'll may need to add a field type of "file_upload" so that you can test the file upload interface of the API
5.  Build the solution, then run the demo project to see the results.

__API METHODS SUPPORTED (Not all listed)__
* ExportLoggingAsync
* ExportDagsAsync
* ImportDagsAsync
* DeleteDagsAsync
* ExportArmsAsync
* ImportArmsAsync
* DeleteArmsAsync
* ExportEventAsync
* ImportEventAsync  
* ExportFileAsync
* ImportFileAsync
* DeleteFileAsync
* ExportMetaDataAsync
* ExportRecordsAsync
* ImportRecordsAsync
* ExportRedcapVersionAsync
* ExportUsersAsync

__Usage__:

1. dotnet restore
2. Add reference to the library in your project, or download from nuget into project
3. Add "using Redcap" namespace
4. Add "using Redcap.Models" for convenience
5. Replace the demo api token with your test project or you can import the data dictionary in \Docs
thats provided for convenience.
6. Feel free to contribute 

__Sample / Example__
```C# 

using Newtonsoft.Json;
using Redcap;

namespace RedcapApiDemo
{
    internal class Program
    {
        static async Task<int> Main(string[] args)
        {
            var apiToken = "3D57A7FA57C8A43F6C8803A84BB3957B";
            var redcap_api = new RedcapApi("http://localhost/redcap/api/");
            var result = await redcap_api.ExportRecordsAsync(apiToken);
            var records = JsonConvert.DeserializeObject(result);
            Console.WriteLine(records);
            Console.ReadLine();
            return 0;
        }
    }
}


```

__Install directly in Package Manager Console or Command Line Interface__
```C#
Package Manager

Install-Package RedcapAPI -Version 1.3.6

```

```C#
.NET CLI

dotnet add package RedcapAPI --version 1.3.6

 ```

```C#
Paket CLI

paket add RedcapAPI --version 1.3.6

```

__Example Project__

A console project has been included with the source code to get started. Some examples of method usage. You can use this to get started potentially.

__Test Project__

A project with associated test cases is included. Make sure to change the api token
