import { FuseTranslationLoaderService } from '@/@fuse/services/translation-loader.service';
import { NGX_MAT_DATE_FORMATS } from '@angular-material-components/datetime-picker';
import { APP_BASE_HREF, registerLocaleData } from '@angular/common';
import { HttpClient, HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import localeBg from '@angular/common/locales/bg';
import { APP_INITIALIZER, Injector, LOCALE_ID, NgModule } from '@angular/core';
import { DateAdapter, MAT_DATE_FORMATS } from '@angular/material/core';
import { MatPaginatorIntl } from '@angular/material/paginator';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { ExtraOptions, NoPreloading, RouterModule, Routes } from '@angular/router';
import { AppComponent } from '@app/app.component';
import { fuseConfig } from '@app/fuse-config';
import { LayoutModule } from '@app/layout/layout.module';
import { Environment } from '@env/environment';
import { FuseProgressBarModule, FuseSidebarModule, FuseThemeOptionsModule } from '@fuse/components';
import { FuseNavigationService } from '@fuse/components/navigation/navigation.service';
import { FuseSharedModule } from '@fuse/fuse-shared.module';
import { FuseModule } from '@fuse/fuse.module';
import { TranslateModule } from '@ngx-translate/core';
import { NgxPermissionsModule } from 'ngx-permissions';
import { AccountModule } from './components/common-app/auth/account.module';
import { ACCOUNT_ROUTES } from './components/common-app/auth/account.routing';
import { IPermissionsService } from './components/common-app/auth/interfaces/permissions-service.interface';
import { SecurityConfig } from './components/common-app/auth/interfaces/security-config.interface';
import { IGenericSecurityService, ISecurityService } from './components/common-app/auth/interfaces/security-service.interface';
import { User } from './components/common-app/auth/models/auth/user.model';
import { AuthRedirectComponent } from './components/common-app/auth/redirects/auth-redirect.component';
import { AuthenticationUtils } from './components/common-app/auth/utils/authentication.utils';
import { OnlinePaymentPageComponent } from './components/common-app/online-payment-page/online-payment-page.component';
import { USER_REGISTRATION_ROUTES } from './components/common-app/user-registration/user-registration.routing';
import { UnauthorizedComponent } from './components/unauthorized/unauthorized.component';
import { GlobalVariables } from './globals';
import { UserAuthDTO } from './models/generated/dtos/UserAuthDTO';
import { SecurityService } from './services/common-app/security.service';
import { UsersService } from './services/common-app/users.service';
import { ENVIRONMENT_CONFIG_TOKEN } from './shared/di/shared-di.tokens';
import { StorageTypes } from './shared/enums/storage-types.enum';
import { AuthInterceptor } from './shared/interceptors/auth.interceptor';
import { IARA_APPLICATION_MODULE } from './shared/modules/application.modules';
import { MainNavigation } from './shared/navigation/base/main.navigation';
import { StorageService } from './shared/services/local-storage.service';
import { PermissionsService } from './shared/services/permissions.service';
import { RequestService } from './shared/services/request.service';
import { CustomMatPaginatorIntl } from './shared/utils/custom.paginator';
import { TLDateAdapter } from './shared/utils/date.adapter';
import { DateUtils } from './shared/utils/date.utils';
import { TranslationUtils } from './shared/utils/translation-utils';




const appRoutes: Routes = [
    {
        path: '',
        pathMatch: 'full',
        component: AuthRedirectComponent,
    },
    {
        path: 'online-payment',
        component: OnlinePaymentPageComponent
    },
    {
        path: 'account',
        children: ACCOUNT_ROUTES
    },
    {
        path: 'administration',
        children: MainNavigation.getRoutes()
    },
    ...USER_REGISTRATION_ROUTES,
    {
        path: '**',
        //redirectTo: 'landing-page'
        component: AuthRedirectComponent
    },

];

export function initializeApplication(translationLoad: FuseTranslationLoaderService,
    securityService: IGenericSecurityService<number, UserAuthDTO>,
    permissionsService: IPermissionsService,
    fuseNavigationService: FuseNavigationService): () => void {

    return async () => {
        await loadTranslationResources(translationLoad).then(async () => {
            await AuthenticationUtils.initApplicationSecurity((securityService as IGenericSecurityService<number, User<number>>)).then(isAuthenticated => {

                AuthenticationUtils.initNavigation(permissionsService, fuseNavigationService, isAuthenticated);

                securityService.isAuthenticatedEvent.subscribe(async (isAuthenticated) => {
                    if (isAuthenticated) {
                        await securityService.getUser().toPromise();
                    }

                    AuthenticationUtils.initNavigation(permissionsService, fuseNavigationService, isAuthenticated);
                });
            });
        });
    };
}

export async function loadTranslationResources(translationLoad: FuseTranslationLoaderService): Promise<void> {
    let language: string = 'bg';

    const storage: StorageService = StorageService.getStorage(StorageTypes.Local)
    if (storage.hasItem('lang')) {
        language = await storage.get('lang') ?? 'bg';
    }
    else {
        storage.addOrUpdate('lang', 'bg');
    }

    const backupTranslation = TranslationUtils.getLocalTranslations(language);
    translationLoad.loadTranslations(backupTranslation);

    //const translationLoader: TranslateLoader = TranslationUtils.getWebTranslationLoader();
    //const translation = await TranslationUtils.getTranslationsFromLoader(translationLoader, language);

    //if (translation !== null && translation !== undefined) {
    //    translationLoad.loadTranslations(translation);
    //}
}

const routerConfig: ExtraOptions = {
    preloadingStrategy: NoPreloading,
    scrollPositionRestoration: 'enabled',
    onSameUrlNavigation: 'ignore',
    urlUpdateStrategy: 'deferred'
};

@NgModule({
    declarations: [
        AppComponent,
        UnauthorizedComponent
    ],
    imports: [
        BrowserModule,
        BrowserAnimationsModule,
        //NoopAnimationsModule,
        HttpClientModule,
        RouterModule.forRoot(appRoutes, routerConfig),

        TranslateModule.forRoot(),

        // Fuse modules
        FuseModule.forRoot(fuseConfig),
        FuseProgressBarModule,
        FuseSharedModule,
        FuseSidebarModule,
        FuseThemeOptionsModule,

        // App modules
        LayoutModule,
        //IARAApplicationModule,
        //PublicApplicationModule,
        //AdministrationApplicationModule,
        NgxPermissionsModule.forRoot(),
        AccountModule.forRoot(SecurityService, RequestService, {
            loginMethodName: 'SignIn',
        } as SecurityConfig, FuseTranslationLoaderService, UsersService, PermissionsService),
        ...IARA_APPLICATION_MODULE
    ],
    bootstrap: [
        AppComponent
    ],
    providers: [
        {
            provide: APP_INITIALIZER,
            useFactory: initializeApplication,
            multi: true,
            deps: [FuseTranslationLoaderService, SecurityService, PermissionsService, FuseNavigationService]
        },
        {
            provide: APP_BASE_HREF,
            useValue: Environment.Instance.appBaseHref
        },
        {
            provide: DateAdapter,
            useClass: TLDateAdapter
        },
        {
            provide: MAT_DATE_FORMATS,
            useValue: DateUtils.TL_DATE_FORMATS
        },
        {
            provide: NGX_MAT_DATE_FORMATS,
            useValue: DateUtils.TL_NGX_DATE_FORMATS
        },
        {
            provide: HTTP_INTERCEPTORS,
            useClass: AuthInterceptor,
            multi: true
        },
        {
            provide: MatPaginatorIntl,
            useClass: CustomMatPaginatorIntl
        },
        {
            provide: LOCALE_ID,
            useValue: 'bg-BG'
        },
        {
            provide: ENVIRONMENT_CONFIG_TOKEN,
            useValue: Environment.Instance
        }
    ]
})
export class AppModule {
    constructor(private injector: Injector) {

        GlobalVariables.AppInjector = this.injector;

        registerLocaleData(localeBg);
    }
}