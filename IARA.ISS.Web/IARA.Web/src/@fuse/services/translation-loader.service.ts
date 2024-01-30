import { Injectable } from '@angular/core';
import { ITranslationService } from '@app/shared/interfaces/translate-service.interface';

import { TranslateService } from '@ngx-translate/core';

export interface Locale {
    lang: string;
    data: Record<string, string>;
    shouldMerge: boolean;
}

@Injectable({
    providedIn: 'root'
})
export class FuseTranslationLoaderService implements ITranslationService {
    /**
     * Constructor
     *
     * @param {TranslateService} _translateService
     */
    constructor(
        private _translateService: TranslateService
    ) {
    }

    // -----------------------------------------------------------------------------------------------------
    // @ Public methods
    // -----------------------------------------------------------------------------------------------------

    /**
     * Load translations
     *
     * @param {Locale} args
     */
    loadTranslations(...args: Locale[]): void {
        const locales = [...args];
        locales.forEach((locale) => {
            this._translateService.setTranslation(locale.lang, locale.data, locale.shouldMerge);
        });
    }

    /**
     * Gets the string value for the given key from the loaded translations
     * @key - code of the translation
     */
    public getValue(key: string): string {
        if (key?.length > 0) {
            return this._translateService.instant(key);
        }
        else {
            return '';
        }
    }
}