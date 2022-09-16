

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { AquacultureInstallationNetCageShapesEnum } from '@app/enums/aquaculture-installation-net-cage-shapes.enum';

export class AquacultureInstallationNetCageDTO { 
    public constructor(obj?: Partial<AquacultureInstallationNetCageDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public netCageTypeId?: number;

    @StrictlyTyped(Number)
    public shape?: AquacultureInstallationNetCageShapesEnum;

    @StrictlyTyped(Number)
    public count?: number;

    @StrictlyTyped(Number)
    public radius?: number;

    @StrictlyTyped(Number)
    public length?: number;

    @StrictlyTyped(Number)
    public width?: number;

    @StrictlyTyped(Number)
    public height?: number;

    @StrictlyTyped(Number)
    public area?: number;

    @StrictlyTyped(Number)
    public volume?: number;

    @StrictlyTyped(Boolean)
    public isActive?: boolean;
}