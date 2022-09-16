import { PageCodeEnum } from '@app/enums/page-code.enum';
import { FishingGearDTO } from '@app/models/generated/dtos/FishingGearDTO';

export class EditFishingGearDialogParamsModel {
    public model: FishingGearDTO | undefined;
    public readOnly: boolean = false;
    public isDraft: boolean = false;
    public pageCode!: PageCodeEnum;

    public constructor(obj?: Partial<EditFishingGearDialogParamsModel>) {
        Object.assign(this, obj);
    }

}