

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { BuyerStatusesEnum } from '@app/enums/buyer-statuses.enum';
import { PageCodeEnum } from '@app/enums/page-code.enum';

export class BuyerDTO { 
    public constructor(obj?: Partial<BuyerDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public applicationId?: number;

    @StrictlyTyped(Number)
    public status?: BuyerStatusesEnum;

    @StrictlyTyped(Number)
    public pageCode?: PageCodeEnum;

    @StrictlyTyped(String)
    public submittedForName?: string;

    @StrictlyTyped(String)
    public subjectNames?: string;

    @StrictlyTyped(String)
    public buyerTypeName?: string;

    @StrictlyTyped(String)
    public buyerStatusName?: string;

    @StrictlyTyped(String)
    public urorrNumber?: string;

    @StrictlyTyped(String)
    public registrationNumber?: string;

    @StrictlyTyped(Date)
    public registrationDate?: Date;

    @StrictlyTyped(Boolean)
    public isActive?: boolean;

    @StrictlyTyped(String)
    public comments?: string;
}