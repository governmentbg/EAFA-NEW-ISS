import { ModuleWithProviders, NgModule } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { RouterModule } from '@angular/router';
import { UserAuthDTO } from '@app/models/generated/dtos/UserAuthDTO';
import { TLInputControlsModule } from '@app/shared/components/input-controls/tl-input-controls.module';
import { REQUEST_SERVICE_TOKEN, TRANSLATE_SERVICE_TOKEN } from '@app/shared/di/shared-di.tokens';
import { IRequestService } from '@app/shared/interfaces/request-service.interface';
import { ITranslationService } from '@app/shared/interfaces/translate-service.interface';
import { TLPipesModule } from '@app/shared/pipes/tl-pipes.module';
import { TLTranslatePipe } from '@app/shared/pipes/tl-translate.pipe';
import { TLCommonModule } from '@app/shared/tl-common.module';
import { FuseNavigationModule } from '@fuse/components';
import { MissingTranslationHandler, TranslateService } from '@ngx-translate/core';
import { ACCOUNT_ROUTES } from './account.routing';
import { ChangePasswordComponent } from './change-password/change-password.component';
import { AuthConfirmationRequiredComponent } from './confirmation-required/confirmation-required.component';
import { PERMISSIONS_SERVICE_TOKEN, SECURITY_CONFIG_TOKEN, SECURITY_SERVICE_TOKEN, USER_SERVICE_TOKEN } from './di/auth-di.tokens';
import { AuthForgotPasswordComponent } from './forgot-password/forgot-password.component';
import { AuthMissingTranslationHandler, TranslationUtils } from './i18n/translation-service.utils';
import { IPermissionsService } from './interfaces/permissions-service.interface';
import { SecurityConfig } from './interfaces/security-config.interface';
import { IGenericSecurityService } from './interfaces/security-service.interface';
import { IGenericUserService } from './interfaces/user-service.interface';
import { User } from './models/auth/user.model';
import { PublicPageFooterComponent } from './public-page/components/public-page-footer.component';
import { PublicPageHeaderComponent } from './public-page/components/public-page-header.component';
import { PublicPageComponent } from './public-page/public-page.component';
import { AuthRedirectComponent } from './redirects/auth-redirect.component';
import { ExternalAuthRedirectComponent } from './redirects/external-auth-redirect.component';
import { ResetPasswordBaseComponent } from './reset-password/components/reset-password-base.component';
import { AuthResetPasswordComponent } from './reset-password/reset-password.component';
import { AuthSidePanelComponent } from './side-panel/auth-side-panel.component';
import { TfAuthenticationComponent } from './sign-in/components/tf-authentication/tf-authentication.component';
import { AuthSignInComponent } from './sign-in/sign-in.component';
import { AuthSignOutComponent } from './sign-out/sign-out.component';
import { AuthSignUpComponent } from './sign-up/sign-up.component';
import { SuccessfulEmailConfirmationComponent } from './successful-email-confirmation/successful-email-confirmation.component';
import { AuthUnlockSessionComponent } from './unlock-session/unlock-session.component';



@NgModule({
    declarations: [
        AuthConfirmationRequiredComponent,
        AuthForgotPasswordComponent,
        AuthResetPasswordComponent,
        AuthSignInComponent,
        AuthSignOutComponent,
        AuthSignUpComponent,
        AuthUnlockSessionComponent,
        AuthSidePanelComponent,
        ChangePasswordComponent,
        PublicPageComponent,
        PublicPageHeaderComponent,
        PublicPageFooterComponent,
        ResetPasswordBaseComponent,
        SuccessfulEmailConfirmationComponent,
        ExternalAuthRedirectComponent,
        AuthRedirectComponent,
        TfAuthenticationComponent
    ],
    imports: [
        TLPipesModule,
        MatButtonModule,
        MatFormFieldModule,
        MatIconModule,
        MatInputModule,
        MatProgressSpinnerModule,
        MatCheckboxModule,
        TLInputControlsModule,
        RouterModule.forChild(ACCOUNT_ROUTES),
        TranslationUtils.registerTranslation(),
        TLCommonModule,
        FuseNavigationModule
    ],
    exports: [
        AuthRedirectComponent,
        ExternalAuthRedirectComponent
    ],
    providers: [
        TLTranslatePipe
    ]
})
export class AccountModule {

    // eslint-disable-next-line @typescript-eslint/no-empty-function
    private constructor(translateService: TranslateService, missingTranslationHandler: AuthMissingTranslationHandler) {
        missingTranslationHandler.loadTranslation(translateService);
    }

    public static forRoot<TIdentifier, TUser extends User<TIdentifier>>(
        securityService: new (...args: any[]) => IGenericSecurityService<TIdentifier, TUser>,
        requestService: new (...args: any[]) => IRequestService,
        securityConfig: SecurityConfig,
        translationService: new (...args: any[]) => ITranslationService,
        userService: new (...args: any[]) => IGenericUserService<TIdentifier, TUser>,
        permissionsService: new (...args: any[]) => IPermissionsService
    ): ModuleWithProviders<AccountModule> {
        return {
            ngModule: AccountModule,
            providers: [
                AuthMissingTranslationHandler,
                {
                    provide: MissingTranslationHandler,
                    useClass: AuthMissingTranslationHandler
                },
                {
                    provide: SECURITY_SERVICE_TOKEN,
                    useExisting: securityService
                },
                {
                    provide: SECURITY_CONFIG_TOKEN,
                    useValue: new SecurityConfig(securityConfig)
                },
                {
                    provide: REQUEST_SERVICE_TOKEN,
                    useExisting: requestService
                },
                {
                    provide: TRANSLATE_SERVICE_TOKEN,
                    useExisting: translationService
                },
                {
                    provide: USER_SERVICE_TOKEN,
                    useExisting: userService
                },
                {
                    provide: PERMISSIONS_SERVICE_TOKEN,
                    useExisting: permissionsService
                }
            ]
        } as unknown as ModuleWithProviders<AccountModule>;
    }
}
