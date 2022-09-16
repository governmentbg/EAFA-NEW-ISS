

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { NomenclatureDTO } from './GenericNomenclatureDTO';
import { InspectionToggleTypesEnum } from '@app/enums/inspection-toggle-types.enum';

export class InspectionLogBookDTO { 
    public constructor(obj?: Partial<InspectionLogBookDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public checkValue?: InspectionToggleTypesEnum;

    @StrictlyTyped(String)
    public description?: string;

    @StrictlyTyped(String)
    public number?: string;

    @StrictlyTyped(String)
    public pageNum?: string;

    @StrictlyTyped(Number)
    public logBookId?: number;

    @StrictlyTyped(Number)
    public pageId?: number;

    @StrictlyTyped(Date)
    public from?: Date;

    @StrictlyTyped(Number)
    public startPage?: number;

    @StrictlyTyped(Number)
    public endPage?: number;

    @StrictlyTyped(NomenclatureDTO)
    public pages?: NomenclatureDTO<number>[];
}