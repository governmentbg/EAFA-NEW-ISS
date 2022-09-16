
import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { FishingTicketPersonDTO } from './FishingTicketPersonDTO';

export class UserTicketDTO {
    public constructor(obj?: Partial<UserTicketDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(String)
    public typeCode?: string;

    @StrictlyTyped(String)
    public periodCode?: string;

    @StrictlyTyped(Date)
    public validFrom?: Date;

    @StrictlyTyped(Date)
    public validTo?: Date;

    @StrictlyTyped(FishingTicketPersonDTO)
    public person?: FishingTicketPersonDTO;

    @StrictlyTyped(FishingTicketPersonDTO)
    public representativePerson?: FishingTicketPersonDTO;

    @StrictlyTyped(Boolean)
    public telkIsIndefinite?: boolean;

    @StrictlyTyped(String)
    public telkNum?: string;

    @StrictlyTyped(Date)
    public telkValidTo?: Date;

    @StrictlyTyped(Number)
    public associationId?: number;

    @StrictlyTyped(String)
    public membershipCardId?: string;

    @StrictlyTyped(Date)
    public membershipCardCreatedOn?: Date;
}