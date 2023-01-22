using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SimpleWeather.NET
{
    // https://docs.microsoft.com/windows/win32/api/unknwn/nn-unknwn-iclassfactory
    [ComImport]
    [ComVisible(false)]
    [Guid("00000001-0000-0000-C000-000000000046")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IClassFactory
    {
        void CreateInstance(
            [MarshalAs(UnmanagedType.Interface)] object pUnkOuter,
            ref Guid riid,
            out IntPtr ppvObject);

        void LockServer([MarshalAs(UnmanagedType.Bool)] bool fLock);
    }

    [ComVisible(true)]
    public class ClassFactory<T> : IClassFactory
    {
        private static readonly Guid IUnknown = new Guid("00000000-0000-0000-C000-000000000046");

        public const int CLASS_E_NOAGGREGATION = unchecked((int)0x80040110);

        private readonly Func<T> createFunction;
        private readonly Dictionary<Guid, Func<object, IntPtr>> marshalFuncByGuid;

        public ClassFactory(Func<T> createFunction, Dictionary<Guid, Func<object, IntPtr>> marshalFuncByGuid)
        {
            this.createFunction = createFunction ?? throw new ArgumentNullException(nameof(createFunction));
            this.marshalFuncByGuid = marshalFuncByGuid ?? throw new ArgumentNullException(nameof(marshalFuncByGuid));
        }

        public void CreateInstance(
            [MarshalAs(UnmanagedType.Interface)] object pUnkOuter,
            ref Guid riid,
            out IntPtr ppvObject)
        {
            if (pUnkOuter != null)
            {
                throw new COMException(string.Empty, CLASS_E_NOAGGREGATION);
            }

            object obj = this.createFunction();
            if (riid == IUnknown)
            {
                ppvObject = WinRT.MarshalInspectable<object>.FromManaged(obj);
            }
            else
            {
                if (!this.marshalFuncByGuid.TryGetValue(riid, out Func<object, IntPtr> marshalFunc))
                {
                    throw new InvalidCastException();
                }

                ppvObject = marshalFunc(obj);
            }
        }

        public void LockServer(bool fLock)
        {
            // No-op
        }
    }
}
