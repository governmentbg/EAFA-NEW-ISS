﻿using System.Threading.Tasks;
using IARA.Mobile.Application.DTObjects.Profile.API;

namespace IARA.Mobile.Insp.Application.Interfaces.Transactions
{
    public interface IProfileTransaction
    {
        Task<ProfileApiDto> GetInfo();

        Task<bool> SaveInfo(EditProfileApiDto profile);

        Task<byte[]> GetPhoto();

        bool HasPermission(string permission);
    }
}
