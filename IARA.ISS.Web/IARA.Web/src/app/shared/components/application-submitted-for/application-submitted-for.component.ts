import { Component, Input, OnChanges, OnDestroy, OnInit, Optional, Self, SimpleChange, SimpleChanges } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, NgControl, Validators } from '@angular/forms';
import { Subscription } from 'rxjs';

import { PageCodeEnum } from '@app/enums/page-code.enum';
import { SubmittedByRolesEnum } from '@app/enums/submitted-by-roles.enum';
import { ApplicantRelationToRecipientDTO } from '@app/models/generated/dtos/ApplicantRelationToRecipientDTO';
import { ApplicationSubmittedForDTO } from '@app/models/generated/dtos/ApplicationSubmittedForDTO';
import { ValidityCheckerDirective } from '@app/shared/directives/validity-checker/validity-checker.directive';
import { Notifier } from '@app/shared/directives/notifier/notifier.class';
import { NotifierDirective } from '@app/shared/directives/notifier/notifier.directive';
import { NotifyingCustomFormControl } from '@app/shared/utils/notifying-custom-form-control';
import { CustodianOfPropertyDTO } from '@app/models/generated/dtos/CustodianOfPropertyDTO';
import { ApplicationSubmittedByDTO } from '@app/models/generated/dtos/ApplicationSubmittedByDTO';
import { EgnLncDTO } from '@app/models/generated/dtos/EgnLncDTO';
import { PersonFullDataDTO } from '@app/models/generated/dtos/PersonFullDataDTO';
import { LegalFullDataDTO } from '@app/models/generated/dtos/LegalFullDataDTO';

@Component({
    selector: 'application-submitted-for',
    templateUrl: './application-submitted-for.component.html'
})
export class ApplicationSubmittedForComponent extends NotifyingCustomFormControl<ApplicationSubmittedForDTO> implements OnInit, OnChanges, OnDestroy {
    @Input()
    public relationLabel: string = '';

    @Input()
    public submittedForLabel: string = '';

    @Input()
    public pageCode!: PageCodeEnum;

    @Input()
    public hideRelation: boolean = false;

    @Input()
    public showPersonal: boolean = false;

    @Input()
    public isIdReadOnly: boolean = false;

    @Input()
    public hideDocument: boolean = false;

    @Input()
    public isForeigner: boolean = false;

    @Input()
    public showOnlyBasicData: boolean = false;

    @Input()
    public disabledLegal: boolean = false;

    @Input()
    public showCustodianOfProperty: boolean = true;

    @Input()
    public middleNameRequired: boolean = false;

    @Input()
    public expectedResults!: ApplicationSubmittedForDTO;

    @Input()
    public submittedByControl: AbstractControl | undefined;

    public readonly roles: typeof SubmittedByRolesEnum = SubmittedByRolesEnum;

    public notifierGroup: Notifier = new Notifier();
    public role: SubmittedByRolesEnum | undefined;
    public custodianOfProperty: CustodianOfPropertyDTO | undefined;

    private submittedByControlSub: Subscription | undefined;

    public constructor(
        @Self() ngControl: NgControl,
        @Optional() @Self() validityChecker: ValidityCheckerDirective,
        @Optional() @Self() notifier: NotifierDirective
    ) {
        super(ngControl, true, validityChecker, notifier)

        this.setSubmittedByRoleSubscription();
    }

    public ngOnInit(): void {
        this.initNotifyingCustomFormControl(this.notifierGroup, () => { this.notify(); });

        if (this.disabledLegal) {
            this.form.get('legalControl')!.disable();
        }
    }

    public ngOnChanges(changes: SimpleChanges): void {
        const disabledLegal: SimpleChange | undefined = changes['disabledLegal'];
        const submittedByControl: SimpleChange | undefined = changes['submittedByControl'];

        if (disabledLegal !== null && disabledLegal !== undefined) {
            if (this.disabledLegal) {
                this.form.get('legalControl')!.disable();
            }
            else {
                this.form.get('legalControl')!.disable();
            }
        }

        if (submittedByControl !== null && submittedByControl !== undefined) {
            this.submittedByControlSub = this.submittedByControl?.valueChanges.subscribe({
                next: (value: ApplicationSubmittedByDTO) => {
                    if (value?.person !== undefined && value?.person !== null) {
                        this.custodianOfProperty = new CustodianOfPropertyDTO({
                            egnLnc: new EgnLncDTO({
                                egnLnc: value.person.egnLnc?.egnLnc,
                                identifierType: value.person.egnLnc?.identifierType
                            }),
                            firstName: value.person.firstName,
                            middleName: value.person.middleName,
                            lastName: value.person.lastName
                        });
                    }
                    else {
                        this.custodianOfProperty = undefined;
                    }
                }
            });
        }
    }

    public ngOnDestroy(): void {
        this.submittedByControlSub?.unsubscribe();
    }

    public writeValue(value: ApplicationSubmittedForDTO): void {
        setTimeout(() => {
            if (value !== undefined && value !== null) {
                this.form.get('relationControl')!.setValue(new ApplicantRelationToRecipientDTO({
                    role: value.submittedByRole,
                    letterOfAttorney: value.submittedByLetterOfAttorney
                }));

                if (value.submittedByRole !== undefined && value.submittedByRole !== null) {
                    if (value.submittedByRole === SubmittedByRolesEnum.Personal && this.showPersonal) {
                        this.form.get('personControl')!.setValue(value.person);

                        if (!this.showOnlyBasicData) {
                            this.form.get('personAddressesControl')!.setValue(value.addresses);
                        }
                    }
                    else if (value.submittedByRole === SubmittedByRolesEnum.PersonalRepresentative) {
                        this.form.get('personControl')!.setValue(value.person);

                        if (!this.showOnlyBasicData) {
                            this.form.get('personAddressesControl')!.setValue(value.addresses);
                        }
                    }
                    else if (value.submittedByRole & SubmittedByRolesEnum.LegalRole) {
                        this.form.get('legalControl')!.setValue(value.legal);

                        if (!this.showOnlyBasicData) {
                            this.form.get('legalAddressesControl')!.setValue(value.addresses);
                        }
                    }
                }
            }
            else {
                this.form.reset();
            }
        });
    }

    public downloadedPersonData(person: PersonFullDataDTO): void {
        this.form.get('personControl')!.setValue(person.person);
        this.form.get('personAddressesControl')?.setValue(person.addresses);
    }

    public downloadedLegalData(legal: LegalFullDataDTO): void {
        this.form.get('legalControl')?.setValue(legal.legal);
        this.form.get('legalAddressesControl')?.setValue(legal.addresses);
    }

    protected buildForm(): AbstractControl {
        return new FormGroup({
            relationControl: new FormControl(),
            personControl: new FormControl(),
            legalControl: new FormControl(),
            personAddressesControl: new FormControl(),
            legalAddressesControl: new FormControl()
        });
    }

    protected getValue(): ApplicationSubmittedForDTO {
        const result: ApplicationSubmittedForDTO = new ApplicationSubmittedForDTO({
            submittedByRole: this.form.get('relationControl')!.value?.role ?? undefined,
            submittedByLetterOfAttorney: this.form.get('relationControl')!.value?.letterOfAttorney ?? undefined
        });

        if (result.submittedByRole !== undefined && result.submittedByRole !== null) {
            if (result.submittedByRole === SubmittedByRolesEnum.Personal && this.showPersonal) {
                result.person = this.form.get('personControl')!.value;
                if (!this.showOnlyBasicData) {
                    result.addresses = this.form.get('personAddressesControl')!.value ?? undefined;
                }
            }
            else if (result.submittedByRole === SubmittedByRolesEnum.PersonalRepresentative) {
                result.person = this.form.get('personControl')!.value;

                if (!this.showOnlyBasicData) {
                    result.addresses = this.form.get('personAddressesControl')!.value ?? undefined;
                }
            }
            else if (result.submittedByRole & SubmittedByRolesEnum.LegalRole) {
                result.legal = this.form.get('legalControl')!.value;

                if (!this.showOnlyBasicData) {
                    result.addresses = this.form.get('legalAddressesControl')!.value ?? undefined;
                }
            }

            if (this.showOnlyBasicData) {
                result.addresses = undefined;
            }
        }

        return result;
    }

    private setSubmittedByRoleSubscription(): void {
        this.form.get('relationControl')!.valueChanges.subscribe({
            next: (value: ApplicantRelationToRecipientDTO) => {
                if (value !== null && value !== undefined) {
                    this.role = value.role;

                    if (this.role !== null && this.role !== undefined && value.role !== null && value.role !== undefined) {
                        // упълномощен представител на физическо лице
                        if (this.role === SubmittedByRolesEnum.PersonalRepresentative) {
                            this.form.get('legalControl')!.clearValidators();
                            this.form.get('personControl')!.setValidators(Validators.required);

                            if (!this.showOnlyBasicData) {
                                this.form.get('legalAddressesControl')!.clearValidators();
                                this.form.get('personAddressesControl')!.setValidators(Validators.required);
                            }
                        }
                        // юридическо лице
                        else if (this.role & SubmittedByRolesEnum.LegalRole) {
                            this.form.get('personControl')!.clearValidators();
                            this.form.get('legalControl')!.setValidators(Validators.required);

                            if (!this.showOnlyBasicData) {
                                this.form.get('personAddressesControl')!.clearValidators();
                                this.form.get('legalAddressesControl')!.setValidators(Validators.required);
                            }
                        }
                        // в лично качество
                        else {
                            this.form.get('personControl')!.clearValidators();
                            this.form.get('legalControl')!.clearValidators();
                            this.form.get('personAddressesControl')!.clearValidators();
                            this.form.get('legalAddressesControl')!.clearValidators();
                        }
                    }

                    this.form.get('personControl')!.updateValueAndValidity();
                    this.form.get('legalControl')!.updateValueAndValidity();
                    this.form.get('personAddressesControl')!.updateValueAndValidity();
                    this.form.get('legalAddressesControl')!.updateValueAndValidity();
                }
            }
        });
    }
}