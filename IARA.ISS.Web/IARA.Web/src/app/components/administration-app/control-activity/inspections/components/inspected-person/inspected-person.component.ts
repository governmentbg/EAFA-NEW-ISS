import { Component, Input, OnInit, Self } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, NgControl } from '@angular/forms';

import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { CustomFormControl } from '@app/shared/utils/custom-form-control';
import { InspectionSubjectPersonnelDTO } from '@app/models/generated/dtos/InspectionSubjectPersonnelDTO';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { RegixPersonDataDTO } from '@app/models/generated/dtos/RegixPersonDataDTO';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { InspectionUtils } from '@app/shared/utils/inspection.utils';
import { InspectedPersonTypeEnum } from '@app/enums/inspected-person-type.enum';
import { PersonFullDataDTO } from '@app/models/generated/dtos/PersonFullDataDTO';

@Component({
    selector: 'inspected-person',
    templateUrl: './inspected-person.component.html'
})
export class InspectedPersonComponent extends CustomFormControl<InspectionSubjectPersonnelDTO | undefined> implements OnInit {

    @Input()
    public title!: string;

    @Input()
    public personType!: InspectedPersonTypeEnum;

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
            this.form.get('personControl')!.setValue(
                new RegixPersonDataDTO({
                    egnLnc: value.egnLnc,
                    firstName: value.firstName,
                    middleName: value.middleName,
                    lastName: value.lastName,
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

    public downloadedPersonData(person: PersonFullDataDTO): void {
        this.form.get('personControl')!.setValue(person.person);
    }

    protected buildForm(): AbstractControl {
        const form = new FormGroup({
            personControl: new FormControl(),
            addressControl: new FormControl(undefined),
            countryControl: new FormControl(undefined),
        });

        return form;
    }

    protected getValue(): InspectionSubjectPersonnelDTO | undefined {
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
}