import { Component, EventEmitter, Input, OnInit, Optional, Output, Self } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, NgControl, ValidationErrors, Validators } from '@angular/forms';
import { AuanWitnessDTO } from '@app/models/generated/dtos/AuanWitnessDTO';
import { CustomFormControl } from '@app/shared/utils/custom-form-control';
import { AddressTypesEnum } from '@app/enums/address-types.enum';
import { ValidityCheckerDirective } from '@app/shared/directives/validity-checker/validity-checker.directive';

@Component({
    selector: 'auan-witness',
    templateUrl: './auan-witness.component.html'
})
export class AuanWitnessComponent extends CustomFormControl<AuanWitnessDTO> implements OnInit {
    @Input()
    public orderNum!: number;

    @Output()
    public deletePanelBtnClicked: EventEmitter<void> = new EventEmitter<void>();

    public isDisabled: boolean = false;
    public readonly today: Date = new Date();

    public readonly addressType: AddressTypesEnum = AddressTypesEnum.PERMANENT;

    private model: AuanWitnessDTO | undefined;

    public constructor(
        @Self() ngControl: NgControl,
        @Self() @Optional() validityChecker: ValidityCheckerDirective
    ) {
        super(ngControl, false, validityChecker);

        this.buildForm();
    }

    public ngOnInit(): void {
        this.initCustomFormControl();

        this.form.valueChanges.subscribe({
            next: () => {
                const value: AuanWitnessDTO = this.getValue();

                this.onChanged(value);
            }
        })
    }

    public ngDoCheck(): void {
        if (this.ngControl?.control?.touched) {
            this.form.markAllAsTouched();
        }
    }

    public writeValue(value: AuanWitnessDTO): void {
        this.model = value;
        if (value !== null && value !== undefined) {
            this.fillForm();
            this.onChanged(this.getValue());
        }
        else {
            this.form.reset();
        }
    }

    public registerOnChange(fn: (value: AuanWitnessDTO) => void): void {
        this.onChanged = fn;
    }

    public registerOnTouched(fn: (value: AuanWitnessDTO) => void): void {
        this.onTouched = fn;
    }

    public setDisabledState(isDisabled: boolean): void {
        this.isDisabled = isDisabled;
        if (this.isDisabled) {
            this.form.disable();
        }
        else {
            this.form.enable();
        }
    }

    public validate(control: AbstractControl): ValidationErrors | null {
        const errors: ValidationErrors = {};
        Object.keys(this.form.controls).forEach((key: string) => {
            const controlErrors: ValidationErrors | null = this.form.controls[key].errors;
            if (controlErrors !== null) {
                errors[key] = controlErrors;
            }
        });

        return Object.keys(errors).length > 0 ? errors : null;
    }

    public deletePanel(): void {
        this.deletePanelBtnClicked.emit();
    }

    private fillForm(): void {
        if (this.model !== undefined) {
            this.form.get('personNameControl')!.setValue(this.model.witnessNames);
            this.form.get('personDateOfBirthControl')!.setValue(this.model.dateOfBirth);
            this.form.get('addressesControl')!.setValue(this.model.address);
        }
    }

    protected buildForm(): AbstractControl {
        return new FormGroup({
            personNameControl: new FormControl(null, [Validators.required, Validators.maxLength(500)]),
            personDateOfBirthControl: new FormControl(null, Validators.required),
            addressesControl: new FormControl(null)
        });
    }

    protected getValue(): AuanWitnessDTO {
        if (this.model === undefined) {
            this.model = new AuanWitnessDTO({ isActive: true });
        }

        this.model.witnessNames = this.form.get('personNameControl')!.value;
        this.model.dateOfBirth = this.form.get('personDateOfBirthControl')!.value;
        this.model.address = this.form.get('addressesControl')!.value;
        this.model.orderNum = this.orderNum;

        return this.model;
    }
}
