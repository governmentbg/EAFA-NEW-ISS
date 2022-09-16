
import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { PaymentTypeAmountDTO } from './PaymentTypeAmountDTO';

export class RecreationalFishingReportResultDTO {
    public constructor(obj?: Partial<RecreationalFishingReportResultDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(PaymentTypeAmountDTO)
    public paymentTypeAmounts?: PaymentTypeAmountDTO[];

    @StrictlyTyped(Number)
    public annulledTicketsCount?: number;

    @StrictlyTyped(Number)
    public duplicateTicketsIssued?: number;

    @StrictlyTyped(Number)
    public duplicateTicketsAmount?: number;
}