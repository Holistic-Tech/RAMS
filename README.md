# RAMS

# Development

## Prequisites

Ensure that you have Crystal Reports 32-bit, `13.0.2000`, installed on your machine. Changes were made to point to this very specific build so that during development and publishing, no changes would need to be made.

### CrystalDecisions Runtime

### Mirror
CRRuntime_32bit_13_0_20.msi - https://drive.google.com/file/d/1WPWe3ATzblY97CB6iUFKyBy2_Wx15ZoC/view?usp=sharing


# The following is to be ignore unless you are attempting to update the primary SAP 

## Getting RAMS Up & Running - Migrating Development Environment To Latest Versions (13.0.2000 -> 13.0.3500)

## Prerequisites

Ensure that you also have the runtimes Installed. For `13.0.3500` you will require Service Pack Runtimes from CrystalDecisions themselves. I've linked to the runtime versions I used from their site, but I'll keep a mirror on our Company Google Drive.

Files:

> CRforVS_redist_install_64bit_13_0_21.zip
> CRforVS_redist_install_32bit_13_0_21.zip

### CrystalDecisions Runtime Direct Links

http://downloads.businessobjects.com/akdlm/cr4vs2010/CRforVS_redist_install_32bit_13_0_21.zip

http://downloads.businessobjects.com/akdlm/cr4vs2010/CRforVS_redist_install_64bit_13_0_21.zip

### Mirror

CRforVS_redist_install_64bit_13_0_21.zip - https://drive.google.com/open?id=1so1jXsoNLXWcbz2bQ7sCasmf999ZHcwR

CRforVS_redist_install_32bit_13_0_21.zip - https://drive.google.com/open?id=1NVJesCKiNbGO4MwmOnHLgNj9jpZzjDP6

---
## WARNING
As I am writing this I am unsure how these new changes, which should be regarded as breaking changes, will affect or be affected by the target server.
---


## Guide

In the event that weird stuff is happening with your RAMS Development environment you can follow these steps.

1. Pull a fresh copy from `develop` branch.
2. Restore Nuget Packages Via the `Package Manager Console`, then run the application. The application should throw errors.
3. Under your References Tree in the `Rcsa.Web` project, you will notice that there are four (4) items with issues.
4. Run the following commands in your `Package Manager Console`

```nuget
Install-Package CrystalReports.ReportSource -Version 13.0.3501
Install-Package CrystalReports.Web -Version 13.0.3501
```

5. Looking under References Tree in the `Rcsa.Web` project you will find that there are now no more issues. Your Application should run. The reports would not work.
6. Change the References in the Reports files (`ReportsForm/` folder)

From
```csharp
<%@ Register assembly="CrystalDecisions.Web, Version=13.0.2000.0, Culture=neutral, PublicKeyToken=692fbea5521e1304" namespace="CrystalDecisions.Web" tagprefix="CR" %>
```

To
```csharp
<%@ Register assembly="CrystalDecisions.Web, Version=13.0.3500.0, Culture=neutral, PublicKeyToken=692fbea5521e1304" namespace="CrystalDecisions.Web" tagprefix="CR" %>
```

7. You should be able to Run the application and view the reports, as intended. if you cannot see the reports (ie. a blank page when you view a report, you have to extract the `aspnet_client.zip` file into the `Rcsa.Web/` folder. So that `Rcsa.Web/aspnet_client/` exists, though you shouldn't need to do it.)
8. These changes have been commited to the develop branch. However refer to this guide if it still isn't working.
