import { ICatchesAndSalesService } from '@app/interfaces/common-app/catches-and-sales.interface';
import { LogBookPageProductDTO } from '@app/models/generated/dtos/LogBookPageProductDTO';
import { LogBookTypesEnum } from '@app/enums/log-book-types.enum';

export class EditLogBookPageProductDialogParamsModel {
    public model: LogBookPageProductDTO | undefined;
    public service!: ICatchesAndSalesService;
    public viewMode: boolean = true;
    public hasPrice: boolean = true;
    public logBookType!: LogBookTypesEnum;

    public constructor(obj?: Partial<EditLogBookPageProductDialogParamsModel>) {
        Object.assign(this, obj);
    }
}