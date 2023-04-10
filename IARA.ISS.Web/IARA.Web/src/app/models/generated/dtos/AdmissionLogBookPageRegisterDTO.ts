

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { LogBookPageStatusesEnum } from '@app/enums/log-book-page-statuses.enum';

export class AdmissionLogBookPageRegisterDTO { 
    public constructor(obj?: Partial<AdmissionLogBookPageRegisterDTO>) {
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
    public isLogBookFinishedOrSuspended?: boolean;

    @StrictlyTyped(String)
    public acceptedPersonNames?: string;

    @StrictlyTyped(Date)
    public handoverDate?: Date;

    @StrictlyTyped(String)
    public storageLocation?: string;

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