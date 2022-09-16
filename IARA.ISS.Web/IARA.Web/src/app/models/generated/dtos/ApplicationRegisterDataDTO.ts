

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { RecordTypesEnum } from '@app/enums/record-types.enum';

export class ApplicationRegisterDataDTO { 
    public constructor(obj?: Partial<ApplicationRegisterDataDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public recordType?: RecordTypesEnum;

    @StrictlyTyped(Number)
    public applicationId?: number;
}