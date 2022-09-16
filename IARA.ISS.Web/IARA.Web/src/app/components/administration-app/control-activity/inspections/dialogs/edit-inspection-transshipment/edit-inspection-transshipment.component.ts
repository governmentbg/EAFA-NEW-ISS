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
import { InspectionCatchMeasureDTO } from '@app/models/generated/dtos/InspectionCatchMeasureDTO';
import { InspectionObservationTextDTO } from '@app/models/generated/dtos/InspectionObservationTextDTO';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';

@Component({
    selector: 'edit-inspection-transshipment',
    templateUrl: './edit-inspection-transshipment.component.html',
})
export class EditInspectionTransshipmentComponent extends BaseInspectionsComponent implements OnInit, IDialogComponent {
    protected model: InspectionTransboardingDTO = new InspectionTransboardingDTO();

    public institutions: NomenclatureDTO<number>[] = [];
    public ships: NomenclatureDTO<number>[] = [];
    public fishes: NomenclatureDTO<number>[] = [];
    public catchTypes: NomenclatureDTO<number>[] = [];
    public catchZones: NomenclatureDTO<number>[] = [];
    public fishSex: NomenclatureDTO<number>[] = [];
    public countries: NomenclatureDTO<number>[] = [];
    public ports: NomenclatureDTO<number>[] = [];
    public vesselTypes: NomenclatureDTO<number>[] = [];
    public associations: NomenclatureDTO<number>[] = [];

    public toggles: InspectionCheckModel[] = [];

    public readonly inspectionObservationCategoryEnum: typeof InspectionObservationCategoryEnum = InspectionObservationCategoryEnum;

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
            this.service.getCheckTypesForInspection(InspectionTypesEnum.ITB)
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

        this.toggles = nomenclatureTables[10].map(f => new InspectionCheckModel(f));

        if (this.id !== null && this.id !== undefined) {
            this.service.get(this.id).subscribe({
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
            patrolVehiclesControl: new FormControl([]),
            sendingShipControl: new FormControl(undefined),
            receivingShipControl: new FormControl(undefined),
            transshipmentCatchesControl: new FormControl([]),
            transshipmentCatchViolationControl: new FormControl(undefined),
            sendingShipcaptainCommentControl: new FormControl(undefined),
            receivingShipcaptainCommentControl: new FormControl(undefined),
            additionalInfoControl: new FormControl(undefined),
            filesControl: new FormControl([])
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

            this.form.get('transshipmentCatchesControl')!.setValue(this.model.transboardedCatchMeasures);

            const transshipmentCatchViolation = this.model.observationTexts?.find(f => f.category === InspectionObservationCategoryEnum.TransshippedCatch);

            if (transshipmentCatchViolation !== null && transshipmentCatchViolation !== undefined) {
                this.form.get('transshipmentCatchViolationControl')!.setValue(transshipmentCatchViolation.text);
            }

            this.form.get('additionalInfoControl')!.setValue(new InspectionAdditionalInfoModel({
                actionsTaken: this.model.actionsTaken,
                administrativeViolation: this.model.administrativeViolation,
                inspectorComment: this.model.inspectorComment,
                violation: this.model.observationTexts?.find(f => f.category === InspectionObservationCategoryEnum.AdditionalInfo)
            }));

            const receivingShip = this.model.receivingShipInspection;

            if (receivingShip !== null && receivingShip !== undefined) {
                this.form.get('receivingShipControl')!.setValue(new InspectedShipSectionsModel({
                    catches: receivingShip.catchMeasures,
                    checks: receivingShip.checks,
                    fishingGears: this.model.fishingGears,
                    logBooks: receivingShip.logBooks,
                    observationTexts: this.model.observationTexts,
                    permits: receivingShip.permitLicenses,
                    personnel: receivingShip.personnel,
                    ship: receivingShip.inspectedShip,
                }));

                this.form.get('receivingShipcaptainCommentControl')!.setValue(receivingShip.captainComment);
            }

            const sendingShip = this.model.sendingShipInspection;

            if (sendingShip !== null && sendingShip !== undefined) {
                this.form.get('sendingShipControl')!.setValue(new InspectedShipSectionsModel({
                    catches: sendingShip.catchMeasures,
                    checks: sendingShip.checks,
                    fishingGears: this.model.fishingGears,
                    logBooks: sendingShip.logBooks,
                    observationTexts: this.model.observationTexts,
                    permits: sendingShip.permitLicenses,
                    personnel: sendingShip.personnel,
                    ship: sendingShip.inspectedShip,
                }));

                this.form.get('receivingShipcaptainCommentControl')!.setValue(sendingShip.captainComment);
            }
        }
    }

    protected fillModel(): void {
        const generalInfo: InspectionGeneralInfoModel = this.form.get('generalInfoControl')!.value;
        const additionalInfo: InspectionAdditionalInfoModel = this.form.get('additionalInfoControl')!.value;
        const sendingShip: InspectedShipSectionsModel = this.form.get('sendingShipControl')!.value;
        const receivingShip: InspectedShipSectionsModel = this.form.get('receivingShipControl')!.value;
        const transshipmentCatchViolation: string = this.form.get('transshipmentCatchViolationControl')!.value;
        const transshipmentCatches: InspectionCatchMeasureDTO[] = this.form.get('transshipmentCatchesControl')!.value;

        this.model = new InspectionTransboardingDTO({
            id: this.model.id,
            startDate: generalInfo.startDate,
            endDate: generalInfo.endDate,
            inspectors: generalInfo.inspectors,
            patrolVehicles: this.form.get('patrolVehiclesControl')!.value,
            files: this.form.get('filesControl')!.value,
            actionsTaken: additionalInfo?.actionsTaken,
            administrativeViolation: additionalInfo?.administrativeViolation === true,
            byEmergencySignal: generalInfo.byEmergencySignal,
            inspectionType: InspectionTypesEnum.ITB,
            inspectorComment: additionalInfo?.inspectorComment,
            isActive: true,
            fishingGears: sendingShip.fishingGears,
            transboardedCatchMeasures: transshipmentCatches,
            observationTexts: [
                ...(sendingShip.observationTexts ?? []),
                ...(receivingShip.observationTexts ?? []),
                additionalInfo?.violation,
                !CommonUtils.isNullOrWhiteSpace(transshipmentCatchViolation)
                    ? new InspectionObservationTextDTO({
                        category: InspectionObservationCategoryEnum.Transshipment,
                        text: transshipmentCatchViolation
                    }) : undefined
            ].filter(f => f !== null && f !== undefined) as InspectionObservationTextDTO[],
            receivingShipInspection: new InspectionTransboardingShipDTO({
                captainComment: this.form.get('receivingShipcaptainCommentControl')!.value,
                catchMeasures: receivingShip.catches,
                checks: receivingShip.checks,
                inspectedShip: receivingShip.ship,
                lastPortVisit: receivingShip.port,
                logBooks: receivingShip.logBooks,
                permitLicenses: receivingShip.permits,
                personnel: receivingShip.personnel,
            }),
            sendingShipInspection: new InspectionTransboardingShipDTO({
                captainComment: this.form.get('sendingShipcaptainCommentControl')!.value,
                catchMeasures: sendingShip.catches,
                checks: sendingShip.checks,
                inspectedShip: sendingShip.ship,
                lastPortVisit: sendingShip.port,
                logBooks: sendingShip.logBooks,
                permitLicenses: sendingShip.permits,
                personnel: sendingShip.personnel,
            }),
        });
    }
}