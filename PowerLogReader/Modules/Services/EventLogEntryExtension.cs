using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Linq;

namespace PowerLogReader.Modules.Services
{
    static class EventLogEntryExtension
    {
        private const string SourceKernelPower = "Microsoft-Windows-Kernel-Power";
        private static readonly long[] KernelPowerOnInstances = { 131, 507};
        private static readonly long[] KernelPowerOffInstances = { 41, 42, 109, 506};
        private const string SourceKernelBoot = "Microsoft-Windows-Kernel-Boot";
        private static readonly long[] KernelBootOnInstances = { 30 };
        private const string SourceKernelGeneral = "Microsoft-Windows-Kernel-General";
        private static readonly long[] KernelGeneralOnInstances = { 12 };
        private static readonly long[] KernelGeneralOffInstances = { 13 };
        private const string SourceEventLog = "EventLog";
        private static readonly long[] EventLogOnInstances = { 6005 };
        private static readonly long[] EventLogOffInstances = { 6006 };

        private static readonly Dictionary<string, string> SourceDic = new Dictionary<string, string>()
        {
            {SourceKernelPower, "Kernel-Power" },
            {SourceKernelBoot, "Kernel-Boot" },
            {SourceKernelGeneral, "Kernel-General" },
        };

        public static string GetSourceName(this PowerLogServiceBase pls, string src)
        {
            if (SourceDic.ContainsKey(src))
            {
                return SourceDic[src];
            } else
            {
                return src;
            }
        }


        public static bool IsPowerOnEvent(this EventRecord record)
        {
            var result = false;
            switch (record.ProviderName)
            {
                case SourceEventLog:
                    result = (EventLogOnInstances.Contains(record.Id));
                    break;
                case SourceKernelPower:
                    result = (KernelPowerOnInstances.Contains(record.Id));
                    break;
                case SourceKernelBoot:
                    result = (KernelBootOnInstances.Contains(record.Id));
                    break;
                case SourceKernelGeneral:
                    result = (KernelGeneralOnInstances.Contains(record.Id));
                    break;
                default:
                    break;
            }
            return result;
        }

        public static bool IsPowerOffEvent(this EventRecord record)
        {
            var result = false;
            switch (record.ProviderName)
            {
                case SourceEventLog:
                    result = (EventLogOffInstances.Contains(record.Id));
                    break;
                case SourceKernelPower:
                    result = (KernelPowerOffInstances.Contains(record.Id));
                    break;
                case SourceKernelGeneral:
                    result = (KernelGeneralOffInstances.Contains(record.Id));
                    break;
                default:
                    break;
            }
            return result;
        }

        public static bool IsPowerOnEvent(this EventLogEntry entry)
        {
            var result = false;
            switch (entry.Source)
            {
                case SourceEventLog:
                    result = (EventLogOnInstances.Contains(entry.InstanceId));
                    break;
                case SourceKernelPower:
                    result = (KernelPowerOnInstances.Contains(entry.InstanceId));
                    break;
                case SourceKernelBoot:
                    result = (KernelBootOnInstances.Contains(entry.InstanceId));
                    break;
                case SourceKernelGeneral:
                    result = (KernelGeneralOnInstances.Contains(entry.InstanceId));
                    break;
                default:
                    break;
            }
            return result;
        }

        public static bool IsPowerOffEvent(this EventLogEntry entry)
        {
            var result = false;
            switch (entry.Source)
            {
                case SourceEventLog:
                    result = (EventLogOffInstances.Contains(entry.InstanceId));
                    break;
                case SourceKernelPower:
                    result = (KernelPowerOffInstances.Contains(entry.InstanceId));
                    break;
                case SourceKernelGeneral:
                    result = (KernelGeneralOffInstances.Contains(entry.InstanceId));
                    break;
                default:
                    break;
            }
            return result;
        }
    }
}
