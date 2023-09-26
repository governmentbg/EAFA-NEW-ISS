import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';

import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { WaterFishingGearTableParams } from '../water-fishing-gears-table/models/water-fishing-gear-table-params';
import { WaterInspectionFishingGearDTO } from '@app/models/generated/dtos/WaterInspectionFishingGearDTO';

@Component({
    selector: 'edit-water-fishing-gear',
    templateUrl: './edit-water-fishing-gear.component.html',
})
export class EditWaterFishingGearComponent implements OnInit, IDialogComponent {
    public form!: FormGroup;

    protected model: WaterInspectionFishingGearDTO | undefined;

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

    public setData(data: WaterFishingGearTableParams, wrapperData: DialogWrapperData): void {
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
            fishingGearControl: new FormControl(null),
            takenControl: new FormControl(null),
            storedControl: new FormControl(null),
            storageControl: new FormControl(null, Validators.maxLength(500))
        });
    }

    protected fillForm(): void {
        if (this.model !== null && this.model !== undefined) {
            this.form.get('fishingGearControl')!.setValue(this.model);
            this.form.get('takenControl')!.setValue(this.model.isTaken);
            this.form.get('storedControl')!.setValue(this.model.isStored);
            this.form.get('storageControl')!.setValue(this.model.storageLocation);
        }
    }

    protected fillModel(): void {
        this.model = this.form.get('fishingGearControl')!.value as WaterInspectionFishingGearDTO;
        this.model.isTaken = this.form.get('takenControl')!.value ?? false;
        this.model.isStored = this.form.get('storedControl')!.value ?? false;
        this.model.storageLocation = this.form.get('storageControl')!.value;
    }
}