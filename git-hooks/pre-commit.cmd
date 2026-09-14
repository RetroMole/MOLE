@echo off
setlocal

set HOOK_DIR=%~dp0
set REPO_ROOT=%HOOK_DIR%..\

cd /d "%REPO_ROOT%"

echo Running dotnet format...
dotnet format RetroMole.sln --exclude "submodules/**" style
dotnet format RetroMole.sln --exclude "submodules/**" whitespace

:: echo Running dotnet build...
:: dotnet build RetroMole.sln

echo Pre-commit checks passed.
exit /b 0
