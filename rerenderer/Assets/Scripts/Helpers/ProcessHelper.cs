#if UNITY_STANDALONE_WIN

// https://forum.unity.com/threads/solved-il2cpp-and-process-start.533988/#post-7508105

using Microsoft.Win32.SafeHandles;
using System;
using System.Diagnostics;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;

/// <summary>
/// Helper system for process (to allow working with IL2CPP generated code)
/// </summary>
public static class ProcessHelper
{
    #region Variables
    
    [DllImport("shell32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    internal static extern uint ShellExecute(IntPtr hwnd, string strOperation, string strFile, string strParameters, string strDirectory, Int32 nShowCmd);

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool CreateProcess
    (
        String lpApplicationName,
        String lpCommandLine,
        SecurityAttributes lpProcessAttributes,
        SecurityAttributes lpThreadAttributes,
        Boolean bInheritHandles,
        CreateProcessFlags dwCreationFlags,
        IntPtr lpEnvironment,
        String lpCurrentDirectory,
        [In] StartupInfo lpStartupInfo,
        out ProcessInformation lpProcessInformation
    );

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool CloseHandle(IntPtr hObject);

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool TerminateProcess(IntPtr processHandle, uint exitCode);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr OpenProcess(ProcessAccessRights access, bool inherit, uint processId);

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetExitCodeProcess(IntPtr hProcess, out uint lpExitCode);

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern UInt32 WaitForSingleObject(IntPtr hHandle, UInt32 dwMilliseconds);

    [DllImport("kernel32.dll")]
    private static extern uint GetCurrentProcessId();

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern bool SetPriorityClass(IntPtr handle, ProcessPriorityClass priorityClass);

    [StructLayout(LayoutKind.Sequential)]
    public class StartupInfo
    {
        public Int32 cb = 0;
        public IntPtr lpReserved = IntPtr.Zero;
        public IntPtr lpDesktop = IntPtr.Zero; // MUST be Zero
        public IntPtr lpTitle = IntPtr.Zero;
        public Int32 dwX = 0;
        public Int32 dwY = 0;
        public Int32 dwXSize = 0;
        public Int32 dwYSize = 0;
        public Int32 dwXCountChars = 0;
        public Int32 dwYCountChars = 0;
        public Int32 dwFillAttribute = 0;
        public Int32 dwFlags = 0;
        public Int16 wShowWindow = 0;
        public Int16 cbReserved2 = 0;
        public IntPtr lpReserved2 = IntPtr.Zero;
        public IntPtr hStdInput = IntPtr.Zero;
        public IntPtr hStdOutput = IntPtr.Zero;
        public IntPtr hStdError = IntPtr.Zero;

        public StartupInfo()
        {
            this.cb = Marshal.SizeOf(this);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ProcessInformation
    {
        public IntPtr hProcess;
        public IntPtr hThread;
        public Int32 dwProcessId;
        public Int32 dwThreadId;
    }

    [StructLayout(LayoutKind.Sequential)]
    public class SecurityAttributes
    {
        public Int32 Length = 0;
        public IntPtr lpSecurityDescriptor = IntPtr.Zero;
        public bool bInheritHandle = false;

        public SecurityAttributes()
        {
            this.Length = Marshal.SizeOf(this);
        }
    }

    [Flags]
    public enum CreateProcessFlags : uint
    {
        DEBUG_PROCESS = 0x00000001,
        DEBUG_ONLY_THIS_PROCESS = 0x00000002,
        CREATE_SUSPENDED = 0x00000004,
        DETACHED_PROCESS = 0x00000008,
        CREATE_NEW_CONSOLE = 0x00000010,
        NORMAL_PRIORITY_CLASS = 0x00000020,
        IDLE_PRIORITY_CLASS = 0x00000040,
        HIGH_PRIORITY_CLASS = 0x00000080,
        REALTIME_PRIORITY_CLASS = 0x00000100,
        CREATE_NEW_PROCESS_GROUP = 0x00000200,
        CREATE_UNICODE_ENVIRONMENT = 0x00000400,
        CREATE_SEPARATE_WOW_VDM = 0x00000800,
        CREATE_SHARED_WOW_VDM = 0x00001000,
        CREATE_FORCEDOS = 0x00002000,
        BELOW_NORMAL_PRIORITY_CLASS = 0x00004000,
        ABOVE_NORMAL_PRIORITY_CLASS = 0x00008000,
        INHERIT_PARENT_AFFINITY = 0x00010000,
        INHERIT_CALLER_PRIORITY = 0x00020000,
        CREATE_PROTECTED_PROCESS = 0x00040000,
        EXTENDED_STARTUPINFO_PRESENT = 0x00080000,
        PROCESS_MODE_BACKGROUND_BEGIN = 0x00100000,
        PROCESS_MODE_BACKGROUND_END = 0x00200000,
        CREATE_BREAKAWAY_FROM_JOB = 0x01000000,
        CREATE_PRESERVE_CODE_AUTHZ_LEVEL = 0x02000000,
        CREATE_DEFAULT_ERROR_MODE = 0x04000000,
        CREATE_NO_WINDOW = 0x08000000,
        PROFILE_USER = 0x10000000,
        PROFILE_KERNEL = 0x20000000,
        PROFILE_SERVER = 0x40000000,
        CREATE_IGNORE_SYSTEM_DEFAULT = 0x80000000,
    }

    [Flags]
    private enum ProcessAccessRights : uint
    {
        PROCESS_CREATE_PROCESS = 0x0080, //  Required to create a process.
        PROCESS_CREATE_THREAD = 0x0002, //  Required to create a thread.
        PROCESS_DUP_HANDLE = 0x0040, // Required to duplicate a handle using DuplicateHandle.
        PROCESS_QUERY_INFORMATION = 0x0400, //  Required to retrieve certain information about a process, such as its token, exit code, and priority class (see OpenProcessToken, GetExitCodeProcess, GetPriorityClass, and IsProcessInJob).
        PROCESS_QUERY_LIMITED_INFORMATION = 0x1000, //  Required to retrieve certain information about a process (see QueryFullProcessImageName). A handle that has the PROCESS_QUERY_INFORMATION access right is automatically granted PROCESS_QUERY_LIMITED_INFORMATION. Windows Server 2003 and Windows XP/2000:  This access right is not supported.
        PROCESS_SET_INFORMATION = 0x0200, //    Required to set certain information about a process, such as its priority class (see SetPriorityClass).
        PROCESS_SET_QUOTA = 0x0100, //  Required to set memory limits using SetProcessWorkingSetSize.
        PROCESS_SUSPEND_RESUME = 0x0800, // Required to suspend or resume a process.
        PROCESS_TERMINATE = 0x0001, //  Required to terminate a process using TerminateProcess.
        PROCESS_VM_OPERATION = 0x0008, //   Required to perform an operation on the address space of a process (see VirtualProtectEx and WriteProcessMemory).
        PROCESS_VM_READ = 0x0010, //    Required to read memory in a process using ReadProcessMemory.
        PROCESS_VM_WRITE = 0x0020, //   Required to write to memory in a process using WriteProcessMemory.
        DELETE = 0x00010000, // Required to delete the object.
        READ_CONTROL = 0x00020000, //   Required to read information in the security descriptor for the object, not including the information in the SACL. To read or write the SACL, you must request the ACCESS_SYSTEM_SECURITY access right. For more information, see SACL Access Right.
        SYNCHRONIZE = 0x00100000, //    The right to use the object for synchronization. This enables a thread to wait until the object is in the signaled state.
        WRITE_DAC = 0x00040000, //  Required to modify the DACL in the security descriptor for the object.
        WRITE_OWNER = 0x00080000, //    Required to change the owner in the security descriptor for the object.
        STANDARD_RIGHTS_REQUIRED = 0x000f0000,
        PROCESS_ALL_ACCESS = STANDARD_RIGHTS_REQUIRED | SYNCHRONIZE | 0xFFF //    All possible access rights for a process object.
    }

    private const UInt32 INFINITE = 0xFFFFFFFF;
    private const UInt32 WAIT_ABANDONED = 0x00000080;
    private const UInt32 WAIT_OBJECT_0 = 0x00000000;
    private const UInt32 WAIT_TIMEOUT = 0x00000102;

    private const int ERROR_NO_MORE_FILES = 0x12;
    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern SafeSnapshotHandle CreateToolhelp32Snapshot(SnapshotFlags flags, uint id);
    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool Process32First(SafeSnapshotHandle hSnapshot, ref PROCESSENTRY32 lppe);
    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool Process32Next(SafeSnapshotHandle hSnapshot, ref PROCESSENTRY32 lppe);

    [Flags]
    private enum SnapshotFlags : uint
    {
        HeapList = 0x00000001,
        Process = 0x00000002,
        Thread = 0x00000004,
        Module = 0x00000008,
        Module32 = 0x00000010,
        All = (HeapList | Process | Thread | Module),
        Inherit = 0x80000000,
        NoHeaps = 0x40000000
    }
    [StructLayout(LayoutKind.Sequential)]
    private struct PROCESSENTRY32
    {
        public uint dwSize;
        public uint cntUsage;
        public uint th32ProcessID;
        public IntPtr th32DefaultHeapID;
        public uint th32ModuleID;
        public uint cntThreads;
        public uint th32ParentProcessID;
        public int pcPriClassBase;
        public uint dwFlags;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)] public string szExeFile;
    };

    [SuppressUnmanagedCodeSecurity] //, HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
    internal sealed class SafeSnapshotHandle : SafeHandleMinusOneIsInvalid
    {
        internal SafeSnapshotHandle() : base(true)
        {
        }

        [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
        internal SafeSnapshotHandle(IntPtr handle) : base(true)
        {
            base.SetHandle(handle);
        }

        protected override bool ReleaseHandle()
        {
            return CloseHandle(base.handle);
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success), DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        private static extern bool CloseHandle(IntPtr handle);
    }

    #endregion

    #region Core
    /// <summary>
    /// Starts the given file
    /// </summary>
    /// <param name="path"></param>
    /// <param name="arguments"></param>
    /// <param name="dir"></param>
    /// <param name="hidden"></param>
    /// <param name="processID"></param>
    /// <returns></returns>
    public static bool Start(string path,
        string arguments,
        string dir,
        bool hidden,
        out uint processID)
    {
        ProcessInformation processinfo;
        processID = 0;
        CreateProcessFlags flags = hidden ? CreateProcessFlags.CREATE_NO_WINDOW : CreateProcessFlags.REALTIME_PRIORITY_CLASS;
        StartupInfo startupinfo = new StartupInfo();
        SecurityAttributes processAttrs = new SecurityAttributes();
        SecurityAttributes threadAttrs = new SecurityAttributes();
        if (!CreateProcess(null,
            $"\"{path}\" {arguments}",
            processAttrs,
            threadAttrs,
            false,
            flags,
            IntPtr.Zero,
            dir,
            startupinfo,
            out processinfo))
        {
            string errorMessage = new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error()).Message;
            UnityEngine.Debug.LogError($"Unable to start process {path} {arguments} {dir} -> {Marshal.GetLastWin32Error()}: {errorMessage}");
            return (false);
        }

        processID = (uint)processinfo.dwProcessId;
        return (true);
    }

    public static bool StartUrl(string url)
    {
        string def = string.Empty;
        IntPtr hWnd = (IntPtr)0;
        return ShellExecute(hWnd, def, url, def, def, 0) == 0;
    }

    private static IntPtr GetProcessHandle(uint processID)
    {
        return (OpenProcess(ProcessAccessRights.PROCESS_ALL_ACCESS, false, processID));
    }

    /// <summary>
    /// Checks if the given process has ended
    /// </summary>
    /// <param name="processID"></param>
    /// <returns></returns>
    public static bool IsProcessEnded(uint processID)
    {
        IntPtr handle = GetProcessHandle(processID);
        if (GetExitCodeProcess(handle, out uint lpExitCode))
        {
            return (lpExitCode != 259);
        }
        else
        {
            return (true);
        }
    }

    /// <summary>
    /// Kills the given process
    /// </summary>
    /// <param name="processID"></param>
    /// <returns></returns>
    public static bool KillProcess(uint processID)
    {
        IntPtr handle = GetProcessHandle(processID);
        if (handle == IntPtr.Zero)
        {
            return (false);
        }
        if (!TerminateProcess(handle, 0))
        {
            return (false);
        }
        if (!CloseHandle(handle))
        {
            return (false);
        }
        return (true);
    }

    /// <summary>
    /// Waits till the given process has exited.
    /// This will stall the main thread, e.g. will freezes the app!
    /// </summary>
    /// <param name="processID"></param>
    public static void WaitForExit(uint processID)
    {
        IntPtr handle = GetProcessHandle(processID);
        if (handle == IntPtr.Zero)
        {
            return;
        }
        UInt32 result = WaitForSingleObject(handle, INFINITE);
        //todo: check result...
    }

    /// <summary>
    /// Get my current process ID
    /// </summary>
    /// <returns></returns>
    public static int GetCurrentProcessID()
    {
        uint id = GetCurrentProcessId();
        return (Convert.ToInt32(id));
    }

    /// <summary>
    /// Get the parent process ID
    /// </summary>
    /// <param name="Id"></param>
    /// <returns></returns>
    public static int GetParentProcessID(int Id)
    {
        PROCESSENTRY32 pe32 = new PROCESSENTRY32 { };
        pe32.dwSize = (uint)Marshal.SizeOf(typeof(PROCESSENTRY32));
        SafeSnapshotHandle hSnapshot = CreateToolhelp32Snapshot(SnapshotFlags.Process, (uint)Id);

        if (hSnapshot.IsInvalid)
        {
            return (-1);
        }

        if (!Process32First(hSnapshot, ref pe32))
        {
            int errno = Marshal.GetLastWin32Error();
            if (errno == ERROR_NO_MORE_FILES)
            {
                return -1;
            }
            return (-1);
        }
        do
        {
            if (pe32.th32ProcessID == (uint)Id)
                return (int)pe32.th32ParentProcessID;
        } while (Process32Next(hSnapshot, ref pe32));

        return (-1);
    }

    public static bool SetProcessPriority(uint processId, ProcessPriorityClass priority)
    {
        IntPtr handle = GetProcessHandle(processId);
        if (handle == IntPtr.Zero)
        {
            return false;
        }

        return SetPriorityClass(handle, priority);
    }

    #endregion

    #region Mutex

    /*
    [DllImport("kernel32.dll")]
    private static extern IntPtr CreateMutex(IntPtr lpMutexAttributes, bool bInitialOwner, string lpName);
    [DllImport("kernel32.dll")]
    private static extern bool ReleaseMutex(IntPtr hMutex);
 
    public static IntPtr CreateMutex(bool initialOwner,
        string name)
    {
        // create IntPtrs for use with CreateMutex()
        IntPtr ipMutexAttr = new IntPtr(0);
        IntPtr ipHMutex = new IntPtr(0);
        ipHMutex = CreateMutex(ipMutexAttr, initialOwner, name);
        if (ipHMutex != IntPtr.Zero) {
            int iGLE = Marshal.GetLastWin32Error();
            if (iGLE == 183) {// Win32Calls.ERROR_ALREADY_EXISTS)
                //allready exists
            }
        }
        return (ipHMutex);
    }
    */
    /*
    public static bool ReleaseMutex(IntPtr intPtr)
    {
        if (intPtr != IntPtr.Zero) {
            return (ReleaseMutex(intPtr));
        } else {
            return (false);
        }
    }
    */

    #endregion
}

#endif
