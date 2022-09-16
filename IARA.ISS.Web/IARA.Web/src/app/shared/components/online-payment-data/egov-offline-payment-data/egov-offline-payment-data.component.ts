import { Component } from "@angular/core";
import { FormControl, FormGroup } from "@angular/forms";
import { IActionInfo } from "../../dialog-wrapper/interfaces/action-info.interface";
import { DialogCloseCallback, IDialogComponent } from "../../dialog-wrapper/interfaces/dialog-content.interface";
import { DialogWrapperData } from "../../dialog-wrapper/models/dialog-action-buttons.model";
import { EGovOfflinePaymentDataDialogParams } from "./models/egov-offline-payment-data-dialog-params.model";

@Component({
    selector: 'egov-offline-payment-data',
    templateUrl: './egov-offline-payment-data.component.html'
})
export class EGovOfflinePaymenDataComponent implements IDialogComponent {

    public paymentDataFormGroup: FormGroup;

    public constructor() {
        this.paymentDataFormGroup = new FormGroup({
            referenceNumber: new FormControl('')
        });
    }

    public setData(data: EGovOfflinePaymentDataDialogParams, buttons: DialogWrapperData): void {
        this.paymentDataFormGroup.controls.referenceNumber!.setValue(data.referenceNumber);
    }

    public dialogButtonClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public saveBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public cancelBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }
}