

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class AquacultureInstallationRaftDTO { 
    public constructor(obj?: Partial<AquacultureInstallationRaftDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public length?: number;

    @StrictlyTyped(Number)
    public width?: number;

    @StrictlyTyped(Number)
    public area?: number;

    @StrictlyTyped(Number)
    public count?: number;

    @StrictlyTyped(Boolean)
    public isActive?: boolean;
}