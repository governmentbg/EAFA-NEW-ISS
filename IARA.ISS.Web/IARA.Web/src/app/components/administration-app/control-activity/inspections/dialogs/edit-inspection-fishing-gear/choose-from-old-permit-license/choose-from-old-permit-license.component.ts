import { Component } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { ChooseFromOldPermitLicenseDialogParamsModel } from '../../../models/choose-from-old-permit-license.model';
import { InspectionPermitLicenseDTO } from '@app/models/generated/dtos/InspectionPermitLicenseDTO';

@Component({
    selector: 'choose-from-old-permit-license',
    templateUrl: './choose-from-old-permit-license.component.html'
})
export class ChooseFromOldPermitLicenseComponent implements IDialogComponent {
    public form: FormGroup;

    public noPermitLicensesError: boolean = false;
    public isShip: boolean = false;

    public permitLicenses: InspectionPermitLicenseDTO[] = [];

    public constructor() {
        this.form = this.buildForm();
    }

    public setData(data: ChooseFromOldPermitLicenseDialogParamsModel, wrapperData: DialogWrapperData): void {
        this.permitLicenses = data.permitLicenses;
        this.isShip = data.isShip;

        if (data.permitLicenses.length === 0) {
            this.noPermitLicensesError = true;
        }
    }

    public saveBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        this.form.markAllAsTouched();

        if (this.form.valid && !this.noPermitLicensesError) {
            const result: number = this.form.get('permitLicenseControl')!.value?.value;
            dialogClose(result);
        }
    }

    public dialogButtonClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public cancelBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    private buildForm(): FormGroup {
        return new FormGroup({
            permitLicenseControl: new FormControl(null, Validators.required)
        });
    }
}