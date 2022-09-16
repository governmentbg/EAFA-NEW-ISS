import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';

import { DialogParamsModel } from '@app/models/common/dialog-params.model';
import { LegalEntityReportInfoDTO } from '@app/models/generated/dtos/LegalEntityReportInfoDTO';
import { PersonReportsService } from '@app/services/administration-app/person-reports.service';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { AddressTypesEnum } from '@app/enums/address-types.enum';

@Component({
    selector: 'legal-entity-report-info',
    templateUrl: './legal-entity-report-info.component.html',
})
export class LegalEntityReportInfoComponent implements OnInit, IDialogComponent {
    public form!: FormGroup;

    public readonly addressTypes: typeof AddressTypesEnum = AddressTypesEnum;

    private id!: number;
    private model!: LegalEntityReportInfoDTO;

    private service!: PersonReportsService;

    public constructor(service: PersonReportsService) {
        this.service = service;
        this.buildForm();
    }

    public ngOnInit(): void {
        this.service.getLegalEntityReport(this.id).subscribe({
            next: (result: LegalEntityReportInfoDTO) => {
                this.model = result;
                this.fillForm();
            }
        });
    }

    public setData(data: DialogParamsModel, wrapperData: DialogWrapperData): void {
        this.id = data.id;
    }

    public saveBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public cancelBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public dialogButtonClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    private buildForm(): void {
        this.form = new FormGroup({
            eikControl: new FormControl(),
            nameControl: new FormControl(),
            phoneControl: new FormControl(),
            emailControl: new FormControl(),
            postalCodeControl: new FormControl(),
            correspondenceAddressControl: new FormControl(),
            courtRegistrationAddressControl: new FormControl(),
        });

        this.form.disable();
    }

    private fillForm(): void {
        this.form.controls.eikControl.setValue(this.model.eik);
        this.form.controls.nameControl.setValue(this.model.legalName);
        this.form.controls.emailControl.setValue(this.model.email);
        this.form.controls.phoneControl.setValue(this.model.phone);
        this.form.controls.postalCodeControl.setValue(this.model.postalCode);
        this.form.controls.correspondenceAddressControl.setValue(this.model.correspondenceAddress);
        this.form.controls.courtRegistrationAddressControl.setValue(this.model.courtRegistrationAddress);
    }
}