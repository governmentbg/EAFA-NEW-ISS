import { Component, Input, OnChanges, OnInit, Self, SimpleChanges } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, NgControl, Validators } from '@angular/forms';

import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { LogBookPagePersonTypesEnum } from '@app/enums/log-book-page-person-types.enum';
import { CustomFormControl } from '@app/shared/utils/custom-form-control';
import { InspectionSubjectPersonnelDTO } from '@app/models/generated/dtos/InspectionSubjectPersonnelDTO';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { RegixPersonDataDTO } from '@app/models/generated/dtos/RegixPersonDataDTO';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { InspectionUtils } from '@app/shared/utils/inspection.utils';
import { RegixLegalDataDTO } from '@app/models/generated/dtos/RegixLegalDataDTO';
import { InspectedPersonTypeEnum } from '@app/enums/inspected-person-type.enum';
import { InspectionShipSubjectNomenclatureDTO } from '@app/models/generated/dtos/InspectionShipSubjectNomenclatureDTO';
import { IdentifierTypeEnum } from '@app/enums/identifier-type.enum';
import { PersonFullDataDTO } from '@app/models/generated/dtos/PersonFullDataDTO';
import { LegalFullDataDTO } from '@app/models/generated/dtos/LegalFullDataDTO';

const PERSON_IDX: number = 0;
const LEGAL_IDX: number = 1;

@Component({
    selector: 'inspected-ship-subject',
    templateUrl: './inspected-ship-subject.component.html'
})
export class InspectedShipSubjectComponent extends CustomFormControl<InspectionSubjectPersonnelDTO | undefined> implements OnInit, OnChanges {

    @Input()
    public title!: string;

    @Input()
    public label!: string;

    @Input()
    public personType!: InspectedPersonTypeEnum;

    @Input()
    public legalType: InspectedPersonTypeEnum | undefined;

    @Input()
    public subjects: InspectionShipSubjectNomenclatureDTO[] = [];

    @Input()
    public countries: NomenclatureDTO<number>[] = [];

    public isFromRegister: boolean = true;
    public hasSubjects: boolean = true;

    public readonly logBookPagePersonTypesEnum: typeof LogBookPagePersonTypesEnum = LogBookPagePersonTypesEnum;

    public personTypes: NomenclatureDTO<LogBookPagePersonTypesEnum>[] = [];

    private model: InspectionSubjectPersonnelDTO | undefined;

    private readonly translate: FuseTranslationLoaderService;

    public constructor(
        @Self() ngControl: NgControl,
        translate: FuseTranslationLoaderService,
    ) {
        super(ngControl);

        this.translate = translate;

        this.personTypes = [
            new NomenclatureDTO({
                value: LogBookPagePersonTypesEnum.Person,
                displayName: translate.getValue('catches-and-sales.log-book-page-person-person-type'),
                isActive: true
            }),
            new NomenclatureDTO({
                value: LogBookPagePersonTypesEnum.LegalPerson,
                displayName: translate.getValue('catches-and-sales.log-book-page-person-person-legal-type'),
                isActive: true
            })
        ];

        this.buildForm();

        this.onMarkAsTouched.subscribe({
            next: () => {
                this.control.updateValueAndValidity();
            }
        });
    }

    public ngOnChanges(changes: SimpleChanges): void {
        // This is so the isFromRegister gets set to false
        // when there are no subjects that can be selected.

        const subjects = changes['subjects'];

        if (subjects !== null && subjects !== undefined && !this.isDisabled) {
            if (!subjects.firstChange) {
                if ((subjects.currentValue as InspectionShipSubjectNomenclatureDTO[]).length === 0) {
                    this.hasSubjects = false;
                    this.form.get('personRegisteredControl')!.setValue(false);
                    this.form.get('subjectControl')!.setValue(null);
                }
                else {
                    this.hasSubjects = true;

                    if (this.model !== undefined && this.model !== null) {
                        this.form.get('personRegisteredControl')!.setValue(this.model.isRegistered);
                    }
                    else {
                        this.form.get('personRegisteredControl')!.setValue(true);
                    }

                    if (this.isFromRegister) {
                        this.form.get('personControl')!.setValue(null);
                        this.form.get('legalControl')!.setValue(null);
                        this.form.get('personControl')!.clearValidators();
                        this.form.get('legalControl')!.clearValidators();
                    }
                }

                this.form.get('personRegisteredControl')!.updateValueAndValidity({ emitEvent: false });
                this.form.get('personControl')!.updateValueAndValidity({ emitEvent: false });
                this.form.get('legalControl')!.updateValueAndValidity({ emitEvent: false });
            }
        }
    }

    public ngOnInit(): void {
        this.initCustomFormControl();
    }

    public writeValue(value: InspectionSubjectPersonnelDTO | undefined): void {
        this.form.get('personTypeControl')!.setValue(this.personTypes[PERSON_IDX]);
        this.model = value;

        if (value !== undefined && value !== null) {
            this.isFromRegister = value.isRegistered === true;
            this.form.get('personRegisteredControl')!.setValue(this.isFromRegister);

            if (value.isRegistered === true) {
                this.form.get('subjectControl')!.setValue(new InspectionShipSubjectNomenclatureDTO({
                    address: value.registeredAddress,
                    code: value.isLegal ? value.eik : value.egnLnc?.egnLnc,
                    displayName: value.firstName
                        + (value.middleName === null || value.middleName === undefined ? ' ' : ' ' + value.middleName)
                        + (value.lastName === null || value.lastName === undefined ? ' ' : ' ' + value.lastName),
                    egnLnc: value.egnLnc,
                    eik: value.eik,
                    entryId: value.entryId,
                    firstName: value.firstName,
                    isActive: value.isActive,
                    isLegal: value.isLegal,
                    lastName: value.lastName,
                    middleName: value.middleName,
                    type: value.type,
                    value: value.id,
                    countryId: value.citizenshipId
                }));
            }
            else if (value.isLegal === true) {
                this.form.get('personTypeControl')!.setValue(this.personTypes[LEGAL_IDX]);

                this.form.get('legalControl')!.setValue(
                    new RegixLegalDataDTO({
                        eik: value.eik,
                        name: value.firstName
                    })
                );
            }
            else if (value.egnLnc !== undefined && value.egnLnc !== null) {
                this.form.get('personTypeControl')!.setValue(this.personTypes[PERSON_IDX]);

                this.form.get('personControl')!.setValue(
                    new RegixPersonDataDTO({
                        egnLnc: value.egnLnc,
                        firstName: value.firstName,
                        middleName: value.middleName,
                        lastName: value.lastName
                    })
                );
            }

            this.form.get('addressControl')!.setValue(
                InspectionUtils.buildAddress(value.registeredAddress, this.translate) ?? value.address
            );

            this.form.get('countryControl')!.setValue(this.countries.find(f => f.value === value.citizenshipId));
        }
        else {
            this.form.get('countryControl')!.setValue(this.countries.find(f => f.code === CommonUtils.COUNTRIES_BG));
        }
    }

    public downloadedPersonData(person: PersonFullDataDTO): void {
        this.form.get('personControl')!.setValue(person.person);

        if (person.addresses !== undefined && person.addresses !== null && person.addresses.length > 0) {
            this.form.get('addressControl')!.setValue(
                InspectionUtils.buildAddress(person.addresses[0], this.translate)
            );

            this.form.get('countryControl')!.setValue(this.countries.find(f => f.value === person.addresses![0].countryId));
        }
        else {
            this.form.get('addressControl')!.setValue(null);
            this.form.get('countryControl')!.setValue(null);
        }
    }

    public downloadedLegalData(legal: LegalFullDataDTO): void {
        this.form.get('legalControl')!.setValue(legal.legal);

        if (legal.addresses !== undefined && legal.addresses !== null && legal.addresses.length > 0) {
            this.form.get('addressControl')!.setValue(
                InspectionUtils.buildAddress(legal.addresses[0], this.translate)
            );

            this.form.get('countryControl')!.setValue(this.countries.find(f => f.value === legal.addresses![0].countryId));
        }
        else {
            this.form.get('addressControl')!.setValue(null);
            this.form.get('countryControl')!.setValue(null);
        }
    }

    protected buildForm(): AbstractControl {
        const form = new FormGroup({
            personRegisteredControl: new FormControl(true),
            personTypeControl: new FormControl(undefined, Validators.required),
            subjectControl: new FormControl(undefined, Validators.required),
            personControl: new FormControl({ disabled: true }),
            legalControl: new FormControl({ disabled: true }),
            addressControl: new FormControl({ value: undefined, disabled: true }),
            countryControl: new FormControl({ value: undefined, disabled: true })
        });

        form.get('personRegisteredControl')!.valueChanges.subscribe({
            next: this.onPersonRegisteredChanged.bind(this)
        });

        form.get('subjectControl')!.valueChanges.subscribe({
            next: this.onSubjectControlChanged.bind(this)
        });

        form.get('personTypeControl')!.valueChanges.subscribe({
            next: this.onPersonTypeChanged.bind(this)
        });

        return form;
    }

    protected getValue(): InspectionSubjectPersonnelDTO | undefined {
        const subject: InspectionShipSubjectNomenclatureDTO | undefined = this.form.get('subjectControl')!.value;

        if (!this.isFromRegister) {
            if (this.form.get('personTypeControl')!.value.value === LogBookPagePersonTypesEnum.Person) {
                const person: RegixPersonDataDTO = this.form.get('personControl')!.value;

                if (person === null || person === undefined) {
                    return undefined;
                }

                return new InspectionSubjectPersonnelDTO({
                    isRegistered: false,
                    address: this.form.get('addressControl')!.value,
                    citizenshipId: this.form.get('countryControl')!.value?.value,
                    egnLnc: person.egnLnc,
                    isLegal: false,
                    firstName: person.firstName,
                    middleName: person.middleName,
                    lastName: person.lastName,
                    type: this.personType
                });
            }
            else {
                const legal: RegixLegalDataDTO = this.form.get('legalControl')!.value;

                if (legal === null || legal === undefined) {
                    return undefined;
                }

                return new InspectionSubjectPersonnelDTO({
                    isRegistered: false,
                    address: this.form.get('addressControl')!.value,
                    citizenshipId: this.form.get('countryControl')!.value?.value,
                    isLegal: true,
                    eik: legal.eik,
                    firstName: legal.name,
                    type: this.legalType!
                });
            }
        }
        else if (subject !== null && subject !== undefined) {
            return new InspectionSubjectPersonnelDTO({
                isRegistered: true,
                registeredAddress: subject.address,
                address: InspectionUtils.buildAddress(subject.address, this.translate),
                citizenshipId: subject.countryId ?? subject.address?.countryId,
                egnLnc: subject.egnLnc,
                eik: subject.eik,
                isLegal: subject.isLegal,
                firstName: subject.firstName,
                middleName: subject.middleName,
                lastName: subject.lastName,
                entryId: subject.entryId,
                id: subject.value,
                // This is here since the representer comes from the owner + captain + permit holder
                // and the type in them is not ReprsPers
                type: this.personType === InspectedPersonTypeEnum.ReprsPers
                    ? InspectedPersonTypeEnum.ReprsPers
                    : subject.type
            });
        }

        return undefined;
    }

    private onPersonRegisteredChanged(value: boolean): void {
        this.isFromRegister = value;

        if (value) {
            this.form.get('subjectControl')!.setValidators([Validators.required]);
            this.form.get('addressControl')!.clearValidators();
            this.form.get('countryControl')!.clearValidators();
            this.form.get('personControl')!.clearValidators();
            this.form.get('legalControl')!.clearValidators();

            this.form.get('addressControl')!.disable();
            this.form.get('countryControl')!.disable();
        }
        else {
            this.form.get('subjectControl')!.clearValidators();
            this.form.get('addressControl')!.setValidators([Validators.required, Validators.maxLength(4000)]);
            this.form.get('countryControl')!.setValidators([Validators.required]);

            this.form.get('addressControl')!.enable();
            this.form.get('countryControl')!.enable();

            this.form.get('subjectControl')!.markAsPending();
            this.form.get('addressControl')!.markAsPending();
            this.form.get('countryControl')!.markAsPending();
        }

        this.form.get('subjectControl')!.updateValueAndValidity({ emitEvent: false });
        this.form.get('addressControl')!.updateValueAndValidity({ emitEvent: false });
        this.form.get('countryControl')!.updateValueAndValidity({ emitEvent: false });
        this.form.get('personControl')!.updateValueAndValidity({ emitEvent: false });
        this.form.get('legalControl')!.updateValueAndValidity({ emitEvent: false });

        if (this.isDisabled) {
            this.form.get('addressControl')!.disable();
            this.form.get('countryControl')!.disable();
        }
    }

    private onSubjectControlChanged(value: InspectionShipSubjectNomenclatureDTO): void {
        if (value !== null && value !== undefined) {
            if (value.egnLnc?.identifierType === IdentifierTypeEnum.LEGAL) {
                this.form.get('personControl')!.setValue(new RegixPersonDataDTO({
                    egnLnc: value.egnLnc,
                    firstName: value.firstName,
                    middleName: value.middleName,
                    lastName: value.lastName
                }), { emitEvent: false });
            }
            else {
                this.form.get('personControl')!.setValue(new RegixLegalDataDTO({
                    eik: value.egnLnc?.egnLnc,
                    name: value.firstName
                }), { emitEvent: false });
            }

            if (value.address !== undefined && value.address !== null) {
                this.form.get('addressControl')!.setValue(InspectionUtils.buildAddress(value.address, this.translate) ?? value.description);
                this.form.get('countryControl')!.setValue(this.countries.find(f => f.value === value.address!.countryId));
            }
        }
        else {
            if (this.isFromRegister) {
                this.form.get('addressControl')!.setValue(undefined);
                this.form.get('countryControl')!.setValue(undefined);
            }
        }
    }

    private onPersonTypeChanged(personType: NomenclatureDTO<LogBookPagePersonTypesEnum>): void {
        if (personType !== null && personType !== undefined && this.isFromRegister === false) {
            switch (personType.value) {
                case LogBookPagePersonTypesEnum.Person:
                    if (this.isDisabled === false) {
                        this.form.get('personControl')!.enable();
                    }
                    this.form.get('legalControl')!.disable();
                    break;
                case LogBookPagePersonTypesEnum.LegalPerson:
                    if (this.isDisabled === false) {
                        this.form.get('legalControl')!.enable();
                    }
                    this.form.get('personControl')!.disable();
                    break;
            }
        }
    }
}