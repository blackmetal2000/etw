This tool comes to patch [ETW](https://learn.microsoft.com/pt-br/windows-hardware/drivers/devtest/event-tracing-for-windows--etw-) writing a RET instruction (0x3c) in `NtTraceEvent` address. Otherwise, this project uses only NT APIs functions. Such as:

- `NtOpenProcess`: get process handle
- `NtQueryInformationProcess`: get NTDLL address
- `LdrGetProcedureAddress`: get target function address
- `NtProtectVirtualMemory`: change protection memory
- `NtAllocateVirtualMemory`: allocate local memory
- `NtWriteVirtualMemory`: write memory
- `NtReadVirtualMemory`: read memory

Enjoy!
