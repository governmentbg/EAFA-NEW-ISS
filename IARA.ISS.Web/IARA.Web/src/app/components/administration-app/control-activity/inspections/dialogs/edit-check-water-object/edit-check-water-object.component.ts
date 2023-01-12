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
import { InspectedPersonTypeEnum } from '@app/enums/inspected-person-type.enum';
import { InspectionCheckModel } from '../../models/inspection-check.model';
import { InspectionObservationTextDTO } from '@app/models/generated/dtos/InspectionObservationTextDTO';
import { InspectionCheckWaterObjectDTO } from '@app/models/generated/dtos/InspectionCheckWaterObjectDTO';

@Component({
    selector: 'edit-check-water-object',
    templateUrl: './edit-check-water-object.component.html',
})
export class EditCheckWaterObjectComponent extends BaseInspectionsComponent implements OnInit, IDialogComponent {
    protected model: InspectionCheckWaterObjectDTO = new InspectionCheckWaterObjectDTO();

    public toggles: InspectionCheckModel[] = [];

    public waterBodyTypes: NomenclatureDTO<number>[] = [];
    public institutions: NomenclatureDTO<number>[] = [];
    public fishes: NomenclatureDTO<number>[] = [];

    public readonly inspectedPersonTypeEnum: typeof InspectedPersonTypeEnum = InspectedPersonTypeEnum;

    public constructor(
        service: InspectionsService,
        translate: FuseTranslationLoaderService,
        nomenclatures: CommonNomenclatures,
        snackbar: MatSnackBar
    ) {
        super(service, translate, nomenclatures, snackbar);
        this.inspectionCode = InspectionTypesEnum.CWO;
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
                NomenclatureTypes.WaterBodyTypes, this.nomenclatures.getWaterBodyTypes.bind(this.nomenclatures), false
            ),
            NomenclatureStore.instance.getNomenclature(
                NomenclatureTypes.Fishes, this.nomenclatures.getFishTypes.bind(this.nomenclatures), false
            ),
            this.service.getCheckTypesForInspection(InspectionTypesEnum.CWO),
        ]).toPromise();

        this.institutions = nomenclatureTables[0];
        this.waterBodyTypes = nomenclatureTables[1];
        this.fishes = nomenclatureTables[2];

        this.toggles = nomenclatureTables[3].map(f => new InspectionCheckModel(f));

        if (this.id !== null && this.id !== undefined) {
            this.service.get(this.id, this.inspectionCode).subscribe({
                next: (dto: InspectionCheckWaterObjectDTO) => {
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
            nameControl: new FormControl(undefined, [Validators.required, Validators.maxLength(500)]),
            typeControl: new FormControl(undefined, Validators.required),
            togglesControl: new FormControl([]),
            mapControl: new FormControl(undefined),
            fishingGearsControl: new FormControl([]),
            vesselsControl: new FormControl([]),
            enginesControl: new FormControl([]),
            catchesControl: new FormControl([]),
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

            this.form.get('filesControl')!.setValue(this.model.files);

            this.form.get('additionalInfoControl')!.setValue(new InspectionAdditionalInfoModel({
                actionsTaken: this.model.actionsTaken,
                administrativeViolation: this.model.administrativeViolation,
                inspectorComment: this.model.inspectorComment,
                violation: this.model.observationTexts?.find(f => f.category === InspectionObservationCategoryEnum.AdditionalInfo),
                violatedRegulations: this.model.violatedRegulations,
            }));

            this.form.get('togglesControl')!.setValue(this.model.checks);

            this.form.get('patrolVehiclesControl')!.setValue(this.model.patrolVehicles);

            this.form.get('nameControl')!.setValue(this.model.objectName);

            this.form.get('typeControl')!.setValue(this.waterBodyTypes.find(f => f.value === this.model.waterObjectTypeId));

            this.form.get('mapControl')!.setValue(this.model.waterObjectLocation);

            this.form.get('fishingGearsControl')!.setValue(this.model.fishingGears);

            this.form.get('vesselsControl')!.setValue(this.model.vessels);

            this.form.get('enginesControl')!.setValue(this.model.engines);

            this.form.get('catchesControl')!.setValue(this.model.catches);
        }
    }

    protected fillModel(): void {
        const generalInfo: InspectionGeneralInfoModel = this.form.get('generalInfoControl')!.value;
        const additionalInfo: InspectionAdditionalInfoModel = this.form.get('additionalInfoControl')!.value;

        this.model = new InspectionCheckWaterObjectDTO({
            id: this.model.id,
            startDate: generalInfo.startDate,
            endDate: generalInfo.endDate,
            inspectors: generalInfo.inspectors,
            reportNum: generalInfo.reportNum,
            files: this.form.get('filesControl')!.value,
            actionsTaken: additionalInfo?.actionsTaken,
            administrativeViolation: additionalInfo?.administrativeViolation === true,
            byEmergencySignal: generalInfo.byEmergencySignal,
            inspectionType: InspectionTypesEnum.CWO,
            inspectorComment: additionalInfo?.inspectorComment,
            violatedRegulations: additionalInfo.violatedRegulations,
            isActive: true,
            checks: this.form.get('togglesControl')!.value,
            patrolVehicles: this.form.get('patrolVehiclesControl')!.value,
            objectName: this.form.get('nameControl')!.value,
            waterObjectTypeId: this.form.get('typeControl')!.value?.value,
            waterObjectLocation: this.form.get('mapControl')!.value,
            fishingGears: this.form.get('fishingGearsControl')!.value,
            vessels: this.form.get('vesselsControl')!.value,
            engines: this.form.get('enginesControl')!.value,
            catches: this.form.get('catchesControl')!.value,
            observationTexts: [
                additionalInfo?.violation,
            ].filter(f => f !== null && f !== undefined) as InspectionObservationTextDTO[],
        });
    }
}