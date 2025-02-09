﻿import { Component, OnInit } from '@angular/core';
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
import { TLConfirmDialog } from '@app/shared/components/confirmation-dialog/tl-confirm-dialog';

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
    public isLegalOwner: boolean = false;

    public readonly inspectedPersonTypeEnum: typeof InspectedPersonTypeEnum = InspectedPersonTypeEnum;

    public constructor(
        service: InspectionsService,
        translate: FuseTranslationLoaderService,
        nomenclatures: CommonNomenclatures,
        confirmDialog: TLConfirmDialog,
        snackbar: MatSnackBar
    ) {
        super(service, translate, nomenclatures, confirmDialog, snackbar);
        this.inspectionCode = InspectionTypesEnum.IVH;
    }

    public async ngOnInit(): Promise<void> {
        if (this.viewMode) {
            this.form.disable();
        }

        const nomenclatureTables = await forkJoin([
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.Institutions, this.nomenclatures.getInstitutions.bind(this.nomenclatures), false),
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.TransportVehicleTypes, this.nomenclatures.getTransportVehicleTypes.bind(this.nomenclatures), false),
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.Countries, this.nomenclatures.getCountries.bind(this.nomenclatures), false),
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.Ships, this.nomenclatures.getShips.bind(this.nomenclatures), false),
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.Fishes, this.nomenclatures.getFishTypes.bind(this.nomenclatures), false),
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.CatchTypes, this.nomenclatures.getCatchInspectionTypes.bind(this.nomenclatures), false),
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.CatchZones, this.nomenclatures.getCatchZones.bind(this.nomenclatures), false),
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.CatchPresentations, this.nomenclatures.getCatchPresentations.bind(this.nomenclatures), false),
            this.service.getBuyers(),
            this.service.getCheckTypesForInspection(InspectionTypesEnum.IVH)
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
                next: (inspection: InspectionTransportVehicleDTO) => {
                    this.model = inspection;

                    this.fillForm();
                }
            });
        }
    }

    protected buildForm(): void {
        this.form = new FormGroup({
            generalInfoControl: new FormControl(undefined),
            mapControl: new FormControl(undefined),
            addressControl: new FormControl(undefined, Validators.maxLength(500)),
            typeControl: new FormControl(undefined, Validators.required),
            countryControl: new FormControl(undefined, Validators.required),
            vehicleRegNumControl: new FormControl(undefined, [Validators.required, Validators.maxLength(20)]),
            vehicleMarkControl: new FormControl(undefined, Validators.maxLength(50)),
            vehicleModelControl: new FormControl(undefined, Validators.maxLength(50)),
            vehicleTrailerNumControl: new FormControl(undefined, Validators.maxLength(20)),
            ownerIsDriverControl: new FormControl(false),
            ownerControl: new FormControl(undefined),
            driverControl: new FormControl(undefined),
            hasSealControl: new FormControl(true),
            sealInstitutionControl: new FormControl(undefined),
            sealConditionControl: new FormControl(undefined, Validators.maxLength(500)),
            buyerControl: new FormControl(undefined),
            destinationControl: new FormControl(undefined, Validators.maxLength(500)),
            catchesControl: new FormControl([]),
            catchTogglesControl: new FormControl([]),
            transporterCommentControl: new FormControl(undefined, Validators.maxLength(4000)),
            additionalInfoControl: new FormControl(undefined),
            filesControl: new FormControl([])
        }, InspectionUtils.atLeastOneCatchValidator());

        this.form.get('generalInfoControl')!.valueChanges.subscribe({
            next: () => {
                this.reportNumAlreadyExistsError = false;
            }
        });

        this.form.get('hasSealControl')!.valueChanges.subscribe({
            next: this.onHasSealChanged.bind(this)
        });

        this.form.get('ownerIsDriverControl')!.valueChanges.subscribe({
            next: (yes: boolean) => {
                if (yes) {
                    const driver: InspectionSubjectPersonnelDTO = this.form.get('ownerControl')!.value;
                    this.form.get('driverControl')!.setValue(driver);
                }

                this.form.get('driverControl')!.updateValueAndValidity({ emitEvent: false });
            }
        });

        this.form.get('ownerControl')!.valueChanges.subscribe({
            next: (owner: InspectionSubjectPersonnelDTO) => {
                if (owner !== undefined && owner !== null) {
                    this.isLegalOwner = owner.isLegal ?? false;
                }
                else {
                    this.isLegalOwner = false;
                }
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

            this.form.get('additionalInfoControl')!.setValue(new InspectionAdditionalInfoModel({
                actionsTaken: this.model.actionsTaken,
                administrativeViolation: this.model.administrativeViolation,
                inspectorComment: this.model.inspectorComment,
                violation: this.model.observationTexts?.find(f => f.category === InspectionObservationCategoryEnum.AdditionalInfo),
                violatedRegulations: this.model.violatedRegulations
            }));

            this.form.get('mapControl')!.setValue(this.model.inspectionLocation);
            this.form.get('addressControl')!.setValue(this.model.inspectionAddress);
            this.form.get('transporterCommentControl')!.setValue(this.model.transporterComment);
            this.form.get('typeControl')!.setValue(this.vehicleTypes.find(x => x.value === this.model.vehicleTypeId));
            this.form.get('countryControl')!.setValue(this.countries.find(x => x.value === this.model.countryId));
            this.form.get('vehicleRegNumControl')!.setValue(this.model.tractorLicensePlateNum);
            this.form.get('vehicleMarkControl')!.setValue(this.model.tractorBrand);
            this.form.get('vehicleModelControl')!.setValue(this.model.tractorModel);
            this.form.get('vehicleTrailerNumControl')!.setValue(this.model.trailerLicensePlateNum);
            this.form.get('destinationControl')!.setValue(this.model.transportDestination);
            this.form.get('sealInstitutionControl')!.setValue(this.institutions.find(f => f.value === this.model.sealInstitutionId));
            this.form.get('sealConditionControl')!.setValue(this.model.sealCondition);
            this.form.get('catchTogglesControl')!.setValue(this.model.checks);
            this.form.get('filesControl')!.setValue(this.model.files);

            this.hasSeal = this.model.isSealed === true;
            this.form.get('hasSealControl')!.setValue(this.model.isSealed);
            this.form.get('catchesControl')!.setValue(this.model.inspectionLogBookPages);

            if (this.model.personnel !== null && this.model.personnel !== undefined) {
                this.form.get('ownerControl')!.setValue(this.model.personnel.find(x => x.type === InspectedPersonTypeEnum.OwnerLegal || x.type === InspectedPersonTypeEnum.OwnerPers));

                const driver = this.model.personnel.find(x => x.type === InspectedPersonTypeEnum.Driver);

                if (driver !== undefined && driver !== null && driver.egnLnc !== null && driver.egnLnc !== undefined) {
                    this.form.get('driverControl')!.setValue(driver);
                }
                else {
                    this.form.get('ownerIsDriverControl')!.setValue(true);
                }

                const buyer = this.model.personnel.find(x => x.type === InspectedPersonTypeEnum.RegBuyer);

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
                        type: InspectedPersonTypeEnum.RegBuyer
                    }));
                }
            }
        }
    }

    protected fillModel(): void {
        const generalInfo: InspectionGeneralInfoModel = this.form.get('generalInfoControl')!.value;
        const additionalInfo: InspectionAdditionalInfoModel = this.form.get('additionalInfoControl')!.value;
        const buyer: InspectedBuyerNomenclatureDTO = this.form.get('buyerControl')!.value;

        this.model.inspectionType = InspectionTypesEnum.IVH;
        this.model.isActive = true;
        this.model.startDate = generalInfo?.startDate;
        this.model.endDate = generalInfo?.endDate;
        this.model.inspectors = generalInfo?.inspectors;
        this.model.reportNum = generalInfo?.reportNum;
        this.model.byEmergencySignal = generalInfo?.byEmergencySignal;
        this.model.actionsTaken = additionalInfo?.actionsTaken;
        this.model.administrativeViolation = additionalInfo?.administrativeViolation === true;
        this.model.inspectorComment = additionalInfo?.inspectorComment;
        this.model.violatedRegulations = additionalInfo?.violatedRegulations;

        this.model.files = this.form.get('filesControl')!.value;
        this.model.transporterComment = this.form.get('transporterCommentControl')!.value;
        this.model.checks = this.form.get('catchTogglesControl')!.value;
        this.model.inspectionAddress = this.form.get('addressControl')!.value;
        this.model.inspectionLocation = this.form.get('mapControl')!.value;
        this.model.vehicleTypeId = this.form.get('typeControl')!.value?.value;
        this.model.countryId = this.form.get('countryControl')!.value?.value;
        this.model.tractorLicensePlateNum = this.form.get('vehicleRegNumControl')!.value;
        this.model.tractorBrand = this.form.get('vehicleMarkControl')!.value;
        this.model.tractorModel = this.form.get('vehicleModelControl')!.value;
        this.model.trailerLicensePlateNum = this.form.get('vehicleTrailerNumControl')!.value;
        this.model.transportDestination = this.form.get('destinationControl')!.value;
        this.model.isSealed = this.form.get('hasSealControl')!.value;
        this.model.sealInstitutionId = this.form.get('sealInstitutionControl')!.value?.value;
        this.model.sealCondition = this.form.get('sealConditionControl')!.value;
        this.model.inspectionLogBookPages = this.form.get('catchesControl')!.value;
  
        this.model.observationTexts = [
            additionalInfo?.violation,
        ].filter(x => x !== null && x !== undefined) as InspectionObservationTextDTO[];

        this.model.personnel = [];
        const driver: InspectionSubjectPersonnelDTO | undefined = this.form.get('driverControl')!.value;

        if (driver !== undefined && driver !== null) {
            this.model.personnel.push(driver);
        }

        if (buyer !== undefined && buyer !== null) {
            this.model.personnel.push(new InspectionSubjectPersonnelDTO({
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
            }));
        }

        this.model.personnel.push(this.modifiedOwner());
    }

    private onHasSealChanged(value: boolean): void {
        this.hasSeal = value;
    }

    private modifiedOwner(): InspectionSubjectPersonnelDTO {
        if (this.form.controls.ownerIsDriverControl.value) {
            const owner: InspectionSubjectPersonnelDTO = this.form.get('ownerControl')!.value;

            return new InspectionSubjectPersonnelDTO({
                address: owner.address,
                citizenshipId: owner.citizenshipId,
                comment: owner.comment,
                egnLnc: owner.egnLnc,
                eik: owner.eik,
                entryId: owner.entryId,
                firstName: owner.firstName,
                hasBulgarianAddressRegistration: owner.hasBulgarianAddressRegistration,
                id: owner.id,
                isActive: owner.isActive,
                isLegal: owner.isLegal,
                isRegistered: owner.isRegistered,
                lastName: owner.lastName,
                middleName: owner.middleName,
                registeredAddress: owner.registeredAddress,
                type: InspectedPersonTypeEnum.OwnerPers
            });
        }
        else {
            return this.form.get('ownerControl')!.value;
        }
    }
}