import { PageCodeEnum } from '@app/enums/page-code.enum';
import { ICommercialFishingService } from '@app/interfaces/common-app/commercial-fishing.interface';
import { ShipNomenclatureDTO } from '@app/models/generated/dtos/ShipNomenclatureDTO';

export class ChoosePermitToCopyFromDialogParams {
    public service!: ICommercialFishingService;
    public shipId: number | undefined;
    public permitId: number | undefined;
    public permitNumber: string | undefined;
    public pageCode!: PageCodeEnum;
    public ships!: ShipNomenclatureDTO[];

    public constructor(obj?: Partial<ChoosePermitToCopyFromDialogParams>) {
        Object.assign(this, obj);
    }
}