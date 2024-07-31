import { Component, EventEmitter, Input, OnInit, Output, Self } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, NgControl, Validators } from '@angular/forms';

import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { CustomFormControl } from '@app/shared/utils/custom-form-control';
import { InspectionSubjectPersonnelDTO } from '@app/models/generated/dtos/InspectionSubjectPersonnelDTO';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { InspectionUtils } from '@app/shared/utils/inspection.utils';
import { RegixLegalDataDTO } from '@app/models/generated/dtos/RegixLegalDataDTO';
import { InspectedPersonTypeEnum } from '@app/enums/inspected-person-type.enum';
import { InspectedBuyerNomenclatureDTO } from '@app/models/generated/dtos/InspectedBuyerNomenclatureDTO';
import { RegixPersonDataDTO } from '@app/models/generated/dtos/RegixPersonDataDTO';
import { PersonFullDataDTO } from '@app/models/generated/dtos/PersonFullDataDTO';
import { LegalFullDataDTO } from '@app/models/generated/dtos/LegalFullDataDTO';

@Component({
    selector: 'inspected-buyer',
    templateUrl: './inspected-buyer.component.html'
})
export class InspectedBuyerComponent extends CustomFormControl<InspectionSubjectPersonnelDTO | undefined> implements OnInit {

    @Output()
    public buyerSelected = new EventEmitter<InspectedBuyerNomenclatureDTO>();

    @Input()
    public title!: string;

    @Input()
    public label!: string;

    @Input()
    public countries: NomenclatureDTO<number>[] = [];

    @Input()
    public buyers: InspectedBuyerNomenclatureDTO[] = [];

    public isFromRegister: boolean = true;
    public isLegal: boolean = true;

    private readonly translate: FuseTranslationLoaderService;

    public constructor(
        @Self() ngControl: NgControl,
        translate: FuseTranslationLoaderService,
    ) {
        super(ngControl);

        this.translate = translate;

        this.buildForm();

        this.onMarkAsTouched.subscribe({
            next: () => {
                this.control.updateValueAndValidity();
            }
        });
    }

    public ngOnInit(): void {
        this.initCustomFormControl();
    }

    public writeValue(value: InspectionSubjectPersonnelDTO | undefined): void {
        if (value !== undefined && value !== null) {
            this.isFromRegister = value.isRegistered === true;
            this.isLegal = value.isLegal === true;

            this.form.get('personRegisteredControl')!.setValue(this.isFromRegister);
            this.form.get('isLegalControl')!.setValue(this.isLegal);

            if (value.isRegistered === true) {
                this.form.get('buyerControl')!.setValue(new InspectedBuyerNomenclatureDTO({
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
                    countryId: value?.citizenshipId,
                }));
            }
            else if (this.isLegal) {
                this.form.get('legalControl')!.setValue(
                    new RegixLegalDataDTO({
                        eik: value.eik,
                        name: value.firstName,
                    })
                );
            }
            else {
                this.form.get('personControl')!.setValue(
                    new RegixPersonDataDTO({
                        firstName: value.firstName,
                        egnLnc: value.egnLnc,
                        middleName: value.middleName,
                        lastName: value.lastName,
                        citizenshipCountryId: value.citizenshipId,
                    })
                );
            }

            this.form.get('addressControl')!.setValue(
                InspectionUtils.buildAddress(value.registeredAddress, this.translate) ?? value.address
            );

            this.form.get('countryControl')!.setValue(this.countries.find(f => f.value === value.citizenshipId));
        }
        else {
            this.form.get('buyerControl')!.setValue(null);
            this.form.get('addressControl')!.setValue(null);
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
            isLegalControl: new FormControl(true),
            buyerControl: new FormControl(undefined, Validators.required),
            legalControl: new FormControl(),
            personControl: new FormControl(),
            addressControl: new FormControl({ value: undefined, disabled: true }),
            countryControl: new FormControl({ value: undefined, disabled: true })
        });

        form.get('personRegisteredControl')!.valueChanges.subscribe({
            next: this.onPersonRegisteredChanged.bind(this)
        });

        form.get('buyerControl')!.valueChanges.subscribe({
            next: this.onBuyerControlChanged.bind(this)
        });

        form.get('isLegalControl')!.valueChanges.subscribe({
            next: (value) => {
                this.isLegal = value;

                if (value) {
                    this.form.get('personControl')!.clearValidators();
                }
                else {
                    this.form.get('legalControl')!.clearValidators();
                }

                this.form.get('personControl')!.updateValueAndValidity({ emitEvent: false });
                this.form.get('legalControl')!.updateValueAndValidity({ emitEvent: false });
            }
        })

        return form;
    }

    protected getValue(): InspectionSubjectPersonnelDTO | undefined {
        const subject: InspectedBuyerNomenclatureDTO | undefined = this.form.get('buyerControl')!.value;

        if (!this.isFromRegister) {
            if (this.isLegal) {
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
                    type: InspectedPersonTypeEnum.RegBuyer,
                });
            }
            else {
                const person: RegixPersonDataDTO = this.form.get('personControl')!.value;

                if (person === null || person === undefined) {
                    return undefined;
                }

                return new InspectionSubjectPersonnelDTO({
                    isRegistered: false,
                    address: this.form.get('addressControl')!.value,
                    citizenshipId: this.form.get('countryControl')!.value?.value,
                    isLegal: false,
                    egnLnc: person.egnLnc,
                    firstName: person.firstName,
                    middleName: person.middleName,
                    lastName: person.lastName,
                    type: InspectedPersonTypeEnum.RegBuyer,
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
                type: InspectedPersonTypeEnum.RegBuyer,
            });
        }

        return undefined;
    }

    private onPersonRegisteredChanged(value: boolean): void {
        this.isFromRegister = value;

        if (value) {
            this.form.get('buyerControl')!.setValidators([Validators.required]);
            this.form.get('addressControl')!.clearValidators();
            this.form.get('countryControl')!.clearValidators();
            this.form.get('personControl')!.clearValidators();
            this.form.get('legalControl')!.clearValidators();

            this.form.get('addressControl')!.disable();
            this.form.get('countryControl')!.disable();
        } else {
            this.form.get('buyerControl')!.clearValidators();
            this.form.get('addressControl')!.setValidators([Validators.required, Validators.maxLength(4000)]);
            this.form.get('countryControl')!.setValidators([Validators.required]);

            this.form.get('addressControl')!.enable();
            this.form.get('countryControl')!.enable();

            this.form.get('addressControl')!.markAsPending();
            this.form.get('countryControl')!.markAsPending();
        }

        this.form.get('buyerControl')!.updateValueAndValidity({ emitEvent: false });
        this.form.get('addressControl')!.updateValueAndValidity({ emitEvent: false });
        this.form.get('countryControl')!.updateValueAndValidity({ emitEvent: false });
        this.form.get('personControl')!.updateValueAndValidity({ emitEvent: false });
        this.form.get('legalControl')!.updateValueAndValidity({ emitEvent: false });

        if (this.isDisabled) {
            this.form.get('addressControl')!.disable();
            this.form.get('countryControl')!.disable();
        }
    }

    private onBuyerControlChanged(value: InspectedBuyerNomenclatureDTO): void {
        if (value !== null && value !== undefined) {
            if (value.address !== undefined && value.address !== null) {
                this.form.get('addressControl')!.setValue(InspectionUtils.buildAddress(value.address, this.translate) ?? value.description);
                this.form.get('countryControl')!.setValue(this.countries.find(f => f.value === value.countryId));
            }
        }

        this.buyerSelected.emit(value);
    }
}