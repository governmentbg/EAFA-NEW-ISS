import { ShipOwnerDTO } from '@app/models/generated/dtos/ShipOwnerDTO';
import { ShipOwnerRegixDataDTO } from '@app/models/generated/dtos/ShipOwnerRegixDataDTO';

export class EditShipOwnerDialogResult {
    public owner: ShipOwnerDTO | ShipOwnerRegixDataDTO | undefined;
    public isTouched: boolean;

    public constructor(owner: ShipOwnerDTO | ShipOwnerRegixDataDTO | undefined, isTouched: boolean) {
        this.owner = owner;
        this.isTouched = isTouched;
    }
}