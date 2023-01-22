using System;
using System.Collections.Generic;
using System.Threading;
using Windows.ApplicationModel.Background;
using WinRT;

namespace SimpleWeather.NET
{
    public partial class App
    {
        private readonly SynchronizationContext syncContext;
        private ClassFactory<InProcBackgroundTask> inProcBackgroundTaskFactory;

        private void RegisterCOMServer()
        {
            this.inProcBackgroundTaskFactory = new ClassFactory<InProcBackgroundTask>(
                () => new InProcBackgroundTask(),
                new Dictionary<Guid, Func<object, IntPtr>>()
                {
                    { typeof(IBackgroundTask).GUID, obj => MarshalInterface<IBackgroundTask>.FromManaged((IBackgroundTask)obj) },
                });
            // On launch register the BackgroundTask class for OOP COM activation
            COMUtilities.RegisterClass<InProcBackgroundTask>(this.inProcBackgroundTaskFactory);
        }
    }
}
