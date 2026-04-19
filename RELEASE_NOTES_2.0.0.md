# 🎉 REDCap API Library 2.0.0 Release Notes

## Overview
Version 2.0.0 is a major release focused on **parity with REDCap API documentation**, **modernization**, and **cleanup**. This release brings the library in line with current REDCap API endpoints while reorganizing the repository to follow .NET conventions.

## ✨ New Features

### New API Endpoints
- **`ExportSurveyAccessCodeAsync`** - Export survey access codes for participants
- **Randomization endpoints** - Full interface coverage for randomized studies
- **User role mapping endpoints** - Enhanced user and role management

### Enhanced Functionality
- **`combineCheckboxOptions`** support for record export methods
- **`delete_logging`** support for record deletion operations
- **ODM (Operational Data Model) format** support across applicable endpoints
- **Expanded `Content` enum** - Added support for newer REDCap content values

### Repository Reorganization
The repository now follows standard .NET conventions:
```
├── src/
│   └── RedcapApi/              # Library source code
├── tests/
│   └── RedcapApi.Tests/        # Unit tests
└── demo/
    └── RedcapApiDemo/          # Example console application
```

## 🔧 Improvements & Fixes
- **Fixed `ExportProjectXmlAsync`** default content mapping
- **Improved documentation** - Enhanced XML comments for IntelliSense support
- **Code cleanup** - Modernized codebase structure and patterns

## 📦 Technical Details
- **Target Framework**: .NET 10.0
- **Package**: [RedcapAPI on NuGet](https://www.nuget.org/packages/RedcapAPI)
- **License**: MIT

## 🚀 Installation

### Package Manager Console
```powershell
Install-Package RedcapAPI -Version 2.0.0
```

### .NET CLI
```bash
dotnet add package RedcapAPI --version 2.0.0
```

### Paket CLI
```bash
paket add RedcapAPI --version 2.0.0
```

## 📚 Highlights
The library now provides comprehensive REDCap API support including:
- Export and import records, metadata, users, roles, DAGs, events, instruments, reports, and files
- Project settings management and next record name generation
- Survey functionality (links, queues, return codes, participants, access codes)
- File repository operations (create, list, export, import, delete)
- Repeating instruments and events support
- Record randomization

## 🔗 Resources
- [GitHub Repository](https://github.com/cctrbic/redcap-api)
- [REDCap Project](https://project-redcap.org)
- [REDCap API Documentation](https://project-redcap.org/api/)

## 📝 Notes
- **Requires**: Local REDCap instance or access to a REDCap server
- **Supported Languages**: C#, F#, VB.NET
- **Maintainer**: Michael Tran (Virginia Commonwealth University)

---

**Thank you for using the REDCap API Library for .NET!** We welcome contributions and feedback. For issues or questions, please visit our [GitHub Issues page](https://github.com/cctrbic/redcap-api/issues).
