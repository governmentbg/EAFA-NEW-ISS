import { PageCodeEnum } from '@app/enums/page-code.enum';
import { ICommercialFishingService } from '@app/interfaces/common-app/commercial-fishing.interface';
import { ShipNomenclatureDTO } from '@app/models/generated/dtos/ShipNomenclatureDTO';

export class ChoosePermitLicenseFromInspectionDialogParams {
    public shipId: number | undefined;
    public service!: ICommercialFishingService;
    public ships!: ShipNomenclatureDTO[];
    public pageCode!: PageCodeEnum;

    public constructor(obj?: Partial<ChoosePermitLicenseFromInspectionDialogParams>) {
        Object.assign(this, obj);
    }
}