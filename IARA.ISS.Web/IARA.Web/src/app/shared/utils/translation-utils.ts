import { HttpClient } from '@angular/common/http';
import { TranslateLoader } from '@ngx-translate/core';
import { AppInjector } from '../../app.module';
import { Translation } from '../../models/common/translation.model';
import { RequestService } from '../services/request.service';
import { FileTranslateLoader } from './http-file-translation.loader';
import { WebTranslateLoader } from './http-translation.loader';
import { locale as bulgarian } from '@app/i18n/bg';
import { locale as english } from '@app/i18n/en';

export class TranslationUtils {
    public static getFileTranslationLoader(): TranslateLoader {
        const httpClient = AppInjector.get(HttpClient);
        return new FileTranslateLoader(httpClient);
    }


    public static getWebTranslationLoader(): TranslateLoader {
        const requestService = AppInjector.get(RequestService);
        return new WebTranslateLoader(requestService);
    }

    public static async getTranslationsFromLoader(translationLoader: TranslateLoader, language: string): Promise<Translation | undefined> {
        const translation = await translationLoader.getTranslation(language).toPromise();
        if (translation != undefined) {
            return new Translation(language, translation, true);
        } else {
            return undefined;
        }
    }

    public static getLocalTranslations(language: string): Translation {
        if (language === 'bg') {
            return new Translation(language, bulgarian, true);
        }
        else {
            return new Translation(language, english, true);
        }
    }
}