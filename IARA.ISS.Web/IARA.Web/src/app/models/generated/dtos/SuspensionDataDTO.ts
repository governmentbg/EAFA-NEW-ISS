

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class SuspensionDataDTO { 
    public constructor(obj?: Partial<SuspensionDataDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public suspensionTypeId?: number;

    @StrictlyTyped(String)
    public suspensionTypeName?: string;

    @StrictlyTyped(Date)
    public validFrom?: Date;

    @StrictlyTyped(Date)
    public validTo?: Date;

    @StrictlyTyped(Number)
    public reasonId?: number;

    @StrictlyTyped(String)
    public reasonName?: string;

    @StrictlyTyped(Date)
    public enactmentDate?: Date;

    @StrictlyTyped(String)
    public orderNumber?: string;

    @StrictlyTyped(Boolean)
    public isActive?: boolean;
}