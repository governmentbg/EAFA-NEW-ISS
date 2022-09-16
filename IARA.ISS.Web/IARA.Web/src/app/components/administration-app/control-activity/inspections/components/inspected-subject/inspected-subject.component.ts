import { Component, Input, OnInit, Self } from '@angular/core';
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

@Component({
    selector: 'inspected-subject',
    templateUrl: './inspected-subject.component.html'
})
export class InspectedSubjectComponent extends CustomFormControl<InspectionSubjectPersonnelDTO | undefined> implements OnInit {

    @Input()
    public title!: string;

    @Input()
    public personType!: InspectedPersonTypeEnum;

    @Input()
    public legalType!: InspectedPersonTypeEnum;

    @Input()
    public countries: NomenclatureDTO<number>[] = [];

    public readonly logBookPagePersonTypesEnum: typeof LogBookPagePersonTypesEnum = LogBookPagePersonTypesEnum;

    public personTypes: NomenclatureDTO<LogBookPagePersonTypesEnum>[] = [];

    private readonly PERSON_IDX = 0;
    private readonly LEGAL_IDX = 1;

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
    }

    public ngOnInit(): void {
        this.initCustomFormControl();
    }

    public writeValue(value: InspectionSubjectPersonnelDTO | undefined): void {
        this.form.get('personTypeControl')!.setValue(this.personTypes[this.PERSON_IDX]);

        if (value !== undefined && value !== null) {
            if (value.isLegal === true) {
                this.form.get('personTypeControl')!.setValue(this.personTypes[this.LEGAL_IDX]);

                this.form.get('legalControl')!.setValue(
                    new RegixLegalDataDTO({
                        eik: value.eik,
                        name: value.firstName,
                    })
                );
                if (this.isDisabled === false) {
                    this.form.get('legalControl')!.enable();
                }
                this.form.get('personControl')!.disable();
            }
            else if (value.egnLnc !== undefined && value.egnLnc !== null) {
                this.form.get('personTypeControl')!.setValue(this.personTypes[this.PERSON_IDX]);

                this.form.get('personControl')!.setValue(
                    new RegixPersonDataDTO({
                        egnLnc: value.egnLnc,
                        firstName: value.firstName,
                        middleName: value.middleName,
                        lastName: value.lastName,
                    })
                );
                if (this.isDisabled === false) {
                    this.form.get('personControl')!.enable();
                }
                this.form.get('legalControl')!.disable();
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

    protected buildForm(): AbstractControl {
        const form = new FormGroup({
            personTypeControl: new FormControl(undefined, Validators.required),
            personControl: new FormControl(),
            legalControl: new FormControl({ disabled: true }),
            addressControl: new FormControl(undefined),
            countryControl: new FormControl(undefined),
        });

        form.get('personTypeControl')!.valueChanges.subscribe({
            next: this.onPersonTypeChanged.bind(this)
        });

        return form;
    }

    protected getValue(): InspectionSubjectPersonnelDTO | undefined {
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
                type: this.personType,
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
                type: this.legalType,
            });
        }
    }

    private onPersonTypeChanged(personType: NomenclatureDTO<LogBookPagePersonTypesEnum>): void {
        if (personType !== null && personType !== undefined) {
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