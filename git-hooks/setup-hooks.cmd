@echo off
setlocal

set SCRIPT_DIR=%~dp0
set REPO_ROOT=%SCRIPT_DIR%..\..

cd /d "%REPO_ROOT%"

git config core.hooksPath git-hooks
echo Git hooks path set to git-hooks
