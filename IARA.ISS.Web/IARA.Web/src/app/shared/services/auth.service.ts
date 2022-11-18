import { HttpParams } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { MatSnackBar } from '@angular/material/snack-bar';
import { Router } from "@angular/router";
import { ChangePasswordComponent } from '@app/components/common-app/my-profile/components/change-password/change-password.component';
import { ChangeUserDataComponent } from '@app/components/common-app/my-profile/components/change-userdata/change-userdata.component';
import { AccountActivationStatusesEnum } from "@app/enums/account-activation-statuses.enum";
import { LoginTypesEnum } from "@app/enums/login-types.enum";
import { DialogParamsModel } from '@app/models/common/dialog-params.model';
import { ChangeUserDataDTO } from '@app/models/generated/dtos/ChangeUserDataDTO';
import { UserAuthDTO } from "@app/models/generated/dtos/UserAuthDTO";
import { UserChangePasswordDTO } from "@app/models/generated/dtos/UserChangePasswordDTO";
import { UserLoginDTO } from "@app/models/generated/dtos/UserLoginDTO";
import { UserPasswordDTO } from '@app/models/generated/dtos/UserPasswordDTO';
import { UserRegistrationDTO } from "@app/models/generated/dtos/UserRegistrationDTO";
import { UserTokenDTO } from "@app/models/generated/dtos/UserTokenDTO";
import { MyProfilePublicService } from '@app/services/public-app/my-profile-public.service';
import { Environment } from "@env/environment";
import { FuseNavigationService } from "@fuse/components/navigation/navigation.service";
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { OidcConfigService, OidcSecurityService } from "angular-auth-oidc-client";
import { NgxPermissionsService } from "ngx-permissions";
import { BehaviorSubject, Observable, Subject } from "rxjs";
import { TLMatDialog } from '../components/dialog-wrapper/tl-mat-dialog';
import { AreaTypes } from "../enums/area-type.enum";
import { IIdentificationToken } from "../interfaces/id-token.interface";
import { IS_PUBLIC_APP } from "../modules/application.modules";
import { MainNavigation } from "../navigation/base/main.navigation";
import { INotificationSecurity } from '../notifications/models/notification-security.interface';
import { CommonUtils } from "../utils/common.utils";
import { RequestProperties } from "./request-properties";
import { RequestService } from "./request.service";

@Injectable({
    providedIn: 'root'
})
export class AuthService implements INotificationSecurity {

    private static readonly OIDC_CONFIGURATION = 'assets/auth.clientConfiguration.json';

    public userRegistrationInfoEvent = new BehaviorSubject<UserAuthDTO | null>(null);

    private translationService: FuseTranslationLoaderService;
    private changePasswordDialog: TLMatDialog<ChangePasswordComponent>;
    private changeUserDataDialog: TLMatDialog<ChangeUserDataComponent>;
    private readonly userContoller: string = 'User';
    private oidcSecurityService: OidcSecurityService;
    private permissionsService: NgxPermissionsService;
    private oidcConfigService: OidcConfigService;
    private requestService: RequestService;
    private fuseNavigationService: FuseNavigationService;
    private _userData: any;
    private router: Router;
    private _userRegistrationInfo?: UserAuthDTO;
    private _userEmail?: string;
    private snackBarRef: MatSnackBar;
    
    public startedUserInfoRequest: boolean = false;
    public startedAuthenticationCheck: boolean = false;

    public isAuthenticatedEvent: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);
    public userInfoReceived: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);

    private myProfileService: MyProfilePublicService;
    private impersonationToken?: string | undefined;

    public get ImpersonationToken(): string | undefined {
        if (this.impersonationToken == null || this.impersonationToken == undefined || this.impersonationToken == '') {
            const impersonationToken = window.sessionStorage.getItem('impersonate');

            if (impersonationToken != null) {
                this.impersonationToken = impersonationToken;
            }
        }

        return this.impersonationToken;
    }

    public set ImpersonationToken(value: string | undefined) {
        this.impersonationToken = value;
        if (value == undefined) {
            window.sessionStorage.removeItem('impersonate');
        } else {
            window.sessionStorage.setItem('impersonate', value);
        }
    }

    public get userRegistrationInfo(): UserAuthDTO | undefined {
        return this._userRegistrationInfo;
    }

    public set userRegistrationInfo(value: UserAuthDTO | undefined) {
        this._userRegistrationInfo = value;
    }

    public get userEmail(): string | undefined {
        return this._userEmail;
    }

    public set userEmail(value: string | undefined) {
        this._userEmail = value;
    }

    public get userData(): any {
        return this._userData;
    }

    constructor(oidcSecurityService: OidcSecurityService,
        permissionsService: NgxPermissionsService,
        oidcConfigService: OidcConfigService,
        requestService: RequestService,
        fuseNavigationService: FuseNavigationService,
        router: Router,
        changePasswordDialog: TLMatDialog<ChangePasswordComponent>,
        changeUserDataDialog: TLMatDialog<ChangeUserDataComponent>,
        translationService: FuseTranslationLoaderService,
        myProfileService: MyProfilePublicService,
        snackBarRef: MatSnackBar) {

        this.snackBarRef = snackBarRef;
        this.translationService = translationService;
        this.fuseNavigationService = fuseNavigationService;
        this.oidcSecurityService = oidcSecurityService;
        this.permissionsService = permissionsService;
        this.oidcConfigService = oidcConfigService;
        this.requestService = requestService;
        this.router = router;
        this.changePasswordDialog = changePasswordDialog;
        this.changeUserDataDialog = changeUserDataDialog;
        this.myProfileService = myProfileService;

        this.oidcSecurityService.userData$.subscribe((userData: any) => {

            this._userData = userData;
        });

        this.oidcSecurityService.isAuthenticated$.subscribe(result => {
            this.changeIsAuthenticatedValue(result);
        });
    }

    public getToken(): string {
        return this.oidcSecurityService.getToken();
    }

    public isAuthenticated(): BehaviorSubject<boolean> {
        return this.isAuthenticatedEvent;
    }

    public logout(urlHandle?: (url: string) => any): Observable<void> {
        window.sessionStorage.removeItem('impersonate');

        const loggedOut: Subject<void> = new Subject<void>();
        this.oidcSecurityService.logoffAndRevokeTokens(urlHandle).subscribe(x => {
            console.log(x);
            loggedOut.next();
            loggedOut.complete();
        });

        return loggedOut;
    }

    public login() {
        if (!this.isAuthenticatedEvent.value) {
            this.startedAuthenticationCheck = true;
            this.oidcSecurityService.authorize();
        }
    }

    public configureAuth(): Promise<void> {
        const configuration = Environment.Instance.ClientAuthConfiguration;
        return this.oidcConfigService.withConfig(configuration);
    }

    public checkAuthentication(includingServer?: boolean): Promise<boolean> {

        if (includingServer == undefined) {
            includingServer = true;
        }

        const checked: Promise<boolean> = new Promise<boolean>((resolve, reject) => {
            if (includingServer) {
                this.oidcSecurityService.checkAuthIncludingServer().subscribe(isAuthenticated => {
                    resolve(isAuthenticated);
                });
            } else {
                this.oidcSecurityService.checkAuth().subscribe(isAuthenticated => {
                    resolve(isAuthenticated);
                })
            }
        });

        return checked;
    }

    private checked: Promise<boolean> | undefined;

    public authorize: boolean = false;

    public checkAuthAndLogin(): Promise<boolean> {
        if (this.checked == undefined) {
            this.checked = new Promise<boolean>((resolve, reject) => {

                if (!this.startedAuthenticationCheck) {
                    this.startedAuthenticationCheck = true;
                    this.oidcSecurityService.checkAuthIncludingServer().subscribe(isAuthenticated => {
                        if (isAuthenticated) {
                            if (this.userInfoReceived.value) {
                                resolve(this.userInfoReceived.value);
                            } else if (!this.startedUserInfoRequest) {
                                this.getUserAuthInfo().subscribe(isUserInfoLoaded => {
                                    if (isUserInfoLoaded) {
                                        resolve(isUserInfoLoaded);
                                    }
                                });
                            } else {
                                this.userInfoReceived.subscribe(isUserInfoReceived => {
                                    if (isUserInfoReceived) {
                                        resolve(isUserInfoReceived);
                                    }
                                });
                            }
                        } else if (!IS_PUBLIC_APP) {
                            this.oidcSecurityService.authorize();
                            this.isAuthenticatedEvent.subscribe(isAuthenticated => {
                                resolve(isAuthenticated);
                            });
                        } else {
                            resolve(false);
                        }
                    });
                } else if (this.isAuthenticatedEvent.value && this.userInfoReceived.value) {
                    resolve(true);
                } else {
                    resolve(false);
                }
            });
        }

        return this.checked;
    }


    public hasUserValidIdToken(): boolean {
        const token = this.oidcSecurityService.getIdToken();

        if (!CommonUtils.isNullOrEmpty(token)) {
            const idToken = this.oidcSecurityService.getPayloadFromIdToken() as IIdentificationToken;
            const validFrom: Date = new Date(idToken.nbf * 1000);
            const validTo: Date = new Date(idToken.exp * 1000);
            const now: Date = new Date();

            if (validFrom <= now && validTo >= now) {
                return true;
            }
        }

        return false;
    }

    public getUserAuthInfo(): BehaviorSubject<boolean> {
        if (!this.startedUserInfoRequest) {
            this.startedUserInfoRequest = true;
            this.requestService.get<UserAuthDTO>(AreaTypes.Common, this.userContoller, 'GetUserAuthInfo', {
                properties: new RequestProperties({
                    showException: true,
                    rethrowException: true,
                    showProgressSpinner: false
                })
            }).subscribe(
                {
                    next: (user: UserAuthDTO) => {
                        this.userRegistrationInfo = user;

                        if (user.permissions != null && user.permissions != undefined && user.permissions.length > 0) {
                            this.permissionsService.loadPermissions(user.permissions);
                        }
                        this.buildNavigation();

                        this.userRegistrationInfoEvent.next(user);
                        this.userInfoReceived.next(true);
                    },
                    error: (error: any) => {
                        this.router.navigateByUrl('/unauthorized');
                    }
                });
        }

        return this.userInfoReceived;
    }

    public redirectBasedOnUser(): void {
        if (this.userRegistrationInfo != undefined) {
            if (this.userRegistrationInfo.currentLoginType === LoginTypesEnum.PASSWORD) {
                if (!this.openUserModal()) {
                    this.redirectToHome();
                }
            }
            else if (this.userRegistrationInfo.currentLoginType === LoginTypesEnum.EAUTH) {
                if (this.userRegistrationInfo.hasEAuthLogin) {
                    this.redirectToHome();
                }
                else if (!this.userRegistrationInfo.hasEAuthLogin && this.userRegistrationInfo.hasUserPassLogin) {
                    this.navigateToMergePage();
                }
                else if (!this.userRegistrationInfo.hasEAuthLogin && !this.userRegistrationInfo.hasUserPassLogin) {
                    this.navigateToRegistrationPage();
                }
            }
            else if (this.userRegistrationInfo.currentLoginType === LoginTypesEnum.NEW_REGISTRATION) {
                this.navigateToRegistrationPage();
            }
        } else {
            this.redirectToUnauthorized();
        }
    }

    public openUserModal(): boolean {
        if (this.userRegistrationInfo !== undefined && this.userRegistrationInfo !== null) {
            if (this.userRegistrationInfo.userMustChangeData) {
                this.openChangeUserDataDialog();
                return true;
            } else if (this.userRegistrationInfo.userMustChangePassword) {
                this.openChangePasswordDialog();
                return true;
            }
        }

        return false;
    }

    private isDialogOpened: boolean = false;

    public openChangePasswordDialog(): void {
        if (!this.isDialogOpened) {
            const dialog = this.changePasswordDialog.open({
                title: this.translationService.getValue('my-profile.change-password-dialog-title'),
                TCtor: ChangePasswordComponent,
                headerCancelButton: {
                    cancelBtnClicked: this.logout.bind(this)
                },
                componentData: new DialogParamsModel({
                    id: this.userRegistrationInfo?.personId,
                    isApplication: false,
                    isReadonly: false,
                    service: this.myProfileService
                }),
                translteService: this.translationService,
                disableDialogClose: true,
                saveBtn: {
                    id: 'save',
                    color: 'accent',
                    translateValue: this.translationService.getValue('my-profile.change')
                },
                cancelBtn: {
                    id: 'cancel',
                    color: 'primary',
                    translateValue: this.translationService.getValue('common.cancel'),
                    disabled: true
                },
            }, '800px');

            dialog.subscribe({
                next: (password: UserPasswordDTO | undefined) => {

                    this.logout();
                }
            });
            this.isDialogOpened = true;
        }
    }

    public openChangeUserDataDialog(): void {
        if (!this.isDialogOpened) {

            const dialog = this.changeUserDataDialog.open({
                title: this.translationService.getValue('my-profile.change-user-data-dialog-title'),
                TCtor: ChangeUserDataComponent,
                headerCancelButton: {
                    cancelBtnClicked: this.logout.bind(this)
                },
                componentData: {
                    userId: this.userRegistrationInfo!.id!,
                    userMustChangePassword: this.userRegistrationInfo!.userMustChangePassword ?? false
                },
                translteService: this.translationService,
                disableDialogClose: true,
                saveBtn: {
                    id: 'save',
                    color: 'accent',
                    translateValue: this.translationService.getValue('common.save')
                },
                cancelBtn: {
                    id: 'cancel',
                    color: 'primary',
                    translateValue: this.translationService.getValue('common.cancel'),
                    disabled: false,
                    buttonData: this.logout.bind(this)
                },
            }, '1300px');

            dialog.subscribe({
                next: (userData: ChangeUserDataDTO | undefined) => {
                    if (userData != undefined && userData.email != undefined) {
                        this.userEmail = userData.email;
                        this.logout((url) => {
                            this.router.navigateByUrl('/successful-registration');
                        });
                    } else {
                        this.logout();
                    }
                }
            });
            this.isDialogOpened = true;
        }
    }

    public buildNavigation(): void {
        // Get default navigation
        MainNavigation.getFuseNavigation(this.permissionsService, this.isAuthenticatedEvent.value).then(navigation => {
            // Register the navigation to the service
            this.fuseNavigationService.unregister("main");
            this.fuseNavigationService.register('main', navigation);

            // Set the main navigation as our current navigation
            this.fuseNavigationService.setCurrentNavigation('main');
        });
    }


    public getUserPhoto(id: number): Observable<string> {
        const params = new HttpParams().append('id', id.toString());
        return this.requestService.get(AreaTypes.Common, this.userContoller, 'GetUserPhoto', {
            httpParams: params,
            responseType: 'text',
            properties: RequestProperties.NO_SPINNER
        });
    }

    public registerUser(user: UserRegistrationDTO): Observable<number> {
        return this.requestService.post(AreaTypes.Common, this.userContoller, 'AddExternalUser', user, {
            properties: new RequestProperties({
                showException: true,
                rethrowException: true
            })
        });
    }

    public updateUserRegistration(user: UserRegistrationDTO): Observable<number> {
        return this.requestService.post(AreaTypes.Common, this.userContoller, 'UpdateUserRegistration', user, {
            properties: new RequestProperties({
                showException: true,
                rethrowException: true
            })
        })
    }

    public confirmEmailAndPassword(user: UserLoginDTO): Observable<boolean> {

        return this.requestService.post(AreaTypes.Common, this.userContoller, 'ConfirmEmailAndPassword', user, {
            properties: { showProgressSpinner: true, showException: true, rethrowException: true }
        });
    }

    public updateUserEAuthData(user: UserRegistrationDTO): Observable<number> {
        return this.requestService.put(AreaTypes.Common, this.userContoller, 'UpdateUserEAuthData', user);
    }

    public deactivateUserPasswordAccount(egnLnch: string): Observable<number> {
        const httpParams = new HttpParams().append('egnLnch', egnLnch);
        return this.requestService.post(AreaTypes.Common, this.userContoller, 'DeactivateUserPasswordAccount', null, { httpParams: httpParams });
    }

    public resendConfirmationEmail(email: string): Observable<void> {
        const httpParams = new HttpParams().append('email', email);
        return this.requestService.post(AreaTypes.Common, this.userContoller, 'ResendConfirmationEmail', null, { httpParams: httpParams });
    }

    public resendConfirmationEmailForToken(token: string): Observable<void> {
        const body = new UserTokenDTO({ token: token });
        return this.requestService.post(AreaTypes.Common, this.userContoller, 'ResendConfirmationEmailForToken', body);
    }

    public changePassword(info: UserChangePasswordDTO): Observable<void> {
        return this.requestService.post(AreaTypes.Common, this.userContoller, 'ChangePassword', info);
    }

    public activateUserAccount(token: string): Observable<AccountActivationStatusesEnum> {
        const body: UserTokenDTO = new UserTokenDTO({ token: token });
        return this.requestService.post(AreaTypes.Common, this.userContoller, 'ActivateUserAccount', body);
    }

    public redirectToHome(): void {
        if (!IS_PUBLIC_APP) {
            this.redirectToHomePageAdministrativeApp();
        }
        else {
            this.redirectToHomePagePublicApp();
        }
    }

    private redirectToUnauthorized(): void {
        this.router.navigate(['/unauthorized']);
    }

    private redirectToHomePageAdministrativeApp(): void {
        this.router.navigate(['/dashboard']);
    }

    private redirectToHomePagePublicApp(): void {
        this.router.navigate(['/news']);
    }

    private navigateToMergePage(): void {
        this.router.navigate(['/merge-profiles']);
    }

    private navigateToRegistrationPage(): void {
        this.router.navigate(['/registration']);
    }

    private changeIsAuthenticatedValue(value: boolean) {
        if (this.isAuthenticatedEvent.value != value) {
            this.isAuthenticatedEvent.next(value);
        }
    }

}

export interface IClientAuthConfig {
    stsServer: string;
    redirect_url: string;
    client_id: string;
    response_type: string;
    scope: string;
    post_logout_redirect_uri: string;
    start_checksession: boolean;
    silent_renew: boolean;
    silent_renew_url: string;
    post_login_route: string;
    forbidden_route: string;
    unauthorized_route: string;
    max_id_token_iat_offset_allowed_in_seconds: number;
    historyCleanupOff: boolean;
    log_level: number;
    history_cleanup_off: boolean,
    auth_wellknown_endpoint: string
}