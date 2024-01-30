import { Provider } from '@angular/core';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { REQUEST_SERVICE_TOKEN, TRANSLATE_SERVICE_TOKEN } from '@app/shared/di/shared-di.tokens';
import { IRequestService } from '@app/shared/interfaces/request-service.interface';
import { ITranslationService } from '@app/shared/interfaces/translate-service.interface';
import { MainNavigation } from '@app/shared/navigation/base/main.navigation';
import { FuseNavigationService } from '@fuse/components/navigation/navigation.service';
import { SECURITY_CONFIG_TOKEN, SECURITY_SERVICE_TOKEN, USER_SERVICE_TOKEN } from '../di/auth-di.tokens';
import { AuthenticationOptionEnum } from '../enums/authentication-option.enum';
import { IPermissionsService } from '../interfaces/permissions-service.interface';
import { SecurityConfig } from '../interfaces/security-config.interface';
import { IGenericSecurityService } from '../interfaces/security-service.interface';
import { IGenericUserService } from '../interfaces/user-service.interface';
import { User } from '../models/auth/user.model';

export class AuthenticationUtils {
    public static readonly replacePattern: string = '$1******$3';
    public static readonly phoneRegEx: RegExp = /(^\d{3})(\d*)(\d{3}$)/;
    public static readonly emailRegEx: RegExp = /(^.{2})(.*)(@.*)/;

    public static getAuthenticationOptions(translateService: ITranslationService): NomenclatureDTO<AuthenticationOptionEnum>[] {
        return [
            new NomenclatureDTO<AuthenticationOptionEnum>({
                value: AuthenticationOptionEnum.AUTHENTICATOR,
                displayName: translateService.getValue('auth.authenticator'),
                isActive: true
            }),
            new NomenclatureDTO<AuthenticationOptionEnum>({
                value: AuthenticationOptionEnum.SMS,
                displayName: translateService.getValue('auth.sms'),
                isActive: true
            }),
            new NomenclatureDTO<AuthenticationOptionEnum>({
                value: AuthenticationOptionEnum.EMAIL,
                displayName: translateService.getValue('auth.email'),
                isActive: true
            })
        ];
    }

    public static async initApplicationSecurity<TIdentifier, TUser extends User<TIdentifier>>(securityService: IGenericSecurityService<TIdentifier, TUser>): Promise<boolean> {
        if (await securityService.isAuthenticated()) {
            await securityService.getUser().toPromise();
            return true;
        }

        return false;
    }

    public static initNavigation(permissionsService: IPermissionsService, fuseNavigationService:FuseNavigationService, isAuthenticated: boolean) {
        MainNavigation.getFuseNavigation(permissionsService, isAuthenticated).then(navigation => {
            // Register the navigation to the service
            fuseNavigationService.unregister("main");
            fuseNavigationService.register('main', navigation);

            // Set the main navigation as our current navigation
            fuseNavigationService.setCurrentNavigation('main');
        });
    }

    public static obfuscateEmail(email: string): string {
        return email.replace(AuthenticationUtils.emailRegEx, AuthenticationUtils.replacePattern);
    }

    public static obfuscatePhone(phone: string): string {
        return phone.replace(AuthenticationUtils.phoneRegEx, AuthenticationUtils.replacePattern);
    }

    public static getSecurityProviders<TIdentifier, TUser extends User<TIdentifier>>(
        securityService: new (...args: any[]) => IGenericSecurityService<TIdentifier, TUser>,
        requestService: new (...args: any[]) => IRequestService,
        securityConfig: SecurityConfig,
        translationService: new (...args: any[]) => ITranslationService,
        userService: new (...args: any[]) => IGenericUserService<TIdentifier, TUser>
    ): Provider[] {
        return [
            {
                provide: SECURITY_SERVICE_TOKEN,
                useExisting: securityService
            },
            {
                provide: SECURITY_CONFIG_TOKEN,
                useValue: securityConfig
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
            }
        ] as Provider[];
    }
}