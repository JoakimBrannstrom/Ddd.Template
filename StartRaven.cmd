@tasklist /FI "IMAGENAME eq Raven.Server.exe" 2>NUL | find /I /N "Raven.Server.exe">NUL

@if "%ERRORLEVEL%"=="0" GOTO ALREADYRUNNING

@echo **********************
@echo * [Starting RavenDb] *
@echo **********************
:: @start D:\Dropbox\Dev\Svn\LinkBlog\packages\RavenDB.1.0.888\server\Raven.Server.exe
@start D:\Dropbox\Dev\Git\DocumentStation\packages\RavenDB.Server.2.0.2375\tools\Raven.Server.exe

@GOTO STARTPROCESS

:ALREADYRUNNING
@echo *******************************
@echo * Raven was already running.. *
@echo *******************************

:STARTPROCESS
:: @if NOT [%1]==[] @start %1

:END
exit 0
