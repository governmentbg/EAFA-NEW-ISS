

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { PenalDecreeStatusEditDTO } from './PenalDecreeStatusEditDTO';
import { PenalDecreeTypeEnum } from '@app/enums/penal-decree-type.enum';
import { AuanStatusEnum } from '@app/enums/auan-status.enum';

export class PenalDecreeDTO { 
    public constructor(obj?: Partial<PenalDecreeDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public auanId?: number;

    @StrictlyTyped(Number)
    public typeId?: number;

    @StrictlyTyped(Number)
    public decreeType?: PenalDecreeTypeEnum;

    @StrictlyTyped(String)
    public decreeName?: string;

    @StrictlyTyped(String)
    public decreeNum?: string;

    @StrictlyTyped(Date)
    public issueDate?: Date;

    @StrictlyTyped(String)
    public inspectedEntity?: string;

    @StrictlyTyped(Number)
    public deliveryId?: number;

    @StrictlyTyped(String)
    public status?: string;

    @StrictlyTyped(Number)
    public penalDecreeStatus?: AuanStatusEnum;

    @StrictlyTyped(String)
    public penalDecreeStatusName?: string;

    @StrictlyTyped(String)
    public issuer?: string;

    @StrictlyTyped(PenalDecreeStatusEditDTO)
    public statuses?: PenalDecreeStatusEditDTO[];

    @StrictlyTyped(Boolean)
    public isActive?: boolean;
}