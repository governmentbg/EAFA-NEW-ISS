

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { LogBookPageStatusesEnum } from '@app/enums/log-book-page-statuses.enum';

export class AquacultureLogBookPageRegisterDTO { 
    public constructor(obj?: Partial<AquacultureLogBookPageRegisterDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public logBookId?: number;

    @StrictlyTyped(Number)
    public logBookTypeId?: number;

    @StrictlyTyped(Number)
    public pageNumber?: number;

    @StrictlyTyped(Boolean)
    public isLogBookFinished?: boolean;

    @StrictlyTyped(Date)
    public fillingDate?: Date;

    @StrictlyTyped(String)
    public buyerName?: string;

    @StrictlyTyped(String)
    public productionInformation?: string;

    @StrictlyTyped(Number)
    public status?: LogBookPageStatusesEnum;

    @StrictlyTyped(String)
    public statusName?: string;

    @StrictlyTyped(String)
    public cancellationReason?: string;

    @StrictlyTyped(Number)
    public personBuyerId?: number;

    @StrictlyTyped(Number)
    public legalBuyerId?: number;

    @StrictlyTyped(Boolean)
    public isActive?: boolean;
}