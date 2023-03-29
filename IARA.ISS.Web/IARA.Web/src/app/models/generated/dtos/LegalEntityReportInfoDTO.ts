

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { AddressRegistrationDTO } from './AddressRegistrationDTO';

export class LegalEntityReportInfoDTO { 
    public constructor(obj?: Partial<LegalEntityReportInfoDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(String)
    public legalName?: string;

    @StrictlyTyped(String)
    public eik?: string;

    @StrictlyTyped(String)
    public email?: string;

    @StrictlyTyped(String)
    public phone?: string;

    @StrictlyTyped(String)
    public postalCode?: string;

    @StrictlyTyped(AddressRegistrationDTO)
    public correspondenceAddress?: AddressRegistrationDTO;

    @StrictlyTyped(AddressRegistrationDTO)
    public courtRegistrationAddress?: AddressRegistrationDTO;

    @StrictlyTyped(AddressRegistrationDTO)
    public companyHeadquartersAddress?: AddressRegistrationDTO;
}