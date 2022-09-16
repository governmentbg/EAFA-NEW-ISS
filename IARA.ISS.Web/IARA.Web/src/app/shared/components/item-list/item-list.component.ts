import { Component, Input, OnChanges, OnInit, Optional, Self, SimpleChange, SimpleChanges } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, NgControl, ValidatorFn, Validators } from '@angular/forms';
import { StatisticalFormNumStatDTO } from '@app/models/generated/dtos/StatisticalFormNumStatDTO';
import { TLValidators } from '@app/shared/utils/tl-validators';
import { ValidityCheckerDirective } from '@app/shared/directives/validity-checker/validity-checker.directive';
import { CustomFormControl } from '@app/shared/utils/custom-form-control';

@Component({
    selector: 'item-list',
    templateUrl: './item-list.component.html',
    styleUrls: ['./item-list.component.scss']
})
export class ItemListComponent extends CustomFormControl<StatisticalFormNumStatDTO[]> implements OnInit, OnChanges {
    @Input()
    public itemsLabel: string | undefined;

    @Input()
    public valuesLabel: string | undefined;

    @Input()
    public required: boolean = false;

    public formItems: StatisticalFormNumStatDTO[] = [];

    public constructor(@Self() ngControl: NgControl, @Optional() @Self() validityChecker: ValidityCheckerDirective) {
        super(ngControl, false, validityChecker);

        this.onMarkAsTouched.subscribe({
            next: () => {
                this.form.updateValueAndValidity();
            }
        });

        this.form.valueChanges.subscribe({
            next: (value: any) => {
                if (value !== null && value !== undefined) {
                    for (const item of this.formItems) {
                        const formControlName = item.code + 'Control';
                        if (this.form.controls[formControlName] !== undefined && value[formControlName] !== null) {
                            item.statValue = value[formControlName] !== '' ? value[formControlName] : undefined;
                        }
                    }
                }
                else {
                    for (const item of this.formItems) {
                        item.statValue = undefined;
                    }
                }
                
                this.onChanged(this.formItems);
            }
        });
    }

    public ngOnInit(): void {
        this.initCustomFormControl();
    }

    public ngOnChanges(changes: SimpleChanges): void {
        const required: SimpleChange = changes['required'];
        if (required !== undefined && required !== null) {
            this.setRequiredValidator();
        }
    }

    public writeValue(value: StatisticalFormNumStatDTO[]): void {
        if (value !== null && value !== undefined) {
            this.formItems = value;
            this.addControls(this.formItems);
            this.setRequiredValidator();
            this.setDisabledState(this.isDisabled);
            this.onChanged(value);
        }
    }

    protected buildForm(): AbstractControl {
        return new FormGroup({});
    }

    protected getValue(): StatisticalFormNumStatDTO[] {
        return this.formItems;
    }

    private addControls(items: StatisticalFormNumStatDTO[]): void {
        for (const item of items) {
            const controlName: string = item.code! + 'Control';
            this.form.addControl(controlName, new FormControl(item.statValue, TLValidators.number(0)));
        }
    }

    private setRequiredValidator(): void {
        const validators: ValidatorFn[] = [TLValidators.number(0)];
        if (this.required) {
            validators.push(Validators.required);
        }

        for (const control of Object.keys(this.form.controls)) {
            this.form.get(control)!.setValidators(validators);
            this.form.get(control)!.markAsPending();
        }

        this.setDisabledState(this.isDisabled);
    }
}