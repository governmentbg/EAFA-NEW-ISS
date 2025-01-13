import { PageCodeEnum } from '@app/enums/page-code.enum';
import { ICatchesAndSalesService } from '@app/interfaces/common-app/catches-and-sales.interface';
import { LogBookTypesEnum } from '@app/enums/log-book-types.enum';

export class LogBookPageFilesDialogParamsModel {
    public logBookPageId!: number;
    public logBookType!: LogBookTypesEnum;
    public pageCode!: PageCodeEnum;
    public service!: ICatchesAndSalesService;

    public constructor(obj?: Partial<LogBookPageFilesDialogParamsModel>) {
        Object.assign(this, obj);
    }
}