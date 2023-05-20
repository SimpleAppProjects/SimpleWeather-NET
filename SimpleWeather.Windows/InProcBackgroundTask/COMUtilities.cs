using System.Runtime.InteropServices;
using Windows.Win32;
using Windows.Win32.System.Com;

namespace SimpleWeather.NET
{
    public static class COMUtilities
    {
        public static void RegisterClass<T>(IClassFactory classFactory)
        {
            RegisterClassObject(typeof(T).GUID, classFactory);
        }

        private static void RegisterClassObject(Guid clsid, object factory)
        {
            int hr = PInvoke.CoRegisterClassObject(in clsid, factory, CLSCTX.CLSCTX_LOCAL_SERVER, REGCLS.REGCLS_MULTIPLEUSE, out uint _);
            if (hr < 0)
            {
                Marshal.ThrowExceptionForHR(hr);
            }
        }
    }
}
