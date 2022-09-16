

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class RecreationalFishingMembershipCardDTO { 
    public constructor(obj?: Partial<RecreationalFishingMembershipCardDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public associationId?: number;

    @StrictlyTyped(String)
    public cardNum?: string;

    @StrictlyTyped(Date)
    public issueDate?: Date;
}