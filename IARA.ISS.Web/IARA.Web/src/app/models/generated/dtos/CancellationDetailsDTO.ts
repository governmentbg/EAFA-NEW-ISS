

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class CancellationDetailsDTO { 
    public constructor(obj?: Partial<CancellationDetailsDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public reasonId?: number;

    @StrictlyTyped(Date)
    public date?: Date;

    @StrictlyTyped(String)
    public issueOrderNum?: string;

    @StrictlyTyped(String)
    public description?: string;

    @StrictlyTyped(Boolean)
    public isActive?: boolean;
}