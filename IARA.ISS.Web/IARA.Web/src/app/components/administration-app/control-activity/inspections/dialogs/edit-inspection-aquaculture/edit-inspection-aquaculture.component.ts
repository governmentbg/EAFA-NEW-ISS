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
import { ShipNomenclatureDTO } from '@app/models/generated/dtos/ShipNomenclatureDTO';
import { TLConfirmDialog } from '@app/shared/components/confirmation-dialog/tl-confirm-dialog';
import { InspectionSubjectPersonnelDTO } from '@app/models/generated/dtos/InspectionSubjectPersonnelDTO';

@Component({
    selector: 'edit-inspection-aquaculture',
    templateUrl: './edit-inspection-aquaculture.component.html',
})
export class EditInspectionAquacultureComponent extends BaseInspectionsComponent implements OnInit, IDialogComponent {
    protected model: InspectionAquacultureDTO = new InspectionAquacultureDTO();

    public catchToggles: InspectionCheckModel[] = [];

    public institutions: NomenclatureDTO<number>[] = [];
    public countries: NomenclatureDTO<number>[] = [];
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
        confirmDialog: TLConfirmDialog,
        snackbar: MatSnackBar
    ) {
        super(service, translate, nomenclatures, confirmDialog, snackbar);
        this.inspectionCode = InspectionTypesEnum.IAQ;
    }

    public async ngOnInit(): Promise<void> {
        if (this.viewMode) {
            this.form.disable();
        }

        const nomenclatureTables = await forkJoin([
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.Institutions, this.nomenclatures.getInstitutions.bind(this.nomenclatures), false),
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.Countries, this.nomenclatures.getCountries.bind(this.nomenclatures), false),
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.Fishes, this.nomenclatures.getFishTypes.bind(this.nomenclatures), false),
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.CatchTypes, this.nomenclatures.getCatchInspectionTypes.bind(this.nomenclatures), false),
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.CatchZones, this.nomenclatures.getCatchZones.bind(this.nomenclatures), false),
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.FishSex, this.nomenclatures.getFishSex.bind(this.nomenclatures), false),
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.TurbotSizeGroups, this.nomenclatures.getTurbotSizeGroups.bind(this.nomenclatures), false),
            this.service.getAquacultures(),
            this.service.getCheckTypesForInspection(InspectionTypesEnum.IAQ)
        ]).toPromise();

        this.institutions = nomenclatureTables[0];
        this.countries = nomenclatureTables[1];
        this.fishes = nomenclatureTables[2];
        this.catchTypes = nomenclatureTables[3];
        this.catchZones = nomenclatureTables[4];
        this.fishSex = nomenclatureTables[5];
        this.turbotSizeGroups = nomenclatureTables[6];
        this.aquacultures = nomenclatureTables[7];

        this.catchToggles = nomenclatureTables[8].map(x => new InspectionCheckModel(x));

        if (this.id !== null && this.id !== undefined) {
            this.service.get(this.id, this.inspectionCode).subscribe({
                next: (value: InspectionAquacultureDTO) => {
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
            mapControl: new FormControl(undefined),
            aquacultureControl: new FormControl(undefined, Validators.required),
            ownerControl: new FormControl({ disabled: true }),
            representerControl: new FormControl(undefined),
            catchesControl: new FormControl([]),
            catchTogglesControl: new FormControl([]),
            otherFishingGearControl: new FormControl(undefined, Validators.maxLength(4000)),
            representativeControl: new FormControl(undefined, Validators.maxLength(4000)),
            additionalInfoControl: new FormControl(undefined),
            filesControl: new FormControl([])
        });

        this.form.get('generalInfoControl')!.valueChanges.subscribe({
            next: () => {
                this.reportNumAlreadyExistsError = false;
            }
        });

        this.form.get('aquacultureControl')!.valueChanges.subscribe({
            next: (aqua: NomenclatureDTO<number> | undefined) => {
                this.onAquacultureChanged(aqua);
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

        this.model.inspectionType = InspectionTypesEnum.IAQ;
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

        this.model.files = this.form.get('filesControl')!.value;
        this.model.checks = this.form.get('catchTogglesControl')!.value;
        this.model.location = this.form.get('mapControl')!.value;
        this.model.catchMeasures = this.form.get('catchesControl')!.value;
        this.model.aquacultureId = this.form.get('aquacultureControl')!.value?.value;
        this.model.representativeComment = this.form.get('representativeControl')!.value;
        this.model.patrolVehicles = this.form.get('patrolVehiclesControl')!.value;
        this.model.otherFishingGear = this.form.get('otherFishingGearControl')!.value;

        this.model.observationTexts = [
            additionalInfo?.violation,
        ].filter(x => x !== null && x !== undefined) as InspectionObservationTextDTO[];

        this.model.personnel = [
            this.form.get('representerControl')!.value,
        ].filter(x => x !== null && x !== undefined);
    }

    private onAquacultureChanged(aqua: NomenclatureDTO<number> | undefined): void {
        if (aqua !== undefined && aqua !== null && aqua instanceof NomenclatureDTO) {
            this.service.getAquacultureOwner(aqua.value!).subscribe({
                next: (owner: InspectionSubjectPersonnelDTO | undefined) => {
                    this.form.get('ownerControl')!.setValue(owner);
                }
            });
        }
        else {
            this.form.get('ownerControl')!.setValue(undefined);
        }
    }
}