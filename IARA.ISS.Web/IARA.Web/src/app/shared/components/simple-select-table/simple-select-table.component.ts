import { AfterViewInit, Component, Input, OnChanges, OnInit, Self, SimpleChange, SimpleChanges } from '@angular/core';
import { AbstractControl, ControlValueAccessor, FormControl, FormGroup, NgControl, ValidationErrors, Validator } from '@angular/forms';

import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';

type ElementType = NomenclatureDTO<number> | string | undefined | null;

@Component({
    selector: 'simple-select-table',
    templateUrl: './simple-select-table.component.html'
})
export class SimpleSelectTableComponent implements OnInit, OnChanges, ControlValueAccessor, Validator, AfterViewInit {
    @Input()
    public autocompleteLabel: string = '';

    @Input()
    public columnName: string = '';

    @Input()
    public allRows: NomenclatureDTO<number>[] = [];

    public selectedRows: NomenclatureDTO<number>[] = [];

    public formGroup: FormGroup;

    public isDisabled: boolean = false;

    private ngControl: NgControl;
    private onChanged: (value: NomenclatureDTO<number>[]) => void;
    private onTouched: (value: NomenclatureDTO<number>[]) => void;

    public rows: NomenclatureDTO<number>[] = [];

    public constructor(
        @Self() ngControl: NgControl
    ) {
        this.ngControl = ngControl;
        this.ngControl.valueAccessor = this;

        this.onChanged = (value: NomenclatureDTO<number>[]) => { return; }
        this.onTouched = (value: NomenclatureDTO<number>[]) => { return; }

        this.formGroup = new FormGroup({
            inputControl: new FormControl()
        });
    }

    public ngOnInit(): void {
        if (this.ngControl.control) {
            this.ngControl.control.validator = this.validate.bind(this);
        }
    }

    public ngAfterViewInit(): void {
        this.formGroup.get('inputControl')!.valueChanges.subscribe({
            next: (element: ElementType) => {
                this.updateSelectedArray(element);

                this.rows = this.rows.filter(element => !this.selectedRows.includes(element));
                this.rows = this.rows.slice();
                this.onChanged(this.selectedRows);
            }
        });
    }

    public ngOnChanges(changes: SimpleChanges): void {
        const rows: SimpleChange = changes['allRows'];
        if (rows !== undefined && rows !== null) {
            this.refreshData();
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

    public writeValue(values: NomenclatureDTO<number>[]): void {
        if (values !== null && values !== undefined) {
            this.selectedRows = [...values];
            this.refreshData();
        }
        else {
            this.selectedRows = [];
            this.refreshData();
        }

        this.onChanged(this.selectedRows);
    }

    public registerOnChange(fn: (value: NomenclatureDTO<number>[]) => void): void {
        this.onChanged = fn;
    }

    public registerOnTouched(fn: (value: NomenclatureDTO<number>[]) => void): void {
        this.onTouched = fn;
    }

    public setDisabledState(isDisabled: boolean): void {
        this.isDisabled = isDisabled;
        if (this.isDisabled) {
            this.formGroup.disable();
        }
        else {
            this.formGroup.enable();
        }
    }

    public removeRow(row: NomenclatureDTO<number>): void {
        this.selectedRows = this.selectedRows.filter(selectedRow => selectedRow.value !== row.value);
        this.refreshData();

        this.onChanged(this.selectedRows);
    }

    private refreshData(): void {
        this.rows = this.allRows.filter(row => !this.selectedRows.some(selectedRow => selectedRow.value === row.value));
    }

    private updateSelectedArray(element: ElementType): void {
        if (element !== null && element !== undefined && typeof element !== 'string') {
            this.selectedRows = [...this.selectedRows, element];
            this.formGroup.get('inputControl')!.setValue('');
        }
    }
}
