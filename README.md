This tool comes to patch [ETW](https://learn.microsoft.com/pt-br/windows-hardware/drivers/devtest/event-tracing-for-windows--etw-) writing a RET instruction (`0x3c`) in `NtTraceEvent` address. Otherwise, this project uses only NT APIs functions.

> ###### `NtOpenProcess`: get process handle <br> `NtQueryInformationProcess`: get NTDLL address <br> `LdrGetProcedureAddress`: get target function address <br> `NtProtectVirtualMemory`: change protection memory <br> `NtAllocateVirtualMemory`: allocate local memory <br> `NtWriteVirtualMemory`: write memory <br> `NtReadVirtualMemory`: read memory

Enjoy!
