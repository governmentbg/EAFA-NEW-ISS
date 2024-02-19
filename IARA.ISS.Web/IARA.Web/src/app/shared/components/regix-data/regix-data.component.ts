import { AfterViewInit, Component, EventEmitter, Input, OnChanges, OnInit, Optional, Output, Self, SimpleChange, SimpleChanges } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, NgControl, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { forkJoin, Observable, Subscription } from 'rxjs';

import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { GenderEnum } from '@app/enums/gender.enum';
import { IdentifierTypeEnum } from '@app/enums/identifier-type.enum';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { CustodianOfPropertyDTO } from '@app/models/generated/dtos/CustodianOfPropertyDTO';
import { EgnLncDTO } from '@app/models/generated/dtos/EgnLncDTO';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { PersonDocumentDTO } from '@app/models/generated/dtos/PersonDocumentDTO';
import { RegixLegalDataDTO } from '@app/models/generated/dtos/RegixLegalDataDTO';
import { RegixPersonDataDTO } from '@app/models/generated/dtos/RegixPersonDataDTO';
import { CommonNomenclatures } from '@app/services/common-app/common-nomenclatures.service';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { EgnUtils } from '@app/shared/utils/egn.utils';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { TLValidators } from '@app/shared/utils/tl-validators';
import { GetControlErrorLabelTextCallback } from '@app/shared/components/input-controls/base-tl-control';
import { TLError } from '@app/shared/components/input-controls/models/tl-error.model';
import { FormControlDataLoader } from '@app/shared/utils/form-control-data-loader';
import { DocumentTypeEnum } from '@app/enums/document-type.enum';
import { ValidityCheckerDirective } from '@app/shared/directives/validity-checker/validity-checker.directive';
import { NotifyingCustomFormControl } from '@app/shared/utils/notifying-custom-form-control';
import { NotifierDirective } from '@app/shared/directives/notifier/notifier.directive';
import { PersonLegalExtractorService } from '@app/services/common-app/person-legal-extractor.service';
import { PersonFullDataDTO } from '@app/models/generated/dtos/PersonFullDataDTO';
import { LegalFullDataDTO } from '@app/models/generated/dtos/LegalFullDataDTO';
import { IS_PUBLIC_APP } from '../../modules/application.modules';

export class RegixDateOfBirthProperties {
    public min?: Date;
    public max?: Date;
    public validators: ValidatorFn[] = [];
    public getControlErrorLabelText?: GetControlErrorLabelTextCallback;

    public constructor(props?: Partial<RegixDateOfBirthProperties>) {
        Object.assign(this, props);
    }
}

@Component({
    selector: 'regix-data',
    templateUrl: './regix-data.component.html',
    styleUrls: ['./regix-data.component.scss']
})
export class RegixDataComponent extends NotifyingCustomFormControl<RegixPersonDataDTO | RegixLegalDataDTO> implements OnInit, AfterViewInit, OnChanges {
    @Input()
    public isPerson!: boolean;

    @Input()
    public isForeigner: boolean = false;

    @Input()
    public includeForeigner: boolean = false;

    @Input()
    public guidIdentifier: boolean = false;

    @Input()
    public checkboxLabel: string | null = null;

    @Input()
    public isIdReadOnly: boolean = false;

    @Input()
    public showCustodianOfProperty: boolean = false;

    @Input()
    public custodianOfPropertyValue: CustodianOfPropertyDTO | undefined;

    @Input()
    public hideDocument: boolean = false;

    @Input()
    public allDocumentFieldsRequired: boolean = false;

    @Input()
    public showGender: boolean = false;

    @Input()
    public isEmailRequired: boolean = false;

    @Input()
    public isPhoneNumberRequired: boolean = false;

    @Input()
    public showOnlyBasicData: boolean = false;

    @Input()
    public disableOnlyBasicData: boolean = false;

    @Input()
    public hideSearchButton: boolean = false;

    @Input()
    public expectedResults: RegixPersonDataDTO | RegixLegalDataDTO | undefined;

    @Input()
    public dateOfBirthProperties!: RegixDateOfBirthProperties;

    @Input()
    public dateOfBirthRequiredForLncAndForId: boolean = false;

    @Input()
    public readonly: boolean = false;

    @Input()
    public middleNameRequired: boolean = false;

    @Input()
    public isIdentityRequired: boolean = true;

    @Input()
    public isAssociation: boolean = false;

    @Input()
    public showExpectedResults: boolean = false;

    @Output()
    public downloadDataBtnClicked: EventEmitter<PersonFullDataDTO | LegalFullDataDTO> = new EventEmitter<PersonFullDataDTO | LegalFullDataDTO>();

    public readonly today: Date = new Date();

    public showSearchButton: boolean;

    public expectedRegixResults: RegixPersonDataDTO | RegixLegalDataDTO | undefined;

    public documentTypes: NomenclatureDTO<number>[] = [];
    public countries: NomenclatureDTO<number>[] = [];
    public genders: NomenclatureDTO<number>[] = [];

    public getControlErrorLabelTextMethod: GetControlErrorLabelTextCallback = this.getControlErrorLabelText.bind(this);
    public getControlErrorLabelTextDateOfBirthMethod: GetControlErrorLabelTextCallback = this.getControlErrorLabelTextDateOfBirth.bind(this);

    private nomenService: CommonNomenclatures;
    private translate: FuseTranslationLoaderService;
    private personLegalExtractor: PersonLegalExtractorService;

    private dateOfBirthProps: RegixDateOfBirthProperties = new RegixDateOfBirthProperties();

    private id: number | undefined;

    private readonly loader: FormControlDataLoader;
    private readonly cache = new Map<string, PersonFullDataDTO | LegalFullDataDTO | null>();
    private readonly BULGARIA_CODE: string = 'BGR';

    public constructor(
        @Self() ngControl: NgControl,
        nomenService: CommonNomenclatures,
        translate: FuseTranslationLoaderService,
        personLegalExtractor: PersonLegalExtractorService,
        @Optional() @Self() validityChecker: ValidityCheckerDirective,
        @Optional() @Self() notifier: NotifierDirective
    ) {
        super(ngControl, true, validityChecker, notifier);
        this.nomenService = nomenService;
        this.translate = translate;
        this.personLegalExtractor = personLegalExtractor;

        this.showSearchButton = !IS_PUBLIC_APP || this.isAssociation;

        this.loader = new FormControlDataLoader(this.getNomenclatures.bind(this));
    }

    public ngOnInit(): void {
        this.initNotifyingCustomFormControl();
        this.disabledOnlyBasicDataChanged();
        this.loader.load();

        if (this.isPerson === true) {
            if (!this.showOnlyBasicData) {
                if (this.checkboxLabel !== null && this.checkboxLabel !== undefined) {
                    this.hasBulgarianAddressRegistrationChanged(true);
                }

                if (!this.hideDocument) {
                    this.loader.load(() => {
                        this.form.get('documentTypeControl')!.setValue(this.documentTypes.find(x => x.code === DocumentTypeEnum[DocumentTypeEnum.LK]));
                    });
                }
            }
        }

        if (!this.readonly || this.showExpectedResults) {
            this.setValidators();
        }
    }

    public ngAfterViewInit(): void {
        if (!this.showOnlyBasicData) {
            if (this.isPerson === true) {
                if (!this.guidIdentifier) {
                    this.form.get('idNumberControl')!.valueChanges.subscribe({
                        next: (value: EgnLncDTO | string) => {
                            if (value !== null && value !== undefined && typeof value !== 'string' && (!this.readonly || this.showExpectedResults)) {
                                this.setDateOfBirthValidators(value);
                            }
                        }
                    });
                }

                if (this.showGender === true) {
                    this.form.get('genderControl')!.valueChanges.subscribe({
                        next: (value: NomenclatureDTO<number>) => {
                            this.form.get('birthDateControl')!.updateValueAndValidity();
                            this.form.get('birthDateControl')!.markAsTouched();
                        }
                    });
                }

                if (this.checkboxLabel !== null && this.checkboxLabel !== undefined) {
                    this.form.get('checkboxControl')!.valueChanges.subscribe({
                        next: (checked: boolean) => {
                            this.hasBulgarianAddressRegistrationChanged(checked);
                        }
                    });
                }
            }
            else {
                this.form.get('custodianCheckboxControl')!.valueChanges.subscribe({
                    next: (checked: boolean) => {
                        if (checked) {
                            this.form.get('custodianEgnControl')!.setValue(this.custodianOfPropertyValue?.egnLnc);
                            this.form.get('custodianFirstNameControl')!.setValue(this.custodianOfPropertyValue?.firstName);
                            this.form.get('custodianMiddleNameControl')!.setValue(this.custodianOfPropertyValue?.middleName);
                            this.form.get('custodianLastNameControl')!.setValue(this.custodianOfPropertyValue?.lastName);

                            this.form.get('custodianEgnControl')!.disable({ emitEvent: false });
                            this.form.get('custodianFirstNameControl')!.disable({ emitEvent: false });
                            this.form.get('custodianMiddleNameControl')!.disable({ emitEvent: false });
                            this.form.get('custodianLastNameControl')!.disable({ emitEvent: false });
                        }
                        else {
                            this.form.get('custodianEgnControl')!.enable({ emitEvent: false });
                            this.form.get('custodianFirstNameControl')!.enable({ emitEvent: false });
                            this.form.get('custodianMiddleNameControl')!.enable({ emitEvent: false });
                            this.form.get('custodianLastNameControl')!.enable({ emitEvent: false });
                        }
                    }
                });
            }
        }
    }

    public ngOnChanges(changes: SimpleChanges): void {
        const expectedResults: RegixPersonDataDTO | RegixLegalDataDTO | undefined = changes['expectedResults']?.currentValue;
        const dateOfBirthProperties: RegixDateOfBirthProperties | undefined = changes['dateOfBirthProperties']?.currentValue;
        const disableOnlyBasicData: boolean | undefined = changes['disableOnlyBasicData']?.currentValue;
        const middleNameRequired: boolean | undefined = changes['middleNameRequired']?.currentValue;
        const custodianOfPropertyValue: SimpleChange | undefined = changes['custodianOfPropertyValue'];

        this.dateOfBirthProps = dateOfBirthProperties ?? new RegixDateOfBirthProperties();

        if (disableOnlyBasicData !== null && disableOnlyBasicData !== undefined) {
            this.disableOnlyBasicData = disableOnlyBasicData;
            this.disabledOnlyBasicDataChanged();
        }

        if (expectedResults !== null && expectedResults !== undefined) {
            this.expectedRegixResults = expectedResults;

            if ((!this.readonly || this.showExpectedResults)) {
                this.setValidators();
            }
        }

        if (middleNameRequired !== null && middleNameRequired !== undefined && this.isPerson && (!this.readonly || this.showExpectedResults)) {
            this.middleNameRequired = middleNameRequired;
            this.setMiddleNameValidators();
        }

        if (custodianOfPropertyValue && this.showCustodianOfProperty) {
            if (this.form.get('custodianCheckboxControl')!.value === true) {
                this.form.get('custodianEgnControl')!.setValue(this.custodianOfPropertyValue?.egnLnc);
                this.form.get('custodianFirstNameControl')!.setValue(this.custodianOfPropertyValue?.firstName);
                this.form.get('custodianMiddleNameControl')!.setValue(this.custodianOfPropertyValue?.middleName);
                this.form.get('custodianLastNameControl')!.setValue(this.custodianOfPropertyValue?.lastName);
            }
        }
    }

    public writeValue(value: RegixPersonDataDTO | RegixLegalDataDTO): void {
        setTimeout(() => {
            if (value !== null && value !== undefined) {
                if (this.isPerson === true) {
                    this.loader.load(() => {
                        const person: RegixPersonDataDTO = value as RegixPersonDataDTO;
                        this.form.get('idNumberControl')!.setValue(person.egnLnc);
                        this.form.get('firstNameControl')!.setValue(person.firstName);
                        this.form.get('middleNameControl')!.setValue(person.middleName);
                        this.form.get('lastNameControl')!.setValue(person.lastName);

                        if (!this.showOnlyBasicData) {
                            if (this.checkboxLabel !== null && this.checkboxLabel !== undefined) {
                                this.form.get('checkboxControl')!.setValue(person.hasBulgarianAddressRegistration);
                            }

                            if (!this.hideDocument) {
                                if (person.document?.documentTypeID !== null && person.document?.documentTypeID !== undefined) {
                                    this.form.get('documentTypeControl')!.setValue(this.documentTypes.find(x => x.value === person.document?.documentTypeID));
                                }
                                else {
                                    this.form.get('documentTypeControl')!.setValue(undefined);
                                }

                                this.form.get('documentNumControl')!.setValue(person.document?.documentNumber);
                                this.form.get('documentIssueDateControl')!.setValue(person.document?.documentIssuedOn);
                                this.form.get('documentIssuerControl')!.setValue(person.document?.documentIssuedBy);
                            }

                            if (person.citizenshipCountryId !== undefined && person.citizenshipCountryId !== null) {
                                this.form.get('citizenshipControl')!.setValue(this.countries.find(x => x.value === person.citizenshipCountryId));
                            }
                            else {
                                this.form.get('citizenshipControl')!.setValue(undefined);
                            }

                            if (this.showGender) {
                                if (person.genderId !== null && person.genderId !== undefined) {
                                    this.form.get('genderControl')!.setValue(this.genders.find(x => x.value === person.genderId));
                                }
                                else {
                                    this.form.get('genderControl')!.setValue(undefined);
                                }
                            }

                            this.form.get('birthDateControl')!.setValue(person.birthDate);
                            this.form.get('phoneControl')!.setValue(value.phone);
                            this.form.get('emailControl')!.setValue(value.email);
                        }

                        this.notify();
                    });
                }
                else {
                    const company: RegixLegalDataDTO = value as RegixLegalDataDTO;
                    this.id = company.id;
                    this.form.get('idNumberControl')!.setValue(company.eik);
                    this.form.get('companyNameControl')!.setValue(company.name);
                    this.form.get('phoneControl')!.setValue(company.phone);
                    this.form.get('emailControl')!.setValue(company.email);

                    if (!this.showOnlyBasicData) {
                        if (this.showCustodianOfProperty) {
                            this.form.get('custodianCheckboxControl')!.setValue(company.isCustodianOfPropertySameAsApplicant ?? false);
                            this.form.get('custodianEgnControl')!.setValue(company.custodianOfProperty?.egnLnc);
                            this.form.get('custodianFirstNameControl')!.setValue(company.custodianOfProperty?.firstName);
                            this.form.get('custodianMiddleNameControl')!.setValue(company.custodianOfProperty?.middleName);
                            this.form.get('custodianLastNameControl')!.setValue(company.custodianOfProperty?.lastName);
                        }
                    }

                    this.notify();
                }
            }
            else {
                this.form.reset();

                if (this.isPerson === true) {
                    if (this.checkboxLabel !== null && this.checkboxLabel !== undefined) {
                        this.form.get('checkboxControl')!.setValue(true);

                        this.hasBulgarianAddressRegistrationChanged(true);
                    }

                    if (!this.hideDocument) {
                        this.loader.load(() => {
                            this.form.get('documentTypeControl')!.setValue(this.documentTypes.find(x => x.code === DocumentTypeEnum[DocumentTypeEnum.LK]));
                        });
                    }
                }
            }

            if (this.isDisabled) {
                this.form.disable({ emitEvent: false });
            }
        });
    }

    public validate(control: AbstractControl): ValidationErrors | null {
        const errors: ValidationErrors = {};
        for (const key of Object.keys(this.form.controls)) {
            if (key === 'idNumberControl' || key === 'custodianEgnControl') {
                for (const error in this.form.controls[key].errors) {
                    if (!['egn', 'pnf', 'eik'].includes(error) && !['expectedValueNotMatching'].includes(error)) {
                        errors[error] = this.form.controls[key].errors![error];
                    }
                }
            }
            else {
                if (this.form.controls[key].errors !== null && this.form.controls[key].errors !== undefined) {
                    for (const error in this.form.controls[key].errors) {
                        if (!['expectedValueNotMatching'].includes(error)) {
                            errors[error] = this.form.controls[key].errors![error];
                        }
                    }
                }
            }
        }
        return Object.keys(errors).length === 0 ? null : errors;
    }

    public searchBtnClicked(): void {
        const identifier: EgnLncDTO | string = this.form.get('idNumberControl')!.value;

        if (typeof identifier === 'string') {
            const cached: PersonFullDataDTO | LegalFullDataDTO | null | undefined = this.cache.get(identifier);

            if (cached !== undefined) {
                if (cached !== null) {
                    this.downloadDataBtnClicked.emit(cached);
                }
            }
            else {
                this.personLegalExtractor.tryGetLegal(identifier).subscribe({
                    next: (legal: LegalFullDataDTO | undefined) => {
                        if (legal) {
                            this.downloadDataBtnClicked.emit(legal);
                            this.cache.set(identifier, legal);
                        }
                        else {
                            this.cache.set(identifier, null);
                        }
                    }
                });
            }
        }
        else {
            const cached: PersonFullDataDTO | LegalFullDataDTO | null | undefined = this.cache.get(`${identifier.identifierType}|${identifier.egnLnc}`);

            if (cached !== undefined) {
                if (cached !== null) {
                    this.downloadDataBtnClicked.emit(cached);
                }
            }
            else {
                this.personLegalExtractor.tryGetPerson(identifier.identifierType!, identifier.egnLnc!).subscribe({
                    next: (person: PersonFullDataDTO | undefined) => {
                        if (person) {
                            this.downloadDataBtnClicked.emit(person);
                            this.cache.set(`${identifier.identifierType}|${identifier.egnLnc}`, person);
                        }
                        else {
                            this.cache.set(`${identifier.identifierType}|${identifier.egnLnc}`, null);
                        }
                    }
                });
            }
        }
    }

    public custodianSearchBtnClicked(): void {
        const identifier: EgnLncDTO = this.form.get('custodianEgnControl')!.value;

        if (identifier && identifier.egnLnc && identifier.egnLnc.length !== 0) {
            this.personLegalExtractor.tryGetPerson(identifier.identifierType!, identifier.egnLnc).subscribe({
                next: (person: PersonFullDataDTO | undefined) => {
                    if (person) {
                        this.form.get('custodianEgnControl')!.setValue(person.person!.egnLnc);
                        this.form.get('custodianFirstNameControl')!.setValue(person.person!.firstName);
                        this.form.get('custodianMiddleNameControl')!.setValue(person.person!.middleName);
                        this.form.get('custodianLastNameControl')!.setValue(person.person!.lastName);
                    }
                }
            });
        }
    }

    public getControlErrorLabelText(controlName: string, errorValue: unknown, errorCode: string): TLError | undefined {
        const result: TLError | undefined = CommonUtils.getControlErrorLabelTextForRegixExpectedValueValidator(controlName, errorValue, errorCode);
        if (result !== undefined) {
            return result;
        }

        if (errorCode === 'egn') {
            if (errorValue === true) {
                return new TLError({ text: this.translate.getValue('regix-data.invalid-egn'), type: 'warn' });
            }
        }
        else if (errorCode === 'pnf') {
            if (errorValue === true) {
                return new TLError({ text: this.translate.getValue('regix-data.invalid-pnf'), type: 'warn' });
            }
        }
        else if (errorCode === 'eik') {
            if (errorValue === true) {
                return new TLError({ text: this.translate.getValue('regix-data.invalid-eik'), type: 'warn' });
            }
        }
        return undefined;
    }

    public getControlErrorLabelTextDateOfBirth(controlName: string, errorValue: unknown, errorCode: string): TLError | undefined {
        const result: TLError | undefined = CommonUtils.getControlErrorLabelTextForRegixExpectedValueValidator(controlName, errorValue, errorCode);
        if (result !== undefined) {
            return result;
        }

        if (this.dateOfBirthProps.getControlErrorLabelText) {
            return this.dateOfBirthProps.getControlErrorLabelText(controlName, errorValue, errorCode);
        }
        return undefined;
    }

    public onIdNumberEnterDown(): void {
        this.searchBtnClicked();
    }

    public onCustodianEgnEnterDown(): void {
        this.custodianSearchBtnClicked();
    }

    protected buildForm(): AbstractControl {
        return new FormGroup({
            idNumberControl: new FormControl(null),
            firstNameControl: new FormControl(null),
            middleNameControl: new FormControl(null),
            lastNameControl: new FormControl(null),
            companyNameControl: new FormControl(null),
            custodianCheckboxControl: new FormControl(false),
            custodianFirstNameControl: new FormControl(null),
            custodianMiddleNameControl: new FormControl(null),
            custodianLastNameControl: new FormControl(null),
            custodianEgnControl: new FormControl(null),
            documentTypeControl: new FormControl(null),
            documentNumControl: new FormControl(null),
            documentIssueDateControl: new FormControl(null),
            documentIssuerControl: new FormControl(null),
            birthDateControl: new FormControl(null),
            citizenshipControl: new FormControl(null),
            genderControl: new FormControl(null),
            phoneControl: new FormControl(null),
            emailControl: new FormControl(null),
            checkboxControl: new FormControl(true)
        });
    }

    protected getValue(): RegixPersonDataDTO | RegixLegalDataDTO {
        if (this.isPerson) {
            const defaultEgnLnc: EgnLncDTO | undefined = this.guidIdentifier
                ? new EgnLncDTO({
                    identifierType: IdentifierTypeEnum.GUID
                })
                : undefined;

            if (this.showOnlyBasicData) {
                return new RegixPersonDataDTO({
                    egnLnc: this.form.get('idNumberControl')!.value ?? defaultEgnLnc,
                    firstName: this.form.get('firstNameControl')!.value ?? undefined,
                    middleName: this.form.get('middleNameControl')!.value ?? undefined,
                    lastName: this.form.get('lastNameControl')!.value ?? undefined
                });
            }
            else {
                return new RegixPersonDataDTO({
                    egnLnc: this.form.get('idNumberControl')!.value ?? defaultEgnLnc,
                    firstName: this.form.get('firstNameControl')!.value ?? undefined,
                    middleName: this.form.get('middleNameControl')!.value ?? undefined,
                    lastName: this.form.get('lastNameControl')!.value ?? undefined,
                    document: !this.hideDocument
                        ? new PersonDocumentDTO({
                            documentTypeID: this.form.get('documentTypeControl')!.value?.value ?? undefined,
                            documentNumber: this.form.get('documentNumControl')!.value ?? undefined,
                            documentIssuedOn: this.form.get('documentIssueDateControl')!.value ?? undefined,
                            documentIssuedBy: this.form.get('documentIssuerControl')!.value ?? undefined
                        })
                        : undefined,
                    citizenshipCountryId: this.form.get('citizenshipControl')!.value?.value ?? undefined,
                    genderId: this.form.get('genderControl')!.value?.value ?? undefined,
                    phone: this.form.get('phoneControl')!.value ?? undefined,
                    email: this.form.get('emailControl')!.value ?? undefined,
                    birthDate: this.form.get('birthDateControl')!.value ?? undefined,
                    hasBulgarianAddressRegistration: this.form.get('checkboxControl')!.value ?? undefined
                });
            }
        }
        else {
            if (this.showOnlyBasicData) {
                return new RegixLegalDataDTO({
                    id: this.id,
                    eik: this.form.get('idNumberControl')!.value ?? undefined,
                    name: this.form.get('companyNameControl')!.value ?? undefined,
                    isCustodianOfPropertySameAsApplicant: false
                });
            }
            else {
                return new RegixLegalDataDTO({
                    id: this.id,
                    eik: this.form.get('idNumberControl')!.value ?? undefined,
                    name: this.form.get('companyNameControl')!.value ?? undefined,
                    phone: this.form.get('phoneControl')!.value ?? undefined,
                    email: this.form.get('emailControl')!.value ?? undefined,
                    isCustodianOfPropertySameAsApplicant: this.showCustodianOfProperty
                        ? this.form.get('custodianCheckboxControl')!.value ?? false
                        : false,
                    custodianOfProperty: this.showCustodianOfProperty
                        ? new CustodianOfPropertyDTO({
                            egnLnc: this.form.get('custodianEgnControl')!.value ?? undefined,
                            firstName: this.form.get('custodianFirstNameControl')!.value ?? undefined,
                            middleName: this.form.get('custodianMiddleNameControl')!.value ?? undefined,
                            lastName: this.form.get('custodianLastNameControl')!.value ?? undefined,
                        })
                        : undefined
                });
            }
        }
    }

    private setValidators(): void {
        if (this.isPerson) {
            const expectedResults: RegixPersonDataDTO | undefined = this.expectedRegixResults as RegixPersonDataDTO;

            if (this.isIdentityRequired && !this.guidIdentifier) {
                this.form.get('idNumberControl')!.setValidators(Validators.required);
            }
            else {
                this.form.get('idNumberControl')!.clearValidators();
            }

            this.form.get('firstNameControl')!.setValidators([
                TLValidators.expectedValueMatch(expectedResults?.firstName), Validators.required, Validators.maxLength(200)
            ]);

            this.setMiddleNameValidators();

            this.form.get('lastNameControl')!.setValidators([
                TLValidators.expectedValueMatch(expectedResults?.lastName), Validators.required, Validators.maxLength(200)
            ]);

            if (!this.showOnlyBasicData) {
                if (!this.hideDocument) {
                    this.form.get('documentTypeControl')!.setValidators([
                        TLValidators.expectedValueMatch(expectedResults?.document?.documentTypeName), Validators.required
                    ]);
                    this.form.get('documentNumControl')!.setValidators([
                        TLValidators.expectedValueMatch(expectedResults?.document?.documentNumber), Validators.required, Validators.maxLength(50)
                    ]);

                    if (this.allDocumentFieldsRequired) {
                        this.form.get('documentIssueDateControl')!.setValidators([
                            TLValidators.expectedValueMatch(expectedResults?.document?.documentIssuedOn), Validators.required
                        ]);
                        this.form.get('documentIssuerControl')!.setValidators([
                            TLValidators.expectedValueMatch(expectedResults?.document?.documentIssuedBy), Validators.required, Validators.maxLength(50)
                        ]);
                    }
                    else {
                        this.form.get('documentIssueDateControl')!.setValidators([
                            TLValidators.expectedValueMatch(expectedResults?.document?.documentIssuedOn)
                        ]);
                        this.form.get('documentIssuerControl')!.setValidators([
                            TLValidators.expectedValueMatch(expectedResults?.document?.documentIssuedBy), Validators.maxLength(50)
                        ]);
                    }
                }

                this.form.get('citizenshipControl')!.setValidators([
                    TLValidators.expectedValueMatch(expectedResults?.citizenshipCountryName)
                ]);

                if (this.showGender) {
                    this.form.get('genderControl')!.setValidators([
                        TLValidators.expectedValueMatch(expectedResults?.genderName), Validators.required
                    ]);
                }

                if (this.isPhoneNumberRequired) {
                    this.form.get('phoneControl')!.setValidators([
                        TLValidators.expectedValueMatch(expectedResults?.phone), Validators.required, Validators.maxLength(50)
                    ]);
                }
                else {
                    this.form.get('phoneControl')!.setValidators([
                        TLValidators.expectedValueMatch(expectedResults?.phone), Validators.maxLength(50)
                    ]);
                }

                if (this.isEmailRequired) {
                    this.form.get('emailControl')!.setValidators([
                        TLValidators.expectedValueMatch(expectedResults?.email), Validators.required, Validators.email, Validators.maxLength(256)
                    ]);
                }
                else {
                    this.form.get('emailControl')!.setValidators([
                        TLValidators.expectedValueMatch(expectedResults?.email), Validators.email, Validators.maxLength(256)
                    ]);
                }

                this.setDateOfBirthValidators();
            }
        }
        else {
            const expectedResults: RegixLegalDataDTO | undefined = this.expectedRegixResults as RegixLegalDataDTO;

            if (this.isIdentityRequired) {
                this.form.get('idNumberControl')!.setValidators([
                    TLValidators.expectedValueMatch(expectedResults?.eik), Validators.required, Validators.maxLength(13), TLValidators.eik
                ]);
            }
            else {
                this.form.get('idNumberControl')!.setValidators([
                    TLValidators.expectedValueMatch(expectedResults?.eik), Validators.maxLength(13), TLValidators.eik
                ]);
            }

            this.form.get('companyNameControl')!.setValidators([
                TLValidators.expectedValueMatch(expectedResults?.name), Validators.required, Validators.maxLength(500)
            ]);

            if (!this.showOnlyBasicData) {
                if (this.isPhoneNumberRequired === true) {
                    this.form.get('phoneControl')!.setValidators([
                        TLValidators.expectedValueMatch(expectedResults?.phone), Validators.required, Validators.maxLength(50)
                    ]);
                }
                else {
                    this.form.get('phoneControl')!.setValidators([
                        TLValidators.expectedValueMatch(expectedResults?.phone), Validators.maxLength(50)
                    ]);
                }

                if (this.isEmailRequired === true) {
                    this.form.get('emailControl')!.setValidators([
                        TLValidators.expectedValueMatch(expectedResults?.email), Validators.required, Validators.email, Validators.maxLength(256)
                    ]);
                }
                else {
                    this.form.get('emailControl')!.setValidators([
                        TLValidators.expectedValueMatch(expectedResults?.email), Validators.email, Validators.maxLength(256)
                    ]);
                }

                if (this.showCustodianOfProperty === true) {
                    this.form.get('custodianEgnControl')!.setValidators([Validators.required]);

                    this.form.get('custodianFirstNameControl')!.setValidators([
                        TLValidators.expectedValueMatch(expectedResults?.custodianOfProperty?.firstName), Validators.required, Validators.maxLength(200)
                    ]);
                    this.form.get('custodianMiddleNameControl')!.setValidators([
                        TLValidators.expectedValueMatch(expectedResults?.custodianOfProperty?.middleName), Validators.maxLength(200)
                    ]);
                    this.form.get('custodianLastNameControl')!.setValidators([
                        TLValidators.expectedValueMatch(expectedResults?.custodianOfProperty?.lastName), Validators.required, Validators.maxLength(200)
                    ]);
                }
            }
        }

        this.form.updateValueAndValidity();

        if (this.isDisabled) {
            this.form.disable({ emitEvent: false });
        }
    }

    private setMiddleNameValidators(): void {
        if (this.isPerson) {
            const expectedResults: RegixPersonDataDTO | undefined = this.expectedRegixResults as RegixPersonDataDTO;

            if (this.middleNameRequired) {
                this.form.get('middleNameControl')!.setValidators([
                    TLValidators.expectedValueMatch(expectedResults?.middleName),
                    Validators.maxLength(200),
                    Validators.required
                ]);

                this.form.get('middleNameControl')!.markAsPending({ onlySelf: true, emitEvent: false });
            }
            else {
                this.form.get('middleNameControl')!.setValidators([
                    TLValidators.expectedValueMatch(expectedResults?.middleName),
                    Validators.maxLength(200)
                ]);
            }
        }
    }

    private setDateOfBirthValidators(egnLnc?: EgnLncDTO): void {
        if (!this.isDisabled && egnLnc !== undefined && egnLnc !== null) {
            const validators: ValidatorFn[] = [
                TLValidators.expectedValueMatch((this.expectedRegixResults as RegixPersonDataDTO)?.birthDate),
                ...this.dateOfBirthProps.validators
            ];

            if (egnLnc.identifierType === IdentifierTypeEnum.EGN) {
                if (this.form.get('idNumberControl')!.valid && egnLnc.egnLnc !== null && egnLnc.egnLnc !== undefined) {
                    if (this.showGender) {
                        const gender: GenderEnum = EgnUtils.getPersonSexFromEgn(egnLnc.egnLnc!);
                        this.form.get('genderControl')!.setValue(this.genders.find(x => x.code === GenderEnum[gender]));
                        this.form.get('genderControl')!.markAsTouched();
                    }

                    this.form.get('birthDateControl')!.setValue(EgnUtils.getDateOfBirthFromEgn(egnLnc.egnLnc!));
                    this.form.get('birthDateControl')!.markAsTouched();
                }
            }
            else if (egnLnc.identifierType === IdentifierTypeEnum.LNC || egnLnc.identifierType === IdentifierTypeEnum.FORID) {
                if (this.dateOfBirthRequiredForLncAndForId) {
                    validators.unshift(Validators.required);
                }
            }

            this.form.get('birthDateControl')!.setValidators(validators);
            this.form.get('birthDateControl')!.markAsPending({ emitEvent: false });
        }
    }

    private hasBulgarianAddressRegistrationChanged(yes: boolean): void {
        if (yes) {
            this.loader.load(() => {
                this.form.get('citizenshipControl')!.setValue(this.countries.find(x => x.code === this.BULGARIA_CODE));
            });
        }
    }

    private disabledOnlyBasicDataChanged(): void {
        if (this.disableOnlyBasicData === true) {
            if (this.isPerson === true) {
                this.form.get('idNumberControl')!.disable();
                this.form.get('firstNameControl')!.disable();
                this.form.get('middleNameControl')!.disable();
                this.form.get('lastNameControl')!.disable();
            }
            else {
                this.form.get('idNumberControl')!.disable();
                this.form.get('companyNameControl')!.disable();
            }
        }
        else if (!this.form.disabled) {
            if (this.isPerson === true) {
                this.form.get('idNumberControl')!.enable();
                this.form.get('firstNameControl')!.enable();
                this.form.get('middleNameControl')!.enable();
                this.form.get('lastNameControl')!.enable();
            }
            else {
                this.form.get('idNumberControl')!.enable();
                this.form.get('companyNameControl')!.enable();
            }
        }
    }

    private getNomenclatures(): Subscription {
        const observables: Observable<NomenclatureDTO<number>[]>[] = [];

        if (this.isPerson === true) {
            observables.push(NomenclatureStore.instance.getNomenclature(
                NomenclatureTypes.Countries, this.nomenService.getCountries.bind(this.nomenService), false
            ));

            if (!this.showOnlyBasicData) {
                if (!this.hideDocument) {
                    observables.push(NomenclatureStore.instance.getNomenclature(
                        NomenclatureTypes.DocumentTypes, this.nomenService.getDocumentTypes.bind(this.nomenService), false
                    ));
                }

                if (this.showGender) {
                    observables.push(NomenclatureStore.instance.getNomenclature(
                        NomenclatureTypes.Genders, this.nomenService.getGenders.bind(this.nomenService)
                    ));
                }
            }
        }

        const subscription: Subscription = forkJoin(observables).subscribe({
            next: (nomenclatures: NomenclatureDTO<number>[][]) => {
                if (this.isPerson === true) {
                    this.countries = nomenclatures[0];

                    if (!this.showOnlyBasicData) {
                        if (!this.hideDocument) {
                            this.documentTypes = nomenclatures[1];
                        }

                        if (this.showGender) {
                            this.genders = nomenclatures[2];
                        }
                    }
                }
                this.loader.complete();
            }
        });

        return subscription;
    }
}
