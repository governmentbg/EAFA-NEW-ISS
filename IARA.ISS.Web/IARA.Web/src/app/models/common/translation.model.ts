import { Locale } from '@fuse/services/translation-loader.service';

export class Translation implements Locale {
    constructor(public lang: string, public data: any, public shouldMerge: boolean = false) { }
}