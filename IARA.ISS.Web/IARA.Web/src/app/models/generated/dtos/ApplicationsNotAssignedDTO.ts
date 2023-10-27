

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class ApplicationsNotAssignedDTO { 
    public constructor(obj?: Partial<ApplicationsNotAssignedDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public paperApplicationsCount?: number;

    @StrictlyTyped(Number)
    public onlineApplicationsCount?: number;
}