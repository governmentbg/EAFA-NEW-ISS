﻿using System.Threading.Tasks;
using IARA.Mobile.Application.DTObjects.Profile.API;
using IARA.Mobile.Application.DTObjects.Users;
using IARA.Mobile.Application.Extensions;
using IARA.Mobile.Application.Interfaces.Transactions;
using IARA.Mobile.Domain.Models;
using IARA.Mobile.Pub.Application.Interfaces.Transactions;
using IARA.Mobile.Pub.Application.Transactions.Base;

namespace IARA.Mobile.Pub.Application.Transactions
{
    public class ProfileTransaction : BaseTransaction, IProfileTransaction, IPasswordTransaction
    {
        public ProfileTransaction(BaseTransactionProvider provider) : base(provider)
        {
        }

        public async Task<ProfileApiDto> GetInfo()
        {
            HttpResult<ProfileApiDto> result = await RestClient.GetAsync<ProfileApiDto>("Profile/Get");

            if (result.IsSuccessful && result.Content != null)
            {
                return result.Content;
            }

            return null;
        }

        public Task<bool> SaveInfo(EditProfileApiDto profile)
        {
            return RestClient.PostAsFormDataAsync("Profile/Edit", profile)
                .ContinueWith(task => task.Result.IsSuccessful);
        }

        public async Task<byte[]> GetPhoto()
        {
            HttpResult<byte[]> result = await RestClient.GetAsync<byte[]>("Profile/GetUserPhoto");

            if (result.IsSuccessful && result.Content != null)
            {
                return result.Content;
            }

            return null;
        }

        public async Task<FileResponse> GetUserPhoto()
        {
            HttpResult<FileResponse> result = await RestClient.GetAsync<FileResponse>("Profile/GetUserPhoto");

            if (result.IsSuccessful && result.Content != null)
            {
                return result.Content;
            }

            return null;
        }

        public Task<bool> ChangePassword(UserPasswordDto userPassword)
        {
            return RestClient.PostAsync("Profile/ChangeUserPassword", userPassword)
                .IsSuccessfulResult();
        }
    }
}
