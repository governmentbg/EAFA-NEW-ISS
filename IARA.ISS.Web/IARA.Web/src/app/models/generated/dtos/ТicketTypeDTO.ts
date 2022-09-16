
import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class TicketTypeDTO {
    public constructor(obj?: Partial<TicketTypeDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(String)
    public code?: string;

    @StrictlyTyped(String)
    public name?: string;

    @StrictlyTyped(Boolean)
    public isActive?: boolean;
}