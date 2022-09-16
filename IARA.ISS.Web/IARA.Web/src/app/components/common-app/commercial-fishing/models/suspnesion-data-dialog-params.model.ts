import { SuspensionDataDTO } from '@app/models/generated/dtos/SuspensionDataDTO';
import { ICommercialFishingService } from '@app/interfaces/common-app/commercial-fishing.interface';

export class SuspnesionDataDialogParams {
    public model!: SuspensionDataDTO;
    public isPermit!: boolean;
    public viewMode: boolean = true;
    public service!: ICommercialFishingService;

    public constructor(obj?: Partial<SuspnesionDataDialogParams>) {
        Object.assign(this, obj);
    }
}