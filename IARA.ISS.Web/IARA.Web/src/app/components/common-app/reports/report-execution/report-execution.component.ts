import { AfterViewInit, Component, Input, ViewChild } from '@angular/core';
import { FormControl, FormGroup, ValidatorFn, Validators } from '@angular/forms';
import { ReportParameterTypeEnum } from '@app/enums/report-parameter-type.enum';
import { ReportTypesEnum } from '@app/enums/reports-type.enum';
import { IBaseReportService } from '@app/interfaces/administration-app/base-report.interface';
import { ExecuteReportDTO } from '@app/models/generated/dtos/ExecuteReportDTO';
import { ExecutionParamDTO } from '@app/models/generated/dtos/ExecutionParamDTO';
import { ExecutionReportInfoDTO } from '@app/models/generated/dtos/ExecutionReportInfoDTO';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { ReportGridRequestDTO } from '@app/models/generated/dtos/ReportGridRequestDTO';
import { ReportParameterExecuteDTO } from '@app/models/generated/dtos/ReportParameterExecuteDTO';
import { IRemoteTLDatatableComponent } from '@app/shared/components/data-table/interfaces/tl-remote-datatable.interface';
import { TLDataTableComponent } from '@app/shared/components/data-table/tl-data-table.component';
import { BaseDataTableManager } from '@app/shared/utils/base-data-table.manager';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { DateUtils } from '@app/shared/utils/date.utils';
import { TLValidators } from '@app/shared/utils/tl-validators';
import { Observable } from 'rxjs';
import { ReportSchema } from '../models/report-schema.model';

export type ResultType = {
    propertyName: string;
    value: string;
};

@Component({
    selector: 'report-execution',
    templateUrl: './report-execution.component.html'
})
export class ReportExecutionComponent implements AfterViewInit {

    public constructor() {
        this.parametersFormGroup = new FormGroup({});
        this._report = new ExecuteReportDTO();
        this.executionModel = new ExecutionReportInfoDTO();
        this.isExecuteResultClicked = false;
    }

    public ngAfterViewInit(): void {
        this.gridManager = new BaseDataTableManager<ResultType>({
            tlDataTable: this.datatable,
            requestServiceMethod: this._reportService.executePagedQuery.bind(this._reportService)
        }, new ReportGridRequestDTO());
    }

    private gridManager!: BaseDataTableManager<ResultType>;

    private get GridRequest(): ReportGridRequestDTO {
        return this.gridManager.GridRequest as ReportGridRequestDTO;
    }

    @Input()
    public showParameters: boolean = true;

    public get reportId(): number | undefined {
        return this._report.id;
    }

    @Input()
    public set reportId(value: number | undefined) {
        if (value !== null && value !== undefined) {
            this._reportId = value;
            this._report.id = value;
            this.executionModel.reportId = value;
            this.getReport();
            this.zoomPercentageString = '100%';
            this.zoomPercentage = 100;
        }
    }

    public get report(): ExecuteReportDTO | undefined {
        return this._report;
    }

    @Input()
    public set report(value: ExecuteReportDTO | undefined) {
        if (value != undefined) {
            this.isExecuteResultClicked = false;
            this.reportDisplayColumns = [];

            this._reportId = value.id;
            this.parameters = value.parameters;
            this.sqlQuery = value.sqlQuery;

            this._report.name = value.name;
            this._report.reportGroupId = value.reportGroupId;
            this._report.reportType = value.reportType;
            this._report.sqlQuery = value.sqlQuery;
        }
    }

    public get sqlQuery(): string | undefined {
        return this._report.sqlQuery;
    }

    @Input()
    public set sqlQuery(value: string | undefined) {
        this.executionModel.sqlQuery = value;
        this._report.sqlQuery = value;
        this._report.reportType = ReportTypesEnum.SQL;
    }

    public get parameters(): ReportParameterExecuteDTO[] | undefined {
        return this._report.parameters;
    }

    @Input()
    public set parameters(value: ReportParameterExecuteDTO[] | undefined) {
        if (value != undefined) {
            const parameters = this.getParametersValues(value);
            const formGroup = this.buildParametersFormGroup(parameters);
            this.executionModel.parameters = parameters;
            this.parametersFormGroup = formGroup;
            this._report.parameters = value;
        }
    }


    public isExecuteResultClicked: boolean;
    public _report: ExecuteReportDTO;
    public parametersFormGroup: FormGroup;
    public parameterTypeEnum: typeof ReportParameterTypeEnum = ReportParameterTypeEnum;
    public reportType: typeof ReportTypesEnum = ReportTypesEnum;

    @ViewChild(TLDataTableComponent)
    public datatable!: IRemoteTLDatatableComponent;

    public reportDisplayColumns: ReportSchema[] = [];
    public executionModel!: ExecutionReportInfoDTO;

    @Input()
    public set reportService(value: IBaseReportService) {
        this._reportService = value;
        this.getReport();
    }

    private _reportService!: IBaseReportService;

    private _reportId: number | undefined;

    public getReport(): void {
        if (this._reportId != undefined) {
            this._reportService?.getExecuteReport(this._reportId).subscribe({
                next: (result: ExecuteReportDTO) => {
                    this.report = result;
                }
            });
        }
    }

    public executeReport(): void {
        this.parametersFormGroup.markAllAsTouched();
        if (this.parametersFormGroup.valid) {
            if (this._report!.reportType === ReportTypesEnum.SQL) {

                this.datatable.pageNumber = 0;
                this.GridRequest.pageNumber = 1;
                this.isExecuteResultClicked = true;
                this.GridRequest.reportId = this.reportId;
                this.GridRequest.sqlQuery = this.sqlQuery;
                this.GridRequest.sortColumns = [];

                if (this.parameters != undefined) {
                    this.GridRequest.parameters = this.getParametersValues(this.parameters!);
                } else {
                    this.GridRequest.parameters = undefined;
                }

                this.getReportColums().subscribe(schema => {
                    this.reportDisplayColumns = schema;
                    this.gridManager.refreshData();
                });
            }
        }
    }

    private getReportColums(): Observable<ReportSchema[]> {
        return this._reportService.getColumnNames(this.executionModel);
    }

    public downloadJasperReport(): void {
        this.parametersFormGroup.markAllAsTouched();
        if (this.parametersFormGroup.valid) {
            if (this._report!.reportType === ReportTypesEnum.JasperPDF || this._report!.reportType === ReportTypesEnum.JasperWord) {

                if (this.parameters != undefined) {
                    this.executionModel.parameters = this.getParametersValues(this.parameters!);
                } else {
                    this.executionModel.parameters = undefined;
                }

                this._reportService.downloadReport(this.executionModel).subscribe();
            }
        }
    }

    private zoomPercentage: number = 100;
    public zoomPercentageString = "100%";

    public zoomIn(): void {
        if (this.zoomPercentage < 1000) {
            this.zoomPercentage += 50;
            this.zoomPercentageString = `${this.zoomPercentage}%`;
        }
    }

    public zoomOut(): void {
        if (this.zoomPercentage > 100) {
            this.zoomPercentage -= 50;
            this.zoomPercentageString = `${this.zoomPercentage}%`;
        }
    }


    public exportToExcel(): void {
        this.parametersFormGroup.markAllAsTouched();
        if (this.parametersFormGroup.valid) {
            if (this._report!.reportType === ReportTypesEnum.SQL) {

                if (this.parameters != undefined) {
                    this.executionModel.parameters = this.getParametersValues(this.parameters!);
                } else {
                    this.executionModel.parameters = undefined;
                }

                this._reportService.generateCSV(this.executionModel).subscribe();
            }
        }
    }

    private getParametersValues(parameters: ReportParameterExecuteDTO[]): ExecutionParamDTO[] {
        const parameterValues: ExecutionParamDTO[] = [];

        if (parameters !== null && parameters !== undefined) {

            for (const parameter of parameters) {

                const formControlName: string = parameter.code + 'Control';
                let parameterValue = this.parametersFormGroup.get(formControlName)?.value;

                if (parameter.dataType === ReportParameterTypeEnum.DateTime
                    || parameter.dataType === ReportParameterTypeEnum.Date
                    || parameter.dataType === ReportParameterTypeEnum.Time
                    || parameter.dataType === ReportParameterTypeEnum.Year
                    || parameter.dataType === ReportParameterTypeEnum.Month) {

                    if (parameterValue !== null && parameterValue !== undefined) {
                        parameterValue = DateUtils.ToISODateString(parameterValue);
                    }
                }
                else if (parameterValue instanceof NomenclatureDTO) {
                    parameterValue = parameterValue.value;
                }

                parameterValues.push(new ExecutionParamDTO({
                    name: parameter.code,
                    value: parameterValue,
                    type: parameter.dataType,
                    defaultValue: parameter.defaultValue,
                    isMandatory: parameter.isMandatory,
                    pattern: parameter.pattern
                }));
            }
        }

        return parameterValues;
    }

    private buildParametersFormGroup(parameters: ExecutionParamDTO[]): FormGroup {

        const parametersFormGroup = new FormGroup({});

        for (const parameter of parameters) {
            const controlName: string = parameter.name + 'Control';
            parametersFormGroup.addControl(controlName, new FormControl(parameter.defaultValue));
            const validator: ValidatorFn | null = parametersFormGroup.controls[controlName].validator;

            if (parameter.isMandatory === true) {
                if (validator !== null && validator !== undefined) {

                    parametersFormGroup.controls[controlName].setValidators([
                        validator,
                        Validators.required
                    ]);
                }
                else {
                    parametersFormGroup.controls[controlName].setValidators(Validators.required);
                }
            }

            if (!CommonUtils.isNullOrEmpty(parameter.pattern)) {
                if (validator !== null && validator !== undefined) {
                    parametersFormGroup.controls[controlName].setValidators([
                        validator,
                        Validators.pattern(parameter.pattern!)
                    ]);
                }
                else {
                    parametersFormGroup.controls[controlName].setValidators(Validators.pattern(parameter.pattern!));
                }
            }

            switch (parameter.type) {
                case ReportParameterTypeEnum.Int: {
                    if (validator !== null && validator !== undefined) {
                        parametersFormGroup.controls[controlName].setValidators([
                            validator,
                            TLValidators.number(undefined, undefined, 0)
                        ]);
                    }
                    else {
                        parametersFormGroup.controls[controlName].setValidators(TLValidators.number(undefined, undefined, 0));
                    }
                } break;
                case ReportParameterTypeEnum.Decimal: {
                    if (validator !== null && validator !== undefined) {
                        parametersFormGroup.controls[controlName].setValidators([
                            validator,
                            TLValidators.number()
                        ]);
                    }
                    else {
                        parametersFormGroup.controls[controlName].setValidators(TLValidators.number());
                    }
                } break;
            }
        }

        return parametersFormGroup;
    }
}