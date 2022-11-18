import { AfterViewInit, Component, EventEmitter, Input, OnChanges, OnInit, Output, Self, SimpleChanges } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, NgControl, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { MatFormFieldAppearance } from '@angular/material/form-field';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { IdentifierTypeEnum } from '@app/enums/identifier-type.enum';
import { EgnLncDTO } from '@app/models/generated/dtos/EgnLncDTO';
import { TLValidators } from '@app/shared/utils/tl-validators';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { GetControlErrorLabelTextCallback } from '../input-controls/base-tl-control';
import { CustomFormControl } from '@app/shared/utils/custom-form-control';

type ValueType = 'egn' | 'lnc';

@Component({
    selector: 'egn-lnc-input',
    templateUrl: './egn-lnc-input.component.html'
})
export class EgnLncInputComponent extends CustomFormControl<EgnLncDTO | undefined> implements OnInit, AfterViewInit, OnChanges {
    @Input()
    public isForeigner: boolean = false;

    @Input()
    public multilineError: boolean = false;

    @Input()
    public includeForeigner: boolean = false;

    @Input()
    public readonly: boolean = false;

    @Input()
    public appearance: MatFormFieldAppearance = 'legacy';

    @Input()
    public getControlErrorLabelText?: GetControlErrorLabelTextCallback;

    @Input()
    public expectedResult: string | undefined;

    @Input()
    public showSearchButton: boolean = false;

    @Input()
    public emitEgnPnfErrors: boolean = false;

    @Input()
    public isIdentityRequired: boolean = true;

    @Output()
    public searchButtonClicked: EventEmitter<EgnLncDTO> = new EventEmitter<EgnLncDTO>();

    @Output()
    public focusout: EventEmitter<void> = new EventEmitter<void>();

    public expectedValue!: string;

    public label!: string;
    public valueTypes!: NomenclatureDTO<IdentifierTypeEnum>[];

    public readonly toggleButtonId: string;
    public readonly selectButtonId: string;

    public static counter: number = 0;

    private translate: FuseTranslationLoaderService;

    private get egnValidators(): ValidatorFn[] {
        const validators = [
            TLValidators.expectedValueMatch(this.expectedValue),
            TLValidators.exactLength(10),
            TLValidators.number(0, undefined, 0),
            TLValidators.egn
        ];

        if (this.isIdentityRequired) {
            validators.push(Validators.required);
        }

        return validators;
    }
    private get lncValidators(): ValidatorFn[] {
        const validators = [
            TLValidators.expectedValueMatch(this.expectedValue),
            Validators.required,
            TLValidators.exactLength(10),
            TLValidators.number(0, undefined, 0),
            TLValidators.pnf
        ];

        if (this.isIdentityRequired) {
            validators.push(Validators.required);
        }

        return validators;
    }
    private get forIdValidators(): ValidatorFn[] {
        const validators = [
            TLValidators.expectedValueMatch(this.expectedValue),
            Validators.required,
            Validators.maxLength(15)
        ];

        if (this.isIdentityRequired) {
            validators.push(Validators.required);
        }

        return validators;
    }

    public constructor(@Self() ngControl: NgControl, translate: FuseTranslationLoaderService) {
        super(ngControl);
        this.translate = translate;

        this.valueTypes = [
            new NomenclatureDTO<IdentifierTypeEnum>({
                value: IdentifierTypeEnum.EGN,
                displayName: this.translate.getValue('regix-data.egn'),
                isActive: true
            }),
            new NomenclatureDTO<IdentifierTypeEnum>({
                value: IdentifierTypeEnum.LNC,
                displayName: this.translate.getValue('regix-data.lnc'),
                isActive: true
            }),
            new NomenclatureDTO<IdentifierTypeEnum>({
                value: IdentifierTypeEnum.FORID,
                displayName: this.translate.getValue('regix-data.for'),
                isActive: true
            })
        ];

        this.toggleButtonId = `egn-lnc-value-type-button-toggle-${EgnLncInputComponent.counter}`;
        this.selectButtonId = `egn-lnc-value-type-select-${EgnLncInputComponent.counter}`;

        ++EgnLncInputComponent.counter;
    }

    public ngOnInit(): void {
        this.initCustomFormControl();

        this.setForeignerFieldAndValidators();
    }

    public ngAfterViewInit(): void {
        if (!this.isForeigner || this.includeForeigner) {
            this.form.get('valueTypeControl')!.valueChanges.subscribe({
                next: (type: ValueType | NomenclatureDTO<IdentifierTypeEnum>) => {
                    this.onValueTypeChanged(type);
                }
            });
        }
    }

    public ngOnChanges(changes: SimpleChanges): void {
        const expectedResult: string | undefined = changes['expectedResult']?.currentValue;

        if (expectedResult !== null && expectedResult !== undefined) {
            this.expectedValue = expectedResult;

            if (this.includeForeigner) {
                switch (this.form.get('valueTypeControl')!.value?.value) {
                    case IdentifierTypeEnum.EGN:
                        this.form.get('valueControl')!.setValidators(this.egnValidators);
                        break;
                    case IdentifierTypeEnum.LNC:
                        this.form.get('valueControl')!.setValidators(this.lncValidators);
                        break;
                    case IdentifierTypeEnum.FORID:
                        this.form.get('valueControl')!.setValidators(this.forIdValidators);
                        break;
                }
            }
            else {
                if (this.form.get('valueTypeControl')!.value === 'egn') {
                    this.form.get('valueControl')!.setValidators(this.egnValidators);
                }
                else if (this.form.get('valueTypeControl')!.value === 'lnc') {
                    this.form.get('valueControl')!.setValidators(this.lncValidators);
                }
            }
        }

        const isForeigner: boolean | undefined = changes['isForeigner']?.currentValue;
        const includeForeigner: boolean | undefined = changes['includeForeigner']?.currentValue;
        if ((isForeigner !== undefined && isForeigner !== null) || (includeForeigner !== undefined && includeForeigner !== null)) {
            this.setForeignerFieldAndValidators();
        }

        if (isForeigner === true && includeForeigner === true) {
            throw new Error('regix-data: cannot have isForeigner and includeForeigner both true!');
        }

        const readonly: boolean | undefined = changes['readonly']?.currentValue;
        this.disableValueTypeIfReadOnly(readonly ?? this.readonly);

        if (this.isDisabled) {
            this.form.disable({ emitEvent: false });
        }
    }

    public writeValue(value: EgnLncDTO | undefined | null): void {
        setTimeout(() => {
            if (value) {
                this.form.get('valueControl')!.setValue(value.egnLnc);

                if (this.includeForeigner) {
                    this.form.get('valueTypeControl')!.setValue(this.valueTypes.find(x => x.value === value.identifierType));
                }
                else if (!this.isForeigner) {
                    if (value.identifierType !== IdentifierTypeEnum.FORID) {
                        this.form.get('valueTypeControl')!.setValue(value.identifierType === IdentifierTypeEnum.EGN ? 'egn' : 'lnc');
                    }
                }
            }
            else {
                this.form.get('valueControl')!.setValue(null);

                if (this.includeForeigner) {
                    this.form.get('valueTypeControl')!.setValue(this.valueTypes.find(x => x.value === IdentifierTypeEnum.EGN));
                }
                else if (!this.isForeigner) {
                    this.form.get('valueTypeControl')!.setValue('egn');
                }
            }
        });
    }

    public validate(control: AbstractControl): ValidationErrors | null {
        const errors: ValidationErrors = {};
        for (const key of Object.keys(this.form.controls)) {
            if (key === 'valueControl') {
                for (const error in this.form.controls[key].errors) {
                    if (error === 'egn' || error === 'pnf' || error === 'expectedValueNotMatching') {
                        if (this.emitEgnPnfErrors) {
                            errors[error] = this.form.controls[key].errors![error];
                        }
                    }
                    else {
                        errors[error] = this.form.controls[key].errors![error];
                    }
                }
            }
            else {
                for (const key of Object.keys(this.form.controls)) {
                    if (this.form.controls[key].errors !== null && this.form.controls[key].errors !== undefined) {
                        for (const error in this.form.controls[key].errors) {
                            if (!['expectedValueNotMatching'].includes(error)) {
                                errors[error] = this.form.controls[key].errors![error];
                            }
                        }
                    }
                }
            }
        }
        return Object.keys(errors).length === 0 ? null : errors;
    }

    public onFocusOut(): void {
        if (this.form.get('valueControl')!.valid) {
            this.focusout.emit();
        }
    }

    public searchBtnClicked(): void {
        if (this.isIdNumberValidExceptEgn()) {
            this.searchButtonClicked.emit(this.getValue());
        }
    }

    protected buildForm(): AbstractControl {
        return new FormGroup({
            valueControl: new FormControl(),
            valueTypeControl: new FormControl()
        });
    }

    protected getValue(): EgnLncDTO | undefined {
        const egnLnc = this.form.get('valueControl')!.value;

        if (!egnLnc) {
            return undefined;
        }

        const result: EgnLncDTO = new EgnLncDTO({
            egnLnc: egnLnc
        });

        if (this.includeForeigner) {
            result.identifierType = this.form.get('valueTypeControl')!.value!.value;
        }
        else if (this.isForeigner) {
            result.identifierType = IdentifierTypeEnum.FORID;
        }
        else {
            result.identifierType = this.form.get('valueTypeControl')!.value === 'egn' ? IdentifierTypeEnum.EGN : IdentifierTypeEnum.LNC;
        }

        return result;
    }

    private isIdNumberValidExceptEgn(): boolean {
        const errors: ValidationErrors | null = this.form.get('valueControl')!.errors;
        if (errors === null) {
            return true;
        }

        if (errors['egn'] && Object.keys(errors).length === 1) {
            return true;
        }
        return false;
    }

    private setForeignerFieldAndValidators(): void {
        if (this.isForeigner) {
            this.form.get('valueControl')!.setValidators(this.forIdValidators);
            this.label = this.translate.getValue('regix-data.foreigner-id-number');
        }
        else {
            this.form.get('valueControl')!.setValidators(this.egnValidators);

            if (this.includeForeigner) {
                this.setDropdownStyles();
                this.form.get('valueTypeControl')!.setValue(this.valueTypes.find(x => x.value === IdentifierTypeEnum.EGN));
            }
            else {
                this.setToggleButtonsStyles();
                this.form.get('valueTypeControl')!.setValue('egn');
            }
            this.label = this.translate.getValue('regix-data.egn');
        }

        if (this.isDisabled) {
            this.form.disable({ emitEvent: false });
        }
    }

    private onValueTypeChanged(type: ValueType | NomenclatureDTO<IdentifierTypeEnum>): void {
        if (this.includeForeigner && typeof type !== 'string') {
            switch (type.value) {
                case IdentifierTypeEnum.EGN:
                    this.label = this.translate.getValue('regix-data.egn');
                    this.form.get('valueControl')!.setValidators(this.egnValidators);
                    break;
                case IdentifierTypeEnum.LNC:
                    this.label = this.translate.getValue('regix-data.lnc');
                    this.form.get('valueControl')!.setValidators(this.lncValidators);
                    break;
                case IdentifierTypeEnum.FORID:
                    this.label = this.translate.getValue('regix-data.foreigner-id-number');
                    this.form.get('valueControl')!.setValidators(this.forIdValidators);
                    break;
            }
            this.form.get('valueControl')!.updateValueAndValidity({ emitEvent: false });
            this.onChanged(this.getValue());
        }
        else if (!this.isForeigner) {
            if (type === 'egn') {
                this.label = this.translate.getValue('regix-data.egn');
                this.form.get('valueControl')!.setValidators(this.egnValidators);
            }
            else {
                this.label = this.translate.getValue('regix-data.lnc');
                this.form.get('valueControl')!.setValidators(this.lncValidators);
            }
            this.form.get('valueControl')!.updateValueAndValidity({ emitEvent: false });
            this.onChanged(this.getValue());
        }
    }

    private setDropdownStyles(): void {
        const interval: number = setInterval(() => {
            const dropdown: HTMLElement | null = document.getElementById(this.selectButtonId);

            if (dropdown !== null) {
                dropdown.style.transform = 'translateY(3px)';

                const formField: HTMLElement = dropdown.getElementsByTagName('mat-form-field').item(0) as HTMLElement;
                formField.style.height = '2em';

                if (this.appearance === 'outline') {
                    formField.style.bottom = '0.3em';
                }

                clearInterval(interval);
            }
        });
    }

    private setToggleButtonsStyles(): void {
        const interval: number = setInterval(() => {
            let toggle: HTMLElement | null = null;
            while (toggle === null) {
                toggle = document.getElementById(this.toggleButtonId);
            }

            toggle.style.height = '2em';
            toggle.style.alignItems = 'center';

            if (this.appearance === 'outline') {
                toggle.style.verticalAlign = 'super';
            }

            const labels: HTMLCollectionOf<Element> = toggle.getElementsByClassName('mat-button-toggle-label-content');
            for (let i = 0; i < labels.length; ++i) {
                const label: HTMLElement = labels.item(i) as HTMLElement;
                label.style.padding = '0 6px';
            }

            clearInterval(interval);
        });
    }

    private disableValueTypeIfReadOnly(readonly: boolean): void {
        if (readonly === true) {
            this.form.get('valueTypeControl')?.disable({ emitEvent: false });
        }
        else {
            this.form.get('valueTypeControl')?.enable({ emitEvent: false });
        }
    }
}