import { Component, EventEmitter, OnInit, Output, Self, ViewChild } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, NgControl, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';

import { CustomFormControl } from '@app/shared/utils/custom-form-control';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { InspectedPermitTableModel } from '../../models/inspected-permit-table.model';
import { InspectionUtils } from '@app/shared/utils/inspection.utils';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { InspectionToggleTypesEnum } from '@app/enums/inspection-toggle-types.enum';
import { InspectionCheckModel } from '../../models/inspection-check.model';
import { RecordChangedEventArgs } from '@app/shared/components/data-table/models/record-changed-event.model';
import { TLDataTableComponent } from '@app/shared/components/data-table/tl-data-table.component';
import { InspectionCheckDTO } from '@app/models/generated/dtos/InspectionCheckDTO';
import { InspectionPermitDTO } from '@app/models/generated/dtos/InspectionPermitDTO';

@Component({
    selector: 'inspected-permits-table',
    templateUrl: './inspected-permits-table.component.html'
})
export class InspectedPermitsTableComponent extends CustomFormControl<InspectionPermitDTO[]> implements OnInit {
    public permitsFormGroup!: FormGroup;

    @Output()
    public permitOptionPicked: EventEmitter<number[]> = new EventEmitter<number[]>();

    @ViewChild(TLDataTableComponent)
    private datatable!: TLDataTableComponent;

    public permits: InspectedPermitTableModel[] = [];

    public readonly options: NomenclatureDTO<InspectionToggleTypesEnum>[];
    public readonly fakeToggle: InspectionCheckModel;

    private readonly translate: FuseTranslationLoaderService;

    public constructor(
        @Self() ngControl: NgControl,
        translate: FuseTranslationLoaderService,
    ) {
        super(ngControl);

        this.translate = translate;

        this.options = InspectionUtils.getToggleCheckOptions(translate);

        this.fakeToggle = new InspectionCheckModel({
            value: 0,
            isMandatory: true
        });

        this.onMarkAsTouched.subscribe({
            next: () => {
                this.control.updateValueAndValidity();
            }
        });
    }

    public ngOnInit(): void {
        this.initCustomFormControl();
    }

    public writeValue(value: InspectionPermitDTO[]): void {
        if (value !== undefined && value !== null) {
            const permits = value.map(f => new InspectedPermitTableModel({
                description: f.description,
                id: f.id,
                isRegistered: f.permitLicenseId !== null && f.permitLicenseId !== undefined,
                licenseNumber: f.licenseNumber,
                permitNumber: f.permitNumber,
                from: f.from,
                to: f.to,
                typeName: f.typeName,
                checkValue: f.checkValue,
                permitLicenseId: f.permitLicenseId,
                typeId: f.typeId,
                checkDTO: new InspectionCheckDTO({
                    id: 0,
                    checkValue: f.checkValue
                }),
            }));

            setTimeout(() => {
                this.permits = permits;
            });
        }
        else {
            setTimeout(() => {
                this.permits = [];
            });
        }
    }

    public findOption(check?: InspectionCheckDTO): string | undefined {
        return this.options.find(f => f.value === check?.checkValue)?.displayName
            ?? this.translate.getValue('inspections.toggle-unchosen');
    }

    public onAddRecord(): void {
        this.permitsFormGroup.get('licenseNumberControl')!.enable();
        this.permitsFormGroup.get('optionsControl')!.disable();
    }

    public onEditRecord(record: InspectedPermitTableModel): void {
        this.permitsFormGroup.get('licenseNumberControl')!.disable();
        this.permitsFormGroup.get('optionsControl')!.enable();
        this.permitsFormGroup.get('optionsControl')!.setValue(record.checkValue);
    }

    public permitRecordChanged(event: RecordChangedEventArgs<InspectedPermitTableModel>): void {
        event.Record.checkDTO = this.permitsFormGroup.get('optionsControl')!.value;
        event.Record.checkValue = event.Record.checkDTO?.checkValue;
        this.permits = this.datatable.rows;
        this.control.updateValueAndValidity();
        this.onChanged(this.getValue());

        this.permitOptionPicked.emit(
            this.permits
                .filter(f => f.checkValue === InspectionToggleTypesEnum.Y || f.checkValue === InspectionToggleTypesEnum.N)
                .map(f => f.permitLicenseId!)
        );
    }

    protected buildForm(): AbstractControl {
        this.permitsFormGroup = new FormGroup({
            licenseNumberControl: new FormControl({ value: undefined, disabled: true }, [Validators.required]),
            typeNameControl: new FormControl({ value: undefined, disabled: true }),
            validFromControl: new FormControl({ value: undefined, disabled: true }),
            validToControl: new FormControl({ value: undefined, disabled: true }),
            descriptionControl: new FormControl(undefined),
            optionsControl: new FormControl(undefined, [Validators.required]),
        });

        return new FormControl(undefined, this.permitsValidator());
    }

    protected getValue(): InspectionPermitDTO[] {
        return this.permits.map(f => new InspectionPermitDTO({
            id: f.id,
            checkValue: f.checkDTO?.checkValue,
            description: f.description,
            from: f.from,
            to: f.to,
            licenseNumber: f.licenseNumber,
            permitNumber: f.permitNumber,
            typeName: f.typeName,
            typeId: f.typeId,
            permitLicenseId: f.permitLicenseId,
        }));
    }

    private permitsValidator(): ValidatorFn {
        return (): ValidationErrors | null => {
            if (this.permits !== undefined && this.permits !== null) {
                for (const permit of this.permits) {
                    if (permit.checkValue === null || permit.checkValue === undefined) {
                        return { 'permitsMustBeChecked': true };
                    }
                }
            }
            return null;
        };
    }
}