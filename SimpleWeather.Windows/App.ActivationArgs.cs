using SimpleWeather.Utils;
using Windows.ApplicationModel.Activation;

namespace SimpleWeather.NET
{
    /// <summary>
    /// A wrapper class to handle both 
    /// Microsoft.UI.Xaml.LaunchActivatedEventArgs and Windows.ApplicationModel.Activation.LaunchActivatedEventArgs
    /// </summary>
    public class LaunchActivatedEventExArgs
    {
        internal LaunchActivatedEventExArgs() { }

        public bool PrelaunchActivated { get; init; }
        public string TileId { get; init; }
        public ApplicationExecutionState PreviousExecutionState { get; init; }
        public string Arguments { get; init; }
        public ActivationKind Kind { get; init; }
    }

    public static class LaunchActivatedEventExArgsExtensions
    {
        public static LaunchActivatedEventExArgs ToCompatArgs(this Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            var preLaunchActivated = args.RunCatching(() =>
            {
                return args?.UWPLaunchActivatedEventArgs?.PrelaunchActivated ?? false;
            }).GetOrDefault(false);
            var tileId = args.RunCatching(() =>
            {
                return args?.UWPLaunchActivatedEventArgs?.TileId;
            }).GetOrNull();
            var prevExecState = args.RunCatching(() =>
            {
                return args?.UWPLaunchActivatedEventArgs?.PreviousExecutionState ?? ApplicationExecutionState.NotRunning;
            }).GetOrDefault(ApplicationExecutionState.NotRunning);
            var arguments = args.RunCatching(() =>
            {
                return args?.UWPLaunchActivatedEventArgs?.Arguments;
            }).GetOrNull();
            var activKind = args.RunCatching(() =>
            {
                return args?.UWPLaunchActivatedEventArgs?.Kind ?? ActivationKind.Launch;
            }).GetOrDefault(ActivationKind.Launch);

            return new LaunchActivatedEventExArgs()
            {
                PrelaunchActivated = preLaunchActivated,
                TileId = tileId,
                PreviousExecutionState = prevExecState,
                Arguments = arguments,
                Kind = activKind
            };
        }

        public static LaunchActivatedEventExArgs ToCompatArgs(this Windows.ApplicationModel.Activation.LaunchActivatedEventArgs args)
        {
            return new LaunchActivatedEventExArgs()
            {
                PrelaunchActivated = args.PrelaunchActivated,
                TileId = args.TileId,
                PreviousExecutionState = args.PreviousExecutionState,
                Arguments = args.Arguments,
                Kind = args.Kind
            };
        }
    }
}
