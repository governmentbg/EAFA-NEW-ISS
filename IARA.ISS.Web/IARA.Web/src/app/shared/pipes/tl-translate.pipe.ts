import { Pipe } from '@angular/core';
import { TranslatePipe } from '@ngx-translate/core';

export type TranslateParameter = 'cap'

@Pipe({ name: 'tlTranslate' })
export class TLTranslatePipe extends TranslatePipe {
    transform(value: string, ...params: TranslateParameter[]): string {
        let result: string = super.transform(value) as string;
        for (const param of params) {
            if (param === 'cap') {
                result = result[0].toUpperCase() + result.substr(1);
            }
        }
        return result;
    }
}