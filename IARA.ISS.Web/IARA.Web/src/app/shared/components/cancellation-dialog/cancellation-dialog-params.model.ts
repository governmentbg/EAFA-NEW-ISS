import { CancellationDetailsDTO } from '@app/models/generated/dtos/CancellationDetailsDTO';
import { CancellationReasonGroupEnum } from '@app/enums/cancellation-reason-group.enum';

export class CancellationDialogParams {
    public model: CancellationDetailsDTO | undefined;
    public group!: CancellationReasonGroupEnum;

    public constructor(params?: Partial<CancellationDialogParams>) {
        Object.assign(this, params);
    }
}