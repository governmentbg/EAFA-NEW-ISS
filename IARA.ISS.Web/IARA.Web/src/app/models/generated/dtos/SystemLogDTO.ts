
import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class SystemLogDTO {
    public constructor(obj?: Partial<SystemLogDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Date)
    public logDate?: Date;

    @StrictlyTyped(String)
    public actionType?: string;

    @StrictlyTyped(String)
    public application?: string;

    @StrictlyTyped(String)
    public module?: string;

    @StrictlyTyped(String)
    public action?: string;

    @StrictlyTyped(String)
    public tableName?: string;

    @StrictlyTyped(String)
    public username?: string;

    @StrictlyTyped(String)
    public ipAddress?: string;

    @StrictlyTyped(String)
    public browserInfo?: string;
}