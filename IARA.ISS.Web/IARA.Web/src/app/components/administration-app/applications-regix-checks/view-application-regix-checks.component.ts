import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';

import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { DialogParamsModel } from '@app/models/common/dialog-params.model';
import { ApplicationRegixChecksService } from '@app/services/administration-app/application-regix-checks.service';
import { ApplicationRegixCheckRequestEditDTO } from '@app/models/generated/dtos/ApplicationRegixCheckRequestEditDTO';

@Component({
    selector: 'view-application-regix-checks',
    templateUrl: './view-application-regix-checks.component.html'
})
export class ViewApplicationRegixChecksComponent implements IDialogComponent, OnInit {
    public form!: FormGroup;

    public requestContent: Record<string, unknown> | undefined;
    public responseContent: Record<string, unknown> | undefined;
    public expectedResponseContent: Record<string, unknown> | undefined;

    private id!: number;
    private regixCheck!: ApplicationRegixCheckRequestEditDTO;

    private service: ApplicationRegixChecksService;

    public constructor(service: ApplicationRegixChecksService) {
        this.service = service;

        this.buildForm();
    }

    public ngOnInit(): void {
        this.service.get(this.id).subscribe({
            next: (regixCheck: ApplicationRegixCheckRequestEditDTO) => {
                this.regixCheck = regixCheck;
                this.fillForm();
            }
        });
    }

    public setData(data: DialogParamsModel, wrapperData: DialogWrapperData): void {
        this.id = data.id;
    }

    public dialogButtonClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public saveBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public cancelBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    private buildForm(): void {
        this.form = new FormGroup({
            applicationIdControl: new FormControl(),
            applicationTypeControl: new FormControl(),
            webServiceNameControl: new FormControl(),
            remoteAddressControl: new FormControl(),
            requestDateTimeControl: new FormControl(),
            responseStatusControl: new FormControl(),
            responseDateTimeControl: new FormControl(),
            errorLevelControl: new FormControl(),
            attemptsControl: new FormControl(),
            errorDescriptionControl: new FormControl()
        });
    }

    private fillForm(): void {
        this.form.get('applicationIdControl')!.setValue(this.regixCheck.applicationId);
        this.form.get('applicationTypeControl')!.setValue(this.regixCheck.applicationType);
        this.form.get('webServiceNameControl')!.setValue(this.regixCheck.webServiceName);
        this.form.get('remoteAddressControl')!.setValue(this.regixCheck.remoteAddress);
        this.form.get('requestDateTimeControl')!.setValue(this.regixCheck.requestDateTime);
        this.form.get('responseStatusControl')!.setValue(this.regixCheck.responseStatus);
        this.form.get('responseDateTimeControl')!.setValue(this.regixCheck.responseDateTime);
        this.form.get('errorLevelControl')!.setValue(this.regixCheck.errorLevel);
        this.form.get('attemptsControl')!.setValue(this.regixCheck.attempts);
        this.form.get('errorDescriptionControl')!.setValue(this.regixCheck.errorDescription);

        if (this.regixCheck.requestContent) {
            this.requestContent = JSON.parse(this.regixCheck.requestContent);
        }

        if (this.regixCheck.responseContent) {
            this.responseContent = JSON.parse(this.regixCheck.responseContent);
        }

        if (this.regixCheck.expectedResponseContent) {
            this.expectedResponseContent = JSON.parse(this.regixCheck.expectedResponseContent);
        }

        this.form.disable();
    }
}