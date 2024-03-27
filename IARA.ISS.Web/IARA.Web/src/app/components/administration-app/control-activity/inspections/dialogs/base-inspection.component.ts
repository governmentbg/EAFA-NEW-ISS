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
                            error: (err: HttpErrorResponse) => {
                                this.handleErrorResponse(err);
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
            this.service.downloadReport(this.id!, this.model.reportNum!).subscribe();
        }
        else if (actionInfo.id === 'flux') {
            this.service.downloadFluxXml(this.id!, this.model.inspectionType!).subscribe();
        }
        else if (actionInfo.id === 'more-corrections-needed' && this.canEditNumber) {
            this.model.id = this.id;

            this.service.sendForFurtherCorrections(this.mapToDraft()).subscribe({
                next: () => {
                    dialogClose(this.model);
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

        const camelCaseModel = CommonUtils.convertKeysToCamelCase(this.model);

        draft.json = JSON.stringify(camelCaseModel);

        return draft;
    }

    private handleErrorResponse(response: HttpErrorResponse): void {
        if (response.error?.code === ErrorCode.NotInspector) {
            this.snackbar.open(this.translate.getValue('inspections.not-inspector'));
        }
        else if (response.error?.code === ErrorCode.InspectionReportNumAlreadyExists) {
            this.reportNumAlreadyExistsError = true;
            this.form.get('generalInfoControl')!.setErrors({ reportNumAlreadyExists: true });
            this.validityCheckerGroup.validate();
        }
    }
}