﻿using IARA.Mobile.Domain.Models;
using IARA.Mobile.Pub.Application.Transactions;
using IARA.Mobile.Pub.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IARA.Mobile.Pub.Application.Interfaces.Transactions
{
    public interface IAuthenticationTransaction
    {
        Task<JwtToken> LogIn(AuthCredentials authCredentials);
        Task<bool> LogOut();
    }
}
