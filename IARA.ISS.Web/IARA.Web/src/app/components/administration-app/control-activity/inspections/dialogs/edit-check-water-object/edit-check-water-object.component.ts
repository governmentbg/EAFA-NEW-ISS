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
import { TLConfirmDialog } from '@app/shared/components/confirmation-dialog/tl-confirm-dialog';

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
    public countries: NomenclatureDTO<number>[] = [];

    public hasOffender: boolean = true;

    public readonly inspectedPersonTypeEnum: typeof InspectedPersonTypeEnum = InspectedPersonTypeEnum;

    public constructor(
        service: InspectionsService,
        translate: FuseTranslationLoaderService,
        nomenclatures: CommonNomenclatures,
        confirmDialog: TLConfirmDialog,
        snackbar: MatSnackBar
    ) {
        super(service, translate, nomenclatures, confirmDialog, snackbar);
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
            NomenclatureStore.instance.getNomenclature(
                NomenclatureTypes.Countries, this.nomenclatures.getCountries.bind(this.nomenclatures), false
            ),
            this.service.getCheckTypesForInspection(InspectionTypesEnum.CWO),
        ]).toPromise();

        this.institutions = nomenclatureTables[0];
        this.waterBodyTypes = nomenclatureTables[1];
        this.fishes = nomenclatureTables[2];
        this.countries = nomenclatureTables[3];

        this.toggles = nomenclatureTables[4].map(x => new InspectionCheckModel(x));

        if (this.id !== null && this.id !== undefined) {
            this.service.get(this.id, this.inspectionCode).subscribe({
                next: (value: InspectionCheckWaterObjectDTO) => {
                    this.model = value;
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
            hasNoOffenderControl: new FormControl(false),
            offendersControl: new FormControl(undefined, Validators.required),
            fishingGearsControl: new FormControl([]),
            vesselsControl: new FormControl([]),
            enginesControl: new FormControl([]),
            catchesControl: new FormControl([]),
            additionalInfoControl: new FormControl(undefined),
            filesControl: new FormControl([])
        });

        this.form.get('generalInfoControl')!.valueChanges.subscribe({
            next: () => {
                this.reportNumAlreadyExistsError = false;
            }
        });

        this.form.get('hasNoOffenderControl')!.valueChanges.subscribe({
            next: (yes: boolean) => {
                if (yes) {
                    this.form.get('offendersControl')!.setValue(undefined);
                    this.form.get('offendersControl')!.clearValidators();
                    this.hasOffender = false;
                }
                else {
                    this.form.get('offendersControl')!.setValidators([Validators.required]);
                    this.hasOffender = true;
                }

                this.form.get('offendersControl')!.updateValueAndValidity({ emitEvent: false });
            }
        });
    }

    protected fillForm(): void {
        if (this.id !== undefined && this.id !== null) {
            const generalInfo: InspectionGeneralInfoModel = new InspectionGeneralInfoModel({
                reportNum: this.model.reportNum,
                startDate: this.model.startDate,
                endDate: this.model.endDate,
                inspectors: this.model.inspectors,
                byEmergencySignal: this.model.byEmergencySignal,
            });

            const additionalInfo: InspectionAdditionalInfoModel = new InspectionAdditionalInfoModel({
                actionsTaken: this.model.actionsTaken,
                administrativeViolation: this.model.administrativeViolation,
                inspectorComment: this.model.inspectorComment,
                violation: this.model.observationTexts?.find(x => x.category === InspectionObservationCategoryEnum.AdditionalInfo),
                violatedRegulations: this.model.violatedRegulations,
            });

            this.form.get('generalInfoControl')!.setValue(generalInfo);
            this.form.get('additionalInfoControl')!.setValue(additionalInfo);

            this.form.get('togglesControl')!.setValue(this.model.checks);
            this.form.get('patrolVehiclesControl')!.setValue(this.model.patrolVehicles);
            this.form.get('nameControl')!.setValue(this.model.objectName);
            this.form.get('typeControl')!.setValue(this.waterBodyTypes.find(x => x.value === this.model.waterObjectTypeId));
            this.form.get('mapControl')!.setValue(this.model.waterObjectLocation);
            this.form.get('fishingGearsControl')!.setValue(this.model.fishingGears);
            this.form.get('vesselsControl')!.setValue(this.model.vessels);
            this.form.get('enginesControl')!.setValue(this.model.engines);
            this.form.get('catchesControl')!.setValue(this.model.catches);
            this.form.get('filesControl')!.setValue(this.model.files);

            if (this.model.personnel !== undefined && this.model.personnel !== null && this.model.personnel.length > 0) {
                this.form.get('hasNoOffenderControl')!.setValue(false);
                this.form.get('offendersControl')!.setValue(this.model.personnel);
            }
            else {
                this.hasOffender = false;
                this.form.get('hasNoOffenderControl')!.setValue(true);
            }
        }
    }

    protected fillModel(): void {
        const generalInfo: InspectionGeneralInfoModel = this.form.get('generalInfoControl')!.value;
        const additionalInfo: InspectionAdditionalInfoModel = this.form.get('additionalInfoControl')!.value;

        this.model.inspectionType = InspectionTypesEnum.CWO;
        this.model.isActive = true;
        this.model.startDate = generalInfo?.startDate;
        this.model.endDate = generalInfo?.endDate;
        this.model.inspectors = generalInfo?.inspectors;
        this.model.reportNum = generalInfo?.reportNum;

        this.model.actionsTaken = additionalInfo?.actionsTaken;
        this.model.administrativeViolation = additionalInfo?.administrativeViolation === true;
        this.model.byEmergencySignal = generalInfo?.byEmergencySignal;
        this.model.inspectorComment = additionalInfo?.inspectorComment;
        this.model.violatedRegulations = additionalInfo?.violatedRegulations;

        this.model.checks = this.form.get('togglesControl')!.value;
        this.model.patrolVehicles = this.form.get('patrolVehiclesControl')!.value;
        this.model.objectName = this.form.get('nameControl')!.value;
        this.model.waterObjectTypeId = this.form.get('typeControl')!.value?.value;
        this.model.waterObjectLocation = this.form.get('mapControl')!.value;
        this.model.fishingGears = this.form.get('fishingGearsControl')!.value;
        this.model.vessels = this.form.get('vesselsControl')!.value;
        this.model.engines = this.form.get('enginesControl')!.value;
        this.model.catches = this.form.get('catchesControl')!.value;
        this.model.files = this.form.get('filesControl')!.value;

        this.model.observationTexts = [
            additionalInfo?.violation
        ].filter(x => x !== null && x !== undefined) as InspectionObservationTextDTO[];
   
        if (this.hasOffender) {
            this.model.personnel = this.form.get('offendersControl')!.value;
        }
        else {
            this.model.personnel = [];
        }
    }
}