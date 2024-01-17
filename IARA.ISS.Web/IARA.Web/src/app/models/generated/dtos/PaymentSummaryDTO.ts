

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { PaymentTariffDTO } from './PaymentTariffDTO';

export class PaymentSummaryDTO { 
    public constructor(obj?: Partial<PaymentSummaryDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(String)
    public paymentRequestNum?: string;

    @StrictlyTyped(PaymentTariffDTO)
    public tariffs?: PaymentTariffDTO[];

    @StrictlyTyped(Number)
    public totalPrice?: number;

    @StrictlyTyped(Boolean)
    public hasCalculatedTariffs?: boolean;

    @StrictlyTyped(Number)
    public totalPaidPrice?: number;
}