import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';

import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { WaterInspectionVesselDTO } from '@app/models/generated/dtos/WaterInspectionVesselDTO';
import { WaterVesselTableParams } from '../water-vessels-table/models/water-vessel-table-params';
import { InspectionsService } from '@app/services/administration-app/inspections.service';
import { TLValidators } from '@app/shared/utils/tl-validators';

@Component({
    selector: 'edit-water-vessel',
    templateUrl: './edit-water-vessel.component.html',
})
export class EditWaterVesselComponent implements OnInit, IDialogComponent {
    public form!: FormGroup;

    protected model: WaterInspectionVesselDTO = new WaterInspectionVesselDTO();

    public vesselTypes: NomenclatureDTO<number>[] = [];

    private readOnly: boolean = false;

    private readonly service: InspectionsService;

    public constructor(service: InspectionsService) {
        this.service = service;

        this.buildForm();
    }

    public async ngOnInit(): Promise<void> {
        if (this.readOnly) {
            this.form.disable();
        }

        this.vesselTypes = await NomenclatureStore.instance.getNomenclature(
            NomenclatureTypes.InspectionVesselTypes, this.service.getInspectionVesselTypes.bind(this.service), false
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
            numberControl: new FormControl(null, Validators.maxLength(50)),
            colorControl: new FormControl(null, Validators.maxLength(50)),
            lengthControl: new FormControl(null, TLValidators.number(0)),
            widthControl: new FormControl(null, TLValidators.number(0)),
            totalControl: new FormControl(null, TLValidators.number(0, undefined, 0)),
            takenControl: new FormControl(null),
            storedControl: new FormControl(null),
            storageControl: new FormControl(null, Validators.maxLength(500)),
            descriptionControl: new FormControl(null, Validators.maxLength(500))
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
        this.form.get('descriptionControl')!.setValue(this.model.description);
    }

    protected fillModel(): void {
        this.model.vesselTypeId = this.form.get('typeControl')!.value?.value;
        this.model.number = this.form.get('numberControl')!.value;
        this.model.color = this.form.get('colorControl')!.value;
        this.model.isTaken = this.form.get('takenControl')!.value ?? false;
        this.model.isStored = this.form.get('storedControl')!.value ?? false;
        this.model.storageLocation = this.form.get('storageControl')!.value;
        this.model.description = this.form.get('descriptionControl')!.value;

        const totalCount: number | undefined = Number(this.form.get('totalControl')!.value);
        this.model.totalCount = isNaN(totalCount) ? undefined : totalCount;

        const length: number | undefined = Number(this.form.get('lengthControl')!.value);
        this.model.length = isNaN(length) ? undefined : length;

        const width: number | undefined = Number(this.form.get('widthControl')!.value);
        this.model.width = isNaN(width) ? undefined : width;
    }
}