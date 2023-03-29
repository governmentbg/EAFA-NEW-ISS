

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class LogBookPageEditExceptionRegisterDTO { 
    public constructor(obj?: Partial<LogBookPageEditExceptionRegisterDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(String)
    public userNames?: string;

    @StrictlyTyped(String)
    public logBookTypeName?: string;

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