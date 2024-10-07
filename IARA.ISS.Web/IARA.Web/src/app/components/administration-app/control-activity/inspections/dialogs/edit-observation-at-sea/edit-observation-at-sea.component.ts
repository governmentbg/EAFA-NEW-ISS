import { Component, OnInit } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { forkJoin } from 'rxjs';

import { IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { InspectionsService } from '@app/services/administration-app/inspections.service';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { CommonNomenclatures } from '@app/services/common-app/common-nomenclatures.service';
import { BaseInspectionsComponent } from '../base-inspection.component';
import { InspectionGeneralInfoModel } from '../../models/inspection-general-info-model';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { InspectionTypesEnum } from '@app/enums/inspection-types.enum';
import { InspectionAdditionalInfoModel } from '../../models/inspection-additional-info.model';
import { InspectionObservationCategoryEnum } from '@app/enums/inspection-observation-category.enum';
import { InspectionObservationToolNomenclatureDTO } from '@app/models/generated/dtos/InspectionObservationToolNomenclatureDTO';
import { ObservationToolOnBoardEnum } from '@app/enums/observation-tool-on-board.enum';
import { InspectionObservationAtSeaDTO } from '@app/models/generated/dtos/InspectionObservationAtSeaDTO';
import { VesselDuringInspectionDTO } from '@app/models/generated/dtos/VesselDuringInspectionDTO';
import { InspectionObservationToolDTO } from '@app/models/generated/dtos/InspectionObservationToolDTO';
import { InspectionVesselActivityNomenclatureDTO } from '@app/models/generated/dtos/InspectionVesselActivityNomenclatureDTO';
import { InspectionObservationTextDTO } from '@app/models/generated/dtos/InspectionObservationTextDTO';
import { ShipNomenclatureDTO } from '@app/models/generated/dtos/ShipNomenclatureDTO';
import { TLConfirmDialog } from '@app/shared/components/confirmation-dialog/tl-confirm-dialog';
import { TLValidators } from '@app/shared/utils/tl-validators';

@Component({
    selector: 'edit-observation-at-sea',
    templateUrl: './edit-observation-at-sea.component.html',
})
export class EditObservationAtSeaComponent extends BaseInspectionsComponent implements OnInit, IDialogComponent {
    protected model: InspectionObservationAtSeaDTO = new InspectionObservationAtSeaDTO();

    public onBoardObservationTools: InspectionObservationToolNomenclatureDTO[] = [];
    public centerObservationTools: InspectionObservationToolNomenclatureDTO[] = [];
    public onBoardActivities: InspectionVesselActivityNomenclatureDTO[] = [];
    public fishingActivities: InspectionVesselActivityNomenclatureDTO[] = [];
    public ships: ShipNomenclatureDTO[] = [];
    public institutions: NomenclatureDTO<number>[] = [];
    public vesselTypes: NomenclatureDTO<number>[] = [];
    public countries: NomenclatureDTO<number>[] = [];

    public hasOnBoardOtherSelected: boolean = false;
    public hasCenterOtherSelected: boolean = false;
    public fishingActivitySelected: boolean = false;
    public otherActivitySelected: boolean = false;
    public otherFishingActivitySelected: boolean = false;

    public constructor(
        service: InspectionsService,
        translate: FuseTranslationLoaderService,
        nomenclatures: CommonNomenclatures,
        confirmDialog: TLConfirmDialog,
        snackbar: MatSnackBar
    ) {
        super(service, translate, nomenclatures, confirmDialog, snackbar);
        this.inspectionCode = InspectionTypesEnum.OFS;
    }

    public async ngOnInit(): Promise<void> {
        if (this.viewMode) {
            this.form.disable();
        }

        const nomenclatureTables: (NomenclatureDTO<number>[] | InspectionObservationToolNomenclatureDTO[] | InspectionVesselActivityNomenclatureDTO[])[] = await forkJoin([
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.ObservationTools, this.nomenclatures.getObservationTools.bind(this.nomenclatures), false),
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.VesselActivities, this.nomenclatures.getVesselActivities.bind(this.nomenclatures), false),
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.Ships, this.nomenclatures.getShips.bind(this.nomenclatures), false),
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.Institutions, this.nomenclatures.getInstitutions.bind(this.nomenclatures), false),
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.VesselTypes, this.nomenclatures.getVesselTypes.bind(this.nomenclatures), false),
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.Countries, this.nomenclatures.getCountries.bind(this.nomenclatures), false)
        ]).toPromise();

        this.onBoardObservationTools = (nomenclatureTables[0] as InspectionObservationToolNomenclatureDTO[]).filter(x => x.onBoard === ObservationToolOnBoardEnum.OnBoard || x.onBoard === ObservationToolOnBoardEnum.Both);
        this.centerObservationTools = (nomenclatureTables[0] as InspectionObservationToolNomenclatureDTO[]).filter(x => x.onBoard === ObservationToolOnBoardEnum.Center || x.onBoard === ObservationToolOnBoardEnum.Both);
        this.onBoardActivities = (nomenclatureTables[1] as InspectionVesselActivityNomenclatureDTO[]).filter(x => x.isFishingActivity === false);
        this.fishingActivities = (nomenclatureTables[1] as InspectionVesselActivityNomenclatureDTO[]).filter(x => x.isFishingActivity === true);

        this.ships = nomenclatureTables[2];
        this.institutions = nomenclatureTables[3];
        this.vesselTypes = nomenclatureTables[4];
        this.countries = nomenclatureTables[5];

        if (this.id !== null && this.id !== undefined) {
            this.service.get(this.id, this.inspectionCode).subscribe({
                next: (inspection: InspectionObservationAtSeaDTO) => {
                    this.model = inspection;
                    this.fillForm();
                }
            });
        }
    }

    protected buildForm(): void {
        this.form = new FormGroup({
            generalInfoControl: new FormControl(undefined),
            patrolVehiclesControl: new FormControl([]),
            onBoardObservationToolsControl: new FormControl([]),
            otherOnBoardObservationToolsControl: new FormControl(undefined, Validators.maxLength(100)),
            shipControl: new FormControl(undefined),
            courseControl: new FormControl(undefined, Validators.maxLength(20)),
            speedControl: new FormControl(undefined, TLValidators.number(0)),
            centerObservationToolsControl: new FormControl([]),
            otherCenterObservationToolsControl: new FormControl(undefined, Validators.maxLength(100)),
            hasShipCommunicationControl: new FormControl(false),
            hasShipContactControl: new FormControl(false),
            onBoardActivitiesControl: new FormControl([]),
            otherOnBoardActivitiesControl: new FormControl(undefined, Validators.maxLength(4000)),
            fishingActivitiesControl: new FormControl([]),
            otherFishingActivitiesControl: new FormControl(undefined, Validators.maxLength(4000)),
            shipCommunicationControl: new FormControl(undefined, Validators.maxLength(4000)),
            additionalInfoControl: new FormControl(undefined),
            filesControl: new FormControl([])
        });

        this.form.get('generalInfoControl')!.valueChanges.subscribe({
            next: () => {
                this.reportNumAlreadyExistsError = false;
            }
        });

        this.form.get('onBoardObservationToolsControl')!.valueChanges.subscribe({
            next: this.onOnBoardObservationToolsChanged.bind(this)
        });

        this.form.get('centerObservationToolsControl')!.valueChanges.subscribe({
            next: this.onCenterObservationToolsChanged.bind(this)
        });

        this.form.get('onBoardActivitiesControl')!.valueChanges.subscribe({
            next: this.onOnBoardActivitiesChanged.bind(this)
        });

        this.form.get('fishingActivitiesControl')!.valueChanges.subscribe({
            next: this.onFishingActivitiesChanged.bind(this)
        });
    }

    protected fillForm(): void {
        if (this.id !== undefined && this.id !== null) {
            this.form.get('generalInfoControl')!.setValue(new InspectionGeneralInfoModel({
                reportNum: this.model.reportNum,
                startDate: this.model.startDate,
                endDate: this.model.endDate,
                inspectors: this.model.inspectors,
                byEmergencySignal: this.model.byEmergencySignal
            }));

            this.form.get('additionalInfoControl')!.setValue(new InspectionAdditionalInfoModel({
                actionsTaken: this.model.actionsTaken,
                administrativeViolation: this.model.administrativeViolation,
                inspectorComment: this.model.inspectorComment,
                violation: this.model.observationTexts?.find(x => x.category === InspectionObservationCategoryEnum.AdditionalInfo),
                violatedRegulations: this.model.violatedRegulations
            }));

            this.form.get('patrolVehiclesControl')!.setValue(this.model.patrolVehicles);
            this.form.get('filesControl')!.setValue(this.model.files);
            this.form.get('shipControl')!.setValue(this.model.observedVessel);
            this.form.get('shipCommunicationControl')!.setValue(this.model.shipCommunicationDescription);
            this.form.get('courseControl')!.setValue(this.model.course);
            this.form.get('speedControl')!.setValue(this.model.speed);
            this.form.get('hasShipCommunicationControl')!.setValue(this.model.hasShipCommunication);
            this.form.get('hasShipContactControl')!.setValue(this.model.hasShipContact);

            if (this.model.observationTools !== null && this.model.observationTools !== undefined) {
                this.form.get('onBoardObservationToolsControl')!.setValue(
                    this.onBoardObservationTools.filter(f => this.model.observationTools!.find(s => s.isOnBoard === true && s.observationToolId === f.value))
                );

                this.form.get('centerObservationToolsControl')!.setValue(
                    this.centerObservationTools.filter(f => this.model.observationTools!.find(s => s.isOnBoard === false && s.observationToolId === f.value))
                );

                const otherOnBoardId: number | undefined = this.onBoardObservationTools.find(x => x.code === 'Other')!.value;
                const otherCenterId: number | undefined = this.centerObservationTools.find(x => x.code === 'Other')!.value;

                const onBoardDesc: InspectionObservationToolDTO | undefined = this.model.observationTools.find(x => x.isOnBoard === true && x.observationToolId === otherOnBoardId);
                const centerDesc: InspectionObservationToolDTO | undefined = this.model.observationTools.find(x => x.isOnBoard === false && x.observationToolId === otherCenterId);

                if (onBoardDesc !== null && onBoardDesc !== undefined) {
                    this.form.get('otherOnBoardObservationToolsControl')!.setValue(onBoardDesc.description);
                    this.hasOnBoardOtherSelected = true;
                }

                if (centerDesc !== null && centerDesc !== undefined) {
                    this.form.get('otherCenterObservationToolsControl')!.setValue(centerDesc.description);
                    this.hasCenterOtherSelected = true;
                }
            }

            if (this.model.observedVesselActivities !== null && this.model.observedVesselActivities !== undefined) {
                this.form.get('onBoardActivitiesControl')!.setValue(
                    this.model.observedVesselActivities.filter(x => x.isFishingActivity === false)
                );

                const fishingId: number | undefined = this.onBoardActivities.find(x => x.code === 'Fishing')!.value;
                const otherOnBoardId: number | undefined = this.onBoardActivities.find(x => x.code === 'Other')!.value;
                const onBoardDesc: InspectionVesselActivityNomenclatureDTO | undefined = this.model.observedVesselActivities.find(x => x.isFishingActivity === false && x.value === otherOnBoardId);

                if (onBoardDesc !== null && onBoardDesc !== undefined) {
                    this.form.get('otherOnBoardActivitiesControl')!.setValue(onBoardDesc.description);
                    this.otherActivitySelected = true;
                }

                const fishing = this.model.observedVesselActivities.find(x => x.isFishingActivity === false && x.value === fishingId);

                if (fishing !== null && fishing !== undefined) {
                    this.fishingActivitySelected = true;
                    this.form.get('fishingActivitiesControl')!.setValue(this.model.observedVesselActivities.filter(x => x.isFishingActivity === true));

                    const otherFishingId: number | undefined = this.fishingActivities.find(x => x.code === 'OF')!.value;
                    const fishingDesc: InspectionVesselActivityNomenclatureDTO | undefined = this.model.observedVesselActivities.find(x => x.isFishingActivity === true && x.value === otherFishingId);

                    if (fishingDesc !== null && fishingDesc !== undefined) {
                        this.form.get('otherFishingActivitiesControl')!.setValue(fishingDesc.description);
                        this.otherFishingActivitySelected = true;
                    }
                }
            }
        }
    }

    protected fillModel(): void {
        const generalInfo: InspectionGeneralInfoModel = this.form.get('generalInfoControl')!.value;
        const additionalInfo: InspectionAdditionalInfoModel = this.form.get('additionalInfoControl')!.value;

        this.model.inspectionType = InspectionTypesEnum.OFS;
        this.model.isActive = true;
        this.model.startDate = generalInfo.startDate;
        this.model.endDate = generalInfo.endDate;
        this.model.inspectors = generalInfo.inspectors;
        this.model.reportNum = generalInfo.reportNum;
        this.model.byEmergencySignal = generalInfo?.byEmergencySignal;
        this.model.actionsTaken = additionalInfo?.actionsTaken;
        this.model.administrativeViolation = additionalInfo?.administrativeViolation === true;
        this.model.inspectorComment = additionalInfo?.inspectorComment;
        this.model.violatedRegulations = additionalInfo?.violatedRegulations;

        this.model.patrolVehicles = this.form.get('patrolVehiclesControl')!.value;
        this.model.files = this.form.get('filesControl')!.value;
        this.model.shipCommunicationDescription = this.form.get('shipCommunicationControl')!.value;
        this.model.observedVessel = this.form.get('shipControl')!.value;
        this.model.course = this.form.get('courseControl')!.value;
        this.model.hasShipCommunication = this.form.get('hasShipCommunicationControl')!.value;
        this.model.hasShipContact = this.form.get('hasShipContactControl')!.value;
        this.model.speed = this.form.get('speedControl')!.value;
        this.model.observationTools = this.mapObservationTools();
        this.model.observedVesselActivities = this.mapVesselActivities();

        this.model.observationTexts = [
            additionalInfo?.violation
        ].filter(x => x !== null && x !== undefined) as InspectionObservationTextDTO[];
    }

    private onOnBoardObservationToolsChanged(values: InspectionObservationToolNomenclatureDTO[]): void {
        if (values === null || values === undefined) {
            return;
        }

        if (values.findIndex(f => f.code === 'Other') >= 0) {
            this.hasOnBoardOtherSelected = true;
            this.form.get('otherOnBoardObservationToolsControl')!.enable();
        }
        else {
            this.hasOnBoardOtherSelected = false;
            this.form.get('otherOnBoardObservationToolsControl')!.disable();
        }

        if (this.viewMode === true) {
            this.form.get('otherOnBoardObservationToolsControl')!.disable();
        }
    }

    private onCenterObservationToolsChanged(values: InspectionObservationToolNomenclatureDTO[]): void {
        if (values === null || values === undefined) {
            return;
        }

        if (values.findIndex(f => f.code === 'Other') >= 0) {
            this.hasCenterOtherSelected = true;
            this.form.get('otherCenterObservationToolsControl')!.enable();
        }
        else {
            this.hasCenterOtherSelected = false;
            this.form.get('otherCenterObservationToolsControl')!.disable();
        }

        if (this.viewMode === true) {
            this.form.get('otherCenterObservationToolsControl')!.disable();
        }
    }

    private onOnBoardActivitiesChanged(values: InspectionObservationToolNomenclatureDTO[]): void {
        if (values === null || values === undefined) {
            return;
        }

        if (values.findIndex(f => f.code === 'Other') >= 0) {
            this.otherActivitySelected = true;
            this.form.get('otherOnBoardActivitiesControl')!.enable();
        }
        else {
            this.otherActivitySelected = false;
            this.form.get('otherOnBoardActivitiesControl')!.disable();
        }

        if (values.findIndex(f => f.code === 'Fishing') >= 0) {
            this.fishingActivitySelected = true;
            this.form.get('fishingActivitiesControl')!.enable();

            if (this.otherFishingActivitySelected === true) {
                this.form.get('otherFishingActivitiesControl')!.enable();
            }
        }
        else {
            this.fishingActivitySelected = false;
            this.form.get('fishingActivitiesControl')!.disable();
            this.form.get('otherFishingActivitiesControl')!.disable();
        }

        if (this.viewMode === true) {
            this.form.get('otherOnBoardActivitiesControl')!.disable();
            this.form.get('fishingActivitiesControl')!.disable();
            this.form.get('otherFishingActivitiesControl')!.disable();
        }
    }

    private onFishingActivitiesChanged(values: InspectionObservationToolNomenclatureDTO[]): void {
        if (values === null || values === undefined) {
            return;
        }

        if (values.findIndex(f => f.code === 'OF') >= 0) {
            this.otherFishingActivitySelected = true;
            this.form.get('otherFishingActivitiesControl')!.enable();
        }
        else {
            this.otherFishingActivitySelected = false;
            this.form.get('otherFishingActivitiesControl')!.disable();
        }

        if (this.viewMode === true) {
            this.form.get('otherFishingActivitiesControl')!.disable();
        }
    }

    private mapObservationTools(): InspectionObservationToolDTO[] {
        const tools: InspectionObservationToolDTO[] = [];

        for (const tool of this.form.get('centerObservationToolsControl')!.value as NomenclatureDTO<number>[]) {
            tools.push(new InspectionObservationToolDTO({
                isOnBoard: false,
                observationToolId: tool.value,
                description: tool.code === 'Other'
                    ? this.form.get('otherCenterObservationToolsControl')!.value
                    : undefined,
            }));
        }

        for (const tool of this.form.get('onBoardObservationToolsControl')!.value as NomenclatureDTO<number>[]) {
            tools.push(new InspectionObservationToolDTO({
                isOnBoard: true,
                observationToolId: tool.value,
                description: tool.code === 'Other'
                    ? this.form.get('otherOnBoardObservationToolsControl')!.value
                    : undefined,
            }));
        }

        return tools;
    }

    private mapVesselActivities(): InspectionVesselActivityNomenclatureDTO[] {
        const activities: InspectionVesselActivityNomenclatureDTO[] = [];

        for (const activity of this.form.get('onBoardActivitiesControl')!.value as NomenclatureDTO<number>[]) {
            activities.push(new InspectionVesselActivityNomenclatureDTO({
                value: activity.value,
                code: activity.code,
                displayName: activity.displayName,
                isFishingActivity: false,
                description: activity.code === 'Other'
                    ? this.form.get('otherCenterObservationToolsControl')!.value
                    : undefined,
            }));
        }

        if (this.fishingActivitySelected) {
            for (const activity of this.form.get('fishingActivitiesControl')!.value as NomenclatureDTO<number>[]) {
                activities.push(new InspectionVesselActivityNomenclatureDTO({
                    value: activity.value,
                    code: activity.code,
                    displayName: activity.displayName,
                    isFishingActivity: true,
                    description: activity.code === 'OF'
                        ? this.form.get('otherFishingActivitiesControl')!.value
                        : undefined,
                }));
            }
        }

        return activities;
    }
}