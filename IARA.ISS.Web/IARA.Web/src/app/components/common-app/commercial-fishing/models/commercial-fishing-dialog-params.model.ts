import { CommercialFishingApplicationEditDTO } from '@app/models/generated/dtos/CommercialFishingApplicationEditDTO';
import { DialogParamsModel } from '@app/models/common/dialog-params.model';

export class CommercialFishingDialogParamsModel extends DialogParamsModel {
    public model!: CommercialFishingApplicationEditDTO;

    public constructor(obj?: Partial<CommercialFishingDialogParamsModel>) {
        super(obj);
        Object.assign(this, obj);
    }
}