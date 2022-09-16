

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class AuanRegisterDTO { 
    public constructor(obj?: Partial<AuanRegisterDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public inspectionId?: number;

    @StrictlyTyped(String)
    public auanNum?: string;

    @StrictlyTyped(String)
    public inspectedEntity?: string;

    @StrictlyTyped(String)
    public drafter?: string;

    @StrictlyTyped(Date)
    public draftDate?: Date;

    @StrictlyTyped(Boolean)
    public isActive?: boolean;
}