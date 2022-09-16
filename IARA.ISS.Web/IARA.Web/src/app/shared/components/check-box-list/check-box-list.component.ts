import { Component, Input, OnChanges, OnInit, Self, SimpleChanges } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, NgControl } from '@angular/forms';

import { CustomFormControl } from '@app/shared/utils/custom-form-control';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';

@Component({
    selector: 'check-box-list',
    templateUrl: './check-box-list.component.html'
})
export class CheckboxListComponent extends CustomFormControl<NomenclatureDTO<number>[] | undefined> implements OnInit, OnChanges {

    @Input()
    public checks: NomenclatureDTO<number>[] = [];

    @Input()
    public contentLayout: 'row' | 'column' = 'row';

    @Input()
    public label!: string;

    private values: NomenclatureDTO<number>[] = [];

    public constructor(@Self() ngControl: NgControl) {
        super(ngControl, false);

        this.formArray.valueChanges.subscribe({
            next: () => {
                this.onChanged(this.getValue());
            }
        });
    }

    public ngOnChanges(changes: SimpleChanges): void {
        const checks = changes['checks'];

        if (checks !== null && checks !== undefined) {
            const keys = Object.keys(this.form.controls);

            for (let i = 0; i < keys.length; i++) {
                this.form.removeControl(keys[i]);
            }

            const currentChecks = checks.currentValue as NomenclatureDTO<number>[];
            const length = currentChecks.length;

            for (let i = 0; i < length; i++) {
                const found = this.values.find(f => f.value === currentChecks[i].value);

                this.form.addControl(i.toString(), new FormControl({
                    value: found !== null && found !== undefined,
                    disabled: this.isDisabled
                }));
            }
        }
    }

    public ngOnInit(): void {
        this.initCustomFormControl();
    }

    public writeValue(value: NomenclatureDTO<number>[] | undefined): void {
        if (value !== null && value !== undefined) {
            this.values = value;
            const foundIndexex: number[] = [];

            for (const check of value) {
                const index = this.checks.findIndex(f => f.value === check.value);

                if (index >= 0 && this.form.controls[index.toString()] !== undefined) {
                    this.form.controls[index.toString()].setValue(true);
                    foundIndexex.push(index);
                }
            }

            for (let i = 0; i < this.checks.length; i++) {
                if (!foundIndexex.includes(i) && this.form.controls[i.toString()] !== undefined) {
                    this.form.controls[i.toString()].setValue(false);
                }
            }
        }
        else {
            this.values = [];
            const keys = Object.keys(this.form.controls);

            for (let i = 0; i < keys.length; i++) {
                this.form.controls[keys[i]].setValue(false);
            }
        }
    }

    protected buildForm(): AbstractControl {
        return new FormGroup({});
    }

    protected getValue(): NomenclatureDTO<number>[] | undefined {
        const values: boolean[] = [];

        const keys = Object.keys(this.form.controls);

        for (let i = 0; i < keys.length; i++) {
            values[i] = this.form.controls[keys[i]].value;
        }

        const result = values.map((checked: boolean, index: number) => {
            if (checked) {
                return this.checks[index];
            }

            return undefined;
        }).filter(f => f !== null && f !== undefined) as NomenclatureDTO<number>[];

        return result;
    }
}