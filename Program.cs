using System;
using System.Diagnostics;
using System.Management;

namespace iisewp
{
    class Program
    {
        private const string IIS_EXPRESS_PROCESS_NAME = "iisexpress";
        private const string COMMAND_LINE_SITE = "/site:\"";
        private const string COMMAND_LINE_APPPOOL = "/apppool:\"";

        static void Main(string[] args)
        {
            Process[] iisExpressProcesses = GetIISExpressProcesses();
            if (0 == iisExpressProcesses.Length)
            {
                Console.WriteLine("No IIS Express processes found.");
                return;
            }

            Console.WriteLine("PID     Site Name      App Pool");
            Console.WriteLine("---------------------------------------------");

            DisplayInformationForProcesses(iisExpressProcesses);
        }

        private static Process[] GetIISExpressProcesses()
        {
            return Process.GetProcessesByName(IIS_EXPRESS_PROCESS_NAME);
        }

        private static void DisplayInformationForProcesses(Process[] processes)
        {
            foreach (Process iisProcess in processes)
            {
                var processQuery = new SelectQuery("Win32_Process", string.Format("ProcessID=\"{0}\"", iisProcess.Id));
                var searcher = new ManagementObjectSearcher(processQuery);

                foreach (ManagementObject processResult in searcher.Get())
                {
                    DisplayProcessDetailsFromCommandLine(iisProcess.Id, processResult["CommandLine"].ToString());
                }
            }
        }

        private static void DisplayProcessDetailsFromCommandLine(int processId, string commandLine)
        {
            string siteText = GetSwitchValue(commandLine, COMMAND_LINE_SITE);
            string appPoolText = GetSwitchValue(commandLine, COMMAND_LINE_APPPOOL);

            Console.WriteLine(string.Format("{0}\t{1}\t{2}", processId.ToString(), siteText, appPoolText));
        }

        private static string GetSwitchValue(string commandLine, string switchName)
        {
            int startTextPosition = commandLine.IndexOf(switchName);
            string partialText = commandLine.Substring(startTextPosition + switchName.Length);
            int endTextPosition = partialText.IndexOf('\"');
            return partialText.Substring(0, endTextPosition);
        }
    }
}
