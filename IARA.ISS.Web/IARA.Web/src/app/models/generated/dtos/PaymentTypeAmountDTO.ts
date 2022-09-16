
import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class PaymentTypeAmountDTO {
    public constructor(obj?: Partial<PaymentTypeAmountDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public paymentTypeId?: number;

    @StrictlyTyped(Number)
    public amount?: number;
}