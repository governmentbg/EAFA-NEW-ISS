export interface ITranslateService {
    /**
     * Gets the string value for the given key from the loaded translations
     * @key - code of the translation resource
     */
    getValue(key: string): string;
}