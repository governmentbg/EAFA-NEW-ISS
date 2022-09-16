import { CommercialFishingLogBookEditDTO } from '@app/models/generated/dtos/CommercialFishingLogBookEditDTO';
import { LogBookEditDTO } from '@app/models/generated/dtos/LogBookEditDTO';
import { LogBookTypesEnum } from '@app/enums/log-book-types.enum';
import { ICatchesAndSalesService } from '@app/interfaces/common-app/catches-and-sales.interface';
import { LogBookGroupsEnum } from '@app/enums/log-book-groups.enum';
import { LogBookPagePersonTypesEnum } from '@app/enums/log-book-page-person-types.enum';

export class EditLogBookDialogParamsModel {
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