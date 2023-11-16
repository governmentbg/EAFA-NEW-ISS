

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { SystemLogDTO } from './SystemLogDTO';

export class BaseSystemLogDTO { 
    public constructor(obj?: Partial<BaseSystemLogDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(String)
    public application?: string;

    @StrictlyTyped(String)
    public module?: string;

    @StrictlyTyped(String)
    public action?: string;

    @StrictlyTyped(String)
    public username?: string;

    @StrictlyTyped(String)
    public ipAddress?: string;

    @StrictlyTyped(String)
    public browserInfo?: string;

    @StrictlyTyped(Date)
    public logDate?: Date;

    @StrictlyTyped(String)
    public eventUID?: string;

    @StrictlyTyped(String)
    public tableId?: string;

    @StrictlyTyped(SystemLogDTO)
    public systemLogs?: SystemLogDTO[];
}