

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { PaymentTariffDTO } from './PaymentTariffDTO';

export class PaymentSummaryDTO { 
    public constructor(obj?: Partial<PaymentSummaryDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(PaymentTariffDTO)
    public tariffs?: PaymentTariffDTO[];

    @StrictlyTyped(Number)
    public totalPrice?: number;
}