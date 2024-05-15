import { ICatchesAndSalesService } from '@app/interfaces/common-app/catches-and-sales.interface';
import { LogBookTypesEnum } from '@app/enums/log-book-types.enum';
import { ShipNomenclatureDTO } from '@app/models/generated/dtos/ShipNomenclatureDTO';

export class EditPageNumberDilogParamsModel {
    public service!: ICatchesAndSalesService;
    public pageId!: number;
    public pageNumber!: number;
    public logBookType!: LogBookTypesEnum;
    public logBookId!: number;
    public ships: ShipNomenclatureDTO[] = [];

    public constructor(obj?: Partial<EditPageNumberDilogParamsModel>) {
        Object.assign(this, obj);
    }
}