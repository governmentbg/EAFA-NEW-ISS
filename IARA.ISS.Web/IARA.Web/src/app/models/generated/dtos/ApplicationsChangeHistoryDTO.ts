
import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class ApplicationsChangeHistoryDTO {
    public constructor(obj?: Partial<ApplicationsChangeHistoryDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public applicationId?: number;

    @StrictlyTyped(String)
    public statusName?: string;

    @StrictlyTyped(String)
    public statusReason?: string;

    @StrictlyTyped(Date)
    public modifiedDate?: Date;

    @StrictlyTyped(Number)
    public modifiedByUserId?: number;

    @StrictlyTyped(String)
    public modifiedByUserName?: string;

    @StrictlyTyped(Number)
    public assignedUserId?: number;

    @StrictlyTyped(String)
    public assingedUserName?: string;

    @StrictlyTyped(String)
    public territoryUnitName?: string;

    @StrictlyTyped(String)
    public paymentStatus?: string;

    @StrictlyTyped(String)
    public paymentStatusCode?: string;

    @StrictlyTyped(Boolean)
    public hasApplicationDraftContent?: boolean;
}