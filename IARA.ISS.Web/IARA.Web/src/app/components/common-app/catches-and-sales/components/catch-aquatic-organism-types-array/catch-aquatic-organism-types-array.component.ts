import { Component, Input, OnInit, Self } from '@angular/core';
import { AbstractControl, ControlValueAccessor, FormArray, FormControl, NgControl, ValidationErrors, Validator } from '@angular/forms';
import { forkJoin, Observable, Subscription } from 'rxjs';

import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';

import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { CatchRecordFishDTO } from '@app/models/generated/dtos/CatchRecordFishDTO';
import { FormControlDataLoader } from '@app/shared/utils/form-control-data-loader';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { SturgeonGendersEnum } from '@app/enums/sturgeon-genders.enum';
import { CommonNomenclatures } from '@app/services/common-app/common-nomenclatures.service';
import { ICatchesAndSalesService } from '@app/interfaces/common-app/catches-and-sales.interface';
import { CatchZoneNomenclatureDTO } from '@app/models/generated/dtos/CatchZoneNomenclatureDTO';
import { WaterTypesEnum } from '@app/enums/water-types.enum';
import { FishNomenclatureDTO } from '@app/models/generated/dtos/FishNomenclatureDTO';
import { CustomFormControl } from '@app/shared/utils/custom-form-control';


@Component({
    selector: 'catch-aquatic-organism-types-array',
    templateUrl: './catch-aquatic-organism-types-array.component.html'
})
export class CatchAquaticOrganismTypesArrayComponent extends CustomFormControl<CatchRecordFishDTO[]> implements OnInit {
    @Input()
    public service!: ICatchesAndSalesService;

    @Input()
    public waterType!: WaterTypesEnum;

    public isDisabled: boolean = false;
    public aquaticOrganisms: FishNomenclatureDTO[] = [];
    public sturgeonGenders: NomenclatureDTO<SturgeonGendersEnum>[] = [];
    public turbotSizeGroups: NomenclatureDTO<number>[] = [];
    public catchQuadrants: CatchZoneNomenclatureDTO[] = [];
    public fishSizes: NomenclatureDTO<number>[] = [];
    public catchTypes: NomenclatureDTO<number>[] = [];

    private translationService: FuseTranslationLoaderService;
    private readonly loader!: FormControlDataLoader;

    private commonNomenclaturesService: CommonNomenclatures;

    public constructor(
        @Self() ngControl: NgControl,
        translate: FuseTranslationLoaderService,
        commonNomenclaturesService: CommonNomenclatures
    ) {
        super(ngControl);

        this.translationService = translate;
        this.commonNomenclaturesService = commonNomenclaturesService;

        this.loader = new FormControlDataLoader(this.getNomenclatures.bind(this));
    }

    public ngOnInit(): void {
        this.initCustomFormControl();
        this.loader.load();
    }

    protected buildForm(): AbstractControl {
        return new FormArray([]);
    }

    public writeValue(values: CatchRecordFishDTO[]): void {
        this.loader.load(() => {
            this.fillForm(values);
        });
    }

    public addCatchAquaticOrganismTypeControl(): void {
        const catchAquaticOrganismType: CatchRecordFishDTO = new CatchRecordFishDTO({
            isActive: true
        });

        const control: FormControl = new FormControl(catchAquaticOrganismType);
        this.formArray.push(control);
        
        this.onChanged(this.getValue());
    }

    public deleteCatchAquaticOrganismTypeControl(catchControl: FormControl, index: number): void {
        this.formArray.removeAt(index);
    }

    protected getValue(): CatchRecordFishDTO[] {
        return this.formArray.value;
    }

    private fillForm(values: CatchRecordFishDTO[]): void {
        if (values === null || values === undefined || values.length === 0) {
            this.reset();
        }
        else {
            this.formArray.clear();
        }

        if (values) {
            this.fillControls(values);
            this.setDisabledState(this.isDisabled);
        }
    }

    private reset(): void {
        this.formArray.clear();
    }

    private fillControls(aquaticOrganisms: CatchRecordFishDTO[]): void {
        for (const aquaticOrganism of aquaticOrganisms) {
            const newControl: FormControl = new FormControl(aquaticOrganism);
            this.formArray.push(newControl);
            
            this.onChanged(this.getValue());
        }
    }

    private getNomenclatures(): Subscription {
        this.sturgeonGenders = [
            new NomenclatureDTO<SturgeonGendersEnum>({
                value: SturgeonGendersEnum.Male,
                displayName: this.translationService.getValue('catches-and-sales.ship-page-catch-sturgeon-gender-male'),
                isActive: true
            }),
            new NomenclatureDTO<SturgeonGendersEnum>({
                value: SturgeonGendersEnum.Female,
                displayName: this.translationService.getValue('catches-and-sales.ship-page-catch-sturgeon-gender-female'),
                isActive: true
            }),
        ];

        const observables: Observable<NomenclatureDTO<number>[]>[] = [];

        observables.push(NomenclatureStore.instance.getNomenclature(
            NomenclatureTypes.Fishes, this.commonNomenclaturesService.getFishTypes.bind(this.commonNomenclaturesService), false
        ));

        observables.push(NomenclatureStore.instance.getNomenclature(
            NomenclatureTypes.TurbotSizeGroups, this.service.getTurbotSizeGroups.bind(this.service), false
        ));

        observables.push(NomenclatureStore.instance.getNomenclature(
            NomenclatureTypes.CatchZones, this.commonNomenclaturesService.getCatchZones.bind(this.commonNomenclaturesService), false
        ));

        observables.push(NomenclatureStore.instance.getNomenclature(
            NomenclatureTypes.FishSizes, this.service.getFishSizes.bind(this.service), false
        ));

        observables.push(NomenclatureStore.instance.getNomenclature(
            NomenclatureTypes.CatchTypes, this.service.getCatchTypes.bind(this.service), false
        ));

        const subscription: Subscription = forkJoin(observables).subscribe({
            next: (nomenclatures: FishNomenclatureDTO[][] | NomenclatureDTO<number>[][]) => {
                this.aquaticOrganisms = nomenclatures[0] as FishNomenclatureDTO[];
                this.turbotSizeGroups = nomenclatures[1];
                this.catchQuadrants = nomenclatures[2];
                this.fishSizes = nomenclatures[3];
                this.catchTypes = nomenclatures[4];

                let isDanube: boolean = false;
                let isBlackSea: boolean = false;

                if (this.waterType === WaterTypesEnum.BLACK_SEA) {
                    isBlackSea = true;
                }
                else if (this.waterType === WaterTypesEnum.DANUBE) {
                    isDanube = true;
                }

                this.aquaticOrganisms = this.aquaticOrganisms.filter(x =>
                    this.waterType === WaterTypesEnum.Unknown
                    || (x.isBlackSea === isBlackSea && x.isDanube === isDanube)
                );

                this.loader.complete();
            }
        });

        return subscription;
    }
}