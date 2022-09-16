import { DialogParamsModel } from '@app/models/common/dialog-params.model';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';

export class EditTicketDialogParams extends DialogParamsModel {
    public type!: NomenclatureDTO<number>;
    public period!: NomenclatureDTO<number>;
    public isPersonal!: boolean;
    public isAssociation!: boolean;
    public isRenewal!: boolean;

    public constructor(params?: Partial<EditTicketDialogParams>) {
        if (params !== undefined) {
            super((params as DialogParamsModel));
            Object.assign(this, params);
        } else {
            super();
        }
    }
}