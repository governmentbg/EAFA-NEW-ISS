import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
    name: 'tlJoin'
})
export class TLJoinPipe implements PipeTransform {
    public transform(input: Array<any>, separator: string = ','): string {
        if (input === null || input === undefined || Array.isArray(input) === false) {
            return '';
        }

        return input.join(separator);
    }
}