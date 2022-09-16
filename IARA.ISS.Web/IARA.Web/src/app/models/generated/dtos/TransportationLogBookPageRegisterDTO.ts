

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { LogBookPageStatusesEnum } from '@app/enums/log-book-page-statuses.enum';

export class TransportationLogBookPageRegisterDTO { 
    public constructor(obj?: Partial<TransportationLogBookPageRegisterDTO>) {
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
    public vehicleNumber?: string;

    @StrictlyTyped(String)
    public deliveryLocation?: string;

    @StrictlyTyped(String)
    public recieverName?: string;

    @StrictlyTyped(Date)
    public loadingDate?: Date;

    @StrictlyTyped(String)
    public productsInformation?: string;

    @StrictlyTyped(Number)
    public status?: LogBookPageStatusesEnum;

    @StrictlyTyped(String)
    public statusName?: string;

    @StrictlyTyped(Boolean)
    public consistsOriginProducts?: boolean;

    @StrictlyTyped(String)
    public cancellationReason?: string;

    @StrictlyTyped(Boolean)
    public isActive?: boolean;
}