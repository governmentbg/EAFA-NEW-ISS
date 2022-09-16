
import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { RegixPersonDataDTO } from './RegixPersonDataDTO';
import { AddressRegistrationDTO } from './AddressRegistrationDTO';

export class PersonReportInfoDTO {
    public constructor(obj?: Partial<PersonReportInfoDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(String)
    public comments?: string;

    @StrictlyTyped(RegixPersonDataDTO)
    public regixPersonData?: RegixPersonDataDTO;

    @StrictlyTyped(AddressRegistrationDTO)
    public addressRegistrations?: AddressRegistrationDTO[];
}