using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;
using System.Diagnostics;

namespace Standalone
{
    public static class ProcessExtension
    {
        public static IEnumerable<Process> GetChildProcesses(this Process process)
        {
            List<Process> children = new List<Process>();
            ManagementObjectSearcher mos = new ManagementObjectSearcher(String.Format("Select * From Win32_Process Where ParentProcessID={0}", process.Id));

            foreach (ManagementObject mo in mos.Get())
            {
                children.Add(Process.GetProcessById(Convert.ToInt32(mo["ProcessID"])));
            }

            return children;
        }
    }
}
