import { AfterViewInit, Component, OnInit } from "@angular/core";
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { IReportService } from '@app/interfaces/administration-app/report.interface';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { NReportParameterEditDTO } from '@app/models/generated/dtos/NReportParameterEditDTO';
import { ReportParameterDTO } from '@app/models/generated/dtos/ReportParameterDTO';
import { ReportAdministrationService } from '@app/services/administration-app/report-administration.service';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { ReportDefinitionDialogParams } from './models/report-definition-dialog-params.model';

@Component({
    selector: 'edit-report-definition',
    templateUrl: './edit-report-definition.component.html'
})
export class EditReportDefinitionComponent implements IDialogComponent, OnInit, AfterViewInit {
    public model!: ReportParameterDTO;
    public availableParameters: NomenclatureDTO<number>[] = [];
    public parameterPropertiesGroup: FormGroup;

    private readonly reportService: IReportService;

    private isAddDialog: boolean;

    public constructor(reportService: ReportAdministrationService) {
        this.reportService = reportService;
        this.isAddDialog = false;

        this.parameterPropertiesGroup = new FormGroup({
            isMandatoryControl: new FormControl(false, Validators.required),
            defaultValueControl: new FormControl(null, Validators.maxLength(4000)),
            patternControl: new FormControl(null, Validators.maxLength(200)),
            errorMessageControl: new FormControl(null, Validators.maxLength(200)),
            orderNumberControl: new FormControl(),
            codeControl: new FormControl(null, Validators.maxLength(200)),
            parameterNameControl: new FormControl(null, Validators.required),
        });
    }

    public async ngOnInit(): Promise<void> {
        this.availableParameters = await this.reportService.getAvailableNParameters().toPromise();

        if (!this.isAddDialog) {
            const wantedParameter: NomenclatureDTO<number> = this.availableParameters.filter(parameter => parameter.value === this.model.parameterId)[0];
            this.parameterPropertiesGroup.controls.parameterNameControl.setValue(wantedParameter);

            this.fillForm(this.model);
        }
    }

    public ngAfterViewInit(): void {
        this.parameterPropertiesGroup.controls.parameterNameControl.valueChanges.subscribe({
            next: (parameter: NomenclatureDTO<number> | undefined) => {
                if (parameter !== null && parameter !== undefined && typeof parameter !== 'string') {
                    this.reportService.getNParameter(parameter.value!).subscribe({
                        next: (result: NReportParameterEditDTO) => {
                            this.fillForm(result);
                        }
                    });
                }
                else {
                    this.parameterPropertiesGroup.get('codeControl')!.setValue(undefined);
                }
            }
        });
    }

    public setData(data: ReportDefinitionDialogParams, wrapperData: DialogWrapperData): void {
        //edit
        if (data !== undefined) {
            if (data.viewMode) this.parameterPropertiesGroup.disable();

            this.model = data.parameter;
            this.fillForm(this.model);
        }
        //add
        else {
            this.isAddDialog = true;
            this.model = new ReportParameterDTO();
        }
    }

    public saveBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        this.parameterPropertiesGroup.markAllAsTouched();
        if (this.parameterPropertiesGroup.valid) {
            this.model = this.fillModel(this.parameterPropertiesGroup);
            dialogClose(this.model);
        }
    }

    public dialogButtonClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public cancelBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    private fillModel(formGroup: FormGroup): ReportParameterDTO {
        this.model.isMandatory = formGroup.controls.isMandatoryControl.value;
        this.model.defaultValue = formGroup.controls.defaultValueControl.value;
        this.model.pattern = formGroup.controls.patternControl.value;
        this.model.errorMessage = formGroup.controls.errorMessageControl.value;
        this.model.code = formGroup.controls.codeControl.value;

        let orderNumberValueAsNumber: number;
        const orderNumberValue: string = formGroup.controls.orderNumberControl.value;
        if (!CommonUtils.isNullOrEmpty(orderNumberValue)) {
            orderNumberValueAsNumber = Number(orderNumberValue);
        }
        this.model.orderNumber = isNaN(orderNumberValueAsNumber!) ? undefined : orderNumberValueAsNumber!;

        this.model.parameterId = (formGroup.controls.parameterNameControl.value as NomenclatureDTO<number>).value;
        this.model.parameterName = (formGroup.controls.parameterNameControl.value as NomenclatureDTO<number>).displayName;
        this.model.isActive = true;

        return this.model;
    }

    private fillForm(model: ReportParameterDTO | NReportParameterEditDTO): void {
        if (model instanceof ReportParameterDTO) {
            this.parameterPropertiesGroup.controls.isMandatoryControl.setValue(model.isMandatory);
            this.parameterPropertiesGroup.controls.orderNumberControl.setValue(model.orderNumber);
            const parameter: NomenclatureDTO<number> = this.availableParameters.find(x => x.value === model.parameterId)!;
            this.parameterPropertiesGroup.controls.parameterNameControl.setValue(parameter);
        }

        this.parameterPropertiesGroup.controls.defaultValueControl.setValue(model.defaultValue);
        this.parameterPropertiesGroup.controls.patternControl.setValue(model.pattern);
        this.parameterPropertiesGroup.controls.errorMessageControl.setValue(model.errorMessage);
        this.parameterPropertiesGroup.controls.codeControl.setValue(model.code);
    }
}