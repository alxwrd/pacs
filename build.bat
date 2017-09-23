IF [%1]==[] (SET CONFIG=Debug) ELSE (SET CONFIG=%1)
c:\Windows\Microsoft.NET\Framework64\v3.5\msbuild.exe src\pacs.csproj /p:Configuration=%CONFIG%