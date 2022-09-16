import { FuseTranslationLoaderService } from '@/@fuse/services/translation-loader.service';
import { NGX_MAT_DATE_FORMATS } from '@angular-material-components/datetime-picker';
import { registerLocaleData } from '@angular/common';
import { HttpClient, HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import localeBg from '@angular/common/locales/bg';
import { APP_INITIALIZER, Injector, LOCALE_ID, NgModule } from '@angular/core';
import { DateAdapter, MAT_DATE_FORMATS } from '@angular/material/core';
import { MatPaginatorIntl } from '@angular/material/paginator';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { RouterModule, Routes } from '@angular/router';
import { AppComponent } from '@app/app.component';
import { fuseConfig } from '@app/fuse-config';
import { LayoutModule } from '@app/layout/layout.module';
import { Environment } from '@env/environment';
import { FuseProgressBarModule, FuseSidebarModule, FuseThemeOptionsModule } from '@fuse/components';
import { FuseSharedModule } from '@fuse/fuse-shared.module';
import { FuseModule } from '@fuse/fuse.module';
import { TranslateModule } from '@ngx-translate/core';
import { AuthModule } from 'angular-auth-oidc-client';
import { NgxPermissionsModule } from 'ngx-permissions';
import { IConfiguration } from '../environments/environment.interface';
import { OnlinePaymentPageComponent } from './components/common-app/online-payment-page/online-payment-page.component';
import { ChangePasswordPageComponent } from './components/common-app/user-registration/change-password/change-password-page.component';
import { CreateProfileComponent } from './components/common-app/user-registration/create-profile/create-profile.component';
import { MergeProfilesComponent } from './components/common-app/user-registration/merge-profiles/merge-profiles.component';
import { RedirectPageComponent } from './components/common-app/user-registration/redirect-page/redirect-page.component';
import { SuccessfulEmailConfirmationComponent } from './components/common-app/user-registration/success-pages/successful-email-confrimation/successful-email-confirmation.component';
import { SuccessfulPasswordChangeComponent } from './components/common-app/user-registration/success-pages/successful-password-change/successful-password-change.component';
import { SuccessfulRegistrationComponent } from './components/common-app/user-registration/success-pages/successful-registration/successful-registration.component';
import { TermsAndConditionsComponent } from './components/common-app/user-registration/terms-and-conditions/terms-and-conditions.component';
import { UnauthorizedComponent } from './components/unauthorized/unauthorized.component';
import { AuthInterceptor } from './shared/interceptors/auth.interceptor';
import { IARA_APPLICATION_MODULE } from './shared/modules/application.modules';
import { AuthService } from './shared/services/auth.service';
import { LocalStorageService } from './shared/services/local-storage.service';
import { CustomMatPaginatorIntl } from './shared/utils/custom.paginator';
import { TLDateAdapter } from './shared/utils/date.adapter';
import { DateUtils } from './shared/utils/date.utils';
import { TranslationUtils } from './shared/utils/translation-utils';




const appRoutes: Routes = [
    {
        path: 'unauthorized',
        component: UnauthorizedComponent
    },
    {
        path: 'registration',
        component: CreateProfileComponent
    },
    {
        path: 'merge-profiles',
        component: MergeProfilesComponent
    },
    {
        path: 'terms-and-conditions',
        component: TermsAndConditionsComponent
    },
    {
        path: 'change-password',
        component: ChangePasswordPageComponent
    },
    {
        path: 'successful-change',
        component: SuccessfulPasswordChangeComponent
    },
    {
        path: 'successful-registration',
        component: SuccessfulRegistrationComponent
    },
    {
        path: 'successful-email-confirmation',
        component: SuccessfulEmailConfirmationComponent
    },
    {
        path: 'redirect',
        component: RedirectPageComponent
    },
    {
        path: 'online-payment',
        component: OnlinePaymentPageComponent
    },
    {
        path: '',
        component: RedirectPageComponent
    },
    {
        path: '**',
        redirectTo: 'redirect'
    }
];

export let AppInjector: Injector;


export function initializeApplication(translationLoad: FuseTranslationLoaderService,
    localStorageService: LocalStorageService,
    authService: AuthService,
    httpClient: HttpClient): () => void {

    authService.buildNavigation();

    return async () => {

        await httpClient.get<IConfiguration>(`${Environment.Instance.frontendBaseUrl}/assets/endpoints.json`)
            .toPromise().then(config => {

                if (config.servicesBaseUrl != '') {
                    Environment.Instance.servicesBaseUrl = config.servicesBaseUrl;
                }
                if (config.identityServerBaseUrl != '') {
                    Environment.Instance.identityServerBaseUrl = config.identityServerBaseUrl;
                }

                if (config.apiBasePath != '') {
                    Environment.Instance.apiBasePath = config.apiBasePath;
                }

                authService.configureAuth().then(result => {
                    authService.checkAuthentication().then(isAuthenticated => {
                        if (isAuthenticated) {
                            authService.getUserAuthInfo();
                        }
                    });
                });
            });

        loadTranslationResources(translationLoad, localStorageService);
    };
}

export async function loadTranslationResources(translationLoad: FuseTranslationLoaderService, localStorageService: LocalStorageService) {
    let language: string;
    if (localStorageService.hasItem('lang')) {
        language = localStorageService.get('lang');
    }
    else {
        localStorageService.addOrUpdate('lang', 'bg');
        language = 'bg';
    }

    const backupTranslation = TranslationUtils.getLocalTranslations(language);
    translationLoad.loadTranslations(backupTranslation);

    //let translationLoader: TranslateLoader = TranslationUtils.getWebTranslationLoader();
    //let translation = await TranslationUtils.getTranslationsFromLoader(translationLoader, language);

    //if (translation != undefined)
    //    translationLoad.loadTranslations(backupTranslation, translation);
}

@NgModule({
    declarations: [
        AppComponent,
        UnauthorizedComponent
    ],
    imports: [
        BrowserModule,
        BrowserAnimationsModule,
        HttpClientModule,
        RouterModule.forRoot(appRoutes),

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
        AuthModule.forRoot(),
        NgxPermissionsModule.forRoot(),
        ...IARA_APPLICATION_MODULE
    ],
    bootstrap: [
        AppComponent
    ],
    providers: [
        AuthService,
        {
            provide: APP_INITIALIZER,
            useFactory: initializeApplication,
            multi: true,
            deps: [FuseTranslationLoaderService, LocalStorageService, AuthService, HttpClient]
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
        }
    ]
})
export class AppModule {
    constructor(private injector: Injector) {
        AppInjector = this.injector;

        registerLocaleData(localeBg);
    }
}