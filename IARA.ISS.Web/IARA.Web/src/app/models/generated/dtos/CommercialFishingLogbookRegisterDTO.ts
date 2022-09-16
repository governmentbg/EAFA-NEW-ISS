

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class CommercialFishingLogbookRegisterDTO { 
    public constructor(obj?: Partial<CommercialFishingLogbookRegisterDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public logbookId?: number;

    @StrictlyTyped(String)
    public number?: string;

    @StrictlyTyped(String)
    public logBookTypeName?: string;

    @StrictlyTyped(Date)
    public issueDate?: Date;

    @StrictlyTyped(Date)
    public finishDate?: Date;

    @StrictlyTyped(String)
    public statusName?: string;

    @StrictlyTyped(Number)
    public startPageNumber?: number;

    @StrictlyTyped(Number)
    public endPageNumber?: number;

    @StrictlyTyped(Boolean)
    public isActive?: boolean;
}