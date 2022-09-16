import { Observable } from 'rxjs';
import { LogBookPageCancellationReasonDTO } from '@app/models/generated/dtos/LogBookPageCancellationReasonDTO';

export type JustifiedCancellationMethod = (reasonData: LogBookPageCancellationReasonDTO) => Observable<void>;

export class JustifiedCancellationDialogParams {
    public model: LogBookPageCancellationReasonDTO | undefined;
    public reasonControlLabel: string | undefined;
    public reasonControlTooltipResouce: string | undefined;
    public cancellationServiceMethod!: JustifiedCancellationMethod;

    public constructor(obj?: Partial<JustifiedCancellationDialogParams>) {
        Object.assign(this, obj);
    }
}