

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { RegixPersonDataDTO } from './RegixPersonDataDTO';
import { RegixLegalDataDTO } from './RegixLegalDataDTO';
import { AddressRegistrationDTO } from './AddressRegistrationDTO';
import { ChangeOfCircumstancesDataTypeEnum } from '@app/enums/change-of-circumstances-data-type.enum';

export class ChangeOfCircumstancesDTO { 
    public constructor(obj?: Partial<ChangeOfCircumstancesDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public typeId?: number;

    @StrictlyTyped(Number)
    public dataType?: ChangeOfCircumstancesDataTypeEnum;

    @StrictlyTyped(RegixPersonDataDTO)
    public person?: RegixPersonDataDTO;

    @StrictlyTyped(RegixLegalDataDTO)
    public legal?: RegixLegalDataDTO;

    @StrictlyTyped(AddressRegistrationDTO)
    public address?: AddressRegistrationDTO;

    @StrictlyTyped(Number)
    public shipId?: number;

    @StrictlyTyped(Number)
    public aquacultureFacilityId?: number;

    @StrictlyTyped(Number)
    public permitId?: number;

    @StrictlyTyped(Number)
    public permitLicenceId?: number;

    @StrictlyTyped(Number)
    public buyerId?: number;

    @StrictlyTyped(String)
    public description?: string;
}