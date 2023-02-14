import { AfterViewInit, Component, EventEmitter, Input, OnChanges, OnInit, Output, Self, SimpleChanges, ViewChild } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, NgControl, ValidationErrors, Validators } from '@angular/forms';
import { BehaviorSubject, forkJoin, Subscription } from 'rxjs';

import { FillDef, MapOptions, SimplePolygonStyleDef, StrokeDef, TLMapViewerComponent } from '@tl/tl-angular-map';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { CatchRecordFishDTO } from '@app/models/generated/dtos/CatchRecordFishDTO';
import { TLValidators } from '@app/shared/utils/tl-validators';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { FishCodesEnum } from '@app/enums/fish-codes.enum';
import { FishNomenclatureDTO } from '@app/models/generated/dtos/FishNomenclatureDTO';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { TLPopoverComponent } from '@app/shared/components/tl-popover/tl-popover.component';
import { CatchZoneNomenclatureDTO } from '@app/models/generated/dtos/CatchZoneNomenclatureDTO';
import { FishFamilyTypesEnum } from '@app/enums/fish-family-types.enum';
import { SturgeonGendersEnum } from '@app/enums/sturgeon-genders.enum';
import { FormControlDataLoader } from '@app/shared/utils/form-control-data-loader';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { ICatchesAndSalesService } from '@app/interfaces/common-app/catches-and-sales.interface';
import { ValidityCheckerGroupDirective } from '@app/shared/directives/validity-checker/validity-checker-group.directive';
import { CustomFormControl } from '@app/shared/utils/custom-form-control';
import { CatchTypeCodesEnum } from '@app/enums/catch-type-codes.enum';
import { CatchSizeCodesEnum } from '@app/enums/catch-size-codes.enum';
import { WaterTypesEnum } from '@app/enums/water-types.enum';
import { ShipLogBookPageDataService } from '../ship-log-book/services/ship-log-book-page-data.service';

const DEFAULT_CATCH_TYPE_CODE = CatchTypeCodesEnum.TAKEN_ONBOARD;
const DEFAULT_CATCH_SIZE_CODE = CatchSizeCodesEnum.LSC;

@Component({
    selector: 'catch-aquatic-organism-type',
    templateUrl: './catch-aquatic-organism-type.component.html',
    styleUrls: ['./catch-aquatic-organism-type.component.scss']
})
export class CatchAquaticOrganismTypeComponent extends CustomFormControl<CatchRecordFishDTO> implements OnInit, AfterViewInit, OnChanges {
    @Input()
    public service!: ICatchesAndSalesService;

    @Input()
    public shipLogBookPageDataService!: ShipLogBookPageDataService;

    @Input()
    public aquaticOrganisms: NomenclatureDTO<number>[] = [];

    @Input()
    public sturgeonGenders: NomenclatureDTO<SturgeonGendersEnum>[] = [];

    @Input()
    public turbotSizeGroups: NomenclatureDTO<number>[] = [];

    @Input()
    public catchQuadrants: CatchZoneNomenclatureDTO[] = [];

    @Input()
    public fishSizes: NomenclatureDTO<number>[] = [];

    @Input()
    public catchTypes: NomenclatureDTO<number>[] = [];

    @Input()
    public waterType!: WaterTypesEnum;

    @Output()
    public deletePanelBtnClicked: EventEmitter<void> = new EventEmitter<void>();

    public expansionPanelTitle: string = '';
    public defaultExpansionPanelTitle: string = '';


    public showTurbotControls: boolean = false;
    public showStrugeonControls: boolean = false;

    public isMapPopoverOpened: boolean = false;
    public mapOptions: MapOptions | undefined;

    @ViewChild(TLMapViewerComponent)
    private mapViewer!: TLMapViewerComponent;

    @ViewChild(TLPopoverComponent)
    private mapPopover!: TLPopoverComponent;

    private model: CatchRecordFishDTO | undefined;

    private temporarySelectedGridSector: CatchZoneNomenclatureDTO | undefined;

    private readonly translationService: FuseTranslationLoaderService;
    private readonly hasShipLogBookPageDataServiceSubject: BehaviorSubject<boolean>;

    private hasShipLogBookPageDataServiceSubscription: Subscription | undefined;

    public constructor(
        @Self() ngControl: NgControl,
        translate: FuseTranslationLoaderService
    ) {
        super(ngControl, false);

        this.translationService = translate;
        this.hasShipLogBookPageDataServiceSubject = new BehaviorSubject<boolean>(false);

        this.defaultExpansionPanelTitle = this.translationService.getValue('catches-and-sales.ship-page-catch-single-catch-title');

        this.mapOptions = new MapOptions();
        this.mapOptions.showGridLayer = true;
        this.mapOptions.gridLayerStyle = this.createCustomGridLayerStyle();
        this.mapOptions.selectGridLayerStyle = this.createCustomSelectGridLayerStyle();

        this.form.get('thirdCountryCatchZoneControl')!.valueChanges.subscribe({
            next: (value: string | undefined) => {
                this.thirdCountryCatchZoneControlValueChanged(value);
            }
        });

        this.form.get('isContinentalCatchControl')!.valueChanges.subscribe({
            next: (value: boolean | undefined) => {
                this.setCatchQuadrantControlValidators();
            }
        });

        this.form.get('catchQuadrantControl')!.valueChanges.subscribe({
            next: (quadrant: CatchZoneNomenclatureDTO | string | null | undefined) => {
                if (quadrant !== null && quadrant !== undefined && CommonUtils.isNomenclature<number>(quadrant)) {
                    this.form.get('catchZoneControl')!.setValue(quadrant.zone?.toString());
                }
                else {
                    this.form.get('catchZoneControl')!.reset();
                }
            }
        });
    }

    public ngOnInit(): void {
        this.initCustomFormControl();

        this.form.valueChanges.subscribe({
            next: () => {
                const value: CatchRecordFishDTO = this.getValue();
                this.onChanged(value);
                this.setExpansionPanelTitle(value);
            }
        });
    }

    public ngOnChanges(changes: SimpleChanges): void {
        if ('waterType' in changes) {
            this.setCatchQuadrantControlValidators();
        }

        if ('shipLogBookPageDataService' in changes) {
            if (this.shipLogBookPageDataService !== null && this.shipLogBookPageDataService !== undefined) {
                this.hasShipLogBookPageDataServiceSubject.next(true);
            }
            else {
                this.hasShipLogBookPageDataServiceSubject.next(false);
            }
        }
    }

    public ngAfterViewInit(): void {
        this.form.get('aquaticOrganismControl')!.valueChanges.subscribe({
            next: (value: FishNomenclatureDTO | undefined) => {
                this.setTurbotAndSturgeonFlagsAndValidators(value);
            }
        });
    }

    public writeValue(value: CatchRecordFishDTO): void {
        this.model = value;

        if (value !== null && value !== undefined) {
            this.fillForm();
        }
        else {
            this.hasShipLogBookPageDataServiceSubscription?.unsubscribe();

            this.hasShipLogBookPageDataServiceSubscription = this.hasShipLogBookPageDataServiceSubject.subscribe({
                next: (hasShipLogBookPageDataService: boolean) => {
                    if (hasShipLogBookPageDataService) {
                        this.form.reset(new CatchRecordFishDTO({
                            id: this.shipLogBookPageDataService.nextNewCatchRecordId,
                            isActive: true,
                            unloadedQuantityKg: 0,
                            unloadedInOtherTripQuantityKg: 0
                        }));
                    }
                }
            });
        }

        setTimeout(() => {
            this.onChanged(this.getValue());
        });
    }

    public deletePanel(): void {
        this.deletePanelBtnClicked.emit();
    }

    public onPopoverToggled(isOpened: boolean): void {
        this.isMapPopoverOpened = isOpened;

        setTimeout(() => {
            if (this.isMapPopoverOpened === true) {
                this.mapViewer.selectedGridSectorsChangeEvent.subscribe({
                    next: (selectedGridSectors: string[] | undefined) => {
                        if (!CommonUtils.isNullOrEmpty(selectedGridSectors)) {
                            this.temporarySelectedGridSector = this.catchQuadrants.find(x => x.code === selectedGridSectors![0])!;
                        }
                    }
                });

                const quadrant: string | null | undefined = this.form.get('catchQuadrantControl')!.value?.code;
                if (quadrant !== null && quadrant !== undefined) {
                    this.mapViewer.gridLayerIsRenderedEvent.subscribe({
                        next: (isMapRendered: boolean) => {
                            if (isMapRendered === true) {
                                setTimeout(() => {
                                    this.mapViewer.selectGridSectors([quadrant]);
                                }, 1000); // необходимо, защото иначе не се появява на картата квадрантът - ако няма забавяне
                            }
                        }
                    });
                }
            }
        });
    }

    public onQuadrantChosenBtnClicked(): void {
        this.form.get('catchQuadrantControl')!.setValue(this.temporarySelectedGridSector);
        this.form.get('catchZoneControl')!.setValue(this.temporarySelectedGridSector?.zone?.toString());
        this.mapPopover.closePopover(true);
    }

    public onMapPopoverCancelBtnClicked(): void {
        this.temporarySelectedGridSector = undefined;
        this.mapViewer.selectGridSectors([]);
        this.mapPopover.closePopover(true);
    }

    private fillForm(): void {
        if (this.model !== undefined && this.model !== null) {
            if (this.model.fishId !== null && this.model.fishId !== undefined) {
                const aquaticOrganism: FishNomenclatureDTO = this.aquaticOrganisms.find(x => x.value === this.model!.fishId)!;
                this.model.fishName = aquaticOrganism.displayName;
                this.form.get('aquaticOrganismControl')!.setValue(aquaticOrganism);
                this.setTurbotAndSturgeonFlagsAndValidators(aquaticOrganism);
            }
            else {
                this.setTurbotAndSturgeonFlagsAndValidators(undefined);
            }

            this.form.get('quantityKgControl')!.setValue(this.model.quantityKg);

            if (this.model.catchTypeId !== null && this.model.catchTypeId !== undefined) {
                this.form.get('catchTypeControl')!.setValue(this.catchTypes.find(x => x.value === this.model!.catchTypeId));
            }
            else {
                const defaultCatchType: NomenclatureDTO<number> = this.catchTypes.find(x => x.code === CatchTypeCodesEnum[DEFAULT_CATCH_TYPE_CODE] && x.isActive)!;
                this.form.get('catchTypeControl')!.setValue(defaultCatchType);
            }

            if (this.model.catchSizeId !== null && this.model.catchSizeId !== undefined) {
                this.form.get('catchSizeControl')!.setValue(this.fishSizes.find(x => x.value === this.model!.catchSizeId));
            }
            else {
                const defaultCatchSize: NomenclatureDTO<number> = this.fishSizes.find(x => x.code === CatchSizeCodesEnum[DEFAULT_CATCH_SIZE_CODE])!;
                this.form.get('catchSizeControl')!.setValue(defaultCatchSize);
            }

            if (this.model.sturgeonGender !== null && this.model.sturgeonGender !== undefined) {
                const sturgeonGender: NomenclatureDTO<SturgeonGendersEnum> = this.sturgeonGenders.find(x => x.value === this.model!.sturgeonGender)!;
                this.form.get('sturgeonGenderControl')!.setValue(sturgeonGender);
            }

            this.form.get('sturgeonSizeControl')!.setValue(this.model.sturgeonSize);

            this.form.get('turbotCountControl')!.setValue(this.model.turbotCount);
            if (this.model.turbotSizeGroupId !== undefined) {
                const turbotSizeGroup: NomenclatureDTO<number> = this.turbotSizeGroups.find(x => x.value === this.model!.turbotSizeGroupId)!;
                this.form.get('turbotSizeGroupControl')!.setValue(turbotSizeGroup);
            }

            if (this.model.catchQuadrantId !== null && this.model.catchQuadrantId !== undefined) {
                const quadrant: CatchZoneNomenclatureDTO = this.catchQuadrants.find(x => x.value === this.model!.catchQuadrantId)!;
                this.form.get('catchQuadrantControl')!.setValue(quadrant);
                this.form.get('catchZoneControl')!.setValue(quadrant.zone?.toString());
            }

            this.form.get('thirdCountryCatchZoneControl')!.setValue(this.model.thirdCountryCatchZone);
            this.form.get('isContinentalCatchControl')!.setValue(this.model.isContinentalCatch ?? false);

            this.setExpansionPanelTitle(this.model);
        }
    }

    protected buildForm(): AbstractControl {
        return new FormGroup({
            aquaticOrganismControl: new FormControl(undefined, Validators.required),
            quantityKgControl: new FormControl(undefined, [Validators.required, TLValidators.number(0)]),
            catchTypeControl: new FormControl(undefined, Validators.required),
            catchSizeControl: new FormControl(undefined, Validators.required),

            sturgeonGenderControl: new FormControl(),
            sturgeonSizeControl: new FormControl(),

            turbotCountControl: new FormControl(),
            turbotSizeGroupControl: new FormControl(),

            catchZoneControl: new FormControl(),
            catchQuadrantControl: new FormControl(undefined, Validators.required),
            thirdCountryCatchZoneControl: new FormControl(undefined, Validators.maxLength(50)),
            isContinentalCatchControl: new FormControl(false)
        });
    }

    protected getValue(): CatchRecordFishDTO {
        if (this.model === undefined || this.model === null) {
            this.model = new CatchRecordFishDTO({
                id: this.shipLogBookPageDataService.nextNewCatchRecordId,
                isActive: true,
                unloadedQuantityKg: 0,
                unloadedInOtherTripQuantityKg: 0
            });
        }

        this.model.fishId = this.form.get('aquaticOrganismControl')!.value?.value;
        this.model.fishName = this.form.get('aquaticOrganismControl')!.value?.displayName ?? '';
        this.model.quantityKg = this.form.get('quantityKgControl')!.value;
        const catchType: NomenclatureDTO<number> | undefined = this.form.get('catchTypeControl')!.value;
        this.model.catchTypeId = catchType?.value;
        this.model.catchSizeId = this.form.get('catchSizeControl')!.value?.value;

        this.model.sturgeonGender = this.form.get('sturgeonGenderControl')!.value?.value;
        this.model.sturgeonSize = this.form.get('sturgeonSizeControl')!.value;

        this.model.turbotCount = this.form.get('turbotCountControl')!.value;
        this.model.turbotSizeGroupId = this.form.get('turbotSizeGroupControl')!.value?.value;

        this.model.catchQuadrantId = this.form.get('catchQuadrantControl')!.value?.value;
        this.model.catchQuadrant = this.form.get('catchQuadrantControl')!.value?.displayName ?? '';
        this.model.catchZone = this.form.get('catchZoneControl')!.value;
        this.model.thirdCountryCatchZone = this.form.get('thirdCountryCatchZoneControl')!.value;
        this.model.isContinentalCatch = this.form.get('isContinentalCatchControl')!.value ?? false;

        if (catchType !== null && catchType !== undefined) {
            this.model.isDetainedOnBoard = CatchTypeCodesEnum[catchType.code! as keyof typeof CatchTypeCodesEnum] === CatchTypeCodesEnum.TAKEN_ONBOARD;
        }
        else {
            this.model.isDetainedOnBoard = false;
        }

        return this.model;
    }

    private setTurbotAndSturgeonFlagsAndValidators(value: FishNomenclatureDTO | undefined): void {
        if (value !== null && value !== undefined) {
            if (FishCodesEnum[value.code as keyof typeof FishCodesEnum] === FishCodesEnum.TUR) {
                this.showTurbotControls = true;

                if (this.isDisabled) {
                    this.form.get('turbotCountControl')!.disable();
                    this.form.get('turbotSizeGroupControl')!.disable();
                }
            }
            else {
                this.showTurbotControls = false;

                if (value.familyType === FishFamilyTypesEnum.Sturgeon) {
                    this.showStrugeonControls = true;

                    if (this.isDisabled) {
                        this.form.get('sturgeonGenderControl')!.disable();
                        this.form.get('sturgeonSizeControl')!.disable();
                    }
                }
                else {
                    this.showStrugeonControls = false;
                }
            }
        }
        else {
            this.showTurbotControls = false;
            this.showStrugeonControls = false;
        }

        this.setTurbotControlsValidators();
        this.setSturgeonControlsValidators();
    }

    private setSturgeonControlsValidators(): void {
        if (this.showStrugeonControls === true) {
            this.form.get('sturgeonGenderControl')!.setValidators(Validators.required);
            this.form.get('sturgeonSizeControl')!.setValidators(Validators.required);

            this.form.get('sturgeonGenderControl')!.markAsPending();
            this.form.get('sturgeonSizeControl')!.markAsPending();
        }
        else {
            this.form.get('sturgeonGenderControl')!.setValidators(null);
            this.form.get('sturgeonSizeControl')!.setValidators(null);
        }
    }

    private setTurbotControlsValidators(): void {
        if (this.showTurbotControls === true) {
            this.form.get('turbotCountControl')!.setValidators([Validators.required, TLValidators.number(0)]);
            this.form.get('turbotSizeGroupControl')!.setValidators(Validators.required);

            this.form.get('turbotCountControl')!.markAsPending();
            this.form.get('turbotSizeGroupControl')!.markAsPending();
        }
        else {
            this.form.get('turbotCountControl')!.setValidators(null);
            this.form.get('turbotSizeGroupControl')!.setValidators(null);
        }
    }

    private setExpansionPanelTitle(value: CatchRecordFishDTO): void {
        if (value.fishName !== undefined && value.fishName !== null && value.fishName.length > 0) {
            this.expansionPanelTitle = `${value.fishName}`;

            if (value.quantityKg !== undefined && value.quantityKg !== null) {
                this.expansionPanelTitle = `${this.expansionPanelTitle} - ${value.quantityKg}kg`;
            }
        }
    }

    private createCustomGridLayerStyle(): SimplePolygonStyleDef {
        const layerStyle: SimplePolygonStyleDef = new SimplePolygonStyleDef();
        layerStyle.fill = new FillDef('rgba(255,255,255,0.4)');
        layerStyle.stroke = new StrokeDef('rgb(0,116,2,1)', 1);

        return layerStyle;

    }

    private createCustomSelectGridLayerStyle(): SimplePolygonStyleDef {
        const layerStyle: SimplePolygonStyleDef = new SimplePolygonStyleDef();
        layerStyle.fill = new FillDef('rgb(255,177,34, 0.1)');
        layerStyle.stroke = new StrokeDef('rgb(255,177,34,1)', 3);

        return layerStyle;
    }

    private thirdCountryCatchZoneControlValueChanged(value: string | undefined): void {
        const isContinentalCatchControlValue: boolean | undefined = this.form.get('isContinentalCatchControl')!.value;
            
        if (value !== null && value !== undefined && value !== '') {
            this.form.get('catchQuadrantControl')!.clearValidators();
            this.form.get('catchQuadrantControl')!.markAsPending();
            this.form.get('catchQuadrantControl')!.updateValueAndValidity({ emitEvent: false });
        }
        else {
            if (isContinentalCatchControlValue === false
                || isContinentalCatchControlValue === null
                || isContinentalCatchControlValue === undefined
            ) {
                this.form.get('catchQuadrantControl')!.setValidators([Validators.required]);
                this.form.get('catchQuadrantControl')!.markAsPending();
                this.form.get('catchQuadrantControl')!.updateValueAndValidity({ emitEvent: false });
            }
        }

        if (this.isDisabled) {
            this.form.get('catchQuadrantControl')!.disable();
        }
    }

    private setCatchQuadrantControlValidators(): void {
        const isContinentalCatch: boolean = this.form.get('isContinentalCatchControl')!.value;
        const thirdCountryCatchZone: string | undefined = this.form.get('thirdCountryCatchZoneControl')!.value;
        const isDanubeWaterType: boolean = this.waterType === WaterTypesEnum.DANUBE;

        if (isContinentalCatch || (thirdCountryCatchZone !== null && thirdCountryCatchZone !== undefined) || isDanubeWaterType) {
            this.form.get('catchQuadrantControl')!.clearValidators();
        }
        else {
            this.form.get('catchQuadrantControl')!.setValidators([Validators.required]);
        }

        this.form.get('catchQuadrantControl')!.markAsPending();
        this.form.get('catchQuadrantControl')!.updateValueAndValidity({ emitEvent: false });

        if (this.isDisabled) {
            this.form.get('catchQuadrantControl')!.disable();
        }
    }
}