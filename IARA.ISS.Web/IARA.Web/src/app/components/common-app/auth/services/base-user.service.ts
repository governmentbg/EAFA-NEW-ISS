import { Inject, Injectable } from '@angular/core';
import { REQUEST_SERVICE_TOKEN } from '@app/shared/di/shared-di.tokens';
import { IRequestService } from '@app/shared/interfaces/request-service.interface';
import { Observable, of, Subject } from 'rxjs';
import { PERMISSIONS_SERVICE_TOKEN, SECURITY_CONFIG_TOKEN } from '../di/auth-di.tokens';
import { TokenStatus } from '../enums/token-status.enum';
import { IPermissionsService } from '../interfaces/permissions-service.interface';
import { SecurityConfig } from '../interfaces/security-config.interface';
import { IGenericUserService } from '../interfaces/user-service.interface';
import { UpdatePasswordModel } from '../models/auth/change-password.model';
import { PasswordValidatorModel } from '../models/auth/password-validator.model';
import { ChangeUserPasswordModel } from '../models/auth/update-user-password.model';
import { User } from '../models/auth/user.model';

@Injectable({
    providedIn: 'root'
})
export abstract class BaseUserService<TIdentifier, TUser extends User<TIdentifier>> implements IGenericUserService<TIdentifier, TUser> {

    protected readonly securityConfig: SecurityConfig;
    protected readonly requestService: IRequestService;
    protected readonly permissionsService: IPermissionsService;
    private _user?: TUser;

    public constructor(@Inject(SECURITY_CONFIG_TOKEN) securityConfig: SecurityConfig,
        @Inject(REQUEST_SERVICE_TOKEN) requestService: IRequestService,
        @Inject(PERMISSIONS_SERVICE_TOKEN) permissionsService: IPermissionsService) {
        this.securityConfig = securityConfig;
        this.requestService = requestService;
        this.permissionsService = permissionsService;
    }

    public get User(): TUser {
        return this._user!;
    }


    public abstract updateUserPassword(data: UpdatePasswordModel): Observable<void>;

    public abstract getPasswordValidators(): Observable<PasswordValidatorModel>;

    public abstract changeUserPassword(data: ChangeUserPasswordModel): Observable<void>;

    public abstract checkTokenStatus(token: string): Observable<TokenStatus>;

    public abstract forgotPassword(email: string): Observable<void>;

    public getUser(): Observable<TUser> {
       
        const subject: Subject<TUser> = new Subject();
        if (this._user == undefined) {
            this.requestService.get<TUser>(this.securityConfig.baseRoute, this.securityConfig.securityController, this.securityConfig.userMethodName).subscribe(user => {
                if (user != undefined && user.permissions != undefined && user.permissions.length > 0) {
                    this.permissionsService.loadPermissions(user.permissions);
                } else {
                    this.permissionsService.loadPermissions([]);
                }

                this._user = user;
                subject.next(user);
                subject.complete();
            });
        } else {
            return of(this._user);
        }

        return subject;
    }
}