import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { DialogParamsModel } from '@app/models/common/dialog-params.model';
import { PersonReportInfoDTO } from '@app/models/generated/dtos/PersonReportInfoDTO';
import { PersonReportsService } from '@app/services/administration-app/person-reports.service';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';

@Component({
    selector: 'persons-report-info',
    templateUrl: './persons-report-info.component.html',
})
export class PersonsReportInfoComponent implements OnInit, IDialogComponent {
    public form!: FormGroup;

    private id!: number;
    private model!: PersonReportInfoDTO;
    private service!: PersonReportsService;

    public constructor(service: PersonReportsService) {
        this.service = service;
        this.buildForm();
    }

    public ngOnInit(): void {
        this.service.getPersonReport(this.id).subscribe({
            next: (result: PersonReportInfoDTO) => {
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
            regixDataControl: new FormControl(),
            addressControl: new FormControl(),
            commentsControl: new FormControl(),
            documentsControl: new FormControl()
        });

        this.form.disable();
    }

    private fillForm(): void {
        this.form.controls.regixDataControl.setValue(this.model.regixPersonData);
        this.form.controls.addressControl.setValue(this.model.addressRegistrations);
        this.form.controls.commentsControl.setValue(this.model.comments);
    }
}