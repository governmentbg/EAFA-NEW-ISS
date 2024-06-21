import { Component, EventEmitter, OnInit, ViewChild } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { forkJoin, Observable, Subject, Subscription } from 'rxjs';
import { map } from 'rxjs/operators';
import { MatSnackBar } from '@angular/material/snack-bar';

import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { PageCodeEnum } from '@app/enums/page-code.enum';
import { SubmittedByRolesEnum } from '@app/enums/submitted-by-roles.enum';
import { IApplicationsService } from '@app/interfaces/administration-app/applications.interface';
import { ICommercialFishingService } from '@app/interfaces/common-app/commercial-fishing.interface';
import { DialogParamsModel } from '@app/models/common/dialog-params.model';
import { ApplicationRegiXCheckDTO } from '@app/models/generated/dtos/ApplicationRegiXCheckDTO';
import { CommercialFishingApplicationEditDTO } from '@app/models/generated/dtos/CommercialFishingApplicationEditDTO';
import { CommercialFishingEditDTO } from '@app/models/generated/dtos/CommercialFishingEditDTO';
import { CommercialFishingRegixDataDTO } from '@app/models/generated/dtos/CommercialFishingRegixDataDTO';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { RegixLegalDataDTO } from '@app/models/generated/dtos/RegixLegalDataDTO';
import { RegixPersonDataDTO } from '@app/models/generated/dtos/RegixPersonDataDTO';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { TLError } from '@app/shared/components/input-controls/models/tl-error.model';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { ApplicationDialogData, ApplicationUtils } from '@app/shared/utils/application.utils';
import { ApplicationContentDTO } from '@app/models/generated/dtos/ApplicationContentDTO';
import { RegixChecksWrapperDTO } from '@app/models/generated/dtos/RegixChecksWrapperDTO';
import { SystemParametersService } from '@app/services/common-app/system-parameters.service';
import { ApplicationSubmittedByRegixDataDTO } from '@app/models/generated/dtos/ApplicationSubmittedByRegixDataDTO';
import { ApplicationSubmittedForRegixDataDTO } from '@app/models/generated/dtos/ApplicationSubmittedForRegixDataDTO';
import { ApplicationSubmittedForDTO } from '@app/models/generated/dtos/ApplicationSubmittedForDTO';
import { ApplicationSubmittedByDTO } from '@app/models/generated/dtos/ApplicationSubmittedByDTO';
import { CommonNomenclatures } from '@app/services/common-app/common-nomenclatures.service';
import { FishNomenclatureDTO } from '@app/models/generated/dtos/FishNomenclatureDTO';
import { ApplicationPaymentInformationDTO } from '@app/models/generated/dtos/ApplicationPaymentInformationDTO';
import { HttpErrorResponse } from '@angular/common/http';
import { CommercialFishingValidationErrorsEnum } from '@app/enums/commercial-fishing-validation-errors.enum';
import { IHeaderAuditButton } from '@app/shared/components/dialog-wrapper/interfaces/header-audit-button.interface';
import { TLMatDialog } from '@app/shared/components/dialog-wrapper/tl-mat-dialog';
import { HeaderCloseFunction } from '@app/shared/components/dialog-wrapper/interfaces/header-cancel-button.interface';
import { TLConfirmDialog } from '@app/shared/components/confirmation-dialog/tl-confirm-dialog';
import { TLDataTableComponent } from '@app/shared/components/data-table/tl-data-table.component';
import { CommercialFishingLogBookEditDTO } from '@app/models/generated/dtos/CommercialFishingLogBookEditDTO';
import { CommercialFishingDialogParamsModel } from '../../models/commercial-fishing-dialog-params.model';
import { IApplicationRegister } from '@app/interfaces/common-app/application-register.interface';
import { QuotaAquaticOrganismDTO } from '@app/models/generated/dtos/QuotaAquaticOrganismDTO';
import { QuotaSpiciesPortDTO } from '@app/models/generated/dtos/QuotaSpiciesPortDTO';
import { RecordChangedEventArgs } from '@app/shared/components/data-table/models/record-changed-event.model';
import { CommandTypes } from '@app/shared/components/data-table/enums/command-type.enum';
import { SuspensionDataDTO } from '@app/models/generated/dtos/SuspensionDataDTO';
import { SuspnesionDataDialogParams } from '../../models/suspnesion-data-dialog-params.model';
import { ShipNomenclatureDTO } from '@app/models/generated/dtos/ShipNomenclatureDTO';
import { LogBookTypesEnum } from '@app/enums/log-book-types.enum';
import { LogBookGroupsEnum } from '@app/enums/log-book-groups.enum';
import { ErrorCode, ErrorModel } from '@app/models/common/exception.model';
import { PermitNomenclatureDTO } from '@app/models/generated/dtos/PermitNomenclatureDTO';
import { IS_PUBLIC_APP } from '@app/shared/modules/application.modules';
import { ChoosePermitLicenseForRenewalComponent } from '../choose-permit-license-for-renewal/choose-permit-license-for-renewal.component';
import { ChoosePermitLicenseForRenewalDialogParams } from '../choose-permit-license-for-renewal/models/choose-permit-license-for-renewal-dialog-params.model';
import { WaterTypesEnum } from '@app/enums/water-types.enum';
import { FishingGearDTO } from '@app/models/generated/dtos/FishingGearDTO';
import { PoundNetNomenclatureDTO } from '@app/models/generated/dtos/PoundNetNomenclatureDTO';
import { QualifiedFisherNomenclatureDTO } from '@app/models/generated/dtos/QualifiedFisherNomenclatureDTO';
import { LogBookPagePersonTypesEnum } from '@app/enums/log-book-page-person-types.enum';
import { CommercialFishingShipFilters } from '../../models/commercial-fishing-ship-filters.model';
import { CommercialFishingPublicService } from '@app/services/public-app/commercial-fishing-public.service';
import { FileTypeEnum } from '@app/enums/file-types.enum';
import { ValidityCheckerGroupDirective } from '@app/shared/directives/validity-checker/validity-checker-group.directive';
import { Notifier } from '@app/shared/directives/notifier/notifier.class';
import { PaymentSummaryDTO } from '@app/models/generated/dtos/PaymentSummaryDTO';
import { PaymentTariffDTO } from '@app/models/generated/dtos/PaymentTariffDTO';
import { PermitLicenseTariffCalculationParameters } from '../../models/permit-license-tariff-calculation-parameters.model';
import { OverlappingLogBooksComponent } from '@app/shared/components/overlapping-log-books/overlapping-log-books.component';
import { OverlappingLogBooksDialogParamsModel } from '@app/shared/components/overlapping-log-books/models/overlapping-log-books-dialog-params.model';
import { OverlappingLogBooksParameters } from '@app/shared/components/overlapping-log-books/models/overlapping-log-books-parameters.model';
import { PermittedFileTypeDTO } from '@app/models/generated/dtos/PermittedFileTypeDTO';
import { DuplicatesEntryDTO } from '@app/models/generated/dtos/DuplicatesEntryDTO';
import { ChoosePermitToCopyFromComponent } from '../choose-permit-to-copy-from/choose-permit-to-copy-from.component';
import { ChoosePermitToCopyFromDialogParams } from '../choose-permit-to-copy-from/models/choose-permit-to-copy-from-dialog-params.model';
import { ChoosePermitToCopyFromDialogResult } from '../choose-permit-to-copy-from/models/choose-permit-to-copy-from-dialog-result.model';
import { ShipsUtils } from '@app/shared/utils/ships.utils';
import { IGroupedOptions } from '@app/shared/components/input-controls/tl-autocomplete/interfaces/grouped-options.interface';
import { FishingGearMarkStatusesEnum } from '@app/enums/fishing-gear-mark-statuses.enum';
import { FishingGearPingerStatusesEnum } from '@app/enums/fishing-gear-pinger-statuses.enum';
import { FormControlDataLoader } from '@app/shared/utils/form-control-data-loader';
import { CommercialFishingAdministrationService } from '@app/services/administration-app/commercial-fishing-administration.service';
import { RequestProperties } from '@app/shared/services/request-properties';
import { PaymentDataComponent } from '@app/shared/components/payment-data/payment-data.component';
import { PaymentDataInfo } from '@app/shared/components/payment-data/models/payment-data-info.model';
import { PaymentDataDTO } from '@app/models/generated/dtos/PaymentDataDTO';
import { PaymentTypesEnum } from '@app/enums/payment-types.enum';
import { PaymentStatusesEnum } from '@app/enums/payment-statuses.enum';
import { SimpleAuditMethod } from '../log-books/log-books.component';
import { TLPictureRequestMethod } from '@app/shared/components/tl-picture-uploader/tl-picture-uploader.component';
import { FileInfoDTO } from '@app/models/generated/dtos/FileInfoDTO';
import { PersonFullDataDTO } from '@app/models/generated/dtos/PersonFullDataDTO';
import { EgnLncDTO } from '@app/models/generated/dtos/EgnLncDTO';
import { PersonLegalExtractorService } from '@app/services/common-app/person-legal-extractor.service';
import { FishQuotaDTO } from '@app/models/generated/dtos/FishQuotaDTO';
import { GetControlErrorLabelTextCallback } from '@app/shared/components/input-controls/base-tl-control';
import { LogBookStatusesEnum } from '@app/enums/log-book-statuses.enum';
import { EditSuspensionComponent } from '../suspensions/components/edit-suspension/edit-suspension.component';
import { PermissionsService } from '@app/shared/services/permissions.service';
import { PermissionsEnum } from '@app/shared/enums/permissions.enum';
import { DateUtils } from '@app/shared/utils/date.utils';
import { FishingGearUtils } from '../../utils/fishing-gear.utils';

type AquaticOrganismsToAddType = NomenclatureDTO<number> | NomenclatureDTO<number>[] | string | undefined | null;
type SaveApplicationDraftFnType = ((applicationId: number, model: IApplicationRegister, dialogClose: HeaderCloseFunction) => void) | undefined;
type SaveMethodType = 'save' | 'saveAndPrint' | 'saveAndStartPermitLicense';

const SHIP_WITH_ONLINE_LOG_BOOKS_LENGTH: number = 12;
const AQUATIC_ORGANISMS_PER_PAGE: number = 5;

@Component({
    selector: 'edit-commercial-fishing',
    templateUrl: './edit-commercial-fishing.component.html'
})
export class EditCommercialFishingComponent implements OnInit, IDialogComponent {
    public form!: FormGroup;
    public pageCode!: PageCodeEnum;
    public service!: ICommercialFishingService;
    public readonly pageCodes: typeof PageCodeEnum = PageCodeEnum;
    public readonly logBookTypes: typeof LogBookTypesEnum = LogBookTypesEnum;

    public isThirdCountryPermit!: boolean;
    public isApplication!: boolean;
    public isApplicationHistoryMode!: boolean;
    public isReadonly!: boolean;
    public viewMode!: boolean;
    public showOnlyRegiXData!: boolean;
    public showRegiXData: boolean = false;
    public isEditing: boolean = false;
    public isEditingSubmittedBy: boolean = false;
    public loadRegisterFromApplication: boolean = false;
    public permitLicenseIsValid: boolean = true;
    public hideBasicPaymentInfo: boolean = false;
    public hideSubmittedByDocument: boolean = false;
    public isQualifiedFisherPhotoRequired: boolean = false;

    public hasAnyAquaticOrganismTypesSelected: boolean = false;
    public hasCaptainNotQualifiedFisherError: boolean = false;
    public hasSubmittedForNotShipOwnerError: boolean = false;
    public hasNoEDeliveryRegistrationError: boolean = false;
    public hasInvalidPermitRegistrationNumber: boolean = false;
    public selectedShipAlreadyHasValidBlackSeaPermitError: boolean = false;
    public selectedShipAlreadyHasValidDanubePermitError: boolean = false;
    public selectedShipHasNoBlackSeaPermitError: boolean = false;
    public selectedShipHasNoDanubePermitError: boolean = false;
    public selectedShipHasNoPoundNetPermitError: boolean = false;
    public selectedShipIsThirdCountryError: boolean = false;
    public selectedShipIsNotThirdCountryError: boolean = false;
    public hasNoPermitRegisterForPermitLicenseError: boolean = false;
    public hasShipEventExistsOnSameDateError: boolean = false;
    public duplicatedMarkNumbers: string[] = [];
    public duplicatedPingerNumbers: string[] = [];

    public isPublicApp: boolean = false;
    public isPermitLicense: boolean = false;
    public hasDelivery: boolean = false;
    public isPaid: boolean = false;
    public noShipSelected: boolean = true; // Needed only for permit licenses
    public isOnlineApplication: boolean = false;
    public refreshFileTypes: Subject<void> = new Subject<void>();
    public isSuspended: boolean = false;
    public canReadSuspensions: boolean = false;
    public isFishingGearsApplication: boolean = false;
    public isRegisterEntry: boolean = false;
    public isIdReadOnly: boolean = false;

    public submittedByRole: SubmittedByRolesEnum | undefined;
    public readonly submittedByRoles: typeof SubmittedByRolesEnum = SubmittedByRolesEnum;

    public notifier: Notifier = new Notifier();
    public regixChecks: ApplicationRegiXCheckDTO[] = [];
    public expectedResults: CommercialFishingRegixDataDTO;
    public duplicates: DuplicatesEntryDTO[] = [];

    public aquaticOrganismTypesControl: FormControl = new FormControl();

    public readonly logBookGroup: LogBookGroupsEnum = LogBookGroupsEnum.Ship;
    public readonly disabledAddBtnTooltipText: string;
    public permitLicenseRegisterId: number | undefined;
    public shipId: number | undefined;

    public ships: ShipNomenclatureDTO[] = [];
    public qualifiedFishers: QualifiedFisherNomenclatureDTO[] = []; // needed only when isApplication is FALSE
    public waterTypes: NomenclatureDTO<number>[] = [];
    public allAquaticOrganismTypes: FishNomenclatureDTO[] = []; // for Permit License only (when pageCode !== CatchQuotaSpecies)
    public aquaticOrganismTypes: FishNomenclatureDTO[] = [];
    public quotaAquaticOrganismTypes: FishNomenclatureDTO[] = []; // need only for CatchQuotaSpecies Permit License
    public selectedAquaticOrganismTypes: FishNomenclatureDTO[] = []; // for Permit License only (when pageCode !== CatchQuotaSpecies)
    public waterAquaticOrganismTypes: FishNomenclatureDTO[] = [];
    public holderShipRelations: NomenclatureDTO<boolean>[] = [];
    public groundForUseTypes: NomenclatureDTO<number>[] = [];
    public poundNets: IGroupedOptions<number>[] = []; // needed only when Permit/Permit License is for pound net
    public allPorts: NomenclatureDTO<number>[] = []; // needed only for CatchQuotaSpecies Permit License
    public ports: NomenclatureDTO<number>[] = []; // needed only for CatchQuotaSpecies Permit License
    public quotaAquaticOrganismsForm: FormGroup | undefined; // need only for CatchQuotaSpecies Permit License
    public quotaAquaticOrganisms: QuotaAquaticOrganismDTO[] = []; // need only for CatchQuotaSpecies Permit License
    public allPermits: PermitNomenclatureDTO[] = [] //needed only when RecordType == Application && PageCode is PermitLicense/Quota/PoundNetLicense
    public permits: PermitNomenclatureDTO[] = []; // needed only when RecordType == Application && PageCode is PermitLicense/Quota/PoundNetLicense

    public submittedForLabel: string = '';
    public qualifiedFisherSameAsSubmittedForLabel: string = '';

    public maxNumberOfFishingGears: number = 0;
    public readonly aquaticOrganismsPerPage: number;
    public onlyOnlineLogBooks: boolean | undefined; // needed in Register entry for pertmi licenses - to pass to log-books component
    public logBookOwnerType: LogBookPagePersonTypesEnum | undefined; // whether the type of submittedFor is Person or Legal (for admission and transportation log books)

    public model!: CommercialFishingEditDTO | CommercialFishingApplicationEditDTO | CommercialFishingRegixDataDTO;

    public getLogBookAuditMethod!: SimpleAuditMethod;
    public photoRequestMethod?: TLPictureRequestMethod;
    public getControlErrorLabelTextMethod: GetControlErrorLabelTextCallback = this.getControlErrorLabelText.bind(this);

    /**
     * Cast of service property - needed for <log-books> component for the register for administration app - permit licenses only
     * */
    public administrationService: CommercialFishingAdministrationService | undefined;

    @ViewChild('logBooksTable')
    private logBooksTable!: TLDataTableComponent;

    @ViewChild('quotaAquaticOrganismTypesTable')
    private quotaAquaticOrganismTypesTable!: TLDataTableComponent;

    @ViewChild(ValidityCheckerGroupDirective)
    private validityCheckerGroup!: ValidityCheckerGroupDirective;

    private id!: number;
    /**
     * Set only when renewal of permit license
     * */
    private renewalPermitLicenseId: number | undefined;
    /**
     * Set only when renewal from permit
     * */
    private renewalPermitId: number | undefined;
    private applicationId: number | undefined;
    private applicationsService: IApplicationsService | undefined;
    private dialogRightSideActions: IActionInfo[] | undefined;
    private translationService: FuseTranslationLoaderService;
    private systemParametersService: SystemParametersService;
    private commonNomenclatures: CommonNomenclatures;
    private confirmDialog: TLConfirmDialog;
    private onRecordAddedOrEdittedEvent: EventEmitter<number> | undefined;
    private saveApplicationDraftContentActionClicked: SaveApplicationDraftFnType;
    private modelLoadedFromPermit: boolean = false;
    private shipFilters!: CommercialFishingShipFilters;
    private ignoreLogBookConflicts: boolean = false;
    private isAddingRegister: boolean = false;

    private readonly personLegalExtractor: PersonLegalExtractorService;
    private readonly fisherCache: Map<string, PersonFullDataDTO | null> = new Map<string, PersonFullDataDTO | null>();

    private readonly editCommercialFishingPermitLicenseDialog: TLMatDialog<EditCommercialFishingComponent>;
    private readonly addSuspensionDialog: TLMatDialog<EditSuspensionComponent>;
    private readonly choosePermitLicenseForRenewalDialog: TLMatDialog<ChoosePermitLicenseForRenewalComponent>;
    private readonly choosePermitToCopyFromDialog: TLMatDialog<ChoosePermitToCopyFromComponent>;
    private readonly overlappingLogBooksDialog: TLMatDialog<OverlappingLogBooksComponent>;
    private readonly snackbar: MatSnackBar;
    private readonly paymentDataDialog: TLMatDialog<PaymentDataComponent>;
    private readonly nomenclaturesService: CommonNomenclatures;
    private readonly permissionsService: PermissionsService;
    private readonly loader: FormControlDataLoader;

    public constructor(
        translate: FuseTranslationLoaderService,
        systemParametersService: SystemParametersService,
        commonNomenclatures: CommonNomenclatures,
        confirmDialog: TLConfirmDialog,
        editCommercialFishingPermitLicenseDialog: TLMatDialog<EditCommercialFishingComponent>,
        addSuspensionDialog: TLMatDialog<EditSuspensionComponent>,
        choosePermitLicenseForRenewalDialog: TLMatDialog<ChoosePermitLicenseForRenewalComponent>,
        choosePermitToCopyFromDialog: TLMatDialog<ChoosePermitToCopyFromComponent>,
        overlappingLogBooksDialog: TLMatDialog<OverlappingLogBooksComponent>,
        snackbar: MatSnackBar,
        paymentDataDialog: TLMatDialog<PaymentDataComponent>,
        personLegalExtractor: PersonLegalExtractorService,
        nomenclaturesService: CommonNomenclatures,
        permissionsService: PermissionsService
    ) {
        this.translationService = translate;
        this.systemParametersService = systemParametersService;
        this.commonNomenclatures = commonNomenclatures;
        this.confirmDialog = confirmDialog;
        this.editCommercialFishingPermitLicenseDialog = editCommercialFishingPermitLicenseDialog;
        this.addSuspensionDialog = addSuspensionDialog;
        this.choosePermitLicenseForRenewalDialog = choosePermitLicenseForRenewalDialog;
        this.choosePermitToCopyFromDialog = choosePermitToCopyFromDialog;
        this.overlappingLogBooksDialog = overlappingLogBooksDialog;
        this.snackbar = snackbar;
        this.paymentDataDialog = paymentDataDialog;
        this.personLegalExtractor = personLegalExtractor;
        this.nomenclaturesService = nomenclaturesService;
        this.permissionsService = permissionsService;

        this.expectedResults = new CommercialFishingRegixDataDTO({
            submittedBy: new ApplicationSubmittedByRegixDataDTO({ addresses: [], person: new RegixPersonDataDTO() }),
            submittedFor: new ApplicationSubmittedForRegixDataDTO({ addresses: [], legal: new RegixLegalDataDTO(), person: new RegixPersonDataDTO() }),
        });

        this.isPublicApp = IS_PUBLIC_APP;

        this.disabledAddBtnTooltipText = this.translationService.getValue('commercial-fishing.cannot-add-log-books-to-suspended-permit-license');

        this.holderShipRelations = [
            new NomenclatureDTO<boolean>({
                value: true,
                displayName: this.translationService.getValue('commercial-fishing.holder-is-ship-owner'),
                isActive: true
            }),
            new NomenclatureDTO<boolean>({
                value: false,
                displayName: this.translationService.getValue('commercial-fishing.holder-is-ship-user'),
                isActive: true
            })
        ];

        this.aquaticOrganismsPerPage = AQUATIC_ORGANISMS_PER_PAGE;

        this.loader = new FormControlDataLoader(this.getNomenclatures.bind(this));
    }

    public ngOnInit(): void {
        if (this.isPermitLicense) {
            this.submittedForLabel = this.translationService.getValue('commercial-fishing.permit-license-submitted-for-panel');
            this.qualifiedFisherSameAsSubmittedForLabel = this.translationService.getValue('commercial-fishing.permit-license-qualified-fisher-same-as-submitted-for');
        }
        else {
            this.submittedForLabel = this.translationService.getValue('commercial-fishing.permit-submitted-for-panel');
            this.qualifiedFisherSameAsSubmittedForLabel = this.translationService.getValue('commercial-fishing.permit-qualified-fisher-same-as-submitted-for');
        }

        this.loader.load(() => {
            this.loadData();
        });
    }

    public setData(data: DialogParamsModel | CommercialFishingDialogParamsModel, buttons: DialogWrapperData): void {
        this.id = data.id;
        this.applicationId = data.applicationId;
        this.applicationsService = data.applicationsService;
        this.isApplication = data.isApplication;
        this.isReadonly = data.isReadonly;
        this.isApplicationHistoryMode = data.isApplicationHistoryMode;
        this.viewMode = data.viewMode;
        this.showOnlyRegiXData = data.showOnlyRegiXData;
        this.showRegiXData = data.showRegiXData;
        this.service = data.service as ICommercialFishingService;

        this.getLogBookAuditMethod = this.service.getLogBookAudit.bind(this.service);

        if (!IS_PUBLIC_APP) {
            this.administrationService = this.service as CommercialFishingAdministrationService;
        }

        this.dialogRightSideActions = buttons.rightSideActions;
        this.saveApplicationDraftContentActionClicked = buttons.rightSideActions!.find(x => x.id === 'save-draft-content')?.buttonData?.callbackFn;
        this.pageCode = data.pageCode;
        this.onRecordAddedOrEdittedEvent = data.onRecordAddedOrEdittedEvent;
        this.loadRegisterFromApplication = data.loadRegisterFromApplication;

        if (this.pageCode === PageCodeEnum.RightToFishThirdCountry) {
            this.isThirdCountryPermit = true;
        }
        else {
            this.isThirdCountryPermit = false;
        }

        if (this.pageCode === PageCodeEnum.CommFish || this.pageCode === PageCodeEnum.RightToFishThirdCountry || this.pageCode === PageCodeEnum.PoundnetCommFish) {
            this.isPermitLicense = false;
            this.canReadSuspensions = this.permissionsService.has(PermissionsEnum.PermitSuspensionRead);
        }
        else {
            this.isPermitLicense = true;
            this.canReadSuspensions = this.permissionsService.has(PermissionsEnum.PermitLicenseSuspensionRead);
        }

        this.hideSubmittedByDocument = this.pageCode === PageCodeEnum.CatchQuataSpecies;
        this.isQualifiedFisherPhotoRequired = this.pageCode !== PageCodeEnum.CatchQuataSpecies && this.pageCode !== PageCodeEnum.PoundnetCommFish;

        this.buildForm();

        if (data instanceof CommercialFishingDialogParamsModel) {
            this.model = data.model;
            this.modelLoadedFromPermit = true;
            if (this.model instanceof CommercialFishingApplicationEditDTO) {
                this.model.pageCode = this.pageCode;
            }
            else if (this.model.pageCode === PageCodeEnum.FishingGearsCommFish) {
                this.isFishingGearsApplication = true;
            }
        }
    }

    public async saveBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): Promise<void> {
        this.form.markAllAsTouched();
        this.validityCheckerGroup.validate();

        if (this.form.valid && !this.viewMode && !this.isReadonly) {
            if (this.isFishingGearsApplication) {
                this.openFishingGearsApplicationConfirmationDialog().subscribe({ //Завършване на заявление за маркиране на риболовни уреди
                    next: (ok: boolean) => {
                        if (ok) {
                            this.saveOrPrintCommercialFishingRecord(actionInfo.id, dialogClose);
                        }
                    }
                });
            }
            else if (this.isPermitLicense && !this.isApplication && this.onlyOnlineLogBooks) { // В случай че имаме удостоверение само за онлайн (риболовни) дневници
                const logBooks: CommercialFishingLogBookEditDTO[] = this.form.get('logBooksControl')!.value;

                if (logBooks === null
                    || logBooks === undefined
                    || logBooks.length === 0
                    || !logBooks.some(x => x.isActive)
                ) {
                    this.openNoActiveElectronicLogBookConfirmationDialog().subscribe({
                        next: (ok: boolean) => {
                            if (ok) {
                                this.saveOrPrintCommercialFishingRecord(actionInfo.id, dialogClose);
                            }
                        }
                    });
                }
                else {
                    const logBookTypes = await NomenclatureStore.instance.getNomenclature<number>(
                        NomenclatureTypes.LogBookTypes, this.nomenclaturesService.getLogBookTypes.bind(this.nomenclaturesService), false
                    ).toPromise();

                    const logBookStatuses = await NomenclatureStore.instance.getNomenclature<number>(
                        NomenclatureTypes.LogBookStatuses, this.nomenclaturesService.getLogBookStatuses.bind(this.nomenclaturesService), false
                    ).toPromise();

                    const shipLogBookTypeIds: number[] = logBookTypes
                        .filter(x => x.code === LogBookTypesEnum[LogBookTypesEnum.Ship])
                        .map(x => x.value!);

                    const activeLogBookStatusIds: number[] = logBookStatuses
                        .filter(x => x.code === LogBookStatusesEnum[LogBookStatusesEnum.New]
                            || x.code === LogBookStatusesEnum[LogBookStatusesEnum.Renewed])
                        .map(x => x.value!);

                    const shipLogBooks = logBooks.filter(x => x.isActive && x.isOnline && shipLogBookTypeIds.includes(x.logBookTypeId!));

                    const activeShipLogBooks = shipLogBooks.filter(x => activeLogBookStatusIds.includes(x.statusId!));

                    if (activeShipLogBooks === null
                        || activeShipLogBooks === undefined
                        || activeShipLogBooks.length === 0
                    ) { // Няма активни/презаверени риболовни дневници
                        this.openNoActiveElectronicLogBookConfirmationDialog().subscribe({
                            next: (ok: boolean) => {
                                if (ok) {
                                    this.saveOrPrintCommercialFishingRecord(actionInfo.id, dialogClose);
                                }
                            }
                        });
                    }
                    else {
                        this.saveOrPrintCommercialFishingRecord(actionInfo.id, dialogClose);
                    }
                }
            }
            else {
                this.saveOrPrintCommercialFishingRecord(actionInfo.id, dialogClose);
            }
        }
        else if (actionInfo.id === 'print' && (this.viewMode || this.isReadonly) && this.model instanceof CommercialFishingEditDTO) {
            this.service.downloadRegister(this.model.id!, this.pageCode).subscribe();
        }
    }

    public cancelBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public async dialogButtonClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): Promise<void> {
        let applicationAction: boolean = false;

        if (actionInfo.id === 'suspend') {
            this.addSuspension(dialogClose);
        }
        else if (actionInfo.id === 'copy-data-from-old-permit-license') {
            this.copyDataFromOldPermitLicenseBtnClicked();
        }
        else if (actionInfo.id === 'copy-data-from-permit') {
            this.copyDataFromPermitBtnClicked();
        }
        else if (actionInfo.id === 'flux') {
            this.form.markAllAsTouched();
            this.validityCheckerGroup.validate();

            if (this.form.valid) {
                this.fillModel();
                this.model = CommonUtils.sanitizeModelStrings(this.model);

                this.service.downloadPermitFluxXml(this.model).subscribe();
            }
        }
        else {
            if (this.model instanceof CommercialFishingApplicationEditDTO
                || (this.model instanceof CommercialFishingRegixDataDTO && this.showOnlyRegiXData)
            ) {
                this.model = this.fillModel();
                CommonUtils.sanitizeModelStrings(this.model);

                if (this.model instanceof CommercialFishingApplicationEditDTO) {
                    if (actionInfo.id === 'save-draft-content' && (this.model.id === undefined || this.model.id === null)) {
                        if (this.model.qualifiedFisherPhoto && this.model.qualifiedFisherPhoto.file) {
                            const photo: string = await CommonUtils.getFileAsBase64(this.model.qualifiedFisherPhoto.file);
                            this.model.qualifiedFisherPhotoBase64 = photo;
                            this.model.qualifiedFisherPhoto = undefined;
                        }
                    }
                }

                applicationAction = ApplicationUtils.applicationDialogButtonClicked(new ApplicationDialogData({
                    action: actionInfo,
                    dialogClose: dialogClose,
                    applicationId: this.applicationId!,
                    model: this.model,
                    readOnly: this.isReadonly,
                    viewMode: this.viewMode,
                    editForm: this.form,
                    saveFn: this.saveCommercialFishingRecord.bind(this),
                    onMarkAsTouched: () => {
                        this.validityCheckerGroup.validate();
                    }
                }));
            }

            if (!this.isReadonly && !this.viewMode && !applicationAction) {
                this.form.markAllAsTouched();
                this.validityCheckerGroup.validate();

                if (this.form.valid) {
                    switch (actionInfo.id) {
                        case 'save':
                        case 'print':
                            return this.saveBtnClicked(actionInfo, dialogClose); break;
                        case 'save-and-start-permit-license':
                            return this.saveAndStartPermitLicenseBtnClicked(actionInfo, dialogClose); break;
                        default:
                            throw new Error(`Unknown action identifier in commercial fishing component: ${actionInfo.id}`);
                    }
                }
            }
            else if (!applicationAction && actionInfo.id === 'print') {
                return this.saveBtnClicked(actionInfo, dialogClose);
            }
        }
    }

    public addSuspension(dialogClose: DialogCloseCallback): void {
        const data: SuspnesionDataDialogParams = new SuspnesionDataDialogParams({
            isPermit: !this.isPermitLicense,
            recordId: this.id,
            pageCode: this.pageCode,
            service: this.service,
            postOnAdd: true,
            viewMode: false
        });
        const headerTitle: string = this.translationService.getValue('suspensions.add-suspension-dialog-title');
        const dialog = this.addSuspensionDialog.openWithTwoButtons({
            title: headerTitle,
            TCtor: EditSuspensionComponent,
            headerAuditButton: undefined,
            headerCancelButton: {
                cancelBtnClicked: (closeFn: HeaderCloseFunction) => { closeFn(); }
            },
            componentData: data,
            translteService: this.translationService,
            viewMode: false
        }, '850px');

        dialog.subscribe({
            next: (result: SuspensionDataDTO | undefined) => {
                if (result !== null && result !== undefined) {
                    // при добавяне на ново прекратяване през диалога за УСР/РСР, трябва да е запазено към базата вече и директно затваряме диалога
                    NomenclatureStore.instance.clearNomenclature(NomenclatureTypes.Ships);
                    dialogClose(this.model.id);
                }
            }
        });
    }

    public selectAllPermittedAquaticOrganisms(): void {
        this.selectedAquaticOrganismTypes = [];
        this.aquaticOrganismTypes = this.allAquaticOrganismTypes.slice();

        const selectedWaterType: NomenclatureDTO<number> | undefined = this.form.get('waterTypeControl')!.value;
        if (selectedWaterType !== null && selectedWaterType !== undefined) {
            this.fileterAquaticOrganismTypesByWaterType(WaterTypesEnum[selectedWaterType.code as keyof typeof WaterTypesEnum]);
        }

        this.updateSelectedAquaticOrganismTypes(this.aquaticOrganismTypes.slice());

        this.form.updateValueAndValidity({ onlySelf: true });
    }

    public deselectAllAquaticOrganisms(): void {
        this.selectedAquaticOrganismTypes = [];
        this.aquaticOrganismTypes = this.allAquaticOrganismTypes.slice();

        const selectedWaterType: NomenclatureDTO<number> | undefined = this.form.get('waterTypeControl')!.value;
        if (selectedWaterType !== null && selectedWaterType !== undefined) {
            this.fileterAquaticOrganismTypesByWaterType(WaterTypesEnum[selectedWaterType.code as keyof typeof WaterTypesEnum]);
        }

        this.form.updateValueAndValidity({ onlySelf: true });
    }

    public selectAllPermittedPorts(): void {
        this.quotaAquaticOrganisms = [];

        for (const quotaOrganismType of this.quotaAquaticOrganismTypes) {
            const ports: NomenclatureDTO<number>[] = this.filterQuotaSpiciesPortsCollection(quotaOrganismType.value);

            for (const port of ports) {
                const quotaAquaticOrganism: QuotaAquaticOrganismDTO = new QuotaAquaticOrganismDTO({
                    aquaticOrganismId: quotaOrganismType.value,
                    portId: port.value
                });

                this.quotaAquaticOrganisms.push(quotaAquaticOrganism);
            }
        }

        this.quotaAquaticOrganisms = this.quotaAquaticOrganisms.slice();
        this.form.updateValueAndValidity({ onlySelf: true });
        this.form.markAsTouched({ onlySelf: true });
    }

    public removeAquaticOrganismType(row: NomenclatureDTO<number>): void {
        const selectedWaterType: NomenclatureDTO<number> | undefined = this.form.get('waterTypeControl')!.value;
        if (selectedWaterType !== null && selectedWaterType !== undefined) {
            this.fileterAquaticOrganismTypesByWaterType(WaterTypesEnum[selectedWaterType.code as keyof typeof WaterTypesEnum]);
        }

        this.selectedAquaticOrganismTypes = this.selectedAquaticOrganismTypes.filter(x => x.value !== row.value).slice();
        this.aquaticOrganismTypes = this.aquaticOrganismTypes.filter(x => !this.selectedAquaticOrganismTypes.includes(x));

        if (this.isApplication
            && this.isPaid
            && !this.isReadonly
            && !this.viewMode
            && this.isPermitLicense
        ) { // update applied tariffs based on selected ship
            this.updatePermitLicenseAppliedTariffs();
        }

        this.form.updateValueAndValidity({ onlySelf: true });
    }

    public quotaAquaticOrganismChanged(event: RecordChangedEventArgs<QuotaAquaticOrganismDTO> | undefined): void {
        switch (event!.Command) {
            case CommandTypes.Add:
            case CommandTypes.Edit:
            case CommandTypes.Delete:
            case CommandTypes.UndoDelete: {
                this.quotaAquaticOrganismTypes = this.filterQuotaAquaticOrganismTypesCollection();
            } break;
        }

        this.quotaAquaticOrganisms = this.quotaAquaticOrganismTypesTable.rows?.map((row: QuotaAquaticOrganismDTO) => {
            return new QuotaAquaticOrganismDTO({
                aquaticOrganismId: row.aquaticOrganismId,
                portId: row.portId
            });
        }) ?? [];

        this.form.updateValueAndValidity({ onlySelf: true });
        this.form.markAsTouched({ onlySelf: true });
    }

    public getControlErrorLabelText(controlName: string, errorValue: unknown, errorCode: string): TLError | undefined {
        switch (errorCode) {
            case 'egn':
                if (errorValue === true) {
                    return new TLError({ text: this.translationService.getValue('regix-data.invalid-egn'), type: 'warn' });
                }
                break;
            case 'shipIsDestroyedOrDeregistered':
                if (errorValue === true) {
                    return new TLError({ text: this.translationService.getValue('commercial-fishing.ship-is-destroyed-or-deregistered-error'), type: 'error' });
                } break;
            case 'shipIsForbinnedForPermitsAndLicenses':
                if (errorValue === true) {
                    return new TLError({ text: this.translationService.getValue('commercial-fishing.ship-is-forbidden-for-permits-and-liceses-error'), type: 'error' });
                } break;
            case 'shipHasNoActiveFishQuota':
                if (errorValue === true) {
                    return new TLError({ text: this.translationService.getValue('commercial-fishing.ship-has-no-active-fish-quota-error'), type: 'error' });
                }
                break;
            case 'shipHasBlackSeaPermit':
                if (errorValue === true) {
                    return new TLError({ text: this.translationService.getValue('commercial-fishing.ship-has-black-sea-permit-error'), type: 'error' });
                } break;
            case 'shipHasDanubePermit':
                if (errorValue === true) {
                    return new TLError({ text: this.translationService.getValue('commercial-fishing.ship-has-danube-permit-error'), type: 'error' });
                } break;
            case 'shipHasNoBlackSeaPermit':
                if (errorValue === true) {
                    return new TLError({ text: this.translationService.getValue('commercial-fishing.ship-has-no-black-sea-permit-error'), type: 'error' });
                }
                break;
            case 'shipHasNoDanubePermit':
                if (errorValue === true) {
                    return new TLError({ text: this.translationService.getValue('commercial-fishing.ship-has-no-danube-permit-error'), type: 'error' });
                }
                break;
            case 'shipHasNoPoundNetPermit':
                if (errorValue === true) {
                    return new TLError({ text: this.translationService.getValue('commercial-fishing.ship-has-no-pound-net-permit-error'), type: 'error' });
                }
                break;
            case 'shipIsThirdParty':
                if (errorValue === true) {
                    return new TLError({ text: this.translationService.getValue('commercial-fishing.ship-is-third-country-error'), type: 'error' });
                }
                break;
            case 'shipIsNotThirdParty':
                if (errorValue === true) {
                    return new TLError({ text: this.translationService.getValue('commercial-fishing.ship-is-not-third-country-error'), type: 'error' });
                }
                break;
            case 'noPermitRegisterForPermitLicense':
                if (errorValue === true) {
                    return new TLError({ text: this.translationService.getValue('commercial-fishing.no-permit-register-for-permit-license'), type: 'error' });
                }
                break;
            case 'poundNetAlreadyHasPermit':
                if (errorValue === true) {
                    return new TLError({ text: this.translationService.getValue('commercial-fishing.pound-net-already-has-valid-permit'), type: 'error' });
                }
        }

        return undefined;
    }

    public fileTypeFilterFn(options: PermittedFileTypeDTO[]): PermittedFileTypeDTO[] {
        const pdfs: FileTypeEnum[] = [FileTypeEnum.SIGNEDAPPL, FileTypeEnum.APPLICATION_PDF];
        const offlines: FileTypeEnum[] = [FileTypeEnum.PAYEDFEE, FileTypeEnum.SCANNED_FORM];

        let result: PermittedFileTypeDTO[] = options;

        if (!this.isOnlineApplication) {
            result = result.filter(x => !pdfs.includes(FileTypeEnum[x.code as keyof typeof FileTypeEnum]));
        }

        if (this.isOnlineApplication) {
            if (this.isApplication && !this.isReadonly) {
                result = result.filter(x => !pdfs.includes(FileTypeEnum[x.code as keyof typeof FileTypeEnum]));
            }

            result = result.filter(x => !offlines.includes(FileTypeEnum[x.code as keyof typeof FileTypeEnum]));
        }

        return result;
    }

    public onQualifiedFisherIdNumberEnterDown(): void {
        this.qualifiedFisherSearchBtnClicked();
    }

    public qualifiedFisherSearchBtnClicked(): void {
        if (!this.isPublicApp) {
            const identifier: EgnLncDTO = this.form.get('qualifiedFisherIdNumberControl')!.value;
            const cached: PersonFullDataDTO | null | undefined = this.fisherCache.get(`${identifier.identifierType}|${identifier.egnLnc}`);

            if (cached !== undefined) {
                if (cached !== null) {
                    this.form.get('qualifiedFisherIdNumberControl')!.setValue(cached.person!.egnLnc);
                    this.form.get('qualifiedFisherFirstNameControl')!.setValue(cached.person!.firstName);
                    this.form.get('qualifiedFisherMiddleNameControl')!.setValue(cached.person!.middleName);
                    this.form.get('qualifiedFisherLastNameControl')!.setValue(cached.person!.lastName);
                }
            }
            else {
                this.service.tryGetQualifiedFisher(identifier.identifierType!, identifier.egnLnc!).subscribe({
                    next: (person: PersonFullDataDTO | undefined) => {
                        if (person) {
                            this.form.get('qualifiedFisherIdNumberControl')!.setValue(person.person!.egnLnc);
                            this.form.get('qualifiedFisherFirstNameControl')!.setValue(person.person!.firstName);
                            this.form.get('qualifiedFisherMiddleNameControl')!.setValue(person.person!.middleName);
                            this.form.get('qualifiedFisherLastNameControl')!.setValue(person.person!.lastName);

                            this.fisherCache.set(`${identifier.identifierType}|${identifier.egnLnc}`, person);
                        }
                        else {
                            this.fisherCache.set(`${identifier.identifierType}|${identifier.egnLnc}`, null);
                        }
                    }
                });
            }
        }
    }

    private async loadData(): Promise<void> {
        if (!this.showOnlyRegiXData) {
            this.maxNumberOfFishingGears = (await this.systemParametersService.systemParameters()).maxNumberFishingGears!;

            this.form.get('shipControl')!.valueChanges.subscribe({
                next: (ship: ShipNomenclatureDTO | undefined | string) => {
                    this.selectedShipAlreadyHasValidBlackSeaPermitError = false;
                    this.selectedShipAlreadyHasValidDanubePermitError = false;
                    this.selectedShipHasNoBlackSeaPermitError = false;
                    this.selectedShipHasNoDanubePermitError = false;
                    this.selectedShipHasNoPoundNetPermitError = false;
                    this.selectedShipIsThirdCountryError = false;
                    this.selectedShipIsNotThirdCountryError = false;
                    this.hasNoPermitRegisterForPermitLicenseError = false;
                    this.shipId = undefined;

                    if (!this.isPublicApp && this.isPermitLicense && this.isApplication && !this.showOnlyRegiXData) {
                        if (ship !== null && ship !== undefined && ship instanceof NomenclatureDTO) {
                            this.noShipSelected = false;
                            this.shipId = ship.value;
                            this.getShipPermitsNomenclature(ship.value!);
                        }
                        else {
                            this.allPermits = [];
                            this.permits = [];
                            this.form.get('permitLicensePermitControl')!.reset();
                            this.form.get('permitLicensePermitControl')!.updateValueAndValidity();
                            this.noShipSelected = true;
                            this.onlyOnlineLogBooks = undefined;
                        }
                    }

                    if (ship !== null && ship !== undefined && ship instanceof NomenclatureDTO && this.isPermitLicense && !this.isApplication) {
                        this.onlyOnlineLogBooks = ShipsUtils.hasErs(ship);
                    }

                    if ((ship instanceof NomenclatureDTO || ship === null || ship === undefined)
                        && this.isApplication
                        && this.isPaid
                        && !this.isReadonly
                        && !this.viewMode
                        && this.isPermitLicense
                    ) { // update applied tariffs based on selected ship
                        this.updatePermitLicenseAppliedTariffs();
                    }
                }
            });

            if (this.pageCode === PageCodeEnum.PoundnetCommFish || this.pageCode === PageCodeEnum.PoundnetCommFishLic) {
                this.form.get('poundNetControl')!.valueChanges.subscribe({
                    next: (poundNet: PoundNetNomenclatureDTO | undefined) => {
                        if (poundNet !== null && poundNet !== undefined && poundNet instanceof NomenclatureDTO) {
                            this.form.get('poundNetDepthControl')!.setValue(poundNet.depth);
                            this.form.get('poundNetStatusControl')!.setValue(poundNet.statusName);
                        }
                        else {
                            this.form.get('poundNetDepthControl')!.setValue(null);
                            this.form.get('poundNetStatusControl')!.setValue(null);
                        }

                        if ((poundNet instanceof NomenclatureDTO || poundNet === null || poundNet === undefined)
                            && this.pageCode === PageCodeEnum.PoundnetCommFishLic
                            && this.isApplication
                            && this.isPaid
                            && !this.isReadonly
                            && !this.viewMode
                        ) { // update applied tariffs based on selected ship
                            this.updatePermitLicenseAppliedTariffs();
                        }
                    }
                });
            }

            if (this.isApplication === true && this.isPermitLicense) {
                // make autocomplete for permits readonly
            }
        }

        if (!this.isReadonly && !this.viewMode) {
            if (!this.showOnlyRegiXData) {
                this.form.get('submittedForControl')!.valueChanges.subscribe({
                    next: (value: ApplicationSubmittedForDTO) => {
                        if (value !== null && value !== undefined) {
                            this.hasSubmittedForNotShipOwnerError = false;
                            this.submittedByRole = value.submittedByRole;

                            if (this.submittedByRole !== SubmittedByRolesEnum.Personal && this.submittedByRole !== SubmittedByRolesEnum.PersonalRepresentative) {
                                this.form.get('qualifiedFisherSameAsSubmittedForControl')!.setValue(false);
                                this.form.get('qualifiedFisherSameAsSubmittedForControl')!.disable({ emitEvent: false });
                            }
                            else {
                                this.form.get('qualifiedFisherSameAsSubmittedForControl')!.enable({ emitEvent: false });
                                this.form.get('qualifiedFisherSameAsSubmittedForControl')!.setValue(false);
                            }

                            this.checkAndSetQualifiedFisherValues();
                        }
                    }
                });

                if (this.isApplication) {
                    this.form.get('qualifiedFisherIdNumberControl')!.valueChanges.subscribe({
                        next: () => {
                            this.hasCaptainNotQualifiedFisherError = false;
                        }
                    });

                    this.form.get('qualifiedFisherFirstNameControl')!.valueChanges.subscribe({
                        next: () => {
                            this.hasCaptainNotQualifiedFisherError = false;
                        }
                    });

                    this.form.get('qualifiedFisherMiddleNameControl')!.valueChanges.subscribe({
                        next: () => {
                            this.hasCaptainNotQualifiedFisherError = false;
                        }
                    });

                    this.form.get('qualifiedFisherLastNameControl')!.valueChanges.subscribe({
                        next: () => {
                            this.hasCaptainNotQualifiedFisherError = false;
                        }
                    });

                    this.form.get('deliveryDataControl')!.valueChanges.subscribe({
                        next: () => {
                            this.hasNoEDeliveryRegistrationError = false;
                        }
                    });

                    if (this.isPublicApp && this.isPermitLicense) {
                        this.form.get('permitLicensePermitNumberControl')!.valueChanges.subscribe({
                            next: () => {
                                this.hasInvalidPermitRegistrationNumber = false;
                            }
                        });
                    }
                }
                else {
                    this.form.get('qualifiedFisherControl')!.valueChanges.subscribe({
                        next: (qualifiedFisher: QualifiedFisherNomenclatureDTO | undefined) => {
                            if (qualifiedFisher !== null && qualifiedFisher !== undefined && qualifiedFisher instanceof NomenclatureDTO) {
                                this.form.get('qualifiedFisherRegistrationNumberControl')!.setValue(qualifiedFisher.registrationNumber);
                            }
                            else {
                                this.form.get('qualifiedFisherRegistrationNumberControl')!.setValue(null);
                            }
                        }
                    });

                    this.form.get('suspensionsControl')!.valueChanges.subscribe({
                        next: (suspensions: SuspensionDataDTO[]) => {
                            this.updateIsSuspendedFlag();
                            this.hasShipEventExistsOnSameDateError = false;
                            this.form.updateValueAndValidity({ onlySelf: true });
                        }
                    });
                }

                this.form.get('qualifiedFisherSameAsSubmittedForControl')!.valueChanges.subscribe({
                    next: (_: boolean) => {
                        this.checkAndSetQualifiedFisherValues();
                    }
                });

                this.form.get('submittedByControl')!.valueChanges.subscribe({
                    next: (_: ApplicationSubmittedByDTO) => {
                        if (this.submittedByRole === SubmittedByRolesEnum.Personal) {
                            this.hasSubmittedForNotShipOwnerError = false;
                        }
                        this.checkAndSetQualifiedFisherValues();
                    }
                });
            }

            if (!this.isApplication && !this.showOnlyRegiXData && !this.isPermitLicense) {
                this.form.get('isPermitUnlimitedControl')!.valueChanges.subscribe({
                    next: (isUnlimited: boolean) => {
                        if (isUnlimited) {
                            this.form.get('validFromControl')!.reset();
                            this.form.get('validFromControl')!.clearValidators();

                            this.form.get('validToControl')!.reset();
                            this.form.get('validToControl')!.clearValidators();

                            this.form.get('validFromUnlimitedControl')!.setValidators(Validators.required);
                            this.form.get('validFromUnlimitedControl')!.markAsPending();
                        }
                        else {
                            this.form.get('validFromControl')!.setValidators(Validators.required);
                            this.form.get('validToControl')!.setValidators(Validators.required);

                            this.form.get('validFromUnlimitedControl')!.clearValidators();
                            this.form.get('validFromUnlimitedControl')!.reset();
                        }

                        this.form.get('validFromControl')!.markAsPending();
                        this.form.get('validFromControl')!.updateValueAndValidity({ emitEvent: false });

                        this.form.get('validToControl')!.markAsPending();
                        this.form.get('validToControl')!.updateValueAndValidity({ emitEvent: false });
                    }
                });
            }

            if (!this.showOnlyRegiXData) {
                if (this.pageCode === PageCodeEnum.CatchQuataSpecies) {
                    this.quotaAquaticOrganismsForm!.get('aquaticOrganismIdControlHidden')!.valueChanges.subscribe({
                        next: (aquaticOrganism: FishNomenclatureDTO) => {
                            this.ports = this.filterQuotaSpiciesPortsCollection(aquaticOrganism?.value ?? undefined);
                        }
                    });
                }
                else if (this.isPermitLicense) {
                    this.aquaticOrganismTypesControl.valueChanges.subscribe({
                        next: (aquaticOrganismType: NomenclatureDTO<number> | string | undefined | null) => {
                            this.updateSelectedAquaticOrganismTypes(aquaticOrganismType);
                            this.form.updateValueAndValidity({ onlySelf: true });
                        }
                    });
                }

                if (this.isPermitLicense === true) {
                    this.form.get('fishingGearsGroup.fishingGearsControl')!.valueChanges.subscribe({
                        next: (value: FishingGearDTO[] | undefined) => {
                            this.form.updateValueAndValidity({ emitEvent: false });

                            if (this.isApplication
                                && this.isPaid
                                && !this.isReadonly
                                && !this.viewMode
                            ) { // update applied tariffs based on selected ship
                                this.updatePermitLicenseAppliedTariffs();
                            }
                        }
                    });
                }
            }
        }

        //заявление за маркиране на риболовни уреди
        if (this.isFishingGearsApplication && this.applicationId !== undefined && this.applicationId !== null) {
            this.form.disable();
            this.aquaticOrganismTypesControl.disable();
            this.form.get('fishingGearsGroup.fishingGearsControl')!.enable();

            this.fillForm();
        }
        else if (this.modelLoadedFromPermit === true && this.model !== null && this.model !== undefined) {
            if (this.model instanceof CommercialFishingApplicationEditDTO) {
                this.model.pageCode = this.pageCode;

                this.hasDelivery = this.model.hasDelivery ?? false;
                this.isPaid = this.model.isPaid ?? false;
                this.isOnlineApplication = this.model.isOnlineApplication!;
            }

            this.fillForm();
        }
        else {
            // извличане на исторически данни за заявление
            if (this.isApplicationHistoryMode && this.applicationId !== undefined) {
                this.form.disable();

                if (this.applicationsService) {
                    this.applicationsService.getApplicationChangeHistoryContent(this.applicationId).subscribe({
                        next: (content: ApplicationContentDTO) => {
                            const contentObject: Record<string, unknown> = JSON.parse(content.draftContent!);
                            const commercialFishingApplication: CommercialFishingApplicationEditDTO = new CommercialFishingApplicationEditDTO(contentObject);
                            commercialFishingApplication.files = content.files;
                            commercialFishingApplication.applicationId = content.applicationId;

                            this.isOnlineApplication = commercialFishingApplication.isOnlineApplication!;
                            this.refreshFileTypes.next();

                            this.model = commercialFishingApplication;

                            if (this.model instanceof CommercialFishingApplicationEditDTO) {
                                this.model.pageCode = this.pageCode;

                                this.hasDelivery = this.model.hasDelivery ?? false;
                                this.isPaid = this.model.isPaid ?? false;
                                this.isOnlineApplication = this.model.isOnlineApplication!;
                            }
                            this.fillForm();

                            if (!this.isPublicApp && content.latestRegiXChecks !== undefined && content.latestRegiXChecks !== null && content.latestRegiXChecks.length > 0) {
                                this.showRegiXData = true;

                                setTimeout(() => {
                                    this.regixChecks = content.latestRegiXChecks!;
                                }, 100);
                            }
                        }
                    });
                }
            }
            else if (this.applicationId !== undefined && this.applicationId !== null && (this.id === undefined || this.id === null) && !this.isApplication) {
                if (this.loadRegisterFromApplication === true) {  // извличане на данни за регистър по id на заявление
                    if (this.isReadonly || this.viewMode) {
                        this.form.disable();
                    }
                    this.isEditing = true;
                    this.isEditingSubmittedBy = true;

                    this.service.getRegisterByApplicationId(this.applicationId, this.pageCode).subscribe({
                        next: (commercialFishingRegister: unknown) => {
                            this.model = new CommercialFishingEditDTO(commercialFishingRegister as CommercialFishingEditDTO);

                            if (this.model instanceof CommercialFishingEditDTO) {
                                this.model.pageCode = this.pageCode;

                                if (this.pageCode == PageCodeEnum.PoundnetCommFishLic
                                    || this.pageCode == PageCodeEnum.RightToFishResource
                                    || this.pageCode == PageCodeEnum.CatchQuataSpecies
                                ) {
                                    this.setPermitLicenseIsValidFlag(this.model);
                                }
                            }

                            this.isOnlineApplication = (commercialFishingRegister as CommercialFishingEditDTO).isOnlineApplication!;
                            this.refreshFileTypes.next();

                            this.fillForm();
                        }
                    });
                }
                else { // извличане на данни за създаване на регистров запис от заявление
                    this.isEditing = false;
                    this.isEditingSubmittedBy = false;
                    this.isAddingRegister = true;

                    this.service.getApplicationDataForRegister(this.applicationId, this.pageCode).subscribe({
                        next: (commercialFishingRegister: CommercialFishingEditDTO) => {
                            this.model = commercialFishingRegister;

                            if (this.model instanceof CommercialFishingEditDTO) {
                                this.model.pageCode = this.pageCode;

                                if (this.pageCode == PageCodeEnum.PoundnetCommFishLic
                                    || this.pageCode == PageCodeEnum.RightToFishResource
                                    || this.pageCode == PageCodeEnum.CatchQuataSpecies
                                ) {
                                    this.setPermitLicenseIsValidFlag(this.model);
                                }

                                if (this.isPermitLicense) {
                                    this.permitLicenseRegisterId = this.model.id;
                                }
                            }

                            this.isOnlineApplication = commercialFishingRegister.isOnlineApplication!;
                            this.refreshFileTypes.next();
                            this.fillForm();
                        }
                    });
                }
            }
            else {
                if (this.isReadonly || this.viewMode) {
                    this.form.disable();
                }

                if (this.isApplication && this.applicationId !== null && this.applicationId !== undefined) {
                    this.isEditing = false;
                    this.isEditingSubmittedBy = false;

                    if (this.showOnlyRegiXData) { // извличане на данни за RegiX сверка от служител
                        this.service.getRegixData(this.applicationId, this.pageCode).subscribe({
                            next: (regixData: RegixChecksWrapperDTO<CommercialFishingRegixDataDTO>) => {
                                this.model = new CommercialFishingRegixDataDTO(regixData.dialogDataModel);
                                this.expectedResults = new CommercialFishingRegixDataDTO(regixData.regiXDataModel);
                                this.expectedResults.applicationId = this.applicationId;
                                this.expectedResults.id = this.model.id;
                                this.expectedResults.submittedBy = regixData.regiXDataModel?.submittedBy ?? new ApplicationSubmittedByRegixDataDTO({
                                    addresses: [], person: new RegixPersonDataDTO()
                                });
                                this.expectedResults.submittedFor = regixData.regiXDataModel?.submittedFor ?? new ApplicationSubmittedForRegixDataDTO({
                                    addresses: [],
                                    legal: new RegixLegalDataDTO(),
                                    person: new RegixPersonDataDTO()
                                });

                                this.fillForm();
                            }
                        });
                    }
                    else {
                        // извличане на данни за заявление
                        this.isEditing = false;
                        this.isEditingSubmittedBy = false;

                        this.service.getApplication(this.applicationId, this.showRegiXData, this.pageCode).subscribe({
                            next: (commercialFishingApplication: CommercialFishingApplicationEditDTO) => {
                                if (commercialFishingApplication === null || commercialFishingApplication === undefined) {
                                    commercialFishingApplication = new CommercialFishingApplicationEditDTO({
                                        applicationId: this.applicationId,
                                        qualifiedFisherSameAsSubmittedFor: false,
                                        isHolderShipOwner: true
                                    });
                                }
                                else {
                                    if (commercialFishingApplication.applicationId === null || commercialFishingApplication.applicationId === undefined) {
                                        commercialFishingApplication.applicationId = this.applicationId;
                                        commercialFishingApplication.qualifiedFisherSameAsSubmittedFor = false;
                                        commercialFishingApplication.isHolderShipOwner = true;
                                    }
                                }

                                this.model = commercialFishingApplication;
                                this.refreshFileTypes.next();

                                if (this.showRegiXData) {
                                    this.expectedResults = new CommercialFishingRegixDataDTO(commercialFishingApplication.regiXDataModel);
                                    commercialFishingApplication.regiXDataModel = undefined;
                                }

                                if (this.model instanceof CommercialFishingApplicationEditDTO) {
                                    this.model.pageCode = this.pageCode;
                                    this.hasDelivery = this.model.hasDelivery ?? false;
                                    this.isPaid = this.model.isPaid ?? false;
                                    this.isOnlineApplication = this.model.isOnlineApplication!;

                                    if (this.isPublicApp && this.isOnlineApplication) {
                                        this.isEditingSubmittedBy = true;

                                        if (this.model.submittedBy === undefined || this.model.submittedBy === null) {
                                            const service = this.service as CommercialFishingPublicService;
                                            service.getCurrentUserAsSubmittedBy().subscribe({
                                                next: (submittedBy: ApplicationSubmittedByDTO) => {
                                                    (this.model as CommercialFishingApplicationEditDTO).submittedBy = submittedBy;
                                                    this.fillForm();
                                                }
                                            });
                                        }
                                        else {
                                            this.fillForm();
                                        }
                                    }
                                    else {
                                        this.fillForm();
                                    }
                                }
                            }
                        });
                    }
                }
                else if (this.id !== undefined && this.id !== null) {
                    // извличане на данни за регистров запис
                    this.isEditing = true;
                    this.isEditingSubmittedBy = true;
                    this.isRegisterEntry = true;

                    this.service.getRecord(this.id, this.pageCode).subscribe({
                        next: (commercialFishingRecord: CommercialFishingEditDTO) => {
                            this.model = commercialFishingRecord;

                            if (this.model instanceof CommercialFishingEditDTO) {
                                this.model.pageCode = this.pageCode;

                                if (this.pageCode == PageCodeEnum.PoundnetCommFishLic
                                    || this.pageCode == PageCodeEnum.RightToFishResource
                                    || this.pageCode == PageCodeEnum.CatchQuataSpecies
                                ) {
                                    this.setPermitLicenseIsValidFlag(this.model);
                                }

                                if (this.isPermitLicense) {
                                    this.permitLicenseRegisterId = this.model.id;
                                }
                            }

                            this.isOnlineApplication = commercialFishingRecord.isOnlineApplication!;
                            this.refreshFileTypes.next();
                            this.fillForm();
                        }
                    });
                }
            }
        }
    }

    private fileterAquaticOrganismTypesByWaterType(waterTypeCode: WaterTypesEnum | undefined): void {
        if (waterTypeCode !== null && waterTypeCode !== undefined) {
            switch (waterTypeCode) {
                case WaterTypesEnum.BLACK_SEA: {
                    setTimeout(() => {
                        this.selectedAquaticOrganismTypes = this.selectedAquaticOrganismTypes.filter(x => x.isBlackSea);
                        this.aquaticOrganismTypes = this.allAquaticOrganismTypes.filter(x => x.isBlackSea && !this.selectedAquaticOrganismTypes.includes(x))
                        this.waterAquaticOrganismTypes = this.allAquaticOrganismTypes.filter(x => x.isBlackSea);
                    });
                } break;
                case WaterTypesEnum.DANUBE: {
                    setTimeout(() => {
                        this.selectedAquaticOrganismTypes = this.selectedAquaticOrganismTypes.filter(x => x.isDanube);
                        this.aquaticOrganismTypes = this.allAquaticOrganismTypes.filter(x => x.isDanube && !this.selectedAquaticOrganismTypes.includes(x))
                        this.waterAquaticOrganismTypes = this.allAquaticOrganismTypes.filter(x => x.isDanube);
                    });
                } break;
            }
        }
    }

    private filterQuotaAquaticOrganismTypesCollection(): FishNomenclatureDTO[] {
        const quotaDate: Date = this.isApplication || this.model?.id === null || this.model?.id === undefined
            ? new Date()
            : (this.model as CommercialFishingEditDTO).validFrom!;

        this.quotaAquaticOrganismTypes = this.allAquaticOrganismTypes
            .filter(x => (x.quotas ?? [])
                .some(x => x.periodFrom!.getTime() <= quotaDate.getTime() && x.periodTo!.getTime() >= quotaDate.getTime()));

        return this.quotaAquaticOrganismTypes.slice();
    }

    private filterQuotaSpiciesPortsCollection(aquaticOrganismId: number | undefined): NomenclatureDTO<number>[] {
        if (aquaticOrganismId !== undefined && aquaticOrganismId !== null) {
            const quotaDate: Date = this.isApplication || this.model?.id === null || this.model?.id === undefined
                ? new Date()
                : (this.model as CommercialFishingEditDTO).validFrom!;

            const fishes: FishNomenclatureDTO[] = this.allAquaticOrganismTypes
                .filter(x => x.value === aquaticOrganismId && (x.quotas ?? [])
                    .some(x => x.periodFrom!.getTime() <= quotaDate.getTime()
                        && x.periodTo!.getTime() >= quotaDate.getTime()));

            let quotas: FishQuotaDTO[] = fishes.map(x => x.quotas).reduce(x => x) ?? [];
            quotas = quotas.filter(x => x.periodFrom!.getTime() <= quotaDate.getTime()
                && x.periodTo!.getTime() >= quotaDate.getTime());

            const permittedPorts: QuotaSpiciesPortDTO[] = quotas
                .map(x => x.permittedPorts ?? [])
                .reduce(x => x);

            const ports: NomenclatureDTO<number>[] = this.allPorts.filter(x => permittedPorts.some(y => y.portId === x.value));

            for (const port of ports) {
                port.isActive = port.isActive && permittedPorts.find(x => x.portId === port.value)!.isActive;
            }

            return ports;
        }
        else {
            return [];
        }
    }

    private copyDataFromOldPermitLicenseBtnClicked(): void {
        const editDialogData: ChoosePermitLicenseForRenewalDialogParams = new ChoosePermitLicenseForRenewalDialogParams({
            permitId: this.form.get('permitLicensePermitControl')?.value?.value,
            permitNumber: this.form.get('permitLicensePermitNumberControl')?.value,
            shipId: this.form.get('shipControl')!.value?.value,
            ships: this.ships,
            pageCode: this.pageCode,
            service: this.service
        });

        this.choosePermitLicenseForRenewalDialog.open({
            title: this.translationService.getValue('commercial-fishing.choose-permit-license-for-reneal-dialog-title'),
            TCtor: ChoosePermitLicenseForRenewalComponent,
            headerCancelButton: {
                cancelBtnClicked: (closeFn: HeaderCloseFunction) => { closeFn(); }
            },
            componentData: editDialogData,
            translteService: this.translationService,
            disableDialogClose: true,
            saveBtn: {
                id: 'save',
                color: 'accent',
                translateValue: 'commercial-fishing.save-copy-data-from-old-permit-license-btn'
            },
            cancelBtn: {
                id: 'cancel',
                color: 'primary',
                translateValue: 'common.cancel',
            },
            viewMode: false
        }).subscribe({
            next: (chosenPermitLicenseId: number | undefined) => {
                if (chosenPermitLicenseId !== null && chosenPermitLicenseId !== undefined) {
                    this.service.getPermitLicenseData(chosenPermitLicenseId).subscribe({
                        next: (result: CommercialFishingApplicationEditDTO) => {
                            this.renewalPermitId = undefined;
                            this.renewalPermitLicenseId = chosenPermitLicenseId;
                            this.setResultToApplicationModelData(result);
                        }
                    });
                }
            }
        });
    }

    private copyDataFromPermitBtnClicked(): void {
        const editDialogData: ChoosePermitToCopyFromDialogParams = new ChoosePermitToCopyFromDialogParams({
            permitId: this.form.get('permitLicensePermitControl')?.value?.value,
            permitNumber: this.form.get('permitLicensePermitNumberControl')?.value,
            shipId: this.form.get('shipControl')!.value?.value,
            ships: this.ships,
            pageCode: this.pageCode,
            service: this.service
        });

        this.choosePermitToCopyFromDialog.open({
            title: this.translationService.getValue('commercial-fishing.choose-permit-to-copy-from-dialog-title'),
            TCtor: ChoosePermitToCopyFromComponent,
            headerCancelButton: {
                cancelBtnClicked: (closeFn: HeaderCloseFunction) => closeFn()
            },
            componentData: editDialogData,
            translteService: this.translationService,
            disableDialogClose: true,
            saveBtn: {
                id: 'save',
                color: 'accent',
                translateValue: 'commercial-fishing.copy-data-from-permit'
            },
            cancelBtn: {
                id: 'cancel',
                color: 'primary',
                translateValue: 'common.cancel',
            },
            viewMode: false
        }).subscribe({
            next: (result: ChoosePermitToCopyFromDialogResult | undefined) => {
                if (result !== null && result !== undefined) {
                    if (result.permitNumber !== null && result.permitNumber !== undefined) {
                        this.service.getPermitLicenseApplicationDataFromPermitNumber(result.permitNumber, this.applicationId!).subscribe({
                            next: (result: CommercialFishingApplicationEditDTO) => {
                                this.renewalPermitLicenseId = undefined;
                                this.renewalPermitId = result.permitLicensePermitId;
                                this.setResultToApplicationModelData(result);
                            },
                            error: (errorResponse: HttpErrorResponse) => {
                                if ((errorResponse.error as ErrorModel)?.code === ErrorCode.InvalidPermitNumber) {
                                    const msg: string = '';
                                    this.snackbar.open(msg, undefined, {
                                        duration: RequestProperties.DEFAULT.showExceptionDurationErr,
                                        panelClass: RequestProperties.DEFAULT.showExceptionColorClassErr
                                    });
                                }
                            }
                        });
                    }
                    else if (result.permitId !== null && result.permitId !== undefined) {
                        this.service.getPermitLicenseApplicationDataFromPermitId(result.permitId, this.applicationId!).subscribe({
                            next: (result: CommercialFishingApplicationEditDTO) => {
                                this.renewalPermitLicenseId = undefined;
                                this.renewalPermitId = result.permitLicensePermitId;
                                this.setResultToApplicationModelData(result);
                            }
                        });
                    }
                }
            }
        });
    }

    private setResultToApplicationModelData(result: CommercialFishingApplicationEditDTO): void {
        const originalPaymentInformation = (this.model as CommercialFishingApplicationEditDTO).paymentInformation; // save payment information
        const originalIsPaid: boolean = (this.model as CommercialFishingApplicationEditDTO).isPaid ?? false;
        const originalHasDelivery: boolean = (this.model as CommercialFishingApplicationEditDTO).hasDelivery ?? false;
        const id: number | undefined = (this.model as CommercialFishingApplicationEditDTO).id;

        this.model = result;

        (this.model as CommercialFishingApplicationEditDTO).paymentInformation = originalPaymentInformation;
        (this.model as CommercialFishingApplicationEditDTO).isPaid = originalIsPaid;
        (this.model as CommercialFishingApplicationEditDTO).hasDelivery = originalHasDelivery;
        (this.model as CommercialFishingApplicationEditDTO).id = id;

        if (this.model instanceof CommercialFishingApplicationEditDTO) {
            this.model.pageCode = this.pageCode;
            this.model.applicationId = this.applicationId;

            this.hasDelivery = this.model.hasDelivery ?? false;
            this.isOnlineApplication = this.model.isOnlineApplication!;
        }

        this.fillForm();
    }

    private saveOrPrintCommercialFishingRecord(actionId: string | undefined, dialogClose: DialogCloseCallback): void {
        if (actionId === 'print') {
            this.saveAndPrintRecord(dialogClose);
        }
        else {
            this.saveCommercialFishingRecord(dialogClose);
        }
    }

    private openNoActiveElectronicLogBookConfirmationDialog(): Observable<boolean> {
        return this.confirmDialog.open({
            title: this.translationService.getValue('commercial-fishing.save-permit-license-with-no-online-log-book-confirmation-title'),
            message: this.translationService.getValue('commercial-fishing.save-permit-license-with-no-online-log-book-confirmation-message'),
            okBtnLabel: this.translationService.getValue('commercial-fishing.save-permit-license-with-no-online-log-book-ok-label'),
            okBtnColor: 'warn',
            cancelBtnLabel: this.translationService.getValue('commercial-fishing.save-permit-license-with-no-online-log-book-cancel-label')
        });
    }

    private openFishingGearsApplicationConfirmationDialog(): Observable<boolean> {
        return this.confirmDialog.open({
            title: this.translationService.getValue('commercial-fishing.complete-application-confirm-dialog-title'),
            message: this.translationService.getValue('commercial-fishing.complete-application-confirm-dialog-message'),
            okBtnLabel: this.translationService.getValue('commercial-fishing.complete-application-confirm-dialog-ok-btn-label')
        });
    }

    private saveAndStartPermitLicenseBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        this.model = this.fillModel();
        CommonUtils.sanitizeModelStrings(this.model);
        this.service.addPermitApplicationAndStartPermitLicenseApplication(this.model).subscribe({
            next: async (permitLicenseModel: CommercialFishingApplicationEditDTO) => {
                NomenclatureStore.instance.clearNomenclature(NomenclatureTypes.Ships);

                if (this.pageCode === PageCodeEnum.PoundnetCommFish || this.pageCode === PageCodeEnum.PoundnetCommFishLic) {
                    NomenclatureStore.instance.clearNomenclature(NomenclatureTypes.PoundNets);
                }

                // Open payment dialog
                const paymentDialog = await this.openPaymentDialog(permitLicenseModel.applicationId!);

                paymentDialog.subscribe(async (paymentData: PaymentDataDTO) => {
                    if (paymentData !== null && paymentData !== undefined) {
                        // Open permit license dialog
                        if (permitLicenseModel.paymentInformation === null || permitLicenseModel.paymentInformation === undefined) {
                            permitLicenseModel.paymentInformation = new ApplicationPaymentInformationDTO();
                        }

                        const nomenclatures: NomenclatureDTO<number>[][] = await forkJoin([
                            NomenclatureStore.instance.getNomenclature<number>(NomenclatureTypes.OfflinePaymentTypes, this.commonNomenclatures.getOfflinePaymentTypes.bind(this.commonNomenclatures), false),
                            NomenclatureStore.instance.getNomenclature<number>(NomenclatureTypes.PaymentStatuses, this.commonNomenclatures.getPaymentStatuses, false)
                        ]).toPromise();

                        const paymentTypes = nomenclatures[0];
                        const paymentStatuses = nomenclatures[1];

                        permitLicenseModel.paymentInformation.paymentType = paymentTypes.find(x => x.code === PaymentTypesEnum[paymentData.paymentType!])!.code;
                        permitLicenseModel.paymentInformation.paymentDate = paymentData.paymentDateTime;
                        permitLicenseModel.paymentInformation.referenceNumber = paymentData.paymentRefNumber;
                        permitLicenseModel.paymentInformation.totalPaidPrice = paymentData.totalPaidPrice;
                        permitLicenseModel.paymentInformation.lastUpdateDate = new Date();
                        permitLicenseModel.paymentInformation.paymentStatus = paymentStatuses.find(x => x.code === PaymentStatusesEnum[PaymentStatusesEnum.PaidOK])!.code; // би трябвало това да е статусът, защото е офлайн плащане и щом сме стигнали до тази стъпка, то е успешно

                        this.openPermitLicenseDialog(permitLicenseModel);
                    }

                    dialogClose();
                });
            }, error: (errorResponse: HttpErrorResponse) => {
                this.handleAddEditApplicationErrorResponse(errorResponse, dialogClose, 'saveAndStartPermitLicense');
            }
        });
    }

    private async openPaymentDialog(applicationId: number): Promise<Observable<any>> {
        const paymentTypes = await NomenclatureStore.instance.getNomenclature<number>(
            NomenclatureTypes.OfflinePaymentTypes,
            this.commonNomenclatures.getOfflinePaymentTypes.bind(this.commonNomenclatures),
            false).toPromise();

        const headerTitle: string = this.translationService.getValue('commercial-fishing.enter-permit-license-payment-data-dialog-title');
        const data = new PaymentDataInfo({
            paymentTypes: paymentTypes,
            viewMode: false,
            service: this.applicationsService,
            applicationId: applicationId,
            paymentDateMax: new Date()
        });

        return this.paymentDataDialog.openWithTwoButtons({
            title: headerTitle,
            TCtor: PaymentDataComponent,
            headerCancelButton: {
                cancelBtnClicked: (closeFn: HeaderCloseFunction) => { closeFn(); }
            },
            componentData: data,
            translteService: this.translationService
        }, '1200px');
    }

    private openPermitLicenseDialog(permitLicenseModel: CommercialFishingApplicationEditDTO): void {
        const editDialogData: CommercialFishingDialogParamsModel = new CommercialFishingDialogParamsModel({
            model: permitLicenseModel,
            applicationId: permitLicenseModel.applicationId,
            isApplication: true,
            isApplicationHistoryMode: false,
            isReadonly: false,
            viewMode: false,
            service: this.service,
            applicationsService: this.applicationsService,
            showOnlyRegiXData: false,
            pageCode: permitLicenseModel.pageCode,
            onRecordAddedOrEdittedEvent: this.onRecordAddedOrEdittedEvent
        });

        const auditButton: IHeaderAuditButton | undefined = undefined;

        const rightButtons = [
            {
                id: 'save-draft-content',
                color: 'accent',
                translateValue: 'applications-register.save-draft-content',
                buttonData: { callbackFn: this.saveApplicationDraftContentActionClicked }
            }, {
                id: 'save',
                color: 'accent',
                translateValue: 'applications-register.save-application'
            }
        ];

        const dialog = this.editCommercialFishingPermitLicenseDialog.open({
            title: this.translationService.getValue('commercial-fishing.edit-permit-license-application-dialog-title'),
            TCtor: EditCommercialFishingComponent,
            headerAuditButton: auditButton,
            headerCancelButton: {
                cancelBtnClicked: (closeFn: HeaderCloseFunction) => { closeFn(); }
            },
            componentData: editDialogData,
            translteService: this.translationService,
            disableDialogClose: true,
            cancelBtn: {
                id: 'cancel',
                color: 'primary',
                translateValue: 'common.cancel',
            },
            rightSideActionsCollection: rightButtons,
            viewMode: false
        }, '1400px');

        dialog.subscribe((result: CommercialFishingApplicationEditDTO | undefined) => {
            if (this.onRecordAddedOrEdittedEvent !== null && this.onRecordAddedOrEdittedEvent !== undefined) {
                this.onRecordAddedOrEdittedEvent.emit(result?.id);
            }
        });
    }

    private saveCommercialFishingRecord(dialogClose: DialogCloseCallback, fromSaveAsDraft: boolean = false): Observable<boolean> {
        const saveOrEditDone: EventEmitter<boolean> = new EventEmitter<boolean>();

        this.saveOrEdit(fromSaveAsDraft, this.pageCode).subscribe({
            next: (id: number | void) => {
                this.hasCaptainNotQualifiedFisherError = false;
                this.hasSubmittedForNotShipOwnerError = false;
                this.hasNoEDeliveryRegistrationError = false;
                this.hasInvalidPermitRegistrationNumber = false;
                if (typeof id === 'number' && id !== undefined) {
                    this.model.id = id;
                    dialogClose(this.model);
                }
                else {
                    dialogClose(this.model);
                }

                NomenclatureStore.instance.clearNomenclature(NomenclatureTypes.Ships);

                if (this.pageCode === PageCodeEnum.PoundnetCommFish || this.pageCode === PageCodeEnum.PoundnetCommFishLic) {
                    NomenclatureStore.instance.clearNomenclature(NomenclatureTypes.PoundNets);
                }

                saveOrEditDone.emit(true);
                saveOrEditDone.complete();
            },
            error: (errorResponse: HttpErrorResponse) => {
                this.handleAddEditApplicationErrorResponse(errorResponse, dialogClose, 'save');
            }
        });

        return saveOrEditDone.asObservable();
    }

    private saveAndPrintRecord(dialogClose: DialogCloseCallback): void {
        this.model = this.fillModel();
        CommonUtils.sanitizeModelStrings(this.model);
        let saveOrEditObservable: Observable<boolean>;

        if (this.id !== null && this.id !== undefined) {
            saveOrEditObservable = this.service.editAndDownloadRegister(this.model, this.pageCode, this.ignoreLogBookConflicts);
        }
        else {
            saveOrEditObservable = this.service.addAndDownloadRegister(this.model, this.pageCode, this.ignoreLogBookConflicts);
        }

        saveOrEditObservable.subscribe({
            next: (downloaded: boolean) => {
                if (downloaded === true) {
                    NomenclatureStore.instance.clearNomenclature(NomenclatureTypes.Ships);

                    if (this.pageCode === PageCodeEnum.PoundnetCommFish || this.pageCode === PageCodeEnum.PoundnetCommFishLic) {
                        NomenclatureStore.instance.clearNomenclature(NomenclatureTypes.PoundNets);
                    }

                    dialogClose(this.model);
                }
            },
            error: (errorResponse: HttpErrorResponse) => {
                this.handleAddEditApplicationErrorResponse(errorResponse, dialogClose, 'saveAndPrint');
            }
        });
    }

    private saveOrEdit(fromSaveAsDraft: boolean, pageCode: PageCodeEnum): Observable<number | void> {
        this.model = this.fillModel();
        CommonUtils.sanitizeModelStrings(this.model);
        let saveOrEditObservable: Observable<void | number>;

        if (this.model instanceof CommercialFishingEditDTO) {
            if (this.isFishingGearsApplication) {
                saveOrEditObservable = this.service.completePermitLicenseFishingGearsApplication(this.model);
            }
            else if (this.id !== undefined && this.id !== null) {
                saveOrEditObservable = this.service.editPermit(this.model, pageCode, this.ignoreLogBookConflicts);
            }
            else {
                saveOrEditObservable = this.service.addPermit(this.model, pageCode, this.ignoreLogBookConflicts);
            }
        }
        else {
            if (this.model.id !== undefined && this.model.id !== null) {
                saveOrEditObservable = this.service.editApplication(this.model, this.pageCode, fromSaveAsDraft);
            }
            else {
                saveOrEditObservable = this.service.addApplication(this.model, this.pageCode);
            }
        }

        return saveOrEditObservable;
    }

    private handleAddEditApplicationErrorResponse(errorResponse: HttpErrorResponse, dialogClose: DialogCloseCallback, saveMethod: SaveMethodType): void {
        if (errorResponse.error !== null
            && errorResponse.error !== undefined
            && errorResponse.error.messages !== null
            && errorResponse.error.messages !== undefined
        ) {
            const messages: string[] = errorResponse.error.messages;

            if (Array.isArray(messages) === true) {
                for (const message of messages) {
                    const validationError = CommercialFishingValidationErrorsEnum[message as keyof typeof CommercialFishingValidationErrorsEnum];
                    switch (validationError) {
                        case CommercialFishingValidationErrorsEnum.CaptainNotQualifiedFisherCheck: {
                            this.hasCaptainNotQualifiedFisherError = true;
                        } break;
                        case CommercialFishingValidationErrorsEnum.PermitSubmittedForNotShipOwner: {
                            this.hasSubmittedForNotShipOwnerError = true;
                        } break;
                        case CommercialFishingValidationErrorsEnum.NoEDeliveryRegistration: {
                            this.hasNoEDeliveryRegistrationError = true;
                        } break;
                        case CommercialFishingValidationErrorsEnum.InvalidPermitRegistrationNumber: {
                            if (this.isPublicApp) {
                                this.hasInvalidPermitRegistrationNumber = true;
                            }
                        } break;
                        case CommercialFishingValidationErrorsEnum.ShipAlreadyHasValidBlackSeaPermit: {
                            this.selectedShipAlreadyHasValidBlackSeaPermitError = true;
                            this.form.get('shipControl')!.updateValueAndValidity({ emitEvent: false });
                            this.form.get('shipControl')!.markAsTouched();
                        } break;
                        case CommercialFishingValidationErrorsEnum.ShipAlreadyHasValidDanubePermit: {
                            this.selectedShipAlreadyHasValidDanubePermitError = true;
                            this.form.get('shipControl')!.updateValueAndValidity({ emitEvent: false });
                            this.form.get('shipControl')!.markAsTouched();
                        } break;
                        case CommercialFishingValidationErrorsEnum.ShipHasNoValidBlackSeaPermit: {
                            this.selectedShipHasNoBlackSeaPermitError = true;
                            this.form.get('shipControl')!.updateValueAndValidity({ emitEvent: false });
                            this.form.get('shipControl')!.markAsTouched();
                        } break;
                        case CommercialFishingValidationErrorsEnum.ShipHasNoValidDanubePermit: {
                            this.selectedShipHasNoDanubePermitError = true;
                            this.form.get('shipControl')!.updateValueAndValidity({ emitEvent: false });
                            this.form.get('shipControl')!.markAsTouched();
                        } break;
                        case CommercialFishingValidationErrorsEnum.ShipHasNoPoundNetPermit: {
                            this.selectedShipHasNoPoundNetPermitError = true;
                            this.form.get('shipControl')!.updateValueAndValidity({ emitEvent: false });
                            this.form.get('shipControl')!.markAsTouched();
                        } break;
                        case CommercialFishingValidationErrorsEnum.ShipIsThirdCountry: {
                            this.selectedShipIsThirdCountryError = true;
                            this.form.get('shipControl')!.updateValueAndValidity({ emitEvent: false });
                            this.form.get('shipControl')!.markAsTouched();
                        } break;
                        case CommercialFishingValidationErrorsEnum.ShipIsNotThirdCountry: {
                            this.selectedShipIsNotThirdCountryError = true;
                            this.form.get('shipControl')!.updateValueAndValidity({ emitEvent: false });
                            this.form.get('shipControl')!.markAsTouched();
                        } break;
                    }
                }
            }

            const error = errorResponse.error as ErrorModel;
            if (error?.code === ErrorCode.InvalidLogBookLicensePagesRange && this.model instanceof CommercialFishingEditDTO) {
                this.handleInvalidLogBookLicensePagesRangeError(error.messages[0], dialogClose, saveMethod);
            }
            else if (error?.code === ErrorCode.NoPermitRegisterForPermitLicense) {
                this.hasNoPermitRegisterForPermitLicenseError = true;
                this.form.get('shipControl')!.updateValueAndValidity({ emitEvent: false });
                this.form.get('shipControl')!.markAsTouched();
            }
            else if (error?.code === ErrorCode.DuplicatedMarksNumbers) {
                this.duplicatedMarkNumbers = error?.messages ?? [];
                this.form.get('fishingGearsGroup')!.setErrors({ 'duplicatedMarkNumbers': this.duplicatedMarkNumbers });
                this.form.get('fishingGearsGroup')!.markAsTouched();
            }
            else if (error?.code === ErrorCode.DuplicatedPingersNumbers) {
                this.duplicatedPingerNumbers = error?.messages ?? [];
                this.form.get('fishingGearsGroup')!.setErrors({ 'duplicatedPingerNumbers': this.duplicatedPingerNumbers });
                this.form.get('fishingGearsGroup')!.markAsTouched();
            }
            else if (error?.code === ErrorCode.ShipEventExistsOnSameDate) {
                this.hasShipEventExistsOnSameDateError = true;
                this.form.updateValueAndValidity({ onlySelf: true });
            }
            else if (error?.code === ErrorCode.LogBookHasSubmittedPages) {
                const message: string = this.translationService.getValue('catches-and-sales.cannot-delete-log-book-with-submitted-pages');
                this.snackbar.open(message, undefined, {
                    duration: RequestProperties.DEFAULT.showExceptionDurationErr,
                    panelClass: RequestProperties.DEFAULT.showExceptionColorClassErr
                });
            }
            else if (error?.code === ErrorCode.PermitSuspensionValidToExists || error?.code === ErrorCode.PermitLicenseSuspensionValidToExists) {
                let existingValidTo: Date | undefined;
                if (error?.messages !== null && error?.messages !== undefined && error.messages.length > 0) {
                    const timestamp: number = Date.parse(error.messages[0]);

                    if (isNaN(timestamp) === false) {
                        existingValidTo = new Date(timestamp);
                    }
                }

                let message: string = '';

                if (existingValidTo !== null && existingValidTo !== undefined && existingValidTo) {
                    let msg1: string = '';
                    if (this.isPermitLicense) {
                        msg1 = this.translationService.getValue('suspensions.permit-suspension-valid-to-already-exists');
                    }
                    else {
                        msg1 = this.translationService.getValue('suspensions.permit-license-suspension-valid-to-already-exists');
                    }

                    message = `${msg1}: ${DateUtils.ToDisplayDateString(existingValidTo)}`;
                }
                else {
                    message = this.translationService.getValue('suspensions.there-is-a-suspension-with-already-existing-valid-to-error');
                }

                this.snackbar.open(message, undefined, {
                    duration: RequestProperties.DEFAULT.showExceptionDurationErr,
                    panelClass: RequestProperties.DEFAULT.showExceptionColorClassErr
                });
            }
        }

        setTimeout(() => {
            this.validityCheckerGroup.validate();
        });
    }

    private fillForm(): void {
        this.fillFormApplicationData();

        if (!this.showOnlyRegiXData) {
            this.fillFormBasicInfo();
            this.fillFormQualifiedFisherData();

            if (this.isPermitLicense) {
                if (this.isAddingRegister) {
                    this.photoRequestMethod = this.service.getPermitLicenseFisherPhotoFromApplication.bind(this.service, this.model.applicationId!);
                }
                else if (this.model?.id) {
                    this.photoRequestMethod = this.service.getPermitLicenseFisherPhoto.bind(this.service, this.model.id!);
                }
                else if (this.renewalPermitLicenseId !== null && this.renewalPermitLicenseId !== undefined) { // В режим на преиздаване на УСР от старо УСР
                    this.photoRequestMethod = this.service.getPermitLicenseFisherPhoto.bind(this.service, this.renewalPermitLicenseId);
                }
                else if (this.renewalPermitId !== null && this.renewalPermitId !== undefined) {
                    this.photoRequestMethod = this.service.getPermitFisherPhoto.bind(this.service, this.renewalPermitId); // В режим на преиздаване от старо РСР
                }
            }
            else {
                if (this.isAddingRegister) {
                    this.photoRequestMethod = this.service.getPermitFisherPhotoFromApplication.bind(this.service, this.model.applicationId!);
                }
                else if (this.model?.id) {
                    this.photoRequestMethod = this.service.getPermitFisherPhoto.bind(this.service, this.model.id!);
                }
                else if (this.renewalPermitId !== null && this.renewalPermitId !== undefined) {
                    this.photoRequestMethod = this.service.getPermitFisherPhoto.bind(this.service, this.renewalPermitId); // В режим на преиздаване от старо РСР
                }
            }
        }

        if (!this.isApplication && !this.showOnlyRegiXData) {
            this.fillFormRegisterData();
        }

        if (this.model instanceof CommercialFishingRegixDataDTO && this.showOnlyRegiXData) {
            this.fillFormRegiX();
        }
        else {
            if (this.showRegiXData) {
                this.fillFormRegiX();
            }
        }

        if (this.isApplication
            && this.isPermitLicense
            && this.isPaid
            && !this.isReadonly
            && !this.viewMode
        ) {
            this.updatePermitLicenseAppliedTariffs();
        }
    }

    private fillFormRegiX(): void {
        if (this.model instanceof CommercialFishingRegixDataDTO || this.model instanceof CommercialFishingApplicationEditDTO) {
            if (this.model.applicationRegiXChecks !== undefined && this.model.applicationRegiXChecks !== null) {
                const applicationRegiXChecks: ApplicationRegiXCheckDTO[] = this.model.applicationRegiXChecks;

                setTimeout(() => {
                    this.regixChecks = applicationRegiXChecks;
                });

                this.model.applicationRegiXChecks = undefined;
            }
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

    private fillFormApplicationData(): void {
        if ((this.model instanceof CommercialFishingRegixDataDTO && this.showOnlyRegiXData) || this.model instanceof CommercialFishingApplicationEditDTO) {
            this.form.get('submittedByControl')!.setValue(this.model.submittedBy);

            if (this.model instanceof CommercialFishingApplicationEditDTO) {
                this.form.get('deliveryDataControl')!.setValue(this.model.deliveryData);
                this.form.get('applicationPaymentInformationControl')!.setValue(this.model.paymentInformation);

                this.hideBasicPaymentInfo = this.shouldHidePaymentData();
            }
        }

        this.form.get('submittedForControl')!.setValue(this.model.submittedFor);
    }

    private fillFormBasicInfo(): void {
        if (this.model instanceof CommercialFishingEditDTO || this.model instanceof CommercialFishingApplicationEditDTO) {
            const shipId: number | undefined = this.model.shipId;
            if (shipId !== undefined && shipId !== null) {
                const selectedShip: ShipNomenclatureDTO = ShipsUtils.get(this.ships, shipId)
                this.form.get('shipControl')!.setValue(selectedShip);
                this.onlyOnlineLogBooks = ShipsUtils.hasErs(selectedShip);
            }

            if (this.model instanceof CommercialFishingApplicationEditDTO && this.isPermitLicense) {
                if (this.isPublicApp) {
                    this.form.get('permitLicensePermitNumberControl')!.setValue(this.model.permitLicensePermitNumber);
                }
            }

            const isHolderShipOwner: boolean = this.model.isHolderShipOwner ?? true;
            const holderShipRelation: NomenclatureDTO<boolean> = this.holderShipRelations.find(x => x.value === isHolderShipOwner)!;
            this.form.get('holderShipRelationControl')!.setValue(holderShipRelation);

            if (holderShipRelation.value === false) {
                const model: CommercialFishingEditDTO | CommercialFishingApplicationEditDTO = this.model;
                this.form.get('shipGroundForUseControl')!.setValue(model.shipGroundForUse);

                setTimeout(() => { // setTimeout needed in order to wait for the grounds-for-use component to load in UI (after previous IF)
                    this.form.get('shipGroundForUseControl')!.updateValueAndValidity();
                });
            }

            if (this.pageCode === PageCodeEnum.PoundnetCommFish || this.pageCode === PageCodeEnum.PoundnetCommFishLic) {
                const poundNetId: number | undefined = this.model.poundNetId;
                if (poundNetId !== null && poundNetId !== undefined) {
                    const poundNetGroup: IGroupedOptions<number> = this.poundNets.find(x => (x.options as PoundNetNomenclatureDTO[]).find(y => y.value === poundNetId))!;
                    const poundNet: PoundNetNomenclatureDTO = (poundNetGroup.options as PoundNetNomenclatureDTO[]).find(x => x.value === poundNetId)!;

                    this.form.get('poundNetControl')!.setValue(poundNet);
                    this.form.get('poundNetDepthControl')!.setValue(poundNet.depth);
                    this.form.get('poundNetStatusControl')!.setValue(poundNet.statusName);
                }

                if (this.pageCode === PageCodeEnum.PoundnetCommFish) {
                    this.form.get('poundNetGroundForUseControl')!.setValue(this.model.poundNetGroundForUse);
                }
            }

            if (this.pageCode === PageCodeEnum.CatchQuataSpecies) {
                this.quotaAquaticOrganisms = this.model.quotaAquaticOrganisms ?? [];
                this.quotaAquaticOrganismTypes = this.filterQuotaAquaticOrganismTypesCollection();

                this.form.get('unloaderPhoneNumberControl')!.setValue(this.model.unloaderPhoneNumber);
            }
            else if (this.isPermitLicense) {
                const aquaticOrganismIds: number[] | undefined = this.model.aquaticOrganismTypeIds;

                if (aquaticOrganismIds !== null && aquaticOrganismIds !== undefined) {
                    this.updateSelectedAquaticOrganismTypes(this.aquaticOrganismTypes.filter(
                        (x: NomenclatureDTO<number>) => {
                            return x.value !== null && x.value !== undefined && aquaticOrganismIds.includes(x.value!);
                        }
                    ));
                }
            }

            const waterTypeId: number | undefined = this.model.waterTypeId;
            if (waterTypeId !== null && waterTypeId !== undefined) {
                this.form.get('waterTypeControl')!.setValue(this.waterTypes.find(x => x.value === waterTypeId));
            }

            if (this.isPermitLicense === true) {
                this.form.get('fishingGearsGroup.fishingGearsControl')!.setValue(this.model.fishingGears);
            }

            if (this.model instanceof CommercialFishingEditDTO && this.isPermitLicense) {
                this.form.get('logBooksControl')!.setValue(this.model.logBooks);
            }

            this.form.get('filesControl')!.setValue(this.model.files);
        }
    }

    private fillFormQualifiedFisherData(): void {
        if (this.model instanceof CommercialFishingEditDTO || this.model instanceof CommercialFishingApplicationEditDTO) {
            this.form.get('qualifiedFisherSameAsSubmittedForControl')!.setValue(this.model.qualifiedFisherSameAsSubmittedFor);

            if (this.model instanceof CommercialFishingApplicationEditDTO || this.loadRegisterFromApplication) {
                this.form.get('qualifiedFisherIdNumberControl')!.setValue(this.model.qualifiedFisherIdentifier);
                this.form.get('qualifiedFisherFirstNameControl')!.setValue(this.model.qualifiedFisherFirstName);
                this.form.get('qualifiedFisherMiddleNameControl')!.setValue(this.model.qualifiedFisherMiddleName);
                this.form.get('qualifiedFisherLastNameControl')!.setValue(this.model.qualifiedFisherLastName);
            }
            else {
                const qualifiedFisherId: number | undefined = this.model.qualifiedFisherId;
                if (qualifiedFisherId !== null && qualifiedFisherId !== undefined) {
                    const qualifiedFisher: QualifiedFisherNomenclatureDTO = this.qualifiedFishers.find(x => x.value === qualifiedFisherId)!;
                    this.form.get('qualifiedFisherControl')!.setValue(qualifiedFisher);
                    this.form.get('qualifiedFisherRegistrationNumberControl')!.setValue(qualifiedFisher.registrationNumber);
                }
            }
        }

        if (this.isQualifiedFisherPhotoRequired) {
            if (this.model instanceof CommercialFishingApplicationEditDTO) {
                if (this.model.qualifiedFisherPhotoBase64 && this.model.qualifiedFisherPhotoBase64.length > 0) {
                    this.form.get('qualifiedFisherPhotoControl')!.setValue(this.model.qualifiedFisherPhotoBase64);
                }
                else {
                    this.form.get('qualifiedFisherPhotoControl')!.setValue(this.model.qualifiedFisherPhoto);
                }
            }
            else if (this.model instanceof CommercialFishingEditDTO) {
                this.form.get('qualifiedFisherPhotoControl')!.setValue(this.model.qualifiedFisherPhoto);
            }
        }
    }

    private fillFormRegisterData(): void {
        if (this.model instanceof CommercialFishingEditDTO) {
            if (this.model.id !== null && this.model.id !== undefined) {
                this.form.get('issueDateControl')!.setValue(this.model.issueDate);
                this.form.get('validFromControl')!.setValue(this.model.validFrom);
                this.form.get('validToControl')!.setValue(this.model.validTo);

                if (!this.isPermitLicense) {
                    this.form.get('permitRegistrationNumberControl')!.setValue(this.model.permitRegistrationNumber);
                    this.form.get('validFromUnlimitedControl')!.setValue(this.model.validFrom);

                    if (this.pageCode === PageCodeEnum.CommFish || this.pageCode === PageCodeEnum.PoundnetCommFish) {
                        this.form.get('isPermitUnlimitedControl')!.setValue(this.model.isPermitUnlimited);
                    }
                }
                else {
                    this.form.get('permitLicenseRegistrationNumberControl')!.setValue(this.model.permitLicenseRegistrationNumber);
                }

                setTimeout(() => { // setTimeout needed to avoid [object Object] in place of the edit/delete buttons...
                    const modelSuspensions: SuspensionDataDTO[] | undefined = (this.model as CommercialFishingEditDTO).suspensions;
                    this.form.get('suspensionsControl')!.setValue(modelSuspensions ?? []);
                    this.updateIsSuspendedFlag();
                });

                this.duplicates = this.model.duplicateEntries ?? [];
            }

            if (this.model.submittedFor!.submittedByRole! & SubmittedByRolesEnum.LegalRole) {
                this.logBookOwnerType = LogBookPagePersonTypesEnum.LegalPerson;

                const eik: string | undefined = this.model.submittedFor!.legal?.eik;
                this.isIdReadOnly = CommonUtils.hasDigitsOnly(eik);
            }
            else if (this.model.submittedFor!.submittedByRole! & SubmittedByRolesEnum.PersonalRole) {
                this.logBookOwnerType = LogBookPagePersonTypesEnum.Person;

                const egnLnc: string | undefined = this.model.submittedFor!.person?.egnLnc?.egnLnc;
                this.isIdReadOnly = CommonUtils.hasDigitsOnly(egnLnc);
            }
        }
    }

    private buildForm(): void {
        this.form = new FormGroup({}, [this.oneShipEventOnSameDateValidator()]);
        this.addApplicationFormControls();

        if (!this.showOnlyRegiXData) {
            this.addBasicInformation();
            this.addQualifiedFisherControls();

            this.form.get('waterTypeControl')!.valueChanges.subscribe({
                next: (waterType: NomenclatureDTO<number> | undefined) => {
                    if (this.isPermitLicense) { // filter aquatic organisms, filter permits and recalculate tariffs

                        this.filterPermitsByWaterTypeAndSelectIfOne();

                        if (waterType !== null && waterType !== undefined) {
                            const waterTypeCode: WaterTypesEnum = WaterTypesEnum[waterType.code as keyof typeof WaterTypesEnum];
                            this.fileterAquaticOrganismTypesByWaterType(waterTypeCode);
                        }

                        if (this.isApplication
                            && this.isPaid
                            && !this.isReadonly
                            && !this.viewMode
                        ) { // update applied tariffs based on selected ship
                            this.updatePermitLicenseAppliedTariffs();
                        }
                    }

                    // validate ship filters and selected ship
                    this.shipFilters = this.getShipFilters();
                    this.form.get('shipControl')!.updateValueAndValidity({ emitEvent: false, onlySelf: true });
                }
            });
        }

        if (!this.isApplication && !this.showOnlyRegiXData) {
            this.addRegisterControls();
        }
    }

    private addApplicationFormControls(): void {
        this.form.addControl('submittedByControl', new FormControl());

        if (!this.showOnlyRegiXData) {
            this.form.addControl('deliveryDataControl', new FormControl());
            this.form.addControl('applicationPaymentInformationControl', new FormControl());
        }

        this.form.addControl('submittedForControl', new FormControl());
    }

    private addBasicInformation(): void {
        this.form.addControl('shipControl', new FormControl(undefined, [Validators.required, this.shipAllowedForSelectionValidator()]));

        if (this.isApplication) {
            this.form.get('shipControl')!.setValidators([Validators.required, this.shipAllowedForSelectionValidator()]);
        }
        else {
            if (this.isPermitLicense) {
                this.form.get('shipControl')!.setValidators([Validators.required, this.permitRegisterForPermitLicenseValidator()]);
            }
            else {
                this.form.get('shipControl')!.setValidators([Validators.required]);
            }
        }

        if (this.isPermitLicense && this.isApplication) {
            if (this.isPublicApp) {
                this.form.addControl('permitLicensePermitNumberControl', new FormControl(undefined, Validators.required));
            }
            else {
                this.form.addControl('permitLicensePermitControl', new FormControl(undefined, [Validators.required]));
            }
        }

        this.form.addControl('holderShipRelationControl', new FormControl(undefined, Validators.required));
        this.form.addControl('shipGroundForUseControl', new FormControl(undefined));

        if (this.pageCode === PageCodeEnum.PoundnetCommFish || this.pageCode === PageCodeEnum.PoundnetCommFishLic) {
            this.form.addControl('poundNetControl', new FormControl(undefined, Validators.required));
            this.form.addControl('poundNetDepthControl', new FormControl(undefined));
            this.form.addControl('poundNetStatusControl', new FormControl(undefined));

            if (this.pageCode === PageCodeEnum.PoundnetCommFish) {

                this.form.addControl('poundNetGroundForUseControl', new FormControl(undefined, Validators.required));
            }
        }

        if (this.pageCode === PageCodeEnum.CatchQuataSpecies) {
            this.quotaAquaticOrganismsForm = new FormGroup({
                aquaticOrganismIdControl: new FormControl(undefined, Validators.required),
                portIdControl: new FormControl(undefined, Validators.required)
            });

            this.form.addControl('unloaderPhoneNumberControl', new FormControl(undefined, Validators.required));

            if (!this.isReadonly && !this.viewMode) {
                if (this.form.validator !== null && this.form.validator !== undefined) {
                    this.form.setValidators([this.form.validator, this.atLeastOneQuotaAquaticOrganism(), this.uniqueQuotaPortValidator()]);
                }
                else {
                    this.form.setValidators([this.atLeastOneQuotaAquaticOrganism(), this.uniqueQuotaPortValidator()]);
                }
            }
        }
        else if (this.isPermitLicense) {
            if (!this.isReadonly && !this.viewMode) {
                if (this.form.validator !== null && this.form.validator !== undefined) {
                    this.form.setValidators([this.form.validator, this.atLeastOneAquaticOrganism()]);
                }
                else {
                    this.form.setValidators([this.atLeastOneAquaticOrganism()]);
                }
            }
        }

        this.form.addControl('waterTypeControl', new FormControl(null, Validators.required));

        if (this.isPermitLicense === true) {
            this.form.addControl('fishingGearsGroup',
                new FormGroup({
                    fishingGearsControl: new FormControl(undefined)
                })
            );

            if (!this.isReadonly && !this.viewMode) {
                if (this.form.get('fishingGearsGroup')!.validator !== null && this.form.get('fishingGearsGroup')!.validator !== undefined) {
                    this.form.get('fishingGearsGroup')!.setValidators([
                        this.form.get('fishingGearsGroup')!.validator!,
                        FishingGearUtils.atLeastOneFishingGear(),
                        FishingGearUtils.permitLicenseDuplicateMarkNumbersValidator(),
                        FishingGearUtils.permitLicenseDuplicatePingerNumbersValidator()
                    ]);
                }
                else {
                    this.form.get('fishingGearsGroup')!.setValidators([
                        FishingGearUtils.atLeastOneFishingGear(),
                        FishingGearUtils.permitLicenseDuplicateMarkNumbersValidator(),
                        FishingGearUtils.permitLicenseDuplicatePingerNumbersValidator()
                    ]);
                }
            }

            this.form.get('fishingGearsGroup.fishingGearsControl')!.valueChanges.subscribe({
                next: (values: FishingGearDTO[] | null | undefined) => {
                    this.updateDuplicatedMarksAndPingers(values);
                }
            });
        }

        this.form.addControl('filesControl', new FormControl(undefined));
    }

    private addQualifiedFisherControls(): void {
        this.form.addControl('qualifiedFisherSameAsSubmittedForControl', new FormControl(false));

        if (this.isQualifiedFisherPhotoRequired) {
            this.form.addControl('qualifiedFisherPhotoControl', new FormControl(null, Validators.required));
        }

        if (this.isApplication || this.loadRegisterFromApplication) {
            this.form.addControl('qualifiedFisherIdNumberControl', new FormControl(undefined, Validators.required));
            this.form.addControl('qualifiedFisherFirstNameControl', new FormControl(undefined, Validators.required));
            this.form.addControl('qualifiedFisherMiddleNameControl', new FormControl(undefined));
            this.form.addControl('qualifiedFisherLastNameControl', new FormControl(undefined, Validators.required));
        }
        else {
            this.form.addControl('qualifiedFisherControl', new FormControl(undefined, Validators.required));
            this.form.addControl('qualifiedFisherRegistrationNumberControl', new FormControl(undefined));
        }
    }

    private addRegisterControls(): void {
        const now: Date = new Date();
        this.form.addControl('issueDateControl', new FormControl(now, Validators.required));
        this.form.addControl('suspensionsControl', new FormControl([]));

        if (!this.isPermitLicense) {
            this.form.addControl('permitRegistrationNumberControl', new FormControl());
            this.form.addControl('validFromUnlimitedControl', new FormControl(undefined, Validators.required));
            this.form.addControl('isPermitUnlimitedControl', new FormControl(true));

            this.form.addControl('validFromControl', new FormControl());
            this.form.addControl('validToControl', new FormControl());
        }
        else {
            this.form.addControl('permitLicenseRegistrationNumberControl', new FormControl());

            const now: Date = new Date();
            const validToDate: Date = new Date(now.getFullYear(), 11, 31); // последният ден от текущата година

            this.form.addControl('validFromControl', new FormControl(undefined, Validators.required));
            this.form.addControl('validToControl', new FormControl(validToDate, Validators.required));
        }

        if (this.isPermitLicense) {
            this.form.addControl('logBooksControl', new FormControl());
        }
    }

    private fillModel(): CommercialFishingEditDTO | CommercialFishingApplicationEditDTO | CommercialFishingRegixDataDTO {
        const model: CommercialFishingEditDTO | CommercialFishingApplicationEditDTO | CommercialFishingRegixDataDTO = Object.create(this.model);

        if (model instanceof CommercialFishingEditDTO || model instanceof CommercialFishingApplicationEditDTO) {
            model.pageCode = this.pageCode;
        }

        this.fillModelApplicationData(model);
        this.fillBasicInformation(model);
        this.fillQualifiedFisherData(model);

        if (!this.isApplication && !this.showOnlyRegiXData) {
            this.fillRegisterData(model);
        }

        return model;
    }

    private fillModelApplicationData(model: CommercialFishingEditDTO | CommercialFishingApplicationEditDTO | CommercialFishingRegixDataDTO): void {
        if (model instanceof CommercialFishingApplicationEditDTO || (model instanceof CommercialFishingRegixDataDTO && this.showOnlyRegiXData)) {
            model.submittedBy = this.form.get('submittedByControl')!.value;
            model.submittedFor = this.form.get('submittedForControl')!.value;

            if (model instanceof CommercialFishingApplicationEditDTO) {
                model.deliveryData = this.form.get('deliveryDataControl')!.value;
                model.paymentInformation = this.form.get('applicationPaymentInformationControl')!.value;
            }
        }
        else if (model instanceof CommercialFishingEditDTO) {
            model.submittedFor = this.form.get('submittedForControl')!.value;
        }
    }

    private fillBasicInformation(model: CommercialFishingEditDTO | CommercialFishingApplicationEditDTO | CommercialFishingRegixDataDTO): void {
        if (model instanceof CommercialFishingApplicationEditDTO || model instanceof CommercialFishingEditDTO) {
            model.shipId = this.form.get('shipControl')!.value?.value;

            if (model instanceof CommercialFishingApplicationEditDTO && this.isPermitLicense) {
                if (this.isPublicApp) {
                    model.permitLicensePermitNumber = this.form.get('permitLicensePermitNumberControl')!.value;
                }
                else {
                    model.permitLicensePermitId = this.form.get('permitLicensePermitControl')!.value?.value;
                }
            }

            model.isHolderShipOwner = (this.form.get('holderShipRelationControl')!.value as NomenclatureDTO<boolean>)?.value;
            if (model.isHolderShipOwner === false) {
                model.shipGroundForUse = this.form.get('shipGroundForUseControl')!.value;
            }
            else {
                model.shipGroundForUse = undefined;
            }

            if (this.pageCode === PageCodeEnum.PoundnetCommFish || this.pageCode === PageCodeEnum.PoundnetCommFishLic) {
                model.poundNetId = this.form.get('poundNetControl')!.value?.value;

                if (this.pageCode === PageCodeEnum.PoundnetCommFish) {
                    model.poundNetGroundForUse = this.form.get('poundNetGroundForUseControl')!.value;
                }
            }

            if (this.pageCode === PageCodeEnum.CatchQuataSpecies) {
                model.quotaAquaticOrganisms = this.quotaAquaticOrganismTypesTable.rows.map((row: QuotaAquaticOrganismDTO) => {
                    return new QuotaAquaticOrganismDTO({
                        aquaticOrganismId: row.aquaticOrganismId,
                        portId: row.portId
                    });
                });

                model.unloaderPhoneNumber = this.form.get('unloaderPhoneNumberControl')!.value;
            }
            else if (this.isPermitLicense) {
                model.aquaticOrganismTypeIds = [];
                for (const el of this.selectedAquaticOrganismTypes) {
                    model.aquaticOrganismTypeIds.push(el.value!);
                }
            }

            model.waterTypeId = this.form.get('waterTypeControl')!.value?.value;

            if (this.isPermitLicense === true) {
                model.fishingGears = this.form.get('fishingGearsGroup.fishingGearsControl')!.value;
                model.fishingGears = model.fishingGears?.filter(x =>
                    (x.id !== null && x.id !== undefined) || ((x.id === null || x.id === undefined) && x.isActive)
                );
            }

            if (model instanceof CommercialFishingEditDTO) {
                if (this.isPermitLicense) {
                    model.logBooks = this.form.get('logBooksControl')!.value;
                }
            }

            model.files = this.form.get('filesControl')!.value;
        }
    }

    private fillQualifiedFisherData(model: CommercialFishingEditDTO | CommercialFishingApplicationEditDTO | CommercialFishingRegixDataDTO): void {
        if (model instanceof CommercialFishingEditDTO) {
            model.qualifiedFisherId = this.form.get('qualifiedFisherControl')!.value.value;

            if (this.isQualifiedFisherPhotoRequired) {
                model.qualifiedFisherPhoto = this.form.get('qualifiedFisherPhotoControl')!.value;
            }
        }
        else if (model instanceof CommercialFishingApplicationEditDTO) {
            model.qualifiedFisherSameAsSubmittedFor = this.form.get('qualifiedFisherSameAsSubmittedForControl')!.value;

            model.qualifiedFisherIdentifier = this.form.get('qualifiedFisherIdNumberControl')!.value;
            model.qualifiedFisherFirstName = this.form.get('qualifiedFisherFirstNameControl')!.value;
            model.qualifiedFisherMiddleName = this.form.get('qualifiedFisherMiddleNameControl')!.value;
            model.qualifiedFisherLastName = this.form.get('qualifiedFisherLastNameControl')!.value;

            if (this.isQualifiedFisherPhotoRequired) {
                const photo: FileInfoDTO | string | null = this.form.get('qualifiedFisherPhotoControl')?.value;
                if (photo !== undefined && photo !== null) {
                    if (typeof photo === 'string') {
                        model.qualifiedFisherPhotoBase64 = photo;
                    }
                    else {
                        model.qualifiedFisherPhoto = photo;
                    }
                }
                else {
                    model.qualifiedFisherPhotoBase64 = undefined;
                    model.qualifiedFisherPhoto = undefined;
                }
            }
        }
    }

    private fillRegisterData(model: CommercialFishingEditDTO): void {
        model.issueDate = this.form.get('issueDateControl')!.value;

        if (!this.isPermitLicense) {
            model.isPermitUnlimited = this.form.get('isPermitUnlimitedControl')!.value;

            if (model.isPermitUnlimited) {
                model.validFrom = this.form.get('validFromUnlimitedControl')!.value;
            }
            else {
                model.validFrom = this.form.get('validFromControl')!.value;
                model.validTo = this.form.get('validToControl')!.value;
            }
        }
        else {
            model.validFrom = this.form.get('validFromControl')!.value;
            model.validTo = this.form.get('validToControl')!.value;
        }

        model.suspensions = this.form.get('suspensionsControl')!.value;
    }

    private getShipPermitsNomenclature(shipId: number): void {
        this.service.getPermitNomenclatures(shipId, false, this.pageCode === PageCodeEnum.PoundnetCommFishLic).subscribe({
            next: (values: PermitNomenclatureDTO[]) => {
                this.allPermits = values;
                this.filterPermitsByWaterTypeAndSelectIfOne();
            }
        });
    }

    private filterPermitsByWaterTypeAndSelectIfOne(): void {
        if (this.isPublicApp === false) {
            const waterType: NomenclatureDTO<number> | undefined = this.form.get('waterTypeControl')!.value;

            if (waterType !== null && waterType !== undefined) { // should filter permits
                switch (waterType.code) {
                    case WaterTypesEnum[WaterTypesEnum.BLACK_SEA]:
                    case WaterTypesEnum[WaterTypesEnum.DANUBE]: {
                        this.permits = this.allPermits.filter(x => x.waterTypeId === waterType.value);
                    } break;
                }
            }
            else {
                this.permits = this.allPermits.slice();
            }

            if (this.model instanceof CommercialFishingApplicationEditDTO) {
                const permitId: number | undefined = this.model.permitLicensePermitId;
                this.form.get('permitLicensePermitControl')!.setValue(undefined);

                if (permitId !== null && permitId !== undefined) {
                    this.form.get('permitLicensePermitControl')!.setValue(this.permits.find(x => x.value === permitId));
                }
                else {
                    if (this.permits.filter(x => x.isActive).length === 1) { // Ако има само едно разрешително възможно за избор, направо го избираме
                        this.form.get('permitLicensePermitControl')!.setValue(this.permits.filter(x => x.isActive)[0]);
                    }
                }
            }
        }
    }

    private createAndFilterPoundNets(poundNets: PoundNetNomenclatureDTO[]): void {
        const map = new Map<string, PoundNetNomenclatureDTO[]>();

        for (const poundNet of poundNets) {
            const value: NomenclatureDTO<number>[] | undefined = map.get(poundNet.statusCode!);
            if (value !== undefined) {
                value.push(poundNet);
            }
            else {
                map.set(poundNet.statusCode!, [poundNet]);
            }
        }

        for (const [group, types] of map) {
            this.poundNets.push({
                name: types[0].statusName ?? group,
                options: types
            });
        }

        this.poundNets = this.poundNets.slice();

        if (this.isApplication && this.pageCode === PageCodeEnum.PoundnetCommFishLic) {
            this.poundNets = this.poundNets.filter(x => (x.options as PoundNetNomenclatureDTO[]).some(y => y.hasPoundNetPermit === true));

            for (const poundNetGroup of this.poundNets) {
                poundNetGroup.options = (poundNetGroup.options as PoundNetNomenclatureDTO[]).filter(x => x.hasPoundNetPermit === true);
            }
        }
    }

    private updatePermitLicenseAppliedTariffs(): void {
        (this.model as CommercialFishingApplicationEditDTO).paymentInformation = this.form.get('applicationPaymentInformationControl')!.value;

        const parameters: PermitLicenseTariffCalculationParameters = this.getPermitLiceseTariffCalculationParameters();

        this.service.calculatePermitLicenseAppliedTariffs(parameters).subscribe({
            next: (appliedTariffs: PaymentTariffDTO[]) => {
                if ((this.model as CommercialFishingApplicationEditDTO).paymentInformation!.paymentSummary !== null
                    && (this.model as CommercialFishingApplicationEditDTO).paymentInformation!.paymentSummary !== undefined
                ) {
                    (this.model as CommercialFishingApplicationEditDTO).paymentInformation!.paymentSummary!.tariffs = appliedTariffs;
                    (this.model as CommercialFishingApplicationEditDTO).paymentInformation!.paymentSummary!.totalPrice = this.calculateAppliedTariffsTotalPrice(appliedTariffs);
                }
                else {
                    (this.model as CommercialFishingApplicationEditDTO).paymentInformation!.paymentSummary = new PaymentSummaryDTO({
                        tariffs: appliedTariffs,
                        totalPrice: this.calculateAppliedTariffsTotalPrice(appliedTariffs)
                    });
                }

                this.form.get('applicationPaymentInformationControl')!.setValue((this.model as CommercialFishingApplicationEditDTO).paymentInformation);
                this.hideBasicPaymentInfo = this.shouldHidePaymentData();
            }
        });
    }

    private calculateAppliedTariffsTotalPrice(appliedTariffs: PaymentTariffDTO[]): number {
        return appliedTariffs.map(x => x.quantity! * x.unitPrice!).reduce((a, b) => a + b, 0);
    }

    private getPermitLiceseTariffCalculationParameters(): PermitLicenseTariffCalculationParameters {
        let aquaticOrganismTypeIds: number[] = [];

        if (this.pageCode === PageCodeEnum.CatchQuataSpecies) {
            aquaticOrganismTypeIds = this.quotaAquaticOrganisms.map(x => x.aquaticOrganismId!);
        }
        else if (this.isPermitLicense) {
            aquaticOrganismTypeIds = this.selectedAquaticOrganismTypes.map(x => x.value!);
        }

        const paymentIntormation: ApplicationPaymentInformationDTO = this.form.get('applicationPaymentInformationControl')!.value;

        let excludedTariffsIds: number[] = [];

        if (paymentIntormation.paymentSummary !== null
            && paymentIntormation.paymentSummary !== undefined
            && paymentIntormation.paymentSummary.tariffs !== null
            && paymentIntormation.paymentSummary.tariffs !== undefined
        ) {
            excludedTariffsIds = paymentIntormation.paymentSummary.tariffs.filter(x => !x.isChecked).map(x => x.tariffId!);
        }

        const parameters: PermitLicenseTariffCalculationParameters = new PermitLicenseTariffCalculationParameters({
            applicationId: this.applicationId,
            pageCode: this.pageCode,
            shipId: this.form.get('shipControl')!.value?.value,
            waterTypeCode: this.form.get('waterTypeControl')!.value?.code,
            aquaticOrganismTypeIds: aquaticOrganismTypeIds,
            fishingGears: this.form.get('fishingGearsGroup.fishingGearsControl')!.value,
            poundNetId: this.form.get('poundNetControl')?.value?.value,
            excludedTariffsIds: excludedTariffsIds
        });

        return parameters;
    }

    private updateSelectedAquaticOrganismTypes(aquaticOrganismType: AquaticOrganismsToAddType): void {
        if (aquaticOrganismType !== null && aquaticOrganismType !== undefined && typeof aquaticOrganismType !== 'string') {
            if (Array.isArray(aquaticOrganismType) && aquaticOrganismType.length > 0 && aquaticOrganismType[0] instanceof NomenclatureDTO) {
                this.selectedAquaticOrganismTypes = this.selectedAquaticOrganismTypes.concat(aquaticOrganismType);
            }
            else if (aquaticOrganismType instanceof NomenclatureDTO) {
                this.selectedAquaticOrganismTypes.push(aquaticOrganismType);
            }

            setTimeout(() => {
                this.selectedAquaticOrganismTypes = this.selectedAquaticOrganismTypes.slice();
            });

            this.aquaticOrganismTypes = this.waterAquaticOrganismTypes.filter(x => !this.selectedAquaticOrganismTypes.includes(x)).slice();
            this.aquaticOrganismTypesControl.setValue(undefined);

            if (this.isApplication
                && this.isPaid
                && !this.isReadonly
                && !this.viewMode
                && this.isPermitLicense
            ) { // update applied tariffs based on selected ship
                this.updatePermitLicenseAppliedTariffs();
            }
        }
    }

    private checkAndSetQualifiedFisherValues(): void {
        const isSameAsSubmittedFor = this.form.get('qualifiedFisherSameAsSubmittedForControl')!.value;

        if (this.isApplication) {
            if (isSameAsSubmittedFor) {
                let submittedForPerson: RegixPersonDataDTO | undefined = undefined;

                if (this.submittedByRole === SubmittedByRolesEnum.PersonalRepresentative) {
                    submittedForPerson = this.form.get('submittedForControl')!.value?.person;
                }
                else if (this.submittedByRole === SubmittedByRolesEnum.Personal) {
                    submittedForPerson = this.form.get('submittedByControl')!.value?.person;
                }

                if (submittedForPerson !== null && submittedForPerson !== undefined) {
                    this.form.get('qualifiedFisherIdNumberControl')!.setValue(submittedForPerson.egnLnc);
                    this.form.get('qualifiedFisherFirstNameControl')!.setValue(submittedForPerson.firstName);
                    this.form.get('qualifiedFisherMiddleNameControl')!.setValue(submittedForPerson.middleName);
                    this.form.get('qualifiedFisherLastNameControl')!.setValue(submittedForPerson.lastName);

                    this.form.get('qualifiedFisherIdNumberControl')!.disable();
                    this.form.get('qualifiedFisherFirstNameControl')!.disable();
                    this.form.get('qualifiedFisherMiddleNameControl')!.disable();
                    this.form.get('qualifiedFisherLastNameControl')!.disable();
                }
                else {
                    this.clearQualifiedFisherControls();
                }
            }
            else {
                this.form.get('qualifiedFisherIdNumberControl')!.enable();
                this.form.get('qualifiedFisherFirstNameControl')!.enable();
                this.form.get('qualifiedFisherMiddleNameControl')!.enable();
                this.form.get('qualifiedFisherLastNameControl')!.enable();
            }
        }
        else { // register
            if (!this.isFishingGearsApplication) {
                if (isSameAsSubmittedFor) { // TODO set qualified fisher to be as submitted for somehow ???
                    this.form.get('qualifiedFisherControl')!.disable();
                }
                else {
                    this.form.get('qualifiedFisherControl')!.enable();
                }
            }
        }
    }

    private clearQualifiedFisherControls(): void {
        this.form.get('qualifiedFisherIdNumberControl')!.setValue(null);
        this.form.get('qualifiedFisherFirstNameControl')!.setValue(null);
        this.form.get('qualifiedFisherMiddleNameControl')!.setValue(null);
        this.form.get('qualifiedFisherLastNameControl')!.setValue(null);
    }

    private handleInvalidLogBookLicensePagesRangeError(logBookNumber: string, dialogClose: DialogCloseCallback, saveMethod: SaveMethodType): void {
        if (this.model instanceof CommercialFishingEditDTO) {
            this.ignoreLogBookConflicts = false;
            const logBooks: CommercialFishingLogBookEditDTO[] = this.form.get('logBooksControl')!.value;

            const logBook: CommercialFishingLogBookEditDTO | undefined = logBooks!.find(x => x.logbookNumber === logBookNumber);

            if (logBook !== null && logBook !== undefined) {
                setTimeout(() => {
                    logBook.hasError = true;
                    this.form.get('logBooksControl')!.setValue((this.model as CommercialFishingEditDTO).logBooks);
                });
            }

            const ranges: OverlappingLogBooksParameters[] = [];

            for (const logBook of logBooks.filter(x => x.permitLicenseIsActive)) {
                const range: OverlappingLogBooksParameters = new OverlappingLogBooksParameters({
                    logBookId: logBook.logBookId,
                    typeId: logBook.logBookTypeId,
                    OwnerType: logBook.ownerType,
                    startPage: logBook.permitLicenseStartPageNumber,
                    endPage: logBook.permitLicenseEndPageNumber
                });
                ranges.push(range);
            }

            const editDialogData: OverlappingLogBooksDialogParamsModel = new OverlappingLogBooksDialogParamsModel({
                service: this.service as CommercialFishingAdministrationService,
                logBookGroup: this.logBookGroup,
                ranges: ranges
            });

            this.overlappingLogBooksDialog.open({
                title: this.translationService.getValue('catches-and-sales.overlapping-log-books-dialog-title'),
                TCtor: OverlappingLogBooksComponent,
                headerCancelButton: {
                    cancelBtnClicked: (closeFn: HeaderCloseFunction) => { closeFn(); }
                },
                componentData: editDialogData,
                translteService: this.translationService,
                disableDialogClose: true,
                cancelBtn: {
                    id: 'cancel',
                    color: 'primary',
                    translateValue: 'common.cancel',
                },
                saveBtn: {
                    id: 'save',
                    color: 'error',
                    translateValue: 'catches-and-sales.overlapping-log-books-save-despite-conflicts'
                }
            }, '1300px').subscribe({
                next: (save: boolean | undefined) => {
                    if (save) {
                        this.ignoreLogBookConflicts = true;
                        switch (saveMethod) {
                            case 'save': this.saveCommercialFishingRecord(dialogClose); break;
                            case 'saveAndPrint': this.saveAndPrintRecord(dialogClose); break;
                        }
                    }
                }
            });
        }
    }

    private updateDuplicatedMarksAndPingers(values: FishingGearDTO[] | null | undefined): void {
        if (values !== null && values !== undefined) {
            const marksMarkNumbers = values.map(x => x.marks?.filter(x => x.isActive && x.selectedStatus === FishingGearMarkStatusesEnum.NEW).map(x => `${x.fullNumber?.prefix ?? ''}${x.fullNumber?.inputValue ?? ''}`));
            if (marksMarkNumbers !== null && marksMarkNumbers !== undefined && marksMarkNumbers.length > 0) {
                const markNumbers = marksMarkNumbers.reduce(x => x);
                const dupMarksCopy = this.duplicatedMarkNumbers.slice();

                if (markNumbers !== null && markNumbers !== undefined) {
                    for (const mark of dupMarksCopy) {
                        if (!markNumbers.includes(mark)) {
                            const indexToDelete = this.duplicatedMarkNumbers.findIndex(x => x === mark);
                            this.duplicatedMarkNumbers.splice(indexToDelete, 1);
                        }
                    }
                }
            }

            const pingersPingerNumbers = values.map(x => x.pingers?.filter(x => x.isActive && x.selectedStatus === FishingGearPingerStatusesEnum.NEW)?.map(x => x.number!));
            if (pingersPingerNumbers !== null && pingersPingerNumbers !== undefined && pingersPingerNumbers.length > 0) {
                const pingerNumbers = pingersPingerNumbers.reduce(x => x);
                const dupPingersCopy = this.duplicatedPingerNumbers.slice();

                for (const pinger of dupPingersCopy) {
                    if (!pingerNumbers?.includes(pinger)) {
                        const indexToDelete = this.duplicatedPingerNumbers.findIndex(x => x === pinger);
                        this.duplicatedPingerNumbers.splice(indexToDelete, 1);
                    }
                }
            }

            this.duplicatedMarkNumbers = this.duplicatedMarkNumbers.slice();
            this.duplicatedPingerNumbers = this.duplicatedPingerNumbers.slice();
        }
        else {
            this.duplicatedMarkNumbers = [];
            this.duplicatedPingerNumbers = [];
        }

        this.form.get('fishingGearsGroup')!.updateValueAndValidity({ emitEvent: false });
    }

    /// Nomenclatures

    private getNomenclatures(): Subscription {
        type NomenclatureTypes = NomenclatureDTO<number>;
        const observables: Observable<NomenclatureTypes[]>[] = [];

        if (!this.showOnlyRegiXData) {
            this.shipFilters = this.getShipFilters();

            observables.push(this.getShipNomenclatures());

            observables.push(NomenclatureStore.instance.getNomenclature<number>(NomenclatureTypes.WaterTypes, this.service.getWaterTypes.bind(this.service), false));

            if (!this.isApplication && !this.loadRegisterFromApplication) {
                observables.push(this.getQualifiedFisherNomenclatures());
            }

            if (this.isPermitLicense) {
                observables.push(NomenclatureStore.instance.getNomenclature<number>(NomenclatureTypes.Fishes, this.commonNomenclatures.getFishTypes.bind(this.commonNomenclatures), false));

                if (this.pageCode === PageCodeEnum.CatchQuataSpecies) {
                    observables.push(NomenclatureStore.instance.getNomenclature<number>(NomenclatureTypes.Ports, this.commonNomenclatures.getPorts.bind(this.commonNomenclatures), false));
                }
            }

            observables.push(NomenclatureStore.instance.getNomenclature<number>(NomenclatureTypes.GroundForUseTypes, this.service.getHolderGroundForUseTypes.bind(this.service), false));

            if (this.pageCode === PageCodeEnum.PoundnetCommFish || this.pageCode === PageCodeEnum.PoundnetCommFishLic) {
                observables.push(this.getPoundNetsNomenclature()); //poundNets
            }
        }

        if (observables.length > 0) {
            const subscription: Subscription = forkJoin(observables).subscribe({
                next: (nomenclatures: NomenclatureTypes[][]) => {
                    let index = 0;
                    if (!this.showOnlyRegiXData) {
                        this.ships = nomenclatures[index++];
                        this.waterTypes = nomenclatures[index++];

                        if (!this.isApplication && !this.loadRegisterFromApplication) {
                            this.qualifiedFishers = nomenclatures[index++];
                        }

                        if (this.isPermitLicense) {
                            this.allAquaticOrganismTypes = nomenclatures[index++];

                            if (this.pageCode === PageCodeEnum.CatchQuataSpecies) {
                                const ports = nomenclatures[index++];
                                this.allPorts = this.deepCopyPorts(ports);

                                this.quotaAquaticOrganismTypes = this.filterQuotaAquaticOrganismTypesCollection();
                                this.ports = this.filterQuotaSpiciesPortsCollection(undefined);
                            }
                            else {
                                this.allAquaticOrganismTypes = this.allAquaticOrganismTypes.filter(x => x.isActive
                                    && (x.quotas === null || x.quotas === undefined || x.quotas.length === 0));

                                this.aquaticOrganismTypes = this.allAquaticOrganismTypes;
                            }
                        }

                        this.groundForUseTypes = nomenclatures[index++];

                        if (this.pageCode === PageCodeEnum.PoundnetCommFish || this.pageCode === PageCodeEnum.PoundnetCommFishLic) {
                            const poundNets = nomenclatures[index++];

                            this.createAndFilterPoundNets(poundNets);
                        }
                    }

                    this.loader.complete();
                }
            });

            return subscription;
        }

        this.loader.complete();
        return new Subscription();
    }

    private getShipNomenclatures(): Observable<NomenclatureDTO<number>[]> {
        return NomenclatureStore.instance.getNomenclature(
            NomenclatureTypes.Ships, this.commonNomenclatures.getShips.bind(this.commonNomenclatures), false
        );
    }

    private getShipFilters(): CommercialFishingShipFilters {
        const filters: CommercialFishingShipFilters = new CommercialFishingShipFilters();
        const selectedWaterType: NomenclatureDTO<number> | undefined = this.form.get('waterTypeControl')!.value;

        if (this.isReadonly || this.viewMode) {
            filters.hasBlackSeaPermit = undefined;
            filters.hasDanubePermit = undefined;

            filters.hasBlackSeaPermitApplication = undefined;
            filters.hasDanubePermitApplication = undefined;

            filters.isDestroyedOrDeregistered = undefined;
            filters.isThirdCountryShip = undefined;

            filters.hasActiveFishQuota = undefined;
        }
        else if (this.pageCode === PageCodeEnum.CommFish) {
            filters.isThirdCountryShip = false;
            filters.isDestroyedOrDeregistered = false;
            filters.isForbiddenForLicenses = false;

            if (this.isApplication || this.id === null || this.id === undefined) {
                if (selectedWaterType === null || selectedWaterType === undefined) {
                    filters.hasBlackSeaPermit = undefined;
                    filters.hasDanubePermit = undefined;
                }
                else if (selectedWaterType.code === WaterTypesEnum[WaterTypesEnum.BLACK_SEA]) {
                    filters.hasBlackSeaPermit = false;
                    filters.hasDanubePermit = undefined;
                }
                else if (selectedWaterType.code === WaterTypesEnum[WaterTypesEnum.DANUBE]) {
                    filters.hasBlackSeaPermit = undefined;
                    filters.hasDanubePermit = false;
                }
                else {
                    filters.hasBlackSeaPermit = undefined;
                    filters.hasDanubePermit = undefined;
                }
            }
        }
        else if (this.pageCode === PageCodeEnum.RightToFishResource) {
            if (this.isApplication) {
                if (selectedWaterType === null || selectedWaterType === undefined) {
                    filters.hasBlackSeaPermitApplication = undefined;
                    filters.hasDanubePermitApplication = undefined;
                }
                else if (selectedWaterType.code === WaterTypesEnum[WaterTypesEnum.BLACK_SEA]) {
                    filters.hasBlackSeaPermitApplication = true;
                    filters.hasDanubePermitApplication = undefined;
                }
                else if (selectedWaterType.code === WaterTypesEnum[WaterTypesEnum.DANUBE]) {
                    filters.hasBlackSeaPermitApplication = undefined;
                    filters.hasDanubePermitApplication = true;
                }
                else {
                    filters.hasBlackSeaPermitApplication = undefined;
                    filters.hasDanubePermitApplication = undefined;
                }

                filters.isDestroyedOrDeregistered = false;
                filters.isForbiddenForLicenses = false;
            }
            else if (this.id === null || this.id === undefined) {
                if (selectedWaterType === null || selectedWaterType === undefined) {
                    filters.hasBlackSeaPermit = undefined;
                    filters.hasDanubePermit = undefined;
                }
                else if (selectedWaterType.code === WaterTypesEnum[WaterTypesEnum.BLACK_SEA]) {
                    filters.hasBlackSeaPermit = true;
                    filters.hasDanubePermit = undefined;
                }
                else if (selectedWaterType.code === WaterTypesEnum[WaterTypesEnum.DANUBE]) {
                    filters.hasBlackSeaPermit = undefined;
                    filters.hasDanubePermit = true;
                }
                else {
                    filters.hasBlackSeaPermit = undefined;
                    filters.hasDanubePermit = undefined;
                }

                filters.isDestroyedOrDeregistered = false;
                filters.isForbiddenForLicenses = false;
            }
        }
        else if (this.pageCode === PageCodeEnum.PoundnetCommFish) {
            filters.isThirdCountryShip = false;
            filters.isForbiddenForLicenses = false;

            if (this.isApplication) {
                filters.isDestroyedOrDeregistered = false;
            }
        }
        else if (this.pageCode === PageCodeEnum.PoundnetCommFishLic) {
            filters.isThirdCountryShip = false;

            if (this.isApplication) {
                filters.hasPoundNetPermitApplication = true;
                filters.isDestroyedOrDeregistered = false;
                filters.isForbiddenForLicenses = false;
            }
            else if (this.id === null || this.id === undefined) {
                filters.hasPoundNetPermit = true;
                filters.isForbiddenForLicenses = false;
            }
        }
        else if (this.pageCode === PageCodeEnum.RightToFishThirdCountry) {
            filters.isThirdCountryShip = true;
            filters.isForbiddenForLicenses = false;

            if (this.isApplication || this.id === null || this.id === undefined) {
                if (selectedWaterType === null || selectedWaterType === undefined) {
                    filters.hasBlackSeaPermit = undefined;
                    filters.hasDanubePermit = undefined;
                }
                else if (selectedWaterType.code === WaterTypesEnum[WaterTypesEnum.BLACK_SEA]) {
                    filters.hasBlackSeaPermit = false;
                    filters.hasDanubePermit = undefined;
                }
                else if (selectedWaterType.code === WaterTypesEnum[WaterTypesEnum.DANUBE]) {
                    filters.hasBlackSeaPermit = undefined;
                    filters.hasDanubePermit = false;
                }
                else {
                    filters.hasBlackSeaPermit = undefined;
                    filters.hasDanubePermit = undefined;
                }

                filters.isDestroyedOrDeregistered = false;
            }
        }
        else if (this.pageCode === PageCodeEnum.CatchQuataSpecies) {
            filters.hasActiveFishQuota = true;
            filters.isThirdCountryShip = false;

            if (this.isApplication || this.id === null || this.id === undefined) {
                if (selectedWaterType === null || selectedWaterType === undefined) {
                    filters.hasBlackSeaPermitApplication = undefined;
                    filters.hasDanubePermitApplication = undefined;
                }
                else if (selectedWaterType.code === WaterTypesEnum[WaterTypesEnum.BLACK_SEA]) {
                    filters.hasBlackSeaPermitApplication = true;
                    filters.hasDanubePermitApplication = undefined;
                }
                else if (selectedWaterType.code === WaterTypesEnum[WaterTypesEnum.DANUBE]) {
                    filters.hasBlackSeaPermitApplication = undefined;
                    filters.hasDanubePermitApplication = true;
                }
                else {
                    filters.hasBlackSeaPermitApplication = undefined;
                    filters.hasDanubePermitApplication = undefined;
                }

                filters.isDestroyedOrDeregistered = false;
                filters.isForbiddenForLicenses = false;
            }
        }
        else {
            if (selectedWaterType === null || selectedWaterType === undefined) {
                filters.hasBlackSeaPermit = undefined;
                filters.hasDanubePermit = undefined;
            }
            else if (selectedWaterType.code === WaterTypesEnum[WaterTypesEnum.BLACK_SEA]) {
                filters.hasBlackSeaPermit = true;
                filters.hasDanubePermit = undefined;
            }
            else if (selectedWaterType.code === WaterTypesEnum[WaterTypesEnum.DANUBE]) {
                filters.hasBlackSeaPermit = undefined;
                filters.hasDanubePermit = true;
            }
            else {
                filters.hasBlackSeaPermit = undefined;
                filters.hasDanubePermit = undefined;
            }

            filters.isThirdCountryShip = undefined;
            filters.isForbiddenForLicenses = undefined;
        }

        return filters;
    }

    private getQualifiedFisherNomenclatures(): Observable<QualifiedFisherNomenclatureDTO[]> {
        return this.service.getQualifiedFishers().pipe(map((qualifiedFishers: QualifiedFisherNomenclatureDTO[]) => {
            for (const fisher of qualifiedFishers) {
                fisher.description = `${this.translationService.getValue('commercial-fishing.qualified-fisher-registration-number')}: ${fisher.registrationNumber}`;
            }
            return qualifiedFishers;
        }));
    }

    private getPoundNetsNomenclature(): Observable<PoundNetNomenclatureDTO[]> {
        return NomenclatureStore.instance.getNomenclature(
            NomenclatureTypes.PoundNets, this.service.getPoundNets.bind(this.service), false
        ).pipe(map((poundNets: PoundNetNomenclatureDTO[]) => {
            for (const poundNet of poundNets) {
                poundNet.description = `${this.translationService.getValue('commercial-fishing.pound-net-depth-range')}: ${poundNet.depth}`;
            }

            return poundNets;
        }));
    }

    private setPermitLicenseIsValidFlag(model: CommercialFishingEditDTO): void {
        const now = new Date();
        this.permitLicenseIsValid = model.validFrom !== null
            && model.validFrom !== undefined
            && model.validFrom!.getTime() <= now.getTime()
            && model.validTo !== null
            && model.validTo !== undefined
            && model.validTo!.getTime() > now.getTime();

        this.form.get('validFromControl')!.valueChanges.subscribe({
            next: () => {
                this.updatePermitLicenseIsValidFlag();
            }
        });

        this.form.get('validToControl')!.valueChanges.subscribe({
            next: () => {
                this.updatePermitLicenseIsValidFlag();
            }
        });
    }

    private updatePermitLicenseIsValidFlag(): void {
        const now = new Date();
        const validFrom = this.form.get('validFromControl')!.value;
        const validTo = this.form.get('validToControl')!.value;

        if (validFrom !== null
            && validFrom !== undefined
            && validFrom.getTime() <= now.getTime()
            && validTo !== null
            && validTo !== undefined
            && validTo.getTime() > now.getTime()
        ) {
            this.permitLicenseIsValid = true;
        }
        else {
            this.permitLicenseIsValid = false;
        }
    }

    private shouldHidePaymentData(): boolean {
        return (this.model as CommercialFishingApplicationEditDTO).paymentInformation?.paymentType === null
            || (this.model as CommercialFishingApplicationEditDTO).paymentInformation?.paymentType === undefined
            || (this.model as CommercialFishingApplicationEditDTO).paymentInformation?.paymentType === '';
    }

    /// Validators

    private atLeastOneQuotaAquaticOrganism(): ValidatorFn {
        return (control: AbstractControl): ValidationErrors | null => {
            if (this.quotaAquaticOrganisms !== null && this.quotaAquaticOrganisms !== undefined) {
                if (this.quotaAquaticOrganisms.length > 0) {
                    return null;
                }
            }

            return { 'atLeastOneQuotaOrganism': true };
        }
    }

    private atLeastOneAquaticOrganism(): ValidatorFn {
        return (control: AbstractControl): ValidationErrors | null => {
            if (this.selectedAquaticOrganismTypes !== null && this.selectedAquaticOrganismTypes !== undefined) {
                if (this.selectedAquaticOrganismTypes.length > 0) {
                    return null;
                }
            }

            return { 'atLeastOneOrganism': true };
        }
    }

    private shipAllowedForSelectionValidator(): ValidatorFn {
        return (form: AbstractControl): ValidationErrors | null => {
            if (form === null || form === undefined) {
                return null;
            }

            const ship: ShipNomenclatureDTO | undefined = form.value;

            if (ship === null || ship === undefined) {
                return null;
            }

            if (this.shipFilters.isDestroyedOrDeregistered !== null
                && this.shipFilters.isDestroyedOrDeregistered !== undefined
                && ShipsUtils.isDestOrDereg(ship) !== this.shipFilters.isDestroyedOrDeregistered
            ) {
                return { 'shipIsDestroyedOrDeregistered': true };
            }

            if (this.shipFilters.isForbiddenForLicenses !== null
                && this.shipFilters.isForbiddenForLicenses !== undefined
                && ShipsUtils.isForbiddenForPermits(ship) !== this.shipFilters.isForbiddenForLicenses
            ) {
                return { 'shipIsForbinnedForPermitsAndLicenses': true };
            }

            if (this.shipFilters.hasActiveFishQuota !== null
                && this.shipFilters.hasActiveFishQuota !== undefined
                && ShipsUtils.hasActiveFishQuota(ship) !== this.shipFilters.hasActiveFishQuota
            ) {
                return { 'shipHasNoActiveFishQuota': true };
            }

            if (this.shipFilters.hasBlackSeaPermit !== null
                && this.shipFilters.hasBlackSeaPermit !== undefined
                && ShipsUtils.hasBlackSeaPermit(ship) !== this.shipFilters.hasBlackSeaPermit
            ) {
                if (ShipsUtils.hasBlackSeaPermit(ship)) {
                    return { 'shipHasBlackSeaPermit': true };
                }
                else {
                    return { 'shipHasNoBlackSeaPermit': true };
                }
            }

            if (this.shipFilters.hasDanubePermit !== null
                && this.shipFilters.hasDanubePermit !== undefined
                && ShipsUtils.hasDanubePermit(ship) !== this.shipFilters.hasDanubePermit
            ) {
                if (ShipsUtils.hasDanubePermit(ship)) {
                    return { 'shipHasDanubePermit': true };
                }
                else {
                    return { 'shipHasNoDanubePermit': true };
                }
            }

            if (this.selectedShipAlreadyHasValidBlackSeaPermitError) {
                return { 'shipHasBlackSeaPermit': true };
            }

            if (this.selectedShipAlreadyHasValidDanubePermitError) {
                return { 'shipHasDanubePermit': true };
            }

            if (this.selectedShipHasNoBlackSeaPermitError) {
                return { 'shipHasNoBlackSeaPermit': true };
            }

            if (this.selectedShipHasNoDanubePermitError) {
                return { 'shipHasNoDanubePermit': true };
            }

            if (this.shipFilters.hasBlackSeaPermitApplication !== null
                && this.shipFilters.hasBlackSeaPermitApplication !== undefined
                && ShipsUtils.hasBlackSeaPermitAppl(ship) !== this.shipFilters.hasBlackSeaPermitApplication
            ) {
                if (ShipsUtils.hasBlackSeaPermitAppl(ship)) {
                    return { 'shipHasBlackSeaPermit': true };
                }
                else if (!ShipsUtils.hasBlackSeaPermit(ship)) {
                    return { 'shipHasNoBlackSeaPermit': true };
                }
            }

            if (this.shipFilters.hasDanubePermitApplication !== null
                && this.shipFilters.hasDanubePermitApplication !== undefined
                && ShipsUtils.hasDanubePermitAppl(ship) !== this.shipFilters.hasDanubePermitApplication
            ) {
                if (ShipsUtils.hasDanubePermitAppl(ship)) {
                    return { 'shipHasDanubePermit': true };
                }
                else if (!ShipsUtils.hasDanubePermit(ship)) {
                    return { 'shipHasNoDanubePermit': true };
                }
            }

            if (this.shipFilters.hasPoundNetPermit !== null
                && this.shipFilters.hasPoundNetPermit !== undefined
                && ShipsUtils.hasPoundNetPermit(ship) !== this.shipFilters.hasPoundNetPermit
            ) {
                if (ShipsUtils.hasPoundNetPermit(ship)) {
                    return { 'shipHasPoundNetPermit': true };
                }
                else {
                    return { 'shipHasNoPoundNetPermit': true };
                }
            }


            if (this.selectedShipHasNoPoundNetPermitError) {
                return { 'shipHasNoPoundNetPermit': true };
            }

            if (this.shipFilters.hasPoundNetPermitApplication !== null
                && this.shipFilters.hasPoundNetPermitApplication !== undefined
                && ShipsUtils.hasPoundNetPermitAppl(ship) !== this.shipFilters.hasPoundNetPermitApplication
            ) {
                if (ShipsUtils.hasPoundNetPermitAppl(ship)) {
                    return { 'shipHasPoundNetPermit': true };
                }
                else {
                    return { 'shipHasNoPoundNetPermit': true };
                }
            }

            if (this.shipFilters.isThirdCountryShip !== null
                && this.shipFilters.isThirdCountryShip !== undefined
                && ShipsUtils.isThirdParty(ship) !== this.shipFilters.isThirdCountryShip
            ) {
                if (ShipsUtils.isThirdParty(ship)) {
                    return { 'shipIsThirdParty': true };
                }
                else {
                    return { 'shipIsNotThirdParty': true };
                }
            }

            if (this.selectedShipIsThirdCountryError) {
                return { 'shipIsThirdParty': true };
            }

            if (this.selectedShipIsNotThirdCountryError) {
                return { 'shipIsNotThirdParty': true };
            }

            return null;
        }
    }

    private updateIsSuspendedFlag(): void {
        const suspensions: SuspensionDataDTO[] = this.form.get('suspensionsControl')!.value;

        if (suspensions !== null && suspensions !== undefined) {
            const now: Date = new Date();

            this.isSuspended = suspensions.some(x =>
                x.isActive
                && ((x.enactmentDate !== null && x.enactmentDate !== undefined && x.enactmentDate <= now)
                    || (x.validFrom !== null && x.validFrom !== undefined && x.validFrom <= now))
                && ((x.validTo !== null && x.validTo !== undefined && x.validTo > now)
                    || x.validTo === null
                    || x.validTo === undefined)
            );
        }
        else {
            this.isSuspended = false;
        }
    }

    private permitRegisterForPermitLicenseValidator(): ValidatorFn {
        return (form: AbstractControl): ValidationErrors | null => {
            if (form === null || form === undefined) {
                return null;
            }

            if (this.isApplication || !this.isPermitLicense) {
                return null;
            }

            if (this.hasNoPermitRegisterForPermitLicenseError) {
                return { 'noPermitRegisterForPermitLicense': true };
            }

            return null;
        }
    }

    private oneShipEventOnSameDateValidator(): ValidatorFn {
        return (form: AbstractControl): ValidationErrors | null => {
            if (form === null || form === undefined) {
                return null;
            }

            if (this.hasShipEventExistsOnSameDateError && !this.isPermitLicense && !this.isApplication) {
                if (this.isEditing) {
                    const suspensions: SuspensionDataDTO[] = this.form.get('suspensionsControl')!.value;

                    if (suspensions !== null && suspensions !== undefined && suspensions.length > 0) {
                        return { 'suspendResumeIsSecondShipEventToday': true };
                    }
                    else {
                        return null;
                    }
                }
                else {
                    return { 'addPermitIsSecondShipEventToday': true };
                }
            }

            return null;
        }
    }

    private uniqueQuotaPortValidator(): ValidatorFn {
        return (form: AbstractControl): ValidationErrors | null => {
            if (form === null || form === undefined) {
                return null;
            }

            const groupedByFishId = CommonUtils.groupBy(this.quotaAquaticOrganisms, x => x.aquaticOrganismId!);

            for (const fishGroupId in groupedByFishId) {
                const fishGrouped = groupedByFishId[fishGroupId];
                const groupedByPort = CommonUtils.groupBy(fishGrouped, x => x.portId!);

                for (const portGroupId in groupedByPort) {
                    if (groupedByPort[portGroupId].length > 1) {

                        const fishName: string = this.quotaAquaticOrganismTypes.find(x => x.value === Number(fishGroupId))!.displayName!;
                        const portName: string = this.ports.find(x => x.value === Number(portGroupId))!.displayName!;

                        return {
                            quotaPortUnique: {
                                fishName: fishName,
                                portName: portName
                            }
                        };
                    }
                }
            }

            return null;
        }
    }

    // helpers

    private deepCopyPorts(ports: NomenclatureDTO<number>[]): NomenclatureDTO<number>[] {
        if (ports !== null && ports !== undefined) {
            const copiedPorts: NomenclatureDTO<number>[] = [];

            for (const port of ports) {
                const stringified: string = JSON.stringify(port);
                const newPort: NomenclatureDTO<number> = new NomenclatureDTO<number>(JSON.parse(stringified));

                copiedPorts.push(newPort);
            }

            return copiedPorts;
        }
        else {
            return [];
        }
    }
}