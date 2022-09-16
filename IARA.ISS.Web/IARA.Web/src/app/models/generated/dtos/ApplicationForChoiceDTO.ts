

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { PageCodeEnum } from '@app/enums/page-code.enum';

export class ApplicationForChoiceDTO { 
    public constructor(obj?: Partial<ApplicationForChoiceDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public pageCode?: PageCodeEnum;

    @StrictlyTyped(String)
    public eventisNumber?: string;

    @StrictlyTyped(Date)
    public submitDateTime?: Date;

    @StrictlyTyped(String)
    public submittedBy?: string;

    @StrictlyTyped(String)
    public submittedFor?: string;

    @StrictlyTyped(String)
    public type?: string;

    @StrictlyTyped(Boolean)
    public isChecked?: boolean;
}