import { Component, Input, OnChanges, OnInit, Self, SimpleChanges, ViewChild } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, NgControl, Validators } from '@angular/forms';
import { Subject } from 'rxjs';
import { debounceTime } from 'rxjs/operators';
import { FillDef, MapOptions, PinDef, SimplePolygonStyleDef, StrokeDef, TLMapViewerComponent } from '@tl/tl-angular-map';

import { CustomFormControl } from '@app/shared/utils/custom-form-control';
import { LocationDTO } from '@app/models/generated/dtos/LocationDTO';
import { CoordinateUtils } from '@app/shared/utils/coordinate.utis';
import { CommonUtils } from '@app/shared/utils/common.utils';

@Component({
    selector: 'inspection-map-viewer',
    templateUrl: './inspection-map-viewer.component.html',
    styleUrls: ['./inspection-map-viewer.component.scss']
})
export class InspectionMapViewerComponent extends CustomFormControl<LocationDTO | undefined> implements OnInit, OnChanges {

    public mapOptions: MapOptions | undefined;
    public selectIcon: string;

    @Input()
    public isRequired: boolean = true;

    @ViewChild(TLMapViewerComponent)
    private mapViewer!: TLMapViewerComponent;

    private searchingCoordinates = new Subject();

    public constructor(@Self() ngControl: NgControl) {
        super(ngControl);

        this.selectIcon = 'pin_drop';
        this.mapOptions = new MapOptions();
        this.mapOptions.showGridLayer = false;
        this.mapOptions.gridLayerStyle = this.createCustomGridLayerStyle();
        this.mapOptions.selectGridLayerStyle = this.createCustomSelectGridLayerStyle();

        this.onMarkAsTouched.subscribe({
            next: () => {
                this.control.updateValueAndValidity();
            }
        });
    }

    public ngOnChanges(changes: SimpleChanges): void {
        if ('isRequired' in changes) {
            if (this.isRequired) {
                this.form.get('longitudeControl')!.setValidators(Validators.required);
                this.form.get('latitudeControl')!.setValidators(Validators.required);
            }
            else {
                this.form.get('longitudeControl')!.clearValidators();
                this.form.get('latitudeControl')!.clearValidators();
            }
        }
    }

    public ngOnInit(): void {
        this.initCustomFormControl();

        this.searchingCoordinates.pipe(debounceTime(100)).subscribe({
            next: this.onCoordinatesChanged.bind(this)
        });
    }

    public writeValue(value: LocationDTO | undefined): void {
        if (value !== undefined && value !== null) {
            this.form.get('longitudeControl')!.setValue(value.dmsLongitude!);
            this.form.get('latitudeControl')!.setValue(value.dmsLatitude!);
        }
    }

    public toggleMapSelection(): void {
        if (this.selectIcon === 'pin_drop') {
            this.selectIcon = 'edit_location';
            this.mapViewer.startClickMapTool();
        }
        else {
            this.selectIcon = 'pin_drop';
            this.mapViewer.stopClickMapTool();
        }
    }

    public onMapClicked(coordinateNums: number[]): void {
        if (this.isDisabled) {
            return;
        }

        const pin: PinDef = new PinDef(coordinateNums, 'assets/map-icons/map-pin-red.png', undefined);
        this.mapViewer.clearPins();
        this.mapViewer.addPinsToMap([pin]);

        this.form.get('longitudeControl')!.setValue(CoordinateUtils.ConvertToDMS(coordinateNums[0]));
        this.form.get('latitudeControl')!.setValue(CoordinateUtils.ConvertToDMS(coordinateNums[1]));
    }

    protected buildForm(): AbstractControl {
        const form = new FormGroup({
            longitudeControl: new FormControl(undefined, Validators.required),
            latitudeControl: new FormControl(undefined, Validators.required),
        });

        form.get('longitudeControl')!.valueChanges.subscribe({
            next: this.onCoordinatesChanging.bind(this)
        });

        form.get('latitudeControl')!.valueChanges.subscribe({
            next: this.onCoordinatesChanging.bind(this)
        });

        return form;
    }

    protected getValue(): LocationDTO | undefined {
        const long: string = this.form.get('longitudeControl')!.value;
        const lat: string = this.form.get('latitudeControl')!.value;

        if (CommonUtils.isNullOrUndefined(long) || CommonUtils.isNullOrUndefined(lat)) {
            return undefined;
        }

        return new LocationDTO({
            dmsLatitude: lat,
            dmsLongitude: long
        });
    }

    private onCoordinatesChanging(): void {
        this.searchingCoordinates.next();
    }

    private onCoordinatesChanged(): void {
        const long = this.form.get('longitudeControl')!;
        const lat = this.form.get('latitudeControl')!;

        if (long.invalid || lat.invalid) {
            return;
        }

        const coordinateNums: number[] = [
            CoordinateUtils.ConvertFromDMS(long.value),
            CoordinateUtils.ConvertFromDMS(lat.value),
        ];
        const pin: PinDef = new PinDef(coordinateNums, 'assets/map-icons/map-pin-red.png', undefined);
        this.mapViewer.clearPins();
        this.mapViewer.addPinsToMap([pin]);
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
}