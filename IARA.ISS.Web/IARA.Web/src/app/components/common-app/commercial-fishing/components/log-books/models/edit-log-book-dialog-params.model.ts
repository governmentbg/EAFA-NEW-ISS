import { CommercialFishingLogBookEditDTO } from '@app/models/generated/dtos/CommercialFishingLogBookEditDTO';
import { LogBookEditDTO } from '@app/models/generated/dtos/LogBookEditDTO';
import { LogBookTypesEnum } from '@app/enums/log-book-types.enum';
import { ICatchesAndSalesService } from '@app/interfaces/common-app/catches-and-sales.interface';
import { LogBookGroupsEnum } from '@app/enums/log-book-groups.enum';
import { LogBookPagePersonTypesEnum } from '@app/enums/log-book-page-person-types.enum';
import { ILogBookService } from '../../edit-log-book/interfaces/log-book.interface';

export class EditLogBookDialogParamsModel {
    /**
     * If the id is passed and there is no model, 
     * the dialog will try to get its data from the server according to the LogBookGroupsEnum
     * */
    public registerId: number | undefined;
    /**
     * If the id is passed and there is no model, 
     * the dialog will try to get its data from the server
     * */
    public logBookPermitLicenseId: number | undefined;
    /**
     * If the id is passed and there is no model, 
     * the dialog will try to get its data from the server according to the LogBookGroupsEnum
     * */
    public logBookId: number | undefined;
    /**
     * If some id is passed, this serive should be passed as well,
     * in order to get data from the server
     * */
    public service: ILogBookService | undefined;

    public model: LogBookEditDTO | CommercialFishingLogBookEditDTO | undefined;
    public readOnly: boolean = false;
    public logBookGroup!: LogBookGroupsEnum;
    public pagesRangeError: boolean = false;

    /**
     * For when LogBookGroupsEnum = Ship
     **/
    public isOnline: boolean | undefined;
    /**
     * For when LogBookGroupsEnum = Ship
     **/
    public ownerType: LogBookPagePersonTypesEnum | undefined;

    public isForPermitLicense: boolean = false;

    public constructor(obj?: Partial<EditLogBookDialogParamsModel>) {
        Object.assign(this, obj);
    }
}