import { DialogParamsModel } from '@app/models/common/dialog-params.model';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';

export class ChooseFromOldPermitLicenseDialogParamsModel extends DialogParamsModel {
    public permitLicenses: NomenclatureDTO<number>[] = [];
    public isShip: boolean = false;

    public constructor(params?: Partial<ChooseFromOldPermitLicenseDialogParamsModel>) {
        super(params);
        Object.assign(this, params);
    }
}