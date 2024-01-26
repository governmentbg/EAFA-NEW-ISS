import { HttpErrorResponse, HttpHeaders, HttpParams } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { REQUEST_SERVICE_TOKEN } from '@app/shared/di/shared-di.tokens';
import { HttpStatusCode } from '@app/shared/enums/http-status-codes.enum';
import { StorageTypes } from '@app/shared/enums/storage-types.enum';
import { IRequestService } from '@app/shared/interfaces/request-service.interface';
import { StorageService } from '@app/shared/services/local-storage.service';
import { RequestProperties } from '@app/shared/services/request-properties';
import { IRequestServiceParams } from '@app/shared/services/request-service-params.interface';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { BehaviorSubject, Observable, Subject } from 'rxjs';
import { PERMISSIONS_SERVICE_TOKEN, SECURITY_CONFIG_TOKEN, USER_SERVICE_TOKEN } from '../di/auth-di.tokens';
import { LoginResultTypes } from '../enums/login-result-types.enum';
import { IPermissionsService } from '../interfaces/permissions-service.interface';
import { SecurityConfig } from '../interfaces/security-config.interface';
import { IGenericSecurityService } from '../interfaces/security-service.interface';
import { IGenericUserService } from '../interfaces/user-service.interface';
import { AuthCredentials } from '../models/auth/auth-credentials.model';
import { JwtToken } from '../models/auth/jwt-token.model';
import { LoginResult } from '../models/auth/login-result.model';
import { User } from '../models/auth/user.model';

@Injectable({
    providedIn: 'root'
})
export abstract class BaseSecurityService<TIdentifier, TUser extends User<TIdentifier>> implements IGenericSecurityService<TIdentifier, TUser> {
    private static readonly TOKEN_NAME = 'token';

    protected readonly requestService: IRequestService;
    protected readonly router: Router;
    private _token?: JwtToken;
    private _impersonationToken: string | undefined;
    protected storage?: StorageService;

    private _isAuthenticatedEvent: BehaviorSubject<boolean>;
    private _user: User<TIdentifier> | undefined;
    private _persistToken: boolean = false;
    private matDialog: MatDialog;
    private securityConfig: SecurityConfig;
    private permissionsService: IPermissionsService;
    private usersService: IGenericUserService<TIdentifier, TUser>;

    public get isAuthenticatedEvent(): Observable<boolean> {
        return this._isAuthenticatedEvent;
    }

    public get User(): TUser {
        return this.usersService.User;
    }

    public get impersonationToken(): string | undefined {
        return this._impersonationToken;
    }

    public set impersonationToken(value: string | undefined) {
        this._impersonationToken = value;
    }

    public constructor(@Inject(REQUEST_SERVICE_TOKEN) requestService: IRequestService,
        @Inject(SECURITY_CONFIG_TOKEN) securityConfig: SecurityConfig,
        @Inject(PERMISSIONS_SERVICE_TOKEN) permissionsService: IPermissionsService,
        @Inject(USER_SERVICE_TOKEN) usersService: IGenericUserService<TIdentifier, TUser>,
        router: Router,
        matDialog: MatDialog) {

        this.usersService = usersService;
        this.permissionsService = permissionsService;
        this.securityConfig = securityConfig;
        this.router = router;
        this.matDialog = matDialog;
        this.requestService = requestService;
        this._isAuthenticatedEvent = new BehaviorSubject<boolean>(false);

        this.requestService.errorEvent.subscribe(error => {
            const navigateToSignIn = `${this.securityConfig.authModulePath}/sign-in`;
            if (error.status == HttpStatusCode.Unauthorized) {
                this.clearToken();
                this.router.navigate([navigateToSignIn]);
            } else if (error.status == HttpStatusCode.NetworkAuthenticationRequired) {
                this.clearToken();
                this.router.navigate([navigateToSignIn]);
            }
        });

        this.requestService.newTokenSent.subscribe(async (newToken) => {
            await this.readTokenMeta(newToken);
        });
    }

    public getUser(): Observable<TUser> {
        return this.usersService.getUser();
    }

    public abstract getUserRedirectPath(): Promise<string>;

    public logout(): Observable<void> {
        const mutex: Subject<void> = new Subject();

        this.requestService.post(this.securityConfig.baseRoute, this.securityConfig.securityController, this.securityConfig.logoutMethodName).subscribe({
            next: () => {
                this.localLogout(mutex);
            },
            error: (error: HttpErrorResponse) => {
                this.localLogout(mutex);
            }
        });

        return mutex;
    }

    private localLogout(mutex: Subject<void>) {
        this.clearToken();
        mutex.next();
        mutex.complete();
    }

    public clearToken(): Promise<boolean> {

        this._token = undefined;
        this._user = undefined;

        if (this.storage != undefined) {
            const persistToken = CommonUtils.toBoolean(this.storage!.get('persistToken') as string);

            if (persistToken) {
                return this.storage!.removeItem(BaseSecurityService.TOKEN_NAME).then(result => {
                    this._isAuthenticatedEvent.next(false);
                    return result;
                });
            } else {
                return this.storage!.removeItem(BaseSecurityService.TOKEN_NAME).then(result => {
                    this._isAuthenticatedEvent.next(false);
                    return result;
                });
            }
        } else {

            this.storage = this.getStorageOrDefault();

            if (this.storage != undefined) {
                return this.storage.removeItem(BaseSecurityService.TOKEN_NAME).then(result => {
                    this._isAuthenticatedEvent.next(false);
                    return result;
                });
            } else {
                return Promise.resolve(false);
            }
        }
    }

    public get token(): string | undefined {
        if (this._token == undefined) {
            this._token = this.getTokenFromStorage();
        }

        if (this._token != undefined) {
            if (this._token.validTo > new Date()) {
                return this._token.token;
            } else {
                this.clearToken();
            }
        }

        return undefined;
    }

    public get isTempToken(): boolean {
        return this._token?.isTempToken ?? false;
    }

    public login(credentials: AuthCredentials): Promise<LoginResult | undefined> {
        const subject: Subject<LoginResult | undefined> = new Subject();
        this._persistToken = credentials.rememberMe;
        const properties = this.loginRequestProperties;
        //properties.withCredentials = true;

        this.requestService.post<JwtToken, AuthCredentials>(this.securityConfig.baseRoute, this.securityConfig.securityController, this.securityConfig.loginMethodName, credentials, properties).subscribe({
            next: (token) => {
                this.tokenSuccessHandler(token, credentials.rememberMe, subject, true);
            },
            error: (error: HttpErrorResponse) => {
                this.tokenErrorHandler(error, subject);
            }
        });

        return subject.toPromise();
    }


    public readTokenMeta(jwtToken: string, shouldEmit?: boolean): Promise<LoginResult | undefined> {

        let headers: HttpHeaders = new HttpHeaders();
        const subject: Subject<LoginResult | undefined> = new Subject();

        headers = headers.append('Authorization', `Bearer ${jwtToken}`);
        headers = headers.append('X-Skip-Reissue', 'true');

        const properties = this.loginRequestProperties;

        properties.headers = headers;

        this.requestService.get<JwtToken>(this.securityConfig.baseRoute, this.securityConfig.securityController, this.securityConfig.readTokenDataMethodName, properties).subscribe(token => {
            this.tokenSuccessHandler(token, false, subject, shouldEmit);
        }, (error: HttpErrorResponse) => {
            this.tokenErrorHandler(error, subject);
        });

        return subject.toPromise();
    }

    private getStorageOrDefault() {
        let token = StorageService.getStorage(StorageTypes.Session).get(BaseSecurityService.TOKEN_NAME) as string;

        if (token != undefined) {
            return StorageService.getStorage(StorageTypes.Session);
        } else {
            token = StorageService.getStorage(StorageTypes.Local).get(BaseSecurityService.TOKEN_NAME) as string;

            if (token != undefined) {
                return StorageService.getStorage(StorageTypes.Local);
            } else {
                return StorageService.getStorage(StorageTypes.Session);
            }
        }
    }

    private tokenSuccessHandler(token: JwtToken, rememberMe: boolean, subject?: Subject<LoginResult | undefined>, shouldEmit?: boolean) {
        this._token = token;

        let result: LoginResult;

        if (this.token != undefined) {

            result = new LoginResult(LoginResultTypes.Success);
            result.isTempToken = token.isTempToken;

            if (!token.isTempToken) {
                this.saveToken(rememberMe, token);

                if (shouldEmit != null && shouldEmit == true) {
                    this._isAuthenticatedEvent.next(true);
                }
            }

        } else {
            result = new LoginResult(LoginResultTypes.Fail);
        }

        if (subject != undefined) {
            subject.next(result);
            subject.complete();
        }
    }

    private tokenErrorHandler(error: HttpErrorResponse, subject?: Subject<LoginResult | undefined>) {
        let result: LoginResult | undefined = new LoginResult(LoginResultTypes.Fail);
        if (error.status == 0 || error.status == HttpStatusCode.InternalServerError) {
            result = undefined;
        } else if (error.status == HttpStatusCode.UnprocessableEntity) {
            Object.assign(result, error.error);
        }

        this._isAuthenticatedEvent.next(false);

        if (subject != undefined) {
            subject.next(result);
            subject.complete();
        }
    }

    private saveToken(shouldPersist: boolean, token: JwtToken) {

        if (shouldPersist) {
            this.storage = StorageService.getStorage(StorageTypes.Local);

        } else {
            this.storage = StorageService.getStorage(StorageTypes.Session);
        }

        this.storage!.addOrUpdate(BaseSecurityService.TOKEN_NAME, JSON.stringify(token));
    }


    private getTokenFromStorage(): JwtToken | undefined {

        let token: string | undefined = undefined;

        if (this.storage == undefined) {
            token = StorageService.getStorage(StorageTypes.Session).get(BaseSecurityService.TOKEN_NAME) as string;

            if (token != undefined) {
                this.storage = StorageService.getStorage(StorageTypes.Session);
            } else {
                token = StorageService.getStorage(StorageTypes.Local).get(BaseSecurityService.TOKEN_NAME) as string;

                if (token != undefined) {
                    this.storage = StorageService.getStorage(StorageTypes.Local);
                }
            }
        } else {
            token = this.storage.get(BaseSecurityService.TOKEN_NAME) as string;
        }

        return this.buildJwtToken(token);
    }

    private buildJwtToken(token: string | undefined): JwtToken | undefined {
        if (token != undefined) {

            const tempToken = JSON.parse(token) as { Token: string, TokenId: string, ValidTo: string; };

            const jwtToken = new JwtToken();
            jwtToken.token = tempToken.Token;
            jwtToken.tokenId = tempToken.TokenId;
            jwtToken.validTo = new Date(tempToken.ValidTo);
            return jwtToken;
        }

        return undefined;
    }

    public get loginRequestProperties(): IRequestServiceParams {
        const httpParams: HttpParams = new HttpParams();
        const requestProperties = RequestProperties.DEFAULT;
        requestProperties.rethrowException = true;
        requestProperties.showException = false;

        const requestParams = { httpParams: httpParams, properties: requestProperties, responseTypeCtr: JwtToken } as IRequestServiceParams;

        return requestParams;
    }

    public authorize(): void {
        this.matDialog.closeAll();
    }

    public isAuthenticated(): Promise<boolean> {
        if (this.token != undefined && !this.isTempToken) {
            if (!this._isAuthenticatedEvent.value) {
                this._isAuthenticatedEvent.next(true);
            }
        } else {
            if (this._isAuthenticatedEvent.value) {
                this._isAuthenticatedEvent.next(false);
            }
        }

        return Promise.resolve(this._isAuthenticatedEvent.value);
    }

}