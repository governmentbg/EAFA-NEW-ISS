import { Component, Input, OnInit, Self } from '@angular/core';
import { AbstractControl, ControlValueAccessor, FormControl, FormGroup, NgControl, ValidationErrors, Validator } from '@angular/forms';
import { NgxEditorModel } from 'ngx-monaco-editor';
import { ExecutionReportInfoDTO } from '@app/models/generated/dtos/ExecutionReportInfoDTO';
import { IReportService } from '@app/interfaces/administration-app/report.interface';
import { forkJoin } from 'rxjs';
import { ReportAdministrationService } from '@app/services/administration-app/report-administration.service';
import { HttpErrorResponse } from '@angular/common/http';
import { ErrorSnackbarComponent } from '@app/shared/components/error-snackbar/error-snackbar.component';
import { RequestProperties } from '@app/shared/services/request-properties';
import { ErrorCode, ErrorModel } from '@app/models/common/exception.model';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ResultType } from '../../../common-app/reports/report-execution/report-execution.component';
import { ReportSchema } from '../../../common-app/reports/models/report-schema.model';

@Component({
    selector: 'report-sql',
    templateUrl: './report-sql.component.html'
})
export class ReportSqlComponent implements OnInit, ControlValueAccessor, Validator {
    @Input()
    public reportInfo: ExecutionReportInfoDTO | undefined;

    @Input()
    public hasExecuteTable: boolean | undefined = false;

    public formGroup: FormGroup;

    public options = {
        theme: 'vs-light',
        readOnly: false
    };

    public model: NgxEditorModel = {
        value: '',
        language: 'sql'
    };

    public isDisabled: boolean = false;

    public executeResults: ResultType[] = [];
    public reportDisplayColumns: ReportSchema[] = [];
    public isExecuteResultClicked: boolean = false;
    public sqlQuery: string | undefined;

    private readonly reportService: IReportService;
    private readonly translateService: FuseTranslationLoaderService;
    private readonly snackbar: MatSnackBar;

    private ngControl: NgControl;
    private onChanged: (value: string) => void;
    private onTouched: (value: string) => void;

    public constructor(
        @Self() ngControl: NgControl,
        reportService: ReportAdministrationService,
        translateService: FuseTranslationLoaderService,
        snackbar: MatSnackBar
    ) {
        this.ngControl = ngControl;
        this.ngControl.valueAccessor = this;

        this.onChanged = (value: string) => { return; }
        this.onTouched = (value: string) => { return; }

        this.reportService = reportService;
        this.translateService = translateService;
        this.snackbar = snackbar;

        this.formGroup = new FormGroup({
            editorControl: new FormControl()
        });
    }

    public ngOnInit(): void {
        if (this.ngControl.control) {
            this.ngControl.control.validator = this.validate.bind(this);
        }

        this.formGroup.valueChanges.subscribe({
            next: () => {
                const value: string = this.formGroup.get('editorControl')!.value;
                this.sqlQuery = value;
                this.onChanged(value);
            }
        });
    }

    public writeValue(value: string): void {
        this.sqlQuery = undefined;

        if (value !== null && value !== undefined) {
            this.model = {
                value: value,
                language: 'sql'
            };
            this.formGroup.controls.editorControl.setValue(value);
            this.sqlQuery = value;
            this.onChanged(value);
        }
    }

    public registerOnChange(fn: (value: string) => void): void {
        this.onChanged = fn;
    }

    public registerOnTouched(fn: (value: string) => void): void {
        this.onTouched = fn;
    }

    public setDisabledState(isDisabled: boolean): void {
        this.isDisabled = isDisabled;
        if (this.isDisabled) {
            this.options.readOnly = true;
            this.hasExecuteTable = false;
            this.formGroup.disable();
        }
        else {
            this.options.readOnly = false;
            this.formGroup.enable();
        }
    }

    public validate(control: AbstractControl): ValidationErrors | null {
        const errors: ValidationErrors = {};

        Object.keys(this.formGroup.controls).forEach((key: string) => {
            const controlErrors: ValidationErrors | null = this.formGroup.controls[key].errors;
            if (controlErrors !== null) {
                errors[key] = controlErrors;
            }
        });

        return Object.keys(errors).length > 0 ? errors : null;
    }

    public executeResultClicked(): void {
        this.executeResults = [];
        this.reportDisplayColumns = [];

       
            if (this.sqlQuery !== null && this.sqlQuery !== undefined) {
                if (this.reportInfo === null || this.reportInfo === undefined) {
                    this.reportInfo = new ExecutionReportInfoDTO();
                }

                this.reportInfo.sqlQuery = this.sqlQuery;
                forkJoin(
                    this.reportService.getColumnNamesRawSql(this.reportInfo),
                    this.reportService.executeRawSql(this.reportInfo)
                ).subscribe({
                    next: (results: (ReportSchema[] | ResultType[])[]) => {
                        this.reportDisplayColumns = results[0] as ReportSchema[];
                        setTimeout(() => {
                            this.executeResults = results[1] as ResultType[];
                        });
                    },
                    error: (response: HttpErrorResponse) => {
                        if (response !== null && response !== undefined && response.error !== null && response.error !== undefined) {
                            if (response.error.messages !== null && response.error.messages !== undefined) {
                                const messages: string[] = response.error.messages;
                                if (messages.length !== 0) {
                                    this.snackbar.openFromComponent(ErrorSnackbarComponent, {
                                        data: response.error as ErrorModel,
                                        duration: RequestProperties.DEFAULT.showExceptionDurationErr,
                                        panelClass: RequestProperties.DEFAULT.showExceptionColorClassErr
                                    });
                                }
                                else {
                                    this.snackbar.openFromComponent(ErrorSnackbarComponent, {
                                        data: new ErrorModel({ messages: [this.translateService.getValue('service.an-error-occurred-in-the-app')] }),
                                        duration: RequestProperties.DEFAULT.showExceptionDurationErr,
                                        panelClass: RequestProperties.DEFAULT.showExceptionColorClassErr
                                    });
                                }
                            }
                            if ((response.error as ErrorModel).code === ErrorCode.InvalidSqlQuery) {
                                this.formGroup.get('editorControl')!.setErrors({ 'invalidSqlQuery': true });
                            }
                        }
                    }
                });
            
        }

        this.isExecuteResultClicked = true;
    }
}