import { Component, OnInit } from '@angular/core';

import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { FormControl, FormGroup } from '@angular/forms';
import { FluxVmsRequestsService } from '@app/services/administration-app/flux-vms-requests.service';
import { FLUXVMSRequestEditDTO } from '@app/models/generated/dtos/FLUXVMSRequestEditDTO';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { ViewFluxVmsRequestsDialogParams } from './models/view-flux-vms-requests-dialog-params.model';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { FluxAcdrReportStatusEnum } from '@app/enums/flux-acdr-report-status.enum';

@Component({
    selector: 'view-flux-vms-requests',
    templateUrl: './view-flux-vms-requests.component.html'
})
export class ViewFluxVmsRequestsComponent implements IDialogComponent, OnInit {
    public form!: FormGroup;

    public requestContent: Record<string, unknown> | undefined;
    public responseContent: Record<string, unknown> | undefined;

    public canDownload: boolean = false;

    public readonly icIconSize: number = CommonUtils.IC_ICON_SIZE;

    private id!: number;
    private acdrId: number | undefined;
    private request!: FLUXVMSRequestEditDTO;

    private service: FluxVmsRequestsService;
    private translate: FuseTranslationLoaderService;

    public constructor(
        service: FluxVmsRequestsService,
        translate: FuseTranslationLoaderService
    ) {
        this.service = service;
        this.translate = translate;

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

    public downloadContent(): void {
        if (this.acdrId !== undefined && this.acdrId !== null) {
            this.service.downloadAcdrRequestContent(this.acdrId).subscribe();
        }
    }

    public setData(data: ViewFluxVmsRequestsDialogParams, wrapperData: DialogWrapperData): void {
        this.id = data.id;
        this.acdrId = data.acdrId;

        if (data.acdrId !== undefined && data.acdrId !== null && (data.reportStatus === FluxAcdrReportStatusEnum.MANUAL || data.reportStatus === FluxAcdrReportStatusEnum.GENERATED)) {
            this.canDownload = true;
        }
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