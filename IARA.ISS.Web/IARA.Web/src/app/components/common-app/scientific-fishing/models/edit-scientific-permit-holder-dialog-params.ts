import { IScientificFishingService } from '@app/interfaces/common-app/scientific-fishing.interface';
import { ScientificFishingPermitHolderDTO } from '@app/models/generated/dtos/ScientificFishingPermitHolderDTO';
import { ScientificFishingPermitHolderRegixDataDTO } from '@app/models/generated/dtos/ScientificFishingPermitHolderRegixDataDTO';

export class EditScientificPermitHolderDialogParams {
    public service!: IScientificFishingService;
    public requestDate: Date | undefined;
    public model: ScientificFishingPermitHolderDTO | undefined;
    public isEgnLncReadOnly: boolean = false;
    public readOnly: boolean = false;
    public showOnlyRegiXData: boolean = false;
    public expectedResults: ScientificFishingPermitHolderRegixDataDTO = new ScientificFishingPermitHolderRegixDataDTO();

    public constructor(params?: Partial<EditScientificPermitHolderDialogParams>) {
        Object.assign(this, params);
    }
}