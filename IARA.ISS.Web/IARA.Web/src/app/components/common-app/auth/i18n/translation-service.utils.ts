import { ModuleWithProviders } from '@angular/compiler/src/core';
import { MissingTranslationHandler, MissingTranslationHandlerParams, TranslateModule, TranslateModuleConfig, TranslateService } from '@ngx-translate/core'
import { auth_BG_resources } from './auth-translations.bg';
import { StorageService } from '@app/shared/services/local-storage.service';
import { StorageTypes } from '@app/shared/enums/storage-types.enum';
import { auth_EN_resources } from './auth-translations.en';

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
        const local = StorageService.getStorage(StorageTypes.Local).get('lang')!.toString();
        const resources = local === 'bg' ? auth_BG_resources : auth_EN_resources;

        if (!this.defaultTranslationLoaded) {
            translateService.setTranslation(local, resources, true);
            this.defaultTranslationLoaded = true;
        }
    }

    public handle(params: MissingTranslationHandlerParams) {
        this.loadTranslation(params.translateService);
    }

}
