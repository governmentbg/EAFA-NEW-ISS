

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { RegixPersonDataDTO } from './RegixPersonDataDTO';
import { RegixLegalDataDTO } from './RegixLegalDataDTO';
import { AddressRegistrationDTO } from './AddressRegistrationDTO';
import { LogBookPagePersonTypesEnum } from '@app/enums/log-book-page-person-types.enum';

export class LogBookPagePersonDTO { 
    public constructor(obj?: Partial<LogBookPagePersonDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public personType?: LogBookPagePersonTypesEnum;

    @StrictlyTyped(Number)
    public buyerId?: number;

    @StrictlyTyped(RegixPersonDataDTO)
    public person?: RegixPersonDataDTO;

    @StrictlyTyped(RegixLegalDataDTO)
    public personLegal?: RegixLegalDataDTO;

    @StrictlyTyped(AddressRegistrationDTO)
    public addresses?: AddressRegistrationDTO[];
}