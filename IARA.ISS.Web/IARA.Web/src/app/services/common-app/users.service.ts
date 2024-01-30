import { HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { TokenStatus } from '@app/components/common-app/auth/enums/token-status.enum';
import { IGenericUserService } from '@app/components/common-app/auth/interfaces/user-service.interface';
import { UpdatePasswordModel } from '@app/components/common-app/auth/models/auth/change-password.model';
import { PasswordValidatorModel } from '@app/components/common-app/auth/models/auth/password-validator.model';
import { ChangeUserPasswordModel } from '@app/components/common-app/auth/models/auth/update-user-password.model';
import { TokenModel } from '@app/components/common-app/auth/models/token.model';
import { BaseUserService } from '@app/components/common-app/auth/services/base-user.service';
import { AccountActivationStatusesEnum } from '@app/enums/account-activation-statuses.enum';
import { ChangeUserDataDTO } from '@app/models/generated/dtos/ChangeUserDataDTO';
import { ForgotPasswordDTO } from '@app/models/generated/dtos/ForgotPasswordDTO';
import { UserAuthDTO } from '@app/models/generated/dtos/UserAuthDTO';
import { UserLoginDTO } from '@app/models/generated/dtos/UserLoginDTO';
import { UserRegistrationDTO } from '@app/models/generated/dtos/UserRegistrationDTO';
import { UserTokenDTO } from '@app/models/generated/dtos/UserTokenDTO';
import { RequestProperties } from '@app/shared/services/request-properties';
import { Observable } from 'rxjs';

@Injectable({
    providedIn: 'root'
})
export class UsersService extends BaseUserService<string, UserAuthDTO> implements IGenericUserService<string, UserAuthDTO> {

    public getUserData(): Observable<ChangeUserDataDTO> {
        return this.requestService.get(this.securityConfig.baseRoute, this.securityConfig.userController, 'GetAllUserData');
    }

    public changeUserPassword(data: ChangeUserPasswordModel): Observable<void> {
        return this.requestService.post(this.securityConfig.baseRoute, this.securityConfig.userController, 'ChangePassword', data);
    }

    public updateUserPassword(data: UpdatePasswordModel): Observable<void> {
        return this.requestService.post(this.securityConfig.baseRoute, this.securityConfig.userController, this.securityConfig.changePasswordMethodName, data);
    }

    //public changePassword(info: UserChangePasswordDTO): Observable<void> {
    //    return this.requestService.post(this.securityConfig.baseRoute, this.securityConfig.userController, 'ChangePassword', info);
    //}

    public getPasswordValidators(): Observable<PasswordValidatorModel> {
        return this.requestService.get(this.securityConfig.baseRoute, this.securityConfig.userController, this.securityConfig.passwordValidatorsMethodName);
    }

    public checkTokenStatus(token: string): Observable<TokenStatus> {
        const data: TokenModel = new TokenModel(token);

        return this.requestService.post<TokenStatus, TokenModel>(this.securityConfig.baseRoute, this.securityConfig.userController, this.securityConfig.checkEmailTokenMethodName, data);
    }

    public forgotPassword(email: string): Observable<void> {
        return this.requestService.post(this.securityConfig.baseRoute, this.securityConfig.userController, this.securityConfig.forgotPasswordMethodName, new ForgotPasswordDTO({
            email: email
        }));
    }

    public updateUserData(userData: ChangeUserDataDTO): Observable<void> {
        return this.requestService.post(this.securityConfig.baseRoute, this.securityConfig.userController, 'UpdateAllUserData', userData, {
            properties: new RequestProperties({ rethrowException: true, showException: true }),
            successMessage: 'succ-updated-user-profile'
        });
    }

    public registerUser(user: UserRegistrationDTO): Observable<number> {
        return this.requestService.post(this.securityConfig.baseRoute, this.securityConfig.userController, 'AddExternalUser', user, {
            properties: new RequestProperties({
                showException: true,
                rethrowException: true
            })
        });
    }

    public updateUserRegistration(user: UserRegistrationDTO): Observable<number> {
        return this.requestService.post(this.securityConfig.baseRoute, this.securityConfig.userController, 'UpdateUserRegistration', user, {
            properties: new RequestProperties({
                showException: true,
                rethrowException: true
            })
        });
    }

    public confirmEmailAndPassword(user: UserLoginDTO): Observable<boolean> {

        return this.requestService.post(this.securityConfig.baseRoute, this.securityConfig.userController, 'ConfirmEmailAndPassword', user, {
            properties: { showProgressSpinner: true, showException: true, rethrowException: true }
        });
    }

    public updateUserEAuthData(user: UserRegistrationDTO): Observable<number> {
        return this.requestService.put(this.securityConfig.baseRoute, this.securityConfig.userController, 'UpdateUserEAuthData', user);
    }

    public deactivateUserPasswordAccount(egnLnch: string): Observable<number> {
        const httpParams = new HttpParams().append('egnLnch', egnLnch);
        return this.requestService.post(this.securityConfig.baseRoute, this.securityConfig.userController, 'DeactivateUserPasswordAccount', null, { httpParams: httpParams });
    }

    public resendConfirmationEmailForToken(token: string): Observable<void> {
        const body = new UserTokenDTO({ token: token });
        return this.requestService.post(this.securityConfig.baseRoute, this.securityConfig.userController, 'ResendConfirmationEmailForToken', body);
    }

    public activateUserAccount(token: string): Observable<AccountActivationStatusesEnum> {
        const body: UserTokenDTO = new UserTokenDTO({ token: token });
        return this.requestService.post(this.securityConfig.baseRoute, this.securityConfig.userController, 'ActivateUserAccount', body);
    }

    public resendConfirmationEmail(email: string): Observable<void> {
        const httpParams = new HttpParams().append('email', email);
        return this.requestService.post(this.securityConfig.baseRoute, this.securityConfig.userController, 'ResendConfirmationEmail', null, { httpParams: httpParams });
    }
}