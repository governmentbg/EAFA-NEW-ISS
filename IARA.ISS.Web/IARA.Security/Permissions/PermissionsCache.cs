using System.Collections.Generic;
using System.Threading;
using IARA.Common;

namespace IARA.Security
{
    public sealed class PermissionsCache
    {
        private Dictionary<string, int> permissionsByCode;
        private Dictionary<int, string> permissionsById;
        private ManualResetEvent waitHandle = new ManualResetEvent(true);
        private ScopedServiceProviderFactory scopedServiceProviderFactory;

        public static PermissionsCache Permissions { get; private set; }

        public PermissionsCache(ScopedServiceProviderFactory scopedServiceProviderFactory)
        {
            Permissions = this;
            this.scopedServiceProviderFactory = scopedServiceProviderFactory;
            permissionsByCode = new Dictionary<string, int>();
            permissionsById = new Dictionary<int, string>();
            RefreshPermissions();
        }

        public void RefreshPermissions()
        {
            if (waitHandle.WaitOne())
            {
                waitHandle.Reset();
                permissionsByCode.Clear();
                permissionsById.Clear();

                try
                {
                    using var serviceProvider = scopedServiceProviderFactory.GetServiceProvider();
                    foreach (var permission in serviceProvider.GetService<IPermissionsService>().GetAllPermissions())
                    {
                        permissionsByCode.Add(permission.Name, permission.ID);
                        permissionsById.Add(permission.ID, permission.Name);
                    }
                }
                finally
                {
                    waitHandle.Set();
                }
            }
        }

        public int GetPermissionIdByCode(string permissionCode)
        {
            return this[permissionCode];
        }

        public string GetPermissionCodeById(int permissionId)
        {
            return this[permissionId];
        }

        public int this[string permissionCode]
        {
            get
            {
                waitHandle.WaitOne();
                return permissionsByCode[permissionCode];
            }
        }

        public string this[int permissionId]
        {
            get
            {
                waitHandle.WaitOne();
                return permissionsById[permissionId];
            }
        }
    }
}
