import { Pipe } from '@angular/core';

@Pipe({ name: 'tlCoordinates' })
export class TLCoordinatesPipe {
    transform(value: string): string {
        if (value != undefined && value != null && value != '') {
            value = value.trim();
            if (value != '') {
                const values: string[] = value.split(' ');
                return `${values[0]}° ${values[1]}' ${Number(parseFloat(values[2]).toFixed(2))}"`;
            } else {
                return '';
            }
        } else {
            return '';
        }
    }
}