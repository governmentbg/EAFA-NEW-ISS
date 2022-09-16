import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';

import { IFishingCapacityService } from '@app/interfaces/common-app/fishing-capacity.interface';
import { DialogParamsModel } from '@app/models/common/dialog-params.model';
import { FishingCapacityCertificateEditDTO } from '@app/models/generated/dtos/FishingCapacityCertificateEditDTO';
import { FishingCapacityAdministrationService } from '@app/services/administration-app/fishing-capacity-administration.service';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { AddressTypesEnum } from '@app/enums/address-types.enum';
import { DateRangeData } from '@app/shared/components/input-controls/tl-date-range/tl-date-range.component';

@Component({
    selector: 'edit-capacity-certificate',
    templateUrl: './edit-capacity-certificate.component.html'
})
export class EditCapacityCertificateComponent implements OnInit, IDialogComponent {
    public form!: FormGroup;
    public isHolderPerson!: boolean;
    public isIdReadOnly: boolean = false;

    public readonly companyHeadquartersType: AddressTypesEnum = AddressTypesEnum.COMPANY_HEADQUARTERS;

    private service: IFishingCapacityService;
    private id!: number;
    private model!: FishingCapacityCertificateEditDTO;
    private readOnly: boolean = false;

    public constructor(service: FishingCapacityAdministrationService) {
        this.service = service;

        this.buildForm();
    }

    public ngOnInit(): void {
        this.service.getCapacityCertificate(this.id).subscribe({
            next: (certificate: FishingCapacityCertificateEditDTO) => {
                this.model = certificate;
                this.isHolderPerson = this.model.isHolderPerson!;
                this.fillForm();
            }
        });
    }

    public saveBtnClicked(action: IActionInfo, dialogClose: DialogCloseCallback): void {
        this.form.markAllAsTouched();
        if (this.form.valid) {
            this.fillModel();
            CommonUtils.sanitizeModelStrings(this.model);

            this.service.editCapacityCertificate(this.model).subscribe({
                next: () => {
                    dialogClose();
                }
            });
        }
    }

    public cancelBtnClicked(action: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public dialogButtonClicked(action: IActionInfo, dialogClose: DialogCloseCallback): void {
        if (action.id === 'print') {
            if (this.readOnly) {
                this.service.downloadFishingCapacityCertificate(this.id).subscribe({
                    next: () => {
                        // nothing to do
                    }
                });
            }
            else {
                this.form.markAllAsTouched();
                if (this.form.valid) {
                    this.fillModel();
                    CommonUtils.sanitizeModelStrings(this.model);

                    this.service.editCapacityCertificate(this.model).subscribe({
                        next: () => {
                            this.service.downloadFishingCapacityCertificate(this.id).subscribe({
                                next: () => {
                                    dialogClose();
                                }
                            });
                        }
                    });
                }
            }
        }
    }

    public setData(data: DialogParamsModel, wrapperData: DialogWrapperData): void {
        this.id = data?.id;
        this.readOnly = data?.isReadonly ?? false;
    }

    private buildForm(): void {
        this.form = new FormGroup({
            numberControl: new FormControl({ value: '', disabled: true }),
            tonnageControl: new FormControl({ value: '', disabled: true }),
            powerControl: new FormControl({ value: '', disabled: true }),
            validityDateControl: new FormControl({ value: '', disabled: true }),
            duplicateOfNumberControl: new FormControl({ value: '', disabled: true }),
            holderControl: new FormControl(null),
            addressesControl: new FormControl(null),
            commentsControl: new FormControl('', Validators.maxLength(4000))
        });
    }

    private fillForm(): void {
        this.form.get('numberControl')!.setValue(this.model.certificateNum);
        this.form.get('tonnageControl')!.setValue(this.model.grossTonnage!.toFixed(2));
        this.form.get('powerControl')!.setValue(this.model.power!.toFixed(2));
        this.form.get('validityDateControl')!.setValue(new DateRangeData({ start: this.model.validFrom, end: this.model.validTo }));
        this.form.get('duplicateOfNumberControl')!.setValue(this.model.duplicateOfCertificateNum);
        this.form.get('commentsControl')!.setValue(this.model.comments);

        if (this.isHolderPerson) {
            this.form.get('holderControl')!.setValue(this.model.person);
            this.isIdReadOnly = this.model.person?.egnLnc?.egnLnc?.includes('migrate') !== true;
        }
        else {
            this.form.get('holderControl')!.setValue(this.model.legal);
            this.isIdReadOnly = this.model.legal?.eik?.includes('migrate') !== true;
        }

        this.form.get('addressesControl')!.setValue(this.model.addresses);

        if (this.readOnly) {
            this.form.disable();
        }
    }

    private fillModel(): void {
        this.model.comments = this.form.get('commentsControl')!.value;

        this.model.isHolderPerson = this.isHolderPerson;

        if (this.isHolderPerson) {
            this.model.person = this.form.get('holderControl')!.value;
        }
        else {
            this.model.legal = this.form.get('holderControl')!.value;
        }

        this.model.addresses = this.form.get('addressesControl')!.value;
    }
}