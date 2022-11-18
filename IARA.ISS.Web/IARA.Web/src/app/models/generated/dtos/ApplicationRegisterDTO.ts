

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { ApplicationsChangeHistoryDTO } from './ApplicationsChangeHistoryDTO';
import { ApplicationHierarchyTypesEnum } from '@app/enums/application-hierarchy-types.enum';
import { ApplicationStatusesEnum } from '@app/enums/application-statuses.enum';
import { PaymentStatusesEnum } from '@app/enums/payment-statuses.enum';
import { PageCodeEnum } from '@app/enums/page-code.enum';

export class ApplicationRegisterDTO { 
    public constructor(obj?: Partial<ApplicationRegisterDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(String)
    public accessCode?: string;

    @StrictlyTyped(String)
    public eventisNum?: string;

    @StrictlyTyped(Date)
    public submitDateTime?: Date;

    @StrictlyTyped(String)
    public submittedFor?: string;

    @StrictlyTyped(String)
    public type?: string;

    @StrictlyTyped(String)
    public sourceName?: string;

    @StrictlyTyped(Number)
    public sourceCode?: ApplicationHierarchyTypesEnum;

    @StrictlyTyped(String)
    public statusName?: string;

    @StrictlyTyped(Number)
    public statusCode?: ApplicationStatusesEnum;

    @StrictlyTyped(Number)
    public prevStatusCode?: ApplicationStatusesEnum;

    @StrictlyTyped(String)
    public statusReason?: string;

    @StrictlyTyped(String)
    public assignedUser?: string;

    @StrictlyTyped(Number)
    public assignedUserId?: number;

    @StrictlyTyped(Boolean)
    public isActive?: boolean;

    @StrictlyTyped(Number)
    public paymentStatus?: PaymentStatusesEnum;

    @StrictlyTyped(Number)
    public pageCode?: PageCodeEnum;

    @StrictlyTyped(ApplicationsChangeHistoryDTO)
    public changeHistoryRecords?: ApplicationsChangeHistoryDTO[];
}