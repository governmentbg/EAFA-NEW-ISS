

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { InspectedPersonTypeEnum } from '@app/enums/inspected-person-type.enum';

export class InspectedEntityEmailDTO { 
    public constructor(obj?: Partial<InspectedEntityEmailDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public inspectionId?: number;

    @StrictlyTyped(String)
    public name?: string;

    @StrictlyTyped(String)
    public firstName?: string;

    @StrictlyTyped(String)
    public lastName?: string;

    @StrictlyTyped(String)
    public email?: string;

    @StrictlyTyped(Number)
    public personId?: number;

    @StrictlyTyped(Number)
    public legalId?: number;

    @StrictlyTyped(Number)
    public type?: InspectedPersonTypeEnum;
}