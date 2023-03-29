import { LogBookTypesEnum } from '@app/enums/log-book-types.enum';
import { ICatchesAndSalesService } from '@app/interfaces/common-app/catches-and-sales.interface';
import { LogBookPageStatusesEnum } from '@app/enums/log-book-page-statuses.enum';

export class AddLogBookPageDialogParams {
    public logBookType!: LogBookTypesEnum;
    public service!: ICatchesAndSalesService;
    public logBookId!: number;
    public logBookTypeId!: number;
    public pageNumber: number | undefined;
    public pageStatus: LogBookPageStatusesEnum | undefined;

    public constructor(obj?: Partial<AddLogBookPageDialogParams>) {
        Object.assign(this, obj);
    }
}