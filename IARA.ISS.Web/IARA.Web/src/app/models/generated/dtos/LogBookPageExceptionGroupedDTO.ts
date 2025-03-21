

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class LogBookPageExceptionGroupedDTO { 
    public constructor(obj?: Partial<LogBookPageExceptionGroupedDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public logBookPageExceptionIds?: number[];

    @StrictlyTyped(String)
    public userNames?: string;

    @StrictlyTyped(String)
    public logBookTypeNames?: string;

    @StrictlyTyped(String)
    public logBookNumber?: string;

    @StrictlyTyped(Boolean)
    public isValidNow?: boolean;

    @StrictlyTyped(Date)
    public exceptionActiveFrom?: Date;

    @StrictlyTyped(Date)
    public exceptionActiveTo?: Date;

    @StrictlyTyped(Date)
    public editPageFrom?: Date;

    @StrictlyTyped(Date)
    public editPageTo?: Date;

    @StrictlyTyped(Boolean)
    public isActive?: boolean;
}