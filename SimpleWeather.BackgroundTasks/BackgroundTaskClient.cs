using System;
using System.Runtime.InteropServices;
using Windows.ApplicationModel.Background;
using Windows.Win32;
using Windows.Win32.System.Com;
using WinRT;

namespace SimpleWeather.BackgroundTasks
{
    internal class BackgroundTaskClient
    {
        private static Guid inProcBackgroundTaskHostGuid = new Guid("148C5627-665B-4DAC-AB27-64397E80335A");

        private readonly IBackgroundTask backgroundTask;

        public BackgroundTaskClient()
        {
            // Activate the object Out of Process
            object obj;

            try
            {
                int hr = PInvoke.CoCreateInstance(inProcBackgroundTaskHostGuid, null, CLSCTX.CLSCTX_LOCAL_SERVER, typeof(IBackgroundTask).GUID, out obj);
                if (hr < 0)
                {
                    Marshal.ThrowExceptionForHR(hr);
                }
            }
            catch (Exception)
            {
                int hr = PInvoke.CoCreateInstance(inProcBackgroundTaskHostGuid, null, CLSCTX.CLSCTX_LOCAL_SERVER, typeof(IBackgroundTask).GUID, out obj);
                if (hr < 0)
                {
                    Marshal.ThrowExceptionForHR(hr);
                }
            }

            this.backgroundTask = MarshalInterface<IBackgroundTask>.FromAbi(Marshal.GetIUnknownForObject(obj));
        }

        public void Run(IBackgroundTaskInstance taskInstance)
        {
            try
            {
                this.backgroundTask.Run(taskInstance);
            }
            catch (Exception)
            {
                this.backgroundTask.Run(taskInstance);
            }
        }
    }
}
