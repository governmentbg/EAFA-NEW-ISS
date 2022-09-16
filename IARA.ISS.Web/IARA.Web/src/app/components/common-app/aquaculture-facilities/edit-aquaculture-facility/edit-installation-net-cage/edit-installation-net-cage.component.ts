import { Component } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';

import { AquacultureInstallationNetCageShapesEnum } from '@app/enums/aquaculture-installation-net-cage-shapes.enum';
import { AquacultureInstallationNetCageDTO } from '@app/models/generated/dtos/AquacultureInstallationNetCageDTO';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { TLValidators } from '@app/shared/utils/tl-validators';
import { EditInstallationNetCageDialogParams } from '../models/edit-installation-net-cage-dialog-params.model';

function netCageRectArea(length: number, width: number, height: number): number {
    return length * width;
}

function netCageRectVolume(length: number, width: number, height: number): number {
    return length * width * height;
}

function netCageCircArea(radius: number): number {
    return Math.PI * Math.pow(radius, 2);
}

function netCageCircVolume(radius: number, height: number): number {
    return Math.PI * Math.pow(radius, 2) * height;
}

@Component({
    selector: 'edit-installation-net-cage',
    templateUrl: './edit-installation-net-cage.component.html'
})
export class EditInstallationNetCageComponent implements IDialogComponent {
    public form!: FormGroup;
    public areaControl: FormControl = new FormControl({ value: '', disabled: true });
    public volumeControl: FormControl = new FormControl({ value: '', disabled: true });

    public netCageTypes: NomenclatureDTO<number>[] = [];
    public netCageShapes: NomenclatureDTO<AquacultureInstallationNetCageShapesEnum>[] = [];

    public shapes: typeof AquacultureInstallationNetCageShapesEnum = AquacultureInstallationNetCageShapesEnum;

    private isReadOnly: boolean = false;

    private model!: AquacultureInstallationNetCageDTO;

    public constructor() {
        this.buildForm();
    }

    public saveBtnClicked(action: IActionInfo, dialogClose: DialogCloseCallback): void {
        if (this.isReadOnly) {
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

    public cancelBtnClicked(action: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public dialogButtonClicked(action: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public setData(data: EditInstallationNetCageDialogParams, wrapperData: DialogWrapperData): void {
        this.netCageTypes = data.netCageTypes;
        this.netCageShapes = data.netCageShapes;
        this.isReadOnly = data.readOnly;

        if (data.model === undefined) {
            this.model = new AquacultureInstallationNetCageDTO({ isActive: true });
        }
        else {
            if (this.isReadOnly) {
                this.form.disable();
            }

            this.model = data.model;
            this.fillForm();
        }
    }

    private buildForm(): void {
        this.form = new FormGroup({
            typeControl: new FormControl(null, Validators.required),
            shapeControl: new FormControl(null, Validators.required),
            countControl: new FormControl(null, [Validators.required, TLValidators.number(1)]),
            radiusControl: new FormControl(null, TLValidators.number(0)),
            lengthControl: new FormControl(null, TLValidators.number(0)),
            widthControl: new FormControl(null, TLValidators.number(0)),
            heightControl: new FormControl(null, TLValidators.number(0))
        });

        this.form.get('shapeControl')!.valueChanges.subscribe({
            next: (shape: NomenclatureDTO<AquacultureInstallationNetCageShapesEnum> | undefined) => {
                if (shape !== undefined) {
                    if (shape.value === AquacultureInstallationNetCageShapesEnum.Rectangular) {
                        this.form.get('radiusControl')!.clearValidators();
                        this.form.get('lengthControl')!.setValidators([Validators.required, TLValidators.number(0)]);
                        this.form.get('widthControl')!.setValidators([Validators.required, TLValidators.number(0)]);
                        this.form.get('heightControl')!.setValidators([Validators.required, TLValidators.number(0)]);

                        this.form.get('radiusControl')!.setValue('');
                        this.form.get('lengthControl')!.setValue('');
                        this.form.get('widthControl')!.setValue('');
                        this.form.get('heightControl')!.setValue('');

                        this.form.get('lengthControl')!.markAsPending({ emitEvent: false });
                        this.form.get('widthControl')!.markAsPending({ emitEvent: false });
                        this.form.get('heightControl')!.markAsPending({ emitEvent: false });
                        this.form.get('lengthControl')!.markAsUntouched();
                        this.form.get('widthControl')!.markAsUntouched();
                        this.form.get('heightControl')!.markAsUntouched();
                    }
                    else if (shape.value === AquacultureInstallationNetCageShapesEnum.Circular) {
                        this.form.get('lengthControl')!.clearValidators();
                        this.form.get('widthControl')!.clearValidators();
                        this.form.get('radiusControl')!.setValidators([Validators.required, TLValidators.number(0)]);
                        this.form.get('heightControl')!.setValidators([Validators.required, TLValidators.number(0)]);

                        this.form.get('radiusControl')!.setValue('');
                        this.form.get('lengthControl')!.setValue('');
                        this.form.get('widthControl')!.setValue('');
                        this.form.get('heightControl')!.setValue('');

                        this.form.get('radiusControl')!.markAsPending({ emitEvent: false });
                        this.form.get('heightControl')!.markAsPending({ emitEvent: false });
                        this.form.get('radiusControl')!.markAsUntouched();
                        this.form.get('heightControl')!.markAsUntouched();
                    }
                }
            }
        });

        this.form.valueChanges.subscribe({
            next: () => {
                const shape: AquacultureInstallationNetCageShapesEnum | undefined = this.form.get('shapeControl')!.value?.value;

                if (shape === AquacultureInstallationNetCageShapesEnum.Rectangular) {
                    let l: number = NaN;
                    let w: number = NaN;
                    let h: number = NaN;
                    let n: number = NaN;

                    if (!this.isNullOrEmptyString(this.form.get('lengthControl')!.value)) {
                        l = Number(this.form.get('lengthControl')!.value);
                    }
                    if (!this.isNullOrEmptyString(this.form.get('widthControl')!.value)) {
                        w = Number(this.form.get('widthControl')!.value);
                    }
                    if (!this.isNullOrEmptyString(this.form.get('heightControl')!.value)) {
                        h = Number(this.form.get('heightControl')!.value);
                    }
                    if (!this.isNullOrEmptyString(this.form.get('countControl')!.value)) {
                        n = Number(this.form.get('countControl')!.value);
                    }

                    if (!isNaN(l) && !isNaN(w) && !isNaN(h) && !isNaN(n)) {
                        this.areaControl.setValue((n * netCageRectArea(l, w, h)).toFixed(2));
                        this.volumeControl.setValue((n * netCageRectVolume(l, w, h)).toFixed(2));
                    }
                    else {
                        this.areaControl.setValue('');
                        this.volumeControl.setValue('');
                    }
                }
                else if (shape === AquacultureInstallationNetCageShapesEnum.Circular) {
                    let r: number = NaN;
                    let h: number = NaN;
                    let n: number = NaN;

                    if (!this.isNullOrEmptyString(this.form.get('radiusControl')!.value)) {
                        r = Number(this.form.get('radiusControl')!.value);
                    }
                    if (!this.isNullOrEmptyString(this.form.get('heightControl')!.value)) {
                        h = Number(this.form.get('heightControl')!.value);
                    }
                    if (!this.isNullOrEmptyString(this.form.get('countControl')!.value)) {
                        n = Number(this.form.get('countControl')!.value);
                    }
                    if (!isNaN(r) && !isNaN(h) && !isNaN(n)) {
                        this.areaControl.setValue((n * netCageCircArea(r)).toFixed(2));
                        this.volumeControl.setValue((n * netCageCircVolume(r, h)).toFixed(2));
                    }
                    else {
                        this.areaControl.setValue('');
                        this.volumeControl.setValue('');
                    }
                }
            }
        });
    }

    private fillForm(): void {
        this.form.get('typeControl')!.setValue(this.netCageTypes.find(x => x.value === this.model.netCageTypeId));
        this.form.get('shapeControl')!.setValue(this.netCageShapes.find(x => x.value === this.model.shape));
        this.form.get('countControl')!.setValue(this.model.count);

        switch (this.model.shape) {
            case AquacultureInstallationNetCageShapesEnum.Rectangular:
                this.form.get('lengthControl')!.setValue(this.model.length);
                this.form.get('widthControl')!.setValue(this.model.width);
                this.form.get('heightControl')!.setValue(this.model.height);
                break;
            case AquacultureInstallationNetCageShapesEnum.Circular:
                this.form.get('radiusControl')!.setValue(this.model.radius);
                this.form.get('heightControl')!.setValue(this.model.height);
                break;
        }

        this.areaControl.setValue(this.model.area?.toFixed(2));
        this.volumeControl.setValue(this.model.volume?.toFixed(2));
    }

    private fillModel(): void {
        this.model.netCageTypeId = this.form.get('typeControl')!.value?.value;
        this.model.shape = this.form.get('shapeControl')!.value?.value;
        this.model.count = this.form.get('countControl')!.value

        switch (this.model.shape) {
            case AquacultureInstallationNetCageShapesEnum.Rectangular:
                this.model.length = this.form.get('lengthControl')!.value;
                this.model.width = this.form.get('widthControl')!.value;
                this.model.height = this.form.get('heightControl')!.value;
                break;
            case AquacultureInstallationNetCageShapesEnum.Circular:
                this.model.radius = this.form.get('radiusControl')!.value;
                this.model.height = this.form.get('heightControl')!.value;
                break;
        }

        this.model.area = this.areaControl.value;
        this.model.volume = this.volumeControl.value;
    }

    private isNullOrEmptyString(value: unknown): boolean {
        return value === null || value === undefined || value === '';
    }
}