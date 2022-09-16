import { DialogParamsModel } from '@app/models/common/dialog-params.model';
import { AquacultureFacilityEditDTO } from '@app/models/generated/dtos/AquacultureFacilityEditDTO';

export class EditAquacultureFacilityDialogParams extends DialogParamsModel {
    public isChangeOfCircumstancesApplication: boolean = false;
    public isDeregistrationApplication: boolean = false;
    public model: AquacultureFacilityEditDTO | undefined;

    public constructor(params?: Partial<EditAquacultureFacilityDialogParams>) {
        super(params);
        Object.assign(this, params);
    }
}