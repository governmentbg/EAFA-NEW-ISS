import { ShipOwnerDTO } from '@app/models/generated/dtos/ShipOwnerDTO';
import { ShipOwnerRegixDataDTO } from '@app/models/generated/dtos/ShipOwnerRegixDataDTO';
import { ApplicationSubmittedForDTO } from '@app/models/generated/dtos/ApplicationSubmittedForDTO';

export class EditShipOwnerDialogParams {
    public model: ShipOwnerDTO | undefined;
    public isEgnLncReadOnly: boolean = false;
    public isApplication: boolean = false;
    public readOnly: boolean = false;
    public showOnlyRegiXData: boolean = false;
    public isDraft: boolean = false;
    public isThirdPartyShip: boolean = false;
    public submittedFor!: ApplicationSubmittedForDTO;
    public expectedResults: ShipOwnerRegixDataDTO = new ShipOwnerRegixDataDTO();

    public constructor(params?: Partial<EditShipOwnerDialogParams>) {
        Object.assign(this, params);
    }
}