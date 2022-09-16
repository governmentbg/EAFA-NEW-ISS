

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class RecreationalFishingAssociationDTO { 
    public constructor(obj?: Partial<RecreationalFishingAssociationDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(String)
    public name?: string;

    @StrictlyTyped(String)
    public territoryUnit?: string;

    @StrictlyTyped(String)
    public eik?: string;

    @StrictlyTyped(String)
    public phone?: string;

    @StrictlyTyped(Number)
    public membersCount?: number;

    @StrictlyTyped(Boolean)
    public isCanceled?: boolean;

    @StrictlyTyped(Boolean)
    public isActive?: boolean;
}