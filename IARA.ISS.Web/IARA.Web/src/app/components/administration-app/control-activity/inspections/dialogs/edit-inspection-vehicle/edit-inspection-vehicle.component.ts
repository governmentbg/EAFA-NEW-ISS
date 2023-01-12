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
import { InspectionTransportVehicleDTO } from '@app/models/generated/dtos/InspectionTransportVehicleDTO';
import { InspectedBuyerNomenclatureDTO } from '@app/models/generated/dtos/InspectedBuyerNomenclatureDTO';
import { InspectionSubjectPersonnelDTO } from '@app/models/generated/dtos/InspectionSubjectPersonnelDTO';
import { InspectionUtils } from '@app/shared/utils/inspection.utils';

@Component({
    selector: 'edit-inspection-vehicle',
    templateUrl: './edit-inspection-vehicle.component.html',
})
export class EditInspectionVehicleComponent extends BaseInspectionsComponent implements OnInit, IDialogComponent {
    protected model: InspectionTransportVehicleDTO = new InspectionTransportVehicleDTO();

    public catchToggles: InspectionCheckModel[] = [];

    public institutions: NomenclatureDTO<number>[] = [];
    public vehicleTypes: NomenclatureDTO<number>[] = [];
    public countries: NomenclatureDTO<number>[] = [];
    public buyers: InspectedBuyerNomenclatureDTO[] = [];
    public ships: InspectedBuyerNomenclatureDTO[] = [];
    public fishes: NomenclatureDTO<number>[] = [];
    public catchTypes: NomenclatureDTO<number>[] = [];
    public catchZones: NomenclatureDTO<number>[] = [];
    public presentations: NomenclatureDTO<number>[] = [];

    public hasSeal: boolean = true;

    public readonly inspectedPersonTypeEnum: typeof InspectedPersonTypeEnum = InspectedPersonTypeEnum;

    public constructor(
        service: InspectionsService,
        translate: FuseTranslationLoaderService,
        nomenclatures: CommonNomenclatures,
        snackbar: MatSnackBar
    ) {
        super(service, translate, nomenclatures, snackbar);
        this.inspectionCode = InspectionTypesEnum.IVH;
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
                NomenclatureTypes.TransportVehicleTypes, this.nomenclatures.getTransportVehicleTypes.bind(this.nomenclatures), false
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
                NomenclatureTypes.CatchPresentations, this.nomenclatures.getCatchPresentations.bind(this.nomenclatures), false
            ),
            this.service.getBuyers(),
            this.service.getCheckTypesForInspection(InspectionTypesEnum.IVH),
        ]).toPromise();

        this.institutions = nomenclatureTables[0];
        this.vehicleTypes = nomenclatureTables[1];
        this.countries = nomenclatureTables[2];
        this.ships = nomenclatureTables[3];
        this.fishes = nomenclatureTables[4];
        this.catchTypes = nomenclatureTables[5];
        this.catchZones = nomenclatureTables[6];
        this.presentations = nomenclatureTables[7];
        this.buyers = nomenclatureTables[8];

        this.catchToggles = nomenclatureTables[9].map(f => new InspectionCheckModel(f));

        if (this.id !== null && this.id !== undefined) {
            this.service.get(this.id, this.inspectionCode).subscribe({
                next: (dto: InspectionTransportVehicleDTO) => {
                    this.model = dto;

                    this.fillForm();
                }
            });
        }
    }

    protected buildForm(): void {
        this.form = new FormGroup({
            generalInfoControl: new FormControl(undefined),
            mapControl: new FormControl(undefined),
            addressControl: new FormControl(undefined),
            typeControl: new FormControl(undefined, Validators.required),
            countryControl: new FormControl(undefined, Validators.required),
            vehicleRegNumControl: new FormControl(undefined),
            vehicleMarkControl: new FormControl(undefined),
            vehicleModelControl: new FormControl(undefined),
            vehicleTrailerNumControl: new FormControl(undefined),
            ownerControl: new FormControl(undefined),
            driverControl: new FormControl(undefined),
            hasSealControl: new FormControl(true),
            sealInstitutionControl: new FormControl(undefined),
            sealConditionControl: new FormControl(undefined),
            buyerControl: new FormControl(undefined),
            destinationControl: new FormControl(undefined),
            catchesControl: new FormControl([]),
            catchTogglesControl: new FormControl([]),
            transporterCommentControl: new FormControl(undefined),
            additionalInfoControl: new FormControl(undefined),
            filesControl: new FormControl([])
        });

        this.form.get('hasSealControl')!.valueChanges.subscribe({
            next: this.onHasSealChanged.bind(this)
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
            this.form.get('addressControl')!.setValue(this.model.inspectionAddress);

            this.form.get('transporterCommentControl')!.setValue(this.model.transporterComment);

            this.form.get('typeControl')!.setValue(this.vehicleTypes.find(f => f.value === this.model.vehicleTypeId));
            this.form.get('countryControl')!.setValue(this.countries.find(f => f.value === this.model.countryId));
            this.form.get('vehicleRegNumControl')!.setValue(this.model.tractorLicensePlateNum);
            this.form.get('vehicleMarkControl')!.setValue(this.model.tractorBrand);
            this.form.get('vehicleModelControl')!.setValue(this.model.tractorModel);
            this.form.get('vehicleTrailerNumControl')!.setValue(this.model.trailerLicensePlateNum);
            this.form.get('destinationControl')!.setValue(this.model.transportDestination);

            this.form.get('sealInstitutionControl')!.setValue(this.institutions.find(f => f.value === this.model.sealInstitutionId));
            this.form.get('sealConditionControl')!.setValue(this.model.sealCondition);

            this.form.get('catchTogglesControl')!.setValue(this.model.checks);

            this.hasSeal = this.model.isSealed === true;
            this.form.get('hasSealControl')!.setValue(this.model.isSealed);

            this.form.get('catchesControl')!.setValue(this.model.catchMeasures);

            if (this.model.personnel !== null && this.model.personnel !== undefined) {
                this.form.get('ownerControl')!.setValue(
                    this.model.personnel.find(f => f.type === InspectedPersonTypeEnum.OwnerLegal || f.type === InspectedPersonTypeEnum.OwnerPers)
                );
                this.form.get('driverControl')!.setValue(
                    this.model.personnel.find(f => f.type === InspectedPersonTypeEnum.Driver)
                );

                const buyer = this.model.personnel.find(f => f.type === InspectedPersonTypeEnum.RegBuyer);

                if (buyer !== null && buyer !== undefined) {
                    this.form.get('buyerControl')!.setValue(new InspectedBuyerNomenclatureDTO({
                        address: buyer.registeredAddress,
                        countryId: buyer.citizenshipId,
                        displayName: buyer.firstName
                            + (buyer.middleName !== null && buyer.middleName !== undefined ? ' ' + buyer.middleName : '')
                            + (buyer.lastName !== null && buyer.lastName !== undefined ? ' ' + buyer.lastName : ''),
                        egnLnc: buyer.egnLnc,
                        eik: buyer.eik,
                        entryId: buyer.entryId,
                        firstName: buyer.firstName,
                        isLegal: buyer.isLegal,
                        lastName: buyer.lastName,
                        middleName: buyer.middleName,
                        value: buyer.id,
                        type: InspectedPersonTypeEnum.RegBuyer,
                    }));
                }
            }
        }
    }

    protected fillModel(): void {
        const generalInfo: InspectionGeneralInfoModel = this.form.get('generalInfoControl')!.value;
        const additionalInfo: InspectionAdditionalInfoModel = this.form.get('additionalInfoControl')!.value;
        const buyer: InspectedBuyerNomenclatureDTO = this.form.get('buyerControl')!.value;

        this.model = new InspectionTransportVehicleDTO({
            id: this.model.id,
            startDate: generalInfo.startDate,
            endDate: generalInfo.endDate,
            inspectors: generalInfo.inspectors,
            reportNum: generalInfo.reportNum,
            files: this.form.get('filesControl')!.value,
            actionsTaken: additionalInfo?.actionsTaken,
            administrativeViolation: additionalInfo?.administrativeViolation === true,
            byEmergencySignal: generalInfo.byEmergencySignal,
            inspectionType: InspectionTypesEnum.IVH,
            inspectorComment: additionalInfo?.inspectorComment,
            violatedRegulations: additionalInfo?.violatedRegulations,
            isActive: true,
            transporterComment: this.form.get('transporterCommentControl')!.value,
            checks: this.form.get('catchTogglesControl')!.value,
            inspectionAddress: this.form.get('addressControl')!.value,
            inspectionLocation: this.form.get('mapControl')!.value,
            vehicleTypeId: this.form.get('typeControl')!.value?.value,
            countryId: this.form.get('countryControl')!.value?.value,
            tractorLicensePlateNum: this.form.get('vehicleRegNumControl')!.value,
            tractorBrand: this.form.get('vehicleMarkControl')!.value,
            tractorModel: this.form.get('vehicleModelControl')!.value,
            trailerLicensePlateNum: this.form.get('vehicleTrailerNumControl')!.value,
            transportDestination: this.form.get('destinationControl')!.value,
            isSealed: this.form.get('hasSealControl')!.value,
            sealInstitutionId: this.form.get('sealInstitutionControl')!.value?.value,
            sealCondition: this.form.get('sealConditionControl')!.value,
            catchMeasures: this.form.get('catchesControl')!.value,
            observationTexts: [
                additionalInfo?.violation,
            ].filter(f => f !== null && f !== undefined) as InspectionObservationTextDTO[],
            personnel: [
                buyer !== null && buyer !== undefined
                    ? new InspectionSubjectPersonnelDTO({
                        entryId: buyer.entryId,
                        citizenshipId: buyer.countryId,
                        egnLnc: buyer.egnLnc,
                        eik: buyer.eik,
                        isLegal: buyer.isLegal,
                        firstName: buyer.firstName,
                        middleName: buyer.middleName,
                        lastName: buyer.lastName,
                        registeredAddress: buyer.address,
                        isRegistered: true,
                        type: InspectedPersonTypeEnum.RegBuyer,
                        address: InspectionUtils.buildAddress(buyer.address, this.translate)
                    }) : undefined,
                this.form.get('ownerControl')!.value,
                this.form.get('driverControl')!.value,
            ].filter(f => f !== null && f !== undefined),
        });
    }

    private onHasSealChanged(value: boolean): void {
        this.hasSeal = value;
    }
}