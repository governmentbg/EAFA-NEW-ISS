
import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class CatchRecordTicketDTO {
    public constructor(obj?: Partial<CatchRecordTicketDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(String)
    public typeName?: string;

    @StrictlyTyped(Date)
    public validFrom?: Date;

    @StrictlyTyped(Date)
    public validTo?: Date;

    @StrictlyTyped(String)
    public personFullName?: string;

    @StrictlyTyped(String)
    public statusName?: string;
}