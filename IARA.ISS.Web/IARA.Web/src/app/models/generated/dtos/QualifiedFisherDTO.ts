

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { PageCodeEnum } from '@app/enums/page-code.enum';

export class QualifiedFisherDTO { 
    public constructor(obj?: Partial<QualifiedFisherDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public applicationId?: number;

    @StrictlyTyped(Number)
    public pageCode?: PageCodeEnum;

    @StrictlyTyped(String)
    public registrationNum?: string;

    @StrictlyTyped(Date)
    public registrationDate?: Date;

    @StrictlyTyped(String)
    public name?: string;

    @StrictlyTyped(Boolean)
    public isWithMaritimeEducation?: boolean;

    @StrictlyTyped(String)
    public diplomaOrExamLabel?: string;

    @StrictlyTyped(String)
    public diplomaOrExamNumber?: string;

    @StrictlyTyped(Number)
    public deliveryId?: number;

    @StrictlyTyped(Boolean)
    public isActive?: boolean;
}