using System.Collections.Immutable;
using Vanara.PInvoke;
using static Vanara.PInvoke.Ole32;

namespace SimpleWeather.NET
{
    public static class COMUtilities
    {
        private static readonly HashSet<uint> RegistrationCookies = [];

        public static uint RegisterClass<T>(IClassFactory classFactory)
        {
            return RegisterClassObject(typeof(T).GUID, classFactory);
        }

        private static uint RegisterClassObject(Guid clsid, object factory)
        {
            HRESULT hr = CoRegisterClassObject(in clsid, factory, CLSCTX.CLSCTX_LOCAL_SERVER, REGCLS.REGCLS_MULTIPLEUSE | REGCLS.REGCLS_SUSPENDED, out uint cookie);
            hr.ThrowIfFailed();

            RegistrationCookies.Add(cookie);

            hr = CoResumeClassObjects();
            hr.ThrowIfFailed();

            return cookie;
        }

        public static void UnregisterClassObject(uint cookie)
        {
            CoRevokeClassObject(cookie);
            RegistrationCookies.Remove(cookie);
        }

        public static void RevokeRegistrations()
        {
            var cookies = RegistrationCookies.ToImmutableHashSet();
            foreach (var cookie in cookies)
            {
                UnregisterClassObject(cookie);
            }
        }
    }
}
