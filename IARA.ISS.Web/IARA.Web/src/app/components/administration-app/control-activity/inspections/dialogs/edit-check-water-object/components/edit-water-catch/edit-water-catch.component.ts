import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';

import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { CatchActionEnum } from '@app/enums/catch-action.enum';
import { InspectionCatchMeasureDTO } from '@app/models/generated/dtos/InspectionCatchMeasureDTO';
import { WaterCatchTableParams } from '../water-catches-table/models/water-catch-table-params';

@Component({
    selector: 'edit-water-catch',
    templateUrl: './edit-water-catch.component.html',
})
export class EditWaterCatchComponent implements OnInit, IDialogComponent {
    public form!: FormGroup;

    protected model: InspectionCatchMeasureDTO = new InspectionCatchMeasureDTO();

    public fishes: NomenclatureDTO<number>[] = [];
    public storedTypes: NomenclatureDTO<CatchActionEnum>[] = [];

    private readOnly: boolean = false;

    public constructor(translate: FuseTranslationLoaderService) {
        this.storedTypes = [
            new NomenclatureDTO<number>({
                value: CatchActionEnum.Stored,
                displayName: translate.getValue('inspections.water-inspection-stored'),
                isActive: true
            }),
            new NomenclatureDTO<number>({
                value: CatchActionEnum.Returned,
                displayName: translate.getValue('inspections.water-inspection-returned'),
                isActive: true
            }),
            new NomenclatureDTO<number>({
                value: CatchActionEnum.Donated,
                displayName: translate.getValue('inspections.water-inspection-donated'),
                isActive: true
            }),
            new NomenclatureDTO<number>({
                value: CatchActionEnum.Destroyed,
                displayName: translate.getValue('inspections.water-inspection-destroyed'),
                isActive: true
            })
        ];

        this.buildForm();
    }

    public async ngOnInit(): Promise<void> {
        if (this.readOnly) {
            this.form.disable();
        }

        this.fillForm();
    }

    public setData(data: WaterCatchTableParams, wrapperData: DialogWrapperData): void {
        if (data.model !== undefined && data.model !== null) {
            this.model = data.model;
        }

        this.readOnly = data.readOnly;
        this.fishes = data.fishes;
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
            typeControl: new FormControl(null),
            quantityControl: new FormControl(null),
            takenControl: new FormControl(null),
            storedControl: new FormControl(null, [Validators.required]),
            storageControl: new FormControl(null)
        });
    }

    protected fillForm(): void {
        this.form.get('typeControl')!.setValue(this.fishes.find(f => f.value === this.model.fishId));
        this.form.get('quantityControl')!.setValue(this.model.catchQuantity);
        this.form.get('takenControl')!.setValue(this.model.isTaken === true);
        this.form.get('storedControl')!.setValue(this.storedTypes.find(f => f.value === this.model.action));
        this.form.get('storageControl')!.setValue(this.model.storageLocation);
    }

    protected fillModel(): void {
        this.model.fishId = this.form.get('typeControl')!.value?.value;
        this.model.isTaken = this.form.get('takenControl')!.value ?? false;
        this.model.action = this.form.get('storedControl')!.value?.value;
        this.model.storageLocation = this.form.get('storageControl')!.value;

        const catchQuantity: number | undefined = Number(this.form.get('quantityControl')!.value);
        this.model.catchQuantity = isNaN(catchQuantity) ? undefined : catchQuantity;
    }
}