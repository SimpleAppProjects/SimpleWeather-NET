using System;
using System.Runtime.InteropServices;
using Vanara.PInvoke;
using Windows.ApplicationModel.Background;
using WinRT;

namespace SimpleWeather.BackgroundTasks
{
    internal class BackgroundTaskClient
    {
        private static Guid inProcBackgroundTaskHostGuid = new Guid("E3E44B22-74AE-47CE-A507-6EBE2F832B8F");

        private readonly IBackgroundTask backgroundTask;

        public BackgroundTaskClient()
        {
            // Activate the object Out of Process
            object obj;

            try
            {
                HRESULT hr = Ole32.CoCreateInstance(inProcBackgroundTaskHostGuid, null, Ole32.CLSCTX.CLSCTX_LOCAL_SERVER, typeof(IBackgroundTask).GUID, out obj);
                hr.ThrowIfFailed();
            }
            catch (Exception)
            {
                HRESULT hr = Ole32.CoCreateInstance(inProcBackgroundTaskHostGuid, null, Ole32.CLSCTX.CLSCTX_LOCAL_SERVER, typeof(IBackgroundTask).GUID, out obj);
                hr.ThrowIfFailed();
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
