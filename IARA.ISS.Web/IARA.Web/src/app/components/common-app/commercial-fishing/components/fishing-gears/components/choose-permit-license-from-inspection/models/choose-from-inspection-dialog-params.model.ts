import { PageCodeEnum } from '@app/enums/page-code.enum';
import { ICommercialFishingService } from '@app/interfaces/common-app/commercial-fishing.interface';
import { ShipNomenclatureDTO } from '@app/models/generated/dtos/ShipNomenclatureDTO';

export class ChooseFromInspectionDialogParams {
    public shipId: number | undefined;
    public service!: ICommercialFishingService;
    public ships!: ShipNomenclatureDTO[];
    public pageCode!: PageCodeEnum;

    public constructor(obj?: Partial<ChooseFromInspectionDialogParams>) {
        Object.assign(this, obj);
    }
}