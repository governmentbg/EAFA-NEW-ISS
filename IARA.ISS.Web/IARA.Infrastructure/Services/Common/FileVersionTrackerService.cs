using System.Diagnostics;
using System.IO;
using System.Reflection;
using IARA.Interfaces.Common;
using Microsoft.Extensions.DependencyInjection;
using TL.Dependency.Abstractions;
using TL.WebNotifications.Abstractions.Interfaces;
using TL.WebNotifications.Abstractions.Models;

namespace IARA.Infrastructure.Services.Common
{
    [ServiceScope(ServiceLifetime.Singleton)]
    public class FileVersionTrackerService : IFileVersionTrackerService
    {
        private const string VersionFileName = "version.json";
        private const string DefaultVersion = "1.0.0";
        private const string ContentFolderName = "Content";

        private DirectoryInfo contentDir;
        private bool disposedValue;
        private FileSystemWatcher fileSystemWatcher;
        private IScopedServiceProviderFactory scopedServiceProviderFactory;

        public FileVersionTrackerService(IScopedServiceProviderFactory scopedServiceProviderFactory)
        {
            this.scopedServiceProviderFactory = scopedServiceProviderFactory;
        }

        public DirectoryInfo ContentDir
        {
            get
            {
                if (contentDir == null)
                {
                    contentDir = new DirectoryInfo(Path.Combine(Directory.GetCurrentDirectory(), ContentFolderName));
                }

                return contentDir;
            }
        }


        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public string GetVersion()
        {
            string version = GetVersionFromJsonFile();

            string fileFullPath = Assembly.GetExecutingAssembly().Location;

            if (!string.IsNullOrEmpty(fileFullPath))
            {
                FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(fileFullPath);

                if (!string.IsNullOrEmpty(fileVersionInfo.ProductVersion) && fileVersionInfo.ProductVersion != "-")
                {
                    version = fileVersionInfo.ProductVersion;
                }
                else if (!string.IsNullOrEmpty(fileVersionInfo.FileVersion))
                {
                    version = fileVersionInfo.FileVersion;
                }
            }
            else
            {
                var informationalVersionAttribute = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>();

                if (informationalVersionAttribute != null && !string.IsNullOrEmpty(informationalVersionAttribute.InformationalVersion))
                {
                    version = informationalVersionAttribute.InformationalVersion;
                }
                else
                {
                    var versionAttribute = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyVersionAttribute>();

                    if (versionAttribute != null && !string.IsNullOrEmpty(versionAttribute.Version))
                    {
                        version = versionAttribute.Version;
                    }
                }
            }

            return version;
        }

        public void NotifyVersionChange()
        {
            using IScopedServiceProvider serviceProvider = scopedServiceProviderFactory.GetServiceProvider();

            var notificationsSender = serviceProvider.GetService<IWebNotificationsSender<WebNotificationTypes>>();

            notificationsSender.AddBroadcastNotification(new BroadcastNotificationWrapper<WebNotificationTypes>
            {
                Notification = new Notification<string, WebNotificationTypes>
                {
                    Message = GetVersion(),
                    Type = WebNotificationTypes.Version
                }
            });
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this.fileSystemWatcher?.Dispose();
                }

                disposedValue = true;
            }
        }

        private string GetVersionFromJsonFile()
        {
            string version = DefaultVersion;

            try
            {
                if (ContentDir.Exists)
                {
                    var versionFileInfo = ContentDir.GetFiles(VersionFileName, SearchOption.TopDirectoryOnly).FirstOrDefault();

                    if (versionFileInfo != null && versionFileInfo.Exists)
                    {
                        using FileStream file = new FileStream(versionFileInfo.FullName, FileMode.Open, FileAccess.Read, FileShare.Read);
                        using StreamReader reader = new StreamReader(file);
                        return reader.ReadToEnd();
                    }
                }

                return version;
            }
            catch (Exception)
            {
                return version;
            }
        }

        private FileSystemWatcher SafeTrackFile(string fullPath, string filename)
        {
            try
            {
                return TrackFile(fullPath, filename);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private FileSystemWatcher TrackFile(string fullPath, string filename)
        {
            FileSystemWatcher watcher = new FileSystemWatcher(fullPath);

            watcher.Filter = filename;
            watcher.NotifyFilter = NotifyFilters.Attributes
                                 | NotifyFilters.CreationTime
                                 | NotifyFilters.DirectoryName
                                 | NotifyFilters.FileName
                                 | NotifyFilters.LastAccess
                                 | NotifyFilters.LastWrite
                                 | NotifyFilters.Security
                                 | NotifyFilters.Size;

            watcher.Changed += OnFileChanged;
            watcher.Created += OnFileChanged;
            watcher.Deleted += OnFileChanged;
            watcher.Error += this.Watcher_Error;
            watcher.Renamed += this.Watcher_Renamed;

            return watcher;
        }

        private void Watcher_Renamed(object sender, RenamedEventArgs e)
        {

        }

        private void Watcher_Error(object sender, ErrorEventArgs e)
        {

        }

        private void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            switch (e.ChangeType)
            {
                case WatcherChangeTypes.Created:
                case WatcherChangeTypes.Deleted:
                case WatcherChangeTypes.Changed:
                case WatcherChangeTypes.Renamed:
                case WatcherChangeTypes.All:
                    {
                        try
                        {
                            this.NotifyVersionChange();
                        }
                        catch (Exception)
                        { }
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
