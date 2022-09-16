
import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { NomenclatureDTO } from './GenericNomenclatureDTO';
import { LocationDTO } from './LocationDTO';

export class ReportViolationDTO {
    public constructor(obj?: Partial<ReportViolationDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(NomenclatureDTO)
    public signalType?: NomenclatureDTO<number>;

    @StrictlyTyped(String)
    public description?: string;

    @StrictlyTyped(String)
    public phone?: string;

    @StrictlyTyped(Date)
    public date?: Date;

    @StrictlyTyped(LocationDTO)
    public location?: LocationDTO;
}