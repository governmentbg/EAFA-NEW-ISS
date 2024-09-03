

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { InspectedDeclarationCatchDTO } from './InspectedDeclarationCatchDTO';

export class InspectedLogBookPageDataDTO { 
    public constructor(obj?: Partial<InspectedLogBookPageDataDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public shipLogBookPageId?: number;

    @StrictlyTyped(String)
    public shipLogBookPageNumber?: string;

    @StrictlyTyped(Date)
    public shipPageFillDate?: Date;

    @StrictlyTyped(Number)
    public transportationLogBookPageId?: number;

    @StrictlyTyped(Number)
    public transportationLogBookPageNumber?: number;

    @StrictlyTyped(Date)
    public transportationPageLoadingDate?: Date;

    @StrictlyTyped(Number)
    public admissionLogBookPageId?: number;

    @StrictlyTyped(Number)
    public admissionLogBookPageNumber?: number;

    @StrictlyTyped(Date)
    public admissionPageHandoverDate?: Date;

    @StrictlyTyped(Number)
    public firstSaleLogBookPageId?: number;

    @StrictlyTyped(Number)
    public firstSaleLogBookPageNumber?: number;

    @StrictlyTyped(Date)
    public firstSalePageSaleDate?: Date;

    @StrictlyTyped(Number)
    public aquacultureLogBookPageId?: number;

    @StrictlyTyped(Number)
    public aquacultureLogBookPageNumber?: number;

    @StrictlyTyped(Date)
    public aquaculturePageFillingDate?: Date;

    @StrictlyTyped(InspectedDeclarationCatchDTO)
    public inspectionCatchMeasures?: InspectedDeclarationCatchDTO[];
}