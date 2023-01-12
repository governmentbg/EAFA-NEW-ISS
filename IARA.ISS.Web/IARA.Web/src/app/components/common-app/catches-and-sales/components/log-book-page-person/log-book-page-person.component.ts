import { AfterViewInit, Component, DoCheck, Input, OnInit, Self, ViewChild } from '@angular/core';
import { AbstractControl, ControlValueAccessor, FormControl, FormGroup, NgControl, ValidationErrors, Validator, Validators } from '@angular/forms';
import { Subscription } from 'rxjs';

import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { LogBookPagePersonDTO } from '@app/models/generated/dtos/LogBookPagePersonDTO';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { LogBookPagePersonTypesEnum } from '@app/enums/log-book-page-person-types.enum';
import { FormControlDataLoader } from '@app/shared/utils/form-control-data-loader';
import { ICatchesAndSalesService } from '@app/interfaces/common-app/catches-and-sales.interface';
import { ValidityCheckerGroupDirective } from '@app/shared/directives/validity-checker/validity-checker-group.directive';
import { PersonFullDataDTO } from '@app/models/generated/dtos/PersonFullDataDTO';
import { LegalFullDataDTO } from '@app/models/generated/dtos/LegalFullDataDTO';


@Component({
    selector: 'log-book-page-person',
    templateUrl: './log-book-page-person.component.html'
})
export class LogBookPagePersonComponent implements OnInit, AfterViewInit, DoCheck, ControlValueAccessor, Validator {
    @Input()
    public readonly: boolean = false;

    @Input()
    public isIdReadOnly: boolean = true;

    @Input()
    public isForeigner: boolean = false;

    @Input()
    public showOnlyBasicData: boolean = false;

    @Input()
    public service!: ICatchesAndSalesService;

    public readonly logBookPagePersonTypesEnum: typeof LogBookPagePersonTypesEnum = LogBookPagePersonTypesEnum;

    public form!: FormGroup;
    public model!: LogBookPagePersonDTO;

    public personTypes: NomenclatureDTO<number>[] = [];
    public registeredBuyers: NomenclatureDTO<number>[] = [];

    @ViewChild(ValidityCheckerGroupDirective)
    private validityCheckerGroup!: ValidityCheckerGroupDirective;

    private translationService: FuseTranslationLoaderService;

    private ngControl: NgControl;
    private onChanged: (value: LogBookPagePersonDTO) => void = (value: LogBookPagePersonDTO) => { return; };
    private onTouched: (value: LogBookPagePersonDTO) => void = (value: LogBookPagePersonDTO) => { return; };

    private isDisabled: boolean = false;

    private readonly loader: FormControlDataLoader;

    public constructor(
        @Self() ngControl: NgControl,
        translate: FuseTranslationLoaderService
    ) {
        this.ngControl = ngControl;
        this.ngControl.valueAccessor = this;

        this.translationService = translate;

        this.loader = new FormControlDataLoader(this.getNomenclatures.bind(this));

        this.personTypes = [
            new NomenclatureDTO({
                value: LogBookPagePersonTypesEnum.Person,
                displayName: this.translationService.getValue('catches-and-sales.log-book-page-person-person-type'),
                isActive: true
            }),
            new NomenclatureDTO({
                value: LogBookPagePersonTypesEnum.LegalPerson,
                displayName: this.translationService.getValue('catches-and-sales.log-book-page-person-person-legal-type'),
                isActive: true
            }),
            new NomenclatureDTO({
                value: LogBookPagePersonTypesEnum.RegisteredBuyer,
                displayName: this.translationService.getValue('catches-and-sales.log-book-page-person-registered-buyer-type'),
                isActive: true
            })
        ];

        this.buildForm();
    }

    public ngOnInit(): void {
        if (this.ngControl.control) {
            this.ngControl.control.validator = this.validate.bind(this);
        }

        this.loader.load();
    }

    public ngAfterViewInit(): void {
        this.form.valueChanges.subscribe({
            next: () => {
                const value: LogBookPagePersonDTO = this.getValue();
                this.onChanged(value);
            }
        });

        this.form.get('personTypeControl')!.valueChanges.subscribe({
            next: (selectedPersonType: NomenclatureDTO<LogBookPagePersonTypesEnum> | undefined | null) => {
                this.form.get('personControl')!.reset();
                this.form.get('personControl')!.setValidators(null);

                if (!this.showOnlyBasicData) {
                    this.form.get('addressesControl')!.reset();
                    this.form.get('addressesControl')!.setValidators(null);
                }

                this.form.get('legalControl')!.reset();
                this.form.get('legalControl')!.setValidators(null);
                this.form.get('registeredBuyerControl')!.reset();
                this.form.get('registeredBuyerControl')!.setValidators(null);

                if (selectedPersonType !== null && selectedPersonType !== undefined) {
                    switch (selectedPersonType.value) {
                        case LogBookPagePersonTypesEnum.Person: {
                            this.form.get('personControl')!.setValidators(Validators.required);
                            if (!this.showOnlyBasicData) {
                                this.form.get('addressesControl')!.setValidators(Validators.required);
                            }
                        } break;
                        case LogBookPagePersonTypesEnum.LegalPerson: {
                            this.form.get('legalControl')!.setValidators(Validators.required);
                            if (!this.showOnlyBasicData) {
                                this.form.get('addressesControl')!.setValidators(Validators.required);
                            }
                        } break;
                        case LogBookPagePersonTypesEnum.RegisteredBuyer: {
                            this.form.get('registeredBuyerControl')!.setValidators(Validators.required);
                        } break;
                    }
                }

                this.form.get('registeredBuyerControl')!.markAsPending();

                if (this.isDisabled === true) {
                    this.form.get('personTypeControl')!.disable({ emitEvent: false });
                    this.form.get('personControl')!.disable({ emitEvent: false });
                    this.form.get('addressesControl')!.disable({ emitEvent: false });
                    this.form.get('legalControl')!.disable({ emitEvent: false });
                    this.form.get('registeredBuyerControl')!.disable({ emitEvent: false });
                }
            }
        });
    }

    public ngDoCheck(): void {
        if (this.ngControl?.control?.touched) {
            this.validate(this.ngControl.control!);
            this.validityCheckerGroup.validate();
        }
    }

    public writeValue(value: LogBookPagePersonDTO): void {
        this.model = value;

        this.loader.load(() => {
            if (this.model !== null && this.model !== undefined) {
                this.fillForm();
                this.onChanged(this.model);
            }
            else {
                this.form.reset();
            }
        });
    }

    public registerOnChange(fn: (value: LogBookPagePersonDTO) => void): void {
        this.onChanged = fn;
    }

    public registerOnTouched(fn: (value: LogBookPagePersonDTO) => void): void {
        this.onTouched = fn;
    }

    public setDisabledState(isDisabled: boolean): void {
        this.isDisabled = isDisabled;
        if (isDisabled) {
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

    public downloadedPersonData(person: PersonFullDataDTO): void {
        this.form.get('personControl')!.setValue(person.person);
        this.form.get('addressesControl')?.setValue(person.addresses);
    }

    public downloadedLegalData(legal: LegalFullDataDTO): void {
        this.form.get('legalControl')!.setValue(legal.legal);
        this.form.get('addressesControl')?.setValue(legal.addresses);
    }

    private fillForm(): void {
        if (this.model.personType !== null && this.model.personType !== undefined) {
            const personTypeNomenclature: NomenclatureDTO<LogBookPagePersonTypesEnum> = this.personTypes.find(x => x.value === this.model.personType)!;
            this.form.get('personTypeControl')!.setValue(personTypeNomenclature);

            switch (this.model.personType) {
                case LogBookPagePersonTypesEnum.RegisteredBuyer: {
                    if (this.model.buyerId !== null && this.model.buyerId !== undefined) {
                        const buyer: NomenclatureDTO<number> = this.registeredBuyers.find(x => x.value === this.model.buyerId)!;
                        this.form.get('registeredBuyerControl')!.setValue(buyer);
                    }
                } break;
                case LogBookPagePersonTypesEnum.Person: {
                    this.form.get('personControl')!.setValue(this.model.person);
                    if (!this.showOnlyBasicData) {
                        this.form.get('addressesControl')!.setValue(this.model.addresses);
                    }
                } break;
                case LogBookPagePersonTypesEnum.LegalPerson: {
                    this.form.get('legalControl')!.setValue(this.model.personLegal);
                    if (!this.showOnlyBasicData) {
                        this.form.get('addressesControl')!.setValue(this.model.addresses);
                    }
                } break;
                default: throw new Error(`Unknown log book page person type: ${this.model.personType!.toString()}`);
            }
        }
    }

    private buildForm(): void {
        this.form = new FormGroup({
            personTypeControl: new FormControl(undefined, Validators.required),
            personControl: new FormControl(),
            addressesControl: new FormControl(),
            legalControl: new FormControl(),
            registeredBuyerControl: new FormControl()
        });
    }

    private getValue(): LogBookPagePersonDTO {
        const model: LogBookPagePersonDTO = new LogBookPagePersonDTO();
        model.personType = this.form.get('personTypeControl')!.value?.value;

        if (model.personType !== null && model.personType !== undefined) {
            switch (model.personType) {
                case LogBookPagePersonTypesEnum.RegisteredBuyer: {
                    model.buyerId = this.form.get('registeredBuyerControl')!.value?.value;
                } break;
                case LogBookPagePersonTypesEnum.Person: {
                    model.person = this.form.get('personControl')!.value;
                    if (!this.showOnlyBasicData) {
                        model.addresses = this.form.get('addressesControl')!.value;
                    }
                } break;
                case LogBookPagePersonTypesEnum.LegalPerson: {
                    model.personLegal = this.form.get('legalControl')!.value;
                    if (!this.showOnlyBasicData) {
                        model.addresses = this.form.get('addressesControl')!.value;
                    }
                } break;
            }
        }

        return model;
    }

    private getNomenclatures(): Subscription {
        const subscription: Subscription = this.service.getRegisteredBuyersNomenclature().subscribe({
            next: (values: NomenclatureDTO<number>[]) => {
                this.registeredBuyers = values;
                this.loader.complete();
            }
        })

        return subscription;
    }
}