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
import { InspectionTypesEnum } from '@app/enums/inspection-types.enum';
import { InspectionAdditionalInfoModel } from '../../models/inspection-additional-info.model';
import { InspectionObservationCategoryEnum } from '@app/enums/inspection-observation-category.enum';
import { InspectedPersonTypeEnum } from '@app/enums/inspected-person-type.enum';
import { InspectionCheckModel } from '../../models/inspection-check.model';
import { InspectionObservationTextDTO } from '@app/models/generated/dtos/InspectionObservationTextDTO';
import { InspectionFisherDTO } from '@app/models/generated/dtos/InspectionFisherDTO';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { TLValidators } from '@app/shared/utils/tl-validators';

@Component({
    selector: 'edit-inspection-person',
    templateUrl: './edit-inspection-person.component.html',
})
export class EditInspectionPersonComponent extends BaseInspectionsComponent implements OnInit, IDialogComponent {
    protected model: InspectionFisherDTO = new InspectionFisherDTO();

    public institutions: NomenclatureDTO<number>[] = [];
    public countries: NomenclatureDTO<number>[] = [];
    public ships: NomenclatureDTO<number>[] = [];
    public fishes: NomenclatureDTO<number>[] = [];
    public catchTypes: NomenclatureDTO<number>[] = [];
    public catchZones: NomenclatureDTO<number>[] = [];
    public fishSex: NomenclatureDTO<number>[] = [];
    public turbotSizeGroups: NomenclatureDTO<number>[] = [];

    public catchToggles: InspectionCheckModel[] = [];

    public hasTicket: boolean = true;

    public readonly inspectedPersonTypeEnum: typeof InspectedPersonTypeEnum = InspectedPersonTypeEnum;

    public constructor(
        service: InspectionsService,
        translate: FuseTranslationLoaderService,
        nomenclatures: CommonNomenclatures,
        snackbar: MatSnackBar
    ) {
        super(service, translate, nomenclatures, snackbar);
        this.inspectionCode = InspectionTypesEnum.IFP;
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
            this.service.getCheckTypesForInspection(InspectionTypesEnum.IFP),
        ]).toPromise();

        this.institutions = nomenclatureTables[0];
        this.countries = nomenclatureTables[1];
        this.ships = nomenclatureTables[2];
        this.fishes = nomenclatureTables[3];
        this.catchTypes = nomenclatureTables[4];
        this.catchZones = nomenclatureTables[5];
        this.fishSex = nomenclatureTables[6];
        this.turbotSizeGroups = nomenclatureTables[7];

        this.catchToggles = nomenclatureTables[8].map(f => new InspectionCheckModel(f));

        if (this.id !== null && this.id !== undefined) {
            this.service.get(this.id, this.inspectionCode).subscribe({
                next: (dto: InspectionFisherDTO) => {
                    this.model = dto;

                    this.fillForm();
                }
            });
        }
    }

    protected buildForm(): void {
        this.form = new FormGroup({
            generalInfoControl: new FormControl(undefined),
            patrolVehiclesControl: new FormControl(undefined),
            mapControl: new FormControl(undefined),
            addressControl: new FormControl(undefined, Validators.maxLength(500)),
            personControl: new FormControl(undefined),
            hasTicketControl: new FormControl(true),
            ticketControl: new FormControl(undefined, Validators.maxLength(50)),
            fishingGearCountControl: new FormControl(undefined, [Validators.required, TLValidators.number(0, undefined, 0)]),
            hookCountControl: new FormControl(undefined, [Validators.required, TLValidators.number(0, undefined, 0)]),
            catchesControl: new FormControl(undefined),
            catchTogglesControl: new FormControl(undefined),
            fishermanCommentControl: new FormControl(undefined, Validators.maxLength(4000)),
            additionalInfoControl: new FormControl(undefined),
            filesControl: new FormControl(undefined)
        });

        this.form.get('generalInfoControl')!.valueChanges.subscribe({
            next: () => {
                this.reportNumAlreadyExistsError = false;
            }
        });

        this.form.get('hasTicketControl')!.valueChanges.subscribe({
            next: this.onHasTicketChanged.bind(this)
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

            this.form.get('mapControl')!.setValue(this.model.inspectionLocation);

            this.form.get('catchTogglesControl')!.setValue(this.model.checks);

            this.form.get('catchesControl')!.setValue(this.model.catchMeasures);

            this.form.get('addressControl')!.setValue(this.model.inspectionAddress);

            this.form.get('patrolVehiclesControl')!.setValue(this.model.patrolVehicles);

            this.form.get('fishermanCommentControl')!.setValue(this.model.fishermanComment);

            this.form.get('fishingGearCountControl')!.setValue(this.model.fishingRodsCount);

            this.form.get('hookCountControl')!.setValue(this.model.fishingHooksCount);

            if (this.model.ticketNum !== null && this.model.ticketNum !== undefined) {
                this.hasTicket = true;
                this.form.get('hasTicketControl')!.setValue(true);
                this.form.get('ticketControl')!.setValue(this.model.ticketNum);
            }

            if (this.model.personnel !== null && this.model.personnel !== undefined) {
                this.form.get('personControl')!.setValue(
                    this.model.personnel.find(f => f.type === InspectedPersonTypeEnum.CaptFshmn)
                );
            }
        }
    }

    protected fillModel(): void {
        const generalInfo: InspectionGeneralInfoModel = this.form.get('generalInfoControl')!.value;
        const additionalInfo: InspectionAdditionalInfoModel = this.form.get('additionalInfoControl')!.value;

        this.model = new InspectionFisherDTO({
            id: this.model.id,
            startDate: generalInfo.startDate,
            endDate: generalInfo.endDate,
            inspectors: generalInfo.inspectors,
            reportNum: generalInfo.reportNum,
            files: this.form.get('filesControl')!.value,
            actionsTaken: additionalInfo?.actionsTaken,
            administrativeViolation: additionalInfo?.administrativeViolation === true,
            byEmergencySignal: generalInfo.byEmergencySignal,
            inspectionType: InspectionTypesEnum.IFP,
            inspectorComment: additionalInfo?.inspectorComment,
            violatedRegulations: additionalInfo?.violatedRegulations,
            isActive: true,
            checks: this.form.get('catchTogglesControl')!.value,
            inspectionLocation: this.form.get('mapControl')!.value,
            catchMeasures: this.form.get('catchesControl')!.value,
            patrolVehicles: this.form.get('patrolVehiclesControl')!.value,
            inspectionAddress: this.form.get('addressControl')!.value,
            fishermanComment: this.form.get('fishermanCommentControl')!.value,
            fishingHooksCount: this.form.get('hookCountControl')!.value,
            fishingRodsCount: this.form.get('fishingGearCountControl')!.value,
            ticketNum: this.hasTicket
                ? this.form.get('ticketControl')!.value
                : undefined,
            observationTexts: [
                additionalInfo?.violation,
            ].filter(f => f !== null && f !== undefined) as InspectionObservationTextDTO[],
            personnel: [
                this.form.get('personControl')!.value,
            ].filter(f => f !== null && f !== undefined),
        });
    }

    private onHasTicketChanged(value: boolean): void {
        this.hasTicket = value;
    }
}