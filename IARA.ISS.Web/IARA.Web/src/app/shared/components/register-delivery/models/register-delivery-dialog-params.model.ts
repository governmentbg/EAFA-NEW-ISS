import { IDeliveryService } from "@app/interfaces/common-app/delivery.interface";
import { PageCodeEnum } from '@app/enums/page-code.enum';

export class RegisterDeliveryDialogParams {
    public deliveryId!: number;
    public isPublicApp: boolean = false;
    public service!: IDeliveryService;
    public pageCode!: PageCodeEnum;
    public registerId!: number;
    public viewMode: boolean = false;

    public constructor(obj?: Partial<RegisterDeliveryDialogParams>) {
        Object.assign(this, obj);
    }

}