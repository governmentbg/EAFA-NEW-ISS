

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { AddressRegistrationDTO } from './AddressRegistrationDTO';
import { AuanWitnessDTO } from './AuanWitnessDTO';
import { FileInfoDTO } from './FileInfoDTO';
import { InspDeliveryTypesEnum } from '@app/enums/insp-delivery-types.enum';
import { InspDeliveryConfirmationTypesEnum } from '@app/enums/insp-delivery-confirmation-types.enum';

export class AuanDeliveryDataDTO { 
    public constructor(obj?: Partial<AuanDeliveryDataDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public deliveryType?: InspDeliveryTypesEnum;

    @StrictlyTyped(Date)
    public sentDate?: Date;

    @StrictlyTyped(String)
    public referenceNum?: string;

    @StrictlyTyped(String)
    public deliveryReason?: string;

    @StrictlyTyped(String)
    public stateService?: string;

    @StrictlyTyped(Number)
    public territoryUnitId?: number;

    @StrictlyTyped(AddressRegistrationDTO)
    public address?: AddressRegistrationDTO;

    @StrictlyTyped(Boolean)
    public isDelivered?: boolean;

    @StrictlyTyped(Date)
    public deliveryDate?: Date;

    @StrictlyTyped(Number)
    public confirmationType?: InspDeliveryConfirmationTypesEnum;

    @StrictlyTyped(Boolean)
    public isEDeliveryRequested?: boolean;

    @StrictlyTyped(Date)
    public refusalDate?: Date;

    @StrictlyTyped(AuanWitnessDTO)
    public refusalWitnesses?: AuanWitnessDTO[];

    @StrictlyTyped(FileInfoDTO)
    public files?: FileInfoDTO[];
}