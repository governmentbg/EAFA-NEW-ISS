import { Component, Input, OnInit, Self } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, NgControl, Validators } from '@angular/forms';
import { map } from 'rxjs/operators';
import { Observable } from 'rxjs';

import { CustomFormControl } from '@app/shared/utils/custom-form-control';
import { InspectionGeneralInfoModel } from '../../models/inspection-general-info-model';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { InspectorTableModel } from '../../models/inspector-table-model';
import { InspectorDTO } from '@app/models/generated/dtos/InspectorDTO';
import { InspectionsService } from '@app/services/administration-app/inspections.service';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';

@Component({
    selector: 'inspection-general-info',
    templateUrl: './inspection-general-info.component.html'
})
export class InspectionGeneralInfoComponent extends CustomFormControl<InspectionGeneralInfoModel> implements OnInit {

    @Input()
    public institutions: NomenclatureDTO<number>[] = [];

    @Input()
    public hasEmergencySignal: boolean = true;

    private readonly service: InspectionsService;
    private readonly translate: FuseTranslationLoaderService;

    public constructor(@Self() ngControl: NgControl, translate: FuseTranslationLoaderService, service: InspectionsService) {
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
    }

    public writeValue(value: InspectionGeneralInfoModel): void {
        if (value !== undefined && value !== null) {
            this.form.get('reportNumberControl')!.setValue(value.reportNum);
            this.form.get('inspectionStartDateControl')!.setValue(value.startDate);
            this.form.get('inspectionEndDateControl')!.setValue(value.endDate);
            this.form.get('inspectorsControl')!.setValue(value.inspectors);
            this.form.get('emergencySignalControl')!.setValue(value.byEmergencySignal === true);
        }
        else {
            this.form.get('reportNumberControl')!.setValue(this.translate.getValue('inspections.report-num-on-save'));
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

    protected buildForm(): AbstractControl {
        return new FormGroup({
            reportNumberControl: new FormControl({ value: undefined, disabled: true }),
            inspectionStartDateControl: new FormControl(undefined, Validators.required),
            inspectionEndDateControl: new FormControl(undefined, Validators.required),
            emergencySignalControl: new FormControl(false),
            inspectorsControl: new FormControl(undefined),
        });
    }

    protected getValue(): InspectionGeneralInfoModel {
        return new InspectionGeneralInfoModel({
            //reportNum: this.form.get('reportNumberControl')!.value, No reason to pass the value
            startDate: this.form.get('inspectionStartDateControl')!.value,
            endDate: this.form.get('inspectionEndDateControl')!.value,
            inspectors: this.form.get('inspectorsControl')!.value,
            byEmergencySignal: this.form.get('emergencySignalControl')!.value ?? false,
        });
    }

    private getDateWith1HourInFuture(): Date {
        const today: Date = new Date();
        today.setHours(today.getHours() + 1);
        return today;
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
}