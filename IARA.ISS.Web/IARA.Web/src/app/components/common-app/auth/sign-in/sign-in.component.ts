import { Component, Inject, OnInit, ViewEncapsulation } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { NumberNomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { TLConfirmDialog } from '@app/shared/components/confirmation-dialog/tl-confirm-dialog';
import { TLSnackbar } from '@app/shared/components/snackbar/tl.snackbar';
import { ENVIRONMENT_CONFIG_TOKEN, TRANSLATE_SERVICE_TOKEN } from '@app/shared/di/shared-di.tokens';
import { ITranslationService } from '@app/shared/interfaces/translate-service.interface';
import { IEnvironmentConfig } from '@env/environment.interface';
import { fuseAnimations } from '@fuse/animations';
import { SECURITY_CONFIG_TOKEN, SECURITY_SERVICE_TOKEN } from '../di/auth-di.tokens';
import { LoginResultTypes } from '../enums/login-result-types.enum';
import { SecurityConfig } from '../interfaces/security-config.interface';
import { IGenericSecurityService } from '../interfaces/security-service.interface';
import { AuthCredentials } from '../models/auth/auth-credentials.model';
import { LoginResult } from '../models/auth/login-result.model';
import { TFAuthenticationModel } from '../models/auth/tf-authentication.model';
import { User } from '../models/auth/user.model';

type ShowFormType = 'login' | 'roles' | 'authentication';

@Component({
    selector: 'auth-sign-in',
    templateUrl: './sign-in.component.html',
    styleUrls: ['./sign-in.component.scss'],
    encapsulation: ViewEncapsulation.None,
    animations: fuseAnimations
})
export class AuthSignInComponent<TIdentifier, TUser extends User<TIdentifier>> implements OnInit {

    public signInForm!: FormGroup;
    public rolesForm!: FormGroup;
    public userRoles?: NumberNomenclatureDTO[];
    public authenticationData!: TFAuthenticationModel;
    public showAlert: boolean = false;
    public formToShow: ShowFormType = 'login';

    private readonly router: Router;
    private readonly securityService: IGenericSecurityService<TIdentifier, TUser>;
    private readonly snackbar: TLSnackbar;
    private readonly translateService: ITranslationService;
    private readonly confirmDialog: TLConfirmDialog;
    private readonly environmentConfig: IEnvironmentConfig;
    private readonly securityConfig: SecurityConfig;

    private revokeExistingToken: boolean = false;

    public constructor(
        router: Router,
        @Inject(SECURITY_SERVICE_TOKEN) securityService: IGenericSecurityService<TIdentifier, TUser>,
        @Inject(SECURITY_CONFIG_TOKEN) securityConfig: SecurityConfig,
        snackbar: TLSnackbar,
        @Inject(TRANSLATE_SERVICE_TOKEN) translateService: ITranslationService,
        confirmDialog: TLConfirmDialog,
        @Inject(ENVIRONMENT_CONFIG_TOKEN) environmentConfig: IEnvironmentConfig) {
        this.securityConfig = securityConfig;
        this.securityService = securityService;
        this.router = router;
        this.confirmDialog = confirmDialog;
        this.snackbar = snackbar;
        this.translateService = translateService;
        this.environmentConfig = environmentConfig;
        this.buildForm();
    }

    public ngOnInit(): void {
        const state = window.history.state;
        if (state != undefined && state.tokenProcessed == true) {
            this.successfulLoginHandler(state.loginResult);
        } else if (this.securityService.token != undefined) {
            if (this.securityService.isTempToken) {
                this.securityService.clearToken();
            }
            else {
                this.router.navigate(['']);
            }
        }
    }

    public goToEAuth(): void {
        const location = `${this.environmentConfig.apiBaseUrl}/${this.securityConfig.baseRoute}/${this.securityConfig.securityController}/${this.securityConfig.eauthRedirectMethodName}`;
        window.location.href = location;
    }

    public issueTokenFor(): Promise<void> | undefined {
        if (this.userRoles != undefined) {
            let selectedValue: NumberNomenclatureDTO;

            if (this.userRoles.length === 1) {
                selectedValue = this.userRoles[0];
            } else {
                if (this.rolesForm.invalid) {
                    return;
                }

                this.rolesForm.disable();

                selectedValue = this.rolesForm.controls.roleIdControl.value as NumberNomenclatureDTO;
            }

            // eslint-disable-next-line @typescript-eslint/no-empty-function
            return new Promise<void>((re) => { });
            //return this.securityService.issueJwtToken(selectedValue.value as number).then(result => {
            //    this.rolesForm.enable();
            //    this.successfulLoginHandler(result);
            //});
        }
        return;
    }

    public async signIn(): Promise<void> {
        // Return if the form is invalid
        if (this.signInForm.invalid) {
            return;
        }

        // Disable the form
        this.signInForm.disable();

        await this.securityService.login(new AuthCredentials({
            userName: this.signInForm?.get('username')?.value ?? '',
            password: this.signInForm?.get('password')?.value ?? '',
            rememberMe: this.signInForm?.get('rememberMe')?.value ?? false,
            revokeExistingToken: this.revokeExistingToken
        })).then(result => {
            this.signInForm.enable();
            this.successfulLoginHandler(result);
        });
    }

    public onValidatedPIN(): void {
        if (this.userRoles != undefined && this.userRoles.length > 1) {
            this.formToShow = 'roles';
        } else {
            this.issueTokenFor();
        }
    }

    public onGoToSignInClicked(): void {
        this.formToShow = 'login';
        this.signInForm.reset();
        this.rolesForm.reset();

        if (this.securityService.token != undefined && this.securityService.isTempToken) {
            this.securityService.clearToken();
        }
    }

    private buildForm(): void {
        // Create the form
        this.signInForm = new FormGroup({
            username: new FormControl('', [Validators.required]),
            password: new FormControl('', [Validators.required]),
            rememberMe: new FormControl(false)
        });

        this.rolesForm = new FormGroup({
            roleIdControl: new FormControl(undefined, [Validators.required])
        });
    }

    private async successfulLoginHandler(result: LoginResult | undefined): Promise<void> {
        if (result) {
            if (result.isTempToken) {

                //this.securityService.getTFAuthenticationData().subscribe({
                //    next: (result: TFAuthenticationModel) => {
                //        this.authenticationData = result;
                //        if (this.authenticationData != undefined) {
                //            this.formToShow = 'authentication';
                //        } else {
                //            this.formToShow = 'roles';
                //        }
                //    }
                //});

                //this.fillUserRoles().then(result => {
                //    this.userRoles = result;
                //});
            } else {
                switch (result.type) {
                    case LoginResultTypes.Success: {

                        await this.securityService.getUser().toPromise().then((user: TUser) => {

                            if (user.passwordExpiryDate != undefined) {

                                user.passwordExpiryDate = new Date(user.passwordExpiryDate);

                                const timeDifference = (user.passwordExpiryDate.getTime() - new Date().getTime());

                                const daysDifference = Math.floor(timeDifference / 1000 / 60 / 60 / 24);

                                if (daysDifference <= 14) {
                                    let message = this.translateService.getValue('auth.password-expiry-comming');
                                    message = message.replace('{0}', daysDifference.toString());
                                    this.snackbar.warning(message);
                                }
                            }

                            return user;
                        });

                        this.router.navigate([`${this.securityConfig.authModulePath}/auth-redirect`], { skipLocationChange: false, state: { isFromLogin: true } });
                    }
                        break;
                    case LoginResultTypes.Locked:
                        this.snackbar.error(this.translateService.getValue('auth.your-profile-is-locked-until') + new Date(result.lockedUntil!).toLocaleTimeString());
                        break;
                    case LoginResultTypes.EmailNotConfirmed:
                        this.snackbar.error(this.translateService.getValue('auth.confirm-your-email'));
                        break;
                    case LoginResultTypes.Blocked:
                        this.snackbar.error(this.translateService.getValue('auth.blocked-profile'));
                        break;
                    case LoginResultTypes.OtherSessionExists:
                        {
                            this.confirmDialog.open({
                                title: this.translateService.getValue('auth.existing-user-session'),
                                message: this.translateService.getValue('auth.revoke-existing-session'),
                                okBtnLabel: this.translateService.getValue('common.yes'),
                                cancelBtnLabel: this.translateService.getValue('common.no')
                            }).subscribe(result => {
                                if (result) {
                                    this.revokeExistingToken = result;
                                    this.signIn();
                                }
                            });
                        }
                        //this.snackbar.warning(this.translateService.getValue('auth.revoke-existing-session'));
                        break;
                    case LoginResultTypes.ForbiddenIP: {
                        this.snackbar.error(this.translateService.getValue('auth.forbidden-ip-address'));
                    } break;
                    case LoginResultTypes.PasswordExpired: {
                        this.snackbar.error(this.translateService.getValue('auth.password-expired'));
                    } break;
                    default:
                        {
                            let message: string = '';
                            if (result.message != null && result.message != undefined && result.message != '') {
                                message = result.message;
                            } else {
                                message = this.translateService.getValue('auth.wrong-username-or-password');
                            }

                            this.snackbar.error(message);
                        }
                        break;
                }
            }
        } else {
            this.snackbar.error(this.translateService.getValue('auth.server-error'));
        }
    }

    //private fillUserRoles(): Promise<NumberNomenclatureDTO[] | undefined> {
    //    return this.securityService.getUserRoles().toPromise();
    //}
}
