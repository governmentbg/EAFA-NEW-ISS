import { Component, ElementRef, EventEmitter, Input, OnChanges, OnInit, Optional, Output, Self, SimpleChanges } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, NgControl, Validators } from '@angular/forms';

import { CustomFormControl } from '@app/shared/utils/custom-form-control';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { InspectionShipNomenclatureDTO } from '@app/models/generated/dtos/InspectionShipNomenclatureDTO';
import { InspectionsService } from '@app/services/administration-app/inspections.service';
import { VesselDuringInspectionDTO } from '@app/models/generated/dtos/VesselDuringInspectionDTO';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { InspectedShipParams } from './models/inspected-ship-params.model';

@Component({
    selector: 'inspected-ship',
    templateUrl: './inspected-ship.component.html'
})
export class InspectedShipComponent extends CustomFormControl<VesselDuringInspectionDTO | undefined> implements OnInit, OnChanges, IDialogComponent {

    @Output()
    public shipSelected: EventEmitter<VesselDuringInspectionDTO> = new EventEmitter<VesselDuringInspectionDTO>();

    @Input()
    public hasMap: boolean = true;

    @Input()
    public ships: NomenclatureDTO<number>[] = [];

    @Input()
    public vesselTypes: NomenclatureDTO<number>[] = [];

    @Input()
    public countries: NomenclatureDTO<number>[] = [];

    @Input()
    public isShipRequired: boolean = true;

    public isFromRegister: boolean = true;
    public canChangeRegister: boolean = true;

    private selectedShip: VesselDuringInspectionDTO | undefined;
    private readOnly: boolean = false;

    private readonly service: InspectionsService;
    private readonly element: ElementRef<HTMLElement>;

    public constructor(
        @Optional() @Self() ngControl: NgControl,
        @Self() element: ElementRef<HTMLElement>,
        service: InspectionsService
    ) {
        super(ngControl);

        this.service = service;
        this.element = element;

        this.onMarkAsTouched.subscribe({
            next: () => {
                this.control.updateValueAndValidity();
            }
        });
    }

    public ngOnChanges(changes: SimpleChanges): void {
        if ('hasMap' in changes) {
            if (this.hasMap && this.isDisabled === false) {
                this.form.get('shipMapControl')!.setValidators(Validators.required);
                this.form.get('shipMapControl')!.markAsPending();
                this.form.get('shipMapControl')!.updateValueAndValidity({ emitEvent: false });
                this.form.get('shipMapControl')!.enable();
            }
            else {
                this.form.get('shipMapControl')!.clearValidators();
                this.form.get('shipMapControl')!.markAsPending();
                this.form.get('shipMapControl')!.updateValueAndValidity({ emitEvent: false });
                this.form.get('shipMapControl')!.disable();
            }
        }

        if ('isShipRequired' in changes) {
            this.onShipRegisteredChanged(this.isFromRegister);
        }
    }

    public ngOnInit(): void {
        this.initCustomFormControl();

        if (this.readOnly === true) {
            this.form.disable();
        }

        if (this.selectedShip !== null && this.selectedShip !== undefined) {
            this.writeValue(this.selectedShip);
        }
    }

    public setDisabledState(isDisabled: boolean): void {
        this.isDisabled = isDisabled;

        if (this.isDisabled) {
            this.form.disable();
        }
        else {
            this.form.enable();

            if (this.isFromRegister) {
                this.form.get('nameControl')!.disable();
                this.form.get('externalMarkControl')!.disable();
                this.form.get('cfrControl')!.disable();
                this.form.get('flagControl')!.disable();
                this.form.get('uviControl')!.disable();
                this.form.get('callsignControl')!.disable();
                this.form.get('shipTypeControl')!.disable();
                this.form.get('mmsiControl')!.disable();
            } else {
                this.form.get('nameControl')!.enable();
                this.form.get('externalMarkControl')!.enable();
                this.form.get('cfrControl')!.enable();
                this.form.get('flagControl')!.enable();
                this.form.get('uviControl')!.enable();
                this.form.get('callsignControl')!.enable();
                this.form.get('shipTypeControl')!.enable();
                this.form.get('mmsiControl')!.enable();
            }
        }
    }

    public setData(data: InspectedShipParams, wrapperData: DialogWrapperData): void {
        this.ships = data.ships;
        this.hasMap = data.hasMap;
        this.readOnly = data.readOnly;

        this.selectedShip = data.model;
    }

    public dialogButtonClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public saveBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose(this.getValue());
    }

    public cancelBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public writeValue(value: VesselDuringInspectionDTO | undefined): void {
        if (value !== undefined && value !== null) {
            this.isFromRegister = value.shipId !== undefined && value.shipId !== null;
            this.form.get('shipRegisteredControl')!.setValue(this.isFromRegister);

            this.form.get('shipMapControl')!.setValue(value.location);

            if (this.isFromRegister) {
                this.selectedShip = new VesselDuringInspectionDTO(value);
                const ship = this.ships.find(f => f.value === this.selectedShip!.shipId);
                setTimeout(() => {
                    // Кода стига до тук преди angular да построи своя UI,
                    // карайки [options]="ship" да не се е случило и стойността да не се покаже.
                    this.form.get('shipControl')!.setValue(ship, { emitEvent: false });
                });
            }

            this.form.get('flagControl')!.setValue(this.countries.find(f => f.value === value.flagCountryId));
            this.form.get('uviControl')!.setValue(value.uvi);
            this.form.get('callsignControl')!.setValue(value.regularCallsign);
            this.form.get('shipTypeControl')!.setValue(this.vesselTypes.find(f => f.value === value.vesselTypeId));
            this.form.get('mmsiControl')!.setValue(value.mmsi);
        }
        else {
            this.form.get('shipControl')!.setValue(null, { emitEvent: false });
            this.form.get('shipMapControl')!.setValue(null);
            this.form.get('flagControl')!.setValue(this.countries.find(f => f.code === CommonUtils.COUNTRIES_BG));
            this.form.get('uviControl')!.setValue(null);
            this.form.get('callsignControl')!.setValue(null);
            this.form.get('shipTypeControl')!.setValue(null);
            this.form.get('mmsiControl')!.setValue(null);
        }
    }

    protected buildForm(): AbstractControl {
        const form = new FormGroup({
            shipRegisteredControl: new FormControl(true),
            shipControl: new FormControl(undefined, Validators.required),
            nameControl: new FormControl(undefined),
            externalMarkControl: new FormControl(undefined),
            cfrControl: new FormControl(undefined),
            flagControl: new FormControl({ value: undefined, disabled: true }),
            uviControl: new FormControl({ value: undefined, disabled: true }),
            callsignControl: new FormControl({ value: undefined, disabled: true }),
            shipTypeControl: new FormControl({ value: undefined, disabled: true }),
            mmsiControl: new FormControl({ value: undefined, disabled: true }),
            shipMapControl: new FormControl(undefined, Validators.required),
        });

        form.get('shipRegisteredControl')!.valueChanges.subscribe({
            next: this.onShipRegisteredChanged.bind(this)
        });

        form.get('shipControl')!.valueChanges.subscribe({
            next: this.onShipChanged.bind(this)
        });

        return form;
    }

    protected getValue(): VesselDuringInspectionDTO | undefined {
        if (!this.isFromRegister) {
            return new VesselDuringInspectionDTO({
                isRegistered: false,
                cfr: this.form.get('cfrControl')!.value,
                externalMark: this.form.get('externalMarkControl')!.value,
                flagCountryId: this.form.get('flagControl')!.value?.value,
                mmsi: this.form.get('mmsiControl')!.value,
                name: this.form.get('nameControl')!.value,
                regularCallsign: this.form.get('callsignControl')!.value,
                uvi: this.form.get('uviControl')!.value,
                vesselTypeId: this.form.get('shipTypeControl')!.value?.value,
                location: this.form.get('shipMapControl')!.value,
            });
        }
        else if (this.selectedShip !== undefined && this.selectedShip !== null) {
            this.selectedShip.isRegistered = true;
            this.selectedShip.location = this.form.get('shipMapControl')!.value;
            return this.selectedShip;
        }

        return undefined;
    }

    private onShipRegisteredChanged(value: boolean): void {
        this.isFromRegister = value;

        if (this.isDisabled) {
            return;
        }

        if (value) {
            this.form.get('shipControl')!.setValidators(this.isShipRequired ? [Validators.required] : []);
            this.form.get('nameControl')!.clearValidators();
            this.form.get('externalMarkControl')!.clearValidators();
            this.form.get('cfrControl')!.clearValidators();
            this.form.get('flagControl')!.clearValidators();
            this.form.get('uviControl')!.clearValidators();
            this.form.get('callsignControl')!.clearValidators();
            this.form.get('shipTypeControl')!.clearValidators();
            this.form.get('mmsiControl')!.clearValidators();

            this.form.get('nameControl')!.disable();
            this.form.get('externalMarkControl')!.disable();
            this.form.get('cfrControl')!.disable();
            this.form.get('flagControl')!.disable();
            this.form.get('uviControl')!.disable();
            this.form.get('callsignControl')!.disable();
            this.form.get('shipTypeControl')!.disable();
            this.form.get('mmsiControl')!.disable();
        } else {
            this.form.get('shipControl')!.clearValidators();
            this.form.get('nameControl')!.setValidators(this.isShipRequired ? [Validators.required, Validators.maxLength(500)] : [Validators.maxLength(500)]);
            this.form.get('externalMarkControl')!.setValidators(this.isShipRequired ? [Validators.required, Validators.maxLength(50)] : [Validators.maxLength(50)]);
            this.form.get('cfrControl')!.setValidators([Validators.maxLength(20)]);
            this.form.get('flagControl')!.setValidators(this.isShipRequired ? [Validators.required] : []);
            this.form.get('uviControl')!.setValidators([Validators.maxLength(20)]);
            this.form.get('callsignControl')!.setValidators([Validators.maxLength(50)]);
            this.form.get('shipTypeControl')!.setValidators(this.isShipRequired ? [Validators.required] : []);
            this.form.get('mmsiControl')!.setValidators([Validators.maxLength(20)]);

            this.form.get('nameControl')!.enable();
            this.form.get('externalMarkControl')!.enable();
            this.form.get('cfrControl')!.enable();
            this.form.get('flagControl')!.enable();
            this.form.get('uviControl')!.enable();
            this.form.get('callsignControl')!.enable();
            this.form.get('shipTypeControl')!.enable();
            this.form.get('mmsiControl')!.enable();
        }

        this.form.get('shipControl')!.markAsPending();
        this.form.get('nameControl')!.markAsPending();
        this.form.get('externalMarkControl')!.markAsPending();
        this.form.get('cfrControl')!.markAsPending();
        this.form.get('flagControl')!.markAsPending();
        this.form.get('uviControl')!.markAsPending();
        this.form.get('callsignControl')!.markAsPending();
        this.form.get('shipTypeControl')!.markAsPending();
        this.form.get('mmsiControl')!.markAsPending();

        this.form.get('shipControl')!.updateValueAndValidity({ emitEvent: false });
        this.form.get('nameControl')!.updateValueAndValidity({ emitEvent: false });
        this.form.get('externalMarkControl')!.updateValueAndValidity({ emitEvent: false });
        this.form.get('cfrControl')!.updateValueAndValidity({ emitEvent: false });
        this.form.get('flagControl')!.updateValueAndValidity({ emitEvent: false });
        this.form.get('uviControl')!.updateValueAndValidity({ emitEvent: false });
        this.form.get('callsignControl')!.updateValueAndValidity({ emitEvent: false });
        this.form.get('shipTypeControl')!.updateValueAndValidity({ emitEvent: false });
        this.form.get('mmsiControl')!.updateValueAndValidity({ emitEvent: false });
    }

    private async onShipChanged(value: InspectionShipNomenclatureDTO): Promise<void> {
        if (typeof value === 'string' || value === null || value === undefined || value.value === null || value.value === undefined || value.value === this.selectedShip?.shipId) {
            return;
        }

        this.element.nativeElement.focus();

        this.selectedShip = await this.service.getShip(value.value!).toPromise();

        this.form.get('flagControl')!.setValue(this.countries.find(f => f.value === this.selectedShip!.flagCountryId));
        this.form.get('uviControl')!.setValue(this.selectedShip.uvi);
        this.form.get('callsignControl')!.setValue(this.selectedShip.regularCallsign);
        this.form.get('shipTypeControl')!.setValue(this.vesselTypes.find(f => f.value === this.selectedShip!.vesselTypeId));
        this.form.get('mmsiControl')!.setValue(this.selectedShip.mmsi);

        this.shipSelected.emit(this.selectedShip);
    }
}