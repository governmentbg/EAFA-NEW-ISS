using System;
using System.Collections.Generic;
using IARA.Common.Constants;
using IARA.Common.Enums;
using IARA.Common.Utils;
using IARA.EntityModels.Entities;

namespace IARA.Fakes.MockupData
{
    public class UserManagementData
    {
        public static readonly string IivanovPasswordHash = CommonUtils.GetPasswordHash("iivanov", "iivanov@technologica.com");

        public static List<User> Users
        {
            get
            {
                return new List<User>
                {
                    new User
                    {
                        Id = 1,
                        Username = "ppetrov",
                        IsInternalUser = true,
                        ValidFrom = DateTime.Now.AddYears(-20),
                        ValidTo = DateTime.MaxValue,
                        HasEauthLogin = false,
                        HasUserPassLogin = false,
                        PersonId = PersonsData.Persons[5].Id
                    },
                    new User
                    {
                        Id = 2,
                        Username = "kqneva",
                        IsInternalUser = true,
                        ValidFrom = DateTime.Now.AddYears(-20),
                        ValidTo = new DateTime(2021, 4, 1),
                        HasEauthLogin = false,
                        HasUserPassLogin = false,
                        PersonId = PersonsData.Persons[6].Id
                    },
                    new User
                    {
                        Id = 3,
                        Username = "ppetrova",
                        IsInternalUser = true,
                        ValidFrom = DateTime.Now.AddYears(-20),
                        ValidTo = DateTime.MaxValue,
                        HasEauthLogin = false,
                        HasUserPassLogin = false,
                        PersonId = PersonsData.Persons[7].Id
                    },
                    new User
                    {
                        Id = 4,
                        Username = "zstambolova",
                        Email = "z_stambolova@gmail.com",
                        IsInternalUser = true,
                        ValidFrom = DateTime.Now.AddYears(-20),
                        ValidTo = DateTime.MaxValue,
                        HasEauthLogin = false,
                        HasUserPassLogin = false,
                        PersonId = PersonsData.Persons[8].Id
                    },
                    new User
                    {
                        Id = 5,
                        Username = "igeorgiev",
                        IsInternalUser = false,
                        ValidFrom = DateTime.Now.AddYears(-20),
                        ValidTo = DateTime.MaxValue,
                        HasEauthLogin = false,
                        HasUserPassLogin = false,
                        PersonId = PersonsData.Persons[9].Id
                    },
                    new User
                    {
                        Id = 6,
                        Username = "akirilоv",
                        Email = "akirilov@gmail.com",
                        IsInternalUser = false,
                        ValidFrom = DateTime.Now.AddYears(-20),
                        ValidTo = DateTime.MaxValue,
                        HasEauthLogin = false,
                        HasUserPassLogin = false,
                        PersonId = PersonsData.Persons[10].Id
                    },
                    new User
                    {
                        Id = 7,
                        Username = PersonsData.EmailAddresses[5].Email,
                        Email = PersonsData.EmailAddresses[5].Email,
                        IsInternalUser = false,
                        ValidFrom = DateTime.Now.AddYears(-20),
                        ValidTo = DefaultConstants.MAX_VALID_DATE,
                        HasEauthLogin = false,
                        HasUserPassLogin = true,
                        PersonId = PersonsData.Persons[11].Id,
                        Password = IivanovPasswordHash
                    }
                };
            }
        }

        public static List<UserRole> UserRoles
        {
            get
            {
                return new List<UserRole>
                {
                    new UserRole
                    {
                        Id = 1,
                        UserId = Users[0].Id,
                        RoleId = RolesData.Roles[0].Id,
                        IsActive = true,
                        AccessValidFrom = new DateTime(2020, 1, 1),
                        AccessValidTo = DefaultConstants.MAX_VALID_DATE
                    },
                    new UserRole
                    {
                        Id = 2,
                        UserId = Users[1].Id,
                        RoleId = RolesData.Roles[1].Id,
                        IsActive = true
                    },
                    new UserRole
                    {
                        Id = 3,
                        UserId = Users[2].Id,
                        RoleId = RolesData.Roles[1].Id,
                        IsActive = true
                    },
                    new UserRole
                    {
                        Id = 4,
                        UserId = Users[2].Id,
                        RoleId = RolesData.Roles[0].Id,
                        IsActive = true,
                        AccessValidFrom = new DateTime(2020, 1, 1),
                        AccessValidTo = DefaultConstants.MAX_VALID_DATE
                    },
                    new UserRole
                    {
                        Id = 5,
                        UserId = Users[3].Id,
                        RoleId = RolesData.Roles[1].Id,
                        IsActive = true
                    },
                    new UserRole
                    {
                        Id = 6,
                        UserId = Users[4].Id,
                        RoleId = RolesData.Roles[0].Id,
                        IsActive = true,
                        AccessValidFrom = new DateTime(2020, 1, 1),
                        AccessValidTo = DefaultConstants.MAX_VALID_DATE
                    },
                    new UserRole
                    {
                        Id = 7,
                        UserId = Users[5].Id,
                        RoleId = RolesData.Roles[0].Id,
                        IsActive = true,
                        AccessValidFrom = new DateTime(2020, 1, 1),
                        AccessValidTo = DefaultConstants.MAX_VALID_DATE
                    },
                    new UserRole
                    {
                        Id = 8,
                        UserId = Users[5].Id,
                        RoleId = RolesData.Roles[1].Id,
                        IsActive = true
                    },
                    new UserRole
                    {
                        Id = 9,
                        UserId = Users[6].Id,
                        RoleId = RolesData.Roles[3].Id,
                        AccessValidFrom = new DateTime(2021, 5, 1, 12, 0, 0),
                        AccessValidTo = DefaultConstants.MAX_VALID_DATE,
                        IsActive = true
                    }
                };
            }
        }

        public static List<UserMobileDevice> UserMobileDevices
        {
            get
            {
                return new List<UserMobileDevice>
                {
                    new UserMobileDevice
                    {
                        Id = 1,
                        UserId = Users[0].Id,
                        Imei = "1f8a5rs5982a",
                        RequestAccessDate = new DateTime(2020, 09, 01),
                        AccessStatus = UserMobileDeviceAccessStatusEnum.Requested.ToString(),
                        IsActive = true
                    },
                    new UserMobileDevice
                    {
                        Id = 2,
                        UserId = Users[1].Id,
                        Imei = "2f8a5rs5982b",
                        RequestAccessDate = new DateTime(2020, 08, 01),
                        AccessStatus = UserMobileDeviceAccessStatusEnum.Approved.ToString(),
                        IsActive = true
                    },
                    new UserMobileDevice
                    {
                        Id = 3,
                        UserId = Users[0].Id,
                        Imei = "bf8a5rs5982a",
                        RequestAccessDate = new DateTime(2012, 08, 01),
                        AccessStatus = UserMobileDeviceAccessStatusEnum.Blocked.ToString(),
                        IsActive = true
                    },
                    new UserMobileDevice
                    {
                        Id = 4,
                        UserId = Users[2].Id,
                        Imei = "hf8a5rs5982q",
                        RequestAccessDate = new DateTime(2021, 02, 01),
                        AccessStatus = UserMobileDeviceAccessStatusEnum.Requested.ToString(),
                        IsActive = true
                    },
                    new UserMobileDevice
                    {
                        Id = 5,
                        UserId = Users[2].Id,
                        Imei = "6f8a5rs5982a",
                        RequestAccessDate = new DateTime(2020, 08, 01),
                        AccessStatus = UserMobileDeviceAccessStatusEnum.Requested.ToString(),
                        IsActive = true
                    },
                    new UserMobileDevice
                    {
                        Id = 6,
                        UserId = Users[3].Id,
                        Imei = "89f8a5rs5982a",
                        RequestAccessDate = new DateTime(2021, 03, 01),
                        AccessStatus = UserMobileDeviceAccessStatusEnum.Approved.ToString(),
                        IsActive = true
                    },
                    new UserMobileDevice
                    {
                        Id = 7,
                        UserId = Users[3].Id,
                        Imei = "0f8a5rs5982a",
                        RequestAccessDate = new DateTime(2020, 08, 01),
                        AccessStatus = UserMobileDeviceAccessStatusEnum.Blocked.ToString(),
                        IsActive = true
                    },
                    new UserMobileDevice
                    {
                        Id = 8,
                        UserId = Users[2].Id,
                        Imei = "18f8a5rs5982a",
                        RequestAccessDate = new DateTime(2018, 08, 01),
                        AccessStatus = UserMobileDeviceAccessStatusEnum.Blocked.ToString(),
                        IsActive = true
                    },
                    new UserMobileDevice
                    {
                        Id = 9,
                        UserId = Users[1].Id,
                        Imei = "175f8a5rs5982a",
                        RequestAccessDate = new DateTime(2015, 08, 01),
                        AccessStatus = UserMobileDeviceAccessStatusEnum.Blocked.ToString(),
                        IsActive = true
                    },
                    new UserMobileDevice
                    {
                        Id = 10,
                        UserId = Users[0].Id,
                        Imei = "19af8a5rs5982a",
                        RequestAccessDate = new DateTime(2021, 04, 01),
                        AccessStatus = UserMobileDeviceAccessStatusEnum.Approved.ToString(),
                        IsActive = true
                    }
                };
            }
        }

        public static List<UserLegal> UserLegals
        {
            get
            {
                return new List<UserLegal>
                {
                    new UserLegal {
                        UserId = 5,
                        LegalId = LegalsData.Legals[0].Id,
                        IsActive = true,
                        Status = UserLegalStatusEnum.Approved.ToString(),
                        RoleId = RolesData.Roles[2].Id
                    },
                    new UserLegal {
                        UserId = 5,
                        LegalId = LegalsData.Legals[1].Id,
                        IsActive = true,
                        Status = UserLegalStatusEnum.Blocked.ToString(),
                        RoleId = RolesData.Roles[2].Id
                    },
                    new UserLegal {
                        UserId = 6,
                        LegalId = LegalsData.Legals[2].Id,
                        IsActive = true,
                        Status = UserLegalStatusEnum.Approved.ToString(),
                        RoleId = RolesData.Roles[2].Id
                    },
                    new UserLegal {
                        UserId = 6,
                        LegalId = LegalsData.Legals[3].Id,
                        IsActive = true,
                        Status = UserLegalStatusEnum.Requested.ToString(),
                        RoleId = RolesData.Roles[2].Id
                    }
                };
            }
        }

        public static List<UserInfo> UserInfos
        {
            get
            {
                return new List<UserInfo>
                {
                    new UserInfo
                    {
                        UserId = 1,
                        RegistrationDate = new DateTime(2020, 4, 4),
                        IsLocked = false,
                        FailedLoginCount = 0,
                        UserMustChangePassword = false,
                        IsEmailConfirmed = true,
                        SectorId = NSectorsData.Sectors[0].Id,
                        DepartmentId = NDepartmentsData.Departments[0].Id,
                        TerritoryUnitId = NTerritoryUnitsData.TerritoryUnits[1].Id
                    },
                    new UserInfo
                    {
                        UserId = 2,
                        RegistrationDate = new DateTime(2020, 4, 4),
                        IsLocked = false,
                        FailedLoginCount = 0,
                        UserMustChangePassword = false,
                        IsEmailConfirmed = true,
                        SectorId = NSectorsData.Sectors[3].Id,
                        DepartmentId = NDepartmentsData.Departments[3].Id,
                        TerritoryUnitId = NTerritoryUnitsData.TerritoryUnits[2].Id
                    },
                    new UserInfo
                    {
                        UserId = 3,
                        RegistrationDate = new DateTime(2020, 4, 4),
                        IsLocked = false,
                        FailedLoginCount = 0,
                        UserMustChangePassword = false,
                        IsEmailConfirmed = true,
                        SectorId = NSectorsData.Sectors[6].Id,
                        DepartmentId = NDepartmentsData.Departments[1].Id,
                        TerritoryUnitId = NTerritoryUnitsData.TerritoryUnits[1].Id
                    },
                    new UserInfo
                    {
                        UserId = 4,
                        RegistrationDate = new DateTime(2020, 4, 4),
                        IsLocked = false,
                        FailedLoginCount = 0,
                        UserMustChangePassword = false,
                        IsEmailConfirmed = true,
                        SectorId = NSectorsData.Sectors[5].Id,
                        DepartmentId = NDepartmentsData.Departments[2].Id,
                        TerritoryUnitId = NTerritoryUnitsData.TerritoryUnits[0].Id
                    },
                    new UserInfo
                    {
                        UserId = 5,
                        RegistrationDate = new DateTime(2020, 4, 4),
                        IsLocked = false,
                        FailedLoginCount = 0,
                        UserMustChangePassword = false,
                        IsEmailConfirmed = true,
                        SectorId = NSectorsData.Sectors[2].Id,
                        DepartmentId = NDepartmentsData.Departments[1].Id,
                        TerritoryUnitId = NTerritoryUnitsData.TerritoryUnits[3].Id
                    },
                    new UserInfo
                    {
                        UserId = 7,
                        RegistrationDate = new DateTime(2021, 1, 1, 5, 0, 0, 0),
                        IsLocked = false,
                        FailedLoginCount = 0,
                        UserMustChangePassword = false,
                        IsEmailConfirmed = false
                    }
                };
            }
        }
    }
}
