

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { StatisticalFormTypesEnum } from '@app/enums/statistical-form-types.enum';

export class StatisticalFormDTO { 
    public constructor(obj?: Partial<StatisticalFormDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public applicationId?: number;

    @StrictlyTyped(String)
    public registryNumber?: string;

    @StrictlyTyped(String)
    public processUser?: string;

    @StrictlyTyped(Number)
    public year?: number;

    @StrictlyTyped(Date)
    public submissionDate?: Date;

    @StrictlyTyped(String)
    public formObject?: string;

    @StrictlyTyped(String)
    public formTypeName?: string;

    @StrictlyTyped(Number)
    public formType?: StatisticalFormTypesEnum;

    @StrictlyTyped(Boolean)
    public isActive?: boolean;
}