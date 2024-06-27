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
import { InspectionCheckModel } from '../../models/inspection-check.model';
import { InspectionTypesEnum } from '@app/enums/inspection-types.enum';
import { InspectionAdditionalInfoModel } from '../../models/inspection-additional-info.model';
import { InspectedShipSectionsModel } from '../../models/inspected-ship-sections.model';
import { InspectionAtSeaDTO } from '@app/models/generated/dtos/InspectionAtSeaDTO';
import { InspectionObservationCategoryEnum } from '@app/enums/inspection-observation-category.enum';
import { InspectionObservationTextDTO } from '@app/models/generated/dtos/InspectionObservationTextDTO';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { ShipNomenclatureDTO } from '@app/models/generated/dtos/ShipNomenclatureDTO';
import { TLConfirmDialog } from '@app/shared/components/confirmation-dialog/tl-confirm-dialog';

@Component({
    selector: 'edit-inspection-at-sea',
    templateUrl: './edit-inspection-at-sea.component.html',
})
export class EditInspectionAtSeaComponent extends BaseInspectionsComponent implements OnInit, IDialogComponent {
    protected model: InspectionAtSeaDTO = new InspectionAtSeaDTO();

    public institutions: NomenclatureDTO<number>[] = [];
    public ships: ShipNomenclatureDTO[] = [];
    public fishes: NomenclatureDTO<number>[] = [];
    public catchTypes: NomenclatureDTO<number>[] = [];
    public catchZones: NomenclatureDTO<number>[] = [];
    public fishSex: NomenclatureDTO<number>[] = [];
    public countries: NomenclatureDTO<number>[] = [];
    public ports: NomenclatureDTO<number>[] = [];
    public vesselTypes: NomenclatureDTO<number>[] = [];
    public associations: NomenclatureDTO<number>[] = [];
    public turbotSizeGroups: NomenclatureDTO<number>[] = [];

    public toggles: InspectionCheckModel[] = [];

    public constructor(
        service: InspectionsService,
        translate: FuseTranslationLoaderService,
        nomenclatures: CommonNomenclatures,
        confirmDialog: TLConfirmDialog,
        snackbar: MatSnackBar
    ) {
        super(service, translate, nomenclatures, confirmDialog, snackbar);
        this.inspectionCode = InspectionTypesEnum.IBS;
    }

    public async ngOnInit(): Promise<void> {
        if (this.viewMode) {
            this.form.disable();
        }

        const nomenclatureTables = await forkJoin([
            NomenclatureStore.instance.getNomenclature(
                NomenclatureTypes.Institutions, this.nomenclatures.getInstitutions.bind(this.nomenclatures), false
            ),
            NomenclatureStore.instance.getNomenclature(
                NomenclatureTypes.Ships, this.nomenclatures.getShips.bind(this.nomenclatures), false
            ),
            NomenclatureStore.instance.getNomenclature(
                NomenclatureTypes.Fishes, this.nomenclatures.getFishTypes.bind(this.nomenclatures), false
            ),
            NomenclatureStore.instance.getNomenclature(
                NomenclatureTypes.CatchTypes, this.nomenclatures.getCatchInspectionTypes.bind(this.nomenclatures), false
            ),
            NomenclatureStore.instance.getNomenclature(
                NomenclatureTypes.CatchZones, this.nomenclatures.getCatchZones.bind(this.nomenclatures), false
            ),
            NomenclatureStore.instance.getNomenclature(
                NomenclatureTypes.FishSex, this.nomenclatures.getFishSex.bind(this.nomenclatures), false
            ),
            NomenclatureStore.instance.getNomenclature(
                NomenclatureTypes.Countries, this.nomenclatures.getCountries.bind(this.nomenclatures), false
            ),
            NomenclatureStore.instance.getNomenclature(
                NomenclatureTypes.Ports, this.nomenclatures.getPorts.bind(this.nomenclatures), false
            ),
            NomenclatureStore.instance.getNomenclature(
                NomenclatureTypes.VesselTypes, this.nomenclatures.getVesselTypes.bind(this.nomenclatures), false
            ),
            NomenclatureStore.instance.getNomenclature(
                NomenclatureTypes.ShipAssociations, this.nomenclatures.getShipAssociations.bind(this.nomenclatures), false
            ),
            NomenclatureStore.instance.getNomenclature(
                NomenclatureTypes.TurbotSizeGroups, this.nomenclatures.getTurbotSizeGroups.bind(this.nomenclatures), false
            ),
            this.service.getCheckTypesForInspection(InspectionTypesEnum.IBS),
        ]).toPromise();

        this.institutions = nomenclatureTables[0];
        this.ships = nomenclatureTables[1];
        this.fishes = nomenclatureTables[2];
        this.catchTypes = nomenclatureTables[3];
        this.catchZones = nomenclatureTables[4];
        this.fishSex = nomenclatureTables[5];
        this.countries = nomenclatureTables[6];
        this.ports = nomenclatureTables[7];
        this.vesselTypes = nomenclatureTables[8];
        this.associations = nomenclatureTables[9];
        this.turbotSizeGroups = nomenclatureTables[10];

        this.toggles = nomenclatureTables[11].map(f => new InspectionCheckModel(f));

        if (this.id !== null && this.id !== undefined) {
            this.service.get(this.id, this.inspectionCode).subscribe({
                next: (dto: InspectionAtSeaDTO) => {
                    this.model = dto;

                    this.fillForm();
                }
            });
        }
    }

    protected buildForm(): void {
        this.form = new FormGroup({
            generalInfoControl: new FormControl(undefined),
            patrolVehiclesControl: new FormControl([]),
            shipSectionsControl: new FormControl([]),
            captainCommentControl: new FormControl(undefined, Validators.maxLength(4000)),
            additionalInfoControl: new FormControl(undefined),
            filesControl: new FormControl([])
        });

        this.form.get('generalInfoControl')!.valueChanges.subscribe({
            next: () => {
                this.reportNumAlreadyExistsError = false;
            }
        });
    }

    protected fillForm(): void {
        if (this.id !== undefined && this.id !== null) {
            this.form.get('generalInfoControl')!.setValue(new InspectionGeneralInfoModel({
                reportNum: this.model.reportNum,
                startDate: this.model.startDate,
                endDate: this.model.endDate,
                inspectors: this.model.inspectors,
                byEmergencySignal: this.model.byEmergencySignal,
            }));

            this.form.get('patrolVehiclesControl')!.setValue(this.model.patrolVehicles);

            this.form.get('filesControl')!.setValue(this.model.files);

            this.form.get('additionalInfoControl')!.setValue(new InspectionAdditionalInfoModel({
                actionsTaken: this.model.actionsTaken,
                administrativeViolation: this.model.administrativeViolation,
                inspectorComment: this.model.inspectorComment,
                violation: this.model.observationTexts?.find(f => f.category === InspectionObservationCategoryEnum.AdditionalInfo),
                violatedRegulations: this.model.violatedRegulations,
            }));

            const ship: InspectedShipSectionsModel = this.mapModelToShipData();
            this.form.get('shipSectionsControl')!.setValue(ship);

            this.form.get('captainCommentControl')!.setValue(this.model.captainComment);
        }
    }

    protected fillModel(): void {
        const generalInfo: InspectionGeneralInfoModel = this.form.get('generalInfoControl')!.value;
        const additionalInfo: InspectionAdditionalInfoModel = this.form.get('additionalInfoControl')!.value;
        const shipSections: InspectedShipSectionsModel = this.form.get('shipSectionsControl')!.value;

        this.model = new InspectionAtSeaDTO({
            id: this.model.id,
            startDate: generalInfo.startDate,
            endDate: generalInfo.endDate,
            inspectors: generalInfo.inspectors,
            reportNum: generalInfo.reportNum,
            patrolVehicles: this.form.get('patrolVehiclesControl')!.value,
            files: this.form.get('filesControl')!.value,
            actionsTaken: additionalInfo?.actionsTaken,
            administrativeViolation: additionalInfo?.administrativeViolation === true,
            byEmergencySignal: generalInfo.byEmergencySignal,
            inspectionType: InspectionTypesEnum.IBS,
            inspectorComment: additionalInfo?.inspectorComment,
            violatedRegulations: additionalInfo?.violatedRegulations,
            isActive: true,
            captainComment: this.form.get('captainCommentControl')!.value,
            catchMeasures: shipSections?.catches,
            checks: shipSections?.checks,
            fishingGears: shipSections?.fishingGears,
            inspectedShip: shipSections?.ship,
            lastPortVisit: shipSections?.port,
            logBooks: shipSections?.logBooks,
            permitLicenses: shipSections?.permitLicenses,
            permits: shipSections?.permits,
            personnel: shipSections?.personnel,
            observationTexts: [
                ...(shipSections?.observationTexts ?? []),
                additionalInfo?.violation,
            ].filter(f => f !== null && f !== undefined) as InspectionObservationTextDTO[],
        });
    }

    private mapModelToShipData(): InspectedShipSectionsModel {
        const result: InspectedShipSectionsModel = new InspectedShipSectionsModel();

        result.ship = this.model.inspectedShip;
        result.port = this.model.lastPortVisit;
        result.catches = this.model.catchMeasures ?? [];
        result.checks = this.model.checks ?? [];
        result.fishingGears = this.model.fishingGears ?? [];
        result.logBooks = this.model.logBooks ?? [];
        result.observationTexts = this.model.observationTexts ?? [];
        result.permitLicenses = this.model.permitLicenses ?? [];
        result.permits = this.model.permits ?? [];
        result.personnel = this.model.personnel ?? [];

        return result;
    }
}