import { AfterViewInit, Component, DoCheck, Input, OnInit, Self } from '@angular/core';
import { AbstractControl, ControlValueAccessor, FormControl, FormGroup, NgControl, ValidationErrors, Validator, Validators } from '@angular/forms';
import { forkJoin, Subscription } from 'rxjs';
import { FuseTranslationLoaderService } from '../../../../@fuse/services/translation-loader.service';
import { NomenclatureTypes } from '../../../enums/nomenclature.types';
import { NomenclatureDTO } from '../../../models/generated/dtos/GenericNomenclatureDTO';
import { VesselDTO } from '../../../models/generated/dtos/VesselDTO';
import { CommonNomenclatures } from '../../../services/common-app/common-nomenclatures.service';
import { FormControlDataLoader } from '../../utils/form-control-data-loader';
import { NomenclatureStore } from '../../utils/nomenclatures.store';

@Component({
    selector: 'vessel-simple',
    templateUrl: './vessel-simple.component.html'
})
export class VesselSimpleComponent implements OnInit, AfterViewInit, DoCheck, ControlValueAccessor, Validator {
    public form!: FormGroup;

    public countries: NomenclatureDTO<number>[] = [];
    public shipTypes: NomenclatureDTO<number>[] = [];

    private ngControl: NgControl;
    private onChanged: (value: VesselDTO | undefined) => void;
    private onTouched: (value: VesselDTO | undefined) => void;

    private translate: FuseTranslationLoaderService;
    private nomenclatures: CommonNomenclatures;

    private unregisteredVesselId: number | undefined;
    private isActive: boolean | undefined;

    private readonly loader: FormControlDataLoader;

    public constructor(
        @Self() ngControl: NgControl,
        translate: FuseTranslationLoaderService,
        nomenclatures: CommonNomenclatures
    ) {
        this.ngControl = ngControl;
        this.translate = translate;
        this.nomenclatures = nomenclatures;

        this.ngControl.valueAccessor = this;

        this.onChanged = (value: VesselDTO | undefined) => { return value; };
        this.onTouched = (value: VesselDTO | undefined) => { return value; };

        this.buildForm();

        this.loader = new FormControlDataLoader(this.getNomenclatures.bind(this));
    }

    public async ngOnInit(): Promise<void> {
        this.loader.load();

        if (this.ngControl.control) {
            this.ngControl.control.validator = this.validate.bind(this);
        }
    }

    public ngAfterViewInit(): void {
        this.form.valueChanges.subscribe({
            next: () => {
                this.onChanged(this.getValue());
            }
        });
    }

    public ngDoCheck(): void {
        if (this.ngControl?.control?.touched) {
            this.form.markAllAsTouched();
        }
    }

    public writeValue(obj: VesselDTO | undefined): void {
        if (obj !== undefined && obj !== null) {
            this.unregisteredVesselId = obj.unregisteredVesselId;
            this.isActive = obj.isActive;

            this.form.get('vesselNameControl')!.setValue(obj.name);
            this.form.get('vesselExtMarkControl')!.setValue(obj.externalMark);
            this.form.get('vesselCfrControl')!.setValue(obj.cfr);
            this.form.get('vesselImoControl')!.setValue(obj.uvi);
            this.form.get('vesselCallsignControl')!.setValue(obj.regularCallsign);
            this.form.get('vesselMmsiControl')!.setValue(obj.mmsi);

            this.loader.load(() => {
                this.form.get('vesselFlagControl')!.setValue(this.countries.find(x => x.value === obj.flagCountryId));
                this.form.get('vesselTypeControl')!.setValue(this.shipTypes.find(x => x.value === obj.vesselTypeId));
            });
        }
        else {
            this.unregisteredVesselId = undefined;
            this.isActive = undefined;

            this.form.get('vesselNameControl')!.setValue(undefined);
            this.form.get('vesselExtMarkControl')!.setValue(undefined);
            this.form.get('vesselCfrControl')!.setValue(undefined);
            this.form.get('vesselImoControl')!.setValue(undefined);
            this.form.get('vesselCallsignControl')!.setValue(undefined);
            this.form.get('vesselMmsiControl')!.setValue(undefined);
            this.form.get('vesselFlagControl')!.setValue(undefined);
            this.form.get('vesselTypeControl')!.setValue(undefined);
        }
    }

    public registerOnChange(fn: (value: VesselDTO | undefined) => void): void {
        this.onChanged = fn;
    }

    public registerOnTouched(fn: (value: VesselDTO | undefined) => void): void {
        this.onTouched = fn;
    }

    public validate(control: AbstractControl): ValidationErrors | null {
        const errors: ValidationErrors = {};
        for (const key of Object.keys(this.form.controls)) {
            const controlErrors: ValidationErrors | null = this.form.controls[key].errors;
            if (controlErrors !== null) {
                errors[key] = controlErrors;
            }
        }
        return Object.keys(errors).length === 0 ? null : errors;
    }

    private buildForm(): void {
        this.form = new FormGroup({
            vesselNameControl: new FormControl(null, Validators.required),
            vesselExtMarkControl: new FormControl(null, Validators.required),
            vesselFlagControl: new FormControl(null, Validators.required),
            vesselCfrControl: new FormControl(null, Validators.required),
            vesselImoControl: new FormControl(null, Validators.required),
            vesselCallsignControl: new FormControl(null, Validators.required),
            vesselTypeControl: new FormControl(null, Validators.required),
            vesselMmsiControl: new FormControl(null, Validators.required),
        });
    }


    private getValue(): VesselDTO {
        return new VesselDTO({
            shipId: undefined,
            unregisteredVesselId: this.unregisteredVesselId,
            isRegistered: false,
            name: this.form.controls.vesselNameControl.value,
            externalMark: this.form.controls.vesselExtMarkControl.value,
            cfr: this.form.controls.vesselCfrControl.value,
            uvi: this.form.controls.vesselImoControl.value,
            regularCallsign: this.form.controls.vesselCallsignControl.value,
            mmsi: this.form.controls.vesselMmsiControl.value,
            flagCountryId: this.form.controls.vesselFlagControl.value?.value,
            patrolVehicleTypeId: undefined,
            vesselTypeId: this.form.controls.vesselTypeControl.value?.value,
            isActive: this.isActive ?? true
        });
    }

    private getNomenclatures(): Subscription {
        const subscription: Subscription = forkJoin(
            NomenclatureStore.instance.getNomenclature(
                NomenclatureTypes.Countries, this.nomenclatures.getCountries.bind(this.nomenclatures), false
            ),
            NomenclatureStore.instance.getNomenclature(
                NomenclatureTypes.VesselTypes, this.nomenclatures.getVesselTypes.bind(this.nomenclatures), false
            )
        ).subscribe({
            next: (nomenclatures: NomenclatureDTO<number>[][]) => {
                this.countries = nomenclatures[0];
                this.shipTypes = nomenclatures[1];

                this.loader.complete();
            }
        });

        return subscription;
    }
}
