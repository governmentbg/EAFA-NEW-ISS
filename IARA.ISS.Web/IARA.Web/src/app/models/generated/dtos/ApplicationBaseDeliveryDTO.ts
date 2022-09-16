

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { AddressRegistrationDTO } from './AddressRegistrationDTO';

export class ApplicationBaseDeliveryDTO { 
    public constructor(obj?: Partial<ApplicationBaseDeliveryDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public deliveryTypeId?: number;

    @StrictlyTyped(AddressRegistrationDTO)
    public deliveryAddress?: AddressRegistrationDTO;

    @StrictlyTyped(Number)
    public deliveryTeritorryUnitId?: number;

    @StrictlyTyped(String)
    public deliveryEmailAddress?: string;
}