using System.Runtime.InteropServices;
using Windows.ApplicationModel.Background;

namespace SimpleWeather.NET
{
    // The InProcBackgroundTask must be visible to COM and must be given a GUID such
    // that the system can identify this entry point and launch it as necessary
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    [Guid("E3E44B22-74AE-47CE-A507-6EBE2F832B8F")]
    [ComSourceInterfaces(typeof(IBackgroundTask))]
    public class InProcBackgroundTask : IBackgroundTask
    {
        [MTAThread]
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
                deferral?.Complete();
            }
        }
    }
}
