

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class AquacultureInstallationDamDTO { 
    public constructor(obj?: Partial<AquacultureInstallationDamDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public area?: number;
}