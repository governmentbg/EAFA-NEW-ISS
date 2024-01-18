

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { FileInfoDTO } from './FileInfoDTO';
import { InspectionTypesEnum } from '@app/enums/inspection-types.enum';

export class InspectionDraftDTO { 
    public constructor(obj?: Partial<InspectionDraftDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public inspectionType?: InspectionTypesEnum;

    @StrictlyTyped(Date)
    public startDate?: Date;

    @StrictlyTyped(Date)
    public endDate?: Date;

    @StrictlyTyped(Boolean)
    public byEmergencySignal?: boolean;

    @StrictlyTyped(String)
    public inspectorComment?: string;

    @StrictlyTyped(Boolean)
    public administrativeViolation?: boolean;

    @StrictlyTyped(String)
    public actionsTaken?: string;

    @StrictlyTyped(String)
    public json?: string;

    @StrictlyTyped(String)
    public reportNumber?: string;

    @StrictlyTyped(FileInfoDTO)
    public files?: FileInfoDTO[];
}