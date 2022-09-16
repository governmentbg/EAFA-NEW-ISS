import { ICatchesAndSalesService } from '@app/interfaces/common-app/catches-and-sales.interface';
import { LogBookPageDocumentTypesEnum } from '../enums/log-book-page-document-types.enum';

export class AddShipPageDocumentDialogParamsModel {
    public shipLogBookPageId!: number;
    public service!: ICatchesAndSalesService;
    public documentType!: LogBookPageDocumentTypesEnum;

    public constructor(obj?: Partial<AddShipPageDocumentDialogParamsModel>) {
        Object.assign(this, obj);
    }
}