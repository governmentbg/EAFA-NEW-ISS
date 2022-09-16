import { Component, OnInit } from '@angular/core';

import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { FormControl, FormGroup } from '@angular/forms';
import { DialogParamsModel } from '@app/models/common/dialog-params.model';
import { FluxVmsRequestsService } from '@app/services/administration-app/flux-vms-requests.service';
import { FLUXVMSRequestEditDTO } from '@app/models/generated/dtos/FLUXVMSRequestEditDTO';

@Component({
    selector: 'view-flux-vms-requests',
    templateUrl: './view-flux-vms-requests.component.html'
})
export class ViewFluxVmsRequestsComponent implements IDialogComponent, OnInit {
    public form!: FormGroup;

    public requestContent: Record<string, unknown> | undefined;
    public responseContent: Record<string, unknown> | undefined;

    private id!: number;
    private request!: FLUXVMSRequestEditDTO;

    private service: FluxVmsRequestsService;

    public constructor(service: FluxVmsRequestsService) {
        this.service = service;

        this.buildForm();
    }

    public ngOnInit(): void {
        this.service.get(this.id).subscribe({
            next: (request: FLUXVMSRequestEditDTO) => {
                this.request = request;
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
            domainNameControl: new FormControl(),
            webServiceNameControl: new FormControl(),
            isOutgoingControl: new FormControl(),
            requestUUIDControl: new FormControl(),
            requestDateTimeControl: new FormControl(),
            responseStatusControl: new FormControl(),
            responseUUIDControl: new FormControl(),
            responseDateTimeControl: new FormControl(),
            errorDescriptionControl: new FormControl()
        });
    }

    private fillForm(): void {
        this.form.get('domainNameControl')!.setValue(this.request.domainName);
        this.form.get('webServiceNameControl')!.setValue(this.request.webServiceName);
        this.form.get('isOutgoingControl')!.setValue(this.request.isOutgoing);
        this.form.get('requestUUIDControl')!.setValue(this.request.requestUUID);
        this.form.get('requestDateTimeControl')!.setValue(this.request.requestDateTime);
        this.form.get('responseStatusControl')!.setValue(this.request.responseStatus);
        this.form.get('responseUUIDControl')!.setValue(this.request.responseUUID);
        this.form.get('responseDateTimeControl')!.setValue(this.request.responseDateTime);
        this.form.get('errorDescriptionControl')!.setValue(this.request.errorDescription);

        if (this.request.requestContent) {
            this.requestContent = JSON.parse(this.request.requestContent);
        }
        if (this.request.responseContent) {
            this.responseContent = JSON.parse(this.request.responseContent);
        }

        this.form.disable();
    }
}