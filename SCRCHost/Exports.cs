using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace SCRCHost
{
    public static class Exports
    {
        private const int S_OK = 0;
        private const int S_FALSE = 1;
        private const int E_NOINTERFACE = unchecked((int)0x80004002);
        private const int REGDB_E_READREGDB = unchecked((int)0x80040150);
        private const int REGDB_E_CLASSNOTREG = unchecked((int)0x80040154);

        [UnmanagedCallersOnlyAttribute(EntryPoint = "DllGetActivationFactory")]
        public static unsafe int DllGetActivationFactory(IntPtr activatableClassId, IntPtr* activationFactory)
        {
            *activationFactory = IntPtr.Zero;

            var runtimeClassId = global::WinRT.MarshalString.FromAbi(activatableClassId);

            try
            {
                var assembly = GetTargetAssembly(runtimeClassId);
                if (assembly == null)
                {
                    return REGDB_E_CLASSNOTREG;
                }

                var type = assembly.GetType("WinRT.Module");
                if (type == null)
                {
                    return REGDB_E_CLASSNOTREG;
                }
                var getActivationFactory = type.GetMethod("GetActivationFactory");
                if (getActivationFactory == null)
                {
                    return REGDB_E_READREGDB;
                }
                IntPtr factory = (IntPtr)getActivationFactory.Invoke(null, new object[] { runtimeClassId });
                if (factory == IntPtr.Zero)
                {
                    return E_NOINTERFACE;
                }
                *activationFactory = factory;
                return S_OK;
            }
            catch (Exception e)
            {
                Environment.FailFast("DllGetActivationFactory", e);
                return Marshal.GetHRForException(e);
            }
        }

        [UnmanagedCallersOnlyAttribute(EntryPoint = "DllCanUnloadNow")]
        public static int DllCanUnloadNow()
        {
            return S_FALSE;
        }

        private static Assembly GetTargetAssembly(string runtimeClassId)
        {
            string path = runtimeClassId;
            int index = path.LastIndexOf('.');
            string currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            while (index != -1)
            {
                path = path.Substring(0, index);
                var assemblyName = Path.Combine(currentDirectory, path + ".dll");
                try
                {
                    if (File.Exists(assemblyName))
                    {
                        return Assembly.LoadFrom(assemblyName);
                    }
                }
                catch (Exception)
                {
                    // Continue
                }

                index = path.LastIndexOf('.');
            }

            return null;
        }
    }
}