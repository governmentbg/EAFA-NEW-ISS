
import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class RecreationalFishingTicketValidToCalculationParamsDTO {
    public constructor(obj?: Partial<RecreationalFishingTicketValidToCalculationParamsDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public typeId?: number;

    @StrictlyTyped(Number)
    public periodId?: number;

    @StrictlyTyped(Date)
    public validFrom?: Date;

    @StrictlyTyped(Date)
    public birthDate?: Date;

    @StrictlyTyped(Date)
    public telkValidTo?: Date;
}