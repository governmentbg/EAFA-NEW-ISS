import { Pipe, PipeTransform } from '@angular/core';
import { PrefixInputDTO } from '@app/models/generated/dtos/PrefixInputDTO';

@Pipe({
    name: 'tlPrefixInputDisplay'
})
export class TLPrefixInputDisplayPipe implements PipeTransform {

    public transform(value: PrefixInputDTO): string {
        if (value !== null && value !== undefined && typeof value === 'object') {
            const prefixInput: PrefixInputDTO = new PrefixInputDTO(value);

            if (prefixInput.prefix !== null && prefixInput.prefix !== undefined) {
                return `${prefixInput.prefix}${prefixInput.inputValue ?? ''}`;
            }
            else {
                return prefixInput.inputValue ?? '';
            }
        }
        else {
            return '';
        }
    }

}