import { Component, OnInit } from '@angular/core';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { AddressTypesEnum } from '@app/enums/address-types.enum';
import { ChangeUserLegalDTO } from '@app/models/generated/dtos/ChangeUserLegalDTO';
import { ChangeUserLegalDialogParams } from '../models/change-user-legal-dialog-params';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { EikUtils } from '@app/shared/utils/eik.utils';

@Component({
    selector: 'change-userdata-legal',
    templateUrl: './change-userdata-legal.component.html'
})
export class ChangeUserDataLegalComponent implements IDialogComponent {
    public form!: FormGroup;
    public viewMode: boolean = false;
    public isEditing: boolean = false;
    public territoryUnits: NomenclatureDTO<number>[] = []

    public readonly companyHeadquartersType: AddressTypesEnum = AddressTypesEnum.COMPANY_HEADQUARTERS;

    private model!: ChangeUserLegalDTO;

    public constructor() {
        this.form = new FormGroup({
            legalControl: new FormControl(null, Validators.required),
            territoryUnitControl: new FormControl(null, Validators.required),
            legalAddressesControl: new FormControl(null, Validators.required)
        });
    }

    public setData(data: ChangeUserLegalDialogParams, wrapperData: DialogWrapperData): void {
        this.model = data.model!;
        this.viewMode = data.viewMode;
        this.territoryUnits = data.territoryUnits;

        if (this.viewMode) {
            this.form.disable();
        }

        this.fillForm();

        this.isEditing = this.model.legal?.eik !== undefined && EikUtils.isEikValid(this.model.legal.eik);
    }

    public dialogButtonClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public saveBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        if (this.viewMode) {
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

    private fillForm(): void {
        if (this.model.legal?.eik?.includes('ЛРД') === true) {
            this.model.legal.eik = undefined;
        }

        this.form.get('legalControl')!.setValue(this.model.legal);
        this.form.get('territoryUnitControl')!.setValue(this.territoryUnits.find(x => x.value === this.model.territoryUnitId));
        this.form.get('legalAddressesControl')!.setValue(this.model.addresses);
    }

    private fillModel(): void {
        this.model.legal = this.form.get('legalControl')!.value;
        this.model.territoryUnitId = this.form.get('territoryUnitControl')!.value.value;
        this.model.addresses = this.form.get('legalAddressesControl')!.value;
    }
}