import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { forkJoin } from 'rxjs';

import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { CommonNomenclatures } from '@app/services/common-app/common-nomenclatures.service';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { InspectionsService } from '@app/services/administration-app/inspections.service';
import { VesselDuringInspectionDTO } from '@app/models/generated/dtos/VesselDuringInspectionDTO';
import { VesselDTO } from '@app/models/generated/dtos/VesselDTO';
import { PatrolVehicleTableParams } from '../../components/patrol-vehicles-table/models/patrol-vehicle-table-params';
import { PatrolVehicleTypeNomenclatureDTO } from '@app/models/generated/dtos/PatrolVehicleTypeNomenclatureDTO';
import { PatrolVehicleTypeEnum } from '@app/enums/patrol-vehicle-type.enum';
import { CoordinateUtils } from '@app/shared/utils/coordinate.utis';

@Component({
    selector: 'edit-patrol-vehicle',
    templateUrl: './edit-patrol-vehicle.component.html',
})
export class EditPatrolVehicleComponent implements OnInit, IDialogComponent {
    public form!: FormGroup;

    public isFromRegister: boolean = true;
    public isWaterVehicle: boolean = true;
    public hasCoordinates: boolean = true;

    public patrolVehicles: NomenclatureDTO<number>[] = [];
    public countries: NomenclatureDTO<number>[] = [];
    public institutions: NomenclatureDTO<number>[] = [];
    public patrolVehicleTypes: NomenclatureDTO<number>[] = [];

    protected model: VesselDuringInspectionDTO = new VesselDuringInspectionDTO();

    private readonly nomenclatures: CommonNomenclatures;
    private readonly service: InspectionsService;

    private isEdit: boolean = false;
    private readOnly: boolean = false;
    private selectedPatrolVehicle: VesselDTO | undefined;
    private excludeIds: number[] = [];
    private unfilteredPatrolVehicles: NomenclatureDTO<number>[] = [];

    public constructor(nomenclatures: CommonNomenclatures, service: InspectionsService) {
        this.nomenclatures = nomenclatures;
        this.service = service;

        this.buildForm();
    }

    public async ngOnInit(): Promise<void> {
        if (this.readOnly) {
            this.form.disable();
        }

        const nomenclatureTables: PatrolVehicleTypeNomenclatureDTO[][] = await forkJoin(
            NomenclatureStore.instance.getNomenclature(
                NomenclatureTypes.Countries, this.nomenclatures.getCountries.bind(this.nomenclatures), false
            ),
            NomenclatureStore.instance.getNomenclature(
                NomenclatureTypes.Institutions, this.nomenclatures.getInstitutions.bind(this.nomenclatures), false
            ),
            NomenclatureStore.instance.getNomenclature(
                NomenclatureTypes.PatrolVehicleTypes, this.nomenclatures.getPatrolVehicleTypes.bind(this.nomenclatures), false
            ),
            this.service.getPatrolVehicles(this.isWaterVehicle)
        ).toPromise();

        const type: PatrolVehicleTypeEnum = this.isWaterVehicle ? PatrolVehicleTypeEnum.Marine : PatrolVehicleTypeEnum.Ground;

        this.countries = nomenclatureTables[0];
        this.institutions = nomenclatureTables[1];
        this.patrolVehicleTypes = nomenclatureTables[2].filter(f => f.vehicleType === type
            || f.vehicleType === PatrolVehicleTypeEnum.Air
            || f.vehicleType === PatrolVehicleTypeEnum.Other
        );
        this.unfilteredPatrolVehicles = nomenclatureTables[3];
        this.patrolVehicles = nomenclatureTables[3].filter(f => !this.excludeIds.includes(f.value!));;

        this.fillForm();
    }

    public setData(data: PatrolVehicleTableParams, wrapperData: DialogWrapperData): void {
        if (data.model !== undefined && data.model !== null) {
            this.model = data.model;
            this.selectedPatrolVehicle = data.model;
        }

        this.excludeIds = data.excludeIds;
        this.isWaterVehicle = data.isWaterVehicle;
        this.readOnly = data.readOnly;
        this.isEdit = data.isEdit;
        this.hasCoordinates = data.hasCoordinates;

        if (!this.hasCoordinates) {
            this.form.get('mapViewerControl')!.disable();
        }
    }

    public dialogButtonClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public saveBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        if (this.readOnly) {
            dialogClose(this.model);
        }
        else {
            this.form.markAllAsTouched();
            if (this.form.valid) {
                this.fillModel();
                CommonUtils.sanitizeModelStrings(this.model);
                dialogClose(this.model);
            }
        }
    }

    public cancelBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    protected buildForm(): void {
        this.form = new FormGroup({
            patrolVehicleRegisteredControl: new FormControl(true),
            patrolVehicleControl: new FormControl(undefined, Validators.required),
            nameControl: new FormControl(undefined),
            cfrControl: new FormControl(undefined),
            externalMarkControl: new FormControl({ value: undefined, disabled: true }),
            callsignControl: new FormControl({ value: undefined, disabled: true }),
            countryControl: new FormControl({ value: undefined, disabled: true }, Validators.required),
            institutionControl: new FormControl({ value: undefined, disabled: true }, Validators.required),
            patrolVehicleTypeControl: new FormControl({ value: undefined, disabled: true }, Validators.required),
            mapViewerControl: new FormControl(undefined, Validators.required),
        });

        this.form.get('patrolVehicleRegisteredControl')!.valueChanges.subscribe({
            next: this.onPatrolVehicleRegisteredChanged.bind(this)
        });

        this.form.get('patrolVehicleControl')!.valueChanges.subscribe({
            next: this.onPatrolVehicleChanged.bind(this)
        });
    }

    protected fillForm(): void {
        if (this.isEdit) {
            this.isFromRegister = this.model.isRegistered === true;
            this.form.get('patrolVehicleRegisteredControl')!.setValue(this.isFromRegister);

            if (this.model.isRegistered) {
                this.form.get('patrolVehicleControl')!.setValue(this.unfilteredPatrolVehicles.find(f => f.value === this.model!.unregisteredVesselId));
            }
            else {
                this.form.get('nameControl')!.setValue(this.model.name);
                this.form.get('cfrControl')!.setValue(this.model.cfr);
                this.form.get('externalMarkControl')!.setValue(this.model.externalMark);
                this.form.get('callsignControl')!.setValue(this.model.regularCallsign);
            }

            this.form.get('countryControl')!.setValue(this.countries.find(f => f.value === this.model.flagCountryId));
            this.form.get('institutionControl')!.setValue(this.institutions.find(f => f.value === this.model.institutionId));
            this.form.get('patrolVehicleTypeControl')!.setValue(this.patrolVehicleTypes.find(f => f.value === this.model.patrolVehicleTypeId));

            this.form.get('mapViewerControl')!.setValue(this.model.location);
        }
        else {
            this.form.get('countryControl')!.setValue(this.countries.find(f => f.code === CommonUtils.COUNTRIES_BG));
            this.form.get('institutionControl')!.setValue(this.institutions.find(f => f.code === CommonUtils.INSTITUTIONS_IARA));
            this.form.get('patrolVehicleTypeControl')!.setValue(this.patrolVehicleTypes[0]);
        }
    }

    protected fillModel(): void {
        if (this.isFromRegister) {
            this.model = new VesselDuringInspectionDTO(this.selectedPatrolVehicle);
            this.model.isRegistered = true;
        }
        else {
            this.model = new VesselDuringInspectionDTO({
                name: this.form.get('nameControl')!.value,
                cfr: this.form.get('cfrControl')!.value,
                regularCallsign: this.form.get('callsignControl')!.value,
                externalMark: this.form.get('externalMarkControl')!.value,
                patrolVehicleTypeId: this.form.get('patrolVehicleTypeControl')!.value?.value,
                flagCountryId: this.form.get('countryControl')!.value?.value,
                institutionId: this.form.get('institutionControl')!.value?.value,
            });
        }

        this.model.location = this.form.get('mapViewerControl')!.value;

        if (this.model.location) {
            const long = CoordinateUtils.ConvertToDisplayDMS(this.model.location.longitude!);
            const lat = CoordinateUtils.ConvertToDisplayDMS(this.model.location.latitude!);

            this.model.locationText = long + ' ' + lat;
        }
    }

    private onPatrolVehicleRegisteredChanged(value: boolean): void {
        this.isFromRegister = value;

        if (value) {
            this.form.get('patrolVehicleControl')!.setValidators(Validators.required);
            this.form.get('nameControl')!.clearValidators();
            this.form.get('cfrControl')!.clearValidators();
            this.form.get('externalMarkControl')!.clearValidators();
            this.form.get('callsignControl')!.clearValidators();

            this.form.get('countryControl')!.disable();
            this.form.get('institutionControl')!.disable();
            this.form.get('patrolVehicleTypeControl')!.disable();
            this.form.get('externalMarkControl')!.disable();
            this.form.get('callsignControl')!.disable();
        }
        else {
            this.form.get('patrolVehicleControl')!.clearValidators();
            this.form.get('nameControl')!.setValidators([Validators.required, Validators.maxLength(500)]);
            this.form.get('cfrControl')!.setValidators([Validators.maxLength(20)]);
            this.form.get('externalMarkControl')!.setValidators([Validators.maxLength(50)]);
            this.form.get('callsignControl')!.setValidators([Validators.maxLength(50)]);

            this.form.get('countryControl')!.enable();
            this.form.get('institutionControl')!.enable();
            this.form.get('patrolVehicleTypeControl')!.enable();
            this.form.get('externalMarkControl')!.enable();
            this.form.get('callsignControl')!.enable();

            this.form.get('nameControl')!.markAsPending();
            this.form.get('cfrControl')!.markAsPending();
            this.form.get('externalMarkControl')!.markAsPending();
            this.form.get('callsignControl')!.markAsPending();
        }

        if (this.readOnly) {
            this.form.get('nameControl')!.disable();
            this.form.get('cfrControl')!.disable();
            this.form.get('countryControl')!.disable();
            this.form.get('institutionControl')!.disable();
            this.form.get('patrolVehicleTypeControl')!.disable();
            this.form.get('externalMarkControl')!.disable();
            this.form.get('callsignControl')!.disable();
        }

        this.form.get('patrolVehicleControl')!.updateValueAndValidity({ emitEvent: false });
        this.form.get('nameControl')!.updateValueAndValidity({ emitEvent: false });
        this.form.get('cfrControl')!.updateValueAndValidity({ emitEvent: false });
        this.form.get('externalMarkControl')!.updateValueAndValidity({ emitEvent: false });
        this.form.get('callsignControl')!.updateValueAndValidity({ emitEvent: false });
    }

    private async onPatrolVehicleChanged(value: NomenclatureDTO<number> | undefined): Promise<void> {
        if (value === null || value === undefined) {
            return;
        }

        this.selectedPatrolVehicle = await this.service.getPatrolVehicle(value.value!).toPromise();

        this.form.get('externalMarkControl')!.setValue(this.selectedPatrolVehicle.externalMark);
        this.form.get('callsignControl')!.setValue(this.selectedPatrolVehicle.regularCallsign);
        this.form.get('countryControl')!.setValue(this.countries.find(f => f.value === this.selectedPatrolVehicle!.flagCountryId));
        this.form.get('institutionControl')!.setValue(this.institutions.find(f => f.value === this.selectedPatrolVehicle!.institutionId));
        this.form.get('patrolVehicleTypeControl')!.setValue(this.patrolVehicleTypes.find(f => f.value === this.selectedPatrolVehicle!.patrolVehicleTypeId));
    }
}