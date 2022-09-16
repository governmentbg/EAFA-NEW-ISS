using IARA.EntityModels.Entities;
using Microsoft.EntityFrameworkCore;

namespace IARA.DataAccess.Abstractions
{
    public interface IUsersDbContext : IBaseDbContext
    {
        DbSet<Npermission> Npermissions { get; }
        DbSet<NpermissionGroup> NpermissionGroups { get; }
        DbSet<NpermissionType> NpermissionTypes { get; }
        DbSet<Person> Persons { get; }
        DbSet<Role> Roles { get; }
        DbSet<RolePermission> RolePermissions { get; }
        DbSet<User> Users { get; }
        DbSet<UserInfo> UserInfos { get; }
        DbSet<UserLegal> UserLegals { get; }
        DbSet<NnotificationTemplate> NnotificationTemplates { get; }
    }
}
