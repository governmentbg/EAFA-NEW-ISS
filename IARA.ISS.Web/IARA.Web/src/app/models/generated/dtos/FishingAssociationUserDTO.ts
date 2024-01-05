

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { UserLegalStatusEnum } from '@app/enums/user-legal-status.enum';

export class FishingAssociationUserDTO { 
    public constructor(obj?: Partial<FishingAssociationUserDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public associationLegalId?: number;

    @StrictlyTyped(Number)
    public userId?: number;

    @StrictlyTyped(Number)
    public status?: UserLegalStatusEnum;

    @StrictlyTyped(String)
    public statusName?: string;

    @StrictlyTyped(Boolean)
    public isActive?: boolean;
}