import { AfterViewInit, Component, EventEmitter, OnInit, ViewChild } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { forkJoin, Observable, Subject } from 'rxjs';

import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { CommonNomenclatures } from '@app/services/common-app/common-nomenclatures.service';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { DialogParamsModel } from '@app/models/common/dialog-params.model';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { RegixPersonDataDTO } from '@app/models/generated/dtos/RegixPersonDataDTO';
import { BuyerRegixDataDTO } from '@app/models/generated/dtos/BuyerRegixDataDTO';
import { BuyerApplicationEditDTO } from '@app/models/generated/dtos/BuyerApplicationEditDTO';
import { BuyerEditDTO } from '@app/models/generated/dtos/BuyerEditDTO';
import { IApplicationsService } from '@app/interfaces/administration-app/applications.interface';
import { ApplicationContentDTO } from '@app/models/generated/dtos/ApplicationContentDTO';
import { RegixChecksWrapperDTO } from '@app/models/generated/dtos/RegixChecksWrapperDTO';
import { ApplicationDialogData, ApplicationUtils } from '@app/shared/utils/application.utils';
import { PageCodeEnum } from '@app/enums/page-code.enum';
import { IBuyersService } from '@app/interfaces/common-app/buyers.interface';
import { ApplicationPaymentInformationDTO } from '@app/models/generated/dtos/ApplicationPaymentInformationDTO';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { HttpErrorResponse } from '@angular/common/http';
import { ApplicationRegiXCheckDTO } from '@app/models/generated/dtos/ApplicationRegiXCheckDTO';
import { ApplicationSubmittedByRegixDataDTO } from '@app/models/generated/dtos/ApplicationSubmittedByRegixDataDTO';
import { ApplicationSubmittedForRegixDataDTO } from '@app/models/generated/dtos/ApplicationSubmittedForRegixDataDTO';
import { ApplicationValidationErrorsEnum } from '@app/enums/application-validation-errors.enum';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { UsageDocumentDTO } from '@app/models/generated/dtos/UsageDocumentDTO';
import { UsageDocumentRegixDataDTO } from '@app/models/generated/dtos/UsageDocumentRegixDataDTO';
import { TLDataTableComponent } from '@app/shared/components/data-table/tl-data-table.component';
import { UsageDocumentDialogParams } from '@app/shared/components/usage-document/models/usage-document-dialog-params.model';
import { IHeaderAuditButton } from '@app/shared/components/dialog-wrapper/interfaces/header-audit-button.interface';
import { IS_PUBLIC_APP } from '@app/shared/modules/application.modules';
import { TLMatDialog } from '@app/shared/components/dialog-wrapper/tl-mat-dialog';
import { UsageDocumentComponent } from '@app/shared/components/usage-document/usage-document.component';
import { HeaderCloseFunction } from '@app/shared/components/dialog-wrapper/interfaces/header-cancel-button.interface';
import { GridRow } from '@app/shared/components/data-table/models/row.model';
import { TLConfirmDialog } from '@app/shared/components/confirmation-dialog/tl-confirm-dialog';
import { LogBookGroupsEnum } from '@app/enums/log-book-groups.enum';
import { ApplicationSubmittedByDTO } from '@app/models/generated/dtos/ApplicationSubmittedByDTO';
import { CustodianOfPropertyDTO } from '@app/models/generated/dtos/CustodianOfPropertyDTO';
import { BuyerChangeOfCircumstancesApplicationDTO } from '@app/models/generated/dtos/BuyerChangeOfCircumstancesApplicationDTO';
import { ErrorCode, ErrorModel } from '@app/models/common/exception.model';
import { LogBookEditDTO } from '@app/models/generated/dtos/LogBookEditDTO';
import { ValidityCheckerGroupDirective } from '@app/shared/directives/validity-checker/validity-checker-group.directive';
import { BuyersPublicService } from '@app/services/public-app/buyers-public.service';
import { FileTypeEnum } from '@app/enums/file-types.enum';
import { Notifier } from '@app/shared/directives/notifier/notifier.class';
import { CommonDocumentDTO } from '@app/models/generated/dtos/CommonDocumentDTO';
import { CommonDocumentDialogParams } from '@app/shared/components/common-document/models/common-document-dialog-params.model';
import { CommonDocumentComponent } from '@app/shared/components/common-document/common-document.component';
import { CancellationHistoryDialogComponent } from '@app/shared/components/cancellation-history-dialog/cancellation-history-dialog.component';
import { CancellationHistoryEntryDTO } from '@app/models/generated/dtos/CancellationHistoryEntryDTO';
import { CancellationReasonGroupEnum } from '@app/enums/cancellation-reason-group.enum';
import { CancellationHistoryDialogParams } from '@app/shared/components/cancellation-history-dialog/cancellation-history-dialog-params.model';
import { BuyerStatusesEnum } from '@app/enums/buyer-statuses.enum';
import { BuyerTerminationApplicationDTO } from '@app/models/generated/dtos/BuyerTerminationApplicationDTO';
import { OverlappingLogBooksParameters } from '@app/shared/components/overlapping-log-books/models/overlapping-log-books-parameters.model';
import { OverlappingLogBooksDialogParamsModel } from '@app/shared/components/overlapping-log-books/models/overlapping-log-books-dialog-params.model';
import { CommercialFishingAdministrationService } from '@app/services/administration-app/commercial-fishing-administration.service';
import { OverlappingLogBooksComponent } from '@app/shared/components/overlapping-log-books/overlapping-log-books.component';
import { PermittedFileTypeDTO } from '@app/models/generated/dtos/PermittedFileTypeDTO';
import { DuplicatesEntryDTO } from '@app/models/generated/dtos/DuplicatesEntryDTO';
import { AddressTypesEnum } from '@app/enums/address-types.enum';

enum AgentSameAsTypesEnum {
    SubmittedByPerson,
    SubmittedForCustodianOfPropertyPerson,
    Other
}

type SaveMethod = 'save' | 'saveAndPrint' | 'completeChangeOfCircumstancesApplication';

@Component({
    selector: 'edit-buyers.component',
    templateUrl: './edit-buyers.component.html',
    styleUrls: ['./edit-buyers.component.scss']
})
export class EditBuyersComponent implements OnInit, AfterViewInit, IDialogComponent {
    public readonly pageCodeEnum: typeof PageCodeEnum = PageCodeEnum;
    public readonly addressTypesEnum: typeof AddressTypesEnum = AddressTypesEnum;
    public readonly today: Date = new Date();

    public editForm!: FormGroup;
    public pageCode!: PageCodeEnum;
    public notifier: Notifier = new Notifier();
    public expectedResults: BuyerRegixDataDTO;
    public regixChecks: ApplicationRegiXCheckDTO[] = [];
    public duplicates: DuplicatesEntryDTO[] = [];
    public service!: IBuyersService;
    public applicationPaymentInformation: ApplicationPaymentInformationDTO | undefined;
    public agentSameAsOptions: NomenclatureDTO<AgentSameAsTypesEnum>[] = []; // needed only for first sale buyers
    public isAgentSameAsSubmittedForCustodian: boolean = false; // needed only for first sale buyers
    public selectedDocumentForUseType: NomenclatureDTO<number> | undefined;
    public territoryUnits: NomenclatureDTO<number>[] = [];
    public usageDocumentTypes: NomenclatureDTO<number>[] = [];
    public premiseUsageDocuments: UsageDocumentDTO[] = []; // needed only for recordType Register cases
    public buyerStatuses: NomenclatureDTO<number>[] = []; // needed only fore RecordType Register cases
    public babhLicenses: CommonDocumentDTO[] = [];
    public veterinaryVehicleLicenses: CommonDocumentDTO[] = [];
    public cancellationHistory: CancellationHistoryEntryDTO[] = [];
    public cancellationReasons: NomenclatureDTO<number>[] = [];

    public viewMode: boolean = false;
    public isReadonly: boolean = false;
    public isApplication: boolean = false;
    public showOnlyRegiXData: boolean = false;
    public showRegiXData: boolean = false;
    public isApplicationHistoryMode: boolean = false;
    public loadRegisterFromApplication: boolean = false;
    public isFirstSaleCenter: boolean = false;
    public isFirstSaleBuyer: boolean = false;
    public isEditing: boolean = false;
    public isEditingSubmittedBy: boolean = false;
    public isChangeOfCircumstancesApplication: boolean = false;
    public isDeregistrationApplication: boolean = false;
    public isRegisterEntry: boolean = false;

    public isPublicApp: boolean = false;
    public isPaid: boolean = false;
    public hasDelivery: boolean = false;
    public isOnlineApplication: boolean = false;
    public hideBasicPaymentInfo: boolean = false;
    public refreshFileTypes: Subject<void> = new Subject<void>();

    public hasNoEDeliveryRegistrationError: boolean = false;
    public premiseUsageDocumentsTouched: boolean = false;
    public babhLicensesTouched: boolean = false;
    public veterinaryVehicleLicensesTouched: boolean = false;

    public changeOfCircumstancesControl: FormControl = new FormControl();
    public deregistrationReasonControl: FormControl = new FormControl();

    public logBookGroup: LogBookGroupsEnum = LogBookGroupsEnum.DeclarationsAndDocuments;

    @ViewChild('premiseUsageDocumentsTable')
    private premiseUsageDocumentsTable!: TLDataTableComponent;

    @ViewChild(ValidityCheckerGroupDirective)
    private validityCheckerGroup!: ValidityCheckerGroupDirective;

    @ViewChild('babhLicensesTable')
    private babhLicensesTable!: TLDataTableComponent;

    @ViewChild('veterinaryVehicleLicensesTable')
    private veterinaryVehicleLicensesTable!: TLDataTableComponent;

    private id?: number;
    private applicationId?: number;
    private model!: BuyerEditDTO | BuyerApplicationEditDTO | BuyerRegixDataDTO;

    private allBuyerStatuses: NomenclatureDTO<number>[] = [];

    private translationService: FuseTranslationLoaderService;
    private applicationsService?: IApplicationsService;
    private commonNomenclatureService: CommonNomenclatures;
    private editUsageDocumentDialog: TLMatDialog<UsageDocumentComponent>;
    private editBabhLicenseDialog: TLMatDialog<CommonDocumentComponent>;
    private editVeterinaryVehicleLicenseDialog: TLMatDialog<CommonDocumentComponent>;
    private cancelDialog: TLMatDialog<CancellationHistoryDialogComponent>;
    private confirmDialog: TLConfirmDialog;
    private overlappingLogBooksDialog: TLMatDialog<OverlappingLogBooksComponent>;
    private commercialFishingService: CommercialFishingAdministrationService;

    private dialogRightSideActions: Array<IActionInfo> | undefined;

    private ignoreLogBookConflicts: boolean = false;

    public constructor(translationService: FuseTranslationLoaderService,
        commonNomenclatureService: CommonNomenclatures,
        commercialFishingService: CommercialFishingAdministrationService,
        editUsageDocumentDialog: TLMatDialog<UsageDocumentComponent>,
        editBabhLicenseDialog: TLMatDialog<CommonDocumentComponent>,
        editVeterinaryVehicleLicenseDialog: TLMatDialog<CommonDocumentComponent>,
        cancelDialog: TLMatDialog<CancellationHistoryDialogComponent>,
        confirmDialog: TLConfirmDialog,
        overlappingLogBooksDialog: TLMatDialog<OverlappingLogBooksComponent>
    ) {
        this.commonNomenclatureService = commonNomenclatureService;
        this.commercialFishingService = commercialFishingService;
        this.translationService = translationService;
        this.editUsageDocumentDialog = editUsageDocumentDialog;
        this.editBabhLicenseDialog = editBabhLicenseDialog;
        this.editVeterinaryVehicleLicenseDialog = editVeterinaryVehicleLicenseDialog;
        this.cancelDialog = cancelDialog;
        this.confirmDialog = confirmDialog;
        this.overlappingLogBooksDialog = overlappingLogBooksDialog;

        this.isPublicApp = IS_PUBLIC_APP;

        this.expectedResults = new BuyerRegixDataDTO({
            submittedBy: new ApplicationSubmittedByRegixDataDTO(),
            submittedFor: new ApplicationSubmittedForRegixDataDTO(),
            organizer: new RegixPersonDataDTO(),
            premiseUsageDocument: new UsageDocumentRegixDataDTO()
        });

        this.agentSameAsOptions = [
            new NomenclatureDTO<AgentSameAsTypesEnum>({
                value: AgentSameAsTypesEnum.SubmittedByPerson,
                displayName: this.translationService.getValue('buyers-and-sales-centers.agent-same-as-submitted-by'),
                isActive: true
            }),
            new NomenclatureDTO<AgentSameAsTypesEnum>({
                value: AgentSameAsTypesEnum.SubmittedForCustodianOfPropertyPerson,
                displayName: this.translationService.getValue('buyers-and-sales-centers.agent-same-as-submitted-for-custodian-of-property'),
                isActive: true
            }),
            new NomenclatureDTO<AgentSameAsTypesEnum>({
                value: AgentSameAsTypesEnum.Other,
                displayName: this.translationService.getValue('buyers-and-sales-centers.agent-is-different-person'),
                isActive: true
            })
        ];
    }

    public async ngOnInit(): Promise<void> {
        if (!this.isApplication) {
            const nomenclatures: NomenclatureDTO<number>[][] = await forkJoin(
                NomenclatureStore.instance.getNomenclature(NomenclatureTypes.TerritoryUnits, this.commonNomenclatureService.getTerritoryUnits.bind(this.commonNomenclatureService), false),
                NomenclatureStore.instance.getNomenclature(NomenclatureTypes.UsageDocumentTypes, this.commonNomenclatureService.getUsageDocumentTypes.bind(this.commonNomenclatureService), false),
                NomenclatureStore.instance.getNomenclature(NomenclatureTypes.BuyerStatuses, this.service.getBuyerStatuses.bind(this.service), false),
                NomenclatureStore.instance.getNomenclature(NomenclatureTypes.CancellationReasons, this.commonNomenclatureService.getCancellationReasons.bind(this.commonNomenclatureService), false)
            ).toPromise();

            this.territoryUnits = nomenclatures[0];
            this.usageDocumentTypes = nomenclatures[1];
            this.allBuyerStatuses = this.buyerStatuses = nomenclatures[2];
            this.cancellationReasons = nomenclatures[3];

            this.buyerStatuses = this.buyerStatuses.filter((status: NomenclatureDTO<number>) => {
                const notVisibleStatuses: BuyerStatusesEnum[] = [
                    BuyerStatusesEnum.Canceled
                ];

                const stat: BuyerStatusesEnum = BuyerStatusesEnum[status.code as keyof typeof BuyerStatusesEnum];
                return !notVisibleStatuses.includes(stat);
            });
        }

        if (!this.showOnlyRegiXData) {
            (this.editForm.get('hasPremiseOrVehicleGroup') as FormGroup).get('hasUtilityControl')!.valueChanges.subscribe({
                next: (hasUtility: boolean | undefined) => {
                    if (hasUtility === true) {
                        this.editForm.get('premiseNameControl')!.setValidators(Validators.required);
                        this.editForm.get('premiseNameControl')!.markAsPending();
                        this.editForm.get('premiseNameControl')!.updateValueAndValidity();

                        if (this.viewMode || this.isReadonly) {
                            this.editForm.get('premiseNameControl')!.disable({ emitEvent: false });
                        }
                    }
                    else {
                        this.editForm.get('premiseAddressControl')!.setValue(null);
                        this.editForm.get('premiseAddressControl')!.clearValidators();
                        this.editForm.get('premiseAddressControl')!.updateValueAndValidity();
                        this.editForm.get('premiseNameControl')!.setValue(null);
                        this.editForm.get('premiseNameControl')!.clearValidators();
                        this.editForm.get('premiseNameControl')!.updateValueAndValidity();
                    }
                }
            });

            (this.editForm.get('hasPremiseOrVehicleGroup') as FormGroup).get('hasVehicleControl')!.valueChanges.subscribe({
                next: (hasVehicle: boolean | undefined) => {
                    if (hasVehicle === true) {
                        this.editForm.get('vehicleNumberControl')!.setValidators(Validators.required);
                        this.editForm.get('vehicleNumberControl')!.markAsPending();
                        this.editForm.get('vehicleNumberControl')!.updateValueAndValidity();

                        if (this.viewMode || this.isReadonly) {
                            this.editForm.get('vehicleNumberControl')!.disable({ emitEvent: false });
                        }
                    }
                    else {
                        this.editForm.get('vehicleNumberControl')!.setValue(null);

                        const formValidators: ValidatorFn | null = this.editForm.validator;

                        if (formValidators !== null && formValidators !== undefined) {
                            this.editForm.setValidators([formValidators]);
                        }
                        else {
                            this.editForm.setValidators(this.atLeastOneActiveVehicleBabhDocumentValidator());
                        }

                        this.editForm.get('vehicleNumberControl')!.clearValidators();
                        this.editForm.get('vehicleNumberControl')!.updateValueAndValidity();
                    }
                }
            });
        }

        if (this.isApplication === true && !this.showOnlyRegiXData) {
            if (this.isFirstSaleBuyer) {
                this.editForm.get('agentSameAsControl')!.valueChanges.subscribe({
                    next: (agentSameAs: NomenclatureDTO<AgentSameAsTypesEnum> | undefined) => {
                        if (agentSameAs !== null && agentSameAs !== undefined) {
                            switch (agentSameAs.value) {
                                case AgentSameAsTypesEnum.SubmittedByPerson: {
                                    this.editForm.get('agentControl')!.setValue(this.editForm.get('submittedByControl')!.value?.person);
                                    this.editForm.get('agentControl')!.disable();
                                    this.isAgentSameAsSubmittedForCustodian = false;
                                } break;
                                case AgentSameAsTypesEnum.SubmittedForCustodianOfPropertyPerson: {
                                    this.mapSubmittedForCustodianOfPropertyToAgent(this.editForm.get('submittedForControl')!.value);

                                    if (this.viewMode || this.isReadonly) {
                                        this.editForm.disable({ emitEvent: false });
                                    }
                                    else {
                                        this.editForm.get('agentControl')!.enable();
                                    }

                                    this.isAgentSameAsSubmittedForCustodian = true;
                                } break;
                                case AgentSameAsTypesEnum.Other: {
                                    this.isAgentSameAsSubmittedForCustodian = false;

                                    if (this.viewMode || this.isReadonly) {
                                        this.editForm.disable({ emitEvent: false });
                                    }
                                    else {
                                        this.editForm.get('agentControl')!.enable();
                                    }
                                } break;
                                default:
                                    this.editForm.get('agentControl')!.enable();
                                    this.isAgentSameAsSubmittedForCustodian = false;
                                    break;
                            }
                        }
                        else {
                            this.isAgentSameAsSubmittedForCustodian = false;

                            if (this.viewMode || this.isReadonly) {
                                this.editForm.disable({ emitEvent: false });
                            }
                        }
                    }
                });
            }
            else if (this.isFirstSaleCenter) {
                this.editForm.get('organizerSameAsSubmittedByControl')!.valueChanges.subscribe({
                    next: (value: boolean) => {
                        if (value === true) {
                            this.editForm.get('organizerControl')!.setValue(this.editForm.get('submittedByControl')!.value?.person);
                            this.editForm.get('organizerControl')!.disable();
                        }
                        else {
                            this.editForm.get('organizerControl')!.reset();
                            this.editForm.get('organizerControl')!.enable();

                            if (this.viewMode || this.isReadonly) {
                                this.editForm.disable({ emitEvent: false });
                            }
                        }
                    }
                });
            }

            this.editForm.get('submittedByControl')!.valueChanges.subscribe({
                next: (value: ApplicationSubmittedByDTO | undefined) => {
                    if (this.isFirstSaleBuyer) {
                        const agentSameAsValue: NomenclatureDTO<AgentSameAsTypesEnum> | undefined = this.editForm.get('agentSameAsControl')!.value;
                        if (agentSameAsValue !== null && agentSameAsValue !== undefined && agentSameAsValue.value === AgentSameAsTypesEnum.SubmittedByPerson) {
                            this.editForm.get('agentControl')!.setValue(value?.person);
                        }
                    }
                    else if (this.isFirstSaleCenter && this.editForm.get('organizerSameAsSubmittedByControl')!.value === true) {
                        this.editForm.get('organizerControl')!.setValue(value?.person);
                    }
                }
            });

            this.editForm.get('submittedForControl')!.valueChanges.subscribe({
                next: (value: ApplicationSubmittedForRegixDataDTO | undefined) => {
                    if (this.isFirstSaleBuyer) {
                        const agentSameAsValue: NomenclatureDTO<AgentSameAsTypesEnum> | undefined = this.editForm.get('agentSameAsControl')!.value;
                        if (agentSameAsValue !== null && agentSameAsValue !== undefined && agentSameAsValue.value === AgentSameAsTypesEnum.SubmittedForCustodianOfPropertyPerson) {
                            this.mapSubmittedForCustodianOfPropertyToAgent(value);
                        }
                    }
                }
            });
        }

        // данни от заялвение за промяна в обстоятелствата
        if (this.isChangeOfCircumstancesApplication && this.applicationId !== undefined && this.applicationId !== null) {
            this.isRegisterEntry = true;
            this.fillForm();

            this.service.getApplication(this.applicationId, false, this.pageCode).subscribe({
                next: (application: BuyerChangeOfCircumstancesApplicationDTO) => {
                    this.changeOfCircumstancesControl.setValue(application.changes);
                    this.changeOfCircumstancesControl.disable();
                }
            });
        }
        else if (this.isDeregistrationApplication === true && this.applicationId !== undefined && this.applicationId !== null) {
            this.isRegisterEntry = true;
            this.fillForm();

            this.service.getApplication(this.applicationId, false, this.pageCode).subscribe({
                next: (application: BuyerTerminationApplicationDTO) => {
                    this.deregistrationReasonControl.setValue(application.deregistrationReason);
                    this.deregistrationReasonControl.disable();
                }
            });
        }
        else if (this.isApplicationHistoryMode && this.applicationId !== undefined) { // извличане на исторически данни за заявление
            this.editForm.disable();

            if (this.applicationsService !== null && this.applicationsService !== undefined) {
                this.applicationsService.getApplicationChangeHistoryContent(this.applicationId!).subscribe({
                    next: (content: ApplicationContentDTO) => {
                        const contentObject: Record<string, unknown> = JSON.parse(content.draftContent!);
                        const application: BuyerApplicationEditDTO = new BuyerApplicationEditDTO(contentObject);
                        application.files = content.files;
                        application.applicationId = content.applicationId;

                        this.model = new BuyerApplicationEditDTO(application);

                        if (this.model instanceof BuyerApplicationEditDTO) {
                            this.model.pageCode = this.pageCode;
                            this.isPaid = this.model.isPaid!;
                            this.hasDelivery = this.model.hasDelivery!;
                            this.applicationPaymentInformation = this.model.paymentInformation;
                            this.hideBasicPaymentInfo = this.shouldHidePaymentData();
                        }

                        this.isOnlineApplication = application.isOnlineApplication!;
                        this.refreshFileTypes.next();
                        this.fillForm();
                    }
                });
            }
            else {
                throw new Error('applicationsService cannot be null/undefined in applicationsHistoryMode');
            }
        }
        else if ((this.id === undefined || this.id === null) && this.applicationId !== undefined && this.applicationId !== null && !this.isApplication) {
            if (this.loadRegisterFromApplication === true) {  // извличане на данни за регистър по id на заявление
                if (this.isReadonly || this.viewMode) {
                    this.editForm.disable();
                }
                this.isEditing = true;
                this.isEditingSubmittedBy = true;

                this.service.getRegisterByApplicationId(this.applicationId, this.pageCode).subscribe({
                    next: (buyerRegister: unknown) => {
                        this.model = new BuyerEditDTO(buyerRegister as BuyerEditDTO);
                        this.model.pageCode = this.pageCode;

                        this.isOnlineApplication = (buyerRegister as BuyerEditDTO).isOnlineApplication!;
                        this.refreshFileTypes.next();
                        this.fillForm();
                    }
                });
            }
            else { // извличане на данни за създаване на регистров запис от заявление
                this.isEditing = false;
                this.isEditingSubmittedBy = false;

                this.service.getApplicationDataForRegister(this.applicationId!, this.pageCode).subscribe({
                    next: (result: BuyerEditDTO) => {
                        this.model = result;

                        this.isOnlineApplication = result.isOnlineApplication!;
                        this.refreshFileTypes.next();
                        this.fillForm();
                    }
                });
            }
        }
        else {
            if (this.isReadonly || this.viewMode) {
                this.editForm.disable();
            }

            if (this.isApplication) {
                if (this.showOnlyRegiXData) {
                    // извличане на данни за RegiX сверка от служител
                    this.isEditing = false;
                    this.isEditingSubmittedBy = false;

                    this.service.getRegixData(this.applicationId!, this.pageCode).subscribe({
                        next: (regixData: RegixChecksWrapperDTO<BuyerRegixDataDTO>) => {
                            this.model = new BuyerRegixDataDTO(regixData.dialogDataModel);

                            this.expectedResults = new BuyerRegixDataDTO(regixData.regiXDataModel);
                            this.expectedResults.applicationId = this.applicationId;
                            this.expectedResults.id = this.model.id;

                            this.fillForm();
                        }
                    });
                }
                else {
                    // извличане на данни за заявление
                    this.isEditing = false;
                    this.isEditingSubmittedBy = false;

                    this.service.getApplication(this.applicationId!, this.showRegiXData, this.pageCode).subscribe({
                        next: (application: BuyerApplicationEditDTO | null | undefined) => {
                            if (application === null || application === undefined) {
                                application = new BuyerApplicationEditDTO({ applicationId: this.applicationId!, pageCode: this.pageCode });
                            }
                            else {
                                application.applicationId = this.applicationId!;
                                application.pageCode = this.pageCode;
                            }

                            this.isOnlineApplication = application.isOnlineApplication!;
                            this.refreshFileTypes.next();

                            if (this.showRegiXData) {
                                this.expectedResults = new BuyerRegixDataDTO(application.regiXDataModel);
                                application.regiXDataModel = undefined;
                            }

                            this.model = new BuyerApplicationEditDTO(application);

                            if (this.model instanceof BuyerApplicationEditDTO) {
                                this.model.pageCode = this.pageCode;
                                this.isPaid = this.model.isPaid!;
                                this.hasDelivery = this.model.hasDelivery!;
                                this.applicationPaymentInformation = this.model.paymentInformation;
                                this.hideBasicPaymentInfo = this.shouldHidePaymentData();
                                this.isOnlineApplication = this.model.isOnlineApplication!;

                                if (this.isPublicApp && this.isOnlineApplication) {
                                    this.isEditingSubmittedBy = true;

                                    if (this.model.submittedBy === undefined || this.model.submittedBy === null) {
                                        const service = this.service as BuyersPublicService;
                                        service.getCurrentUserAsSubmittedBy().subscribe({
                                            next: (submittedBy: ApplicationSubmittedByDTO) => {
                                                (this.model as BuyerApplicationEditDTO).submittedBy = submittedBy;
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
            else {
                // извличане на данни за регистров запис
                this.isEditing = true;
                this.isEditingSubmittedBy = true;

                this.service.get(this.id!).subscribe({
                    next: (fisher: BuyerEditDTO) => {
                        this.model = fisher;
                        this.isOnlineApplication = fisher.isOnlineApplication!;
                        this.refreshFileTypes.next();
                        this.fillForm();
                    }
                });
            }
        }
    }

    public ngAfterViewInit(): void {
        if (this.isApplication) {
            this.editForm.get('deliveryDataControl')!.valueChanges.subscribe({
                next: () => {
                    this.hasNoEDeliveryRegistrationError = false;
                }
            });
        }
    }

    public setData(data: DialogParamsModel, buttons: DialogWrapperData): void {
        this.id = data.id;
        this.applicationId = data.applicationId;
        this.viewMode = data.viewMode;
        this.isReadonly = data.isReadonly;
        this.isApplication = data.isApplication;
        this.showOnlyRegiXData = data.showOnlyRegiXData;
        this.showRegiXData = data.showRegiXData;
        this.isApplicationHistoryMode = data.isApplicationHistoryMode;
        this.service = data.service as IBuyersService;
        this.applicationsService = data.applicationsService;
        this.dialogRightSideActions = buttons.rightSideActions;
        this.pageCode = data.pageCode;
        this.loadRegisterFromApplication = data.loadRegisterFromApplication;

        if (!this.loadRegisterFromApplication) {
            this.isChangeOfCircumstancesApplication = this.pageCode === PageCodeEnum.ChangeFirstSaleBuyer || this.pageCode === PageCodeEnum.ChangeFirstSaleCenter;
            this.isDeregistrationApplication = this.pageCode === PageCodeEnum.TermFirstSaleBuyer || this.pageCode === PageCodeEnum.TermFirstSaleCenter;
        }
        this.isFirstSaleCenter = data.pageCode === PageCodeEnum.RegFirstSaleCenter || data.pageCode === PageCodeEnum.ChangeFirstSaleCenter || data.pageCode === PageCodeEnum.TermFirstSaleCenter;;
        this.isFirstSaleBuyer = data.pageCode === PageCodeEnum.RegFirstSaleBuyer || data.pageCode === PageCodeEnum.ChangeFirstSaleBuyer || data.pageCode === PageCodeEnum.TermFirstSaleBuyer;

        if (data.model !== undefined && data.model !== null) {
            this.model = data.model;
        }

        this.buildForm();
    }

    public saveBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        this.markAllAsTouched();
        this.validityCheckerGroup.validate();

        if (this.editForm.valid) {
            this.save(dialogClose);
        }
    }

    public cancelBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public dialogButtonClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        if (actionInfo.id === 'print') {
            if ((this.viewMode || this.isReadonly) && this.model instanceof BuyerEditDTO) {
                this.service.downloadRegister(this.model.id!, this.model.buyerType!).subscribe({
                    next: () => {
                        // nothing to do
                    }
                });
            }
            else {
                this.markAllAsTouched();
                this.validityCheckerGroup.validate();

                if (this.editForm.valid) {
                    this.saveAndPrintRecord(dialogClose);
                }
            }
        }
        else if (actionInfo.id === 'cancel' || actionInfo.id === 'activate') {
            if (this.isChangeOfCircumstancesApplication) {
                this.markAllAsTouched();
                this.validityCheckerGroup.validate();

                if (this.editForm.valid) {
                    this.fillModel();
                    CommonUtils.sanitizeModelStrings(this.model);

                    return this.openCancelDialog(actionInfo, dialogClose, actionInfo.id === 'cancel');
                }
            }
            else {
                return this.openCancelDialog(actionInfo, dialogClose, actionInfo.id === 'cancel');
            }
        }
        else {
            let applicationAction: boolean = false;

            if (this.model instanceof BuyerApplicationEditDTO || this.model instanceof BuyerRegixDataDTO) {
                this.fillModel();
                CommonUtils.sanitizeModelStrings(this.model);

                applicationAction = ApplicationUtils.applicationDialogButtonClicked(new ApplicationDialogData({
                    action: actionInfo,
                    dialogClose: dialogClose,
                    applicationId: this.applicationId!,
                    model: this.model,
                    readOnly: this.isReadonly,
                    viewMode: this.viewMode,
                    editForm: this.editForm,
                    saveFn: this.saveData.bind(this),
                    onMarkAsTouched: () => {
                        this.validityCheckerGroup.validate();
                    }
                }));
            }

            if (!this.isReadonly && !this.viewMode && !applicationAction) {
                if (actionInfo.id === 'save') {
                    return this.saveBtnClicked(actionInfo, dialogClose);
                }
            }
        }
    }

    public addEditPremiseUsageDocument(document: UsageDocumentDTO | undefined, viewMode: boolean = false): void {
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
                    getAuditRecordData: this.service.getPremiseUsageDocumentAudit.bind(this.service),
                    tableName: 'BuyerPremiseUsageDocument'
                };
            }

            if (this.isReadonly || viewMode) {
                title = this.translationService.getValue('buyers-and-sales-centers.view-premise-usage-document-dialog-title');
            }
            else {
                title = this.translationService.getValue('buyers-and-sales-centers.edit-premise-usage-document-dialog-title');
            }
        }
        else {
            data = new UsageDocumentDialogParams({
                model: undefined,
                showOnlyRegiXData: false,
                isIdReadOnly: false,
                viewMode: false
            });

            title = this.translationService.getValue('buyers-and-sales-centers.add-premise-usage-document-dialog-title');
        }

        const dialog = this.editUsageDocumentDialog.openWithTwoButtons({
            title: title,
            TCtor: UsageDocumentComponent,
            headerAuditButton: auditBtn,
            headerCancelButton: {
                cancelBtnClicked: this.closeEditPremiseUsageDocumentDialogBtnClicked.bind(this)
            },
            componentData: data,
            translteService: this.translationService,
            viewMode: viewMode
        }, '1350px');

        dialog.subscribe({
            next: (result: UsageDocumentDTO | undefined) => {
                if (result !== undefined) {
                    if (document !== undefined) { // edit
                        const idx: number = this.premiseUsageDocuments.findIndex(x => x === document);
                        this.premiseUsageDocuments[idx] = result;
                    }
                    else { // add
                        this.premiseUsageDocuments.push(result);
                    }

                    this.premiseUsageDocumentsTouched = true;
                    this.editForm.updateValueAndValidity({ emitEvent: false });

                    setTimeout(() => {
                        this.premiseUsageDocuments = this.premiseUsageDocuments.slice();
                    });
                }
            }
        });
    }

    public deletePremiseUsageDocument(document: GridRow<UsageDocumentDTO>): void {
        this.confirmDialog.open({
            title: this.translationService.getValue('buyers-and-sales-centers.delete-premise-usage-document-dialog-title'),
            message: this.translationService.getValue('buyers-and-sales-centers.delete-premise-usage-document-dialog-message'),
            okBtnLabel: this.translationService.getValue('buyers-and-sales-centers.delete-premise-usage-document-dialog-ok-btn-label')
        }).subscribe({
            next: (ok: boolean) => {
                if (ok) {
                    this.premiseUsageDocumentsTable.softDelete(document);

                    this.premiseUsageDocumentsTouched = true;
                    this.editForm.updateValueAndValidity({ emitEvent: false });
                }
            }
        });
    }

    public undoDeletePremiseUsageDocument(document: GridRow<UsageDocumentDTO>): void {
        this.confirmDialog.open().subscribe({
            next: (ok: boolean) => {
                if (ok) {
                    this.premiseUsageDocumentsTable.softUndoDelete(document);

                    this.premiseUsageDocumentsTouched = true;
                    this.editForm.updateValueAndValidity({ emitEvent: false });
                }
            }
        });
    }

    public addEditBabhLicense(license: CommonDocumentDTO | undefined, viewMode: boolean = false): void {
        let data: CommonDocumentDialogParams | undefined;
        let auditBtn: IHeaderAuditButton | undefined;
        let title: string;

        if (license !== undefined) {
            data = new CommonDocumentDialogParams({
                model: license,
                viewMode: this.isReadonly || viewMode
            });

            if (license.id !== undefined && !IS_PUBLIC_APP) {
                auditBtn = {
                    id: license.id,
                    getAuditRecordData: this.service.getBuyerLicensesAudit.bind(this.service),
                    tableName: 'BuyerLicense'
                };
            }

            if (this.isReadonly || viewMode) {
                title = this.translationService.getValue('buyers-and-sales-centers.view-buyers-and-sales-centers-babh-license-dialog-title');
            }
            else {
                title = this.translationService.getValue('buyers-and-sales-centers.edit-buyers-and-sales-centers-babh-license-dialog-title');
            }
        }
        else {
            data = new CommonDocumentDialogParams({
                model: undefined,
                viewMode: false
            });

            title = this.translationService.getValue('buyers-and-sales-centers.add-buyers-and-sales-centers-babh-license-dialog-title');
        }

        const dialog = this.editBabhLicenseDialog.openWithTwoButtons({
            title: title,
            TCtor: CommonDocumentComponent,
            headerAuditButton: auditBtn,
            headerCancelButton: {
                cancelBtnClicked: this.closeEditBabhLicenseDialogBtnClicked.bind(this)
            },
            componentData: data,
            translteService: this.translationService,
            viewMode: viewMode
        }, '1350px');

        dialog.subscribe({
            next: (result: CommonDocumentDTO | undefined) => {
                if (result !== undefined) {
                    if (license !== undefined) { // edit
                        const idx: number = this.babhLicenses.findIndex(x => x === license);
                        this.babhLicenses[idx] = result;
                    }
                    else { // add
                        this.babhLicenses.push(result);
                    }

                    this.babhLicensesTouched = true;
                    this.editForm.updateValueAndValidity({ emitEvent: false });

                    setTimeout(() => {
                        this.babhLicenses = this.babhLicenses.slice();
                    })
                }
            }
        });
    }

    public deleteBabhLicense(license: GridRow<CommonDocumentDTO>): void {
        this.confirmDialog.open({
            title: this.translationService.getValue('buyers-and-sales-centers.delete-babh-license-dialog-title'),
            message: this.translationService.getValue('buyers-and-sales-centers.delete-babh-license-dialog-message'),
            okBtnLabel: this.translationService.getValue('buyers-and-sales-centers.delete-babh-license-dialog-ok-btn-label')
        }).subscribe({
            next: (ok: boolean) => {
                if (ok) {
                    this.babhLicensesTable.softDelete(license);

                    this.babhLicenses = this.babhLicensesTable.rows;

                    this.babhLicensesTouched = true;
                    this.editForm.updateValueAndValidity({ emitEvent: false });
                }
            }
        });
    }

    public undoDeleteBabhLicense(license: GridRow<CommonDocumentDTO>): void {
        this.confirmDialog.open().subscribe({
            next: (ok: boolean) => {
                if (ok) {
                    this.babhLicensesTable.softUndoDelete(license);

                    this.babhLicensesTouched = true;
                    this.editForm.updateValueAndValidity({ emitEvent: false });
                }
            }
        });
    }

    public addEditVeterinaryVehicleLicense(license: CommonDocumentDTO | undefined, viewMode: boolean = false): void {
        let data: CommonDocumentDialogParams | undefined;
        let auditBtn: IHeaderAuditButton | undefined;
        let title: string;

        if (license !== undefined) {
            data = new CommonDocumentDialogParams({
                model: license,
                viewMode: this.isReadonly || viewMode
            });

            if (license.id !== undefined && !IS_PUBLIC_APP) {
                auditBtn = {
                    id: license.id,
                    getAuditRecordData: this.service.getBuyerLicensesAudit.bind(this.service),
                    tableName: 'BuyerLicense'
                };
            }

            if (this.isReadonly || viewMode) {
                title = this.translationService.getValue('buyers-and-sales-centers.view-buyers-and-sales-centers-veterinary-vehicle-license-dialog-title');
            }
            else {
                title = this.translationService.getValue('buyers-and-sales-centers.edit-buyers-and-sales-centers-veterinary-vehicle-license-dialog-title');
            }
        }
        else {
            data = new CommonDocumentDialogParams({
                model: undefined,
                viewMode: false
            });

            title = this.translationService.getValue('buyers-and-sales-centers.add-buyers-and-sales-centers-veterinary-vehicle-license-dialog-title');
        }

        const dialog = this.editVeterinaryVehicleLicenseDialog.openWithTwoButtons({
            title: title,
            TCtor: CommonDocumentComponent,
            headerAuditButton: auditBtn,
            headerCancelButton: {
                cancelBtnClicked: this.closeEditVeterinaryVehicleLicenseDialogBtnClicked.bind(this)
            },
            componentData: data,
            translteService: this.translationService,
            viewMode: viewMode
        }, '1350px');

        dialog.subscribe({
            next: (result: CommonDocumentDTO | undefined) => {
                if (result !== undefined) {
                    if (license !== undefined) { // edit
                        const idx: number = this.veterinaryVehicleLicenses.findIndex(x => x === license);
                        this.veterinaryVehicleLicenses[idx] = result;
                    }
                    else { // add
                        this.veterinaryVehicleLicenses.push(result);
                    }

                    this.veterinaryVehicleLicensesTouched = true;
                    this.editForm.updateValueAndValidity({ emitEvent: false });

                    setTimeout(() => {
                        this.veterinaryVehicleLicenses = this.veterinaryVehicleLicenses.slice();
                    });
                }
            }
        });
    }

    public deleteVeterinaryVehicleLicense(license: GridRow<CommonDocumentDTO>): void {
        this.confirmDialog.open({
            title: this.translationService.getValue('buyers-and-sales-centers.delete-veterinary-vehicle-license-dialog-title'),
            message: this.translationService.getValue('buyers-and-sales-centers.delete-veterinary-vehicle-license-dialog-message'),
            okBtnLabel: this.translationService.getValue('buyers-and-sales-centers.delete-veterinary-vehicle-license-dialog-ok-btn-label')
        }).subscribe({
            next: (ok: boolean) => {
                if (ok) {
                    this.veterinaryVehicleLicensesTable.softDelete(license);

                    this.veterinaryVehicleLicensesTouched = true;
                    this.editForm.updateValueAndValidity({ emitEvent: false });
                }
            }
        });
    }

    public undoDeleteVeterinaryVehicleLicense(license: GridRow<CommonDocumentDTO>): void {
        this.confirmDialog.open().subscribe({
            next: (ok: boolean) => {
                if (ok) {
                    this.veterinaryVehicleLicensesTable.softUndoDelete(license);

                    this.veterinaryVehicleLicensesTouched = true;
                    this.editForm.updateValueAndValidity({ emitEvent: false });
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

    private markAllAsTouched(): void {
        this.editForm.markAllAsTouched();
        this.premiseUsageDocumentsTouched = true;
        this.babhLicensesTouched = true;
        this.veterinaryVehicleLicensesTouched = true;
    }

    private closeEditBabhLicenseDialogBtnClicked(closeFn: HeaderCloseFunction): void {
        closeFn();
    }

    private closeEditVeterinaryVehicleLicenseDialogBtnClicked(closeFn: HeaderCloseFunction): void {
        closeFn();
    }

    private openCancelDialog(action: IActionInfo, dialogClose: DialogCloseCallback, cancelling: boolean): void {
        this.openConfirmDialogForApplication(() => {
            if (this.model instanceof BuyerEditDTO) {
                const title: string = cancelling
                    ? this.translationService.getValue('buyers-and-sales-centers.cancel-buyers-and-sales-center-facility')
                    : this.translationService.getValue('buyers-and-sales-centers.activate-buyers-and-sales-center-facility');

                const dialog = this.cancelDialog.openWithTwoButtons({
                    title: title,
                    TCtor: CancellationHistoryDialogComponent,
                    headerCancelButton: {
                        cancelBtnClicked: this.closeCancelDialogBtnClicked.bind(this)
                    },
                    translteService: this.translationService,
                    componentData: new CancellationHistoryDialogParams({
                        group: this.model.buyerStatus !== BuyerStatusesEnum.Canceled
                            ? CancellationReasonGroupEnum.BuyerCancel
                            : CancellationReasonGroupEnum.BuyerActivate,
                        cancelling: this.model.buyerStatus !== BuyerStatusesEnum.Canceled,
                        statuses: this.model.cancellationHistory
                    })
                }, '1200px');

                dialog.subscribe((result: CancellationHistoryEntryDTO | undefined) => {
                    if (result !== undefined && result !== null) {
                        if (this.isChangeOfCircumstancesApplication === true) {
                            this.service.updateBuyerStatus(this.model.id!, result).subscribe({
                                next: () => {
                                    this.completeChangeOfCircumstancesApplication(dialogClose);
                                }
                            });
                        }
                        else if (this.isDeregistrationApplication) {
                            this.service.updateBuyerStatus(this.model.id!, result, this.model.applicationId!).subscribe({
                                next: () => {
                                    dialogClose(this.model);
                                }
                            });
                        }
                        else {
                            this.service.updateBuyerStatus(this.model.id!, result).subscribe({
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
        this.service.completeBuyerChangeOfCircumstancesApplication(this.model, this.ignoreLogBookConflicts).subscribe({
            next: () => {
                dialogClose(this.model);
            },
            error: (errorResponse: HttpErrorResponse) => {
                this.handleAddApplicationErrorResponse(errorResponse, dialogClose, 'completeChangeOfCircumstancesApplication');
            }
        });
    }

    private closeCancelDialogBtnClicked(closeFn: HeaderCloseFunction): void {
        closeFn();
    }

    private save(dialogClose: DialogCloseCallback): void {
        if (this.isChangeOfCircumstancesApplication === true || this.isDeregistrationApplication === true) {
            this.confirmDialog.open({
                title: this.translationService.getValue('buyers-and-sales-centers.complete-coc-application-confirm-dialog-title'),
                message: this.translationService.getValue('buyers-and-sales-centers.complete-coc-application-confirm-dialog-message'),
                okBtnLabel: this.translationService.getValue('buyers-and-sales-centers.complete-coc-application-confirm-dialog-ok-btn-label')
            }).subscribe({
                next: (ok: boolean) => {
                    if (ok === true) {
                        this.saveData(dialogClose);
                    }
                }
            });
        }
        else {
            this.saveData(dialogClose);
        }
    }

    private openConfirmDialogForApplication(callback: (...args: unknown[]) => unknown): void {
        if (this.isChangeOfCircumstancesApplication === true || this.isDeregistrationApplication === true) {
            this.confirmDialog.open({
                title: this.translationService.getValue('buyers-and-sales-centers.complete-coc-application-confirm-dialog-title'),
                message: this.translationService.getValue('buyers-and-sales-centers.complete-coc-application-confirm-dialog-message'),
                okBtnLabel: this.translationService.getValue('buyers-and-sales-centers.complete-coc-application-confirm-dialog-ok-btn-label')
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

    private saveAndPrintRecord(dialogClose: DialogCloseCallback): void {
        this.fillModel();
        CommonUtils.sanitizeModelStrings(this.model);
        let saveOrEditObservable: Observable<boolean>;

        if (this.id !== null && this.id !== undefined) {
            saveOrEditObservable = this.service.editAndDownloadRegister(this.model, this.ignoreLogBookConflicts);
        }
        else {
            saveOrEditObservable = this.service.addAndDownloadRegister(this.model, this.ignoreLogBookConflicts);
        }

        saveOrEditObservable.subscribe({
            next: (downloaded: boolean) => {
                if (downloaded === true) {
                    dialogClose(this.model);
                }
            },
            error: (errorResponse: HttpErrorResponse) => {
                this.handleAddApplicationErrorResponse(errorResponse, dialogClose, 'saveAndPrint');
            }
        });
    }

    private saveData(dialogClose: DialogCloseCallback, fromSaveAsDraft: boolean = false): Observable<boolean> {
        const saveOrEditDone: EventEmitter<boolean> = new EventEmitter<boolean>();

        this.saveOrEdit(fromSaveAsDraft).subscribe({
            next: (id: number | void) => {
                this.hasNoEDeliveryRegistrationError = false;

                if (typeof id === 'number' && id !== undefined) {
                    this.model.id = id;
                    dialogClose(this.model);
                }
                else {
                    dialogClose(this.model);
                }

                saveOrEditDone.emit(true);
                saveOrEditDone.complete();
            },
            error: (errorResponse: HttpErrorResponse) => {
                this.handleAddApplicationErrorResponse(errorResponse, dialogClose, 'save');
            }
        });

        return saveOrEditDone.asObservable();
    }

    private handleAddApplicationErrorResponse(errorResponse: HttpErrorResponse, dialogClose: DialogCloseCallback, saveMethod: SaveMethod): void {
        const messages: string[] = errorResponse.error.messages;
        if (Array.isArray(messages) === true) {
            for (const message of messages) {
                if (message === ApplicationValidationErrorsEnum.NoEDeliveryRegistration.toString()) {
                    this.hasNoEDeliveryRegistrationError = true;
                    this.validityCheckerGroup.validate();
                }
            }
        }

        if (this.model instanceof BuyerEditDTO) {
            const error = errorResponse.error as ErrorModel;

            if (error?.code === ErrorCode.InvalidLogBookPagesRange) {
                this.handleInvalidLogBookPagesRangeError(error.messages[0], dialogClose, saveMethod);
            }
        }
    }

    private handleInvalidLogBookPagesRangeError(logBookNumber: string, dialogClose: DialogCloseCallback, saveMethod: SaveMethod): void {
        if (this.model instanceof BuyerEditDTO) {
            this.ignoreLogBookConflicts = false;
            const logBooks: LogBookEditDTO[] = this.editForm.get('logBooksControl')!.value ?? [];
            const logBook: LogBookEditDTO | undefined = logBooks.find(x => x.logbookNumber === logBookNumber);

            if (logBook !== null && logBook !== undefined) {
                logBook.hasError = true;
                setTimeout(() => {
                    (this.model as BuyerEditDTO).logBooks = (this.model as BuyerEditDTO).logBooks?.slice();
                    this.editForm.get('logBooksControl')!.setValue((this.model as BuyerEditDTO).logBooks);
                    this.validityCheckerGroup.validate();
                });
            }

            const ranges: OverlappingLogBooksParameters[] = [];

            for (const logBook of logBooks.filter(x => x.logBookIsActive)) {
                const range: OverlappingLogBooksParameters = new OverlappingLogBooksParameters({
                    logBookId: logBook.logBookId,
                    typeId: logBook.logBookTypeId,
                    OwnerType: logBook.ownerType,
                    startPage: logBook.startPageNumber,
                    endPage: logBook.endPageNumber
                });
                ranges.push(range);
            }

            const editDialogData: OverlappingLogBooksDialogParamsModel = new OverlappingLogBooksDialogParamsModel({
                service: this.commercialFishingService,
                logBookGroup: this.logBookGroup,
                ranges: ranges
            });

            this.overlappingLogBooksDialog.open({
                title: this.translationService.getValue('buyers-and-sales-centers.overlapping-log-books-dialog-title'),
                TCtor: OverlappingLogBooksComponent,
                headerCancelButton: {
                    cancelBtnClicked: this.closeOverlappingLogBooksDialogBtnClicked.bind(this)
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
                    translateValue: 'buyers-and-sales-centers.overlapping-log-books-save-despite-conflicts'
                }
            }, '1300px').subscribe({
                next: (save: boolean | undefined) => {
                    if (save) {
                        this.ignoreLogBookConflicts = true;
                        switch (saveMethod) {
                            case 'save': this.saveData(dialogClose); break;
                            case 'saveAndPrint': this.saveAndPrintRecord(dialogClose); break;
                            case 'completeChangeOfCircumstancesApplication': this.completeChangeOfCircumstancesApplication(dialogClose); break;
                        }
                    }
                }
            });
        }
    }

    private saveOrEdit(fromSaveAsDraft: boolean = false): Observable<number | void> {
        this.fillModel();
        CommonUtils.sanitizeModelStrings(this.model);
        let saveOrEditObservable: Observable<void | number>;

        if (this.isChangeOfCircumstancesApplication === true) {
            saveOrEditObservable = this.service.completeBuyerChangeOfCircumstancesApplication(this.model, this.ignoreLogBookConflicts);
        }
        else if (this.model instanceof BuyerEditDTO) {
            if (this.id !== undefined && this.id !== null) {
                saveOrEditObservable = this.service.edit(this.model, this.ignoreLogBookConflicts);
            }
            else {
                saveOrEditObservable = this.service.add(this.model, this.ignoreLogBookConflicts);
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

    private buildForm(): void {
        this.editForm = new FormGroup({
            submittedByControl: new FormControl(),
            submittedForControl: new FormControl()
        }, [
            this.atLeastOneActivePremiseBabhDocumentValidator(),
            this.atLeastOneActiveVehicleBabhDocumentValidator(),
            this.atLeastOneActivePremiseUsageDocumentValidator()
        ]);

        if (this.isFirstSaleBuyer) {
            if (!this.showOnlyRegiXData) {
                this.editForm.addControl('agentSameAsControl', new FormControl(undefined, Validators.required));
            }

            this.editForm.addControl('agentControl', new FormControl(undefined, Validators.required));

        }
        else if (this.isFirstSaleCenter) {
            if (!this.showOnlyRegiXData) {
                this.editForm.addControl('organizerSameAsSubmittedByControl', new FormControl());
            }

            this.editForm.addControl('organizerControl', new FormControl(undefined, Validators.required));
            this.editForm.addControl('premiseUsageDocumentControl', new FormControl());
        }

        if (!this.isApplication) {
            this.editForm.addControl('urorrNumberControl', new FormControl());
            this.editForm.addControl('registrationNumberControl', new FormControl());
            this.editForm.addControl('territoryUnitControl', new FormControl(undefined, Validators.required));
            this.editForm.addControl('registrationDateControl', new FormControl(undefined, Validators.required));
            this.editForm.addControl('buyerStatusControl', new FormControl(undefined, Validators.required));

            this.editForm.addControl('logBooksControl', new FormControl(undefined));
        }

        if (!this.showOnlyRegiXData) {
            this.editForm.addControl('premiseNameControl', new FormControl());
            this.editForm.addControl('premiseAddressControl', new FormControl());

            this.editForm.addControl('hasPremiseOrVehicleGroup', new FormGroup({
                hasUtilityControl: new FormControl(false),
                hasVehicleControl: new FormControl(false)
            }, EditBuyersComponent.atLeastOneRegistrationValidator));

            this.editForm.addControl('vehicleNumberControl', new FormControl(undefined));

            this.editForm.addControl('babhLawLicenseControl', new FormControl());
            this.editForm.addControl('veteniraryVehicleRegLicenseControl', new FormControl());
            this.editForm.addControl('annualTurnoverControl', new FormControl());

            this.editForm.addControl('filesControl', new FormControl());
        }

        this.editForm.addControl('deliveryDataControl', new FormControl());
    }

    private fillForm(): void {
        if (this.model instanceof BuyerEditDTO) {
            this.fillFormRegisterFields();
        }
        else if (this.model instanceof BuyerApplicationEditDTO) {
            this.FillFormApplicationFields();

            if (this.showRegiXData) {
                this.fillFormRegiX();
            }
        }
        else if (this.model instanceof BuyerRegixDataDTO) {
            this.fillFormRegixDataFields();
        }
    }

    private fillFormRegisterFields(): void {
        if (this.model instanceof BuyerEditDTO) {
            this.editForm.get('urorrNumberControl')!.setValue(this.model.urorrNumber);
            this.editForm.get('registrationNumberControl')!.setValue(this.model.registrationNumber);

            if (this.model.territoryUnitId !== null && this.model.territoryUnitId !== undefined) {
                const territoryUnitId: number = this.model.territoryUnitId;
                const territoryUnit: NomenclatureDTO<number> = this.territoryUnits.find(x => x.value === territoryUnitId)!;
                this.editForm.get('territoryUnitControl')!.setValue(territoryUnit);
            }

            this.editForm.get('registrationDateControl')!.setValue(this.model.registrationDate);

            if (this.model.buyerStatus === BuyerStatusesEnum.Canceled) {
                this.editForm.get('buyerStatusControl')!.disable();
                this.buyerStatuses = this.allBuyerStatuses.slice();
            }

            const buyerCode: string = BuyerStatusesEnum[this.model.buyerStatus!];
            this.editForm.get('buyerStatusControl')!.setValue(this.allBuyerStatuses.find(x => x.code === buyerCode));

            const buyerStatus: BuyerStatusesEnum | undefined = this.model.buyerStatus;
            if (buyerStatus !== undefined && buyerStatus !== null) {
                this.editForm.get('buyerStatusControl')!.setValue(this.buyerStatuses.find(x => x.code === BuyerStatusesEnum[buyerStatus]));
            }

            this.editForm.get('submittedForControl')!.setValue(this.model.submittedFor);

            (this.editForm.get('hasPremiseOrVehicleGroup') as FormGroup).get('hasUtilityControl')!.setValue(this.model.hasUtility);
            if (this.model.hasUtility === true) {
                (this.editForm.get('hasPremiseOrVehicleGroup') as FormGroup).get('hasUtilityControl')!.disable({ emitEvent: false });
            }

            (this.editForm.get('hasPremiseOrVehicleGroup') as FormGroup).get('hasVehicleControl')!.setValue(this.model.hasVehicle);
            if (this.model.hasVehicle === true) {
                (this.editForm.get('hasPremiseOrVehicleGroup') as FormGroup).get('hasVehicleControl')!.disable({ emitEvent: false });
            }

            if (this.model.hasUtility === true) {
                this.editForm.get('premiseNameControl')!.setValue(this.model.utilityName);
                this.editForm.get('premiseAddressControl')!.setValue(this.model.premiseAddress);
                this.babhLicenses = this.model.babhLawLicenseDocuments ?? [];
            }

            if (this.model.hasVehicle === true) {
                this.editForm.get('vehicleNumberControl')!.setValue(this.model.vehicleNumber);
                this.veterinaryVehicleLicenses = this.model.veteniraryVehicleRegLicenseDocuments ?? [];
            }

            if (this.viewMode || this.isReadonly) {
                if (this.model.hasUtility === true) {
                    this.editForm.get('premiseNameControl')!.disable();
                }

                if (this.model.hasVehicle === true) {
                    this.editForm.get('vehicleNumberControl')!.disable();
                }
            }

            if (this.isFirstSaleCenter) {
                this.editForm.get('organizerSameAsSubmittedByControl')!.setValue(this.model.organizerSameAsSubmittedBy);
                this.editForm.get('organizerControl')!.setValue(this.model.organizer);
                this.fillFormPremiseUsageDocuments(); // TODO check here if hasUtility is TRUE first ???
            }
            else if (this.isFirstSaleBuyer) {
                this.fillFormAgent();
            }

            this.editForm.get('annualTurnoverControl')!.setValue(this.model.annualTurnover);
            this.editForm.get('logBooksControl')!.setValue(this.model.logBooks);
            this.editForm.get('filesControl')!.setValue(this.model.files);

            this.cancellationHistory = this.model.cancellationHistory ?? [];
            this.duplicates = this.model.duplicateEntries ?? [];

        }
    }

    private FillFormApplicationFields(): void {
        if (this.model instanceof BuyerApplicationEditDTO) {
            this.editForm.get('submittedByControl')!.setValue(this.model.submittedBy);
            this.editForm.get('submittedForControl')!.setValue(this.model.submittedFor);

            (this.editForm.get('hasPremiseOrVehicleGroup') as FormGroup).get('hasUtilityControl')!.setValue(this.model.hasUtility);
            (this.editForm.get('hasPremiseOrVehicleGroup') as FormGroup).get('hasVehicleControl')!.setValue(this.model.hasVehicle);

            if (this.model.hasUtility === true) {
                this.editForm.get('premiseNameControl')!.setValue(this.model.utilityName);
                this.editForm.get('premiseAddressControl')!.setValue(this.model.premiseAddress);
                this.editForm.get('babhLawLicenseControl')!.setValue(this.model.babhLawLicenseDocumnet);
            }

            if (this.model.hasVehicle === true) {
                this.editForm.get('vehicleNumberControl')!.setValue(this.model.vehicleNumber);
                this.editForm.get('veteniraryVehicleRegLicenseControl')!.setValue(this.model.veteniraryVehicleRegLicenseDocument);
            }

            if (this.isFirstSaleCenter) {
                this.editForm.get('organizerSameAsSubmittedByControl')!.setValue(this.model.organizerSameAsSubmittedBy);
                this.editForm.get('organizerControl')!.setValue(this.model.organizer);
                if ((this.editForm.get('hasPremiseOrVehicleGroup') as FormGroup).get('hasUtilityControl')!.value) {
                    this.fillFormPremiseUsageDocuments();
                }
            }
            else if (this.isFirstSaleBuyer) {
                this.fillFormAgent();
            }

            this.editForm.get('annualTurnoverControl')!.setValue(this.model.annualTurnover);
            this.editForm.get('deliveryDataControl')!.setValue(this.model.deliveryData);
            this.editForm.get('filesControl')!.setValue(this.model.files);
        }
    }

    private fillFormRegixDataFields(): void {
        if (this.model instanceof BuyerRegixDataDTO && this.showOnlyRegiXData) {
            this.editForm.get('submittedByControl')!.setValue(this.model.submittedBy);
            this.editForm.get('submittedForControl')!.setValue(this.model.submittedFor);

            if (this.isFirstSaleCenter) {
                this.editForm.get('organizerControl')!.setValue(this.model.organizer);
                // TODO check here if hasUtility is TRUE first ???
                this.fillFormPremiseUsageDocuments();
            }
            else if (this.isFirstSaleBuyer) {
                this.fillFormAgent();
            }

            this.fillFormRegiX();
        }
    }

    private fillFormRegiX(): void {
        if (this.model instanceof BuyerApplicationEditDTO || this.model instanceof BuyerRegixDataDTO) {
            if (this.model.applicationRegiXChecks !== undefined && this.model.applicationRegiXChecks !== null) {
                const applicationRegiXChecks: ApplicationRegiXCheckDTO[] = this.model.applicationRegiXChecks;

                setTimeout(() => {
                    this.regixChecks = applicationRegiXChecks;
                });
            }
        }

        if (!this.viewMode) {
            this.notifier.start();
            this.notifier.onNotify.subscribe({
                next: () => {
                    this.editForm.markAllAsTouched();

                    if (this.showOnlyRegiXData) {
                        ApplicationUtils.enableOrDisableRegixCheckButtons(this.editForm, this.dialogRightSideActions);
                    }

                    this.notifier.stop();
                }
            });
        }
    }

    private fillFormAgent(): void {
        if (this.isFirstSaleBuyer) {
            this.editForm.get('agentControl')!.setValue(this.model.agent);

            if (this.model instanceof BuyerEditDTO || this.model instanceof BuyerApplicationEditDTO) {
                let agentSameAsValue: NomenclatureDTO<AgentSameAsTypesEnum> | undefined = undefined;

                if (this.model.isAgentSameAsSubmittedBy === true) {
                    agentSameAsValue = this.agentSameAsOptions.find(x => x.value === AgentSameAsTypesEnum.SubmittedByPerson)!;
                }
                else if (this.model.isAgentSameAsSubmittedForCustodianOfProperty === true) {
                    agentSameAsValue = this.agentSameAsOptions.find(x => x.value === AgentSameAsTypesEnum.SubmittedForCustodianOfPropertyPerson)!;
                }
                else {
                    agentSameAsValue = this.agentSameAsOptions.find(x => x.value === AgentSameAsTypesEnum.Other)!;
                }

                setTimeout(() => {
                    this.editForm.get('agentSameAsControl')!.setValue(agentSameAsValue);
                });
            }
        }
    }

    private fillFormPremiseUsageDocuments(): void {
        if (this.isFirstSaleCenter) {
            if (this.model instanceof BuyerApplicationEditDTO || this.model instanceof BuyerRegixDataDTO) {
                this.editForm.get('premiseUsageDocumentControl')!.setValue(this.model.premiseUsageDocument);
            }
            else if (this.model instanceof BuyerEditDTO) {
                this.premiseUsageDocuments = (this.model as BuyerEditDTO).premiseUsageDocuments?.slice() ?? [];
            }
        }
    }

    private fillModel(): void {
        if (this.model instanceof BuyerEditDTO) {
            this.fillModelRegisterFields();
        }
        else if (this.model instanceof BuyerApplicationEditDTO) {
            this.fillModelApplicationFields();
        }
        else if (this.model instanceof BuyerRegixDataDTO) {
            this.fillModelRegixDataFields();
        }
    }

    private fillModelRegisterFields(): void {
        if (this.model instanceof BuyerEditDTO) {
            this.model.territoryUnitId = this.editForm.get('territoryUnitControl')!.value?.value;
            this.model.registrationDate = this.editForm.get('registrationDateControl')!.value;
            this.model.submittedFor = this.editForm.get('submittedForControl')!.value;
            this.model.buyerStatus = BuyerStatusesEnum[this.editForm.get('buyerStatusControl')!.value!.code as keyof typeof BuyerStatusesEnum];

            this.model.hasUtility = (this.editForm.get('hasPremiseOrVehicleGroup') as FormGroup).get('hasUtilityControl')!.value ?? false;
            if (this.model.hasUtility === true) {
                this.model.utilityName = this.editForm.get('premiseNameControl')!.value;
                this.model.premiseAddress = this.editForm.get('premiseAddressControl')!.value;
                this.model.babhLawLicenseDocuments = this.babhLicenses;
            }
            else {
                this.model.utilityName = undefined;
                this.model.premiseAddress = undefined;
                this.model.babhLawLicenseDocuments = undefined;
            }

            this.model.hasVehicle = (this.editForm.get('hasPremiseOrVehicleGroup') as FormGroup).get('hasVehicleControl')!.value ?? false;
            if (this.model.hasVehicle === true) {
                this.model.vehicleNumber = this.editForm.get('vehicleNumberControl')!.value;
                this.model.veteniraryVehicleRegLicenseDocuments = this.veterinaryVehicleLicenses;
            }
            else {
                this.model.vehicleNumber = undefined;
                this.model.veteniraryVehicleRegLicenseDocuments = undefined;
            }

            if (this.isFirstSaleBuyer) {
                this.fillModelAgentFields();
            }
            else if (this.isFirstSaleCenter) {
                this.model.organizer = this.editForm.get('organizerControl')!.value;
                this.model.organizerSameAsSubmittedBy = this.editForm.get('organizerSameAsSubmittedByControl')!.value;
                this.fillModelPremiseUsageDocuments();
            }

            this.model.annualTurnover = this.editForm.get('annualTurnoverControl')!.value;
            this.model.babhLawLicenseDocuments = this.babhLicenses;
            this.model.logBooks = this.editForm.get('logBooksControl')!.value;
            this.model.territoryUnitId = this.editForm.get('territoryUnitControl')!.value.value;
            this.model.files = this.editForm.get('filesControl')!.value;
        }
    }

    private fillModelApplicationFields(): void {
        if (this.model instanceof BuyerApplicationEditDTO) {
            this.model.submittedBy = this.editForm.get('submittedByControl')!.value;
            this.model.submittedFor = this.editForm.get('submittedForControl')!.value;

            this.model.hasUtility = (this.editForm.get('hasPremiseOrVehicleGroup') as FormGroup).get('hasUtilityControl')!.value ?? false;
            if (this.model.hasUtility === true) {
                this.model.utilityName = this.editForm.get('premiseNameControl')!.value;
                this.model.premiseAddress = this.editForm.get('premiseAddressControl')!.value;
                this.model.babhLawLicenseDocumnet = this.editForm.get('babhLawLicenseControl')!.value;
            }
            else {
                this.model.utilityName = undefined;
                this.model.premiseAddress = undefined;
                this.model.babhLawLicenseDocumnet = undefined;
            }

            this.model.hasVehicle = (this.editForm.get('hasPremiseOrVehicleGroup') as FormGroup).get('hasVehicleControl')!.value ?? false;
            if (this.model.hasVehicle === true) {
                this.model.vehicleNumber = this.editForm.get('vehicleNumberControl')!.value;
                this.model.veteniraryVehicleRegLicenseDocument = this.editForm.get('veteniraryVehicleRegLicenseControl')!.value;
            }
            else {
                this.model.vehicleNumber = undefined;
                this.model.veteniraryVehicleRegLicenseDocument = undefined;
            }

            if (this.isFirstSaleBuyer) {
                this.fillModelAgentFields();
            }
            else if (this.isFirstSaleCenter) {
                this.model.organizer = this.editForm.get('organizerControl')!.value;
                this.model.organizerSameAsSubmittedBy = this.editForm.get('organizerSameAsSubmittedByControl')!.value;
                this.fillModelPremiseUsageDocuments();
            }

            this.model.annualTurnover = this.editForm.get('annualTurnoverControl')!.value;
            this.model.deliveryData = this.editForm.get('deliveryDataControl')!.value;
            this.model.files = this.editForm.get('filesControl')!.value;
        }
    }

    private fillModelRegixDataFields(): void {
        if (this.model instanceof BuyerRegixDataDTO) {
            this.model.submittedBy = this.editForm.get('submittedByControl')!.value;
            this.model.submittedFor = this.editForm.get('submittedForControl')!.value;

            if (this.isFirstSaleBuyer) {
                this.fillModelAgentFields();
            }
            else if (this.isFirstSaleCenter) {
                this.model.organizer = this.editForm.get('organizerControl')!.value;
                this.fillModelPremiseUsageDocuments();
            }
        }
    }

    private fillModelAgentFields(): void {
        if (this.isFirstSaleBuyer) {
            if (this.model instanceof BuyerEditDTO || this.model instanceof BuyerApplicationEditDTO) {
                const agentSameAsType: AgentSameAsTypesEnum | undefined = this.editForm.get('agentSameAsControl')!.value?.value;
                if (agentSameAsType !== undefined && agentSameAsType !== null) {
                    switch (agentSameAsType) {
                        case AgentSameAsTypesEnum.SubmittedByPerson: {
                            this.model.isAgentSameAsSubmittedBy = true;
                            this.model.isAgentSameAsSubmittedForCustodianOfProperty = false;
                        } break;
                        case AgentSameAsTypesEnum.SubmittedForCustodianOfPropertyPerson: {
                            this.model.isAgentSameAsSubmittedBy = false;
                            this.model.isAgentSameAsSubmittedForCustodianOfProperty = true;
                        } break;
                        default: {
                            this.model.isAgentSameAsSubmittedBy = false;
                            this.model.isAgentSameAsSubmittedForCustodianOfProperty = false;
                        }
                    }
                }
                else {
                    this.model.isAgentSameAsSubmittedBy = false;
                    this.model.isAgentSameAsSubmittedForCustodianOfProperty = false;
                }
            }

            this.model.agent = this.editForm.get('agentControl')!.value;
        }
    }

    private fillModelPremiseUsageDocuments(): void {
        if (this.isFirstSaleCenter) {
            if (this.model instanceof BuyerApplicationEditDTO || this.model instanceof BuyerRegixDataDTO) {
                this.model.premiseUsageDocument = this.editForm.get('premiseUsageDocumentControl')!.value;
            }
            else if (this.model instanceof BuyerEditDTO) {
                this.model.premiseUsageDocuments = this.premiseUsageDocuments;
            }
        }
    }

    private mapSubmittedForCustodianOfPropertyToAgent(value: ApplicationSubmittedForRegixDataDTO | undefined): void {
        const custodianOfProperty: CustodianOfPropertyDTO | undefined = value?.legal?.custodianOfProperty;

        if (custodianOfProperty !== null && custodianOfProperty !== undefined) {
            const agent: RegixPersonDataDTO | undefined = this.editForm.get('agentControl')!.value;

            if (agent === null || agent === undefined) {
                this.editForm.get('agentControl')!.setValue(new RegixPersonDataDTO({
                    egnLnc: custodianOfProperty.egnLnc,
                    firstName: custodianOfProperty.firstName,
                    middleName: custodianOfProperty.middleName,
                    lastName: custodianOfProperty.lastName
                }));
            }
            else {
                agent.egnLnc = custodianOfProperty.egnLnc;
                agent.firstName = custodianOfProperty.firstName;
                agent.middleName = custodianOfProperty.middleName;
                agent.lastName = custodianOfProperty.lastName;

                this.editForm.get('agentControl')!.setValue(agent);
            }

        }
        else {
            const agent: RegixPersonDataDTO | undefined = this.editForm.get('agentControl')!.value;

            if (agent !== null && agent !== undefined) {
                agent.egnLnc = undefined;
                agent.firstName = undefined;
                agent.middleName = undefined;
                agent.lastName = undefined;

                this.editForm.get('agentControl')!.setValue(agent);
            }
        }
    }

    public static atLeastOneRegistrationValidator: ValidatorFn = (control: AbstractControl): ValidationErrors | null => {
        if (!control) {
            return null;
        }

        const hasUtilityControl = control.get('hasUtilityControl');
        const hasVehicleControl = control.get('hasVehicleControl');

        if (!hasUtilityControl || !hasVehicleControl) {
            return null;
        }

        if (hasUtilityControl.value === true && !hasVehicleControl.value) {
            return null;
        }

        if (!hasUtilityControl.value && hasVehicleControl.value === true) {
            return null;
        }

        if (hasUtilityControl.value === true && hasVehicleControl.valid === true) {
            return null;
        }

        return { atLeastOneRegistration: true };
    }

    private atLeastOneActivePremiseBabhDocumentValidator(): ValidatorFn {
        return (control: AbstractControl): ValidationErrors | null => {
            if (control === null || control === undefined) {
                return null;
            }

            if (this.isApplication) {
                return null;
            }

            const hasPremiseOrVehicleGroup: FormGroup | undefined = control.get('hasPremiseOrVehicleGroup') as FormGroup;

            if (hasPremiseOrVehicleGroup === null || hasPremiseOrVehicleGroup === undefined) {
                return null;
            }

            const hasUtilityControl = (control.get('hasPremiseOrVehicleGroup') as FormGroup).get('hasUtilityControl');

            if (hasUtilityControl === null || hasUtilityControl === undefined) {
                return null;
            }

            if (hasUtilityControl.value) {
                if (this.babhLicenses === null || this.babhLicenses === undefined) {
                    return { 'noPremiseBabhLicenses': true };
                }

                if (this.babhLicenses.length === 0) {
                    return { 'noPremiseBabhLicenses': true };
                }

                if (this.babhLicenses.filter(x => x.isActive).length === 0) {
                    return { 'noPremiseBabhLicenses': true };
                }
            }

            return null;
        }
    }

    private atLeastOneActiveVehicleBabhDocumentValidator(): ValidatorFn {
        return (control: AbstractControl): ValidationErrors | null => {
            if (control === null || control === undefined) {
                return null;
            }

            if (this.isApplication) {
                return null;
            }

            const hasPremiseOrVehicleGroup: FormGroup | undefined = control.get('hasPremiseOrVehicleGroup') as FormGroup;

            if (hasPremiseOrVehicleGroup === null || hasPremiseOrVehicleGroup === undefined) {
                return null;
            }

            const hasVehicleControl = (control.get('hasPremiseOrVehicleGroup') as FormGroup).get('hasVehicleControl');

            if (hasVehicleControl === null || hasVehicleControl === undefined) {
                return null;
            }

            if (hasVehicleControl.value) {
                if (this.veterinaryVehicleLicenses === null || this.veterinaryVehicleLicenses === undefined) {
                    return { 'noVehicleBabhLicenses': true };
                }

                if (this.veterinaryVehicleLicenses.length === 0) {
                    return { 'noVehicleBabhLicenses': true };
                }

                if (this.veterinaryVehicleLicenses.filter(x => x.isActive).length === 0) {
                    return { 'noVehicleBabhLicenses': true };
                }
            }

            return null;
        }
    }

    private atLeastOneActivePremiseUsageDocumentValidator(): ValidatorFn {
        return (control: AbstractControl): ValidationErrors | null => {
            if (control === null || control === undefined) {
                return null;
            }

            if (this.isApplication || this.isFirstSaleBuyer) {
                return null;
            }

            const hasPremiseOrVehicleGroup: FormGroup | undefined = control.get('hasPremiseOrVehicleGroup') as FormGroup;

            if (hasPremiseOrVehicleGroup === null || hasPremiseOrVehicleGroup === undefined) {
                return null;
            }

            const hasUtilityControl = (control.get('hasPremiseOrVehicleGroup') as FormGroup).get('hasUtilityControl');

            if (hasUtilityControl === null || hasUtilityControl === undefined) {
                return null;
            }

            if (hasUtilityControl.value) {
                if (this.premiseUsageDocuments === null || this.premiseUsageDocuments === undefined) {
                    return { 'noPremiseUsageDocuments': true };
                }

                if (this.premiseUsageDocuments.length === 0) {
                    return { 'noPremiseUsageDocuments': true };
                }

                if (this.premiseUsageDocuments.filter(x => x.isActive).length === 0) {
                    return { 'noPremiseUsageDocuments': true };
                }
            }

            return null;
        }
    }

    private shouldHidePaymentData(): boolean {
        return this.applicationPaymentInformation?.paymentType === null
            || this.applicationPaymentInformation?.paymentType === undefined
            || this.applicationPaymentInformation?.paymentType === '';
    }

    private closeEditPremiseUsageDocumentDialogBtnClicked(closeFn: HeaderCloseFunction): void {
        closeFn();
    }

    private closeOverlappingLogBooksDialogBtnClicked(closeFn: HeaderCloseFunction): void {
        closeFn();
    }
}