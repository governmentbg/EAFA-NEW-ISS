import { ColumnDTO } from '@app/models/generated/dtos/ColumnDTO';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';

export class ExtendedColumn extends ColumnDTO {
    public constructor(obj?: Partial<ColumnDTO>) {
        super(obj);
    }

    public options?: NomenclatureDTO<number>[];
}