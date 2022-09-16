

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { StatisticalFormRawMaterialOriginEnum } from '@app/enums/statistical-form-raw-material-origin.enum';

export class StatisticalFormReworkRawMaterialDTO { 
    public constructor(obj?: Partial<StatisticalFormReworkRawMaterialDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public fishTypeId?: number;

    @StrictlyTyped(Number)
    public tons?: number;

    @StrictlyTyped(Number)
    public origin?: StatisticalFormRawMaterialOriginEnum;

    @StrictlyTyped(Boolean)
    public isActive?: boolean;

    @StrictlyTyped(String)
    public countryZone?: string;
}