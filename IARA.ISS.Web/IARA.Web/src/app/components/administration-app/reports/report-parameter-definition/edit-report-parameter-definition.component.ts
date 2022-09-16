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
import { DateRangeData } from '@app/shared/components/input-controls/tl-date-range/tl-date-range.component';

@Component({
    selector: 'edit-report-parameter-definition',
    templateUrl: './edit-report-parameter-definition.component.html'
})
export class EditReportParameterDefinitionComponent implements IDialogComponent {
    public parameterPropertiesGroup: FormGroup;
    public dataTypes: string[];

    private readonly reportService: IReportService;

    private model!: NReportParameterEditDTO;
    private isAddDialog: boolean;

    public constructor(reportService: ReportAdministrationService) {
        this.reportService = reportService;
        this.isAddDialog = false;

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
            dateRangeControl: new FormControl(null, Validators.required),
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
                    }
                })
            }
            else {
                this.reportService.addNParameter(this.model).subscribe({
                    next: (result: number) => {
                        this.model.id = result;
                        dialogClose(this.model);
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
        this.model.dateFrom = (formGroup.controls.dateRangeControl.value as DateRangeData)?.start;
        this.model.dateTo = (formGroup.controls.dateRangeControl.value as DateRangeData)?.end;

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

        this.parameterPropertiesGroup.controls.dateRangeControl.setValue(new DateRangeData({ start: model.dateFrom, end: model.dateTo })); 
    }
}