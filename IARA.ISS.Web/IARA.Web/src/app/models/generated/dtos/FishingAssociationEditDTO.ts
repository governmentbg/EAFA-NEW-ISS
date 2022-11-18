

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { RegixLegalDataDTO } from './RegixLegalDataDTO';
import { AddressRegistrationDTO } from './AddressRegistrationDTO';
import { FishingAssociationPersonDTO } from './FishingAssociationPersonDTO';
import { FileInfoDTO } from './FileInfoDTO';

export class FishingAssociationEditDTO { 
    public constructor(obj?: Partial<FishingAssociationEditDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public applicationId?: number;

    @StrictlyTyped(Boolean)
    public isOnlineApplication?: boolean;

    @StrictlyTyped(RegixLegalDataDTO)
    public submittedFor?: RegixLegalDataDTO;

    @StrictlyTyped(AddressRegistrationDTO)
    public submittedForAddresses?: AddressRegistrationDTO[];

    @StrictlyTyped(FishingAssociationPersonDTO)
    public persons?: FishingAssociationPersonDTO[];

    @StrictlyTyped(Number)
    public territoryUnitId?: number;

    @StrictlyTyped(FileInfoDTO)
    public files?: FileInfoDTO[];
}