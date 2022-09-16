import { Pipe, PipeTransform } from '@angular/core';

@Pipe({ name: 'htmlToPlaintext' })
export class HtmlToPlaintextPipe implements PipeTransform {
    /**
     * Transform
     *
     * @param {string} value
     * @returns {string}
     */
    transform(value: string): string {
        return value ? String(value).replace(/<[^>]+>/gm, '') : '';
    }
}