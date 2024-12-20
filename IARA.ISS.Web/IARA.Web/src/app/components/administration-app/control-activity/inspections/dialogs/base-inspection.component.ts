import { FormGroup } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';

import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { PageCodeEnum } from '@app/enums/page-code.enum';
import { InspectionEditDTO } from '@app/models/generated/dtos/InspectionEditDTO';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { RequestProperties } from '@app/shared/services/request-properties';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { CommonNomenclatures } from '@app/services/common-app/common-nomenclatures.service';
import { InspectionsService } from '@app/services/administration-app/inspections.service';
import { InspectionDraftDTO } from '@app/models/generated/dtos/InspectionDraftDTO';
import { HttpErrorResponse } from '@angular/common/http';
import { ErrorCode } from '@app/models/common/exception.model';
import { ValidityCheckerGroupDirective } from '@app/shared/directives/validity-checker/validity-checker-group.directive';
import { Component, ViewChild } from '@angular/core';
import { InspectionTypesEnum } from '@app/enums/inspection-types.enum';
import { InspectionDialogParamsModel } from '../models/inspection-dialog-params.model';
import { TLConfirmDialog } from '@app/shared/components/confirmation-dialog/tl-confirm-dialog';
import { InspectionStatesEnum } from '@app/enums/inspection-states.enum';

@Component({
    template: ''
})
export abstract class BaseInspectionsComponent implements IDialogComponent {
    public pageCode: PageCodeEnum = PageCodeEnum.Inspections;

    public form!: FormGroup;

    public service: InspectionsService;
    public viewMode: boolean = false;
    public canEditNumber: boolean = false;
    public reportNumAlreadyExistsError: boolean = false;
    public isInspectionLockedError: boolean = false;
    public inspectionTypesEnum: typeof InspectionTypesEnum = InspectionTypesEnum;

    @ViewChild(ValidityCheckerGroupDirective)
    public validityCheckerGroup!: ValidityCheckerGroupDirective;

    protected translate: FuseTranslationLoaderService;
    protected nomenclatures: CommonNomenclatures;
    protected confirmDialog: TLConfirmDialog;
    protected snackbar: MatSnackBar;

    protected id: number | undefined;
    protected isSaving: boolean = false;
    protected abstract model: InspectionEditDTO;
    protected inspectionCode!: InspectionTypesEnum;

    public constructor(
        service: InspectionsService,
        translate: FuseTranslationLoaderService,
        nomenclatures: CommonNomenclatures,
        confirmDialog: TLConfirmDialog,
        snackbar: MatSnackBar
    ) {
        this.service = service;
        this.translate = translate;
        this.nomenclatures = nomenclatures;
        this.confirmDialog = confirmDialog;
        this.snackbar = snackbar;

        this.buildForm();
    }

    public setData(data: InspectionDialogParamsModel, buttons: DialogWrapperData): void {
        if (data !== undefined && data !== null) {
            this.id = data.id;
            this.viewMode = data.viewMode;
            this.canEditNumber = data.canEditNumber;
            this.pageCode = data.pageCode;

            //Съобщението за грешка да се показва само за инспекции, създадени от същия инспектор, които не са подписани
            this.isInspectionLockedError = data.isReportLocked && !data.canEditLockedInspections && data.userIsSameAsInspector && data.pageCode !== PageCodeEnum.SignInspections;
        }
    }

    public saveBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        if (this.viewMode) {
            dialogClose();
        }

        this.isSaving = true;
        this.form.markAllAsTouched();
        this.form.updateValueAndValidity({ onlySelf: true });

        setTimeout(() => {
            this.validityCheckerGroup.validate();
            this.isSaving = false;
        });

        if (this.form.valid) {
            this.fillModel();
            CommonUtils.sanitizeModelStrings(this.model);

            let title: string = '';
            let message: string = '';

            if (this.inspectionCode === this.inspectionTypesEnum.CWO || this.inspectionCode === this.inspectionTypesEnum.OFS) {
                title = this.translate.getValue('inspections.submit-report-confirm-dialog-title');
                message = this.translate.getValue('inspections.submit-report-confirm-dialog-message');
            }
            else {
                title = this.translate.getValue('inspections.submit-inspection-confirm-dialog-title');
                message = this.translate.getValue('inspections.submit-inspection-confirm-dialog-message');
            }

            this.confirmDialog.open({
                title: title,
                message: message,
                okBtnLabel: this.translate.getValue('inspections.submit-inspection-confirm-dialog-ok-btn-label')
            }).subscribe({
                next: (ok: boolean) => {
                    if (ok) {
                        this.service.submit(this.model).subscribe({
                            next: (id: number) => {
                                this.id = id;
                                this.model.id = id;
                                dialogClose(dialogClose);
                            },
                            error: (errorResponse: HttpErrorResponse) => {
                                this.handleSubmitErrorResponse(errorResponse);
                            }
                        });
                    }
                }
            });
        }
        else {
            this.snackbar.open(this.translate.getValue('inspections.submit-invalid'), undefined, {
                duration: RequestProperties.DEFAULT.showExceptionDurationErr,
                panelClass: RequestProperties.DEFAULT.showExceptionColorClassErr
            });
        }
    }

    public cancelBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public dialogButtonClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        if (actionInfo.id === 'draft') {
            if (this.viewMode) {
                dialogClose();
            }

            this.fillModel();
            CommonUtils.sanitizeModelStrings(this.model);

            if (this.id !== undefined && this.id !== undefined) {
                this.service.edit(this.mapToDraft()).subscribe({
                    next: (id: number) => {
                        this.id = id;
                        this.model.id = id;
                        dialogClose(this.model);
                    },
                    error: (errorResponse: HttpErrorResponse) => {
                        this.handleErrorResponse(errorResponse);
                    }
                });
            }
            else {
                this.service.add(this.mapToDraft()).subscribe({
                    next: (id: number) => {
                        this.id = id;
                        this.model.id = id;
                        dialogClose(this.model);
                    }
                });
            }
        }
        else if (actionInfo.id === 'print') {
            this.service.downloadReport(this.id!, this.model.reportNum!).subscribe({
                next: () => {
                    //nothing to do
                },
                error: (errorResponse: HttpErrorResponse) => {
                    this.handleErrorResponse(errorResponse);
                }
            });
        }
        else if (actionInfo.id === 'send-to-flux') {
            this.service.sendInspectionToFlux(this.id!).subscribe({
                next: () => {
                    //nothing to do
                },
                error: (errorResponse: HttpErrorResponse) => {
                    this.handleErrorResponse(errorResponse);
                }
            });
        }
        else if (actionInfo.id === 'more-corrections-needed') {
            this.model.id = this.id;
            const draft: InspectionDraftDTO = this.mapToDraft();

            this.service.sendForFurtherCorrections(draft).subscribe({
                next: () => {
                    dialogClose(this.model);
                },
                error: (errorResponse: HttpErrorResponse) => {
                    this.handleErrorResponse(errorResponse);
                }
            });
        }
    }

    protected abstract buildForm(): void;
    protected abstract fillForm(): void;
    protected abstract fillModel(): void;

    private mapToDraft(): InspectionDraftDTO {
        const draft = new InspectionDraftDTO({
            actionsTaken: this.model.actionsTaken,
            administrativeViolation: this.model.administrativeViolation,
            byEmergencySignal: this.model.byEmergencySignal,
            endDate: this.model.endDate,
            files: this.model.files,
            id: this.model.id,
            inspectionType: this.model.inspectionType,
            inspectorComment: this.model.inspectorComment,
            startDate: this.model.startDate,
            reportNumber: this.model.reportNum
        });

        this.model.files = undefined;
        this.model.inspectionState = InspectionStatesEnum.Draft;

        const camelCaseModel = CommonUtils.convertKeysToCamelCase(this.model);

        draft.json = JSON.stringify(camelCaseModel);

        return draft;
    }

    private handleSubmitErrorResponse(response: HttpErrorResponse): void {
        if (response.error?.code === ErrorCode.NotInspector) {
            this.snackbar.open(this.translate.getValue('inspections.not-inspector'));
        }
        else if (response.error?.code === ErrorCode.InspectionReportNumAlreadyExists) {
            this.reportNumAlreadyExistsError = true;
            this.form.get('generalInfoControl')!.setErrors({ reportNumAlreadyExists: true });
            this.validityCheckerGroup.validate();
        }
        else if (response.error?.code == ErrorCode.InvalidInspectionType) {
            const message = this.translate.getValue('inspections.cannot-edit-inspection-of-this-inspection-type');
            this.snackbar.open(message, undefined, {
                duration: RequestProperties.DEFAULT.showExceptionDurationErr,
                panelClass: RequestProperties.DEFAULT.showExceptionColorClassErr
            });
        }
        else if (response.error?.code === ErrorCode.CannotEditInspectionAfterLockHours) {
            const message = this.translate.getValue('inspections.cannot-edit-inspection-after-lock-hours-error');
            this.snackbar.open(message, undefined, {
                duration: RequestProperties.DEFAULT.showExceptionDurationErr,
                panelClass: RequestProperties.DEFAULT.showExceptionColorClassErr
            });
        }
    }

    private handleErrorResponse(response: HttpErrorResponse): void {
        if (response.error !== undefined && response.error !== null) {
            let message: string = '';

            if (response.error?.code === ErrorCode.AlreadySubmitted) {
                message = this.translate.getValue('inspections.inspection-already-submitted');
            }
            else if (response.error?.code === ErrorCode.InvalidInspectionType) {
                message = this.translate.getValue('inspections.cannot-edit-inspection-of-this-inspection-type');
            }
            else if (response.error?.code === ErrorCode.SendFLUXISRFailed) {
                message = this.translate.getValue('inspections.inspection-send-to-flux-error');
            }
            else if (response.error?.code === ErrorCode.InspectionNotSigned) {
                message = this.translate.getValue('inspections.cannot-send-to-flux-not-signed-inspection-error')
            }
            else if (response.error?.code === ErrorCode.CannotEditInspectionAfterLockHours) {
                message = this.translate.getValue('inspections.cannot-edit-inspection-after-lock-hours-error');
            }

            if (message !== undefined && message !== null && message !== '') {
                this.snackbar.open(message, undefined, {
                    duration: RequestProperties.DEFAULT.showExceptionDurationErr,
                    panelClass: RequestProperties.DEFAULT.showExceptionColorClassErr
                });
            }
        }
    }
}