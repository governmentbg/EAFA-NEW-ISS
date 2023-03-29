import { Observable } from 'rxjs';

import { LogBookPageCancellationReasonDTO } from '@app/models/generated/dtos/LogBookPageCancellationReasonDTO';
import { LogBookTypesEnum } from '@app/enums/log-book-types.enum';

export type JustifiedCancellationMethod = (reasonData: LogBookPageCancellationReasonDTO, logBookType: LogBookTypesEnum) => Observable<void>;

export class JustifiedCancellationDialogParams {
    public model: LogBookPageCancellationReasonDTO | undefined;
    public logBookType!: LogBookTypesEnum;
    public reasonControlLabel: string | undefined;
    public reasonControlTooltipResouce: string | undefined;
    public cancellationServiceMethod!: JustifiedCancellationMethod;

    public constructor(obj?: Partial<JustifiedCancellationDialogParams>) {
        Object.assign(this, obj);
    }
}