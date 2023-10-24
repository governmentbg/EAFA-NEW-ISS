import { Component, Input, OnInit, Self } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, NgControl, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { map } from 'rxjs/operators';
import { Observable } from 'rxjs';

import { CustomFormControl } from '@app/shared/utils/custom-form-control';
import { InspectionGeneralInfoModel } from '../../models/inspection-general-info-model';
import { InspectorTableModel } from '../../models/inspector-table-model';
import { InspectorDTO } from '@app/models/generated/dtos/InspectorDTO';
import { InspectionsService } from '@app/services/administration-app/inspections.service';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { CommonUtils } from '@app/shared/components/search-panel/utils';
import { InspectionTypesEnum } from '@app/enums/inspection-types.enum';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';

@Component({
    selector: 'inspection-general-info',
    templateUrl: './inspection-general-info.component.html'
})
export class InspectionGeneralInfoComponent extends CustomFormControl<InspectionGeneralInfoModel> implements OnInit {

    @Input()
    public institutions: NomenclatureDTO<number>[] = [];

    @Input()
    public hasEmergencySignal: boolean = true;

    @Input()
    public canEditNumber: boolean = false;

    @Input()
    public reportNumAlreadyExists: boolean = false;

    @Input()
    public inspectionType: InspectionTypesEnum | undefined;

    public numPrefix?: string;
    public startDateLabel!: string;
    public endDateLabel!: string;

    private numberWritten: boolean = false;
    private skipDisabledCheck: boolean = false;
    private codes: string[] = [];

    private readonly service: InspectionsService;
    private readonly translate: FuseTranslationLoaderService;

    public constructor(
        @Self() ngControl: NgControl,
        service: InspectionsService,
        translate: FuseTranslationLoaderService
    ) {
        super(ngControl);

        this.service = service;
        this.translate = translate;

        this.onMarkAsTouched.subscribe({
            next: () => {
                this.control.updateValueAndValidity();
            }
        });
    }

    public ngOnInit(): void {
        this.initCustomFormControl();

        this.setDateLabels();

        if (!this.canEditNumber) {
            this.form.get('reportNumberControl')!.setValidators([Validators.required, this.formatUserNumber()]);
        }
        else {
            this.form.get('reportNumberControl')!.setValidators([Validators.required, this.formatReportNumber()]);
        }
    }

    public writeValue(value: InspectionGeneralInfoModel): void {
        if (value !== undefined && value !== null) {
            this.skipDisabledCheck = true;

            if (value.reportNum !== undefined && value.reportNum !== null) {
                this.codes = value.reportNum!.split('-');

                this.onReportNumChanged(this.codes);
            }

            this.skipDisabledCheck = false;
            this.numberWritten = true;

            this.form.get('inspectionStartDateControl')!.setValue(value.startDate);
            this.form.get('inspectionEndDateControl')!.setValue(value.endDate);
            this.form.get('inspectorsControl')!.setValue(value.inspectors);
            this.form.get('emergencySignalControl')!.setValue(value.byEmergencySignal === true);
        }
        else {
            //this.form.get('reportNumberControl')!.setValue(this.translate.getValue('inspections.report-num-on-save'));
            this.form.get('inspectionStartDateControl')!.setValue(new Date());
            this.form.get('inspectionEndDateControl')!.setValue(this.getDateWith1HourInFuture());
            this.form.get('emergencySignalControl')!.setValue(false);

            this.getCurrentInspector().subscribe({
                next: (result) => {
                    this.form.get('inspectorsControl')!.setValue([result]);
                    this.onChanged(this.getValue());
                }
            });
        }
    }

    public onReportNumChanged(codes: string[]): void {
        this.reportNumAlreadyExists = false;

        if (this.numberWritten || (this.isDisabled && !this.skipDisabledCheck)) {
            this.numberWritten = false;
            return;
        }

        if (this.canEditNumber) {
            if ((this.codes.length === 0 && this.codes !== codes) || (this.codes === codes && codes.length > 0)) {
                this.numPrefix = '';
                const reportNum: string = `${this.handleNumber(codes[0])}-${this.handleNumber(codes[1])}-${this.handleUserNumber(codes[2])}`;
                this.form.get('reportNumberControl')!.setValue(reportNum);
            }
        }
        else {
            if (codes.length === 3) {
                if (!CommonUtils.isNullOrEmpty(codes[0]) && !CommonUtils.isNullOrEmpty(codes[1])) {
                    this.numPrefix = `${this.handleNumber(codes[0])}-${this.handleNumber(codes[1])}-`;
                    this.form.get('reportNumberControl')!.setValue(this.handleUserNumber(codes[2]));
                }
            }
        }
    }

    protected buildForm(): AbstractControl {
        const form: FormGroup = new FormGroup({
            reportNumberControl: new FormControl(undefined, [Validators.required, Validators.maxLength(50)]),
            inspectionStartDateControl: new FormControl(undefined, Validators.required),
            inspectionEndDateControl: new FormControl(undefined, Validators.required),
            emergencySignalControl: new FormControl(false),
            inspectorsControl: new FormControl(undefined)
        });

        return form;
    }

    protected getValue(): InspectionGeneralInfoModel {
        const result: InspectionGeneralInfoModel = new InspectionGeneralInfoModel({
            startDate: this.form.get('inspectionStartDateControl')!.value,
            endDate: this.form.get('inspectionEndDateControl')!.value,
            inspectors: this.form.get('inspectorsControl')!.value,
            byEmergencySignal: this.form.get('emergencySignalControl')!.value ?? false
        });

        if (this.canEditNumber) {
            result.reportNum = this.form.get('reportNumberControl')!.value;
        }
        else {
            if (!CommonUtils.isNullOrEmpty(this.numPrefix)) {
                result.reportNum = this.numPrefix + this.form.get('reportNumberControl')!.value;
            }
        }

        return result
    }

    private getDateWith1HourInFuture(): Date {
        const today: Date = new Date();
        today.setHours(today.getHours() + 1);
        return today;
    }

    private setDateLabels(): void {
        switch (this.inspectionType) {
            case InspectionTypesEnum.CWO:
                this.startDateLabel = this.translate.getValue('inspections.check-start-date');
                this.endDateLabel = this.translate.getValue('inspections.check-end-date');
                break;
            case InspectionTypesEnum.OFS:
                this.startDateLabel = this.translate.getValue('inspections.observation-start-date');
                this.endDateLabel = this.translate.getValue('inspections.observation-end-date');
                break;
            default:
                this.startDateLabel = this.translate.getValue('inspections.inspection-start-date');
                this.endDateLabel = this.translate.getValue('inspections.inspection-end-date');
                break;
        }
    }

    private getCurrentInspector(): Observable<InspectorTableModel> {
        return this.service.getCurrentInspector()
            .pipe(map((value: InspectorDTO) => {
                const model = new InspectorTableModel(value);
                model.isCurrentUser = true;
                model.isInCharge = true;
                model.hasIdentifiedHimself = false;

                return model;
            }));
    }

    private handleNumber(num: string): string {
        if (!num) {
            return num;
        }

        return num.length > 3
            ? num.substring(0, 3)
            : num.padEnd(3, '0');
    }

    private handleUserNumber(num: string): string {
        if (!num) {
            return num;
        }

        const parts: string[] = num.split('#');

        return parts.length < 2
            ? (num.length > 3
                ? num.substring(0, 3)
                : num.padStart(3, '0'))
            : num;
    }

    private formatUserNumber(): ValidatorFn {
        return (control: AbstractControl): ValidationErrors | null => {
            const reportNum: string = control.value ?? '';

            let newStr: string = reportNum.replace(/[^0-9#]/g, '');
            const parts: string[] = newStr.split('#');

            if (parts.length < 2 || (parts.length === 2 && parts[0].length !== 3)) {
                const res: string = reportNum.replace(/[^0-9#]/g, '');
                if (res.length > 3 && newStr[3] !== '#') {
                    newStr = res.slice(0, 3)
                }

                newStr = newStr.slice();
            }
            else {
                newStr = reportNum;
            }

            if (reportNum !== newStr) {
                control.setValue(newStr);
            }

            return null;
        }
    }

    private formatReportNumber(): ValidatorFn {
        return (control: AbstractControl): ValidationErrors | null => {
            const reportNum: string = control.value ?? '';

            let newStr: string = reportNum.replace(/[^0-9-#]/g, '');
            const parts: string[] = newStr.split('#');
            let numParts: string[] = [];

            if (parts.length === 2) {
                numParts = parts[0].split('-');
            }
            else {
                numParts = newStr.split('-');
            }

            if (numParts.length < 3 || (numParts.length === 3 && numParts.some(x => x.length !== 3))) {
                const res: string = reportNum.replace(/[^0-9#]/g, '');

                if (res.length < 6 && res.length > 3 && newStr[3] !== '-') {
                    newStr = res.slice(0, 3) + '-' + res.slice(3);
                }
                else if (res.length < 9 && res.length > 6 && newStr[7] !== '-') {
                    newStr = res.slice(0, 3) + '-' + res.slice(3, 6) + '-' + res.slice(6);
                }
                else if (res.length > 9 && newStr[11] !== '#') {
                    newStr = res.slice(0, 3) + '-' + res.slice(3, 6) + '-' + res.slice(6, 9) + '#' + res.slice(9);
                }

                newStr = newStr.slice();
            }

            if (reportNum !== newStr) {
                control.setValue(newStr);
            }

            return null;
        }
    }
}