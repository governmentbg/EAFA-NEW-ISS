

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { InspectionObservationCategoryEnum } from '@app/enums/inspection-observation-category.enum';

export class InspectionObservationTextDTO { 
    public constructor(obj?: Partial<InspectionObservationTextDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(String)
    public text?: string;

    @StrictlyTyped(Number)
    public category?: InspectionObservationCategoryEnum;
}