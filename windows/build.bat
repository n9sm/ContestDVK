@echo off
setlocal
cd /d "%~dp0"
set "CSC=%WINDIR%\Microsoft.NET\Framework64\v4.0.30319\csc.exe"
if not exist "%CSC%" set "CSC=%WINDIR%\Microsoft.NET\Framework\v4.0.30319\csc.exe"
if not exist "%CSC%" (
  echo ERROR: .NET Framework C# compiler not found.
  echo Install Microsoft .NET Framework 4.x or Visual Studio Build Tools.
  pause
  exit /b 1
)
if not exist "audio" mkdir audio
"%CSC%" /nologo /target:winexe /out:ContestDVK.exe /reference:System.Windows.Forms.dll /reference:System.Drawing.dll /reference:System.dll Program.cs
if errorlevel 1 (
  echo Build failed.
  pause
  exit /b 1
)
echo.
echo Build completed: %CD%\ContestDVK.exe
echo Keep the audio folder next to the EXE.
pause
