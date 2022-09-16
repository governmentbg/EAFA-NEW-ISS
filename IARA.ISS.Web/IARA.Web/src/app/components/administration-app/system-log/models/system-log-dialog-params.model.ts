import { SystemLogDTO } from "@app/models/generated/dtos/SystemLogDTO";
import { SystemLogViewDTO } from "@app/models/generated/dtos/SystemLogViewDTO";

export class SystemLogDialogParams {
    public systemLogView!: SystemLogViewDTO;
    public systemLog!: SystemLogDTO;

    constructor(obj?: Partial<SystemLogDialogParams>) {
        Object.assign(this, obj);
    }
}