import { AfterViewInit, Component, DoCheck, Input, OnInit, Self } from '@angular/core';
import { AbstractControl, ControlValueAccessor, FormControl, FormGroup, NgControl, ValidationErrors, Validator, Validators } from '@angular/forms';

import { HolderGroundForUseDTO } from '@app/models/generated/dtos/HolderGroundForUseDTO';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { DateRangeIndefiniteData } from '@app/shared/components/date-range-indefinite/date-range-indefinite.component';
import { DateRangeData } from '@app/shared/components/input-controls/tl-date-range/tl-date-range.component';


@Component({
    selector: 'ground-for-use',
    templateUrl: './ground-for-use.component.html'
})
export class GroudForUseComponent implements OnInit, AfterViewInit, DoCheck, ControlValueAccessor, Validator {
    @Input()
    public groundForUseTypes: NomenclatureDTO<number>[] = [];

    public groundForUseForm: FormGroup;

    private id: number | undefined;
    private isDisabled: boolean = false;
    private onChanged: (value: HolderGroundForUseDTO | undefined) => void;
    private onTouched: (value: HolderGroundForUseDTO | undefined) => void;
    private ngControl: NgControl;

    public constructor(@Self() ngControl: NgControl) {
        this.ngControl = ngControl;

        this.onChanged = (value: HolderGroundForUseDTO | undefined) => { return; };
        this.onTouched = (value: HolderGroundForUseDTO | undefined) => { return; };

        this.ngControl.valueAccessor = this;

        this.groundForUseForm = new FormGroup({
            groundForUseTypeControl: new FormControl(undefined, Validators.required),
            groundForUseNumberControl: new FormControl(undefined, [Validators.required, Validators.maxLength(500)]),
            groundForUseValidityRangeControl: new FormControl(undefined, Validators.required)
        });
    }

    public ngOnInit(): void {
        if (this.ngControl.control) {
            this.ngControl.control.validator = this.validate.bind(this);
        }

        if (this.isDisabled) {
            this.groundForUseForm.disable();
        }
    }

    public ngAfterViewInit(): void {
        this.groundForUseForm.valueChanges.subscribe({
            next: () => {
                this.onChanged(this.getValue());
            }
        });
    }

    public ngDoCheck(): void {
        if (this.ngControl?.control?.touched) {
            this.groundForUseForm.markAllAsTouched();
        }
    }

    public writeValue(value: HolderGroundForUseDTO | undefined): void {
        if (value !== null && value !== undefined) {
            this.id = value.id;
            if (value.typeId !== null && value.typeId !== undefined) {
                const groundForUseType: NomenclatureDTO<number> = this.groundForUseTypes.find(x => x.value === value.typeId)!;
                this.groundForUseForm.get('groundForUseTypeControl')!.setValue(groundForUseType);
            }
            this.groundForUseForm.get('groundForUseNumberControl')!.setValue(value.number);
            this.groundForUseForm.get('groundForUseValidityRangeControl')!.setValue(new DateRangeIndefiniteData({
                range: new DateRangeData({ start: value.groundForUseValidFrom, end: value.groundForUseValidTo }),
                indefinite: value.isGroundForUseUnlimited ?? false
                
            }));
        }
        else {
            this.groundForUseForm.reset();
        }
    }

    public registerOnChange(fn: (value: HolderGroundForUseDTO | undefined) => void): void {
        this.onChanged = fn;
    }

    public registerOnTouched(fn: (value: HolderGroundForUseDTO | undefined) => void): void {
        this.onTouched = fn;
    }

    public setDisabledState(isDisabled: boolean): void {
        this.isDisabled = isDisabled;
        if (this.isDisabled) {
            this.groundForUseForm.disable();
        }
        else {
            this.groundForUseForm.enable();
        }
    }

    public validate(control: AbstractControl): ValidationErrors | null {
        const errors: ValidationErrors = this.groundForUseForm.errors ?? {};

        Object.keys(this.groundForUseForm.controls).forEach((key: string) => {
            const controlErrors: ValidationErrors | null = this.groundForUseForm.controls[key].errors;
            if (controlErrors !== null) {
                errors[key] = controlErrors;
            }
        });

        return Object.keys(errors).length === 0 ? null : errors;
    }

    private getValue(): HolderGroundForUseDTO | undefined {
        const result = new HolderGroundForUseDTO({
            id: this.id,
            typeId: this.groundForUseForm.get('groundForUseTypeControl')!.value?.value,
            number: this.groundForUseForm.get('groundForUseNumberControl')!.value
        });

        const validity: DateRangeIndefiniteData | undefined = this.groundForUseForm.get('groundForUseValidityRangeControl')!.value;
        if (validity !== undefined && validity !== null) {
            result.isGroundForUseUnlimited = validity.indefinite;
            result.groundForUseValidFrom = validity.range?.start;
            result.groundForUseValidTo = validity.range?.end;
        }
        else {
            result.isGroundForUseUnlimited = false;
        }
        
        return result;
    }
}
