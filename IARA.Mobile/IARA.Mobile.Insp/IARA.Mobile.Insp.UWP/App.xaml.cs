using FFImageLoading.Forms.Platform;
using IARA.Mobile.Insp.Fonts;
using IARA.Mobile.Insp.UWP.Renderers;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using System;
using System.Collections.Generic;
using System.Reflection;
using TechnoLogica.Xamarin.Fonts;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Xamarin.CommunityToolkit.Effects;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Maps;
using XamScroll = Xamarin.Forms.ScrollView;

namespace IARA.Mobile.Insp.UWP
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public sealed partial class App : Windows.UI.Xaml.Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            InitializeComponent();
            Suspending += OnSuspending;
#if DEBUG
            AppCenter.Start("f6c77461-2e47-4206-b0bc-2d4b3f85da29",
                typeof(Analytics), typeof(Crashes));
#else
            AppCenter.Start("e8fb6919-5e47-4091-a9d2-3865122b754d",
                typeof(Analytics), typeof(Crashes));
#endif
        }

        public string GetAppVersion()
        {
            Package package = Package.Current;
            PackageId packageId = package.Id;
            PackageVersion version = packageId.Version;

            return string.Format("{0}.{1}.{2}.{3}", version.Major, version.Minor, version.Build, version.Revision);
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                DebugSettings.EnableFrameRateCounter = true;
            }
#endif

            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                CachedImageRenderer.Init();
                Rg.Plugins.Popup.Popup.Init();
                Xamarin.Essentials.Platform.OnLaunched(e);

                Assembly[] assembliesToInclude = new Assembly[]
                {
                    typeof(CustomMapRenderer).GetTypeInfo().Assembly,
                    typeof(TouchEffect).GetTypeInfo().Assembly,
                    typeof(Xamarin.CommunityToolkit.UWP.Effects.PlatformTouchEffect).GetTypeInfo().Assembly,
                    typeof(Xamarin.Controls.SignaturePad_UWP_XamlTypeInfo.XamlMetaDataProvider).GetTypeInfo().Assembly,
                    typeof(Xamarin.Essentials.Geolocation).GetTypeInfo().Assembly,
                    typeof(Xamarin.Forms.ExportFontAttribute).GetTypeInfo().Assembly,
                    typeof(LocalFontAssembly).GetTypeInfo().Assembly,
                    typeof(FontAssembly).GetTypeInfo().Assembly,
                };

                Xamarin.Forms.Forms.Init(e, Rg.Plugins.Popup.Popup.GetExtraAssemblies(assembliesToInclude));

                RemoveFaultyRenderers();

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                rootFrame.Navigate(typeof(MainPage), e.Arguments);
            }

            ApplicationView appView = ApplicationView.GetForCurrentView();
            appView.Title = GetAppVersion();

            // Ensure the current window is active
            Window.Current.Activate();
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        private void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            SuspendingDeferral deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }

        private void RemoveFaultyRenderers()
        {
            Type type = Registrar.Registered.GetType();

            Dictionary<Type, Dictionary<Type, (Type target, short priority)>> renderers =
                (Dictionary<Type, Dictionary<Type, (Type target, short priority)>>)type
                    .GetField("_handlers", BindingFlags.NonPublic | BindingFlags.Instance)
                    .GetValue(Registrar.Registered);

            renderers.Remove(typeof(Map));
            renderers.Remove(typeof(XamScroll));

            Registrar.Registered.Register(typeof(Map), typeof(CustomMapRenderer));
            Registrar.Registered.Register(typeof(XamScroll), typeof(CustomScrollViewRenderer));
        }
    }
}
