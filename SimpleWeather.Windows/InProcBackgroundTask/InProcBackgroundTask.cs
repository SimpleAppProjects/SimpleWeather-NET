using System;
using System.Runtime.InteropServices;
using Windows.ApplicationModel.Background;

namespace SimpleWeather.NET
{
    // The InProcBackgroundTask must be visible to COM and must be given a GUID such
    // that the system can identify this entry point and launch it as necessary
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    [Guid("148C5627-665B-4DAC-AB27-64397E80335A")]
    [ComSourceInterfaces(typeof(IBackgroundTask))]
    internal class InProcBackgroundTask : IBackgroundTask
    {
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            BackgroundTaskDeferral deferral = null;
            try
            {
                deferral = taskInstance.GetDeferral();

                await App.Current?.OnBackgroundActivatedAsync(taskInstance);
            }
            finally
            {
                if (deferral != null)
                {
                    deferral.Complete();
                }
            }
        }
    }
}
