Servers srsrting:
  
[POWERSHELL(ADMIN)] - cd "C:\Users\bogac\OneDrive\Desktop\DistLoadV2\Server1"
dotnet run

[POWERSHELL(ADMIN)] - cd "C:\Users\bogac\OneDrive\Desktop\DistLoadV2\Server2"
dotnet run

[POWERSHELL(ADMIN)] - cd "C:\Users\bogac\OneDrive\Desktop\DistLoadV2\Server3"
dotnet run




K6 starting:

[POWERSHELL(ADMIN)] - Set-ExecutionPolicy Bypass -Scope Process -Force; `
		     [System.Net.ServicePointManager]::SecurityProtocol = [System.Net.ServicePointManager]::SecurityProtocol -bor 3072; `
		     iex ((New-Object System.Net.WebClient).DownloadString('https://community.chocolatey.org/install.ps1'))

[POWERSHELL(ADMIN)] - choco install k6

[POWERSHELL(ADMIN)] - cd "C:\Users\bogac\OneDrive\Desktop\DistLoadV2\DistLoad\LoadTesting"

[POWERSHELL] - k6 run test-scenario.js