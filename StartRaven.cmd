@tasklist /FI "IMAGENAME eq Raven.Server.exe" 2>NUL | find /I /N "Raven.Server.exe">NUL

@if "%ERRORLEVEL%"=="0" GOTO ALREADYRUNNING

@echo **********************
@echo * [Starting RavenDb] *
@echo **********************

:: echo %cd% > currentDir.txt
@start .\packages\RavenDB.Server.2.5.2700\tools\Raven.Server.exe

@GOTO STARTPROCESS

:ALREADYRUNNING
@echo *******************************
@echo * Raven was already running.. *
@echo *******************************

:STARTPROCESS
:: @if NOT [%1]==[] @start %1

:END
exit 0
