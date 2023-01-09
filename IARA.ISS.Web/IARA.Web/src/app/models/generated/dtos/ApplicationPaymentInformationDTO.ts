

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { PaymentSummaryDTO } from './PaymentSummaryDTO';

export class ApplicationPaymentInformationDTO { 
    public constructor(obj?: Partial<ApplicationPaymentInformationDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(String)
    public paymentType?: string;

    @StrictlyTyped(Date)
    public paymentDate?: Date;

    @StrictlyTyped(String)
    public paymentStatus?: string;

    @StrictlyTyped(String)
    public referenceNumber?: string;

    @StrictlyTyped(Date)
    public lastUpdateDate?: Date;

    @StrictlyTyped(Boolean)
    public hasCalculatedTariffs?: boolean;

    @StrictlyTyped(Number)
    public totalPaidPrice?: number;

    @StrictlyTyped(PaymentSummaryDTO)
    public paymentSummary?: PaymentSummaryDTO;
}