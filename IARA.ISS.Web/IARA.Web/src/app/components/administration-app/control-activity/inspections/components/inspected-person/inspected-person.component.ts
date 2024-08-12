import { Component, EventEmitter, Input, OnInit, Output, Self } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, NgControl, Validators } from '@angular/forms';

import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { CustomFormControl } from '@app/shared/utils/custom-form-control';
import { InspectionSubjectPersonnelDTO } from '@app/models/generated/dtos/InspectionSubjectPersonnelDTO';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { RegixPersonDataDTO } from '@app/models/generated/dtos/RegixPersonDataDTO';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { InspectionUtils } from '@app/shared/utils/inspection.utils';
import { InspectedPersonTypeEnum } from '@app/enums/inspected-person-type.enum';
import { PersonFullDataDTO } from '@app/models/generated/dtos/PersonFullDataDTO';
import { EgnLncDTO } from '@app/models/generated/dtos/EgnLncDTO';

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

    @Input()
    public isArray: boolean = false;

    @Input()
    public orderNum: number | undefined;

    @Output()
    public deletePanelBtnClicked: EventEmitter<void> = new EventEmitter<void>();

    @Output()
    public downloadPersonDataBtnClicked: EventEmitter<EgnLncDTO> = new EventEmitter<EgnLncDTO>();

    private readonly translate: FuseTranslationLoaderService;

    private id: number | undefined;

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
            this.id = value.id;

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

        this.downloadPersonDataBtnClicked.emit(person.person?.egnLnc);
    }

    public deletePanel(): void {
        this.deletePanelBtnClicked.emit();
    }

    protected buildForm(): AbstractControl {
        const form = new FormGroup({
            personControl: new FormControl(),
            addressControl: new FormControl(undefined, Validators.maxLength(4000)),
            countryControl: new FormControl(undefined)
        });

        return form;
    }

    protected getValue(): InspectionSubjectPersonnelDTO | undefined {
        const person: RegixPersonDataDTO = this.form.get('personControl')!.value;

        if (person === null || person === undefined) {
            return undefined;
        }

        return new InspectionSubjectPersonnelDTO({
            id: this.id,
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
}