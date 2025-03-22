using System.Runtime.InteropServices;
using Windows.ApplicationModel.Background;

namespace SimpleWeather.NET
{
    // The InProcBackgroundTask must be visible to COM and must be given a GUID such
    // that the system can identify this entry point and launch it as necessary
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
#if DEBUG
    [Guid("148C5627-665B-4DAC-AB27-64397E80335A")]
#else
    [Guid("E3E44B22-74AE-47CE-A507-6EBE2F832B8F")]
#endif
    [ComSourceInterfaces(typeof(IBackgroundTask))]
    public partial class InProcBackgroundTask : IBackgroundTask
    {
        [MTAThread]
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            BackgroundTaskDeferral deferral = null;
            try
            {
                deferral = taskInstance.GetDeferral();

                if (App.Current is App app)
                {
                    await app.OnBackgroundActivatedAsync(taskInstance);
                }
            }
            finally
            {
                deferral?.Complete();
            }
        }
    }
}
