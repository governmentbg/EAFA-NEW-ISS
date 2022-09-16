import { InspectorDuringInspectionDTO } from '@app/models/generated/dtos/InspectorDuringInspectionDTO';

export class InspectorTableModel extends InspectorDuringInspectionDTO {
    public institution: string | undefined;
    public isCurrentUser: boolean = false;

    public constructor(params?: Partial<InspectorTableModel>) {
        super(params);
        Object.assign(this, params);
    }
}