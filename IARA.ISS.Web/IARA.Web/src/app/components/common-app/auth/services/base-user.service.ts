import { Inject, Injectable } from '@angular/core';
import { REQUEST_SERVICE_TOKEN } from '@app/shared/di/shared-di.tokens';
import { IRequestService } from '@app/shared/interfaces/request-service.interface';
import { BehaviorSubject, Observable, of, Subject } from 'rxjs';
import { map, mergeMap } from 'rxjs/operators';
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

    private _userBehaviourSubject: BehaviorSubject<TUser | undefined>;
    private _userSubject: Subject<TUser>;
    private isEntered: boolean;

    public constructor(@Inject(SECURITY_CONFIG_TOKEN) securityConfig: SecurityConfig,
        @Inject(REQUEST_SERVICE_TOKEN) requestService: IRequestService,
        @Inject(PERMISSIONS_SERVICE_TOKEN) permissionsService: IPermissionsService) {
        this.securityConfig = securityConfig;
        this.requestService = requestService;
        this.isEntered = false;
        this.permissionsService = permissionsService;
        this._userBehaviourSubject = new BehaviorSubject<TUser | undefined>(undefined);
        this._userSubject = new Subject<TUser>();
        this._userBehaviourSubject.subscribe(user => {
            this._userSubject.next(user);
        });
    }

    public get UserReceived(): Observable<TUser> {
        return this.pipeGetUser(this._userSubject);
    }

    public get User(): TUser | undefined {
        return this._userBehaviourSubject.value;
    }

    public set User(value: TUser | undefined) {
        this._userBehaviourSubject.next(value);
    }

    public abstract updateUserPassword(data: UpdatePasswordModel): Observable<void>;

    public abstract getPasswordValidators(): Observable<PasswordValidatorModel>;

    public abstract changeUserPassword(data: ChangeUserPasswordModel): Observable<void>;

    public abstract checkTokenStatus(token: string): Observable<TokenStatus>;

    public abstract forgotPassword(email: string): Observable<void>;

    public getUser(): Observable<TUser> {

        if (this._userBehaviourSubject.value == undefined) {
            if (!this.isEntered) {
                return this.pipeGetUser(this.getUserObject());
            } else {
                const subject = new Subject<TUser>();

                this._userSubject.subscribe(user => {
                    subject.next(user);
                    subject.complete();
                })

                return subject;
            }
        } else {
            return of(this._userBehaviourSubject.value);
        }
    }

    private getUserObject(): Observable<TUser> {
        const subject = new Subject<TUser>();
        const config = this.securityConfig;
        this.requestService.get<TUser>(config.baseRoute, config.securityController, config.userMethodName).subscribe(user => {
            this._userBehaviourSubject.next(user);
            if (user != undefined && user.permissions != undefined && user.permissions.length > 0) {
                this.permissionsService.loadPermissions(user.permissions);
            } else {
                this.permissionsService.loadPermissions([]);
            }

            subject!.next(user);
            subject!.complete();
        });

        return subject;
    }

    private pipeGetUser(userObservable: Observable<TUser>): Observable<TUser> {
        this.isEntered = true;
        return userObservable.pipe(map((user: TUser) => {
            this.isEntered = false;
            return user;
        }));
    }
}
