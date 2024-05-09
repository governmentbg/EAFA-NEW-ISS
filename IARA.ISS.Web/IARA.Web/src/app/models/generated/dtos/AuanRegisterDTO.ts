

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { AuanStatusEnum } from '@app/enums/auan-status.enum';

export class AuanRegisterDTO { 
    public constructor(obj?: Partial<AuanRegisterDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public inspectionId?: number;

    @StrictlyTyped(String)
    public territoryUnit?: string;

    @StrictlyTyped(String)
    public auanNum?: string;

    @StrictlyTyped(String)
    public inspectedEntity?: string;

    @StrictlyTyped(String)
    public drafter?: string;

    @StrictlyTyped(Number)
    public status?: AuanStatusEnum;

    @StrictlyTyped(String)
    public statusName?: string;

    @StrictlyTyped(Date)
    public draftDate?: Date;

    @StrictlyTyped(Number)
    public deliveryId?: number;

    @StrictlyTyped(String)
    public createdByUser?: string;

    @StrictlyTyped(Boolean)
    public lockedForCorrections?: boolean;

    @StrictlyTyped(Boolean)
    public isActive?: boolean;
}