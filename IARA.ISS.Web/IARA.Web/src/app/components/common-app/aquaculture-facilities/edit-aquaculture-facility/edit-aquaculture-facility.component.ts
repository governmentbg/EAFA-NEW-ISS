import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { forkJoin, Observable, Subject } from 'rxjs';

import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { PageCodeEnum } from '@app/enums/page-code.enum';
import { IApplicationsService } from '@app/interfaces/administration-app/applications.interface';
import { IAquacultureFacilitiesService } from '@app/interfaces/common-app/aquaculture-facilities.interface';
import { ApplicationRegiXCheckDTO } from '@app/models/generated/dtos/ApplicationRegiXCheckDTO';
import { AquacultureApplicationEditDTO } from '@app/models/generated/dtos/AquacultureApplicationEditDTO';
import { AquacultureFacilityEditDTO } from '@app/models/generated/dtos/AquacultureFacilityEditDTO';
import { AquacultureRegixDataDTO } from '@app/models/generated/dtos/AquacultureRegixDataDTO';
import { PermissionsService } from '@app/shared/services/permissions.service';
import { ApplicationSubmittedByRegixDataDTO } from '@app/models/generated/dtos/ApplicationSubmittedByRegixDataDTO';
import { RegixPersonDataDTO } from '@app/models/generated/dtos/RegixPersonDataDTO';
import { ApplicationSubmittedForRegixDataDTO } from '@app/models/generated/dtos/ApplicationSubmittedForRegixDataDTO';
import { RegixLegalDataDTO } from '@app/models/generated/dtos/RegixLegalDataDTO';
import { PermissionsEnum } from '@app/shared/enums/permissions.enum';
import { ApplicationDialogData, ApplicationUtils } from '@app/shared/utils/application.utils';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { ApplicationContentDTO } from '@app/models/generated/dtos/ApplicationContentDTO';
import { RegixChecksWrapperDTO } from '@app/models/generated/dtos/RegixChecksWrapperDTO';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { AddressTypesEnum } from '@app/enums/address-types.enum';
import { AquacultureSalinityEnum } from '@app/enums/aquaculture-salinity.enum';
import { AquacultureTemperatureEnum } from '@app/enums/aquaculture-temperature.enum';
import { AquacultureSystemEnum } from '@app/enums/aquaculture-system.enum';
import { AquacultureCoordinateDTO } from '@app/models/generated/dtos/AquacultureCoordinateDTO';
import { TLDataTableComponent } from '@app/shared/components/data-table/tl-data-table.component';
import { CommonNomenclatures } from '@app/services/common-app/common-nomenclatures.service';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { AquacultureWaterLawCertificateDTO } from '@app/models/generated/dtos/AquacultureWaterLawCertificateDTO';
import { TLValidators } from '@app/shared/utils/tl-validators';
import { AquacultureInstallationDTO } from '@app/models/generated/dtos/AquacultureInstallationDTO';
import { GridRow } from '@app/shared/components/data-table/models/row.model';
import { TLConfirmDialog } from '@app/shared/components/confirmation-dialog/tl-confirm-dialog';
import { EditAquacultureInstallationDialogParams } from './models/edit-aquaculture-installation-dialog-params.model';
import { IHeaderAuditButton } from '@app/shared/components/dialog-wrapper/interfaces/header-audit-button.interface';
import { IS_PUBLIC_APP } from '@app/shared/modules/application.modules';
import { TLMatDialog } from '@app/shared/components/dialog-wrapper/tl-mat-dialog';
import { EditAquacultureInstallationComponent } from './edit-aquaculture-installation/edit-aquaculture-installation.component';
import { HeaderCloseFunction } from '@app/shared/components/dialog-wrapper/interfaces/header-cancel-button.interface';
import { PopulatedAreaNomenclatureExtendedDTO } from '@app/models/generated/dtos/PopulatedAreaNomenclatureExtendedDTO';
import { AquacultureInstallationEditDTO } from '@app/models/generated/dtos/AquacultureInstallationEditDTO';
import { AquacultureHatcheryEquipmentDTO } from '@app/models/generated/dtos/AquacultureHatcheryEquipmentDTO';
import { ErrorCode, ErrorModel } from '@app/models/common/exception.model';
import { RecordChangedEventArgs } from '@app/shared/components/data-table/models/record-changed-event.model';
import { CommandTypes } from '@app/shared/components/data-table/enums/command-type.enum';
import { AquacultureInstallationTypeEnum } from '@app/enums/aquaculture-installation-type.enum';
import { CommonDocumentDTO } from '@app/models/generated/dtos/CommonDocumentDTO';
import { CommonDocumentComponent } from '@app/shared/components/common-document/common-document.component';
import { CommonDocumentDialogParams } from '@app/shared/components/common-document/models/common-document-dialog-params.model';
import { EditAquacultureWaterLawCertificateComponent } from './edit-water-law-certificate/edit-aquaculture-water-law-certificate.component';
import { EditWaterLawCertificateDialogParams } from './models/edit-water-law-certificate-dialog-params.model';
import { UsageDocumentDTO } from '@app/models/generated/dtos/UsageDocumentDTO';
import { UsageDocumentComponent } from '@app/shared/components/usage-document/usage-document.component';
import { UsageDocumentRegixDataDTO } from '@app/models/generated/dtos/UsageDocumentRegixDataDTO';
import { UsageDocumentDialogParams } from '@app/shared/components/usage-document/models/usage-document-dialog-params.model';
import { EditAquacultureFacilityDialogParams } from './models/edit-aquaculture-facility-dialog-params.model';
import { AquacultureChangeOfCircumstancesApplicationDTO } from '@app/models/generated/dtos/AquacultureChangeOfCircumstancesApplicationDTO';
import { AquacultureDeregistrationApplicationDTO } from '@app/models/generated/dtos/AquacultureDeregistrationApplicationDTO';
import { ValidityCheckerGroupDirective } from '@app/shared/directives/validity-checker/validity-checker-group.directive';
import { AquacultureFacilitiesPublicService } from '@app/services/public-app/aquaculture-facilities-public.service';
import { ApplicationSubmittedByDTO } from '@app/models/generated/dtos/ApplicationSubmittedByDTO';
import { FileTypeEnum } from '@app/enums/file-types.enum';
import { Notifier } from '@app/shared/directives/notifier/notifier.class';
import { CancellationReasonGroupEnum } from '@app/enums/cancellation-reason-group.enum';
import { AquacultureStatusEnum } from '@app/enums/aquaculture-status.enum';
import { CancellationHistoryDialogComponent } from '@app/shared/components/cancellation-history-dialog/cancellation-history-dialog.component';
import { CancellationHistoryDialogParams } from '@app/shared/components/cancellation-history-dialog/cancellation-history-dialog-params.model';
import { CancellationHistoryEntryDTO } from '@app/models/generated/dtos/CancellationHistoryEntryDTO';
import { CommercialFishingAdministrationService } from '@app/services/administration-app/commercial-fishing-administration.service';
import { PermittedFileTypeDTO } from '@app/models/generated/dtos/PermittedFileTypeDTO';
import { FishNomenclatureDTO } from '@app/models/generated/dtos/FishNomenclatureDTO';

class AquaticOrganismTableModel {
    public aquaticOrganismId: number;
    public isActive: boolean = true;

    public constructor(aquaticOrganismId: number) {
        this.aquaticOrganismId = aquaticOrganismId;
    }
}

@Component({
    selector: 'edit-aquaculture-facility',
    templateUrl: './edit-aquaculture-facility.component.html',
    styleUrls: ['./edit-aquaculture-facility.component.scss']
})
export class EditAquacultureFacilityComponent implements OnInit, AfterViewInit, IDialogComponent {
    public form!: FormGroup;
    public coordinatesForm: FormGroup | undefined;
    public aquaticOrganismsForm: FormGroup | undefined;
    public hatcheryEquipmentGroup: FormGroup | undefined;

    public changeOfCircumstancesControl: FormControl = new FormControl();
    public deregistrationReasonControl: FormControl = new FormControl();

    public pageCode: PageCodeEnum = PageCodeEnum.AquaFarmReg;

    public readonly companyHeadquartersType: AddressTypesEnum = AddressTypesEnum.COMPANY_HEADQUARTERS;
    public readonly icIconSize: number = CommonUtils.IC_ICON_SIZE;

    public aquaticOrganisms: FishNomenclatureDTO[] = [];
    public allAquaticOrganisms: FishNomenclatureDTO[] = [];
    public unfilteredAquaticOrganisms: FishNomenclatureDTO[] = [];
    public waterLawCertificateTypes: NomenclatureDTO<number>[] = [];
    public usageDocumentTypes: NomenclatureDTO<number>[] = [];
    public waterSalinityTypes: NomenclatureDTO<AquacultureSalinityEnum>[] = [];
    public waterTemperatureTypes: NomenclatureDTO<AquacultureTemperatureEnum>[] = [];
    public systemTypes: NomenclatureDTO<AquacultureSystemEnum>[] = [];
    public powerSupplyTypes: NomenclatureDTO<number>[] = [];
    public territoryUnits: NomenclatureDTO<number>[] = [];
    public populatedAreas: PopulatedAreaNomenclatureExtendedDTO[] = [];
    public waterAreaTypes: NomenclatureDTO<number>[] = [];
    public hatcheryEquipmentTypes: NomenclatureDTO<number>[] = [];
    public installationTypes: NomenclatureDTO<number>[] = [];
    public statuses: NomenclatureDTO<number>[] = [];
    public cancellationReasons: NomenclatureDTO<number>[] = [];

    public cancellationHistory: CancellationHistoryEntryDTO[] = [];
    public aquacultureCoordinates: AquacultureCoordinateDTO[] = [];
    public aquacultureAquaticOrganisms: AquaticOrganismTableModel[] = [];
    public installations: AquacultureInstallationDTO[] = [];
    public hatcheryEquipments: AquacultureHatcheryEquipmentDTO[] = [];
    public usageDocuments: UsageDocumentDTO[] = [];
    public waterLawCertificates: AquacultureWaterLawCertificateDTO[] = [];
    public ovosCertificates: CommonDocumentDTO[] = [];
    public babhCertificates: CommonDocumentDTO[] = [];

    public canAddRecords: boolean = false;
    public canEditRecords: boolean = false;
    public canDeleteRecords: boolean = false;
    public canRestoreRecords: boolean = false;

    public notifier: Notifier = new Notifier();
    public expectedResults: AquacultureRegixDataDTO;
    public regixChecks: ApplicationRegiXCheckDTO[] = [];

    public service!: IAquacultureFacilitiesService;
    public isApplication: boolean = false;
    public viewMode: boolean = false;
    public isReadonly: boolean = false;
    public showOnlyRegiXData: boolean = false;
    public showRegiXData: boolean = false;
    public isChangeOfCircumstancesApplication: boolean = false;
    public isDeregistrationApplication: boolean = false;
    public isEditing: boolean = false;
    public isPaid: boolean = false;
    public hasDelivery: boolean = false;
    public isPublicApp: boolean;
    public isOnlineApplication: boolean = false;
    public refreshFileTypes: Subject<void> = new Subject<void>();
    public isRegisterEntry: boolean = false;
    public loadRegisterFromApplication: boolean = false;
    public hasNoEDeliveryRegistrationError: boolean = false;
    public showHatcheryEquipment: boolean = false;
    public hideBasicPaymentInfo: boolean = false;

    public aquaticOrganismsTouched: boolean = false;
    public installationsTouched: boolean = false;
    public usageDocumentsTouched: boolean = false;
    public waterLawCertificatesTouched: boolean = false;
    public ovosCertificatesTouched: boolean = false;
    public babhCertificatesTouched: boolean = false;

    @ViewChild('coordinatesTable')
    private coordinatesTable!: TLDataTableComponent;

    @ViewChild('aquaticOrganismsTable')
    private aquaticOrganismsTable!: TLDataTableComponent;

    @ViewChild('installationsTable')
    private installationsTable!: TLDataTableComponent;

    @ViewChild('hatcheryEquipmentTable')
    private hatcheryEquipmentTable!: TLDataTableComponent;

    @ViewChild('usageDocumentsTable')
    private usageDocumentsTable!: TLDataTableComponent;

    @ViewChild('waterLawCertificatesTable')
    private waterLawCertificatesTable!: TLDataTableComponent;

    @ViewChild('ovosCertificatesTable')
    private ovosCertificatesTable!: TLDataTableComponent;

    @ViewChild('babhCertificatesTable')
    private babhCertificatesTable!: TLDataTableComponent;

    @ViewChild(ValidityCheckerGroupDirective)
    private validityCheckerGroup!: ValidityCheckerGroupDirective;

    private id: number | undefined;
    private applicationId: number | undefined;
    private isApplicationHistoryMode: boolean = false;
    private dialogRightSideActions: IActionInfo[] | undefined;

    private applicationsService: IApplicationsService | undefined;
    private translate: FuseTranslationLoaderService;
    private nomenclatures!: CommonNomenclatures;
    private permissions: PermissionsService;
    private commercialFishingService: CommercialFishingAdministrationService;

    private allStatuses: NomenclatureDTO<number>[] = [];

    private readonly confirmDialog!: TLConfirmDialog;
    private readonly editInstallationDialog: TLMatDialog<EditAquacultureInstallationComponent>;
    private readonly editUsageDocumentDialog: TLMatDialog<UsageDocumentComponent>;
    private readonly editWaterLawCertificateDialog: TLMatDialog<EditAquacultureWaterLawCertificateComponent>;
    private readonly editOvosCertificateDialog: TLMatDialog<CommonDocumentComponent>;
    private readonly editBabhCertificateDialog: TLMatDialog<CommonDocumentComponent>;
    private readonly cancelDialog: TLMatDialog<CancellationHistoryDialogComponent>;

    private model!: AquacultureFacilityEditDTO | AquacultureApplicationEditDTO | AquacultureRegixDataDTO;

    private readonly internalFishCodes: string[] = ['4'];
    private readonly danubeFishCodes: string[] = ['2', '3', '4'];
    private readonly blackSeaFishCodes: string[] = ['1', '3', '4', '6'];

    public constructor(
        translate: FuseTranslationLoaderService,
        nomenclatures: CommonNomenclatures,
        commercialFishingService: CommercialFishingAdministrationService,
        confirmDialog: TLConfirmDialog,
        permissions: PermissionsService,
        editInstallationDialog: TLMatDialog<EditAquacultureInstallationComponent>,
        editUsageDocumentDialog: TLMatDialog<UsageDocumentComponent>,
        editWaterLawCertificateDialog: TLMatDialog<EditAquacultureWaterLawCertificateComponent>,
        editOvosCertificateDialog: TLMatDialog<CommonDocumentComponent>,
        editBabhCertificateDialog: TLMatDialog<CommonDocumentComponent>,
        cancelDialog: TLMatDialog<CancellationHistoryDialogComponent>
    ) {
        this.translate = translate;
        this.nomenclatures = nomenclatures;
        this.confirmDialog = confirmDialog;
        this.permissions = permissions;
        this.editInstallationDialog = editInstallationDialog;
        this.editUsageDocumentDialog = editUsageDocumentDialog;
        this.editWaterLawCertificateDialog = editWaterLawCertificateDialog;
        this.editOvosCertificateDialog = editOvosCertificateDialog;
        this.editBabhCertificateDialog = editBabhCertificateDialog;
        this.cancelDialog = cancelDialog;
        this.commercialFishingService = commercialFishingService;

        this.isPublicApp = IS_PUBLIC_APP;

        this.expectedResults = new AquacultureRegixDataDTO({
            submittedBy: new ApplicationSubmittedByRegixDataDTO({
                person: new RegixPersonDataDTO(),
                addresses: []
            }),
            submittedFor: new ApplicationSubmittedForRegixDataDTO({
                person: new RegixPersonDataDTO(),
                legal: new RegixLegalDataDTO(),
                addresses: []
            }),
            usageDocument: new UsageDocumentRegixDataDTO()
        });

        this.initEnumNomenclatures();
    }

    public async ngOnInit(): Promise<void> {
        if (!this.showOnlyRegiXData) {
            const nomenclatures: NomenclatureDTO<number>[][] = await forkJoin(
                NomenclatureStore.instance.getNomenclature(NomenclatureTypes.Fishes, this.nomenclatures.getFishTypes.bind(this.nomenclatures), false),
                NomenclatureStore.instance.getNomenclature(NomenclatureTypes.WaterLawCertificateTypes, this.service.getWaterLawCertificateTypes.bind(this.service), false),
                NomenclatureStore.instance.getNomenclature(NomenclatureTypes.UsageDocumentTypes, this.nomenclatures.getUsageDocumentTypes.bind(this.nomenclatures), false),
                NomenclatureStore.instance.getNomenclature(NomenclatureTypes.AquaculturePowerSupplyTypes, this.service.getAquaculturePowerSupplyTypes.bind(this.service), false),
                NomenclatureStore.instance.getNomenclature(NomenclatureTypes.TerritoryUnits, this.nomenclatures.getTerritoryUnits.bind(this.nomenclatures), false),
                NomenclatureStore.instance.getNomenclature(NomenclatureTypes.AquacultureWaterAreaTypes, this.service.getAquacultureWaterAreaTypes.bind(this.service), false),
                NomenclatureStore.instance.getNomenclature(NomenclatureTypes.PopulatedAreas, this.nomenclatures.getPopulatedAreas.bind(this.nomenclatures), false),
                NomenclatureStore.instance.getNomenclature(NomenclatureTypes.AquacultureHatcheryEquipmentTypes, this.service.getHatcheryEquipmentTypes.bind(this.service), false),
                NomenclatureStore.instance.getNomenclature(NomenclatureTypes.AquacultureInstallationTypes, this.service.getInstallationTypes.bind(this.service), false),
                NomenclatureStore.instance.getNomenclature(NomenclatureTypes.AquacultureStatusTypes, this.service.getAquacultureStatusTypes.bind(this.service), false),
                NomenclatureStore.instance.getNomenclature(NomenclatureTypes.CancellationReasons, this.nomenclatures.getCancellationReasons.bind(this.nomenclatures), false)
            ).toPromise();

            this.unfilteredAquaticOrganisms = this.allAquaticOrganisms = this.aquaticOrganisms = nomenclatures[0];
            this.waterLawCertificateTypes = nomenclatures[1];
            this.usageDocumentTypes = nomenclatures[2];
            this.powerSupplyTypes = nomenclatures[3];
            this.territoryUnits = nomenclatures[4];
            this.waterAreaTypes = nomenclatures[5];
            this.populatedAreas = nomenclatures[6];
            this.hatcheryEquipmentTypes = nomenclatures[7];
            this.installationTypes = nomenclatures[8];
            this.allStatuses = this.statuses = nomenclatures[9];
            this.cancellationReasons = nomenclatures[10];

            this.statuses = this.statuses.filter((status: NomenclatureDTO<number>) => {
                const notVisibleStatuses: AquacultureStatusEnum[] = [
                    AquacultureStatusEnum.Application,
                    AquacultureStatusEnum.Canceled
                ];

                const stat: AquacultureStatusEnum = AquacultureStatusEnum[status.code as keyof typeof AquacultureStatusEnum];
                return !notVisibleStatuses.includes(stat);
            });
        }

        // заявление за промяна на обстоятелствата
        if (this.isChangeOfCircumstancesApplication === true && this.applicationId !== undefined && this.applicationId !== null) {
            this.isRegisterEntry = true;
            this.fillForm();

            this.service.getApplication(this.applicationId, false, PageCodeEnum.AquaFarmChange).subscribe({
                next: (application: AquacultureChangeOfCircumstancesApplicationDTO) => {
                    this.changeOfCircumstancesControl.setValue(application.changes);
                    this.changeOfCircumstancesControl.disable();
                }
            });
        }
        // заявление за прекратяване на регистрацията
        else if (this.isDeregistrationApplication === true && this.applicationId !== undefined && this.applicationId !== null) {
            this.isRegisterEntry = true;
            this.fillForm();

            this.service.getApplication(this.applicationId, false, PageCodeEnum.AquaFarmDereg).subscribe({
                next: (application: AquacultureDeregistrationApplicationDTO) => {
                    this.deregistrationReasonControl.setValue(application.deregistrationReason);
                    this.deregistrationReasonControl.disable();
                }
            });
        }
        // извличане на исторически данни за заявление
        else if (this.isApplicationHistoryMode && this.applicationId !== undefined) {
            this.form.disable();

            if (this.applicationsService) {
                this.applicationsService.getApplicationChangeHistoryContent(this.applicationId).subscribe({
                    next: (content: ApplicationContentDTO) => {
                        const contentObject: Record<string, unknown> = JSON.parse(content.draftContent!);
                        const aquaculture: AquacultureApplicationEditDTO = new AquacultureApplicationEditDTO(contentObject);
                        aquaculture.files = content.files;
                        aquaculture.applicationId = content.applicationId;

                        this.isPaid = aquaculture.isPaid!;
                        this.hasDelivery = aquaculture.hasDelivery!;
                        this.hideBasicPaymentInfo = this.shouldHidePaymentData();
                        this.isOnlineApplication = aquaculture.isOnlineApplication!;

                        this.refreshFileTypes.next();

                        this.model = aquaculture;
                        this.fillForm();
                    }
                });
            }
        }
        else if (this.applicationId !== undefined && this.id === undefined && !this.isApplication) {
            // извличане на данни за регистров запис от id на заявление
            if (this.loadRegisterFromApplication === true) {
                this.isEditing = true;

                this.service.getRegisterByApplicationId(this.applicationId, this.pageCode).subscribe({
                    next: (aquaculture: unknown) => {
                        this.model = aquaculture as AquacultureFacilityEditDTO;
                        this.isOnlineApplication = (aquaculture as AquacultureFacilityEditDTO).isOnlineApplication!;
                        this.refreshFileTypes.next();
                        this.fillForm();
                    }
                });
            }
            else {
                // извличане на данни за създаване на регистров запис от заявление
                this.isEditing = false;

                this.service.getApplicationDataForRegister(this.applicationId, this.pageCode).subscribe({
                    next: (aquaculture: AquacultureFacilityEditDTO) => {
                        this.model = aquaculture;
                        this.isOnlineApplication = aquaculture.isOnlineApplication!;
                        this.refreshFileTypes.next();
                        this.fillForm();
                    }
                });
            }
        }
        else {
            if (this.isApplication && this.applicationId !== undefined) {
                // извличане на данни за RegiX сверка от служител
                this.isEditing = false;

                if (this.showOnlyRegiXData) {
                    this.service.getRegixData(this.applicationId, this.pageCode).subscribe({
                        next: (regixData: RegixChecksWrapperDTO<AquacultureRegixDataDTO>) => {
                            this.model = new AquacultureRegixDataDTO(regixData.dialogDataModel);
                            this.expectedResults = new AquacultureRegixDataDTO(regixData.regiXDataModel);

                            this.fillForm();
                        }
                    });
                }
                else {
                    // извличане на данни за заявление
                    this.isEditing = false;

                    this.service.getApplication(this.applicationId, this.showRegiXData, this.pageCode).subscribe({
                        next: (aquaculture: AquacultureApplicationEditDTO) => {
                            aquaculture.applicationId = this.applicationId;

                            this.isOnlineApplication = aquaculture.isOnlineApplication!;
                            this.isPaid = aquaculture.isPaid!;
                            this.hasDelivery = aquaculture.hasDelivery!;
                            this.hideBasicPaymentInfo = this.shouldHidePaymentData();
                            this.refreshFileTypes.next();

                            if (this.showRegiXData) {
                                this.expectedResults = new AquacultureRegixDataDTO(aquaculture.regiXDataModel);
                                aquaculture.regiXDataModel = undefined;
                            }

                            if (this.isPublicApp && this.isOnlineApplication && (aquaculture.submittedBy === undefined || aquaculture.submittedBy === null)) {
                                const service = this.service as AquacultureFacilitiesPublicService;
                                service.getCurrentUserAsSubmittedBy().subscribe({
                                    next: (submittedBy: ApplicationSubmittedByDTO) => {
                                        aquaculture.submittedBy = submittedBy;
                                        this.model = aquaculture;
                                        this.fillForm();
                                    }
                                });
                            }
                            else {
                                this.model = aquaculture;
                                this.fillForm();
                            }
                        }
                    });
                }
            }
            else if (this.id !== undefined) {
                // извличане на данни за регистров запис
                this.isEditing = true;
                this.isRegisterEntry = true;

                this.service.getAquaculture(this.id).subscribe({
                    next: (aquaculture: AquacultureFacilityEditDTO) => {
                        this.model = aquaculture;
                        this.isOnlineApplication = aquaculture.isOnlineApplication!;
                        this.refreshFileTypes.next();
                        this.fillForm();
                    }
                });
            }
        }

        if (this.isReadonly || this.viewMode) {
            this.form.disable();
        }
    }

    public ngAfterViewInit(): void {
        this.aquaticOrganismsForm?.get('aquaticOrganismIdControl')!.valueChanges.subscribe({
            next: () => {
                this.aquaticOrganisms = [...this.allAquaticOrganisms];
                const currentIds: number[] = this.aquaticOrganismsTable.rows.map(x => x.aquaticOrganismId);

                this.aquaticOrganisms = this.aquaticOrganisms.filter(x => !currentIds.includes(x.value!));
                this.aquaticOrganisms = this.aquaticOrganisms.slice();
            }
        });

        this.aquaticOrganismsTable?.recordChanged.subscribe({
            next: (event: RecordChangedEventArgs<AquaticOrganismTableModel>) => {
                this.aquaticOrganismsTouched = true;

                if (event.Command !== CommandTypes.Edit) {
                    this.form.updateValueAndValidity({ onlySelf: true });
                }
            }
        });
    }

    public setData(data: EditAquacultureFacilityDialogParams, wrapperData: DialogWrapperData): void {
        this.id = data.id;
        this.applicationId = data.applicationId;
        this.applicationsService = data.applicationsService;
        this.isApplication = data.isApplication;
        this.isApplicationHistoryMode = data.isApplicationHistoryMode;
        this.showOnlyRegiXData = data.showOnlyRegiXData;
        this.showRegiXData = data.showRegiXData;
        this.service = data.service as IAquacultureFacilitiesService;
        this.dialogRightSideActions = wrapperData.rightSideActions;
        this.pageCode = data.pageCode ?? PageCodeEnum.AquaFarmReg;
        this.loadRegisterFromApplication = data.loadRegisterFromApplication;

        if (!this.loadRegisterFromApplication) {
            this.isChangeOfCircumstancesApplication = data.isChangeOfCircumstancesApplication;
            this.isDeregistrationApplication = data.isDeregistrationApplication;

            if (this.isDeregistrationApplication) {
                this.isReadonly = true;
                this.viewMode = true;
            }
            else {
                this.isReadonly = data.isReadonly;
                this.viewMode = data.viewMode;
            }

            if (this.isChangeOfCircumstancesApplication === true) {
                this.pageCode = PageCodeEnum.AquaFarmChange;
            }
            else if (this.isDeregistrationApplication === true) {
                this.pageCode = PageCodeEnum.AquaFarmDereg;
            }
        }

        if (data.model !== undefined && data.model !== null) {
            this.model = data.model;
        }

        this.setPermissions();
        this.buildForm();
    }

    public saveBtnClicked(action: IActionInfo, dialogClose: DialogCloseCallback): void {
        this.markAllAsTouched();
        this.validityCheckerGroup.validate();

        if (this.form.valid) {
            this.save(dialogClose);
        }
    }

    public cancelBtnClicked(action: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public dialogButtonClicked(action: IActionInfo, dialogClose: DialogCloseCallback): void {
        if (action.id === 'print') {
            if (this.viewMode || this.isReadonly) {
                this.service.downloadAquacultureFacility(this.id!).subscribe({
                    next: () => {
                        // nothing to do
                    }
                });
            }
            else {
                this.editAndDownloadAquacultureFacility(dialogClose);
            }
        }
        else if (action.id === 'cancel' || action.id === 'activate') {
            if (this.isChangeOfCircumstancesApplication) {
                this.form.markAllAsTouched();
                this.validityCheckerGroup.validate();

                if (this.form.valid) {
                    this.fillModel();
                    CommonUtils.sanitizeModelStrings(this.model);

                    return this.openCancelDialog(action, dialogClose, action.id === 'cancel');
                }
            }
            else {
                return this.openCancelDialog(action, dialogClose, action.id === 'cancel');
            }
        }
        else {
            let applicationAction: boolean = false;

            if (this.model instanceof AquacultureApplicationEditDTO || this.model instanceof AquacultureRegixDataDTO) {
                this.fillModel();
                CommonUtils.sanitizeModelStrings(this.model);

                applicationAction = ApplicationUtils.applicationDialogButtonClicked(new ApplicationDialogData({
                    action: action,
                    dialogClose: dialogClose,
                    applicationId: this.applicationId,
                    model: this.model,
                    readOnly: this.isReadonly,
                    viewMode: this.viewMode,
                    editForm: this.form,
                    saveFn: this.saveAquaculture.bind(this),
                    onMarkAsTouched: () => {
                        this.validityCheckerGroup.validate();
                    }
                }));
            }

            if (!this.isReadonly && !this.viewMode && !applicationAction) {
                if (action.id === 'save') {
                    return this.saveBtnClicked(action, dialogClose);
                }
            }
        }
    }

    private editAndDownloadAquacultureFacility(dialogClose: DialogCloseCallback): void {
        this.form.markAllAsTouched();
        this.validityCheckerGroup.validate();

        if (this.form.valid) {
            this.fillModel();
            CommonUtils.sanitizeModelStrings(this.model);

            this.service.editAndDownloadAquaculture(this.model).subscribe({
                next: (download: boolean) => {
                    if (download) {
                        dialogClose(this.model);
                    }
                },
                error: (response: HttpErrorResponse) => {
                    this.handleSaveErrorResponse(response);
                }
            });
        }
    }

    public addEditInstallation(installation: AquacultureInstallationDTO | undefined, viewMode: boolean = false): void {
        let data: EditAquacultureInstallationDialogParams | undefined;
        let auditBtn: IHeaderAuditButton | undefined;
        let title: string;

        if (installation !== undefined) {
            data = new EditAquacultureInstallationDialogParams(this.service, installation, this.isReadonly || viewMode, this.isDraftMode());

            if (installation.id !== undefined && !IS_PUBLIC_APP) {
                auditBtn = {
                    id: installation.id,
                    getAuditRecordData: this.service.getInstallationAudit.bind(this.service),
                    tableName: 'AquacultureFacilityInstallation'
                };
            }

            if (this.isReadonly || viewMode) {
                title = this.translate.getValue('aquacultures.view-aquaculture-installation-dialog-title');
            }
            else {
                title = this.translate.getValue('aquacultures.edit-aquaculture-installation-dialog-title');
            }
        }
        else {
            data = new EditAquacultureInstallationDialogParams(this.service, undefined, false, this.isDraftMode());

            title = this.translate.getValue('aquacultures.add-aquaculture-installation-dialog-title');
        }

        const dialog = this.editInstallationDialog.openWithTwoButtons({
            title: title,
            TCtor: EditAquacultureInstallationComponent,
            headerAuditButton: auditBtn,
            headerCancelButton: {
                cancelBtnClicked: this.closeEditInstallationDialogBtnClicked.bind(this)
            },
            componentData: data,
            translteService: this.translate,
            viewMode: viewMode
        }, '1350px');

        dialog.subscribe({
            next: (result: AquacultureInstallationDTO | undefined) => {
                if (result !== undefined) {
                    // editing 
                    if (installation !== undefined) {
                        installation = result;
                    }
                    // adding
                    else {
                        this.installations.push(result);
                    }

                    this.installationsTouched = true;
                    this.form.updateValueAndValidity({ onlySelf: true });
                    this.installations = this.installations.slice();
                }
            }
        });
    }

    public deleteInstallation(installation: GridRow<AquacultureInstallationDTO>): void {
        this.confirmDialog.open({
            title: this.translate.getValue('aquacultures.delete-installation-dialog-title'),
            message: this.translate.getValue('aquacultures.delete-installation-dialog-message'),
            okBtnLabel: this.translate.getValue('aquacultures.delete-installation-dialog-ok-btn-label')
        }).subscribe({
            next: (ok: boolean) => {
                if (ok) {
                    this.installationsTable.softDelete(installation);

                    this.installationsTouched = true;
                    this.form.updateValueAndValidity({ onlySelf: true });
                }
            }
        });
    }

    public undoDeleteInstallation(installation: GridRow<AquacultureInstallationDTO>): void {
        this.confirmDialog.open().subscribe({
            next: (ok: boolean) => {
                if (ok) {
                    this.installationsTable.softUndoDelete(installation);

                    this.installationsTouched = true;
                    this.form.updateValueAndValidity({ onlySelf: true });
                }
            }
        });
    }

    public addEditUsageDocument(document: UsageDocumentDTO | undefined, viewMode: boolean = false): void {
        let data: UsageDocumentDialogParams | undefined;
        let auditBtn: IHeaderAuditButton | undefined;
        let title: string;

        if (document !== undefined) {
            data = new UsageDocumentDialogParams({
                model: document,
                isIdReadOnly: false,
                showOnlyRegiXData: this.showOnlyRegiXData,
                viewMode: this.isReadonly || viewMode
            });

            if (document.id !== undefined && !IS_PUBLIC_APP) {
                auditBtn = {
                    id: document.id,
                    getAuditRecordData: this.service.getUsageDocumentAudit.bind(this.service),
                    tableName: 'UsageDocument'
                };
            }

            if (this.isReadonly || viewMode) {
                title = this.translate.getValue('aquacultures.view-aquaculture-usage-document-dialog-title');
            }
            else {
                title = this.translate.getValue('aquacultures.edit-aquaculture-usage-document-dialog-title');
            }
        }
        else {
            data = new UsageDocumentDialogParams({
                model: undefined,
                showOnlyRegiXData: false,
                isIdReadOnly: false,
                viewMode: false
            });

            title = this.translate.getValue('aquacultures.add-aquaculture-usage-document-dialog-title');
        }

        const dialog = this.editUsageDocumentDialog.openWithTwoButtons({
            title: title,
            TCtor: UsageDocumentComponent,
            headerAuditButton: auditBtn,
            headerCancelButton: {
                cancelBtnClicked: this.closeEditUsageDocumentDialogBtnClicked.bind(this)
            },
            componentData: data,
            translteService: this.translate,
            viewMode: viewMode
        }, '1350px');

        dialog.subscribe({
            next: (result: UsageDocumentDTO | undefined) => {
                if (result !== undefined) {
                    // editing 
                    if (document !== undefined) {
                        const idx: number = this.usageDocuments.findIndex(x => x === document);
                        this.usageDocuments[idx] = result;
                    }
                    // adding
                    else {
                        this.usageDocuments.push(result);
                    }

                    this.usageDocumentsTouched = true;
                    this.form.updateValueAndValidity({ onlySelf: true });
                    this.usageDocuments = this.usageDocuments.slice();
                }
            }
        });
    }

    public deleteUsageDocument(document: GridRow<UsageDocumentDTO>): void {
        this.confirmDialog.open({
            title: this.translate.getValue('aquacultures.delete-usage-document-dialog-title'),
            message: this.translate.getValue('aquacultures.delete-usage-document-dialog-message'),
            okBtnLabel: this.translate.getValue('aquacultures.delete-usage-document-dialog-ok-btn-label')
        }).subscribe({
            next: (ok: boolean) => {
                if (ok) {
                    this.usageDocumentsTable.softDelete(document);

                    this.usageDocumentsTouched = true;
                    this.form.updateValueAndValidity({ onlySelf: true });
                }
            }
        });
    }

    public undoDeleteUsageDocument(document: GridRow<UsageDocumentDTO>): void {
        this.confirmDialog.open().subscribe({
            next: (ok: boolean) => {
                if (ok) {
                    this.usageDocumentsTable.softUndoDelete(document);

                    this.usageDocumentsTouched = true;
                    this.form.updateValueAndValidity({ onlySelf: true });
                }
            }
        });
    }

    public addEditWaterLawCertificate(certificate: AquacultureWaterLawCertificateDTO | undefined, viewMode: boolean = false): void {
        let data: EditWaterLawCertificateDialogParams | undefined;
        let auditBtn: IHeaderAuditButton | undefined;
        let title: string;

        if (certificate !== undefined) {
            data = new EditWaterLawCertificateDialogParams({
                service: this.service,
                model: certificate,
                viewMode: this.isReadonly || viewMode
            });

            if (certificate.id !== undefined && !IS_PUBLIC_APP) {
                auditBtn = {
                    id: certificate.id,
                    getAuditRecordData: this.service.getWaterLawCertificateAudit.bind(this.service),
                    tableName: 'AquacultureWaterLawCertificate'
                };
            }

            if (this.isReadonly || viewMode) {
                title = this.translate.getValue('aquacultures.view-aquaculture-water-law-certificate-dialog-title');
            }
            else {
                title = this.translate.getValue('aquacultures.edit-aquaculture-water-law-certificate-dialog-title');
            }
        }
        else {
            data = new EditWaterLawCertificateDialogParams({
                service: this.service,
                model: undefined,
                viewMode: false
            });

            title = this.translate.getValue('aquacultures.add-aquaculture-water-law-certificate-dialog-title');
        }

        const dialog = this.editWaterLawCertificateDialog.openWithTwoButtons({
            title: title,
            TCtor: EditAquacultureWaterLawCertificateComponent,
            headerAuditButton: auditBtn,
            headerCancelButton: {
                cancelBtnClicked: this.closeEditWaterLawCertificateDialogBtnClicked.bind(this)
            },
            componentData: data,
            translteService: this.translate,
            viewMode: viewMode
        }, '1350px');

        dialog.subscribe({
            next: (result: AquacultureWaterLawCertificateDTO | undefined) => {
                if (result !== undefined) {
                    // editing 
                    if (certificate !== undefined) {
                        const idx: number = this.waterLawCertificates.findIndex(x => x === certificate);
                        this.waterLawCertificates[idx] = result;
                    }
                    // adding
                    else {
                        this.waterLawCertificates.push(result);

                        this.waterLawCertificatesTouched = true;
                        this.form.updateValueAndValidity({ onlySelf: true });
                    }

                    this.waterLawCertificates = this.waterLawCertificates.slice();
                }
            }
        });
    }

    public deleteWaterLawCertificate(certificate: GridRow<AquacultureWaterLawCertificateDTO>): void {
        this.confirmDialog.open({
            title: this.translate.getValue('aquacultures.delete-water-law-certificate-dialog-title'),
            message: this.translate.getValue('aquaculture.delete-water-law-certificate-dialog-message'),
            okBtnLabel: this.translate.getValue('aquaculture.delete-water-law-certificate-dialog-ok-btn-label')
        }).subscribe({
            next: (ok: boolean) => {
                if (ok) {
                    this.waterLawCertificatesTable.softDelete(certificate);

                    this.waterLawCertificatesTouched = true;
                    this.form.updateValueAndValidity({ onlySelf: true });
                }
            }
        });
    }

    public undoDeleteWaterLawCertificate(certificate: GridRow<AquacultureWaterLawCertificateDTO>): void {
        this.confirmDialog.open().subscribe({
            next: (ok: boolean) => {
                if (ok) {
                    this.waterLawCertificatesTable.softUndoDelete(certificate);

                    this.waterLawCertificatesTouched = true;
                    this.form.updateValueAndValidity({ onlySelf: true });
                }
            }
        });
    }

    public addEditOvosCertificate(certificate: CommonDocumentDTO | undefined, viewMode: boolean = false): void {
        let data: CommonDocumentDialogParams | undefined;
        let auditBtn: IHeaderAuditButton | undefined;
        let title: string;

        if (certificate !== undefined) {
            data = new CommonDocumentDialogParams({
                model: certificate,
                viewMode: this.isReadonly || viewMode
            });

            if (certificate.id !== undefined && !IS_PUBLIC_APP) {
                auditBtn = {
                    id: certificate.id,
                    getAuditRecordData: this.service.getOvosCertificateAudit.bind(this.service),
                    tableName: 'AquacultureOvosCertificate'
                };
            }

            if (this.isReadonly || viewMode) {
                title = this.translate.getValue('aquacultures.view-aquaculture-ovos-certificate-dialog-title');
            }
            else {
                title = this.translate.getValue('aquacultures.edit-aquaculture-ovos-certificate-dialog-title');
            }
        }
        else {
            data = new CommonDocumentDialogParams({
                model: undefined,
                viewMode: false
            });

            title = this.translate.getValue('aquacultures.add-aquaculture-ovos-certificate-dialog-title');
        }

        const dialog = this.editOvosCertificateDialog.openWithTwoButtons({
            title: title,
            TCtor: CommonDocumentComponent,
            headerAuditButton: auditBtn,
            headerCancelButton: {
                cancelBtnClicked: this.closeEditOvosCertificateDialogBtnClicked.bind(this)
            },
            componentData: data,
            translteService: this.translate,
            viewMode: viewMode
        }, '1350px');

        dialog.subscribe({
            next: (result: CommonDocumentDTO | undefined) => {
                if (result !== undefined) {
                    // editing 
                    if (certificate !== undefined) {
                        const idx: number = this.ovosCertificates.findIndex(x => x === certificate);
                        this.ovosCertificates[idx] = result;
                    }
                    // adding
                    else {
                        this.ovosCertificates.push(result);

                        this.ovosCertificatesTouched = true;
                        this.form.updateValueAndValidity({ onlySelf: true });
                    }

                    this.ovosCertificates = this.ovosCertificates.slice();
                }
            }
        });
    }

    public deleteOvosCertificate(certificate: GridRow<CommonDocumentDTO>): void {
        this.confirmDialog.open({
            title: this.translate.getValue('aquacultures.delete-ovos-certificate-dialog-title'),
            message: this.translate.getValue('aquaculture.delete-ovos-certificate-dialog-message'),
            okBtnLabel: this.translate.getValue('aquaculture.delete-ovos-certificate-dialog-ok-btn-label')
        }).subscribe({
            next: (ok: boolean) => {
                if (ok) {
                    this.ovosCertificatesTable.softDelete(certificate);

                    this.ovosCertificatesTouched = true;
                    this.form.updateValueAndValidity({ onlySelf: true });
                }
            }
        });
    }

    public undoDeleteOvosCertificate(certificate: GridRow<CommonDocumentDTO>): void {
        this.confirmDialog.open().subscribe({
            next: (ok: boolean) => {
                if (ok) {
                    this.ovosCertificatesTable.softUndoDelete(certificate);

                    this.ovosCertificatesTouched = true;
                    this.form.updateValueAndValidity({ onlySelf: true });
                }
            }
        });
    }

    public addEditBabhCertificate(certificate: CommonDocumentDTO | undefined, viewMode: boolean = false): void {
        let data: CommonDocumentDialogParams | undefined;
        let auditBtn: IHeaderAuditButton | undefined;
        let title: string;

        if (certificate !== undefined) {
            data = new CommonDocumentDialogParams({
                model: certificate,
                viewMode: this.isReadonly || viewMode
            });

            if (certificate.id !== undefined && !IS_PUBLIC_APP) {
                auditBtn = {
                    id: certificate.id,
                    getAuditRecordData: this.service.getBabhCertificateAudit.bind(this.service),
                    tableName: 'AquacultureBabhCertificate'
                };
            }

            if (this.isReadonly || viewMode) {
                title = this.translate.getValue('aquacultures.view-aquaculture-babh-certificate-dialog-title');
            }
            else {
                title = this.translate.getValue('aquacultures.edit-aquaculture-babh-certificate-dialog-title');
            }
        }
        else {
            data = new CommonDocumentDialogParams({
                model: undefined,
                viewMode: false
            });

            title = this.translate.getValue('aquacultures.add-aquaculture-babh-certificate-dialog-title');
        }

        const dialog = this.editBabhCertificateDialog.openWithTwoButtons({
            title: title,
            TCtor: CommonDocumentComponent,
            headerAuditButton: auditBtn,
            headerCancelButton: {
                cancelBtnClicked: this.closeEditBabhCertificateDialogBtnClicked.bind(this)
            },
            componentData: data,
            translteService: this.translate,
            viewMode: viewMode
        }, '1350px');

        dialog.subscribe({
            next: (result: CommonDocumentDTO | undefined) => {
                if (result !== undefined) {
                    // editing 
                    if (certificate !== undefined) {
                        const idx: number = this.babhCertificates.findIndex(x => x === certificate);
                        this.babhCertificates[idx] = result;
                    }
                    // adding
                    else {
                        this.babhCertificates.push(result);

                        this.babhCertificatesTouched = true;
                        this.form.updateValueAndValidity({ onlySelf: true });
                    }

                    this.babhCertificates = this.babhCertificates.slice();
                }
            }
        });
    }

    public deleteBabhCertificate(certificate: GridRow<CommonDocumentDTO>): void {
        this.confirmDialog.open({
            title: this.translate.getValue('aquacultures.delete-babh-certificate-dialog-title'),
            message: this.translate.getValue('aquaculture.delete-babh-certificate-dialog-message'),
            okBtnLabel: this.translate.getValue('aquaculture.delete-babh-certificate-dialog-ok-btn-label')
        }).subscribe({
            next: (ok: boolean) => {
                if (ok) {
                    this.babhCertificatesTable.softDelete(certificate);

                    this.babhCertificatesTouched = true;
                    this.form.updateValueAndValidity({ onlySelf: true });
                }
            }
        });
    }

    public undoDeleteBabgCertificate(certificate: GridRow<CommonDocumentDTO>): void {
        this.confirmDialog.open().subscribe({
            next: (ok: boolean) => {
                if (ok) {
                    this.babhCertificatesTable.softUndoDelete(certificate);

                    this.babhCertificatesTouched = true;
                    this.form.updateValueAndValidity({ onlySelf: true });
                }
            }
        });
    }

    public fileTypeFilterFn(options: PermittedFileTypeDTO[]): PermittedFileTypeDTO[] {
        const pdfs: FileTypeEnum[] = [FileTypeEnum.SIGNEDAPPL, FileTypeEnum.APPLICATION_PDF];

        let result: PermittedFileTypeDTO[] = options;

        if (this.isApplication || !this.isOnlineApplication) {
            result = result.filter(x => !pdfs.includes(FileTypeEnum[x.code as keyof typeof FileTypeEnum]));
        }

        return result;
    }

    private closeEditInstallationDialogBtnClicked(closeFn: HeaderCloseFunction): void {
        closeFn();
    }

    private closeEditUsageDocumentDialogBtnClicked(closeFn: HeaderCloseFunction): void {
        closeFn();
    }

    private closeEditWaterLawCertificateDialogBtnClicked(closeFn: HeaderCloseFunction): void {
        closeFn();
    }

    private closeEditOvosCertificateDialogBtnClicked(closeFn: HeaderCloseFunction): void {
        closeFn();
    }

    private closeEditBabhCertificateDialogBtnClicked(closeFn: HeaderCloseFunction): void {
        closeFn();
    }

    private openCancelDialog(action: IActionInfo, dialogClose: DialogCloseCallback, cancelling: boolean): void {
        this.openConfirmDialogForApplication(() => {
            if (this.model instanceof AquacultureFacilityEditDTO) {
                const title: string = cancelling
                    ? this.translate.getValue('aquacultures.cancel-aquaculture-facility')
                    : this.translate.getValue('aquacultures.activate-aquaculture-facility');

                const dialog = this.cancelDialog.openWithTwoButtons({
                    title: title,
                    TCtor: CancellationHistoryDialogComponent,
                    headerCancelButton: {
                        cancelBtnClicked: this.closeCancelDialogBtnClicked.bind(this)
                    },
                    translteService: this.translate,
                    componentData: new CancellationHistoryDialogParams({
                        group: this.model.status !== AquacultureStatusEnum.Canceled
                            ? CancellationReasonGroupEnum.AquaCancel
                            : CancellationReasonGroupEnum.AquaActivate,
                        cancelling: this.model.status !== AquacultureStatusEnum.Canceled,
                        statuses: this.model.cancellationHistory
                    })
                }, '1200px');

                dialog.subscribe((result: CancellationHistoryEntryDTO | undefined) => {
                    if (result !== undefined && result !== null) {
                        if (this.isChangeOfCircumstancesApplication === true) {
                            this.service.updateAquacultureStatus(this.model.id!, result).subscribe({
                                next: () => {
                                    this.completeChangeOfCircumstancesApplication(dialogClose);
                                }
                            });
                        }
                        else if (this.isDeregistrationApplication) {
                            this.service.updateAquacultureStatus(this.model.id!, result, this.model.applicationId!).subscribe({
                                next: () => {
                                    dialogClose(this.model);
                                }
                            });
                        }
                        else {
                            this.service.updateAquacultureStatus(this.model.id!, result).subscribe({
                                next: () => {
                                    dialogClose(this.model);
                                }
                            });
                        }
                    }
                });
            }
        });
    }

    private completeChangeOfCircumstancesApplication(dialogClose: DialogCloseCallback): void {
        this.service.completeChangeOfCircumstancesApplication(this.model).subscribe({
            next: () => {
                dialogClose(this.model);
            },
            error: (response: HttpErrorResponse) => {
                this.handleSaveErrorResponse(response);
            }
        });
    }

    private closeCancelDialogBtnClicked(closeFn: HeaderCloseFunction): void {
        closeFn();
    }

    private setPermissions(): void {
        this.canAddRecords = this.isApplication
            ? this.permissions.has(PermissionsEnum.AquacultureFacilitiesApplicationsAddRecords)
            : this.permissions.has(PermissionsEnum.AquacultureFacilitiesAddRecords);

        this.canEditRecords = this.isApplication
            ? this.permissions.has(PermissionsEnum.AquacultureFacilitiesApplicationsEditRecords)
            : this.permissions.has(PermissionsEnum.AquacultureFacilitiesEditRecords);

        this.canDeleteRecords = this.isApplication
            ? this.permissions.has(PermissionsEnum.AquacultureFacilitiesApplicationsDeleteRecords)
            : this.permissions.has(PermissionsEnum.AquacultureFacilitiesDeleteRecords);

        this.canRestoreRecords = this.isApplication
            ? this.permissions.has(PermissionsEnum.AquacultureFacilitiesApplicationsRestoreRecords)
            : this.permissions.has(PermissionsEnum.AquacultureFacilitiesRestoreRecords);
    }

    private initEnumNomenclatures(): void {
        this.waterSalinityTypes = [
            new NomenclatureDTO<AquacultureSalinityEnum>({
                value: AquacultureSalinityEnum.Freshwater,
                displayName: this.translate.getValue('aquacultures.salinity-fresh'),
                isActive: true
            }),
            new NomenclatureDTO<AquacultureSalinityEnum>({
                value: AquacultureSalinityEnum.Saltwater,
                displayName: this.translate.getValue('aquacultures.salinity-salt'),
                isActive: true
            })
        ];

        this.waterTemperatureTypes = [
            new NomenclatureDTO<AquacultureTemperatureEnum>({
                value: AquacultureTemperatureEnum.Cold,
                displayName: this.translate.getValue('aquacultures.temperature-cold'),
                isActive: true
            }),
            new NomenclatureDTO<AquacultureTemperatureEnum>({
                value: AquacultureTemperatureEnum.Warm,
                displayName: this.translate.getValue('aquacultures.temperature-warm'),
                isActive: true
            }),
            new NomenclatureDTO<AquacultureTemperatureEnum>({
                value: AquacultureTemperatureEnum.Mixed,
                displayName: this.translate.getValue('aquacultures.temperature-mixed'),
                isActive: true
            })
        ];

        this.systemTypes = [
            new NomenclatureDTO<AquacultureSystemEnum>({
                value: AquacultureSystemEnum.FullSystem,
                displayName: this.translate.getValue('aquacultures.system-full'),
                isActive: true
            }),
            new NomenclatureDTO<AquacultureSystemEnum>({
                value: AquacultureSystemEnum.NonFullSystem,
                displayName: this.translate.getValue('aquacultures.system-non-full'),
                isActive: true
            })
        ];
    }

    private buildForm(): void {
        if (this.showOnlyRegiXData) {
            this.form = new FormGroup({
                submittedByControl: new FormControl(),
                submittedForControl: new FormControl(),
                usageDocumentControl: new FormControl()
            });
        }
        else if (this.isApplication || this.isApplicationHistoryMode) {
            this.form = new FormGroup({
                submittedByControl: new FormControl(),
                submittedForControl: new FormControl(),
                nameControl: new FormControl(null, [Validators.required, Validators.maxLength(200)]),
                territoryUnitControl: new FormControl(null, Validators.required),
                waterAreaTypeControl: new FormControl(null, Validators.required),
                populatedAreaControl: new FormControl(null),
                ekatteControl: new FormControl({ value: null, disabled: true }),
                locationDescriptionControl: new FormControl(null, [Validators.required, Validators.maxLength(500)]),
                waterSalinityControl: new FormControl(null, Validators.required),
                waterTemperatureControl: new FormControl(null, Validators.required),
                systemControl: new FormControl(null, Validators.required),
                powerSupplyTypeControl: new FormControl(null, Validators.required),
                powerSupplyNameControl: new FormControl(null, [Validators.maxLength(100)]),
                powerSupplyDebitControl: new FormControl(null, TLValidators.number(0)),
                totalWaterAreaControl: new FormControl(null, [Validators.required, TLValidators.number(0)]),
                totalProductionCapacityControl: new FormControl(null, [Validators.required, TLValidators.number(0)]),
                hatcheryCapacityControl: new FormControl(null),
                usageDocumentControl: new FormControl(),
                waterLawCertificateControl: new FormControl(),
                ovosCertificateControl: new FormControl(),
                hasNoBabhCertificateControl: new FormControl(false),
                babhCertificateControl: new FormControl(),

                commentsControl: new FormControl(null, Validators.maxLength(4000)),
                filesControl: new FormControl(),

                deliveryDataControl: new FormControl(),
                applicationPaymentInformationControl: new FormControl()
            }, this.baseFormValidators());

            this.coordinatesForm = new FormGroup({
                longitudeControl: new FormControl('', Validators.required),
                latitudeControl: new FormControl('', Validators.required)
            });

            this.aquaticOrganismsForm = new FormGroup({
                aquaticOrganismIdControl: new FormControl('', Validators.required)
            });

            this.hatcheryEquipmentGroup = new FormGroup({
                equipmentTypeIdControl: new FormControl(null, Validators.required),
                countControl: new FormControl(null, [Validators.required, TLValidators.number(1, undefined, 0)]),
                volumeControl: new FormControl(null, [Validators.required, TLValidators.number(0)])
            });
        }
        else {
            this.form = new FormGroup({
                submittedForControl: new FormControl(),
                regNumControl: new FormControl({ value: null, disabled: true }),
                urorNumControl: new FormControl({ value: null, disabled: true }),
                nameControl: new FormControl(null, [Validators.required, Validators.maxLength(200)]),
                statusControl: new FormControl(null, Validators.required),
                territoryUnitControl: new FormControl({ value: null, disabled: true }, Validators.required),
                waterAreaTypeControl: new FormControl({ value: null, disabled: true }, Validators.required),
                populatedAreaControl: new FormControl(null),
                ekatteControl: new FormControl({ value: null, disabled: true }),
                locationDescriptionControl: new FormControl(null, [Validators.required, Validators.maxLength(500)]),
                waterSalinityControl: new FormControl(null, Validators.required),
                waterTemperatureControl: new FormControl(null, Validators.required),
                systemControl: new FormControl(null, Validators.required),
                powerSupplyTypeControl: new FormControl(null, Validators.required),
                powerSupplyNameControl: new FormControl(null, [Validators.maxLength(100)]),
                powerSupplyDebitControl: new FormControl(null, TLValidators.number()),
                totalWaterAreaControl: new FormControl(null, [Validators.required, TLValidators.number(0)]),
                totalProductionCapacityControl: new FormControl(null, [Validators.required, TLValidators.number(0)]),
                hatcheryCapacityControl: new FormControl(null),
                commentsControl: new FormControl(null, Validators.maxLength(4000)),
                filesControl: new FormControl()
            }, this.baseFormValidators());

            this.coordinatesForm = new FormGroup({
                longitudeControl: new FormControl('', Validators.required),
                latitudeControl: new FormControl('', Validators.required)
            });

            this.aquaticOrganismsForm = new FormGroup({
                aquaticOrganismIdControl: new FormControl('', Validators.required)
            });

            this.hatcheryEquipmentGroup = new FormGroup({
                equipmentTypeIdControl: new FormControl(null, Validators.required),
                countControl: new FormControl(null, [Validators.required, TLValidators.number(1, undefined, 0)]),
                volumeControl: new FormControl(null, [Validators.required, TLValidators.number(0)])
            });
        }

        this.form.get('populatedAreaControl')?.valueChanges.subscribe({
            next: (populatedArea: PopulatedAreaNomenclatureExtendedDTO | undefined | null) => {
                if (populatedArea !== undefined && populatedArea !== null) {
                    this.form.get('ekatteControl')!.setValue(populatedArea.code);
                }
                else {
                    this.form.get('ekatteControl')!.setValue('');
                }
            }
        });

        this.form.get('hasNoBabhCertificateControl')?.valueChanges.subscribe({
            next: (value: boolean) => {
                if (value === true) {
                    this.form.get('babhCertificateControl')?.clearValidators();
                }
                else {
                    this.form.get('babhCertificateControl')?.setValidators(Validators.required);
                }
                this.form.get('babhCertificateControl')?.updateValueAndValidity();
            }
        });

        this.form.get('deliveryDataControl')?.valueChanges.subscribe({
            next: () => {
                this.hasNoEDeliveryRegistrationError = false;
            }
        });

        this.form.get('systemControl')?.valueChanges.subscribe({
            next: (system: NomenclatureDTO<AquacultureSystemEnum> | undefined) => {
                if (system !== undefined && system !== null) {
                    if (system.value === AquacultureSystemEnum.FullSystem) {
                        this.showHatcheryEquipment = true;

                        this.form.get('hatcheryCapacityControl')!.setValidators([Validators.required, TLValidators.number(0)]);
                        this.form.get('hatcheryCapacityControl')!.markAsPending({ emitEvent: false });
                    }
                    else if (system.value === AquacultureSystemEnum.NonFullSystem) {
                        this.showHatcheryEquipment = false;

                        this.form.get('hatcheryCapacityControl')!.clearValidators();
                        this.form.get('hatcheryCapacityControl')!.updateValueAndValidity({ emitEvent: false });
                    }

                    if (this.isReadonly || this.viewMode) {
                        this.form.get('hatcheryCapacityControl')!.disable();
                    }
                }
            }
        });

        this.form.get('waterAreaTypeControl')?.valueChanges.subscribe({
            next: (waterArea: NomenclatureDTO<number> | undefined) => {
                if (waterArea !== undefined && waterArea !== null) {
                    const internal: boolean = this.internalFishCodes.includes(waterArea.code!);
                    const danube: boolean = this.danubeFishCodes.includes(waterArea.code!);
                    const blackSea: boolean = this.blackSeaFishCodes.includes(waterArea.code!);

                    this.allAquaticOrganisms = this.unfilteredAquaticOrganisms.filter(x => {
                        return (internal && x.isInternal === true)
                            || (danube && x.isDanube === true)
                            || (blackSea && x.isBlackSea === true);
                    });

                    const aquacultureOrganismIds: number[] = this.getAquaticOrganismsFromTable() ?? [];

                    setTimeout(() => {
                        this.aquacultureAquaticOrganisms = aquacultureOrganismIds
                            .filter(x => this.allAquaticOrganisms.findIndex(y => y.value === x) !== -1)
                            .map((id: number) => {
                                return new AquaticOrganismTableModel(id);
                            });
                    });
                }
                else {
                    this.allAquaticOrganisms = [...this.unfilteredAquaticOrganisms];
                }

                this.aquaticOrganisms = [...this.allAquaticOrganisms];
            }
        });
    }

    private baseFormValidators(): ValidatorFn[] {
        if (this.isApplication) {
            return [this.aquaticOrganismsValidator(), this.installationsValidator()];
        }
        return [
            this.aquaticOrganismsValidator(), this.installationsValidator(),
            this.usageDocumentsValidator(), this.waterLawCertificatesValidator(), this.ovosCertificatesValidator()
        ];
    }

    private aquaticOrganismsValidator(): ValidatorFn {
        return (control: AbstractControl): ValidationErrors | null => {
            if (this.aquaticOrganismsTable !== undefined && this.aquaticOrganismsTable !== null) {
                if (this.aquaticOrganismsTable.rows.length === 0) {
                    return { 'atLeastOneAquaticOrganismNeeded': true };
                }
            }
            return null;
        };
    }

    private installationsValidator(): ValidatorFn {
        return (control: AbstractControl): ValidationErrors | null => {
            if (!this.installations.some(x => x.isActive !== false)) {
                return { 'atLeastOneInstallationNeeded': true };
            }

            if (this.installations.filter(x => x.isActive !== false).some(x => x.hasValidationErrors)) {
                return { 'installationsValidation': true };
            }
            return null;
        };
    }

    private usageDocumentsValidator(): ValidatorFn {
        return (control: AbstractControl): ValidationErrors | null => {
            if (!this.usageDocuments.some(x => x.isActive !== false)) {
                return { 'atLeastOneActiveUsageDocumentNeeded': true };
            }
            return null;
        };
    }

    private waterLawCertificatesValidator(): ValidatorFn {
        return (control: AbstractControl): ValidationErrors | null => {
            if (!this.waterLawCertificates.some(x => x.isActive !== false)) {
                return { 'atLeastOneActiveWaterLawCertificateNeeded': true };
            }
            return null;
        };
    }

    private ovosCertificatesValidator(): ValidatorFn {
        return (control: AbstractControl): ValidationErrors | null => {
            if (!this.ovosCertificates.some(x => x.isActive !== false)) {
                return { 'atLeastOneActiveOvosCertificateNeeded': true };
            }
            return null;
        };
    }

    private fillForm(): void {
        if (this.model instanceof AquacultureRegixDataDTO) {
            this.form.get('submittedByControl')!.setValue(this.model.submittedBy);
            this.form.get('submittedForControl')!.setValue(this.model.submittedFor);
            this.form.get('usageDocumentControl')!.setValue(this.model.usageDocument);

            this.fillFormRegiX(this.model);
        }
        else if (this.model instanceof AquacultureApplicationEditDTO) {
            this.fillFormApplication(this.model);

            if (this.showRegiXData) {
                this.fillFormRegiX(this.model);
            }
        }
        else if (this.model instanceof AquacultureFacilityEditDTO) {
            this.fillFormRegister(this.model);
        }
    }

    private fillFormRegiX(model: AquacultureRegixDataDTO): void {
        if (model.applicationRegiXChecks !== undefined && model.applicationRegiXChecks !== null) {
            const applicationRegiXChecks: ApplicationRegiXCheckDTO[] = model.applicationRegiXChecks;

            setTimeout(() => {
                this.regixChecks = applicationRegiXChecks;
            });
        }

        if (!this.viewMode) {
            this.notifier.start();
            this.notifier.onNotify.subscribe({
                next: () => {
                    this.form.markAllAsTouched();

                    if (this.showOnlyRegiXData) {
                        ApplicationUtils.enableOrDisableRegixCheckButtons(this.form, this.dialogRightSideActions);
                    }

                    this.notifier.stop();
                }
            });
        }
    }

    private fillFormApplication(model: AquacultureApplicationEditDTO): void {
        this.form.get('submittedByControl')!.setValue(model.submittedBy);
        this.form.get('submittedForControl')!.setValue(model.submittedFor);
        this.form.get('nameControl')!.setValue(model.name);

        if (model.territoryUnitId !== undefined && model.territoryUnitId !== null) {
            this.form.get('territoryUnitControl')!.setValue(this.territoryUnits.find(x => x.value === model.territoryUnitId));
        }

        if (model.waterAreaTypeId !== undefined && model.waterAreaTypeId !== null) {
            this.form.get('waterAreaTypeControl')!.setValue(this.waterAreaTypes.find(x => x.value === model.waterAreaTypeId));
        }

        if (model.populatedAreaId !== undefined && model.populatedAreaId !== null) {
            const populatedArea: NomenclatureDTO<number> = this.populatedAreas.find(x => x.value === model.populatedAreaId)!;
            this.form.get('populatedAreaControl')!.setValue(populatedArea);
            this.form.get('ekatteControl')!.setValue(populatedArea.code);
        }

        this.form.get('locationDescriptionControl')!.setValue(model.locationDescription);

        setTimeout(() => {
            this.aquacultureCoordinates = model.coordinates ?? [];
            this.aquacultureAquaticOrganisms = (model.aquaticOrganismIds ?? []).map((id: number) => {
                return new AquaticOrganismTableModel(id);
            });
            this.installations = model.installations ?? [];
            this.hatcheryEquipments = model.hatcheryEquipment ?? [];

            this.normalizeInstallations();
        });

        if (model.waterSalinity !== undefined && model.waterSalinity !== null) {
            this.form.get('waterSalinityControl')!.setValue(this.waterSalinityTypes.find(x => x.value === model.waterSalinity));
        }

        if (model.waterTemperature !== undefined && model.waterTemperature !== null) {
            this.form.get('waterTemperatureControl')!.setValue(this.waterTemperatureTypes.find(x => x.value === model.waterTemperature));
        }

        if (model.system !== undefined && model.system !== null) {
            this.form.get('systemControl')!.setValue(this.systemTypes.find(x => x.value === model.system));
        }

        if (model.powerSupplyTypeId !== undefined && model.powerSupplyTypeId !== null) {
            this.form.get('powerSupplyTypeControl')!.setValue(this.powerSupplyTypes.find(x => x.value === model.powerSupplyTypeId));
        }

        this.form.get('powerSupplyNameControl')!.setValue(model.powerSupplyName);
        this.form.get('powerSupplyDebitControl')!.setValue(model.powerSupplyDebit);
        this.form.get('totalWaterAreaControl')!.setValue(model.totalWaterArea);
        this.form.get('totalProductionCapacityControl')!.setValue(model.totalProductionCapacity);
        this.form.get('hatcheryCapacityControl')!.setValue(model.hatcheryCapacity);

        this.form.get('usageDocumentControl')!.setValue(model.usageDocument);
        this.form.get('waterLawCertificateControl')!.setValue(model.waterLawCertificate);
        this.form.get('ovosCertificateControl')!.setValue(model.ovosCertificate);

        if (model.hasBabhCertificate !== undefined && model.hasBabhCertificate !== null) {
            this.form.get('hasNoBabhCertificateControl')!.setValue(!model.hasBabhCertificate);
        }

        if (model.hasBabhCertificate === true) {
            this.form.get('babhCertificateControl')!.setValue(model.babhCertificate);
        }
        else {
            this.form.get('babhCertificateControl')!.setValue(null);
        }

        this.form.get('commentsControl')!.setValue(model.comments);
        this.form.get('filesControl')!.setValue(model.files);

        if (this.hasDelivery === true) {
            this.form.get('deliveryDataControl')!.setValue(model.deliveryData);
        }

        if (this.isPaid === true) {
            this.form.get('applicationPaymentInformationControl')!.setValue((this.model as AquacultureApplicationEditDTO).paymentInformation);
        }
    }

    private fillFormRegister(model: AquacultureFacilityEditDTO): void {
        this.form.get('submittedForControl')!.setValue(model.submittedFor);
        this.form.get('regNumControl')!.setValue(model.regNum);
        this.form.get('urorNumControl')!.setValue(model.urorNum);
        this.form.get('nameControl')!.setValue(model.name);
        this.form.get('territoryUnitControl')!.setValue(this.territoryUnits.find(x => x.value === model.territoryUnitId));
        this.form.get('waterAreaTypeControl')!.setValue(this.waterAreaTypes.find(x => x.value === model.waterAreaTypeId));
        this.form.get('populatedAreaControl')!.setValue(this.populatedAreas.find(x => x.value === model.populatedAreaId));
        this.form.get('locationDescriptionControl')!.setValue(model.locationDescription);
        this.form.get('waterSalinityControl')!.setValue(this.waterSalinityTypes.find(x => x.value === model.waterSalinity));
        this.form.get('waterTemperatureControl')!.setValue(this.waterTemperatureTypes.find(x => x.value === model.waterTemperature));
        this.form.get('systemControl')!.setValue(this.systemTypes.find(x => x.value === model.system));
        this.form.get('powerSupplyTypeControl')!.setValue(this.powerSupplyTypes.find(x => x.value === model.powerSupplyTypeId));
        this.form.get('powerSupplyNameControl')!.setValue(model.powerSupplyName);
        this.form.get('powerSupplyDebitControl')!.setValue(model.powerSupplyDebit);
        this.form.get('totalWaterAreaControl')!.setValue(model.totalWaterArea);
        this.form.get('totalProductionCapacityControl')!.setValue(model.totalProductionCapacity);
        this.form.get('hatcheryCapacityControl')!.setValue(model.hatcheryCapacity);
        this.form.get('commentsControl')!.setValue(model.comments);
        this.form.get('filesControl')!.setValue(model.files);

        if (model.status === AquacultureStatusEnum.Canceled) {
            this.form.get('statusControl')!.disable();
            this.statuses = this.allStatuses.slice();
        }

        if (model.status !== AquacultureStatusEnum.Application) {
            const code: string = AquacultureStatusEnum[model.status!];
            this.form.get('statusControl')!.setValue(this.allStatuses.find(x => x.code === code));
        }

        setTimeout(() => {
            this.cancellationHistory = model.cancellationHistory ?? [];
            this.aquacultureCoordinates = model.coordinates ?? [];

            this.aquacultureAquaticOrganisms = (model.aquaticOrganismIds ?? []).map((id: number) => {
                return new AquaticOrganismTableModel(id);
            });

            this.installations = model.installations ?? [];
            this.hatcheryEquipments = model.hatcheryEquipment ?? [];
            this.usageDocuments = model.usageDocuments ?? [];
            this.waterLawCertificates = model.waterLawCertificates ?? [];
            this.ovosCertificates = model.ovosCertificates ?? [];
            this.babhCertificates = model.babhCertificates ?? [];

            this.normalizeInstallations();
        });
    }

    private normalizeInstallations(): void {
        for (const installation of this.installations as AquacultureInstallationEditDTO[]) {
            const type: NomenclatureDTO<number> = this.installationTypes.find(x => x.code === AquacultureInstallationTypeEnum[installation.installationType!])!;
            installation.installationTypeName = type.displayName;

            switch (installation.installationType!) {
                case AquacultureInstallationTypeEnum.Basins:
                    installation.totalArea = this.sum(installation.basins!.filter(x => x.isActive).map(x => x.area! * x.count!));
                    installation.totalVolume = this.sum(installation.basins!.filter(x => x.isActive).map(x => x.volume! * x.count!));
                    installation.totalCount = this.sum(installation.basins!.filter(x => x.isActive).map(x => x.count!));
                    break;
                case AquacultureInstallationTypeEnum.NetCages:
                    installation.totalArea = this.sum(installation.netCages!.filter(x => x.isActive).map(x => x.area! * x.count!));
                    installation.totalVolume = this.sum(installation.netCages!.filter(x => x.isActive).map(x => x.volume! * x.count!));
                    installation.totalCount = this.sum(installation.netCages!.filter(x => x.isActive).map(x => x.count!));
                    break;
                case AquacultureInstallationTypeEnum.Aquariums:
                    installation.totalArea = undefined;
                    installation.totalVolume = installation.aquariums!.count! * installation.aquariums!.volume!;
                    installation.totalCount = installation.aquariums!.count;
                    break;
                case AquacultureInstallationTypeEnum.Collectors:
                    installation.totalArea = this.sum(installation.collectors!.filter(x => x.isActive).map(x => x.totalArea!));
                    installation.totalVolume = undefined;
                    installation.totalCount = this.sum(installation.collectors!.filter(x => x.isActive).map(x => x.totalCount!));
                    break;
                case AquacultureInstallationTypeEnum.Rafts:
                    installation.totalArea = this.sum(installation.rafts!.filter(x => x.isActive).map(x => x.area! * x.count!));
                    installation.totalVolume = undefined;
                    installation.totalCount = this.sum(installation.rafts!.filter(x => x.isActive).map(x => x.count!));
                    break;
                case AquacultureInstallationTypeEnum.Dams:
                    installation.totalArea = installation.dams!.area;
                    installation.totalVolume = undefined;
                    installation.totalCount = 1;
                    break;
                case AquacultureInstallationTypeEnum.RecirculatorySystems:
                    installation.totalArea = this.sum(installation.recirculatorySystems!.map(x => x.area! * x.count!));
                    installation.totalVolume = this.sum(installation.recirculatorySystems!.map(x => x.volume! * x.count!));
                    installation.totalCount = this.sum(installation.recirculatorySystems!.map(x => x.count!));
                    break;
            }
        }
    }

    private fillModel(): void {
        if (this.model instanceof AquacultureRegixDataDTO) {
            this.fillModelRegix(this.model);
        }
        else if (this.model instanceof AquacultureApplicationEditDTO) {
            this.fillModelApplication(this.model);
        }
        else if (this.model instanceof AquacultureFacilityEditDTO) {
            this.fillModelRegister(this.model);
        }
    }

    private fillModelRegix(model: AquacultureRegixDataDTO): void {
        model.submittedBy = this.form.get('submittedByControl')!.value;
        model.submittedFor = this.form.get('submittedForControl')!.value;
        model.usageDocument = this.form.get('usageDocumentControl')!.value;
    }

    private fillModelApplication(model: AquacultureApplicationEditDTO): void {
        model.submittedBy = this.form.get('submittedByControl')!.value;
        model.submittedFor = this.form.get('submittedForControl')!.value;
        model.name = this.form.get('nameControl')!.value;
        model.territoryUnitId = this.form.get('territoryUnitControl')!.value?.value;
        model.waterAreaTypeId = this.form.get('waterAreaTypeControl')!.value?.value;
        model.populatedAreaId = this.form.get('populatedAreaControl')!.value?.value;
        model.locationDescription = this.form.get('locationDescriptionControl')!.value;
        model.waterSalinity = this.form.get('waterSalinityControl')!.value?.value;
        model.waterTemperature = this.form.get('waterTemperatureControl')!.value?.value;
        model.system = this.form.get('systemControl')!.value?.value;
        model.powerSupplyTypeId = this.form.get('powerSupplyTypeControl')!.value?.value;
        model.powerSupplyName = this.form.get('powerSupplyNameControl')!.value;
        model.powerSupplyDebit = this.form.get('powerSupplyDebitControl')!.value;
        model.totalWaterArea = this.form.get('totalWaterAreaControl')!.value;
        model.totalProductionCapacity = this.form.get('totalProductionCapacityControl')!.value;

        if (this.showHatcheryEquipment) {
            model.hatcheryCapacity = this.form.get('hatcheryCapacityControl')!.value;
        }

        model.usageDocument = this.form.get('usageDocumentControl')!.value;
        model.waterLawCertificate = this.form.get('waterLawCertificateControl')!.value;
        model.ovosCertificate = this.form.get('ovosCertificateControl')!.value;
        model.hasBabhCertificate = this.form.get('hasNoBabhCertificateControl')!.value === false;

        if (model.hasBabhCertificate) {
            model.babhCertificate = this.form.get('babhCertificateControl')!.value;
        }
        else {
            model.babhCertificate = undefined;
        }

        model.comments = this.form.get('commentsControl')!.value;
        model.files = this.form.get('filesControl')!.value;

        model.coordinates = this.getCoordinatesFromTable();
        model.aquaticOrganismIds = this.getAquaticOrganismsFromTable();
        model.installations = this.getInstallationsFromTable();
        model.hatcheryEquipment = this.getHatcheryEquipmentFromTable();

        if (this.hasDelivery === true) {
            model.deliveryData = this.form.get('deliveryDataControl')!.value;
        }

        if (this.isPaid === true) {
            (this.model as AquacultureApplicationEditDTO).paymentInformation = this.form.get('applicationPaymentInformationControl')!.value;
        }
    }

    private fillModelRegister(model: AquacultureFacilityEditDTO): void {
        model.submittedFor = this.form.get('submittedForControl')!.value;
        model.name = this.form.get('nameControl')!.value;
        model.territoryUnitId = this.form.get('territoryUnitControl')!.value?.value;
        model.waterAreaTypeId = this.form.get('waterAreaTypeControl')!.value?.value;
        model.populatedAreaId = this.form.get('populatedAreaControl')!.value?.value;
        model.locationDescription = this.form.get('locationDescriptionControl')!.value;
        model.waterSalinity = this.form.get('waterSalinityControl')!.value?.value;
        model.waterTemperature = this.form.get('waterTemperatureControl')!.value?.value;
        model.system = this.form.get('systemControl')!.value?.value;
        model.powerSupplyTypeId = this.form.get('powerSupplyTypeControl')!.value?.value;
        model.powerSupplyName = this.form.get('powerSupplyNameControl')!.value;
        model.powerSupplyDebit = this.form.get('powerSupplyDebitControl')!.value;
        model.totalWaterArea = this.form.get('totalWaterAreaControl')!.value;
        model.totalProductionCapacity = this.form.get('totalProductionCapacityControl')!.value;
        model.comments = this.form.get('commentsControl')!.value;
        model.files = this.form.get('filesControl')!.value;

        if (this.showHatcheryEquipment) {
            model.hatcheryCapacity = this.form.get('hatcheryCapacityControl')!.value;
        }

        model.status = AquacultureStatusEnum[this.form.get('statusControl')!.value!.code as keyof typeof AquacultureStatusEnum];

        model.coordinates = this.getCoordinatesFromTable();
        model.aquaticOrganismIds = this.getAquaticOrganismsFromTable();
        model.installations = this.getInstallationsFromTable();
        model.hatcheryEquipment = this.getHatcheryEquipmentFromTable();
        model.usageDocuments = this.usageDocuments;
        model.waterLawCertificates = this.waterLawCertificates;
        model.ovosCertificates = this.ovosCertificates;
        model.babhCertificates = this.babhCertificates;
    }

    private getCoordinatesFromTable(): AquacultureCoordinateDTO[] {
        const rows = this.coordinatesTable.rows as AquacultureCoordinateDTO[];
        return rows.map(x => new AquacultureCoordinateDTO({
            id: x.id,
            latitude: x.latitude,
            longitude: x.longitude,
            isActive: x.isActive ?? true
        }));
    }

    private getAquaticOrganismsFromTable(): number[] {
        const rows = this.aquaticOrganismsTable.rows as AquaticOrganismTableModel[];
        return rows.map(x => x.aquaticOrganismId);
    }

    private getInstallationsFromTable(): AquacultureInstallationEditDTO[] {
        let rows = this.installationsTable.rows as AquacultureInstallationEditDTO[];

        if (this.isApplication && this.isDraftMode()) {
            rows = rows.filter(x => x.isActive !== false);
        }

        return rows.map(x => new AquacultureInstallationEditDTO({
            id: x.id,
            installationTypeName: x.installationTypeName,
            totalCount: x.totalCount,
            totalArea: x.totalArea,
            totalVolume: x.totalVolume,
            installationType: x.installationType,
            basins: x.basins,
            netCages: x.netCages,
            aquariums: x.aquariums,
            collectors: x.collectors,
            rafts: x.rafts,
            dams: x.dams,
            recirculatorySystems: x.recirculatorySystems,
            comments: x.comments,
            isActive: x.isActive ?? true
        }));
    }

    private getHatcheryEquipmentFromTable(): AquacultureHatcheryEquipmentDTO[] {
        if (this.showHatcheryEquipment === true) {
            let rows = this.hatcheryEquipmentTable.rows as AquacultureHatcheryEquipmentDTO[];

            if (this.isApplication && this.isDraftMode()) {
                rows = rows.filter(x => x.isActive !== false);
            }

            return rows.map(x => new AquacultureHatcheryEquipmentDTO({
                id: x.id,
                equipmentTypeId: x.equipmentTypeId,
                count: x.count,
                volume: x.volume,
                isActive: x.isActive ?? true
            }));
        }
        return [];
    }

    private saveAquaculture(dialogClose: DialogCloseCallback, saveAsDraft: boolean = false): Observable<boolean> {
        const saveOrEditDone: Subject<boolean> = new Subject<boolean>();

        this.saveOrEdit(saveAsDraft).subscribe({
            next: (id: number | void) => {
                this.hasNoEDeliveryRegistrationError = false;

                if (typeof id === 'number' && id !== undefined) {
                    this.model.id = id;
                    dialogClose(this.model);
                }
                else {
                    dialogClose(this.model);
                }

                saveOrEditDone.next(true);
                saveOrEditDone.complete();
            },
            error: (response: HttpErrorResponse) => {
                this.handleSaveErrorResponse(response);
            }
        });

        return saveOrEditDone;
    }

    private handleSaveErrorResponse(errorResponse: HttpErrorResponse): void {
        if (errorResponse.error !== undefined && errorResponse.error !== null) {
            const error: ErrorModel = errorResponse.error as ErrorModel;

            if (error.code === ErrorCode.NoEDeliveryRegistration) {
                this.hasNoEDeliveryRegistrationError = true;
                this.validityCheckerGroup.validate();
            }
        }
    }

    private saveOrEdit(saveAsDraft: boolean): Observable<number | void> {
        this.fillModel();
        CommonUtils.sanitizeModelStrings(this.model);

        if (this.model instanceof AquacultureFacilityEditDTO) {
            if (this.isChangeOfCircumstancesApplication === true) {
                return this.service.completeChangeOfCircumstancesApplication(this.model);
            }

            if (this.id !== undefined) {
                return this.service.editAquaculture(this.model);
            }
            return this.service.addAquaculture(this.model);
        }
        else {
            if (this.model.id !== undefined && this.model.id !== null) {
                return this.service.editApplication(this.model, this.pageCode, saveAsDraft);
            }
            return this.service.addApplication(this.model, this.pageCode);
        }
    }

    private save(dialogClose: DialogCloseCallback): void {
        this.openConfirmDialogForApplication(() => {
            this.saveAquaculture(dialogClose);
        })
    }

    private openConfirmDialogForApplication(callback: (...args: unknown[]) => unknown): void {
        if (this.isChangeOfCircumstancesApplication === true || this.isDeregistrationApplication === true) {
            this.confirmDialog.open({
                title: this.translate.getValue('aquacultures.complete-application-confirm-dialog-title'),
                message: this.translate.getValue('aquacultures.complete-application-confirm-dialog-message'),
                okBtnLabel: this.translate.getValue('aquacultures.complete-application-confirm-dialog-ok-btn-label')
            }).subscribe({
                next: (ok: boolean) => {
                    if (ok === true) {
                        callback();
                    }
                }
            });
        }
        else {
            callback();
        }
    }

    private isDraftMode(): boolean {
        return this.isApplication && (this.model.id === null || this.model.id === undefined);
    }

    private markAllAsTouched(): void {
        this.form.markAllAsTouched();

        this.aquaticOrganismsTouched = true;
        this.installationsTouched = true;
        this.usageDocumentsTouched = true;
        this.waterLawCertificatesTouched = true;
        this.ovosCertificatesTouched = true;
        this.babhCertificatesTouched = true;
    }

    private sum(nums: number[]): number {
        return nums.reduce((sum: number, current: number) => { return sum + current; }, 0);
    }

    private shouldHidePaymentData(): boolean {
        return (this.model as AquacultureApplicationEditDTO)?.paymentInformation?.paymentType === null
            || (this.model as AquacultureApplicationEditDTO)?.paymentInformation?.paymentType === undefined
            || (this.model as AquacultureApplicationEditDTO)?.paymentInformation?.paymentType === '';
    }
}