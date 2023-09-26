import { Component, Input, OnInit, Self } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, NgControl, Validators } from '@angular/forms';
import { InspectedPersonTypeEnum } from '@app/enums/inspected-person-type.enum';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { InspectionSubjectPersonnelDTO } from '@app/models/generated/dtos/InspectionSubjectPersonnelDTO';
import { RegixLegalDataDTO } from '@app/models/generated/dtos/RegixLegalDataDTO';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { CustomFormControl } from '@app/shared/utils/custom-form-control';
import { InspectionUtils } from '@app/shared/utils/inspection.utils';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { LegalFullDataDTO } from '@app/models/generated/dtos/LegalFullDataDTO';

@Component({
    selector: 'inspected-legal',
    templateUrl: './inspected-legal.component.html',
})
export class InspectedLegalComponent extends CustomFormControl<InspectionSubjectPersonnelDTO | undefined> implements OnInit {
    @Input()
    public title!: string;

    @Input()
    public legalType!: InspectedPersonTypeEnum;

    @Input()
    public countries: NomenclatureDTO<number>[] = [];

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
            this.form.get('legalControl')!.setValue(
                new RegixLegalDataDTO({
                    eik: value.eik,
                    name: value.firstName,
                })
            );

            this.form.get('addressControl')!.setValue(
                InspectionUtils.buildAddress(value.registeredAddress, this.translate) ?? value.address
            );
            this.form.get('countryControl')!.setValue(this.countries.find(f => f.value === value.citizenshipId));
        }
        else {
            this.form.get('countryControl')!.setValue(this.countries.find(f => f.code === CommonUtils.COUNTRIES_BG));
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
            legalControl: new FormControl(),
            addressControl: new FormControl(undefined, Validators.maxLength(4000)),
            countryControl: new FormControl(undefined)
        });

        return form;
    }

    protected getValue(): InspectionSubjectPersonnelDTO | undefined {
        const person: RegixLegalDataDTO = this.form.get('legalControl')!.value;

        if (person === null || person === undefined) {
            return undefined;
        }

        return new InspectionSubjectPersonnelDTO({
            isRegistered: false,
            address: this.form.get('addressControl')!.value,
            citizenshipId: this.form.get('countryControl')!.value?.value,
            eik: person.eik,
            isLegal: true,
            firstName: person.name,
            type: this.legalType,
        });
    }
}
