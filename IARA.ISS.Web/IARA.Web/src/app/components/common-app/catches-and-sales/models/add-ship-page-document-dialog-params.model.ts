import { ICatchesAndSalesService } from '@app/interfaces/common-app/catches-and-sales.interface';
import { LogBookTypesEnum } from '@app/enums/log-book-types.enum';
import { LogBookPageDocumentTypesEnum } from '../enums/log-book-page-document-types.enum';

export class AddShipPageDocumentDialogParamsModel {
    public logBookPageId!: number;
    public service!: ICatchesAndSalesService;
    public documentType!: LogBookPageDocumentTypesEnum;
    public logBookType!: LogBookTypesEnum;
    public documentNumber: number | undefined;

    public constructor(obj?: Partial<AddShipPageDocumentDialogParamsModel>) {
        Object.assign(this, obj);
    }
}