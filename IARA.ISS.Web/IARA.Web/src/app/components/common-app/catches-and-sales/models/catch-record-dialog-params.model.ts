import { CatchRecordDTO } from '@app/models/generated/dtos/CatchRecordDTO';
import { ICatchesAndSalesService } from '@app/interfaces/common-app/catches-and-sales.interface';
import { WaterTypesEnum } from '@app/enums/water-types.enum';
import { ShipLogBookPageDataService } from '../components/ship-log-book/services/ship-log-book-page-data.service';

export class CatchRecordDialogParamsModel {
    public model: CatchRecordDTO | undefined;
    public viewMode!: boolean;
    public service!: ICatchesAndSalesService;
    public waterType!: WaterTypesEnum;
    public permitLicenseAquaticOrganismTypeIds!: number[];
    public shipLogBookPageDataService!: ShipLogBookPageDataService;
    public tripStartDateTime: Date | undefined;
    public tripEndDateTime: Date | undefined;

    public constructor(obj?: Partial<CatchRecordDialogParamsModel>) {
        Object.assign(this, obj);
    }
}