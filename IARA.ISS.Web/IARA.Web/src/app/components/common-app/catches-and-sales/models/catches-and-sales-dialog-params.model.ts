import { ICatchesAndSalesService } from '@app/interfaces/common-app/catches-and-sales.interface';
import { CommonLogBookPageDataDTO } from '@app/models/generated/dtos/CommonLogBookPageDataDTO';
import { LogBookPageStatusesEnum } from '@app/enums/log-book-page-statuses.enum';
import { BasicLogBookPageDocumentDataDTO } from '@app/models/generated/dtos/BasicLogBookPageDocumentDataDTO';

export class CatchesAndSalesDialogParamsModel {
    public id: number | undefined;
    public logBookId!: number;
    public logBookTypeId!: number;
    public viewMode!: boolean;
    public service!: ICatchesAndSalesService;
    /** Needed only for first sale, admission and transportation log book pages */
    public commonData: CommonLogBookPageDataDTO | undefined;
    /** Not needed for ship and aquaculture log book pages */
    public shipPageDocumentData: BasicLogBookPageDocumentDataDTO | undefined
    public pageNumber: number | undefined;
    public pageStatus: LogBookPageStatusesEnum | undefined;
    /** Needed only for admission and transportation log book pages, when the log books are for Person/Legal */
    public logBookPermitLicenseId: number | undefined;
    public canEditCommonDataPermission: boolean = false;

    public constructor(obj?: Partial<CatchesAndSalesDialogParamsModel>) {
        Object.assign(this, obj);
    }
}