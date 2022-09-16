

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class StatisticalFormReworkProductDTO { 
    public constructor(obj?: Partial<StatisticalFormReworkProductDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public productTypeId?: number;

    @StrictlyTyped(Boolean)
    public isNewProductType?: boolean;

    @StrictlyTyped(String)
    public productTypeName?: string;

    @StrictlyTyped(Number)
    public tons?: number;

    @StrictlyTyped(Boolean)
    public isActive?: boolean;
}