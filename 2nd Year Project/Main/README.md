<p align="center">
    <h1>EduLocate</h1>
	<p>2nd Year University Project - 2018/19</p>
</p>


## Solution projects


### Primary:	
* **[Server](Server)**
	_.NET Core server application using ASP Core. The mobile applications connect to it to use the API. Users can connect to the website for more information on the project. 
	Project administrators can also connect to manually edit school information._
	
* **[Android Application](Application.Android)**
	_Uses the Mono environment which runs parallel to the ARTVM (Android RunTime Virtual Machine). This runs on-top of the Linux kernel. Provides some platform-specific implementations of services that cannot be made platform-agnostic._
	
* **[iOS Application†](Application.iOS)**
	_Uses the Mono environment with fAOT (full Ahead Of Time) compilation to compile itself into ARM assembly. This runs on-top of the XNU kernel. Provides some platform-specific implementations of services that cannot be made platform-agnostic. _
	_†Due to limitations surrounding Apple's ecosystem, it is not possible to develop an iOS application currently. The scaffolded code has been left in case this changes._

* **[UWP Application](Application.UWP)**
    _Uses Window's C# environment. Provides some platform-specific implementations of services that cannot be made platform-agnostic._

### Supporting:
* **[.Common](Common)**
	_A shared project with no build target/platform or SDK. Provides some simple, cross-project functionality without any project overhead._
	
* **[Core](Core)**
	_.NET Standard POCOs to standardise the core concepts of the solution (Schools, Education stages, etc)._
	
* **[Application Core](Application.Core)**
	_.NET Standard shared logic for the front-end applications using Xamarin. Provides cross-platform views and logic as far as possible to standardise application behaviour across platforms. 
	Not all logic can be made cross-platform, however, and as such uses a Dependency Injection serviced from Xamarin to allow the core to remain platform-agnostic and abstract platform details to their respective projects._
	
* **[Service Interfaces](Services.ServiceInterfaces)**
	_.NET Standard library which exposes interfaces for other projects to rely on. This allows for easy control and management of Dependency Injection services used in the [Application Core](Application.Core) and the [Server](Server)._


### Services:
* **[ClosedXML Excel Data Service](Services.ClosedXmlExcelDataService)**
	_Service implementing `IExcelDataService` to abstract away access to excel workbooks to retrieve data._

* **[Coordinates Service](Services.CoordinateService)**
	_Service implementing `ICoordinatesService` to abstract away converting northing/easting to lat/long values. It is advised to read [this supporting document](https://www.ordnancesurvey.co.uk/docs/support/guide-coordinate-systems-great-britain.pdf) if working on this project as the code very closely mirrors this document where possible._

* **[Google School Metadata Service](Services.GoogleSchoolMetadataService)**
	_Service implementing `ISchoolMetadataService` to abstract away how metadata is retrieved about a school. This implementation uses Google's [Places API](https://developers.google.com/places/web-service/intro) to retrieve information about a school._

* **[Mock Service](Services.MockServices)**
	_Service implementing a range of services to allow for easier testing of the application/server. Can also reduce the cost of using external APIs by only testing them when required by substituting these APIs when the actual information provided is not too important._

* **[Server API School Service](Services.ServerApiSchoolService)**
	_Service implementing `ISchoolService` to abstract away how schools are retrieved. This implementation uses the API exposed by the [Server](Server)._


### Tests:
All tests are ran with NUnit and are located within one project for ease of use. The Tests project targets .NET Core and comes with the NUnit 3 test adapter so that it can be ran from inside Visual Studio and cross-platform. The tests are structured so that their namespaces can closely align with that of what they're testing.
* **[.Common Tests](Tests/.Common)** 
    _The .common tests ensure that the double helper functions as expected._

* **[Application Core](Tests/Application.Core)** 
    _A deal of the business logic in the application core is abstracted away into a Service project. The remaining logic, such as extensions and application-specific services, are tested when not platform-specific. The Translation, Alert, and Theme extension are all testes to ensure correct behaviour. The theme services present are also tested._

* **[Core Tests](Tests/Core)** 
    _The core tests ensure that the expression-bodied members (helpers) on School behave correctly, and that the EducationStages enum is correctly setup as flags._

* **[Service Tests](Tests/Services)** 
    _The service tests ensure that the services present in the soltuion behave correctly. Some services are missing tests as they contain difficult to unit test logic, such as tests which rely on data that is not certain to remain constant._

* **No unit tests exist for the Server**
    _This is as during development, an effort was made to abstract busienss logic away into services for easier dependency management, better code reuse, and to make unit testing easier. As such, the majority of the code left in the Server is very difficult to test (e.g. it handles routing requests)._

* **No unit tests exist for the mobile application's platform-specific projects.**
    _This is as testing platform-specific code is not trivial in C#. All cross-platform code is tested in [Application Core](Tests/Application.Core)._


## Compiling the solution
* Remember to restore NuGet packages *(right click on the solution and click `Restore NuGet Packages`)* and client-side libraries on the [Server](Server) *(right click on `./libman.json` and click `Restore Client-Side Libraries`)* before building.
* Remember to set Environment Variables for the [Server](Server) project (see [here](Server/Data/IdentitySeedingHelper.cs))
* Remember that not all projects (e.g. platform-specific parts) will be built unless specified explicitly, depending on your build configuration.
* Ensure the keys/tokens/connection strings in the `Server`, `.Common`, and the `Core`/`Android`/`iOS`/`UWP` application projects are all correct.
* Because of the nature of .Common, Visual Studio can sometimes believe that parts of it do not exist in projects that reference it. These issues can be ignored as the projects will still compile, it is just a quirk in Visual Studio that can't be easily avoided currently.
* If compiling the Android application cleanly, all of it's dependencies MUST be built first, else the build will fail to link. This is currently a Xamarin limitation.
