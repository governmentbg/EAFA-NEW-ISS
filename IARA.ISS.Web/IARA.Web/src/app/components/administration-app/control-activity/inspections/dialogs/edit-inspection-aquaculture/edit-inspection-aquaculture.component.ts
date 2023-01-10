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
import { InspectionTypesEnum } from '@app/enums/inspection-types.enum';
import { InspectionAdditionalInfoModel } from '../../models/inspection-additional-info.model';
import { InspectionObservationCategoryEnum } from '@app/enums/inspection-observation-category.enum';
import { InspectedPersonTypeEnum } from '@app/enums/inspected-person-type.enum';
import { InspectionCheckModel } from '../../models/inspection-check.model';
import { InspectionObservationTextDTO } from '@app/models/generated/dtos/InspectionObservationTextDTO';
import { InspectionAquacultureDTO } from '@app/models/generated/dtos/InspectionAquacultureDTO';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';

@Component({
    selector: 'edit-inspection-aquaculture',
    templateUrl: './edit-inspection-aquaculture.component.html',
})
export class EditInspectionAquacultureComponent extends BaseInspectionsComponent implements OnInit, IDialogComponent {
    protected model: InspectionAquacultureDTO = new InspectionAquacultureDTO();

    public catchToggles: InspectionCheckModel[] = [];

    public institutions: NomenclatureDTO<number>[] = [];
    public countries: NomenclatureDTO<number>[] = [];
    public ships: NomenclatureDTO<number>[] = [];
    public fishes: NomenclatureDTO<number>[] = [];
    public catchTypes: NomenclatureDTO<number>[] = [];
    public catchZones: NomenclatureDTO<number>[] = [];
    public fishSex: NomenclatureDTO<number>[] = [];
    public aquacultures: NomenclatureDTO<number>[] = [];
    public turbotSizeGroups: NomenclatureDTO<number>[] = [];

    public readonly inspectedPersonTypeEnum: typeof InspectedPersonTypeEnum = InspectedPersonTypeEnum;

    public constructor(
        service: InspectionsService,
        translate: FuseTranslationLoaderService,
        nomenclatures: CommonNomenclatures,
        snackbar: MatSnackBar
    ) {
        super(service, translate, nomenclatures, snackbar);
        this.inspectionCode = InspectionTypesEnum.IAQ;
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
                NomenclatureTypes.TurbotSizeGroups, this.nomenclatures.getTurbotSizeGroups.bind(this.nomenclatures), false
            ),
            this.service.getAquacultures(),
            this.service.getCheckTypesForInspection(InspectionTypesEnum.IAQ),
        ]).toPromise();

        this.institutions = nomenclatureTables[0];
        this.countries = nomenclatureTables[1];
        this.ships = nomenclatureTables[2];
        this.fishes = nomenclatureTables[3];
        this.catchTypes = nomenclatureTables[4];
        this.catchZones = nomenclatureTables[5];
        this.fishSex = nomenclatureTables[6];
        this.turbotSizeGroups = nomenclatureTables[7];
        this.aquacultures = nomenclatureTables[8];

        this.catchToggles = nomenclatureTables[9].map(f => new InspectionCheckModel(f));

        if (this.id !== null && this.id !== undefined) {
            this.service.get(this.id, this.inspectionCode).subscribe({
                next: (dto: InspectionAquacultureDTO) => {
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
            mapControl: new FormControl(undefined),
            aquacultureControl: new FormControl(undefined, Validators.required),
            ownerControl: new FormControl({ disabled: true }),
            representerControl: new FormControl(undefined),
            catchesControl: new FormControl([]),
            catchTogglesControl: new FormControl([]),
            otherFishingGearControl: new FormControl(undefined),
            representativeControl: new FormControl(undefined),
            additionalInfoControl: new FormControl(undefined),
            filesControl: new FormControl([])
        });

        this.form.get('aquacultureControl')!.valueChanges.subscribe({
            next: this.onAquacultureChanged.bind(this)
        })
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

            this.form.get('additionalInfoControl')!.setValue(new InspectionAdditionalInfoModel({
                actionsTaken: this.model.actionsTaken,
                administrativeViolation: this.model.administrativeViolation,
                inspectorComment: this.model.inspectorComment,
                violation: this.model.observationTexts?.find(f => f.category === InspectionObservationCategoryEnum.AdditionalInfo),
                violatedRegulations: this.model.violatedRegulations,
            }));

            this.form.get('mapControl')!.setValue(this.model.location);

            this.form.get('aquacultureControl')!.setValue(this.aquacultures.find(f => f.value === this.model.aquacultureId));

            this.form.get('catchTogglesControl')!.setValue(this.model.checks);

            this.form.get('catchesControl')!.setValue(this.model.catchMeasures);

            this.form.get('representativeControl')!.setValue(this.model.representativeComment);

            this.form.get('patrolVehiclesControl')!.setValue(this.model.patrolVehicles);

            this.form.get('otherFishingGearControl')!.setValue(this.model.otherFishingGear);

            if (this.model.personnel !== null && this.model.personnel !== undefined) {
                this.form.get('ownerControl')!.setValue(
                    this.model.personnel.find(f => f.type === InspectedPersonTypeEnum.OwnerLegal || f.type === InspectedPersonTypeEnum.OwnerPers)
                );
                this.form.get('representerControl')!.setValue(
                    this.model.personnel.find(f => f.type === InspectedPersonTypeEnum.ReprsPers)
                );
            }
        }
    }

    protected fillModel(): void {
        const generalInfo: InspectionGeneralInfoModel = this.form.get('generalInfoControl')!.value;
        const additionalInfo: InspectionAdditionalInfoModel = this.form.get('additionalInfoControl')!.value;

        this.model = new InspectionAquacultureDTO({
            id: this.model.id,
            startDate: generalInfo.startDate,
            endDate: generalInfo.endDate,
            inspectors: generalInfo.inspectors,
            reportNum: generalInfo.reportNum,
            files: this.form.get('filesControl')!.value,
            actionsTaken: additionalInfo?.actionsTaken,
            administrativeViolation: additionalInfo?.administrativeViolation === true,
            byEmergencySignal: generalInfo.byEmergencySignal,
            inspectionType: InspectionTypesEnum.IAQ,
            inspectorComment: additionalInfo?.inspectorComment,
            violatedRegulations: additionalInfo?.violatedRegulations,
            isActive: true,
            checks: this.form.get('catchTogglesControl')!.value,
            location: this.form.get('mapControl')!.value,
            catchMeasures: this.form.get('catchesControl')!.value,
            aquacultureId: this.form.get('aquacultureControl')!.value?.value,
            representativeComment: this.form.get('representativeControl')!.value,
            patrolVehicles: this.form.get('patrolVehiclesControl')!.value,
            otherFishingGear: this.form.get('otherFishingGearControl')!.value,
            observationTexts: [
                additionalInfo?.violation,
            ].filter(f => f !== null && f !== undefined) as InspectionObservationTextDTO[],
            personnel: [
                this.form.get('representerControl')!.value,
            ].filter(f => f !== null && f !== undefined),
        });
    }

    private async onAquacultureChanged(aqua: NomenclatureDTO<number>): Promise<void> {
        if (aqua?.value !== null && aqua?.value !== undefined) {
            const owner = await this.service.getAquacultureOwner(aqua.value!).toPromise();

            this.form.get('ownerControl')!.setValue(owner);
        }
    }
}