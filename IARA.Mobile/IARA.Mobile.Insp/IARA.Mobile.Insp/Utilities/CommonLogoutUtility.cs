using IARA.Mobile.Application.Interfaces.Utilities;
using IARA.Mobile.Insp.Application.Interfaces.Utilities;
using IARA.Mobile.Insp.Helpers;
using System;
using System.IO;
using Xamarin.Essentials;

namespace IARA.Mobile.Insp.Utilities
{
    public class CommonLogoutUtility : ICommonLogout
    {
        private readonly INomenclatureDatesClear _nomenclatureDates;
        private readonly IAuthTokenProvider _authTokenProvider;
        private readonly ICommonNavigator _commonNavigator;
        private readonly IAppDbMigration _appDbMigration;
        private readonly ICurrentUser _currentUser;
        private readonly IDbSettings _dbSettings;
        private readonly ISettings _settings;

        public CommonLogoutUtility(INomenclatureDatesClear nomenclatureDates, IAuthTokenProvider authTokenProvider, ICommonNavigator commonNavigator, IAppDbMigration appDbMigration, ICurrentUser currentUser, IDbSettings dbSettings, ISettings settings)
        {
            _nomenclatureDates = nomenclatureDates ?? throw new ArgumentNullException(nameof(nomenclatureDates));
            _authTokenProvider = authTokenProvider ?? throw new ArgumentNullException(nameof(authTokenProvider));
            _commonNavigator = commonNavigator ?? throw new ArgumentNullException(nameof(commonNavigator));
            _appDbMigration = appDbMigration ?? throw new ArgumentNullException(nameof(appDbMigration));
            _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
            _dbSettings = dbSettings ?? throw new ArgumentNullException(nameof(dbSettings));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        public void DeleteLocalInfo(bool changePage = true)
        {
            _authTokenProvider.Clear();
            SoftDeleteLocalInfo();

            if (changePage)
            {
                _commonNavigator.ToLogin();
            }
        }

        public void SoftDeleteLocalInfo()
        {
            _appDbMigration.DropDatabase();
            _nomenclatureDates.Clear();
            _currentUser.Clear();
            _dbSettings.Clear();
            _settings.Clear();

            foreach (string dir in Directory.GetDirectories(FileSystem.AppDataDirectory))
            {
                if (dir.StartsWith(InspectionFilesHelper.InspectionDirectory))
                {
                    Directory.Delete(dir, true);
                }
            }
        }
    }
}
