import { DialogParamsModel } from '@app/models/common/dialog-params.model';

export class EditShipDialogParams extends DialogParamsModel {
    public isThirdPartyShip?: boolean;

    public constructor(obj?: Partial<EditShipDialogParams>) {
        if (obj !== undefined) {
            super(obj as DialogParamsModel);
            Object.assign(this, obj);
        }
        else {
            super();
        }
    }
}