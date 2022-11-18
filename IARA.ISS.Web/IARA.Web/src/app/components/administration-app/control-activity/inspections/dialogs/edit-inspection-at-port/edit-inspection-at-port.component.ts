import { Component, OnInit } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { FormControl, FormGroup } from '@angular/forms';
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
import { InspectionObservationCategoryEnum } from '@app/enums/inspection-observation-category.enum';
import { InspectionTransboardingDTO } from '@app/models/generated/dtos/InspectionTransboardingDTO';
import { InspectionTransboardingShipDTO } from '@app/models/generated/dtos/InspectionTransboardingShipDTO';
import { VesselDTO } from '@app/models/generated/dtos/VesselDTO';
import { InspectionCatchMeasureDTO } from '@app/models/generated/dtos/InspectionCatchMeasureDTO';
import { InspectionObservationTextDTO } from '@app/models/generated/dtos/InspectionObservationTextDTO';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { PortVisitDTO } from '@app/models/generated/dtos/PortVisitDTO';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';

@Component({
    selector: 'edit-inspection-at-port',
    templateUrl: './edit-inspection-at-port.component.html',
})
export class EditInspectionAtPortComponent extends BaseInspectionsComponent implements OnInit, IDialogComponent {
    protected model: InspectionTransboardingDTO = new InspectionTransboardingDTO();

    public institutions: NomenclatureDTO<number>[] = [];
    public countries: NomenclatureDTO<number>[] = [];
    public ports: NomenclatureDTO<number>[] = [];
    public fishes: NomenclatureDTO<number>[] = [];
    public catchTypes: NomenclatureDTO<number>[] = [];
    public catchZones: NomenclatureDTO<number>[] = [];
    public fishSex: NomenclatureDTO<number>[] = [];
    public ships: NomenclatureDTO<number>[] = [];
    public vesselTypes: NomenclatureDTO<number>[] = [];
    public associations: NomenclatureDTO<number>[] = [];
    public turbotSizeGroups: NomenclatureDTO<number>[] = [];

    public toggles: InspectionCheckModel[] = [];

    public hasTransshipment: boolean = false;

    public constructor(
        service: InspectionsService,
        translate: FuseTranslationLoaderService,
        nomenclatures: CommonNomenclatures,
        snackbar: MatSnackBar
    ) {
        super(service, translate, nomenclatures, snackbar);
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
                NomenclatureTypes.Countries, this.nomenclatures.getCountries.bind(this.nomenclatures), false
            ),
            NomenclatureStore.instance.getNomenclature(
                NomenclatureTypes.Ports, this.nomenclatures.getPorts.bind(this.nomenclatures), false
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
                NomenclatureTypes.Ships, this.nomenclatures.getShips.bind(this.nomenclatures), false
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
            this.service.getCheckTypesForInspection(InspectionTypesEnum.IBP)
        ]).toPromise();

        this.institutions = nomenclatureTables[0];
        this.countries = nomenclatureTables[1];
        this.ports = nomenclatureTables[2];
        this.fishes = nomenclatureTables[3];
        this.catchTypes = nomenclatureTables[4];
        this.catchZones = nomenclatureTables[5];
        this.fishSex = nomenclatureTables[6];
        this.ships = nomenclatureTables[7];
        this.vesselTypes = nomenclatureTables[8];
        this.associations = nomenclatureTables[9];
        this.turbotSizeGroups = nomenclatureTables[10];

        this.toggles = nomenclatureTables[11].map(f => new InspectionCheckModel(f));

        if (this.id !== null && this.id !== undefined) {
            this.service.get(this.id, this.inspectionCode).subscribe({
                next: (dto: InspectionTransboardingDTO) => {
                    this.model = dto;

                    this.fillForm();
                }
            });
        }
    }

    protected buildForm(): void {
        this.form = new FormGroup({
            generalInfoControl: new FormControl(undefined),
            portControl: new FormControl(undefined),
            shipSectionsControl: new FormControl([]),
            hasTransshipmentControl: new FormControl(false),
            transshipmentShipControl: new FormControl({ value: undefined, disabled: true }),
            transshipmentCatchesControl: new FormControl({ value: [], disabled: true }),
            transshipmentViolationControl: new FormControl(undefined),
            nnnShipStatusControl: new FormControl(undefined),
            captainCommentControl: new FormControl(undefined),
            additionalInfoControl: new FormControl(undefined),
            filesControl: new FormControl([])
        });

        this.form.get('hasTransshipmentControl')!.valueChanges.subscribe({
            next: this.onHasTransshipmentChanged.bind(this),
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

            this.form.get('filesControl')!.setValue(this.model.files);

            this.form.get('transshipmentCatchesControl')!.setValue(this.model.transboardedCatchMeasures);

            const transshipmentViolation = this.model.observationTexts?.find(f => f.category === InspectionObservationCategoryEnum.Transshipment);

            if (transshipmentViolation !== null && transshipmentViolation !== undefined) {
                this.form.get('transshipmentViolationControl')!.setValue(transshipmentViolation.text);
            }

            this.form.get('additionalInfoControl')!.setValue(new InspectionAdditionalInfoModel({
                actionsTaken: this.model.actionsTaken,
                administrativeViolation: this.model.administrativeViolation,
                inspectorComment: this.model.inspectorComment,
                violation: this.model.observationTexts?.find(f => f.category === InspectionObservationCategoryEnum.AdditionalInfo),
                violatedRegulations: this.model.violatedRegulations,
            }));

            if (this.model.receivingShipInspection !== null && this.model.receivingShipInspection !== undefined) {
                this.form.get('shipSectionsControl')!.setValue(new InspectedShipSectionsModel({
                    catches: this.model.receivingShipInspection.catchMeasures,
                    checks: this.model.receivingShipInspection.checks,
                    fishingGears: this.model.fishingGears,
                    logBooks: this.model.receivingShipInspection.logBooks,
                    observationTexts: this.model.observationTexts,
                    permitLicenses: this.model.receivingShipInspection.permitLicenses,
                    permits: this.model.receivingShipInspection.permits,
                    personnel: this.model.receivingShipInspection.personnel,
                    ship: this.model.receivingShipInspection.inspectedShip,
                }));

                this.form.get('portControl')!.setValue(this.model.receivingShipInspection.lastPortVisit);
                this.form.get('captainCommentControl')!.setValue(this.model.receivingShipInspection.captainComment);
            }

            if (this.model.sendingShipInspection !== null && this.model.sendingShipInspection !== undefined) {
                this.hasTransshipment = true;
                this.form.get('hasTransshipmentControl')!.setValue(true);
                this.form.get('transshipmentShipControl')!.setValue(this.model.sendingShipInspection.inspectedShip);
            }
        }
    }

    protected fillModel(): void {
        const generalInfo: InspectionGeneralInfoModel = this.form.get('generalInfoControl')!.value;
        const additionalInfo: InspectionAdditionalInfoModel = this.form.get('additionalInfoControl')!.value;
        const shipSections: InspectedShipSectionsModel = this.form.get('shipSectionsControl')!.value;
        const hasTransshipment: boolean = this.form.get('hasTransshipmentControl')!.value;
        const receivingShip: VesselDTO = this.form.get('transshipmentShipControl')!.value;
        const receivingShipCatches: InspectionCatchMeasureDTO[] = this.form.get('transshipmentCatchesControl')!.value;
        const transshipmentViolation: string = this.form.get('transshipmentViolationControl')!.value;
        const port: PortVisitDTO = this.form.get('portControl')!.value;

        this.model = new InspectionTransboardingDTO({
            id: this.model.id,
            startDate: generalInfo.startDate,
            endDate: generalInfo.endDate,
            inspectors: generalInfo.inspectors,
            files: this.form.get('filesControl')!.value,
            actionsTaken: additionalInfo?.actionsTaken,
            administrativeViolation: additionalInfo?.administrativeViolation === true,
            byEmergencySignal: generalInfo.byEmergencySignal,
            inspectionType: InspectionTypesEnum.IBP,
            inspectorComment: additionalInfo?.inspectorComment,
            violatedRegulations: additionalInfo?.violatedRegulations,
            isActive: true,
            fishingGears: shipSections.fishingGears,
            transboardedCatchMeasures: receivingShipCatches,
            observationTexts: [
                ...(shipSections.observationTexts ?? []),
                additionalInfo?.violation,
                !CommonUtils.isNullOrWhiteSpace(transshipmentViolation)
                    ? new InspectionObservationTextDTO({
                        category: InspectionObservationCategoryEnum.Transshipment,
                        text: transshipmentViolation
                    }) : undefined
            ].filter(f => f !== null && f !== undefined) as InspectionObservationTextDTO[],
            receivingShipInspection: new InspectionTransboardingShipDTO({
                captainComment: this.form.get('captainCommentControl')!.value,
                catchMeasures: shipSections.catches,
                checks: shipSections.checks,
                inspectedShip: shipSections.ship,
                lastPortVisit: port,
                logBooks: shipSections.logBooks,
                permitLicenses: shipSections.permitLicenses,
                permits: shipSections.permits,
                personnel: shipSections.personnel,
                nnnShipStatus: this.form.get('nnnShipStatusControl')!.value,
            }),
            sendingShipInspection: hasTransshipment
                ? new InspectionTransboardingShipDTO({
                    inspectedShip: receivingShip,
                }) : undefined,
        });
    }

    private onHasTransshipmentChanged(value: boolean): void {
        this.hasTransshipment = value;

        if (value && this.viewMode === false) {
            this.form.get('transshipmentShipControl')!.enable();
            this.form.get('transshipmentCatchesControl')!.enable();
            this.form.get('transshipmentViolationControl')!.enable();
        }
        else {
            this.form.get('transshipmentShipControl')!.disable();
            this.form.get('transshipmentCatchesControl')!.disable();
            this.form.get('transshipmentViolationControl')!.disable();
        }
    }
}