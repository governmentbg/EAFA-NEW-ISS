import { Component, OnInit, ViewChild } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { forkJoin } from 'rxjs';
import { FillDef, MapOptions, PinDef, SimplePolygonStyleDef, StrokeDef, TLMapViewerComponent } from '@tl/tl-angular-map';

import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { DialogParamsModel } from '@app/models/common/dialog-params.model';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { PoundnetCoordinateDTO } from '@app/models/generated/dtos/PoundnetCoordinateDTO';
import { PoundnetRegisterDTO } from '@app/models/generated/dtos/PoundnetRegisterDTO';
import { PoundnetRegisterService } from '@app/services/administration-app/poundnet-register.service';
import { CommonNomenclatures } from '@app/services/common-app/common-nomenclatures.service';
import { CommandTypes } from '@app/shared/components/data-table/enums/command-type.enum';
import { RecordChangedEventArgs } from '@app/shared/components/data-table/models/record-changed-event.model';
import { TLDataTableComponent } from '@app/shared/components/data-table/tl-data-table.component';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { CoordinateUtils } from '@app/shared/utils/coordinate.utis';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { IPoundnetRegisterService } from '@app/interfaces/administration-app/poundnet-register.interface';
import { TLValidators } from '@app/shared/utils/tl-validators';
import { RangeInputData } from '@app/shared/components/input-controls/tl-range-input/range-input.component';
import { ValidityCheckerGroupDirective } from '@app/shared/directives/validity-checker/validity-checker-group.directive';

@Component({
    selector: 'edit-poundnet-component',
    templateUrl: './edit-poundnet.component.html',
})
export class EditPoundnetComponent implements OnInit, IDialogComponent {
    public form!: FormGroup;
    public coordinatesForm!: FormGroup;

    public seasonalTypes: NomenclatureDTO<number>[] = [];
    public categoryTypes: NomenclatureDTO<number>[] = [];
    public muncipalities: NomenclatureDTO<number>[] = [];
    public districts: NomenclatureDTO<number>[] = [];
    public populatedAreas: NomenclatureDTO<number>[] = [];
    public statuses: NomenclatureDTO<number>[] = [];

    public poundnetCoordinates: PoundnetCoordinateDTO[] = [];
    public mapOptions: MapOptions | undefined;
    public viewMode: boolean = false;
    public gpsCoordinatesTouched: boolean = false;

    @ViewChild(TLMapViewerComponent)
    private mapViewer!: TLMapViewerComponent;

    @ViewChild(TLDataTableComponent)
    private datatable!: TLDataTableComponent;

    @ViewChild(ValidityCheckerGroupDirective)
    private validityCheckerGroup!: ValidityCheckerGroupDirective;

    private service: IPoundnetRegisterService;
    private nomenclatures: CommonNomenclatures;

    private model!: PoundnetRegisterDTO;
    private poundNetId!: number;

    public constructor(service: PoundnetRegisterService, nomenclatures: CommonNomenclatures) {
        this.service = service;
        this.nomenclatures = nomenclatures;

        this.buildForm();

        this.mapOptions = new MapOptions();
        this.mapOptions.showGridLayer = false;
        this.mapOptions.gridLayerStyle = this.createCustomGridLayerStyle();
        this.mapOptions.selectGridLayerStyle = this.createCustomSelectGridLayerStyle();
    }

    public async ngOnInit(): Promise<void> {
        const nomenclatures: NomenclatureDTO<number>[][] = await forkJoin(
            NomenclatureStore.instance.getNomenclature(
                NomenclatureTypes.PoundnetCategoryTypes, this.service.getCategories.bind(this.service), false
            ),
            NomenclatureStore.instance.getNomenclature(
                NomenclatureTypes.PoundnetSeasonalTypes, this.service.getSeasonalTypes.bind(this.service), false
            ),
            NomenclatureStore.instance.getNomenclature(
                NomenclatureTypes.Municipalities, this.nomenclatures.getMunicipalities.bind(this.nomenclatures), false
            ),
            NomenclatureStore.instance.getNomenclature(
                NomenclatureTypes.Districts, this.nomenclatures.getDistricts.bind(this.nomenclatures), false
            ),
            NomenclatureStore.instance.getNomenclature(
                NomenclatureTypes.PopulatedAreas, this.nomenclatures.getPopulatedAreas.bind(this.nomenclatures), false
            ),
            NomenclatureStore.instance.getNomenclature(
                NomenclatureTypes.PoundnetStatuses, this.service.getPoundnetStatuses.bind(this.service), false
            )
        ).toPromise();

        this.categoryTypes = nomenclatures[0];
        this.seasonalTypes = nomenclatures[1];
        this.muncipalities = nomenclatures[2];
        this.districts = nomenclatures[3];
        this.populatedAreas = nomenclatures[4];
        this.statuses = nomenclatures[5];

        if (this.poundNetId !== undefined && this.poundNetId !== null) {
            this.service.get(this.poundNetId).subscribe({
                next: (result: PoundnetRegisterDTO) => {
                    this.model = result;

                    this.fillForm();

                    if (this.viewMode) {
                        this.form.disable();
                    }
                }
            });
        }
    }

    public ngAfterViewInit(): void {
        this.datatable?.recordChanged.subscribe({
            next: (event: RecordChangedEventArgs<PoundnetCoordinateDTO>) => {
                this.gpsCoordinatesTouched = true;
                this.form.updateValueAndValidity({ onlySelf: true });
            }
        });
    }

    public setData(data: DialogParamsModel | undefined, buttons: DialogWrapperData): void {
        if (data === undefined || data === null) {
            this.model = new PoundnetRegisterDTO();
        }
        else {
            this.poundNetId = data.id;
            this.viewMode = data.isReadonly;
        }
    }

    public saveBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        this.markAllAsTouched();
        this.form.updateValueAndValidity({ onlySelf: true });
        this.validityCheckerGroup.validate();

        if (this.form.valid) {
            this.fillModel();
            CommonUtils.sanitizeModelStrings(this.model);

            if (this.poundNetId !== undefined && this.poundNetId !== null) {
                this.service.edit(this.model).subscribe({
                    next: () => {
                        NomenclatureStore.instance.clearNomenclature(NomenclatureTypes.PoundNets);
                        dialogClose(this.model);
                    }
                });
            } else {
                this.service.add(this.model).subscribe({
                    next: (id: number) => {
                        this.model.id = id;
                        NomenclatureStore.instance.clearNomenclature(NomenclatureTypes.PoundNets);
                        dialogClose(this.model);
                    }
                });
            }
        }
    }


    public cancelBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public dialogButtonClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public coordinatesRecordChanged(event: RecordChangedEventArgs<PoundnetCoordinateDTO>): void {
        switch (event.Command) {
            case CommandTypes.Add: {
                const coordinates: number[] = this.convertToCoordinates(event.Record);
                const pin: PinDef = new PinDef(coordinates, 'assets/map-icons/map-pin-gr.png', undefined);
                this.mapViewer.addPinsToMap([pin]);
            } break;
            case CommandTypes.Edit:
            case CommandTypes.Delete: {
                this.mapViewer.clearPins();
                this.poundnetCoordinates = this.datatable.rows;
                this.addPinsToMap(this.poundnetCoordinates);
            } break;
            default:
        }
    }

    private buildForm(): void {
        this.form = new FormGroup({
            numberControl: new FormControl(undefined, [Validators.required, Validators.maxLength(20)]),
            dateControl: new FormControl(undefined, Validators.required),
            nameControl: new FormControl(undefined, [Validators.required, Validators.maxLength(500)]),
            statusControl: new FormControl(undefined, Validators.required),
            seasonalControl: new FormControl(undefined, Validators.required),
            categoryControl: new FormControl(undefined, Validators.required),
            orderNumberControl: new FormControl(undefined, Validators.maxLength(50)),
            orderDateControl: new FormControl(undefined),
            commentsControl: new FormControl(undefined, Validators.maxLength(1000)),
            areaDescriptionControl: new FormControl(undefined, Validators.maxLength(4000)),
            placeDepthControl: new FormControl(undefined),
            towelLengthControl: new FormControl(undefined, TLValidators.number(0)),
            houseWidthControl: new FormControl(undefined, TLValidators.number(0)),
            houseLengthControl: new FormControl(undefined, TLValidators.number(0)),
            bagSizeControl: new FormControl(undefined, TLValidators.number(0)),
            districtControl: new FormControl(undefined),
            muncipalityControl: new FormControl(undefined),
            populatedAreaControl: new FormControl(undefined),
            regionControl: new FormControl(undefined, Validators.maxLength(100)),
            placeDescriptionControl: new FormControl(undefined, Validators.maxLength(500)),
            permitLicensePriceControl: new FormControl(undefined, TLValidators.number(0))
        }, this.coordinatesValidator());

        this.coordinatesForm = new FormGroup({
            degreesControl: new FormControl(),
            minutesControl: new FormControl(),
            secondsControl: new FormControl(),
            longitudeControl: new FormControl('', [Validators.required]),
            latitudeControl: new FormControl('', [Validators.required]),
            isConnectPointControl: new FormControl(false)
        });

    }

    private fillForm(): void {
        this.form.get('numberControl')!.setValue(this.model.poundNetNum);
        this.form.get('dateControl')!.setValue(this.model.registrationDate);
        this.form.get('nameControl')!.setValue(this.model.name);
        this.form.get('statusControl')!.setValue(this.statuses.find(x => x.value === this.model.statusId));
        this.form.get('seasonalControl')!.setValue(this.seasonalTypes.find(x => x.value === this.model.seasonTypeId));
        this.form.get('categoryControl')!.setValue(this.categoryTypes.find(x => x.value === this.model.categoryTypeId));
        this.form.get('orderNumberControl')!.setValue(this.model.activityOrderNum);
        this.form.get('orderDateControl')!.setValue(this.model.activityOrderDate);
        this.form.get('commentsControl')!.setValue(this.model.comments);
        this.form.get('areaDescriptionControl')!.setValue(this.model.areaDescription);
        this.form.get('placeDepthControl')!.setValue(new RangeInputData({ start: this.model.depthFrom, end: this.model.depthTo }));
        this.form.get('towelLengthControl')!.setValue(this.model.towelLength);
        this.form.get('houseWidthControl')!.setValue(this.model.houseWidth);
        this.form.get('houseLengthControl')!.setValue(this.model.houseLength);
        this.form.get('bagSizeControl')!.setValue(this.model.bagEyeSize);
        this.form.get('districtControl')!.setValue(this.districts.find(x => x.value === this.model.districtId));
        this.form.get('muncipalityControl')!.setValue(this.muncipalities.find(x => x.value === this.model.municipalityId));
        this.form.get('populatedAreaControl')!.setValue(this.populatedAreas.find(x => x.value === this.model.populatedAreaId));
        this.form.get('regionControl')!.setValue(this.model.region);
        this.form.get('placeDescriptionControl')!.setValue(this.model.locationDescription);
        this.form.get('permitLicensePriceControl')!.setValue(this.model.permitLicensePrice);

        setTimeout(() => {
            this.poundnetCoordinates = this.model.poundnetCoordinates ?? [];
            this.addPinsToMap(this.poundnetCoordinates);
        });
    }

    private addPinsToMap(poundnetCoordinates: PoundnetCoordinateDTO[]) {
        const pins: PinDef[] = [];

        for (const coordinate of poundnetCoordinates) {
            const coordinates = this.convertToCoordinates(coordinate);
            const pin = new PinDef(coordinates, 'assets/map-icons/map-pin-gr.png', undefined);
            pins.push(pin);
        }

        this.mapViewer.addPinsToMap(pins);
    }

    private fillModel(): void {
        this.model.poundNetNum = this.form.get('numberControl')!.value;
        this.model.registrationDate = this.form.get('dateControl')!.value;
        this.model.name = this.form.get('nameControl')!.value;
        this.model.statusId = this.form.get('statusControl')!.value!.value;
        this.model.seasonTypeId = this.form.get('seasonalControl')!.value!.value;
        this.model.categoryTypeId = this.form.get('categoryControl')!.value!.value;
        this.model.activityOrderNum = this.form.get('orderNumberControl')!.value;
        this.model.activityOrderDate = this.form.get('orderDateControl')!.value;
        this.model.comments = this.form.get('commentsControl')!.value;
        this.model.areaDescription = this.form.get('areaDescriptionControl')!.value;
        this.model.depthFrom = (this.form.get('placeDepthControl')!.value as RangeInputData)?.start;
        this.model.depthTo = (this.form.get('placeDepthControl')!.value as RangeInputData)?.end;
        this.model.towelLength = this.form.get('towelLengthControl')!.value;
        this.model.houseWidth = this.form.get('houseWidthControl')!.value;
        this.model.houseLength = this.form.get('houseLengthControl')!.value;
        this.model.bagEyeSize = this.form.get('bagSizeControl')!.value;
        this.model.districtId = this.form.get('districtControl')!.value?.value;
        this.model.municipalityId = this.form.get('muncipalityControl')!.value?.value;
        this.model.populatedAreaId = this.form.get('populatedAreaControl')!.value?.value;
        this.model.region = this.form.get('regionControl')!.value;
        this.model.locationDescription = this.form.get('placeDescriptionControl')!.value;
        this.model.permitLicensePrice = this.form.get('permitLicensePriceControl')!.value;

        this.model.poundnetCoordinates = this.datatable.rows.map((x: PoundnetCoordinateDTO) => new PoundnetCoordinateDTO({
            id: x.id,
            latitude: x.latitude,
            longitude: x.longitude,
            isConnectPoint: x.isConnectPoint ?? false,
            isActive: x.isActive ?? true
        }));
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

    private convertToCoordinates(coordinate: PoundnetCoordinateDTO): number[] {
        const coordinates: number[] = [
            CoordinateUtils.ConvertFromDMS(coordinate.latitude!),
            CoordinateUtils.ConvertFromDMS(coordinate.longitude!)
        ];
        return coordinates;
    }

    private markAllAsTouched(): void {
        this.form.markAllAsTouched();

        this.gpsCoordinatesTouched = true;
    }

    private coordinatesValidator(): ValidatorFn {
        return (control: AbstractControl): ValidationErrors | null => {
            if (this.datatable !== undefined && this.datatable !== null) {
                if (this.datatable.rows.length === 0) {
                    return { 'atLeastOneCoordinateNeeded': true };
                } else {
                    let connectionPoints = 0;
                    for (const row of this.datatable.rows) {
                        if (row.isConnectPoint) {
                            connectionPoints++;
                        }
                    }
                    if (connectionPoints !== 1) {
                        return { 'atLeastOneConnectionPoint': true };
                    }
                }
            }

            return null;
        };
    }
}
