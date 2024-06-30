using System.Runtime.InteropServices;
using Vanara.PInvoke;

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
            HRESULT hr = Ole32.CoRegisterClassObject(in clsid, factory, Ole32.CLSCTX.CLSCTX_LOCAL_SERVER, Ole32.REGCLS.REGCLS_MULTIPLEUSE, out uint _);
            hr.ThrowIfFailed();
        }
    }
}
