import { Pipe, PipeTransform } from '@angular/core';

@Pipe({ name: 'keys' })
export class KeysPipe implements PipeTransform {
    /**
     * Transform
     *
     * @param value
     * @returns {any}
     */
    transform(value: any): any {
        const keys: any[] = [];

        for (const key in value) {
            if (value.hasOwnProperty(key)) {
                keys.push({
                    key: key,
                    value: value[key]
                });
            }
        }

        return keys;
    }
}