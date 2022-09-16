import { Component, Input, OnInit, Self } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, NgControl, Validators } from '@angular/forms';
import { Subscription } from 'rxjs';

import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { PageCodeEnum } from '@app/enums/page-code.enum';
import { SubmittedByRolesEnum } from '@app/enums/submitted-by-roles.enum';
import { ApplicantRelationToRecipientDTO } from '@app/models/generated/dtos/ApplicantRelationToRecipientDTO';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { SubmittedByRoleNomenclatureDTO } from '@app/models/generated/dtos/SubmittedByRoleNomenclatureDTO';
import { CommonNomenclatures } from '@app/services/common-app/common-nomenclatures.service';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { FormControlDataLoader } from '@app/shared/utils/form-control-data-loader';
import { CustomFormControl } from '@app/shared/utils/custom-form-control';

@Component({
    selector: 'applicant-relation-to-recipient',
    templateUrl: './applicant-relation-to-recipient.component.html'
})
export class ApplicantRelationToRecipientComponent extends CustomFormControl<ApplicantRelationToRecipientDTO> implements OnInit {
    @Input()
    public pageCode!: PageCodeEnum;

    public hasLetterOfAttorney: boolean = false;
    public submittedByRoles: SubmittedByRoleNomenclatureDTO[] = [];

    private allSubmittedByRoles!: SubmittedByRoleNomenclatureDTO[];
    private nomenclatures: CommonNomenclatures;

    private readonly loader: FormControlDataLoader;

    public constructor(@Self() ngControl: NgControl, nomenclatures: CommonNomenclatures) {
        super(ngControl);
        this.nomenclatures = nomenclatures;

        this.setRoleControlSubscriber();

        this.loader = new FormControlDataLoader(this.getSubmittedByRoles.bind(this));
    }

    public ngOnInit(): void {
        this.initCustomFormControl();
        this.loader.load();
    }

    public writeValue(value: ApplicantRelationToRecipientDTO): void {
        if (value !== null && value !== undefined) {
            this.loader.load(() => {
                const role: SubmittedByRoleNomenclatureDTO | undefined
                    = this.submittedByRoles.find(x => x.code === SubmittedByRolesEnum[value.role!]);

                if (role !== undefined) {
                    this.form.get('roleControl')!.setValue(role);
                    this.form.get('letterOfAttorneyControl')!.setValue(value.letterOfAttorney ?? undefined);
                }
            });
        }
        else {
            this.form.reset();
        }
    }

    protected buildForm(): AbstractControl {
        return new FormGroup({
            roleControl: new FormControl(null, Validators.required),
            letterOfAttorneyControl: new FormControl()
        });
    }

    protected getValue(): ApplicantRelationToRecipientDTO {
        const role: NomenclatureDTO<number> = this.form.get('roleControl')?.value;
        const roleCode = role ? SubmittedByRolesEnum[role.code as keyof typeof SubmittedByRolesEnum] : undefined;

        return new ApplicantRelationToRecipientDTO({
            role: roleCode,
            letterOfAttorney: this.form.get('letterOfAttorneyControl')?.value ?? undefined
        });
    }

    private setRoleControlSubscriber(): void {
        this.form.get('roleControl')!.valueChanges.subscribe({
            next: (role: SubmittedByRoleNomenclatureDTO | undefined) => {
                this.hasLetterOfAttorney = role?.hasLetterOfAttorney ?? false;

                if (role !== null && role !== undefined) {
                    if (role.hasLetterOfAttorney) {
                        this.form.get('letterOfAttorneyControl')!.setValidators(Validators.required);
                    }
                    else {
                        this.form.get('letterOfAttorneyControl')!.clearValidators();
                    }
                }
                else {
                    this.form.get('letterOfAttorneyControl')!.clearValidators();
                }

                this.form.get('letterOfAttorneyControl')!.updateValueAndValidity({ emitEvent: false });
            }
        });
    }

    private getSubmittedByRoles(): Subscription {
        return NomenclatureStore.instance.getNomenclature<number>(
            NomenclatureTypes.SubmittedByRoles, this.nomenclatures.getSubmittedByRoles.bind(this.nomenclatures), false
        ).subscribe({
            next: (roles: SubmittedByRoleNomenclatureDTO[]) => {
                if (!this.allSubmittedByRoles || this.allSubmittedByRoles.length === 0) {
                    this.allSubmittedByRoles = roles;
                    this.submittedByRoles = this.allSubmittedByRoles.slice();
                }

                this.loader.complete();
                this.filterSubmittedByRoles();
            }
        });
    }

    private filterSubmittedByRoles(): void {
        if (this.pageCode !== null && this.pageCode !== undefined) {
            const currentRole: NomenclatureDTO<number> | undefined = this.form.get('roleControl')?.value;

            this.submittedByRoles = this.allSubmittedByRoles.filter(x => x.applicationPageCode === this.pageCode);
            if (currentRole !== undefined && currentRole !== null) {
                this.form.get('roleControl')?.setValue(this.submittedByRoles.find(x => x.value === currentRole.value));
            }
        }
    }
}