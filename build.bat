IF [%1]==[] (SET CONFIG=Debug) ELSE (SET CONFIG=%1)
c:\Windows\Microsoft.NET\Framework64\v4.0.30319\msbuild.exe src\pacs.csproj /p:Configuration=%CONFIG%