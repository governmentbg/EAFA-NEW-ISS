
import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class SimpleAuditDTO {
    public constructor(obj?: Partial<SimpleAuditDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(String)
    public createdBy?: string;

    @StrictlyTyped(Date)
    public createdOn?: Date;

    @StrictlyTyped(String)
    public updatedBy?: string;

    @StrictlyTyped(Date)
    public updatedOn?: Date;
}