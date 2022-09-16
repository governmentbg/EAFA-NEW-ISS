
import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class UserTicketBaseDTO {
    public constructor(obj?: Partial<UserTicketBaseDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(String)
    public type?: string;

    @StrictlyTyped(String)
    public typeName?: string;

    @StrictlyTyped(Date)
    public validFrom?: Date;

    @StrictlyTyped(Date)
    public validTo?: Date;

    @StrictlyTyped(String)
    public personFullName?: string;

    @StrictlyTyped(String)
    public statusCode?: string;

    @StrictlyTyped(String)
    public statusName?: string;
}