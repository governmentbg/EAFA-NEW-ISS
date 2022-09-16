import { Observable } from "rxjs";
import { DialogParamsModel } from "@app/models/common/dialog-params.model";

export class ReasonDialogParams extends DialogParamsModel {
    public saveReasonServiceMethod?: (recordId: number, reason: string) => Observable<void>;
    public reasonFieldValue?: string;

    constructor(obj: Partial<ReasonDialogParams>) {
        super({ id: obj.id, isApplication: obj.isApplication, isReadonly: obj.isReadonly, service: obj.service });
        this.saveReasonServiceMethod = obj.saveReasonServiceMethod;
        this.reasonFieldValue = obj.reasonFieldValue;
    }
}