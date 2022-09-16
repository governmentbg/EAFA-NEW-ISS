

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { RegixLegalDataDTO } from './RegixLegalDataDTO';
import { AddressRegistrationDTO } from './AddressRegistrationDTO';
import { FileInfoDTO } from './FileInfoDTO';

export class RecreationalFishingAssociationEditDTO { 
    public constructor(obj?: Partial<RecreationalFishingAssociationEditDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public territoryUnitId?: number;

    @StrictlyTyped(Boolean)
    public isAdding?: boolean;

    @StrictlyTyped(Number)
    public legalId?: number;

    @StrictlyTyped(RegixLegalDataDTO)
    public legal?: RegixLegalDataDTO;

    @StrictlyTyped(AddressRegistrationDTO)
    public legalAddresses?: AddressRegistrationDTO[];

    @StrictlyTyped(Boolean)
    public isCanceled?: boolean;

    @StrictlyTyped(Date)
    public cancellationDate?: Date;

    @StrictlyTyped(String)
    public cancellationReason?: string;

    @StrictlyTyped(FileInfoDTO)
    public files?: FileInfoDTO[];
}