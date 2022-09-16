import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';

import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { CommonNomenclatures } from '@app/services/common-app/common-nomenclatures.service';
import { WaterInspectionVesselDTO } from '@app/models/generated/dtos/WaterInspectionVesselDTO';
import { WaterVesselTableParams } from '../water-vessels-table/models/water-vessel-table-params';

@Component({
    selector: 'edit-water-vessel',
    templateUrl: './edit-water-vessel.component.html',
})
export class EditWaterVesselComponent implements OnInit, IDialogComponent {
    public form!: FormGroup;

    protected model: WaterInspectionVesselDTO = new WaterInspectionVesselDTO();

    public vesselTypes: NomenclatureDTO<number>[] = [];

    private readOnly: boolean = false;

    private readonly nomenclatures: CommonNomenclatures;

    public constructor(nomenclatures: CommonNomenclatures) {
        this.nomenclatures = nomenclatures;

        this.buildForm();
    }

    public async ngOnInit(): Promise<void> {
        if (this.readOnly) {
            this.form.disable();
        }

        this.vesselTypes = await NomenclatureStore.instance.getNomenclature(
            NomenclatureTypes.VesselTypes, this.nomenclatures.getVesselTypes.bind(this.nomenclatures), false
        ).toPromise();

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
            typeControl: new FormControl(null),
            numberControl: new FormControl(null),
            colorControl: new FormControl(null),
            lengthControl: new FormControl(null),
            widthControl: new FormControl(null),
            totalControl: new FormControl(null),
            takenControl: new FormControl(null),
            storedControl: new FormControl(null),
            storageControl: new FormControl(null),
        });
    }

    protected fillForm(): void {
        this.form.get('typeControl')!.setValue(this.vesselTypes.find(f => f.value === this.model.vesselTypeId));
        this.form.get('numberControl')!.setValue(this.model.number);
        this.form.get('colorControl')!.setValue(this.model.color);
        this.form.get('lengthControl')!.setValue(this.model.length);
        this.form.get('widthControl')!.setValue(this.model.width);
        this.form.get('totalControl')!.setValue(this.model.totalCount);
        this.form.get('takenControl')!.setValue(this.model.isTaken);
        this.form.get('storedControl')!.setValue(this.model.isStored);
        this.form.get('storageControl')!.setValue(this.model.storageLocation);
    }

    protected fillModel(): void {
        this.model.vesselTypeId = this.form.get('typeControl')!.value?.value;
        this.model.number = this.form.get('numberControl')!.value;
        this.model.color = this.form.get('colorControl')!.value;
        this.model.length = Number(this.form.get('lengthControl')!.value);
        this.model.width = Number(this.form.get('widthControl')!.value);
        this.model.totalCount = Number(this.form.get('totalControl')!.value);
        this.model.isTaken = this.form.get('takenControl')!.value;
        this.model.isStored = this.form.get('storedControl')!.value;
        this.model.storageLocation = this.form.get('storageControl')!.value;
    }
}