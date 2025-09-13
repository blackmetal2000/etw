## ETW

This tool comes to patch [ETW](https://learn.microsoft.com/pt-br/windows-hardware/drivers/devtest/event-tracing-for-windows--etw-) writing a RET instruction (`0x3c`) in `NtTraceEvent` address. Otherwise, this project only uses NT APIs functions.

> ###### `NtOpenProcess`: get process handle <br> `NtQueryInformationProcess`: get NTDLL address <br> `LdrGetProcedureAddress`: get target function address <br> `NtProtectVirtualMemory`: change protection memory <br> `NtAllocateVirtualMemory`: allocate local memory <br> `NtWriteVirtualMemory`: write memory <br> `NtReadVirtualMemory`: read memory


#### Before

![alt text](https://i.imgur.com/GbYnY4k.png)

#### After

![alt text](https://i.imgur.com/2ezpAfd.png)

#### What to expect?

```txt
PS C:\Windows\Temp> .\etw.exe 9136
[^] Ntdll ADDRESS: 0000007FFF69770000
[^] NtTraceEvent ADDRESS: 0000007FFF6980E090 (offset: 9E090)
[^] Memory PAGE_EXECUTE_READ changed to PAGE_EXECUTE_READWRITE
[^] Enjoy!
PS C:\Windows\Temp>
```
