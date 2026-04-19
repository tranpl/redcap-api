[![Build Status](https://dev.azure.com/cctrbic/redcap-api/_apis/build/status/redcap-api?branchName=master)](https://dev.azure.com/cctrbic/redcap-api/_build/latest?definitionId=122&branchName=master)

[![NuGet](https://img.shields.io/nuget/dt/RedcapApi.svg?style=for-the-badge)](https://www.nuget.org/packages/RedcapAPI) 
[![license](https://img.shields.io/github/license/mashape/apistatus.svg?style=for-the-badge)](https://github.com/cctrbic/redcap-api/blob/master/LICENSE.md)

Project Feedback, using REDCap of course: https://redcap.vcu.edu/surveys/?s=KJLHWRTJYA

# REDCap API Library for .NET
The REDCap Api Library for .NET provides the ability to interact with REDCap programmatically using various .NET languages(C#,F#,VB.NET);

## What's New in 2.0.0

Version 2.0.0 is a cleanup and parity release focused on bringing the library in line with documented REDCap API behavior while modernizing the repository structure.

- Full documented `help.php` endpoint parity audit against REDCap 17.0.2
- Added `ExportSurveyAccessCodeAsync`
- Added interface coverage for randomization and user role mapping endpoints
- Added `combineCheckboxOptions` support for record export methods
- Added `delete_logging` support for record deletion methods
- Added ODM format support where REDCap supports it
- Expanded `Content` coverage for newer REDCap content values
- Fixed `ExportProjectXmlAsync` default content mapping
- Reorganized the repository into conventional `.NET` folders: `src`, `tests`, and `demo`

__Prerequisites__
1.  Local redcap instance installed (visit https://project-redcap.org) if you need to download files(assuming you have access)
2.  Create a new project with "Demographics" for the template; this gives you a basic project to work with.
3.  Create an api token for yourself, replace that with the tokens you see on the "RedcapApiTests.cs" files, and others
4.  You'll may need to add a field type of "file_upload" so that you can test the file upload interface of the API
5.  Build the solution, then run the tests

__Highlights__
* Export and import records, metadata, users, roles, DAGs, events, instruments, reports, and files
* Project export, project XML export, project settings import, and next record name generation
* Survey link, survey queue link, survey return code, survey participants, and survey access code support
* File repository create/list/export/import/delete support
* Repeating instruments/events import and export support
* Randomize record support

__Usage__:

1. dotnet restore
2. Add a reference to the package or project
3. Add "using Redcap" namespace
4. Add "using Redcap.Models" for convenience
5. Use the sample data dictionary in `src/RedcapApi/Docs` if you want a quick test project setup
6. The repository is organized as follows:

    - `src/RedcapApi` - library source
    - `tests/RedcapApi.Tests` - test project
    - `demo/RedcapApiDemo` - demo console app

7. Feel free to contribute

__Sample / Example__
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
            // Use your own API Token here...
            var apiToken = "3D57A7FA57C8A43F6C8803A84BB3957B";
            var redcap_api = new RedcapApi("https://localhost/redcap/api/");

            Console.WriteLine("Exporting all records from project.");
            var result = redcap_api.ExportRecordsAsync(apiToken).Result;

            var data = JsonConvert.DeserializeObject(result);
            Console.WriteLine(data);
            Console.ReadLine();

        }
    }
}

```

__Install directly in Package Manager Console or Command Line Interface__
```C#
Package Manager

Install-Package RedcapAPI -Version 2.0.0

```

```C#
.NET CLI

dotnet add package RedcapAPI --version 2.0.0

 ```

```C#
Paket CLI

paket add RedcapAPI --version 2.0.0

```

__Example Project__

A console project has been included with the source code to get started. Some examples of method usage. You can use this to get started potentially.

__Test Project__

A project with associated test cases is included. Make sure to change the api token

# Please Cite Us
Publications resulting from the use of this software should cite the Wright Center's Clinical and Translational Science Award (CTSA) grant #UL1TR002649, National Center for Advancing Translational Sciences, NIH.

