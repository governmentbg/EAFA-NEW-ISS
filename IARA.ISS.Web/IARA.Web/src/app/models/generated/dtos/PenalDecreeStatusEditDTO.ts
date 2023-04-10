

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { PenalDecreeStatusDTO } from './PenalDecreeStatusDTO';
import { PenalDecreePaymentScheduleDTO } from './PenalDecreePaymentScheduleDTO';
import { PenalDecreeStatusTypesEnum } from '@app/enums/penal-decree-status-types.enum'; 

export class PenalDecreeStatusEditDTO extends PenalDecreeStatusDTO {
    public constructor(obj?: Partial<PenalDecreeStatusEditDTO>) {
        if (obj != undefined) {
            super(obj as PenalDecreeStatusDTO);
            Object.assign(this, obj);
        } 
        else {
            super();
        }
    }
  
    @StrictlyTyped(Number)
    public statusType?: PenalDecreeStatusTypesEnum;

    @StrictlyTyped(Number)
    public penalDecreeId?: number;

    @StrictlyTyped(Date)
    public appealDate?: Date;

    @StrictlyTyped(Number)
    public courtId?: number;

    @StrictlyTyped(String)
    public courtName?: string;

    @StrictlyTyped(String)
    public caseNum?: string;

    @StrictlyTyped(Date)
    public complaintDueDate?: Date;

    @StrictlyTyped(Number)
    public remunerationAmount?: number;

    @StrictlyTyped(Date)
    public enactmentDate?: Date;

    @StrictlyTyped(Number)
    public penalAuthorityTypeId?: number;

    @StrictlyTyped(String)
    public penalAuthorityName?: string;

    @StrictlyTyped(String)
    public amendments?: string;

    @StrictlyTyped(Number)
    public confiscationInstitutionId?: number;

    @StrictlyTyped(String)
    public confiscationInstitution?: string;

    @StrictlyTyped(PenalDecreePaymentScheduleDTO)
    public paymentSchedule?: PenalDecreePaymentScheduleDTO[];

    @StrictlyTyped(Number)
    public paidAmount?: number;
}