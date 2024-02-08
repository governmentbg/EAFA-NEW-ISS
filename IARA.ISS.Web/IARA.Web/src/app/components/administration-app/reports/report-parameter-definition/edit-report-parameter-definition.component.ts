import { Component } from "@angular/core";
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { BaseDialogParamsModel } from '@app/models/common/base-dialog-params.model';
import { NReportParameterEditDTO } from '@app/models/generated/dtos/NReportParameterEditDTO';
import { ReportAdministrationService } from '@app/services/administration-app/report-administration.service';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { ReportParameterTypeEnum } from '@app/enums/report-parameter-type.enum';
import { IReportService } from '@app/interfaces/administration-app/report.interface';
import { HttpErrorResponse } from '@angular/common/http';
import { ErrorCode, ErrorModel } from '@app/models/common/exception.model';
import { RequestProperties } from '@app/shared/services/request-properties';
import { MatSnackBar } from '@angular/material/snack-bar';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { TLSnackbar } from '@app/shared/components/snackbar/tl.snackbar';

@Component({
    selector: 'edit-report-parameter-definition',
    templateUrl: './edit-report-parameter-definition.component.html'
})
export class EditReportParameterDefinitionComponent implements IDialogComponent {
    public parameterPropertiesGroup: FormGroup;
    public dataTypes: string[];

    private readonly reportService: IReportService;
    private readonly translate: FuseTranslationLoaderService;
    private readonly snackbar: TLSnackbar;

    private model!: NReportParameterEditDTO;
    private isAddDialog: boolean;

    public constructor(
        reportService: ReportAdministrationService,
        translate: FuseTranslationLoaderService,
        snackbar: TLSnackbar
    ) {
        this.reportService = reportService;
        this.isAddDialog = false;
        this.translate = translate;
        this.snackbar = snackbar;

        this.dataTypes = [
            ReportParameterTypeEnum[ReportParameterTypeEnum.Int],
            ReportParameterTypeEnum[ReportParameterTypeEnum.Decimal],
            ReportParameterTypeEnum[ReportParameterTypeEnum.Date],
            ReportParameterTypeEnum[ReportParameterTypeEnum.Time],
            ReportParameterTypeEnum[ReportParameterTypeEnum.DateTime],
            ReportParameterTypeEnum[ReportParameterTypeEnum.String],
            ReportParameterTypeEnum[ReportParameterTypeEnum.Nomenclature],
            ReportParameterTypeEnum[ReportParameterTypeEnum.Year],
            ReportParameterTypeEnum[ReportParameterTypeEnum.Month]
        ];

        this.parameterPropertiesGroup = new FormGroup({
            nameControl: new FormControl(null, Validators.compose([
                Validators.required, Validators.maxLength(500)
            ])),
            codeControl: new FormControl(null, Validators.compose([
                Validators.required, Validators.maxLength(500)
            ])),
            descriptionControl: new FormControl(null, Validators.maxLength(1000)),
            typeControl: new FormControl(null, Validators.compose([
                Validators.required, Validators.maxLength(50)
            ])),
            nomenclatureSQLControl: new FormControl(),
            defaultValueControl: new FormControl(null, Validators.maxLength(4000)),
            patternControl: new FormControl(null, Validators.maxLength(200)),
            errorMessageControl: new FormControl(null, Validators.maxLength(200))
        });
    }

    public setData(data: BaseDialogParamsModel, wrapperData: DialogWrapperData): void {
        //edit
        if (data !== undefined) {
            if (data.viewMode) {
                this.parameterPropertiesGroup.disable();
            }

            this.reportService.getNParameter(data.id).subscribe({
                next: (result: NReportParameterEditDTO) => {
                    this.model = result;
                    this.fillForm(this.model);
                }
            })
        }
        //add
        else {
            this.isAddDialog = true;
            this.model = new NReportParameterEditDTO();
        }
    }

    public saveBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        this.parameterPropertiesGroup.markAllAsTouched();
        if (this.parameterPropertiesGroup.valid) {
            this.model = this.fillModel(this.parameterPropertiesGroup);
            this.model = CommonUtils.sanitizeModelStrings(this.model);

            if (!this.isAddDialog) {
                this.reportService.editNParameter(this.model).subscribe({
                    next: () => {
                        dialogClose(this.model);
                    },
                    error: (response: HttpErrorResponse) => {
                        this.handleSQLExceptions(response);
                    }
                })
            }
            else {
                this.reportService.addNParameter(this.model).subscribe({
                    next: (result: number) => {
                        this.model.id = result;
                        dialogClose(this.model);
                    },
                    error: (response: HttpErrorResponse) => {
                        this.handleSQLExceptions(response);
                    }
                })
            }
        }
    }

    public dialogButtonClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public cancelBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    private fillModel(formGroup: FormGroup): NReportParameterEditDTO {
        this.model.name = formGroup.controls.nameControl.value;
        this.model.code = formGroup.controls.codeControl.value;
        this.model.description = formGroup.controls.descriptionControl.value;
        this.model.defaultValue = formGroup.controls.defaultValueControl.value;
        this.model.nomenclatureSQL = formGroup.controls.nomenclatureSQLControl.value;
        this.model.pattern = formGroup.controls.patternControl.value;
        this.model.errorMessage = formGroup.controls.errorMessageControl.value;
        this.model.dataType = ReportParameterTypeEnum[formGroup.controls.typeControl.value as keyof typeof ReportParameterTypeEnum];

        return this.model;
    }

    private fillForm(model: NReportParameterEditDTO): void {
        this.parameterPropertiesGroup.controls.nameControl.setValue(model.name);
        this.parameterPropertiesGroup.controls.codeControl.setValue(model.code);
        this.parameterPropertiesGroup.controls.descriptionControl.setValue(model.description);
        this.parameterPropertiesGroup.controls.defaultValueControl.setValue(model.defaultValue);
        this.parameterPropertiesGroup.controls.nomenclatureSQLControl.setValue(model.nomenclatureSQL);
        this.parameterPropertiesGroup.controls.patternControl.setValue(model.pattern);
        this.parameterPropertiesGroup.controls.errorMessageControl.setValue(model.errorMessage);

        if (model.dataType !== undefined) {
            this.parameterPropertiesGroup.controls.typeControl.setValue(ReportParameterTypeEnum[model.dataType]);
        }
    }

    private handleSQLExceptions(response: HttpErrorResponse): void {
        if (response !== null && response !== undefined && response.error !== null && response.error !== undefined) {
            if (response.error.messages !== null && response.error.messages !== undefined) {
                const messages: string[] = response.error.messages;
                if (messages.length !== 0) {
                    this.snackbar.errorModel(response.error as ErrorModel, RequestProperties.DEFAULT);
                }
                else {
                    this.snackbar.error(this.translate.getValue('service.an-error-occurred-in-the-app'), RequestProperties.DEFAULT.showExceptionDurationErr, RequestProperties.DEFAULT.showExceptionColorClassErr);
                }
            }
            if ((response.error as ErrorModel).code === ErrorCode.InvalidSqlQuery) {
                this.parameterPropertiesGroup.get('nomenclatureSQLControl')!.setErrors({ 'invalidSqlQuery': true });
            }
        }
    }
}