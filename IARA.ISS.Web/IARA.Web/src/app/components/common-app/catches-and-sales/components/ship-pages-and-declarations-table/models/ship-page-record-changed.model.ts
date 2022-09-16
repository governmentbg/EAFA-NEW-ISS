import { ShipLogBookPageRegisterDTO } from '@app/models/generated/dtos/ShipLogBookPageRegisterDTO';

export class ShipPageRecordChanged {
    public shipPage!: ShipLogBookPageRegisterDTO;
    public viewMode: boolean = false;

    public constructor(obj?: Partial<ShipPageRecordChanged>) {
        Object.assign(this, obj);
    }
}