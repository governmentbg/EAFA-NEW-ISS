

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { LogBookPageStatusesEnum } from '@app/enums/log-book-page-statuses.enum';

export class FirstSaleLogBookPageRegisterDTO { 
    public constructor(obj?: Partial<FirstSaleLogBookPageRegisterDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public logBookId?: number;

    @StrictlyTyped(Number)
    public pageNumber?: number;

    @StrictlyTyped(Boolean)
    public isLogBookFinished?: boolean;

    @StrictlyTyped(String)
    public buyerNames?: string;

    @StrictlyTyped(String)
    public saleLocation?: string;

    @StrictlyTyped(Date)
    public saleDate?: Date;

    @StrictlyTyped(String)
    public productsInformation?: string;

    @StrictlyTyped(Number)
    public status?: LogBookPageStatusesEnum;

    @StrictlyTyped(String)
    public statusName?: string;

    @StrictlyTyped(String)
    public cancellationReason?: string;

    @StrictlyTyped(Boolean)
    public isActive?: boolean;
}