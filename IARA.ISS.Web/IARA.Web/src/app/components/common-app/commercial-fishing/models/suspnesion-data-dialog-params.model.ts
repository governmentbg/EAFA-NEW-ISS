import { SuspensionDataDTO } from '@app/models/generated/dtos/SuspensionDataDTO';
import { ICommercialFishingService } from '@app/interfaces/common-app/commercial-fishing.interface';
import { ISuspensionService } from '@app/interfaces/common-app/suspension.interface';
import { PageCodeEnum } from '@app/enums/page-code.enum';

export class SuspnesionDataDialogParams {
    public model!: SuspensionDataDTO;
    public isPermit!: boolean;
    public viewMode: boolean = true;
    public service!: ISuspensionService;
    public postOnAdd: boolean = false;
    public pageCode!: PageCodeEnum;
    public recordId!: number;

    public constructor(obj?: Partial<SuspnesionDataDialogParams>) {
        Object.assign(this, obj);
    }
}