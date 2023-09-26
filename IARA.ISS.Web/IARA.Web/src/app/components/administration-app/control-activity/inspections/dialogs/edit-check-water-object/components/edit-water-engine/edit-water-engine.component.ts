import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';

import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { WaterVesselTableParams } from '../water-vessels-table/models/water-vessel-table-params';
import { WaterInspectionEngineDTO } from '@app/models/generated/dtos/WaterInspectionEngineDTO';
import { TLValidators } from '@app/shared/utils/tl-validators';

@Component({
    selector: 'edit-water-engine',
    templateUrl: './edit-water-engine.component.html',
})
export class EditWaterEngineComponent implements OnInit, IDialogComponent {
    public form!: FormGroup;

    protected model: WaterInspectionEngineDTO = new WaterInspectionEngineDTO();

    private readOnly: boolean = false;

    public constructor() {
        this.buildForm();
    }

    public async ngOnInit(): Promise<void> {
        if (this.readOnly) {
            this.form.disable();
        }

        this.fillForm();
    }

    public setData(data: WaterVesselTableParams, wrapperData: DialogWrapperData): void {
        if (data.model !== undefined && data.model !== null) {
            this.model = data.model;
        }

        this.readOnly = data.readOnly;
    }

    public dialogButtonClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public saveBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        if (this.readOnly) {
            dialogClose(this.model);
        }
        else {
            this.form.markAllAsTouched();
            if (this.form.valid) {
                this.fillModel();
                CommonUtils.sanitizeModelStrings(this.model);
                dialogClose(this.model);
            }
        }
    }

    public cancelBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    protected buildForm(): void {
        this.form = new FormGroup({
            modelControl: new FormControl(null, Validators.maxLength(50)),
            powerControl: new FormControl(null, TLValidators.number(0)),
            typeControl: new FormControl(null, Validators.maxLength(50)),
            totalCountControl: new FormControl(null, TLValidators.number(0, undefined, 0)),
            colorControl: new FormControl(null, Validators.maxLength(50)),
            takenControl: new FormControl(null),
            storedControl: new FormControl(null),
            storageControl: new FormControl(null, Validators.maxLength(500))
        });
    }

    protected fillForm(): void {
        this.form.get('modelControl')!.setValue(this.model.model);
        this.form.get('powerControl')!.setValue(this.model.power);
        this.form.get('typeControl')!.setValue(this.model.type);
        this.form.get('totalCountControl')!.setValue(this.model.totalCount);
        this.form.get('colorControl')!.setValue(this.model.engineDescription);
        this.form.get('takenControl')!.setValue(this.model.isTaken);
        this.form.get('storedControl')!.setValue(this.model.isStored);
        this.form.get('storageControl')!.setValue(this.model.storageLocation);
    }

    protected fillModel(): void {
        this.model.model = this.form.get('modelControl')!.value;
        this.model.type = this.form.get('typeControl')!.value;
        this.model.engineDescription = this.form.get('colorControl')!.value;
        this.model.isTaken = this.form.get('takenControl')!.value ?? false;
        this.model.isStored = this.form.get('storedControl')!.value ?? false;
        this.model.storageLocation = this.form.get('storageControl')!.value;

        const power: number | undefined = Number(this.form.get('powerControl')!.value);
        this.model.power = isNaN(power) ? undefined : power;

        const totalCount: number | undefined = Number(this.form.get('totalCountControl')!.value);
        this.model.totalCount = isNaN(totalCount) ? undefined : totalCount;
    }
}