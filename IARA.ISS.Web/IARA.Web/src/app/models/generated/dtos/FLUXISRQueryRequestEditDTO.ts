

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class FLUXISRQueryRequestEditDTO { 
    public constructor(obj?: Partial<FLUXISRQueryRequestEditDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Date)
    public submittedDateTime?: Date;

    @StrictlyTyped(String)
    public inspectionType?: string;

    @StrictlyTyped(Date)
    public inspectionStart?: Date;

    @StrictlyTyped(Date)
    public inspectionEnd?: Date;
}