import { IApplicationsService } from "@app/interfaces/administration-app/applications.interface"
import { NomenclatureDTO } from "@app/models/generated/dtos/GenericNomenclatureDTO"

export class PaymentDataInfo {
    public paymentTypes!: NomenclatureDTO<number>[];
    public applicationId!: number;
    public service!: IApplicationsService;
    public viewMode: boolean = false;
    public paymentDateMin?: Date;
    public paymentDateMax?: Date;

    public constructor(obj?: Partial<PaymentDataInfo>) {
        Object.assign(this, obj);
    }
}