import { AfterViewInit, Component, Input, OnInit, Optional, Self } from '@angular/core';
import { CustomFormControl } from '@app/shared/utils/custom-form-control';
import { AuanInspectedEntityDTO } from '@app/models/generated/dtos/AuanInspectedEntityDTO';
import { AbstractControl, FormControl, FormGroup, NgControl, Validators } from '@angular/forms';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { AddressTypesEnum } from '@app/enums/address-types.enum';
import { RegixLegalDataDTO } from '@app/models/generated/dtos/RegixLegalDataDTO';
import { PersonFullDataDTO } from '@app/models/generated/dtos/PersonFullDataDTO';
import { LegalFullDataDTO } from '@app/models/generated/dtos/LegalFullDataDTO';
import { ValidityCheckerDirective } from '@app/shared/directives/validity-checker/validity-checker.directive';

@Component({
    selector: 'inspected-entity-basic-info',
    templateUrl: './inspected-entity-basic-info.component.html'
})
export class InspectedEntityBasicInfoComponent extends CustomFormControl<AuanInspectedEntityDTO> implements OnInit, AfterViewInit {
    @Input()
    public isFromRegister: boolean = false;

    @Input()
    public isIdReadOnly: boolean = false;

    public inspectedEntity: AuanInspectedEntityDTO | undefined;
    public inspectedEntityOptions: NomenclatureDTO<boolean>[] = [];

    public readonly companyHeadquartersType: AddressTypesEnum = AddressTypesEnum.COMPANY_HEADQUARTERS;

    private readonly translate: FuseTranslationLoaderService;

    public constructor(
        @Self() ngControl: NgControl,
        @Self() @Optional() validityChecker: ValidityCheckerDirective,
        translate: FuseTranslationLoaderService
    ) {
        super(ngControl, true, validityChecker);

        this.translate = translate;

        this.inspectedEntityOptions = [
            new NomenclatureDTO<boolean>({
                value: true,
                displayName: this.translate.getValue('auan-register.inspected-entity-person'),
                isActive: true
            }),
            new NomenclatureDTO<boolean>({
                value: false,
                displayName: this.translate.getValue('auan-register.inspected-entity-legal'),
                isActive: true
            })
        ];
    }

    public ngOnInit(): void {
        this.initCustomFormControl();
    }

    public ngAfterViewInit(): void {
        this.form.get('isInspectedEntityPersonControl')!.valueChanges.subscribe({
            next: (isPerson: NomenclatureDTO<boolean> | undefined) => {
                if (isPerson !== undefined && isPerson !== null) {
                    this.form.get('personControl')!.clearValidators();
                    this.form.get('personAddressesControl')!.clearValidators();
                    this.form.get('legalControl')!.clearValidators();
                    this.form.get('legalAddressesControl')!.clearValidators();

                    if (this.inspectedEntity !== undefined && this.inspectedEntity !== null) {
                        this.inspectedEntity!.isPerson = isPerson.value;
                    }
                    else {
                        this.inspectedEntity = new AuanInspectedEntityDTO({
                            isUnregisteredPerson: false,
                            isPerson: isPerson.value
                        });
                    }

                    if (isPerson) {
                        this.form.get('personControl')!.setValidators(Validators.required);
                        this.form.get('personAddressesControl')!.setValidators(Validators.required);
                    }
                    else {
                        this.form.get('legalControl')!.setValidators(Validators.required);
                        this.form.get('legalAddressesControl')!.setValidators(Validators.required);
                    }

                    this.form.get('personControl')!.updateValueAndValidity({ emitEvent: false });
                    this.form.get('personAddressesControl')!.updateValueAndValidity({ emitEvent: false });
                    this.form.get('legalControl')!.updateValueAndValidity({ emitEvent: false });
                    this.form.get('legalAddressesControl')!.updateValueAndValidity({ emitEvent: false });
                }
            }
        });
    }

    public writeValue(value: AuanInspectedEntityDTO): void {
        if (value !== undefined && value !== null) {
            this.inspectedEntity = value;

            if (value.isPerson !== undefined && value.isPerson !== null) {
                this.inspectedEntity.isPerson = value.isPerson;

                if (value.isPerson === true) {
                    if (this.inspectedEntity.person !== undefined && this.inspectedEntity.person !== null) {
                        this.form.get('personControl')!.setValue(this.inspectedEntity.person);
                    }
                    else {
                        this.form.get('personControl')!.setValue(this.inspectedEntity.unregisteredPerson);
                    }

                    this.form.get('personAddressesControl')!.setValue(this.inspectedEntity.addresses);
                    this.form.get('personWorkPlaceControl')!.setValue(value.personWorkPlace);
                    this.form.get('personWorkPositionControl')!.setValue(value.personWorkPosition);
                }
                else if (value.isPerson === false) {
                    if (this.inspectedEntity.legal !== undefined && this.inspectedEntity.legal !== null) {
                        this.form.get('legalControl')!.setValue(this.inspectedEntity.legal);
                    }
                    else {
                        this.form.get('legalControl')!.setValue(
                            new RegixLegalDataDTO({
                                eik: this.inspectedEntity.unregisteredPerson?.eik ?? undefined,
                                name: this.inspectedEntity.unregisteredPerson?.firstName ?? undefined
                            })
                        );
                    }

                    this.form.get('legalAddressesControl')!.setValue(this.inspectedEntity.addresses);
                }

                if (!this.isFromRegister) {
                    this.form.get('isInspectedEntityPersonControl')!.setValue(this.inspectedEntityOptions.find(x => x.value === value.isPerson));
                }
            }
        }
        else {
            if (!this.isFromRegister) {
                this.form.get('isInspectedEntityPersonControl')!.setValue(this.inspectedEntityOptions.find(x => x.value === true));

                this.inspectedEntity = new AuanInspectedEntityDTO({
                    isUnregisteredPerson: false,
                    isPerson: true
                });

                this.form.get('personControl')!.setValue(undefined);
                this.form.get('personAddressesControl')!.setValue(undefined);
                this.form.get('personWorkPlaceControl')!.setValue(undefined);
                this.form.get('personWorkPositionControl')!.setValue(undefined);
                this.form.get('legalControl')!.setValue(undefined);
                this.form.get('legalAddressesControl')!.setValue(undefined);
            }
        }
    }

    public downloadedPersonData(person: PersonFullDataDTO): void {
        this.form.get('personControl')!.setValue(person.person);
        this.form.get('personAddressesControl')!.setValue(person.addresses);
    }

    public downloadedLegalData(legal: LegalFullDataDTO): void {
        this.form.get('legalControl')!.setValue(legal.legal);
        this.form.get('legalAddressesControl')!.setValue(legal.addresses);
    }

    protected getValue(): AuanInspectedEntityDTO {
        if (this.inspectedEntity !== undefined && this.inspectedEntity !== null) {
            const result: AuanInspectedEntityDTO = new AuanInspectedEntityDTO({
                isUnregisteredPerson: false,
                isPerson: this.inspectedEntity!.isPerson === true
            });

            if (result.isPerson === true) {
                result.person = this.form.get('personControl')!.value;
                result.addresses = this.form.get('personAddressesControl')!.value;
                result.personWorkPlace = this.form.get('personWorkPlaceControl')!.value;
                result.personWorkPosition = this.form.get('personWorkPositionControl')!.value;
            }
            else if (result.isPerson === false) {
                result.legal = this.form.get('legalControl')!.value;
                result.addresses = this.form.get('legalAddressesControl')!.value;
            }

            return result;
        }

        return new AuanInspectedEntityDTO();
    }

    protected buildForm(): AbstractControl {
        return new FormGroup({
            personControl: new FormControl(null),
            personAddressesControl: new FormControl(null),
            personWorkPlaceControl: new FormControl(null, Validators.maxLength(100)),
            personWorkPositionControl: new FormControl(null, Validators.maxLength(100)),
            legalControl: new FormControl(null),
            legalAddressesControl: new FormControl(null),
            isInspectedEntityPersonControl: new FormControl(null)
        });
    }
}