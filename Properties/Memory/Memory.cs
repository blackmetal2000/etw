using System;
using System.Runtime.InteropServices;

namespace SharpDiabellstar
{
    class Memory
    {
        public static Win32.PAGE_PROTECTION_FLAGS ChangeMemory(IntPtr hProcess, IntPtr BaseAddress, int protectSize, Win32.PAGE_PROTECTION_FLAGS ppf)
        {
            IntPtr size = new IntPtr(protectSize);

            var protectStatus = Win32.NtProtectVirtualMemory(
                hProcess,
                ref BaseAddress,
                ref size,
                ppf,
                out Win32.PAGE_PROTECTION_FLAGS OldAccessProtection
            );

            if (protectStatus != Win32.NTSTATUS.Success)
                throw new Exception($"NtProtectVirtualMemory ERROR! Status: {protectStatus}");

            return OldAccessProtection;

        }

        public static IntPtr AllocateLocalMemory(int RegionSize)
        {
            IntPtr BaseAddress = new IntPtr();
            IntPtr allocationSize = new IntPtr(RegionSize);

            var allocateStatus = Win32.NtAllocateVirtualMemory(
                new IntPtr(-1),
                ref BaseAddress,
                IntPtr.Zero,
                ref allocationSize,
                Win32.VIRTUAL_ALLOCATION_TYPE.MEM_COMMIT | Win32.VIRTUAL_ALLOCATION_TYPE.MEM_RESERVE,
                Win32.PAGE_PROTECTION_FLAGS.PAGE_EXECUTE_READWRITE
            );

            if (allocateStatus != Win32.NTSTATUS.Success)
                throw new Exception($"NtAllocateVirtualMemory ERROR! Status: {allocateStatus}");

            return BaseAddress;
        }

        public static void WriteMemory(IntPtr hProcess, IntPtr BaseAddress, byte[] buffer, int writeSize)
        {
            int writtenSize = 0;

            var writeStatus = Win32.NtWriteVirtualMemory(
                hProcess,
                BaseAddress,
                buffer,
                writeSize,
                ref writtenSize
            );

            if (writeStatus != Win32.NTSTATUS.Success)
                throw new Exception($"NtWriteVirtualMemory ERROR! Status: {writeStatus}");
        }

        public static IntPtr ReadMemory(IntPtr hProcess, IntPtr BaseAddress, IntPtr Buffer, int readSize)
        {
            int NumberOfBytesToReaded = 0;

            var memoryStatus = Win32.NtReadVirtualMemory(
                hProcess,
                BaseAddress,
                Buffer,
                readSize,
                ref NumberOfBytesToReaded
            );

            if (memoryStatus != Win32.NTSTATUS.Success && NumberOfBytesToReaded == 0)
                throw new Exception($"NtReadVirtualMemory ERROR! Status: {memoryStatus}");

            return Buffer;
        }
    }
}