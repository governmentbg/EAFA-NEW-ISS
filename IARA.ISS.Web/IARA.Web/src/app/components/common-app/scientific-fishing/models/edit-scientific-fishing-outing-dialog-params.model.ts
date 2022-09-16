import { IScientificFishingService } from '@app/interfaces/common-app/scientific-fishing.interface';
import { DialogParamsModel } from '@app/models/common/dialog-params.model';
import { ScientificFishingOutingDTO } from '@app/models/generated/dtos/ScientificFishingOutingDTO';

export class EditScientificFishingOutingDialogParams extends DialogParamsModel {
    public permitId: number;
    public service: IScientificFishingService;
    public saveInDialog: boolean;
    public model: ScientificFishingOutingDTO | undefined;

    public constructor(
        permitId: number,
        service: IScientificFishingService,
        model: ScientificFishingOutingDTO | undefined,
        saveInDialog: boolean,
        id?: number,
        readOnly: boolean = false
    ) {
        super({ id: id ?? -1, isReadonly: readOnly });
        this.permitId = permitId;
        this.service = service;
        this.saveInDialog = saveInDialog;
        this.model = model;
    }
}