import { ModuleWithProviders } from '@angular/compiler/src/core';
import { MissingTranslationHandler, MissingTranslationHandlerParams, TranslateModule, TranslateModuleConfig, TranslateService } from '@ngx-translate/core'
import { auth_BG_resources } from './auth-translations.bg';

export class TranslationUtils {

    public static registerTranslation(): ModuleWithProviders {
        return TranslateModule.forChild({
            defaultLanguage: 'bg',
            missingTranslationHandler: {
                provide: MissingTranslationHandler,
                useClass: AuthMissingTranslationHandler
            }
        });
    }
}

export class AuthMissingTranslationHandler extends MissingTranslationHandler {
    private defaultTranslationLoaded: boolean = false;

    public loadTranslation(translateService: TranslateService) {
        if (!this.defaultTranslationLoaded) {
            translateService.setTranslation(translateService.currentLang, auth_BG_resources, true);
            this.defaultTranslationLoaded = true;
        }
    }

    public handle(params: MissingTranslationHandlerParams) {
        this.loadTranslation(params.translateService);
    }

}
