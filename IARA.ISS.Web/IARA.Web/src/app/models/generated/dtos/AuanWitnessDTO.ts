

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { AddressRegistrationDTO } from './AddressRegistrationDTO';

export class AuanWitnessDTO { 
    public constructor(obj?: Partial<AuanWitnessDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public auanId?: number;

    @StrictlyTyped(Number)
    public inspDeliveryId?: number;

    @StrictlyTyped(String)
    public witnessNames?: string;

    @StrictlyTyped(Date)
    public dateOfBirth?: Date;

    @StrictlyTyped(Number)
    public addressId?: number;

    @StrictlyTyped(AddressRegistrationDTO)
    public address?: AddressRegistrationDTO;

    @StrictlyTyped(Number)
    public orderNum?: number;

    @StrictlyTyped(Boolean)
    public isActive?: boolean;
}