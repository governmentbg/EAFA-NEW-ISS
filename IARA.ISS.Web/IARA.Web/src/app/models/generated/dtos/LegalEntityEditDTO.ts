

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { RegixLegalDataDTO } from './RegixLegalDataDTO';
import { AddressRegistrationDTO } from './AddressRegistrationDTO';
import { AuthorizedPersonDTO } from './AuthorizedPersonDTO';
import { FileInfoDTO } from './FileInfoDTO';

export class LegalEntityEditDTO { 
    public constructor(obj?: Partial<LegalEntityEditDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public applicationId?: number;

    @StrictlyTyped(Boolean)
    public isOnlineApplication?: boolean;

    @StrictlyTyped(RegixLegalDataDTO)
    public legal?: RegixLegalDataDTO;

    @StrictlyTyped(AddressRegistrationDTO)
    public addresses?: AddressRegistrationDTO[];

    @StrictlyTyped(AuthorizedPersonDTO)
    public authorizedPeople?: AuthorizedPersonDTO[];

    @StrictlyTyped(FileInfoDTO)
    public files?: FileInfoDTO[];
}