import { DialogParamsModel } from '@app/models/common/dialog-params.model';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { InspectionPermitLicenseDTO } from '@app/models/generated/dtos/InspectionPermitLicenseDTO';

export class ChooseFromOldPermitLicenseDialogParamsModel extends DialogParamsModel {
    public permitLicenses: InspectionPermitLicenseDTO[] = [];
    public isShip: boolean = false;

    public constructor(params?: Partial<ChooseFromOldPermitLicenseDialogParamsModel>) {
        super(params);
        Object.assign(this, params);
    }
}