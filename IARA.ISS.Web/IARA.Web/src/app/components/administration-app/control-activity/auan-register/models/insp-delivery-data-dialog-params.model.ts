import { DialogParamsModel } from '@app/models/common/dialog-params.model';
import { IInspDeliveryService } from '@app/interfaces/administration-app/insp-delivery.interface';

export class InspDeliveryDataDialogParams extends DialogParamsModel {
    public registerId!: number;
    public isAuan: boolean = false;
    public service!: IInspDeliveryService;

    public constructor(obj?: Partial<InspDeliveryDataDialogParams>) {
        super(obj);
        Object.assign(this, obj);
    }
}