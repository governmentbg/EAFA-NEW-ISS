

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { NomenclatureDTO } from './GenericNomenclatureDTO';
import { LogBookTypesEnum } from '@app/enums/log-book-types.enum';

export class InspectionShipLogBookDTO { 
    public constructor(obj?: Partial<InspectionShipLogBookDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(String)
    public number?: string;

    @StrictlyTyped(Date)
    public issuedOn?: Date;

    @StrictlyTyped(Number)
    public startPage?: number;

    @StrictlyTyped(Number)
    public endPage?: number;

    @StrictlyTyped(Number)
    public logBookType?: LogBookTypesEnum;

    @StrictlyTyped(NomenclatureDTO)
    public pages?: NomenclatureDTO<number>[];
}