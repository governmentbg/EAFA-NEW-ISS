

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { PageCodeEnum } from '@app/enums/page-code.enum';

export class TypesCountReportDTO { 
    public constructor(obj?: Partial<TypesCountReportDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(String)
    public icon?: string;

    @StrictlyTyped(String)
    public name?: string;

    @StrictlyTyped(Number)
    public pageCode?: PageCodeEnum;

    @StrictlyTyped(Number)
    public count?: number;

    @StrictlyTyped(Number)
    public typeId?: number;
}