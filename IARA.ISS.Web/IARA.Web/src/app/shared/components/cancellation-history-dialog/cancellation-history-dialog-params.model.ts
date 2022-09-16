import { CancellationReasonGroupEnum } from '@app/enums/cancellation-reason-group.enum';
import { CancellationHistoryEntryDTO } from '@app/models/generated/dtos/CancellationHistoryEntryDTO';

export class CancellationHistoryDialogParams {
    public group!: CancellationReasonGroupEnum;
    public cancelling: boolean = true;
    public statuses: CancellationHistoryEntryDTO[] = [];

    public constructor(params?: Partial<CancellationHistoryDialogParams>) {
        Object.assign(this, params);
    }
}