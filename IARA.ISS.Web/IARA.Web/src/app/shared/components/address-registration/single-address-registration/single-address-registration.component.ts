import { AfterViewInit, Component, Input, OnChanges, OnInit, Optional, Self, SimpleChanges } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, NgControl, ValidationErrors, Validators } from '@angular/forms';
import { forkJoin, Subscription } from 'rxjs';

import { AddressTypesEnum } from '@app/enums/address-types.enum';
import { AddressRegistrationDTO } from '@app/models/generated/dtos/AddressRegistrationDTO';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { MunicipalityNomenclatureExtendedDTO } from '@app/models/generated/dtos/MunicipalityNomenclatureExtendedDTO';
import { PopulatedAreaNomenclatureExtendedDTO } from '@app/models/generated/dtos/PopulatedAreaNomenclatureExtendedDTO';
import { CommonNomenclatures } from '@app/services/common-app/common-nomenclatures.service';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { TLValidators } from '@app/shared/utils/tl-validators';
import { TLError } from '@app/shared/components/input-controls/models/tl-error.model';
import { FormControlDataLoader } from '@app/shared/utils/form-control-data-loader';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { ValidityCheckerDirective } from '@app/shared/directives/validity-checker/validity-checker.directive';
import { NotifyingCustomFormControl } from '@app/shared/utils/notifying-custom-form-control';
import { NotifierDirective } from '@app/shared/directives/notifier/notifier.directive';

const BG_COUNTRY_CODE: string = 'BGR';

@Component({
    selector: 'single-address-registration',
    templateUrl: './single-address-registration.component.html'
})
export class SingleAddressRegistrationComponent extends NotifyingCustomFormControl<AddressRegistrationDTO> implements OnInit, AfterViewInit, OnChanges {
    @Input()
    public label!: string;

    @Input()
    public addressType!: AddressTypesEnum;

    @Input()
    public expectedResults!: AddressRegistrationDTO;

    @Input()
    public readonly: boolean = false;

    public countries: NomenclatureDTO<number>[] = [];
    public districts: NomenclatureDTO<number>[] = [];
    public municipalities: MunicipalityNomenclatureExtendedDTO[] = [];
    public populatedAreas: PopulatedAreaNomenclatureExtendedDTO[] = [];

    private allCountries: NomenclatureDTO<number>[] = [];
    private allDistricts: NomenclatureDTO<number>[] = [];
    private allMunicipalities: MunicipalityNomenclatureExtendedDTO[] = [];
    private allPopulatedAreas: PopulatedAreaNomenclatureExtendedDTO[] = [];

    private nomenclaturesService: CommonNomenclatures;

    private expectedAddress: AddressRegistrationDTO = new AddressRegistrationDTO();

    private readonly loader: FormControlDataLoader;

    public constructor(
        @Self() ngControl: NgControl,
        nomenclaturesService: CommonNomenclatures,
        @Optional() @Self() validityChecker: ValidityCheckerDirective,
        @Optional() @Self() notifier: NotifierDirective
    ) {
        super(ngControl, true, validityChecker, notifier);
        this.nomenclaturesService = nomenclaturesService;

        this.setFromGroupValidators();

        this.loader = new FormControlDataLoader(this.getNomenclatures.bind(this));
    }

    public ngOnInit(): void {
        this.initNotifyingCustomFormControl();
        this.loader.load();

        if (this.readonly) {
            this.form.controls.countryControl!.clearValidators();
            this.form.controls.streetControl!.clearValidators();
            this.form.updateValueAndValidity();
        }
    }

    public ngAfterViewInit(): void {
        this.form.get('countryControl')!.valueChanges.subscribe({
            next: (country: NomenclatureDTO<number>) => {
                this.updateCountryRelatedControlsValidators(country?.code);

                if (!this.isBGChosen()) {
                    this.form.get('districtControl')!.setValue(null);
                    this.form.get('municipalityControl')!.setValue(null);
                    this.form.get('populatedAreaControl')!.setValue(null);
                    this.form.get('districtControl')!.disable();
                    this.form.get('municipalityControl')!.disable()
                    this.form.get('populatedAreaControl')!.disable();
                }
                else {
                    if (this.isDisabled) {
                        this.form.get('districtControl')!.disable();
                        this.form.get('municipalityControl')!.disable()
                        this.form.get('populatedAreaControl')!.disable();
                    }
                }
            }
        });

        this.setupFilters();
    }

    public ngOnChanges(changes: SimpleChanges): void {
        const expectedResult: AddressRegistrationDTO | undefined = changes['expectedResults']?.currentValue;

        if (expectedResult !== null && expectedResult !== undefined) {
            this.expectedAddress = expectedResult;
            this.setFromGroupValidators();
        }
    }

    public writeValue(value: AddressRegistrationDTO): void {
        setTimeout(() => {
            if (value !== null && value !== undefined) {
                this.form.get('regionControl')!.setValue(value.region);
                this.form.get('postalCodeControl')!.setValue(value.postalCode);
                this.form.get('streetControl')!.setValue(value.street);
                this.form.get('streetNumControl')!.setValue(value.streetNum);
                this.form.get('blockNumControl')!.setValue(value.blockNum);
                this.form.get('entranceNumControl')!.setValue(value.entranceNum);
                this.form.get('floorNumControl')!.setValue(value.floorNum);
                this.form.get('apartmentNumControl')!.setValue(value.apartmentNum);

                this.loader.load(() => {
                    if (value.countryId !== null && value.countryId !== undefined) {
                        this.form.get('countryControl')!.setValue(this.countries.find(x => x.value === value.countryId));
                    }
                    else {
                        this.form.get('countryControl')!.setValue(this.countries.find(x => x.code === BG_COUNTRY_CODE && x.isActive));
                    }

                    this.form.get('districtControl')!.setValue(this.districts.find(x => x.value === value.districtId));
                    this.form.get('municipalityControl')!.setValue(this.municipalities.find(x => x.value === value.municipalityId));
                    this.form.get('populatedAreaControl')!.setValue(this.populatedAreas.find(x => x.value === value.populatedAreaId));

                    this.notify();
                });
            }
            else {
                this.form.reset();
            }
        });
    }

    public validate(control: AbstractControl): ValidationErrors | null {
        const errors: ValidationErrors = {};

        for (const key of Object.keys(this.form.controls)) {
            if (this.form.controls[key].errors !== null && this.form.controls[key].errors !== undefined) {
                for (const error in this.form.controls[key].errors) {
                    if (!['expectedValueNotMatching'].includes(error)) {
                        errors[error] = this.form.controls[key].errors![error];
                    }
                }
            }
        }

        return Object.keys(errors).length === 0 ? null : errors;
    }

    protected buildForm(): AbstractControl {
        return new FormGroup({
            countryControl: new FormControl(null),
            districtControl: new FormControl(null),
            municipalityControl: new FormControl(null),
            populatedAreaControl: new FormControl(null),
            regionControl: new FormControl(null),
            postalCodeControl: new FormControl(null),
            streetControl: new FormControl(null),
            streetNumControl: new FormControl(null),
            blockNumControl: new FormControl(null),
            entranceNumControl: new FormControl(null),
            floorNumControl: new FormControl(null),
            apartmentNumControl: new FormControl(null)
        });
    }

    protected getValue(): AddressRegistrationDTO {
        return new AddressRegistrationDTO({
            addressType: this.addressType,
            countryId: this.form.get('countryControl')?.value?.value ?? undefined,
            districtId: this.form.get('districtControl')?.value?.value ?? undefined,
            municipalityId: this.form.get('municipalityControl')?.value?.value ?? undefined,
            populatedAreaId: this.form.get('populatedAreaControl')?.value?.value ?? undefined,
            region: this.form.get('regionControl')?.value ?? undefined,
            postalCode: this.form.get('postalCodeControl')?.value ?? undefined,
            street: this.form.get('streetControl')?.value ?? undefined,
            streetNum: this.form.get('streetNumControl')?.value ?? undefined,
            blockNum: this.form.get('blockNumControl')?.value ?? undefined,
            entranceNum: this.form.get('entranceNumControl')?.value ?? undefined,
            floorNum: this.form.get('floorNumControl')?.value ?? undefined,
            apartmentNum: this.form.get('apartmentNumControl')?.value ?? undefined
        });
    }

    private setFromGroupValidators() {
        if (!this.readonly) {
            this.form.controls.countryControl.setValidators([
                Validators.required, TLValidators.expectedValueMatch(this.expectedAddress.countryName)
            ]);

            this.updateCountryRelatedControlsValidators(this.form.controls.countryControl.value?.code);

            this.form.controls.regionControl.setValidators([
                Validators.maxLength(200), TLValidators.expectedValueMatch(this.expectedAddress.region)
            ]);
            this.form.controls.postalCodeControl.setValidators([
                Validators.maxLength(10), TLValidators.expectedValueMatch(this.expectedAddress.postalCode)
            ]);
            this.form.controls.streetControl.setValidators([
                Validators.required, Validators.maxLength(200), TLValidators.expectedValueMatch(this.expectedAddress.street)
            ]);
            this.form.controls.streetNumControl.setValidators([
                Validators.maxLength(10), TLValidators.expectedValueMatch(this.expectedAddress.streetNum)
            ]);
            this.form.controls.blockNumControl.setValidators([
                Validators.maxLength(10), TLValidators.expectedValueMatch(this.expectedAddress.blockNum)
            ]);
            this.form.controls.entranceNumControl.setValidators([
                Validators.maxLength(10), TLValidators.expectedValueMatch(this.expectedAddress.entranceNum)
            ]);
            this.form.controls.floorNumControl.setValidators([
                Validators.maxLength(10), TLValidators.expectedValueMatch(this.expectedAddress.floorNum)
            ]);
            this.form.controls.apartmentNumControl.setValidators([
                Validators.maxLength(10), TLValidators.expectedValueMatch(this.expectedAddress.apartmentNum)
            ]);

            this.form.controls.countryControl!.markAsPending({ emitEvent: true });
            this.form.controls.streetControl!.markAsPending({ emitEvent: true });

            this.form.updateValueAndValidity();
        }

        if (this.isDisabled) {
            this.form.disable();
        }
    }

    private updateCountryRelatedControlsValidators(code: string | undefined): void {
        if (this.isBGChosen() && !this.readonly) {
            this.form.controls.districtControl!.setValidators([
                Validators.required, TLValidators.expectedValueMatch(this.expectedAddress.districtName)
            ]);
            this.form.controls.municipalityControl!.setValidators([
                Validators.required, TLValidators.expectedValueMatch(this.expectedAddress.municipalityName)
            ]);
            this.form.controls.populatedAreaControl!.setValidators([
                Validators.required, TLValidators.expectedValueMatch(this.expectedAddress.populatedAreaName)
            ]);
        }
        else {
            this.form.controls.districtControl.setValidators([
                TLValidators.expectedValueMatch(this.expectedAddress.districtName)
            ]);
            this.form.controls.municipalityControl.setValidators([
                TLValidators.expectedValueMatch(this.expectedAddress.municipalityName)
            ]);
            this.form.controls.populatedAreaControl.setValidators([
                TLValidators.expectedValueMatch(this.expectedAddress.populatedAreaName)
            ]);
        }

        this.form.controls.districtControl!.markAsPending({ emitEvent: true });
        this.form.controls.municipalityControl!.markAsPending({ emitEvent: true });
        this.form.controls.populatedAreaControl!.markAsPending({ emitEvent: true });
    }

    private getNomenclatures(): Subscription {
        const subscription: Subscription = forkJoin(
            NomenclatureStore.instance.getNomenclature<number>(
                NomenclatureTypes.Countries, this.nomenclaturesService.getCountries.bind(this.nomenclaturesService), false
            ),
            NomenclatureStore.instance.getNomenclature<number>(
                NomenclatureTypes.Districts, this.nomenclaturesService.getDistricts.bind(this.nomenclaturesService), false
            ),
            NomenclatureStore.instance.getNomenclature<number>(
                NomenclatureTypes.Municipalities, this.nomenclaturesService.getMunicipalities.bind(this.nomenclaturesService), false
            ),
            NomenclatureStore.instance.getNomenclature<number>(
                NomenclatureTypes.PopulatedAreas, this.nomenclaturesService.getPopulatedAreas.bind(this.nomenclaturesService), false
            )
        ).subscribe({
            next: (nomenclatures: NomenclatureDTO<number>[][]) => {
                this.allCountries = this.countries = nomenclatures[0];
                this.allDistricts = this.districts = nomenclatures[1];
                this.allMunicipalities = this.municipalities = nomenclatures[2];
                this.allPopulatedAreas = this.populatedAreas = nomenclatures[3];

                this.loader.complete();
            }
        });

        return subscription;
    }

    private setupFilters(): void {
        this.form.get('districtControl')?.valueChanges.subscribe({
            next: (district: NomenclatureDTO<number> | string | null) => {
                const municipality = this.form.get('municipalityControl')?.value;
                const populatedArea = this.form.get('populatedAreaControl')?.value;

                if (isNullOrString(district)) {
                    if (isNullOrString(populatedArea)) {
                        this.municipalities = this.allMunicipalities;
                    }
                    else {
                        this.municipalities = this.allMunicipalities.filter(x => x.value === populatedArea.municipalityId);
                    }

                    if (isNullOrString(municipality)) {
                        this.populatedAreas = this.allPopulatedAreas;
                    }
                    else {
                        this.populatedAreas = this.allPopulatedAreas.filter(x => x.municipalityId === municipality.value);
                    }
                }
                else {
                    this.municipalities = this.allMunicipalities.filter(x => x.districtId === district.value)
                    this.populatedAreas = this.allPopulatedAreas.filter(x => this.municipalities.some(y => y.value === x.municipalityId));
                }
                this.municipalities = this.municipalities.slice();
                this.populatedAreas = this.populatedAreas.slice();

                if (!isNullOrString(municipality) && this.municipalities.find(x => x.value === municipality.value) === undefined) {
                    this.form.get('municipalityControl')!.setValue(null);
                }
                if (!isNullOrString(populatedArea) && this.populatedAreas.find(x => x.value === populatedArea.value) === undefined) {
                    this.form.get('populatedAreaControl')!.setValue(null);
                }
            }
        });

        this.form.get('municipalityControl')?.valueChanges.subscribe({
            next: (municipality: MunicipalityNomenclatureExtendedDTO | string | null) => {
                const district = this.form.get('districtControl')?.value;
                const populatedArea = this.form.get('populatedAreaControl')?.value;

                if (isNullOrString(municipality)) {
                    if (isNullOrString(district)) {
                        this.populatedAreas = this.allPopulatedAreas;
                    }
                    else {
                        this.populatedAreas = this.allPopulatedAreas.filter(x => this.municipalities.some(y => y.value === x.municipalityId));
                    }

                    if (isNullOrString(populatedArea)) {
                        this.districts = this.allDistricts;
                    }
                    else {
                        const mun = this.municipalities.find(x => x.value === populatedArea.municipalityId);
                        if (mun) {
                            this.districts = this.allDistricts.filter(x => x.value === mun.districtId);
                        }
                    }
                }
                else {
                    this.districts = this.allDistricts.filter(x => x.value === municipality.districtId);
                    this.populatedAreas = this.allPopulatedAreas.filter(x => x.municipalityId === municipality.value);
                }
                this.districts = this.districts.slice();
                this.populatedAreas = this.populatedAreas.slice();

                if (!isNullOrString(district) && this.districts.find(x => x.value === district.value) === undefined) {
                    this.form.get('districtControl')!.setValue(null);
                }
                if (!isNullOrString(populatedArea) && this.populatedAreas.find(x => x.value === populatedArea.value) === undefined) {
                    this.form.get('populatedAreaControl')!.setValue(null);
                }
            }
        });

        this.form.get('populatedAreaControl')?.valueChanges.subscribe({
            next: (populatedArea: PopulatedAreaNomenclatureExtendedDTO | string | null) => {
                const district = this.form.get('districtControl')?.value;
                const municipality = this.form.get('municipalityControl')?.value;

                if (isNullOrString(populatedArea)) {
                    if (isNullOrString(district)) {
                        this.municipalities = this.allMunicipalities;
                    }
                    else {
                        this.municipalities = this.allMunicipalities.filter(x => x.districtId === district.value);
                    }

                    if (isNullOrString(municipality)) {
                        this.districts = this.allDistricts;
                    }
                    else {
                        this.districts = this.allDistricts.filter(x => x.value === municipality.districtId);;
                    }
                }
                else {
                    this.municipalities = this.allMunicipalities.filter(x => x.value === populatedArea.municipalityId)
                    this.districts = this.allDistricts.filter(x => this.municipalities.some(y => y.districtId === x.value));
                }
                this.districts = this.districts.slice();
                this.municipalities = this.municipalities.slice();

                if (!isNullOrString(district) && this.districts.find(x => x.value === district.value) === undefined) {
                    this.form.get('districtControl')!.setValue(null);
                }
                if (!isNullOrString(municipality) && this.municipalities.find(x => x.value === municipality.value) === undefined) {
                    this.form.get('municipalityControl')!.setValue(null);
                }
            }
        });
    }

    private isBGChosen(): boolean {
        return this.form.get('countryControl')?.value?.code === BG_COUNTRY_CODE;
    }

    public getControlErrorLabelText(controlName: string, errorValue: unknown, errorCode: string): TLError | undefined {
        return CommonUtils.getControlErrorLabelTextForRegixExpectedValueValidator(controlName, errorValue, errorCode);
    }
}

function isNullOrString(value: unknown): value is null | undefined | string {
    return value === null || value === undefined || typeof value === 'string';
}
