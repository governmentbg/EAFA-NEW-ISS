import { Inject, Injectable } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { PERMISSIONS_SERVICE_TOKEN, SECURITY_CONFIG_TOKEN, USER_SERVICE_TOKEN } from '@app/components/common-app/auth/di/auth-di.tokens';
import { IPermissionsService } from '@app/components/common-app/auth/interfaces/permissions-service.interface';
import { SecurityConfig } from '@app/components/common-app/auth/interfaces/security-config.interface';
import { IGenericUserService } from '@app/components/common-app/auth/interfaces/user-service.interface';
import { BaseSecurityService } from '@app/components/common-app/auth/services/base-security.service';
import { ChangePasswordComponent } from '@app/components/common-app/my-profile/components/change-password/change-password.component';
import { ChangeUserDataComponent } from '@app/components/common-app/my-profile/components/change-userdata/change-userdata.component';
import { DefaultUserPaths } from '@app/enums/default-user-paths.enum';
import { LoginTypesEnum } from '@app/enums/login-types.enum';
import { DialogParamsModel } from '@app/models/common/dialog-params.model';
import { ChangeUserDataDTO } from '@app/models/generated/dtos/ChangeUserDataDTO';
import { UserAuthDTO } from '@app/models/generated/dtos/UserAuthDTO';
import { UserPasswordDTO } from '@app/models/generated/dtos/UserPasswordDTO';
import { TLMatDialog } from '@app/shared/components/dialog-wrapper/tl-mat-dialog';
import { REQUEST_SERVICE_TOKEN } from '@app/shared/di/shared-di.tokens';
import { IRequestService } from '@app/shared/interfaces/request-service.interface';
import { IS_PUBLIC_APP } from '@app/shared/modules/application.modules';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { MyProfilePublicService } from '../public-app/my-profile-public.service';

@Injectable({
    providedIn: 'root'
})
export class SecurityService extends BaseSecurityService<number, UserAuthDTO> {

    private changePasswordDialog: TLMatDialog<ChangePasswordComponent>;
    private changeUserDataDialog: TLMatDialog<ChangeUserDataComponent>;
    private isDialogOpened: boolean = false;
    private translationService: FuseTranslationLoaderService;
    private myProfileService: MyProfilePublicService;

    constructor(@Inject(REQUEST_SERVICE_TOKEN) requestService: IRequestService,
        @Inject(SECURITY_CONFIG_TOKEN) securityConfig: SecurityConfig,
        @Inject(PERMISSIONS_SERVICE_TOKEN) permissionsService: IPermissionsService,
        @Inject(USER_SERVICE_TOKEN) usersService: IGenericUserService<number, UserAuthDTO>,
        router: Router,
        matDialog: MatDialog,
        changePasswordDialog: TLMatDialog<ChangePasswordComponent>,
        changeUserDataDialog: TLMatDialog<ChangeUserDataComponent>,
        translationService: FuseTranslationLoaderService,
        myProfileService: MyProfilePublicService) {
        super(requestService, securityConfig, permissionsService, usersService, router, matDialog);
        this.changePasswordDialog = changePasswordDialog;
        this.changeUserDataDialog = changeUserDataDialog;
        this.translationService = translationService;
        this.myProfileService = myProfileService;
    }

    public async getUserRedirectPath(): Promise<string> {
        if (await this.isAuthenticated()) {
            const user = this.User;
            if (user.currentLoginType === LoginTypesEnum.PASSWORD) {
                if (!this.openUserModal(user)) {
                    return this.redirectToHome();
                }
            }
            else if (user.currentLoginType === LoginTypesEnum.EAUTH) {
                if (user.hasEAuthLogin) {
                    return this.redirectToHome();
                }
                else if (!user.hasEAuthLogin && user.hasUserPassLogin) {
                    return '/merge-profiles';
                }
                else if (!user.hasEAuthLogin && !user.hasUserPassLogin) {
                    return '/registration';
                }
            }
            else if (user.currentLoginType === LoginTypesEnum.NEW_REGISTRATION) {
                return '/registration';
            }
        }

        return DefaultUserPaths.Unauthorized;
    }

    public openUserModal(user: UserAuthDTO): boolean {
        if (user !== undefined && user !== null) {
            if (user.userMustChangeData) {
                this.openChangeUserDataDialog(user);
                return true;
            } else if (user.userMustChangePassword) {
                this.openChangePasswordDialog(user);
                return true;
            }
        }

        return false;
    }

    public openChangeUserDataDialog(user: UserAuthDTO): void {
        if (!this.isDialogOpened) {

            const dialog = this.changeUserDataDialog.open({
                title: this.translationService.getValue('my-profile.change-user-data-dialog-title'),
                TCtor: ChangeUserDataComponent,
                headerCancelButton: {
                    cancelBtnClicked: this.logout.bind(this)
                },
                componentData: {
                    userId: user!.id!,
                    userMustChangePassword: user!.userMustChangePassword ?? false
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
                        this.logout().subscribe(() => {
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

    public openChangePasswordDialog(user: UserAuthDTO): void {
        if (!this.isDialogOpened) {
            const dialog = this.changePasswordDialog.open({
                title: this.translationService.getValue('my-profile.change-password-dialog-title'),
                TCtor: ChangePasswordComponent,
                headerCancelButton: {
                    cancelBtnClicked: (closeFn) => closeFn()
                },
                componentData: new DialogParamsModel({
                    id: user?.personId,
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
                },
            }, '800px');

            dialog.subscribe({
                next: (password: UserPasswordDTO | undefined) => {
                    this.logout();
                    this.isDialogOpened = false;
                    setTimeout(() => {
                        this.router.navigate(['/']);
                    }, 100);
                }
            });
            this.isDialogOpened = true;
        }
    }

    private redirectToHome(): string {
        if (IS_PUBLIC_APP) {
            return DefaultUserPaths.External;
        } else {
            return DefaultUserPaths.Internal;
        }
    }
}
