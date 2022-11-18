

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class FishingAssociationPersonErrorDTO { 
    public constructor(obj?: Partial<FishingAssociationPersonErrorDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(String)
    public egnLnc?: string;

    @StrictlyTyped(String)
    public email?: string;

    @StrictlyTyped(Boolean)
    public egnAndEmailDontMatch?: boolean;
}