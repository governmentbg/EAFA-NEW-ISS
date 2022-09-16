
import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class FishingTicketApplicationDTO {
    public constructor(obj?: Partial<FishingTicketApplicationDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public applicationId?: number;
}