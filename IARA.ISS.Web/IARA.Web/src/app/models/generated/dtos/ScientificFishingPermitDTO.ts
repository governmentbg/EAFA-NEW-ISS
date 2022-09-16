

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { ScientificFishingPermitHolderDTO } from './ScientificFishingPermitHolderDTO';
import { ScientificPermitStatusEnum } from '@app/enums/scientific-permit-status.enum';

export class ScientificFishingPermitDTO { 
    public constructor(obj?: Partial<ScientificFishingPermitDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public applicationId?: number;

    @StrictlyTyped(Number)
    public requestNumber?: number;

    @StrictlyTyped(String)
    public requesterName?: string;

    @StrictlyTyped(String)
    public scientificOrganizationName?: string;

    @StrictlyTyped(String)
    public permitReasons?: string;

    @StrictlyTyped(Date)
    public validTo?: Date;

    @StrictlyTyped(Number)
    public outingsCount?: number;

    @StrictlyTyped(Number)
    public permitStatus?: ScientificPermitStatusEnum;

    @StrictlyTyped(String)
    public permitStatusName?: string;

    @StrictlyTyped(Number)
    public deliveryId?: number;

    @StrictlyTyped(ScientificFishingPermitHolderDTO)
    public holders?: ScientificFishingPermitHolderDTO[];

    @StrictlyTyped(Boolean)
    public isActive?: boolean;
}