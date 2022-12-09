

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { PaymentTypesEnum } from '@app/enums/payment-types.enum';

export class PaymentDataDTO { 
    public constructor(obj?: Partial<PaymentDataDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public paymentType?: PaymentTypesEnum;

    @StrictlyTyped(Date)
    public paymentDateTime?: Date;

    @StrictlyTyped(String)
    public paymentRefNumber?: string;

    @StrictlyTyped(Number)
    public totalPaidPrice?: number;
}