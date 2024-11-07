import { Component, OnInit } from "@angular/core";
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { IReportService } from '@app/interfaces/administration-app/report.interface';
import { BaseDialogParamsModel } from '@app/models/common/base-dialog-params.model';
import { ReportGroupDTO } from '@app/models/generated/dtos/ReportGroupDTO';
import { ReportAdministrationService } from '@app/services/administration-app/report-administration.service';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { ReportTypesEnum } from '@app/enums/reports-type.enum';
import { TLValidators } from '@app/shared/utils/tl-validators';
import { HttpErrorResponse } from '@angular/common/http';
import { ErrorCode } from '@app/models/common/exception.model';
import { TLError } from '@app/shared/components/input-controls/models/tl-error.model';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { GetControlErrorLabelTextCallback } from '@app/shared/components/input-controls/base-tl-control';

@Component({
    selector: 'edit-report-view',
    templateUrl: './edit-report-view.component.html'
})
export class EditReportViewComponent implements OnInit, IDialogComponent {
    public form: FormGroup;
    public groupTypes: string[];

    public groupNameErrorLabelTextMethod: GetControlErrorLabelTextCallback = this.groupNameErrorLabelText.bind(this);

    private id: number | undefined;
    private model!: ReportGroupDTO;
    private isAddDialog: boolean = false;

    private readonly reportService: IReportService;
    private readonly translate: FuseTranslationLoaderService;

    public constructor(
        reportService: ReportAdministrationService,
        translate: FuseTranslationLoaderService
    ) {
        this.reportService = reportService;
        this.translate = translate;

        this.form = this.buildForm();

        this.groupTypes = [
            ReportTypesEnum[ReportTypesEnum.SQL],
            ReportTypesEnum[ReportTypesEnum.Jasper]
        ];
    }

    public ngOnInit(): void {
        if (this.id !== undefined && this.id !== null) {
            this.reportService.getGroup(this.id).subscribe({
                next: (result: ReportGroupDTO) => {
                    this.model = result;
                    this.fillForm();
                }
            });
        }
        else {
            this.isAddDialog = true;
            this.model = new ReportGroupDTO();
        }
    }

    public saveBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        this.form.markAllAsTouched();
        if (this.form.valid) {
            this.fillModel();
            CommonUtils.sanitizeModelStrings(this.model);

            if (!this.isAddDialog) {
                this.reportService.editGroup(this.model).subscribe({
                    next: () => {
                        dialogClose(this.model);
                    },
                    error: (response: HttpErrorResponse) => {
                        this.handleErrorResponse(response);
                    }
                });
            }
            else {
                this.reportService.addGroup(this.model).subscribe({
                    next: (id: number) => {
                        this.model.id = id;
                        dialogClose(this.model);
                    },
                    error: (response: HttpErrorResponse) => {
                        this.handleErrorResponse(response);
                    }
                });
            }
        }
    }

    public setData(data: BaseDialogParamsModel | undefined, wrapperData: DialogWrapperData): void {
        if (data !== undefined && data !== null) {
            this.id = data.id;
        }
    }

    public dialogButtonClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public cancelBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public groupNameErrorLabelText(controlName: string, error: unknown, errorCode: string): TLError | undefined {
        if (controlName === 'nameControl') {
            if (errorCode === 'groupExists') {
                return new TLError({ type: 'error', text: this.translate.getValue('report-view.group-name-already-exist-error') });
            }
        }
        return undefined;
    }

    private buildForm(): FormGroup {
        return new FormGroup({
            nameControl: new FormControl(undefined, [Validators.required, Validators.maxLength(500)]),
            descriptionControl: new FormControl(undefined, [Validators.required, Validators.maxLength(4000)]),
            groupTypeControl: new FormControl(undefined, Validators.required),
            orderNumControl: new FormControl(undefined, [Validators.required, TLValidators.number(0, undefined, 0)])
        });
    }

    private fillModel(): void {
        this.model.name = this.form.get('nameControl')!.value;
        this.model.description = this.form.get('descriptionControl')!.value;
        this.model.groupType = this.form.get('groupTypeControl')!.value;
        this.model.orderNum = this.form.get('orderNumControl')!.value;
    }

    private fillForm(): void {
        this.form.get('nameControl')!.setValue(this.model.name);
        this.form.get('descriptionControl')!.setValue(this.model.description);
        this.form.get('groupTypeControl')!.setValue(this.model.groupType);
        this.form.get('orderNumControl')!.setValue(this.model.orderNum);
    }

    private handleErrorResponse(response: HttpErrorResponse): void {
        if (response !== undefined && response !== null) {
            if (response.error?.code === ErrorCode.ReportGroupNameAlreadyExists) {
                this.form.get('nameControl')!.setErrors({ 'groupExists': true });
                this.form.get('nameControl')!.markAsTouched();
            }
        }
    }
}