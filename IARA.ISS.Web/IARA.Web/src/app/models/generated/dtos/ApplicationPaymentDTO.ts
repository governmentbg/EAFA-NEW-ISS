

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { PaymentTypesEnum } from '@app/enums/payment-types.enum';
import { PaymentStatusesEnum } from '@app/enums/payment-statuses.enum';

export class ApplicationPaymentDTO { 
    public constructor(obj?: Partial<ApplicationPaymentDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public paymentType?: PaymentTypesEnum;

    @StrictlyTyped(Date)
    public paymentDateTime?: Date;

    @StrictlyTyped(String)
    public paymentRefNumber?: string;

    @StrictlyTyped(Number)
    public paymentStatus?: PaymentStatusesEnum;
}