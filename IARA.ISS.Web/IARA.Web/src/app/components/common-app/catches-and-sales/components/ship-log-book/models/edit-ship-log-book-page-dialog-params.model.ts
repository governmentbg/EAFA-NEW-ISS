import { ICatchesAndSalesService } from '@app/interfaces/common-app/catches-and-sales.interface';
import { ShipLogBookPageEditDTO } from '@app/models/generated/dtos/ShipLogBookPageEditDTO';

export class EditShipLogBookPageDialogParams {
    public service!: ICatchesAndSalesService;
    public id: number | undefined;
    public model: ShipLogBookPageEditDTO | undefined;
    public viewMode: boolean = false;

    public constructor(params?: Partial<EditShipLogBookPageDialogParams>) {
        Object.assign(this, params);
    }
}