# The packages

Package name | Description
------------ | -------------
HL7_FM_CLI.ConsoleApp|A .Net Core Console App version of the MAX/FM/FP validation and compiler
HL7_FM_EA_Extension.Setup|Setup Visual Studio project for the EA Extension
HL7_FM_EA_Extension.Tests|Unit tests Visual Studio project for the EA Extension
HL7_FM_EA_Extension|The source code Visual Studio project for the EA Extension
Saxon.Tests|Some Unit tests around using the Saxon XSLT library
SchematronTry.ConsoleApplication|Some example code to try out Schematron Validation
SchematronTrySaxon.ConsoleApplication|Some example code to try out Schematron Validation with Saxon
compare|
import-scripts|XSLT scripts and associated resources for generating a HL7 FM compliant MAX file to be imported in the HL7 FM EA Extension
issue-scripts|XSLT scripts used for bug-fixing of some of the HL7 FM MAX files
publication|Publication tooling for generating publication artifacts (PDF, HTML, CSV) of the FM and FP
validation|Validation schematrons for validating consistency of a FM and FP
MAX_EA.ClassLibrary.Tests|Unit tests Visual Studio project for the MAX EA Extension
MAX_EA.ClassLibrary|The source code Visual Studio project for the MAX EA Extension
MAX_EA.ConsoleApp|The source code Visual Studio project exposes the MAX functionality on the Console
MAX_EA_Extension.ClassLibrary|The source code Visual Studio project that exposes the MAX functionality in Sparx EA
MAX_EA_Extension.Setup|Setup Visual Studio project for the MAX EA Extension

N.B. The MAX_EA packages will be moved to its own git repository.

----------------
Dockerfile

Create the Docker container. Do this once.
```
> docker build -t hl7-fm .
```

Start the Docker container
```
> docker run -it -v "$(pwd)":/app hl7-fm
```
!!! see example scripts gen-**fp.sh

----------------
Using the ehrsfm-tool command line tools to validate and compile FM, ProfileDefinition and Profiles

```
> cd /app/HL7_FM_CLI.ConsoleApp
> dotnet run
```
-----------------
import-scripts

!!! TODO; see FMFP Import Script Steps.docx !!!

```
> cd /app/import-scripts
> ant xlsx2max.sh
```
-----------------
Publication tool

```
> cd /app/publication
> # specials ant 3UFP|3PHRSFM
> ant FP -Dname=<name>
```