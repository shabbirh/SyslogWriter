# SyslogWriter

Simple Console application to write entries to a remote Syslog Server.

Usage:

```SyslogWriter 1.0.0
Copyright (C) 2023 SyslogWriter

  -s, --server     Required. The IP Address (or hostname) of the Syslog Server to Write To

  -p, --port       Required. The port that the Syslog Server is listening on (UDP)

  -l, --level      Required. Valid values are Debug, Info, Warn, Error, Alert, Critical, Emergency

  -m, --message    Required. The message you want to log

  --help           Display this help screen.

  --version        Display version information.
```

Example:

Assuming that you have a syslog server at host "my.syslog.server" or IP "10.10.10.10", listening on UDP port 514, and you want to log an error message:

```
SyslogWriter.exe --server 10.10.10.10 --port 514 --level Error --message "This is a test error message"
```

Levels can be as follows:

```
Debug
Info
Warn
Error
Alert
Critical
Emergency
```

This console application can be used as part of a scheduled task to write log events to a remote syslog server.  

The application is written in dotnet 6, and is cross platform.
