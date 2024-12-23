import { AfterViewInit, Component, forwardRef, Input, OnInit } from '@angular/core';
import { FormControl, FormGroup, ValidatorFn } from '@angular/forms';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { combineLatest, Observable } from 'rxjs';
import { GetControlErrorLabelTextCallback } from '../../input-controls/base-tl-control';
import { TLError } from '../../input-controls/models/tl-error.model';
import { BaseDataColumn } from '../base-data-column';
import { DataType } from '../enums/data-type.enum';
import { GridRow } from '../models/row.model';

export type CallbackFunction = ((row: any) => any);

const FORM_CONTROL_NAME_SUFFIX: string = 'Control';

@Component({
    selector: 'data-column',
    templateUrl: './data-column.component.html',
    providers: [{ provide: BaseDataColumn, useExisting: forwardRef(() => TLDataColumnComponent) }]
})
export class TLDataColumnComponent extends BaseDataColumn implements OnInit, AfterViewInit {
    constructor() {
        super();
    }

    ngOnInit(): void {
        this.formControlName = this.propertyName + FORM_CONTROL_NAME_SUFFIX;
        this.setupCalculatedColumnHandler();
    }

    ngAfterViewInit(): void {
        this.ngxDataColumn.comparator = (val1: any, val2: any, row1: GridRow<any>, row2: GridRow<any>, dir: 'asc' | 'desc') => {

            let lhs: any;
            let rhs: any;

            if (this.dataType === DataType.NOMENCLATURE) {
                lhs = this._collection?.find(x => x.value == row1.data[this.propertyName])?.displayName;
                rhs = this._collection?.find(x => x.value == row2.data[this.propertyName])?.displayName;
            }
            else {
                lhs = row1.data[this.propertyName];
                rhs = row2.data[this.propertyName];
            }

            if (lhs < rhs) {
                return -1;
            }
            if (lhs > rhs) {
                return 1;
            }
            return 0;
        }
    }

    public _collection: NomenclatureDTO<number>[] | undefined;

    @Input()
    public set options(value: NomenclatureDTO<number>[]) {
        this._collection = value;
        this.setNomenclatureValue();
    }


    @Input() public displayOptions: NomenclatureDTO<number>[] | undefined;

    public _formGroup: FormGroup | undefined;
    @Input() public set formGroup(value: FormGroup) {
        this._formGroup = value;
        this.isEditable = true;
        this.setNomenclatureValue();
    }

    @Input() public getControlErrorLabelText: GetControlErrorLabelTextCallback | undefined;

    @Input() public set isEditable(value: boolean) {
        this._isEditable = value;
    }

    @Input() public isRowNumber: boolean = false;

    public _hiddenFormControlName!: string;
    public _formControlName!: string;
    @Input() public set formControlName(value: string) {
        this._formControlName = value;
        this._hiddenFormControlName = `${this._formControlName}Hidden`;
        this.isEditable = true;
        this.setNomenclatureValue();
    }

    private _calculatedValue: CallbackFunction | undefined;
    @Input() public set calculatedValue(value: CallbackFunction) {
        this._calculatedValue = value;
    }

    public _hasSelectedValueFromDropdownValidator: boolean = true;

    @Input() public set hasSelectedValueFromDropdownValidator(value: boolean) {
        this._hasSelectedValueFromDropdownValidator = value;
    }

    public _codeInTemplateOptions: boolean = false;

    @Input()
    public set codeInTemplateOptions(value: boolean) {
        this._codeInTemplateOptions = value;
    }

    public _templateOptions: boolean = false;

    @Input()
    public set templateOptions(value: boolean) {
        this._templateOptions = value;
    }

    private _calculatedColumnControlNames: string[] = [];

    @Input() public set calculatedColumnControlNames(formControls: string[]) {
        this._calculatedColumnControlNames = formControls;
    }

    @Input() public minValue: unknown;
    @Input() public maxValue: unknown;
    @Input() public disabled: boolean = false;

    @Input() public prefixControlValidators: ValidatorFn[] | undefined;

    public _getControlErrorLabelTextMethod: GetControlErrorLabelTextCallback = this._getControlErrorLabelText.bind(this);

    public _getControlErrorLabelText(controlName: string, error: unknown, errorCode: string): TLError | undefined {

        if (this.getControlErrorLabelText != undefined) {
            return this.getControlErrorLabelText(this._formControlName, error, errorCode);
        }

        return undefined;
    }

    public value(row: GridRow<any>): any {
        const props = this.propertyName.split('.');
        let res: any = (row as any)[props[0]];
        for (let i = 1; i < props.length; ++i) {
            res = res[props[i]];
        }
        return res;
    }

    private setNomenclatureValue(): void {
        if (this._dataType == 'nomenclature' && this._formGroup != null && this._formControlName != null && this._collection != null) {
            const formControl: FormControl = this._formGroup!.get(this._formControlName) as FormControl;
            const hiddenFormControl: FormControl = new FormControl(formControl.value, formControl.validator);
            this._formGroup.addControl(this._hiddenFormControlName, hiddenFormControl);

            formControl.valueChanges.subscribe(value => {
                if (value != undefined) {
                    const selectedValue = this._collection?.find(x => x.value == value);
                    hiddenFormControl?.setValue(selectedValue ? selectedValue : (this._hasSelectedValueFromDropdownValidator ? undefined : value));
                } else {
                    hiddenFormControl?.setValue(undefined);
                }
            });

            hiddenFormControl.valueChanges.subscribe((selectedValue: NomenclatureDTO<any> | string) => {
                let value: any = undefined;
                if (selectedValue != undefined) {
                    value = (selectedValue instanceof NomenclatureDTO) ? selectedValue.value : selectedValue;
                }

                formControl.setValue(value, { onlySelf: true, emitEvent: false });
            });
        }
    }

    private setupCalculatedColumnHandler(): void {
        if (this._dataType === DataType.CALCULATED && this._formGroup !== undefined) {
            // get valueChanges on all controls except current
            const controlsToWatch: string[] = this.allControlsExceptCurrent();
            const valueChanges: Observable<any>[] = controlsToWatch.map((control: string) => {
                return (this._formGroup as FormGroup).controls[control].valueChanges;
            });

            combineLatest(valueChanges).subscribe(() => {
                if (this._calculatedValue !== undefined && this._formGroup !== undefined) {
                    const obj: any = {};
                    Object.keys(this._formGroup.controls).forEach((control: string) => {
                        obj[control.slice(0, -FORM_CONTROL_NAME_SUFFIX.length)] = this._formGroup?.controls[control].value;
                    });
                    const value = this._calculatedValue(obj);
                    this._formGroup.get(this._formControlName)?.setValue(value);
                }
            });
        }
    }

    private allControlsExceptCurrent(): string[] {
        if (this._formGroup !== undefined) {
            const controls: string[] = Object.entries(this._formGroup.controls).map(control => control[0]);
            return controls.filter((control: string) => {
                return control !== this._formControlName && this._calculatedColumnControlNames.includes(control);
            });
        }
        return [];
    }
}