using Vanara.PInvoke;
using static Vanara.PInvoke.Ole32;

namespace SimpleWeather.NET
{
    public static class COMUtilities
    {
        private static readonly HashSet<uint> RegistrationCookies = [];

        public static void RegisterClass<T>(IClassFactory classFactory)
        {
            RegisterClassObject(typeof(T).GUID, classFactory);
        }

        private static void RegisterClassObject(Guid clsid, object factory)
        {
            HRESULT hr = CoRegisterClassObject(in clsid, factory, CLSCTX.CLSCTX_LOCAL_SERVER, REGCLS.REGCLS_MULTIPLEUSE | REGCLS.REGCLS_SUSPENDED, out uint cookie);
            hr.ThrowIfFailed();

            RegistrationCookies.Add(cookie);

            hr = CoResumeClassObjects();
            hr.ThrowIfFailed();
        }

        public static void UnregisterClassObject(uint cookie)
        {
            CoRevokeClassObject(cookie);
        }

        public static void RevokeRegistrations()
        {
            foreach (var cookie in RegistrationCookies)
            {
                UnregisterClassObject(cookie);
            }
        }
    }
}
