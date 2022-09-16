

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { LogBookTypesEnum } from '@app/enums/log-book-types.enum';

export class LogBookPageProductDTO { 
    public constructor(obj?: Partial<LogBookPageProductDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public fishId?: number;

    @StrictlyTyped(Number)
    public originDeclarationFishId?: number;

    @StrictlyTyped(Number)
    public originProductId?: number;

    @StrictlyTyped(Number)
    public logBookType?: LogBookTypesEnum;

    @StrictlyTyped(String)
    public catchLocation?: string;

    @StrictlyTyped(Number)
    public productPresentationId?: number;

    @StrictlyTyped(Number)
    public productFreshnessId?: number;

    @StrictlyTyped(Number)
    public productPurposeId?: number;

    @StrictlyTyped(Number)
    public minSize?: number;

    @StrictlyTyped(Number)
    public averageUnitWeightKg?: number;

    @StrictlyTyped(Number)
    public fishSizeCategoryId?: number;

    @StrictlyTyped(Number)
    public quantityKg?: number;

    @StrictlyTyped(Number)
    public unitPrice?: number;

    @StrictlyTyped(String)
    public totalPrice?: string;

    @StrictlyTyped(Number)
    public turbotSizeGroupId?: number;

    @StrictlyTyped(Number)
    public unitCount?: number;

    @StrictlyTyped(Boolean)
    public hasMissingProperties?: boolean;

    @StrictlyTyped(Boolean)
    public isActive?: boolean;
}