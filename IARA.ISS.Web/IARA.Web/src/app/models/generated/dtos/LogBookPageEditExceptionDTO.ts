

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class LogBookPageEditExceptionDTO { 
    public constructor(obj?: Partial<LogBookPageEditExceptionDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public userId?: number;

    @StrictlyTyped(Number)
    public logBookTypeId?: number;

    @StrictlyTyped(Number)
    public logBookId?: number;

    @StrictlyTyped(Date)
    public exceptionActiveFrom?: Date;

    @StrictlyTyped(Date)
    public exceptionActiveTo?: Date;

    @StrictlyTyped(Date)
    public editPageFrom?: Date;

    @StrictlyTyped(Date)
    public editPageTo?: Date;
}