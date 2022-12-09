import { Component, EventEmitter, Injector, Input, OnChanges, OnInit, Output, SimpleChanges, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';

import { EditScientificPermitComponent } from '@app/components/common-app/scientific-fishing/components/edit-scientific-permit/edit-scientific-permit.component';
import { ApplicationHierarchyTypesEnum } from '@app/enums/application-hierarchy-types.enum';
import { ApplicationStatusesEnum } from '@app/enums/application-statuses.enum';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { PageCodeEnum } from '@app/enums/page-code.enum';
import { PaymentStatusesEnum } from '@app/enums/payment-statuses.enum';
import { IApplicationsRegisterService } from '@app/interfaces/administration-app/applications-register.interface';
import { IApplicationsService } from '@app/interfaces/administration-app/applications.interface';
import { IApplicationsActionsService } from '@app/interfaces/common-app/application-actions.interface';
import { IApplicationRegister } from '@app/interfaces/common-app/application-register.interface';
import { ApplicationsRegisterData } from '@app/models/common/applications-register-data.model';
import { DialogParamsModel } from '@app/models/common/dialog-params.model';
import { ReasonData } from '@app/models/common/reason-data.model';
import { ApplicationRegisterDTO } from '@app/models/generated/dtos/ApplicationRegisterDTO';
import { ApplicationsChangeHistoryDTO } from '@app/models/generated/dtos/ApplicationsChangeHistoryDTO';
import { ApplicationTypeDTO } from '@app/models/generated/dtos/ApplicationTypeDTO';
import { AssignedApplicationInfoDTO } from '@app/models/generated/dtos/AssignedApplicationInfoDTO';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { PaymentDataDTO } from '@app/models/generated/dtos/PaymentDataDTO';
import { BuyersAdministrationService } from '@app/services/administration-app/buyers-administration.service';
import { CommercialFishingAdministrationService } from '@app/services/administration-app/commercial-fishing-administration.service';
import { LegalEntitiesAdministrationService } from '@app/services/administration-app/legal-entities-administration.service';
import { QualifiedFishersService } from '@app/services/administration-app/qualified-fishers.service';
import { ScientificFishingAdministrationService } from '@app/services/administration-app/scientific-fishing-administration.service';
import { ShipsRegisterAdministrationService } from '@app/services/administration-app/ships-register-administration.service';
import { CommonNomenclatures } from '@app/services/common-app/common-nomenclatures.service';
import { LegalEntitiesPublicService } from '@app/services/public-app/legal-entities-public.service';
import { ScientificFishingPublicService } from '@app/services/public-app/scientific-fishing-public.service';
import { ShipsRegisterPublicService } from '@app/services/public-app/ships-register-public.service';
import { TLConfirmDialog } from '@app/shared/components/confirmation-dialog/tl-confirm-dialog';
import { TLDataTableComponent } from '@app/shared/components/data-table/tl-data-table.component';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { IHeaderAuditButton } from '@app/shared/components/dialog-wrapper/interfaces/header-audit-button.interface';
import { HeaderCloseFunction } from '@app/shared/components/dialog-wrapper/interfaces/header-cancel-button.interface';
import { TLMatDialog } from '@app/shared/components/dialog-wrapper/tl-mat-dialog';
import { PaymentDataComponent } from '@app/shared/components/payment-data/payment-data.component';
import { TLTranslatePipe } from '@app/shared/pipes/tl-translate.pipe';
import { AuthService } from '@app/shared/services/auth.service';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { CommercialFishingPublicService } from '@app/services/public-app/commercial-fishing-public.service';
import { OnlinePaymentDataDialogParams } from '@app/shared/components/online-payment-data/egov-offline-payment-data/models/online-payment-data-dialog-params.model';
import { OnlinePaymentDataComponent } from '@app/shared/components/online-payment-data/online-payment-data.component';
import { EditBuyersComponent } from '@app/components/common-app/buyers/edit-buyers.component';
import { EditCommercialFishingComponent } from '@app/components/common-app/commercial-fishing/components/edit-commercial-fishing/edit-commercial-fishing.component';
import { EditLegalEntityComponent } from '@app/components/common-app/legals/edit-legal-entity/edit-legal-entity.component';
import { EditFisherComponent } from '@app/components/common-app/qualified-fishers/edit-fisher.component';
import { EditShipComponent } from '@app/components/common-app/ships-register/edit-ship/edit-ship.component';
import { ShipChangeOfCircumstancesComponent } from '@app/components/common-app/ships-register/ship-change-of-circumstances/ship-change-of-circumstances.component';
import { AssignApplicationByAccessCodeComponent } from '../components/assign-application-by-access-code/assign-application-by-access-code.component';
import { ChooseApplicationTypeComponent } from '../components/choose-application-type/choose-application-type.component';
import { EnterReasonComponent } from '../components/enter-reason/enter-reason.component';
import { ReasonDialogParams } from '../components/enter-reason/models/reason-dialog-params.model';
import { FileInApplicationStepperComponent } from '../components/file-in-application-stepper/file-in-application-stepper.component';
import { FileInApplicationDialogParams } from '../components/file-in-application-stepper/models/file-in-application-stepper-dialog-params.model';
import { UploadFileDialogComponent } from '../components/upload-file-dialog/upload-file-dialog.component';
import { ApplicationProcessingHasPermissions } from '../models/application-processing-has-permissions.model';
import { EditDialogInfo } from '../models/edit-dialog-info.model';
import { FishingCapacityPublicService } from '@app/services/public-app/fishing-capacity-public.service';
import { FishingCapacityAdministrationService } from '@app/services/administration-app/fishing-capacity-administration.service';
import { IS_PUBLIC_APP } from '@app/shared/modules/application.modules';
import { IncreaseFishingCapacityComponent } from '@app/components/common-app/fishing-capacity/increase-fishing-capacity/increase-fishing-capacity.component';
import { ReduceFishingCapacityComponent } from '@app/components/common-app/fishing-capacity/reduce-fishing-capacity/reduce-fishing-capacity.component';
import { TransferFishingCapacityComponent } from '@app/components/common-app/fishing-capacity/transfer-fishing-capacity/transfer-fishing-capacity.component';
import { EditAquacultureFacilityComponent } from '@app/components/common-app/aquaculture-facilities/edit-aquaculture-facility/edit-aquaculture-facility.component';
import { AquacultureChangeOfCircumstancesComponent } from '@app/components/common-app/aquaculture-facilities/aquaculture-change-of-circumstances/aquaculture-change-of-circumstances.component';
import { AquacultureDeregistrationComponent } from '@app/components/common-app/aquaculture-facilities/aquaculture-deregistration/aquaculture-deregistration.component';
import { BuyersPublicService } from '@app/services/public-app/buyers-public.service';
import { QualifiedFishersPublicService } from '@app/services/public-app/qualified-fishers-public.service';
import { StatisticalFormsAquaFarmComponent } from '@app/components/common-app/statistical-forms/components/statistical-forms-aqua-farm/statistical-forms-aqua-farm.component';
import { StatisticalFormsReworkComponent } from '@app/components/common-app/statistical-forms/components/statistical-forms-rework/statistical-forms-rework.component';
import { StatisticalFormsFishVesselComponent } from '@app/components/common-app/statistical-forms/components/statistical-forms-fish-vessel/statistical-forms-fish-vessel.component';
import { StatisticalFormsAdministrationService } from '@app/services/administration-app/statistical-forms-administration.service';
import { StatisticalFormsPublicService } from '@app/services/public-app/statistical-forms-public.service';
import { AquacultureFacilitiesAdministrationService } from '@app/services/administration-app/aquaculture-facilities-administration.service';
import { AquacultureFacilitiesPublicService } from '@app/services/public-app/aquaculture-facilities-public.service';
import { BuyerChangeOfCircumstancesComponent } from '@app/components/common-app/buyers/buyer-change-of-circumstances/buyer-change-of-circumstances.component';
import { PaymentDataInfo } from '@app/shared/components/payment-data/models/payment-data-info.model';
import { BuyerTerminationComponent } from '@app/components/common-app/buyers/buyer-termination/buyer-termination.component';
import { DuplicatesRegisterPublicService } from '@app/services/public-app/duplicates-register-public.service';
import { DuplicatesRegisterAdministrationService } from '@app/services/administration-app/duplicates-register-administration.service';
import { DuplicatesApplicationComponent } from '@app/components/common-app/duplicates/duplicates-application.component';
import { CapacityCertificateDuplicateComponent } from '@app/components/common-app/fishing-capacity/capacity-certificate-duplicate/capacity-certificate-duplicate.component';
import { EditLegalAssociationComponent } from '@app/components/common-app/legal-associations/edit-legal-association/edit-legal-association.component';
import { RecreationalFishingAssociationService } from '@app/services/administration-app/recreational-fishing-association.service';
import { RecreationalFishingAssociationPublicService } from '@app/services/public-app/recreational-fishing-association-public.service';
import { AssignApplicationByUserComponent } from '../components/assign-application-by-user/assign-application-by-user.component';

const DIALOG_WIDTH: string = '1600px';
export type ApplicationTablePageType = 'FileInPage' | 'DashboardPage' | 'ApplicationPage' | 'PublicPage';

@Component({
    selector: 'applications-table',
    templateUrl: './applications-table.component.html'
})
export class ApplicationsTableComponent<T extends IDialogComponent> implements OnInit, OnChanges {
    @Input() public applicationsRegisterData!: Map<PageCodeEnum, ApplicationsRegisterData<T>>;
    @Input() public service!: IApplicationsRegisterService;
    @Input() public applicationsService!: IApplicationsService;
    @Input() public pageType: ApplicationTablePageType = 'ApplicationPage';
    @Input() public processingPermissions!: Map<PageCodeEnum, ApplicationProcessingHasPermissions>;
    @Input() public recordsPerPage: number = 20;

    @Output() public onAddedOrEditted: EventEmitter<number> = new EventEmitter<number>();
    @Output() public onDeleted: EventEmitter<number> = new EventEmitter<number>();
    @Output() public onRestored: EventEmitter<number> = new EventEmitter<number>();

    @ViewChild(TLDataTableComponent)
    public datatable!: TLDataTableComponent;

    public userId!: number;
    public showAddButton: boolean = false;
    public showInactiveRecords: boolean = false;

    public readonly icIconSize: number = CommonUtils.IC_ICON_SIZE;
    public readonly applicationSourceTypeEnum: typeof ApplicationHierarchyTypesEnum = ApplicationHierarchyTypesEnum;
    public readonly applicationStatusEnum: typeof ApplicationStatusesEnum = ApplicationStatusesEnum;
    public readonly applicationPaymentStatusEnum: typeof PaymentStatusesEnum = PaymentStatusesEnum;

    private readonly confirmDialog: TLConfirmDialog;
    private readonly translationService: FuseTranslationLoaderService;
    private readonly nomenclaturesService: CommonNomenclatures;
    private readonly addApplicationRequestDialog: TLMatDialog<FileInApplicationStepperComponent>;
    private readonly enterEventisNumberDialog: TLMatDialog<FileInApplicationStepperComponent>;
    private readonly assignApplicationByAccessCodeDialog: TLMatDialog<AssignApplicationByAccessCodeComponent>;
    private readonly assignApplicationByUserIdDialog: TLMatDialog<AssignApplicationByUserComponent>;
    private readonly statusReasonDialog: TLMatDialog<EnterReasonComponent>;
    private readonly paymentDataDialog: TLMatDialog<PaymentDataComponent>;
    private readonly chooseApplicationTypeDialog: TLMatDialog<ChooseApplicationTypeComponent>;
    private readonly uploadFileDialog: TLMatDialog<UploadFileDialogComponent>;
    private readonly onlinePaymentDialog: TLMatDialog<OnlinePaymentDataComponent>;
    private readonly matDialog: MatDialog;
    private readonly injector: Injector;

    private paymentTypes: NomenclatureDTO<number>[] = [];

    private editDialog!: TLMatDialog<T>;
    private editDialogTCtor!: new (...args: unknown[]) => T;
    private editApplicationDialogTitle: string = '';
    private editRegixDataDialogTitle: string = '';
    private addRegisterDialogTitle: string = '';
    private viewApplicationDialogTitle: string = '';
    private viewRegixDataDialogTitle: string = '';
    private viewRegisterDialogTitle: string = '';
    private viewAndConfirmDataRegularityTitle: string = '';
    private customRightSideActionButtons: IActionInfo[] | undefined;
    private customLeftSideActionButtons: IActionInfo[] | undefined;

    private tlTranslatePipe: TLTranslatePipe;
    private router: Router;

    public constructor(
        confirmDialog: TLConfirmDialog,
        translationService: FuseTranslationLoaderService,
        nomenclaturesService: CommonNomenclatures,
        addApplicationRequestDialog: TLMatDialog<FileInApplicationStepperComponent>,
        enterEventisNumberDialog: TLMatDialog<FileInApplicationStepperComponent>,
        assignApplicationByAccessCodeDialog: TLMatDialog<AssignApplicationByAccessCodeComponent>,
        assignApplicationByUserIdDialog: TLMatDialog<AssignApplicationByUserComponent>,
        statusReasonDialog: TLMatDialog<EnterReasonComponent>,
        paymentDataDialog: TLMatDialog<PaymentDataComponent>,
        chooseApplicationTypeDialog: TLMatDialog<ChooseApplicationTypeComponent>,
        uploadFileDialog: TLMatDialog<UploadFileDialogComponent>,
        onlinePaymentDialog: TLMatDialog<OnlinePaymentDataComponent>,
        matDialog: MatDialog,
        injector: Injector,
        tlTranslatePipe: TLTranslatePipe,
        router: Router,
        authService: AuthService
    ) {
        this.confirmDialog = confirmDialog;
        this.translationService = translationService;
        this.nomenclaturesService = nomenclaturesService;
        this.addApplicationRequestDialog = addApplicationRequestDialog;
        this.enterEventisNumberDialog = enterEventisNumberDialog;
        this.assignApplicationByAccessCodeDialog = assignApplicationByAccessCodeDialog;
        this.assignApplicationByUserIdDialog = assignApplicationByUserIdDialog;
        this.statusReasonDialog = statusReasonDialog;
        this.paymentDataDialog = paymentDataDialog;
        this.onlinePaymentDialog = onlinePaymentDialog;
        this.chooseApplicationTypeDialog = chooseApplicationTypeDialog;
        this.uploadFileDialog = uploadFileDialog;
        this.matDialog = matDialog;
        this.injector = injector;
        this.tlTranslatePipe = tlTranslatePipe;
        this.router = router;
        this.userId = authService.userRegistrationInfo!.id!;
    }

    public ngOnInit(): void {
        if (this.pageType === 'ApplicationPage') {
            NomenclatureStore.instance.getNomenclature(
                NomenclatureTypes.OfflinePaymentTypes, this.nomenclaturesService.getOfflinePaymentTypes.bind(this.nomenclaturesService), false
            ).subscribe({
                next: (paymentTypes: NomenclatureDTO<number>[]) => {
                    this.paymentTypes = paymentTypes;
                }
            });
        }
    }

    public ngOnChanges(changes: SimpleChanges): void {
        if ('processingPermissions' in changes) {
            for (const map of this.processingPermissions) {
                this.showAddButton ||= map[1].canAddRecords;
                this.showInactiveRecords ||= map[1].canRestoreRecords;
            }
        }
    }

    public createOrAssignApplication(): void {
        if (this.pageType === 'FileInPage') {
            const data: FileInApplicationDialogParams = new FileInApplicationDialogParams();
            const dialog = this.addApplicationRequestDialog.openWithTwoButtons({
                title: this.translationService.getValue('applications-register.add-application-request-dialog-title'),
                TCtor: FileInApplicationStepperComponent,
                headerCancelButton: {
                    cancelBtnClicked: this.closeAddApplicationRequestDialogBtnClicked.bind(this)
                },
                translteService: this.translationService,
                disableDialogClose: true
            }, DIALOG_WIDTH);

            dialog.subscribe((applicationId: number) => {
                if (applicationId !== null && applicationId !== undefined) {
                    this.onAddedOrEditted.emit(applicationId);
                }
            });
        }
        else if (this.pageType === 'ApplicationPage') {
            const data: DialogParamsModel = new DialogParamsModel({
                service: this.service
            });

            const dialog = this.assignApplicationByAccessCodeDialog.openWithTwoButtons({
                title: this.translationService.getValue('applications-register.assign-appl-via-access-code-dialog-title'),
                TCtor: AssignApplicationByAccessCodeComponent,
                headerCancelButton: {
                    cancelBtnClicked: this.closeAssignApplicationByAccessCodeDialogBtnClicked.bind(this)
                },
                translteService: this.translationService,
                componentData: data
            }, '550px');

            dialog.subscribe((saveData: AssignedApplicationInfoDTO) => {
                if (saveData !== null && saveData !== undefined) {
                    const status: ApplicationStatusesEnum = ApplicationStatusesEnum[saveData.statusCode as keyof typeof ApplicationStatusesEnum];
                    this.initiateOpenEditDialog(saveData.id!, status, undefined, saveData.pageCode!, false);
                }
            });
        }
        else if (this.pageType === 'PublicPage') {
            this.chooseApplicationTypeDialog.openWithTwoButtons({
                TCtor: ChooseApplicationTypeComponent,
                title: this.translationService.getValue('applications-register.choose-application-type-to-submit'),
                translteService: this.translationService,
                disableDialogClose: true,
                headerCancelButton: {
                    cancelBtnClicked: this.closeChooseApplicationTypeDialogBtnClicked.bind(this)
                },
                cancelBtn: {
                    id: 'cancel',
                    color: 'primary',
                    translateValue: 'common.cancel',
                },
                saveBtn: {
                    id: 'save',
                    color: 'accent',
                    translateValue: 'common.save'
                }
            }).subscribe((selectedApplicationType: ApplicationTypeDTO | undefined) => {
                if (selectedApplicationType !== undefined) {
                    this.applicationsService.addApplication(selectedApplicationType.value!).subscribe(
                        (applicationIdentification: {
                            item1: number, // applicationId
                            item2: string // accessCode
                        }) => {
                            this.initiateOpenEditDialog(applicationIdentification.item1,
                                ApplicationStatusesEnum.INITIAL,
                                undefined,
                                selectedApplicationType.pageCode!,
                                false);
                        });
                }
            });
        }
    }

    public closeChooseApplicationTypeDialogBtnClicked(closeFn: HeaderCloseFunction): void {
        closeFn();
    }

    public closeEditDialogBtnClicked(closeFn: HeaderCloseFunction): void {
        closeFn();
    }

    public closeAddApplicationRequestDialogBtnClicked(closeFn: HeaderCloseFunction): void {
        closeFn();
    }

    public closeAssignApplicationByAccessCodeDialogBtnClicked(closeFn: HeaderCloseFunction): void {
        closeFn();
    }

    public openApplicationInViewMode(application: ApplicationRegisterDTO): void {
        this.initiateOpenEditDialog(application.id!, application.statusCode as ApplicationStatusesEnum, application.prevStatusCode, application.pageCode!, true);
    }

    public openApplicationHistoryDraftInViewMode(applicationChangeHistory: ApplicationsChangeHistoryDTO): void {
        const application: ApplicationRegisterDTO = (this.datatable.rows as ApplicationRegisterDTO[]).find(x => x.id === applicationChangeHistory.applicationId)!;
        this.initiateOpenEditDialog(applicationChangeHistory.id as number, undefined, application.prevStatusCode, application.pageCode!, true, true);
    }

    public openRegisterInViewMode(application: ApplicationRegisterDTO): void {
        this.openEditRegisterDialog(application.id!, true, application.pageCode!);
    }

    public deleteApplication(application: ApplicationRegisterDTO): void {
        this.confirmDialog.open({
            title: this.translationService.getValue('applications-register.delete-application-dialog-title'),
            message: this.translationService.getValue('applications-register.confirm-delete-message'),
            okBtnLabel: this.translationService.getValue('applications-register.delete')
        }).subscribe({
            next: (ok: boolean) => {
                if (ok && application?.id) {
                    this.onDeleted.emit(application.id);
                }
            }
        });
    }

    public restoreApplication(application: ApplicationRegisterDTO): void {
        this.confirmDialog.open().subscribe({
            next: (ok: boolean) => {
                if (ok && application?.id) {
                    this.onRestored.emit(application.id);
                }
            }
        });
    }

    public showApplication(application: ApplicationRegisterDTO): void {
        switch (application.pageCode) {
            case PageCodeEnum.SciFi:
                this.navigateByUrl('/scientific-fishing-applications', application.accessCode!); break;
            case PageCodeEnum.ChangeFirstSaleBuyer:
            case PageCodeEnum.ChangeFirstSaleCenter:
            case PageCodeEnum.RegFirstSaleBuyer:
            case PageCodeEnum.RegFirstSaleCenter:
            case PageCodeEnum.TermFirstSaleBuyer:
            case PageCodeEnum.TermFirstSaleCenter:
            case PageCodeEnum.DupFirstSaleBuyer:
            case PageCodeEnum.DupFirstSaleCenter:
                this.navigateByUrl('/sales-centers-applications', application.accessCode!); break;
            case PageCodeEnum.DeregShip:
            case PageCodeEnum.RegVessel:
            case PageCodeEnum.ShipRegChange:
                this.navigateByUrl('/fishing-vessels-applications', application.accessCode!); break;
            case PageCodeEnum.LE:
            case PageCodeEnum.Assocs:
                this.navigateByUrl('/legal-entities-applications', application.accessCode!); break;
            case PageCodeEnum.AptitudeCourceExam:
            case PageCodeEnum.CommFishLicense:
            case PageCodeEnum.CompetencyDup:
                this.navigateByUrl('/qualified-fishers-applications', application.accessCode!); break;
            case PageCodeEnum.CommFish:
            case PageCodeEnum.RightToFishResource:
            case PageCodeEnum.RightToFishThirdCountry:
            case PageCodeEnum.PoundnetCommFish:
            case PageCodeEnum.PoundnetCommFishLic:
            case PageCodeEnum.CatchQuataSpecies:
            case PageCodeEnum.DupCommFish:
            case PageCodeEnum.DupRightToFishThirdCountry:
            case PageCodeEnum.DupPoundnetCommFish:
            case PageCodeEnum.DupRightToFishResource:
            case PageCodeEnum.DupPoundnetCommFishLic:
            case PageCodeEnum.DupCatchQuataSpecies:
                this.navigateByUrl('/commercial-fishing-applications', application.accessCode!); break;
            case PageCodeEnum.AquaFarmReg:
            case PageCodeEnum.AquaFarmChange:
            case PageCodeEnum.AquaFarmDereg:
                this.navigateByUrl('/aquaculture-farms-applications', application.accessCode!); break;
            case PageCodeEnum.ReduceFishCap:
            case PageCodeEnum.IncreaseFishCap:
            case PageCodeEnum.TransferFishCap:
            case PageCodeEnum.CapacityCertDup:
                this.navigateByUrl('/fishing-capacity-applications', application.accessCode!); break;
            case PageCodeEnum.StatFormAquaFarm:
            case PageCodeEnum.StatFormRework:
            case PageCodeEnum.StatFormFishVessel:
                this.navigateByUrl('/statistical-forms-applications', application.accessCode!); break;
            default:
                this.navigateByUrl('/application_processing', application.accessCode!); break;

        }
    }

    public enterEventisNumberActionClicked(application: ApplicationRegisterDTO): void {
        const data: FileInApplicationDialogParams = new FileInApplicationDialogParams({
            applicationId: application.id
        });

        const dialog = this.enterEventisNumberDialog.openWithTwoButtons({
            title: this.translationService.getValue('applications-register.enter-eventis-number-dialog-title'),
            TCtor: FileInApplicationStepperComponent,
            headerCancelButton: {
                cancelBtnClicked: this.closeEnterEventisNumberDialogBtnClicked.bind(this)
            },
            translteService: this.translationService,
            componentData: data
        }, '1000px');

        dialog.subscribe(() => {
            this.onAddedOrEditted.emit(application.id);
        });
    }

    public closeEnterEventisNumberDialogBtnClicked(closeFn: HeaderCloseFunction): void {
        closeFn();
    }

    public assignApplicationViaUserId(application: ApplicationRegisterDTO): void {
        if (this.pageType === 'FileInPage' || this.pageType === 'ApplicationPage') { // Не се позволява присвояване в публичното и в таблото
            const dialog = this.assignApplicationByUserIdDialog.openWithTwoButtons({
                TCtor: AssignApplicationByUserComponent,
                title: this.translationService.getValue('applications-register.assign-application-by-user-dialog-title'),
                translteService: this.translationService,
                componentData: new DialogParamsModel({
                    applicationId: application.id,
                    service: this.service
                }),
                headerCancelButton: {
                    cancelBtnClicked: (closeFn: HeaderCloseFunction) => { closeFn(); }
                },
                saveBtn: {
                    id: 'save',
                    translateValue: 'applications-register.assign-application-by-user-btn',
                    color: 'accent'
                },
                viewMode: false
            }, '1000px');

            dialog.subscribe({
                next: (result: AssignedApplicationInfoDTO | undefined) => {
                    if (result !== null && result !== undefined) {
                        this.onAddedOrEditted.emit(application.id);
                    }
                }
            });
        }
    }

    public editActionClicked(application: ApplicationRegisterDTO): void {
        this.initiateOpenEditDialog(application.id!, application.statusCode as ApplicationStatusesEnum, application.prevStatusCode, application.pageCode!, false);
    }

    public cancellationActionClicked(application: ApplicationRegisterDTO): void {
        this.statusReasonDialog.openWithTwoButtons({
            title: this.translationService.getValue('applications-register.cancellation-reason-dialog-title'),
            TCtor: EnterReasonComponent,
            headerCancelButton: {
                cancelBtnClicked: this.closeCancellationActionDialogBtnClicked.bind(this)
            },
            translteService: this.translationService,
            componentData: new ReasonDialogParams({
                isApplication: true,
                id: application.id,
                saveReasonServiceMethod: this.applicationsService.applicationAnnulment.bind(this.applicationsService)
            }),
            saveBtn: {
                id: 'save-button-id',
                translateValue: this.translationService.getValue('applications-register.cancel-application'),
                color: 'warn'
            },
            disableDialogClose: true
        }, '800px')
            .subscribe(() => {
                this.onAddedOrEditted.emit(application.id);
            });
    }

    public closeCancellationActionDialogBtnClicked(closeFn: HeaderCloseFunction): void {
        closeFn();
    }

    public confirmNoErrorsForApplicationActionClicked(applicationId: number, dialogClose: DialogCloseCallback): void {
        this.applicationsService.confirmNoErrorsForApplication(applicationId).subscribe(() => {
            dialogClose();
            this.onAddedOrEditted.emit(applicationId);
        });
    }

    public sendApplicationForUserCorrectionsActionClicked(model: IApplicationRegister, dialogClose: DialogCloseCallback): void {
        this.statusReasonDialog.openWithTwoButtons({
            title: this.translationService.getValue('applications-register.send-for-user-corrections-reason-dialog-title'),
            TCtor: EnterReasonComponent,
            headerCancelButton: {
                cancelBtnClicked: this.closeCancellationActionDialogBtnClicked.bind(this)
            },
            translteService: this.translationService,
            componentData: new ReasonDialogParams({
                isApplication: true,
                id: model.applicationId,
                reasonFieldValue: model.statusReason
            }),
            disableDialogClose: true
        }, '800px')
            .subscribe((reasonData: ReasonData) => {
                if (reasonData !== undefined) {
                    model.statusReason = reasonData.reason;
                    this.applicationsService.sendApplicationForUserCorrections(model.applicationId!, model.statusReason!).subscribe(() => {
                        dialogClose();
                        this.onAddedOrEditted.emit(model.applicationId);
                    });
                }
            });
    }

    public enterPaymentDataActionClicked(application: ApplicationRegisterDTO): void {
        debugger;
        const headerTitle: string = this.translationService.getValue('applications-register.enter-payment-data-dialog-title');
        const data = new PaymentDataInfo({
            paymentTypes: this.paymentTypes,
            viewMode: false,
            service: this.applicationsService,
            applicationId: application.id,
            paymentDateMax: new Date()
        });
        const dialog = this.paymentDataDialog.openWithTwoButtons({
            title: headerTitle,
            TCtor: PaymentDataComponent,
            headerCancelButton: {
                cancelBtnClicked: this.closeEnterPaymentDataDialogBtnClicked.bind(this)
            },
            componentData: data,
            translteService: this.translationService
        }, '1200px');

        dialog.subscribe((paymentData: PaymentDataDTO) => {
            if (paymentData !== null && paymentData !== undefined) {
                this.onAddedOrEditted.emit(application.id);
            }
        });
    }

    public closeEnterPaymentDataDialogBtnClicked(closeFn: HeaderCloseFunction): void {
        closeFn();
    }

    public enterOnlinePaymentDataActionClicked(application: ApplicationRegisterDTO): void {
        this.openOnlinePaymentDialog(application.id!);
    }

    private openOnlinePaymentDialog(applicationId: number): void {
        const dialog = this.onlinePaymentDialog.open({
            title: this.translationService.getValue('applications-register.initiate-online-payment'),
            TCtor: OnlinePaymentDataComponent,
            headerCancelButton: {
                cancelBtnClicked: this.closePaymentDialogBtnClicked.bind(this)
            },
            translteService: this.translationService,
            componentData: new OnlinePaymentDataDialogParams({
                applicationId: applicationId,
                showTariffs: true
            })
        }, '1000px');

        dialog.subscribe({
            next: () => {
                this.onAddedOrEditted.emit(applicationId);
            }
        });
    }

    public renewPaymentActionClicked(application: ApplicationRegisterDTO): void {
        this.applicationsService.renewApplicationPayment(application.id!).subscribe({
            next: () => {
                this.onAddedOrEditted.emit(application.id);
            }
        });
    }

    public paymentRefusalActionClicked(application: ApplicationRegisterDTO): void {
        this.statusReasonDialog.openWithTwoButtons({
            title: this.translationService.getValue('applications-register.payment-refusal-reason-dialog-title'),
            TCtor: EnterReasonComponent,
            headerCancelButton: {
                cancelBtnClicked: this.closeCancellationActionDialogBtnClicked.bind(this)
            },
            translteService: this.translationService,
            componentData: new ReasonDialogParams({
                isApplication: true,
                id: application.id,
                saveReasonServiceMethod: this.applicationsService.applicationPaymentRefusal.bind(this.applicationsService)
            }),
            saveBtn: {
                id: 'save-button-id',
                translateValue: this.translationService.getValue('applications-register.refuse-payment'),
                color: 'warn'
            },
            disableDialogClose: true
        }, '800px')
            .subscribe((reasonData: ReasonData) => {
                this.onAddedOrEditted.emit(application.id);
            });
    }

    public checkDataRegularityActionClicked(application: ApplicationRegisterDTO): void {
        this.initiateOpenEditDialog(application.id!, application.statusCode as ApplicationStatusesEnum, application.prevStatusCode, application.pageCode!, false);
    }

    public confirmDataIrregularityActionClicked(applicationId: number, dialogClose: DialogCloseCallback): void {
        this.statusReasonDialog.openWithTwoButtons({
            title: this.translationService.getValue('applications-register.confirm-data-irregularity-reason-dialog-title'),
            TCtor: EnterReasonComponent,
            headerCancelButton: {
                cancelBtnClicked: this.closeCancellationActionDialogBtnClicked.bind(this)
            },
            translteService: this.translationService,
            componentData: new ReasonDialogParams({
                isApplication: true,
                id: applicationId
            }),
            disableDialogClose: true
        }, '800px')
            .subscribe((reasonData: ReasonData) => {
                this.applicationsService.confirmApplicationDataIrregularity(applicationId, reasonData.reason!).subscribe(() => {
                    dialogClose();
                    this.onAddedOrEditted.emit(applicationId);
                });
            });
    }

    public confirmDataRegularityActionClicked(applicationId: number, dialogClose: DialogCloseCallback): void {
        this.applicationsService.confirmApplicationDataRegularity(applicationId).subscribe(() => {
            dialogClose();
            this.onAddedOrEditted.emit(applicationId);
        });
    }

    public createRegisterActionClicked(application: ApplicationRegisterDTO): void {
        const data: ApplicationsRegisterData<T> | undefined = this.applicationsRegisterData.get(application.pageCode!);
        if (data?.createRegisterCallback !== undefined && data?.createRegisterCallback !== null) {
            const obs: Observable<any> | undefined = data.createRegisterCallback(application) as Observable<any>;
            if (obs !== null && obs !== undefined) {
                obs.subscribe({
                    next: () => {
                        this.onAddedOrEditted.emit(application.id);
                    }
                });
            }
        }
        else {
            this.openEditRegisterDialog(application.id!, false, application.pageCode!);
        }
    }

    public viewRegisterActionClicked(application: ApplicationRegisterDTO): void {
        this.openEditRegisterDialog(application.id!, true, application.pageCode!);
    }

    public saveApplicationDraftContentActionClicked(applicationId: number, model: IApplicationRegister, dialogClose: HeaderCloseFunction): void {
        this.applicationsService.saveDraftContent(applicationId, model).subscribe(() => {
            dialogClose();
            this.onAddedOrEditted.emit(applicationId);
        });
    }

    public uploadSignedApplicationActionClicked(application: ApplicationRegisterDTO): void {
        this.uploadFileDialog.openWithTwoButtons({
            title: this.translationService.getValue('applications-register.uploading-valid-signed-application-dialog-title'),
            TCtor: UploadFileDialogComponent,
            translteService: this.translationService,
            cancelBtn: {
                id: 'cancel',
                color: 'primary',
                translateValue: 'common.cancel',
            },
            saveBtn: {
                id: 'save',
                color: 'accent',
                translateValue: 'common.save'
            },
            componentData: {
                service: this.applicationsService,
                applicationId: application.id
            },
            disableDialogClose: true,
            headerCancelButton: {
                cancelBtnClicked: this.closeUploadSignedApplicationActionDialogBtnClicked.bind(this)
            }
        }, '1200px').subscribe(() => {
            this.onAddedOrEditted.emit(application.id);
        });
    }

    private closeUploadSignedApplicationActionDialogBtnClicked(closeFn: HeaderCloseFunction): void {
        closeFn();
    }

    public manualRegixChecksStartActionClicked(application: ApplicationRegisterDTO): void {
        this.applicationsService.manualRegixChecksStart(application.id!).subscribe(() => {
            this.onAddedOrEditted.emit(application.id);
        });
    }

    public downloadGeneratedApplicationActionClicked(application: ApplicationRegisterDTO): void {
        this.applicationsService.downloadGeneratedApplicationFile(application.id!).subscribe();
    }

    public goBackToFillApplicationByApplicantActionClicked(application: ApplicationRegisterDTO): void {
        this.confirmDialog.open({
            message: this.translationService.getValue('applications-register.are-you-sure-you-want-to-go-back-to-appl-fill'),
            title: this.translationService.getValue('applications-register.go-back-to-appl-fill-dialog-title'),
            okBtnLabel: this.translationService.getValue('applications-register.confirm-go-back-to-appl-fill'),
            cancelBtnLabel: this.tlTranslatePipe.transform('common.cancel', 'cap')
        }).subscribe((confirmed: boolean) => {
            if (confirmed) {
                this.applicationsService.goBackToFillApplicationByApplicant(application.id!).subscribe(() => {
                    this.onAddedOrEditted.emit(application.id);
                });
            }
        });
    }

    private saveApplicationAndStartRegixChecksActionClicked(model: IApplicationRegister, dialogClose: DialogCloseCallback): void {
        const service: IApplicationsActionsService = this.getServiceInstanceForPageCode(model.pageCode!);

        service.editApplicationDataAndStartRegixChecks(model).subscribe(() => {
            dialogClose();
            this.onAddedOrEditted.emit(model.applicationId);
        });
    }

    private completeApplicationFillingByApplicantActionClicked(applicationId: number): void {
        this.applicationsService.completeApplicationFillingByApplicant(applicationId).subscribe(() => {
            this.onAddedOrEditted.emit(applicationId);
        });
    }

    private initiateOpenEditDialog(
        applicationId: number,
        applicationStatus: ApplicationStatusesEnum | undefined,
        prevApplicationStatus: ApplicationStatusesEnum | undefined,
        pageCode: PageCodeEnum,
        viewMode: boolean = false,
        isApplicationHistoryMode = false
    ): void {
        if (applicationStatus === ApplicationStatusesEnum.CORR_BY_USR_NEEDED && !viewMode) {
            this.applicationsService.initiateApplicationCorrections(applicationId).subscribe((newStatus: ApplicationStatusesEnum) => {
                applicationStatus = newStatus;
                this.onAddedOrEditted.emit(applicationId);
                this.openEditDialog(applicationId, applicationStatus, prevApplicationStatus, pageCode, viewMode, isApplicationHistoryMode);
            });
        }
        else {
            this.openEditDialog(applicationId, applicationStatus, prevApplicationStatus, pageCode, viewMode, isApplicationHistoryMode);
        }
    }

    /**
     *  It's safer to be called through `initiateOpenEditDialog` (because of business logic)
     * @param applicationId
     * @param applicationStatus
     * @param viewMode indicates will the form be opened with disabled/readonly fields
     */
    private openEditDialog(
        applicationId: number,
        applicationStatus: ApplicationStatusesEnum | undefined,
        prevApplicationStatus: ApplicationStatusesEnum | undefined,
        pageCode: PageCodeEnum,
        viewMode: boolean = false,
        isApplicationHistoryMode: boolean = false
    ): void {
        let title: string;
        const rightButtons: IActionInfo[] = [];
        let leftButtons: IActionInfo[] | undefined;

        this.setApplicationDataFields(pageCode);

        let isReadOnly: boolean = !this.processingPermissions.get(pageCode)?.canEditRecords || viewMode;

        let showOnlyRegiXData: boolean = false;
        let showRegixData: boolean = false;
        let service: IApplicationsActionsService = this.getServiceInstanceForPageCode(pageCode);

        let auditButton: IHeaderAuditButton | undefined = undefined;

        if (this.pageType !== 'PublicPage') {
            if (isApplicationHistoryMode) {
                auditButton = {
                    id: applicationId,
                    getAuditRecordData: this.service.getApplicationHistorySimpleAudit.bind(this.service),
                    tableName: 'ApplicationChangeHistory'
                };
            }
            else { // TODO when to put audit from Db.Applications and when from the corresponding (by pageCode) register table and how ???
                //auditButton = {
                //    id: applicationId,
                //    getAuditRecordData: this.applicationsService.getSimpleAudit.bind(this.service)
                //};
            }
        }

        if (this.pageType === 'ApplicationPage') {
            switch (applicationStatus) {
                case ApplicationStatusesEnum.FILL_BY_EMP:
                case ApplicationStatusesEnum.ENTERED_ASSIGNED_APPL: {
                    rightButtons.push({
                        id: 'save-draft-content',
                        color: 'primary',
                        translateValue: 'applications-register.save-draft-content',
                        buttonData: { callbackFn: this.saveApplicationDraftContentActionClicked.bind(this) }
                    }, {
                        id: 'save',
                        color: 'accent',
                        translateValue: 'applications-register.save-application'
                    });

                    if (this.customRightSideActionButtons !== null && this.customRightSideActionButtons !== undefined) {
                        rightButtons.push(...this.customRightSideActionButtons);
                    }

                    if (this.customLeftSideActionButtons !== null && this.customLeftSideActionButtons !== undefined) {
                        leftButtons = [...this.customLeftSideActionButtons];
                    }
                } break;
                case ApplicationStatusesEnum.INSP_CORR_FROM_EMP: {
                    showOnlyRegiXData = true;
                    rightButtons.push({
                        id: 'save-and-start-regix-check',
                        color: 'primary',
                        translateValue: 'applications-register.save-and-start-regix-check',
                        buttonData: { callbackFn: this.saveApplicationAndStartRegixChecksActionClicked.bind(this) },
                        icon: { id: 'ic-fluent-doc-sync-24-regular', size: this.icIconSize },
                        disabled: true
                    }, {
                        id: 'more-corrections-needed',
                        color: 'warn',
                        translateValue: 'applications-register.send-for-further-corrections',
                        buttonData: { callbackFn: this.sendApplicationForUserCorrectionsActionClicked.bind(this) },
                        icon: { id: 'ic-fluent-doc-person-20-regular', size: this.icIconSize }
                    }, {
                        id: 'no-corrections-needed',
                        color: 'accent',
                        translateValue: 'applications-register.confirm-data-correctness',
                        buttonData: { callbackFn: this.confirmNoErrorsForApplicationActionClicked.bind(this) },
                        icon: { id: 'ic-fluent-doc-checkmark-24-regular', size: this.icIconSize }
                    });
                } break;
                case ApplicationStatusesEnum.WAIT_REG_CHKS_ISS_GROUNDS: {
                    isReadOnly = true;
                    rightButtons.push({
                        id: 'confirm-data-irregularity',
                        color: 'warn',
                        translateValue: 'applications-register.confirm-need-for-corrections',
                        buttonData: { callbackFn: this.confirmDataIrregularityActionClicked.bind(this) },
                        icon: { id: 'ic-fluent-document-error-16-regular', size: this.icIconSize }
                    }, {
                        id: 'confirm-data-regularity',
                        color: 'accent',
                        translateValue: 'applications-register.confirm-readiness-for-admin-act',
                        buttonData: { callbackFn: this.confirmDataRegularityActionClicked.bind(this) },
                        icon: { id: 'ic-fluent-doc-checkmark-24-regular', size: this.icIconSize }
                    });
                } break;
                default: {
                    viewMode = true;
                } break;
            }

            // Ако не сме в RegiX режим И предишният статус на заявлението е бил Стартирани проверки
            if (!showOnlyRegiXData && prevApplicationStatus === ApplicationStatusesEnum.EXT_CHK_STARTED) {
                showRegixData = true;
            }
        }
        else if (this.pageType === 'FileInPage') {
            switch (applicationStatus) {
                case ApplicationStatusesEnum.INSP_CORR_FROM_EMP: {
                    showOnlyRegiXData = true;
                    rightButtons.push({
                        id: 'save-and-start-regix-check',
                        color: 'primary',
                        translateValue: 'applications-register.save-and-start-regix-check',
                        buttonData: { callbackFn: this.saveApplicationAndStartRegixChecksActionClicked.bind(this) },
                        icon: { id: 'ic-fluent-doc-sync-24-regular', size: this.icIconSize },
                        disabled: true
                    }, {
                        id: 'more-corrections-needed',
                        color: 'warn',
                        translateValue: 'applications-register.send-for-further-corrections',
                        buttonData: { callbackFn: this.sendApplicationForUserCorrectionsActionClicked.bind(this) },
                        icon: { id: 'ic-fluent-doc-person-20-regular', size: this.icIconSize }
                    }, {
                        id: 'no-corrections-needed',
                        color: 'accent',
                        translateValue: 'applications-register.confirm-data-correctness',
                        buttonData: { callbackFn: this.confirmNoErrorsForApplicationActionClicked.bind(this) },
                        icon: { id: 'ic-fluent-doc-checkmark-24-regular', size: this.icIconSize }
                    });
                } break;
            }
        }
        else if (this.pageType === 'PublicPage') {
            switch (applicationStatus) {
                case ApplicationStatusesEnum.INITIAL:
                case ApplicationStatusesEnum.FILL_BY_APPL: {
                    rightButtons.push({
                        id: 'save-draft-content',
                        color: 'primary',
                        translateValue: 'applications-register.save-draft-content',
                        buttonData: { callbackFn: this.saveApplicationDraftContentActionClicked.bind(this) }
                    }, {
                        id: 'save-and-download-for-sign',
                        color: 'accent',
                        translateValue: 'applications-register.save-application-and-download-for-sign',
                        buttonData: { callbackFn: this.completeApplicationFillingByApplicantActionClicked.bind(this) }
                    });

                    if (this.customRightSideActionButtons !== null && this.customRightSideActionButtons !== undefined) {
                        rightButtons.push(...this.customRightSideActionButtons);
                    }

                    if (this.customLeftSideActionButtons !== null && this.customLeftSideActionButtons !== undefined) {
                        leftButtons = [...this.customLeftSideActionButtons];
                    }
                } break;
            }
        }

        if (this.pageType === 'FileInPage' || this.pageType === 'PublicPage') {
            const editDialogInfo = this.getEditDialogInfo(pageCode as PageCodeEnum, false);
            this.editDialog = editDialogInfo!.editDialog;
            this.editDialogTCtor = editDialogInfo!.editDialogTCtor;
            this.viewApplicationDialogTitle = editDialogInfo!.viewTitle;
            this.viewRegixDataDialogTitle = editDialogInfo!.viewRegixDataTitle;
            this.editRegixDataDialogTitle = editDialogInfo!.editRegixDataTitle;
            this.editApplicationDialogTitle = editDialogInfo!.editApplicationTitle;
            this.viewAndConfirmDataRegularityTitle = editDialogInfo!.viewApplicationDataAndConfrimRegularityTitle;
        }

        if (viewMode) {
            if (showOnlyRegiXData) {
                title = this.viewRegixDataDialogTitle;
            }
            else {
                title = this.viewApplicationDialogTitle;
            }
        }
        else if (showOnlyRegiXData) {
            title = this.editRegixDataDialogTitle;
        }
        else if (isReadOnly && applicationStatus === ApplicationStatusesEnum.WAIT_REG_CHKS_ISS_GROUNDS) {
            title = this.viewAndConfirmDataRegularityTitle;
        }
        else {
            title = this.editApplicationDialogTitle;
        }

        const editDialogData: DialogParamsModel = new DialogParamsModel({
            applicationId: applicationId,
            isApplication: true,
            isApplicationHistoryMode: isApplicationHistoryMode,
            isReadonly: isReadOnly,
            viewMode: viewMode,
            service: service,
            applicationsService: this.applicationsService,
            showOnlyRegiXData: showOnlyRegiXData,
            showRegiXData: showRegixData,
            pageCode: pageCode,
            onRecordAddedOrEdittedEvent: this.onAddedOrEditted,
            isThirdCountry: false // TODO CHANGE (pass as parameter somehow ...)
        });

        const dialog = this.editDialog.open({
            title: title,
            TCtor: this.editDialogTCtor,
            headerAuditButton: auditButton,
            headerCancelButton: {
                cancelBtnClicked: this.closeEditDialogBtnClicked.bind(this)
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
            leftSideActionsCollection: leftButtons,
            viewMode: viewMode
        }, DIALOG_WIDTH);

        dialog.subscribe(() => {
            if (!viewMode) {
                this.onAddedOrEditted.emit(applicationId);
            }
        });
    }

    private openEditRegisterDialog(applicationId: number, viewMode: boolean = false, pageCode: PageCodeEnum): void {
        let title: string = '';
        const isReadOnly: boolean = !this.processingPermissions.get(pageCode)?.canAddAdministrativeActRecords || viewMode;

        const service: IApplicationRegister | IApplicationsActionsService = this.getServiceInstanceForPageCode(pageCode);

        if (this.pageType === 'ApplicationPage') {
            this.setApplicationDataFields(pageCode);
        }
        else {
            const editDialogInfo: EditDialogInfo | null = this.getEditDialogInfo(pageCode, true);
            if (editDialogInfo !== null && editDialogInfo !== undefined) {
                this.editDialog = editDialogInfo!.editDialog;
                this.editDialogTCtor = editDialogInfo!.editDialogTCtor;
                this.viewRegisterDialogTitle = editDialogInfo!.viewRegisterTitle;
            }
        }

        const editDialogData: DialogParamsModel = new DialogParamsModel({
            isReadonly: isReadOnly || viewMode,
            viewMode: viewMode,
            service: service,
            applicationId: applicationId,
            pageCode: pageCode
        });

        if (viewMode) {
            editDialogData.loadRegisterFromApplication = true;
        }
        else {
            editDialogData.loadRegisterFromApplication = false;
        }

        if (viewMode) {
            title = this.viewRegisterDialogTitle;
        }
        else {
            title = this.addRegisterDialogTitle;
        }

        const dialog = this.editDialog.openWithTwoButtons({
            title: title,
            TCtor: this.editDialogTCtor,
            headerCancelButton: {
                cancelBtnClicked: this.closeEditDialogBtnClicked.bind(this)
            },
            componentData: editDialogData,
            translteService: this.translationService,
            disableDialogClose: true,
            viewMode: viewMode
        }, DIALOG_WIDTH);

        dialog.subscribe(() => {
            if (!viewMode) {
                this.onAddedOrEditted.emit(applicationId);
            }
        });
    }


    private setApplicationDataFields(pageCode: PageCodeEnum): void {
        if (this.pageType === 'ApplicationPage') {
            const applicationRegisterData = this.applicationsRegisterData.get(pageCode);

            if (applicationRegisterData === undefined) {
                throw new Error(`No applicationsRegisterData provided for pageCode: ${pageCode}.`);
            }

            this.editDialog = applicationRegisterData.editDialog;
            this.editDialogTCtor = applicationRegisterData.editDialogTCtor;
            this.editApplicationDialogTitle = applicationRegisterData.editApplicationDialogTitle;
            this.editRegixDataDialogTitle = applicationRegisterData.editRegixDataDialogTitle;
            this.addRegisterDialogTitle = applicationRegisterData.addRegisterDialogTitle;
            this.viewApplicationDialogTitle = applicationRegisterData.viewApplicationDialogTitle;
            this.viewRegixDataDialogTitle = applicationRegisterData.viewRegixDataDialogTitle;
            this.viewRegisterDialogTitle = applicationRegisterData.viewRegisterDialogTitle;
            this.viewAndConfirmDataRegularityTitle = applicationRegisterData.viewAndConfrimDataRegularityTitle;
            this.customRightSideActionButtons = applicationRegisterData.customRightSideActionButtons;
            this.customLeftSideActionButtons = applicationRegisterData.customLeftSideActionButtons;
        }
    }

    /**
     * Should be called only from 'FileInPage' and 'PublicPage' mode of the component
     * @param pageCode
     */
    private getEditDialogInfo(pageCode: PageCodeEnum, forRegister: boolean): EditDialogInfo | null {
        let editDialog!: TLMatDialog<IDialogComponent>,
            editDialogTCtor,
            viewDialogTitle!: string,
            viewRegisterTitle!: string,
            viewRegixDataTitle!: string,
            editRegixDataTitle!: string,
            editApplicationDialogTitle!: string,
            viewApplicationDataAndConfrimRegularityTitle!: string;

        switch (pageCode) {
            case PageCodeEnum.SciFi: {
                editDialog = new TLMatDialog<EditScientificPermitComponent>(this.matDialog);
                editDialogTCtor = EditScientificPermitComponent;
                viewDialogTitle = this.translationService.getValue('scientific-fishing.view-permit-dialog-title');
                viewRegisterTitle = this.translationService.getValue('scientific-fishing.permit-register-title');
                viewRegixDataTitle = this.translationService.getValue('scientific-fishing.edit-permit-application-regix-data-dialog-title');
                editRegixDataTitle = this.translationService.getValue('scientific-fishing.view-permit-application-dialog-title');
                editApplicationDialogTitle = this.translationService.getValue('scientific-fishing.edit-permit-application-dialog-title');
                viewApplicationDataAndConfrimRegularityTitle = this.translationService.getValue('scientific-fishing.view-permit-appl-and-confirm-regularity-title');
            } break;
            case PageCodeEnum.CommFish: {
                editDialog = new TLMatDialog<EditCommercialFishingComponent>(this.matDialog);
                editDialogTCtor = EditCommercialFishingComponent;
                viewDialogTitle = this.translationService.getValue('commercial-fishing.view-permit-application-dialog-title');
                viewRegisterTitle = this.translationService.getValue('commercial-fishing.view-permit-dialog-title');
                viewRegixDataTitle = this.translationService.getValue('commercial-fishing.view-permit-application-regix-data-dialog-title');
                editRegixDataTitle = this.translationService.getValue('commercial-fishing.edit-permit-application-regix-data-dialog-title');
                editApplicationDialogTitle = this.translationService.getValue('commercial-fishing.edit-permit-application-dialog-title');
                viewApplicationDataAndConfrimRegularityTitle = this.translationService.getValue('commercial-fishing.view-permit-appl-and-confirm-regularity-title');
            } break;
            case PageCodeEnum.DupCommFish: {
                editDialog = forRegister
                    ? new TLMatDialog<EditCommercialFishingComponent>(this.matDialog)
                    : new TLMatDialog<DuplicatesApplicationComponent>(this.matDialog);
                editDialogTCtor = forRegister
                    ? EditCommercialFishingComponent
                    : DuplicatesApplicationComponent;
                viewDialogTitle = this.translationService.getValue('duplicates.view-permit-application-dialog-title');
                viewRegisterTitle = this.translationService.getValue('duplicates.view-permit-dialog-title');
                viewRegixDataTitle = this.translationService.getValue('duplicates.view-permit-application-regix-data-dialog-title');
                editRegixDataTitle = this.translationService.getValue('duplicates.edit-permit-application-regix-data-dialog-title');
                editApplicationDialogTitle = this.translationService.getValue('duplicates.edit-permit-application-dialog-title');
                viewApplicationDataAndConfrimRegularityTitle = this.translationService.getValue('duplicates.view-permit-appl-and-confirm-regularity-title');
            } break;
            case PageCodeEnum.RightToFishThirdCountry: {
                editDialog = new TLMatDialog<EditCommercialFishingComponent>(this.matDialog);
                editDialogTCtor = EditCommercialFishingComponent;
                viewDialogTitle = this.translationService.getValue('commercial-fishing.view-3rd-country-permit-application-dialog-title');
                viewRegisterTitle = this.translationService.getValue('commercial-fishing.view-3rd-country-permit-dialog-title');
                viewRegixDataTitle = this.translationService.getValue('commercial-fishing.view-3rd-country-permit-application-regix-data-dialog-title');
                editRegixDataTitle = this.translationService.getValue('commercial-fishing.edit-3rd-country-permit-application-regix-data-dialog-title');
                editApplicationDialogTitle = this.translationService.getValue('commercial-fishing.edit-3rd-country-permit-application-dialog-title');
                viewApplicationDataAndConfrimRegularityTitle = this.translationService.getValue('commercial-fishing.view-3rd-country-permit-appl-and-confirm-regularity-title');
            } break;
            case PageCodeEnum.DupRightToFishThirdCountry: {
                editDialog = forRegister
                    ? new TLMatDialog<EditCommercialFishingComponent>(this.matDialog)
                    : new TLMatDialog<DuplicatesApplicationComponent>(this.matDialog);
                editDialogTCtor = forRegister
                    ? EditCommercialFishingComponent
                    : DuplicatesApplicationComponent;
                viewDialogTitle = this.translationService.getValue('duplicates.view-3rd-country-permit-application-dialog-title');
                viewRegisterTitle = this.translationService.getValue('duplicates.view-3rd-country-permit-dialog-title');
                viewRegixDataTitle = this.translationService.getValue('duplicates.view-3rd-country-permit-application-regix-data-dialog-title');
                editRegixDataTitle = this.translationService.getValue('duplicates.edit-3rd-country-permit-application-regix-data-dialog-title');
                editApplicationDialogTitle = this.translationService.getValue('duplicates.edit-3rd-country-permit-application-dialog-title');
                viewApplicationDataAndConfrimRegularityTitle = this.translationService.getValue('duplicates.view-3rd-country-permit-appl-and-confirm-regularity-title');
            } break;
            case PageCodeEnum.PoundnetCommFish: {
                editDialog = new TLMatDialog<EditCommercialFishingComponent>(this.matDialog);
                editDialogTCtor = EditCommercialFishingComponent;
                viewDialogTitle = this.translationService.getValue('commercial-fishing.view-poundnet-permit-application-dialog-title');
                viewRegisterTitle = this.translationService.getValue('commercial-fishing.view-poundnet-permit-dialog-title');
                viewRegixDataTitle = this.translationService.getValue('commercial-fishing.view-poundnet-permit-application-regix-data-dialog-title');
                editRegixDataTitle = this.translationService.getValue('commercial-fishing.edit-poundnet-permit-application-regix-data-dialog-title');
                editApplicationDialogTitle = this.translationService.getValue('commercial-fishing.edit-poundnet-permit-application-dialog-title');
                viewApplicationDataAndConfrimRegularityTitle = this.translationService.getValue('commercial-fishing.view-poundnet-permit-appl-and-confirm-regularity-title');
            } break;
            case PageCodeEnum.DupPoundnetCommFish: {
                editDialog = forRegister
                    ? new TLMatDialog<EditCommercialFishingComponent>(this.matDialog)
                    : new TLMatDialog<DuplicatesApplicationComponent>(this.matDialog);
                editDialogTCtor = forRegister
                    ? EditCommercialFishingComponent
                    : DuplicatesApplicationComponent;
                viewDialogTitle = this.translationService.getValue('duplicates.view-poundnet-permit-application-dialog-title');
                viewRegisterTitle = this.translationService.getValue('duplicates.view-poundnet-permit-dialog-title');
                viewRegixDataTitle = this.translationService.getValue('duplicates.view-poundnet-permit-application-regix-data-dialog-title');
                editRegixDataTitle = this.translationService.getValue('duplicates.edit-poundnet-permit-application-regix-data-dialog-title');
                editApplicationDialogTitle = this.translationService.getValue('duplicates.edit-poundnet-permit-application-dialog-title');
                viewApplicationDataAndConfrimRegularityTitle = this.translationService.getValue('duplicates.view-poundnet-permit-appl-and-confirm-regularity-title');
            } break;
            case PageCodeEnum.RightToFishResource: {
                editDialog = new TLMatDialog<EditCommercialFishingComponent>(this.matDialog);
                editDialogTCtor = EditCommercialFishingComponent;
                viewDialogTitle = this.translationService.getValue('commercial-fishing.view-permit-license-application-dialog-title');
                viewRegisterTitle = this.translationService.getValue('commercial-fishing.view-permit-license-dialog-title');
                viewRegixDataTitle = this.translationService.getValue('commercial-fishing.view-permit-license-application-regix-data-dialog-title');
                editRegixDataTitle = this.translationService.getValue('commercial-fishing.edit-permit-license-application-regix-data-dialog-title');
                editApplicationDialogTitle = this.translationService.getValue('commercial-fishing.edit-permit-license-application-dialog-title');
                viewApplicationDataAndConfrimRegularityTitle = this.translationService.getValue('commercial-fishing.view-permit-license-appl-and-confirm-regularity-title');
            } break;
            case PageCodeEnum.DupRightToFishResource: {
                editDialog = forRegister
                    ? new TLMatDialog<EditCommercialFishingComponent>(this.matDialog)
                    : new TLMatDialog<DuplicatesApplicationComponent>(this.matDialog);
                editDialogTCtor = forRegister
                    ? EditCommercialFishingComponent
                    : DuplicatesApplicationComponent;
                viewDialogTitle = this.translationService.getValue('duplicates.view-permit-license-application-dialog-title');
                viewRegisterTitle = this.translationService.getValue('duplicates.view-permit-license-dialog-title');
                viewRegixDataTitle = this.translationService.getValue('duplicates.view-permit-license-application-regix-data-dialog-title');
                editRegixDataTitle = this.translationService.getValue('duplicates.edit-permit-license-application-regix-data-dialog-title');
                editApplicationDialogTitle = this.translationService.getValue('duplicates.edit-permit-license-application-dialog-title');
                viewApplicationDataAndConfrimRegularityTitle = this.translationService.getValue('duplicates.view-permit-license-appl-and-confirm-regularity-title');
            } break;
            case PageCodeEnum.PoundnetCommFishLic: {
                editDialog = new TLMatDialog<EditCommercialFishingComponent>(this.matDialog);
                editDialogTCtor = EditCommercialFishingComponent;
                viewDialogTitle = this.translationService.getValue('commercial-fishing.view-poundnet-permit-license-application-dialog-title');
                viewRegisterTitle = this.translationService.getValue('commercial-fishing.view-poundnet-permit-license-dialog-title');
                viewRegixDataTitle = this.translationService.getValue('commercial-fishing.view-poundnet-permit-license-application-regix-data-dialog-title');
                editRegixDataTitle = this.translationService.getValue('commercial-fishing.edit-poundnet-permit-license-application-regix-data-dialog-title');
                editApplicationDialogTitle = this.translationService.getValue('commercial-fishing.edit-poundnet-permit-license-application-dialog-title');
                viewApplicationDataAndConfrimRegularityTitle = this.translationService.getValue('commercial-fishing.view-poundnet-permit-license-appl-and-confirm-regularity-title');
            } break;
            case PageCodeEnum.DupPoundnetCommFishLic: {
                editDialog = forRegister
                    ? new TLMatDialog<EditCommercialFishingComponent>(this.matDialog)
                    : new TLMatDialog<DuplicatesApplicationComponent>(this.matDialog);
                editDialogTCtor = forRegister
                    ? EditCommercialFishingComponent
                    : DuplicatesApplicationComponent;
                viewDialogTitle = this.translationService.getValue('duplicates.view-poundnet-permit-license-application-dialog-title');
                viewRegisterTitle = this.translationService.getValue('duplicates.view-poundnet-permit-license-dialog-title');
                viewRegixDataTitle = this.translationService.getValue('duplicates.view-poundnet-permit-license-application-regix-data-dialog-title');
                editRegixDataTitle = this.translationService.getValue('duplicates.edit-poundnet-permit-license-application-regix-data-dialog-title');
                editApplicationDialogTitle = this.translationService.getValue('duplicates.edit-poundnet-permit-license-application-dialog-title');
                viewApplicationDataAndConfrimRegularityTitle = this.translationService.getValue('duplicates.view-poundnet-permit-license-appl-and-confirm-regularity-title');
            } break;
            case PageCodeEnum.CatchQuataSpecies: {
                editDialog = new TLMatDialog<EditCommercialFishingComponent>(this.matDialog);
                editDialogTCtor = EditCommercialFishingComponent;
                viewDialogTitle = this.translationService.getValue('commercial-fishing.view-quata-species-permit-license-application-dialog-title');
                viewRegisterTitle = this.translationService.getValue('commercial-fishing.view-quata-species-permit-license-dialog-title');
                viewRegixDataTitle = this.translationService.getValue('commercial-fishing.view-quata-species-permit-license-application-regix-data-dialog-title');
                editRegixDataTitle = this.translationService.getValue('commercial-fishing.edit-quata-species-permit-license-application-regix-data-dialog-title');
                editApplicationDialogTitle = this.translationService.getValue('commercial-fishing.edit-quata-species-permit-license-application-dialog-title');
                viewApplicationDataAndConfrimRegularityTitle = this.translationService.getValue('commercial-fishing.view-quata-species-permit-appl-and-confirm-regularity-title');
            } break;
            case PageCodeEnum.DupCatchQuataSpecies: {
                editDialog = forRegister
                    ? new TLMatDialog<EditCommercialFishingComponent>(this.matDialog)
                    : new TLMatDialog<DuplicatesApplicationComponent>(this.matDialog);
                editDialogTCtor = forRegister
                    ? EditCommercialFishingComponent
                    : DuplicatesApplicationComponent;
                viewDialogTitle = this.translationService.getValue('duplicates.view-quata-species-permit-license-application-dialog-title');
                viewRegisterTitle = this.translationService.getValue('duplicates.view-quata-species-permit-license-dialog-title');
                viewRegixDataTitle = this.translationService.getValue('duplicates.view-quata-species-permit-license-application-regix-data-dialog-title');
                editRegixDataTitle = this.translationService.getValue('duplicates.edit-quata-species-permit-license-application-regix-data-dialog-title');
                editApplicationDialogTitle = this.translationService.getValue('duplicates.edit-quata-species-permit-license-application-dialog-title');
                viewApplicationDataAndConfrimRegularityTitle = this.translationService.getValue('duplicates.view-quata-species-permit-license-appl-and-confirm-regularity-title');
            } break;
            case PageCodeEnum.LE: {
                editDialog = new TLMatDialog<EditLegalEntityComponent>(this.matDialog);
                editDialogTCtor = EditLegalEntityComponent;
                viewDialogTitle = this.translationService.getValue('legal-entities-page.view-legal-entity-dialog-title');
                viewRegisterTitle = this.translationService.getValue('legal-entities-page.legal-entity-register-title');
                viewRegixDataTitle = this.translationService.getValue('legal-entities-page.view-legal-entity-application-regix-data-dialog-title');
                editRegixDataTitle = this.translationService.getValue('legal-entities-page.edit-legal-entity-application-regix-data-dialog-title');
                editApplicationDialogTitle = this.translationService.getValue('legal-entities-page.edit-legal-entity-application-dialog-title');
                viewApplicationDataAndConfrimRegularityTitle = this.translationService.getValue('legal-entities-page.view-legal-entity-appl-and-confirm-regularity-title');
            } break;
            case PageCodeEnum.Assocs: {
                editDialog = new TLMatDialog<EditLegalAssociationComponent>(this.matDialog);
                editDialogTCtor = EditLegalAssociationComponent;
                viewDialogTitle = this.translationService.getValue('legal-association.view-association-dialog-title');
                viewRegisterTitle = this.translationService.getValue('legal-association.association-register-title');
                viewRegixDataTitle = this.translationService.getValue('legal-association.view-association-application-regix-data-dialog-title');
                editRegixDataTitle = this.translationService.getValue('legal-association.edit-association-application-regix-data-dialog-title');
                editApplicationDialogTitle = this.translationService.getValue('legal-association.edit-association-application-dialog-title');
                viewApplicationDataAndConfrimRegularityTitle = this.translationService.getValue('legal-association.view-association-appl-and-confirm-regularity-title');
            } break;
            case PageCodeEnum.RegVessel: {
                editDialog = new TLMatDialog<EditShipComponent>(this.matDialog);
                editDialogTCtor = EditShipComponent;
                viewDialogTitle = this.translationService.getValue('ships-register.view-ship-dialog-title');
                viewRegisterTitle = this.translationService.getValue('ships-register.view-ship-dialog-title');
                viewRegixDataTitle = this.translationService.getValue('ships-register.view-ship-application-regix-data-dialog-title');
                editRegixDataTitle = this.translationService.getValue('ships-register.edit-ship-application-regix-data-dialog-title');
                editApplicationDialogTitle = this.translationService.getValue('ships-register.edit-ship-application-dialog-title');
                viewApplicationDataAndConfrimRegularityTitle = this.translationService.getValue('ships-register.view-ship-appl-and-confirm-regularity-title');
            } break;
            case PageCodeEnum.ShipRegChange: {
                editDialog = forRegister
                    ? new TLMatDialog<EditShipComponent>(this.matDialog)
                    : new TLMatDialog<ShipChangeOfCircumstancesComponent>(this.matDialog);
                editDialogTCtor = forRegister
                    ? EditShipComponent
                    : ShipChangeOfCircumstancesComponent;
                editDialog = new TLMatDialog<ShipChangeOfCircumstancesComponent>(this.matDialog);
                editDialogTCtor = ShipChangeOfCircumstancesComponent;
                viewDialogTitle = this.translationService.getValue('ships-register.view-change-of-circumstances-dialog-title');
                viewRegisterTitle = this.translationService.getValue('ships-register.view-ship-dialog-title');
                viewRegixDataTitle = this.translationService.getValue('ships-register.view-change-of-circumstances-regix-data-dialog-title');
                editRegixDataTitle = this.translationService.getValue('ships-register.edit-change-of-circumstances-regix-data-dialog-title');
                editApplicationDialogTitle = this.translationService.getValue('ships-register.edit-change-of-circumstances-dialog-title');
                viewApplicationDataAndConfrimRegularityTitle = this.translationService.getValue('ships-register.view-change-of-circumstances-appl-and-confirm-regularity-title');
            } break;
            case PageCodeEnum.DeregShip: {
                editDialog = forRegister
                    ? new TLMatDialog<EditShipComponent>(this.matDialog)
                    : new TLMatDialog<ShipChangeOfCircumstancesComponent>(this.matDialog);
                editDialogTCtor = forRegister
                    ? EditShipComponent
                    : ShipChangeOfCircumstancesComponent;
                editDialog = new TLMatDialog<ShipChangeOfCircumstancesComponent>(this.matDialog);
                editDialogTCtor = ShipChangeOfCircumstancesComponent;
                viewDialogTitle = this.translationService.getValue('ships-register.view-deregistration-dialog-title');
                viewRegisterTitle = this.translationService.getValue('ships-register.view-ship-dialog-title');
                viewRegixDataTitle = this.translationService.getValue('ships-register.view-deregistration-regix-data-dialog-title');
                editRegixDataTitle = this.translationService.getValue('ships-register.edit-deregistration-regix-data-dialog-title');
                editApplicationDialogTitle = this.translationService.getValue('ships-register.edit-deregistration-dialog-title');
                viewApplicationDataAndConfrimRegularityTitle = this.translationService.getValue('ships-register.view-deregistration-appl-and-confirm-regularity-title');
            } break;
            case PageCodeEnum.IncreaseFishCap: {
                editDialog = forRegister
                    ? new TLMatDialog<EditShipComponent>(this.matDialog)
                    : new TLMatDialog<IncreaseFishingCapacityComponent>(this.matDialog);
                editDialogTCtor = forRegister
                    ? EditShipComponent
                    : IncreaseFishingCapacityComponent;
                viewDialogTitle = this.translationService.getValue('fishing-capacity.view-increase-capacity-application-dialog-title');
                viewRegisterTitle = this.translationService.getValue('ships-register.view-ship-dialog-title');
                viewRegixDataTitle = this.translationService.getValue('fishing-capacity.view-increase-capacity-regix-data-dialog-title');
                editRegixDataTitle = this.translationService.getValue('fishing-capacity.edit-increase-capacity-regix-data-dialog-title');
                editApplicationDialogTitle = this.translationService.getValue('fishing-capacity.edit-increase-capacity-application-dialog-title');
                viewApplicationDataAndConfrimRegularityTitle = this.translationService.getValue('fishing-capacity.view-increase-capacity-appl-and-confirm-regularity-title');
            } break;
            case PageCodeEnum.ReduceFishCap: {
                editDialog = forRegister
                    ? new TLMatDialog<EditShipComponent>(this.matDialog)
                    : new TLMatDialog<ReduceFishingCapacityComponent>(this.matDialog);
                editDialogTCtor = forRegister
                    ? EditShipComponent
                    : ReduceFishingCapacityComponent;
                viewDialogTitle = this.translationService.getValue('fishing-capacity.view-reduce-capacity-application-dialog-title');
                viewRegisterTitle = this.translationService.getValue('ships-register.view-ship-dialog-title');
                viewRegixDataTitle = this.translationService.getValue('fishing-capacity.view-reduce-capacity-regix-data-dialog-title');
                editRegixDataTitle = this.translationService.getValue('fishing-capacity.edit-reduce-capacity-regix-data-dialog-title');
                editApplicationDialogTitle = this.translationService.getValue('fishing-capacity.edit-reduce-capacity-application-dialog-title');
                viewApplicationDataAndConfrimRegularityTitle = this.translationService.getValue('fishing-capacity.view-reduce-capacity-appl-and-confirm-regularity-title');
            } break;
            case PageCodeEnum.TransferFishCap: {
                editDialog = new TLMatDialog<TransferFishingCapacityComponent>(this.matDialog);
                editDialogTCtor = TransferFishingCapacityComponent;
                viewDialogTitle = this.translationService.getValue('fishing-capacity.view-transfer-capacity-application-dialog-title');
                viewRegixDataTitle = this.translationService.getValue('fishing-capacity.view-transfer-capacity-regix-data-dialog-title');
                editRegixDataTitle = this.translationService.getValue('fishing-capacity.edit-transfer-capacity-regix-data-dialog-title');
                editApplicationDialogTitle = this.translationService.getValue('fishing-capacity.edit-transfer-capacity-application-dialog-title');
                viewApplicationDataAndConfrimRegularityTitle = this.translationService.getValue('fishing-capacity.view-transfer-capacity-appl-and-confirm-regularity-title');
            } break;
            case PageCodeEnum.CapacityCertDup: {
                editDialog = new TLMatDialog<CapacityCertificateDuplicateComponent>(this.matDialog);
                editDialogTCtor = CapacityCertificateDuplicateComponent;
                viewDialogTitle = this.translationService.getValue('fishing-capacity.view-duplicate-capacity-application-dialog-title');
                viewRegixDataTitle = this.translationService.getValue('fishing-capacity.view-duplicate-capacity-regix-data-dialog-title');
                editRegixDataTitle = this.translationService.getValue('fishing-capacity.edit-duplicate-capacity-regix-data-dialog-title');
                editApplicationDialogTitle = this.translationService.getValue('fishing-capacity.edit-duplicate-capacity-application-dialog-title');
                viewApplicationDataAndConfrimRegularityTitle = this.translationService.getValue('fishing-capacity.view-duplicate-capacity-appl-and-confirm-regularity-title');
            } break;
            case PageCodeEnum.AquaFarmReg: {
                editDialog = new TLMatDialog<EditAquacultureFacilityComponent>(this.matDialog);
                editDialogTCtor = EditAquacultureFacilityComponent;
                viewDialogTitle = this.translationService.getValue('aquacultures.view-aquaculture-application-dialog-title');
                viewRegisterTitle = this.translationService.getValue('aquacultures.view-aquaculture-dialog-title');
                viewRegixDataTitle = this.translationService.getValue('aquacultures.view-aquaculture-application-regix-data-dialog-title');
                editRegixDataTitle = this.translationService.getValue('aquacultures.edit-aquaculture-application-regix-data-dialog-title');
                editApplicationDialogTitle = this.translationService.getValue('aquacultures.edit-aquaculture-application-dialog-title');
                viewApplicationDataAndConfrimRegularityTitle = this.translationService.getValue('aquacultures.view-aquaculture-appl-and-confirm-regularity-title');
            } break;
            case PageCodeEnum.AquaFarmChange: {
                editDialog = forRegister
                    ? new TLMatDialog<EditAquacultureFacilityComponent>(this.matDialog)
                    : new TLMatDialog<AquacultureChangeOfCircumstancesComponent>(this.matDialog);
                editDialogTCtor = forRegister
                    ? EditAquacultureFacilityComponent
                    : AquacultureChangeOfCircumstancesComponent;
                viewDialogTitle = this.translationService.getValue('aquacultures.view-change-of-circumstances-dialog-title');
                viewRegisterTitle = this.translationService.getValue('aquacultures.view-aquaculture-dialog-title');
                viewRegixDataTitle = this.translationService.getValue('aquacultures.view-change-of-circumstances-regix-data-dialog-title');
                editRegixDataTitle = this.translationService.getValue('aquacultures.edit-change-of-circumstances-regix-data-dialog-title');
                editApplicationDialogTitle = this.translationService.getValue('aquacultures.edit-change-of-circumstances-dialog-title');
                viewApplicationDataAndConfrimRegularityTitle = this.translationService.getValue('aquacultures.view-aquaculture-change-of-circumstances-appl-and-confirm-regularity-title');
            } break;
            case PageCodeEnum.AquaFarmDereg: {
                editDialog = forRegister
                    ? new TLMatDialog<EditAquacultureFacilityComponent>(this.matDialog)
                    : new TLMatDialog<AquacultureDeregistrationComponent>(this.matDialog);
                editDialogTCtor = forRegister
                    ? EditAquacultureFacilityComponent
                    : AquacultureDeregistrationComponent;
                viewDialogTitle = this.translationService.getValue('aquacultures.view-deregistration-dialog-title');
                viewRegisterTitle = this.translationService.getValue('aquacultures.view-aquaculture-dialog-title');
                viewRegixDataTitle = this.translationService.getValue('aquacultures.view-deregistration-regix-data-dialog-title');
                editRegixDataTitle = this.translationService.getValue('aquacultures.edit-deregistration-regix-data-dialog-title');
                editApplicationDialogTitle = this.translationService.getValue('aquacultures.edit-deregistration-dialog-title');
                viewApplicationDataAndConfrimRegularityTitle = this.translationService.getValue('aquacultures.view-aquaculture-deregistration-appl-and-confirm-regularity-title');
            } break;
            case PageCodeEnum.CommFishLicense: {
                editDialog = new TLMatDialog<EditFisherComponent>(this.matDialog);
                editDialogTCtor = EditFisherComponent;
                viewDialogTitle = this.translationService.getValue('qualified-fishers-page.view-qualified-fisher-application-dialog-title');
                viewRegisterTitle = this.translationService.getValue('qualified-fishers-page.view-qualified-fisher-dialog-title');
                viewRegixDataTitle = this.translationService.getValue('qualified-fishers-page.view-qualified-fisher-application-regix-data-dialog-title');
                editRegixDataTitle = this.translationService.getValue('qualified-fishers-page.edit-qualified-fisher-application-regix-data-dialog-title');
                editApplicationDialogTitle = this.translationService.getValue('qualified-fishers-page.edit-qualified-fisher-application-dialog-title');
                viewApplicationDataAndConfrimRegularityTitle = this.translationService.getValue('qualified-fishers-page.view-qualified-fisher-appl-and-confirm-regularity-title');
            } break;
            case PageCodeEnum.CompetencyDup: {
                editDialog = forRegister
                    ? new TLMatDialog<EditFisherComponent>(this.matDialog)
                    : new TLMatDialog<DuplicatesApplicationComponent>(this.matDialog);
                editDialogTCtor = forRegister
                    ? EditFisherComponent
                    : DuplicatesApplicationComponent;
                viewDialogTitle = this.translationService.getValue('duplicates.view-qualified-fisher-application-dialog-title');
                viewRegisterTitle = this.translationService.getValue('duplicates.view-qualified-fisher-dialog-title');
                viewRegixDataTitle = this.translationService.getValue('duplicates.view-qualified-fisher-application-regix-data-dialog-title');
                editRegixDataTitle = this.translationService.getValue('duplicates.edit-qualified-fisher-application-regix-data-dialog-title');
                editApplicationDialogTitle = this.translationService.getValue('duplicates.edit-qualified-fisher-application-dialog-title');
                viewApplicationDataAndConfrimRegularityTitle = this.translationService.getValue('duplicates.view-qualified-fisher-appl-and-confirm-regularity-title');
            } break;
            case PageCodeEnum.RegFirstSaleCenter: {
                editDialog = new TLMatDialog<EditBuyersComponent>(this.matDialog);
                editDialogTCtor = EditBuyersComponent;
                viewDialogTitle = this.translationService.getValue('buyers-and-sales-centers.view-center-application-dialog-title');
                viewRegisterTitle = this.translationService.getValue('buyers-and-sales-centers.view-center-register-dialog-title');
                viewRegixDataTitle = this.translationService.getValue('buyers-and-sales-centers.view-center-application-regix-data-dialog-title');
                editRegixDataTitle = this.translationService.getValue('buyers-and-sales-centers.edit-center-application-regix-data-dialog-title');
                editApplicationDialogTitle = this.translationService.getValue('buyers-and-sales-centers.edit-center-application-dialog-title');
                viewApplicationDataAndConfrimRegularityTitle = this.translationService.getValue('buyers-and-sales-centers.view-center-appl-and-confirm-regularity-title');
            } break;
            case PageCodeEnum.RegFirstSaleBuyer: {
                editDialog = new TLMatDialog<EditBuyersComponent>(this.matDialog);
                editDialogTCtor = EditBuyersComponent;
                viewDialogTitle = this.translationService.getValue('buyers-and-sales-centers.view-buyer-application-dialog-title');
                viewRegisterTitle = this.translationService.getValue('buyers-and-sales-centers.view-buyer-register-dialog-title');
                viewRegixDataTitle = this.translationService.getValue('buyers-and-sales-centers.view-buyer-application-regix-data-dialog-title');
                editRegixDataTitle = this.translationService.getValue('buyers-and-sales-centers.edit-buyer-application-regix-data-dialog-title');
                editApplicationDialogTitle = this.translationService.getValue('buyers-and-sales-centers.edit-buyer-application-dialog-title');
                viewApplicationDataAndConfrimRegularityTitle = this.translationService.getValue('buyers-and-sales-centers.view-buyer-appl-and-confirm-regularity-title');
            } break;
            case PageCodeEnum.ChangeFirstSaleBuyer: {
                editDialog = forRegister
                    ? new TLMatDialog<EditBuyersComponent>(this.matDialog)
                    : new TLMatDialog<BuyerChangeOfCircumstancesComponent>(this.matDialog);
                editDialogTCtor = forRegister
                    ? EditBuyersComponent
                    : BuyerChangeOfCircumstancesComponent;
                viewDialogTitle = this.translationService.getValue('buyers-and-sales-centers.view-change-buyer-application-dialog-title');
                viewRegisterTitle = this.translationService.getValue('buyers-and-sales-centers.view-buyer-register-dialog-title');
                viewRegixDataTitle = this.translationService.getValue('buyers-and-sales-centers.view-change-buyer-application-regix-data-dialog-title');
                editRegixDataTitle = this.translationService.getValue('buyers-and-sales-centers.edit-change-buyer-application-regix-data-dialog-title');
                editApplicationDialogTitle = this.translationService.getValue('buyers-and-sales-centers.edit-change-buyer-application-dialog-title');
                viewApplicationDataAndConfrimRegularityTitle = this.translationService.getValue('buyers-and-sales-centers.view-change-buyer-appl-and-confirm-regularity-title');
            } break;
            case PageCodeEnum.TermFirstSaleBuyer: {
                editDialog = forRegister
                    ? new TLMatDialog<EditBuyersComponent>(this.matDialog)
                    : new TLMatDialog<BuyerTerminationComponent>(this.matDialog);
                editDialogTCtor = forRegister
                    ? EditBuyersComponent
                    : BuyerTerminationComponent;
                viewDialogTitle = this.translationService.getValue('buyers-and-sales-centers.view-terminate-buyer-application-dialog-title');
                viewRegisterTitle = this.translationService.getValue('buyers-and-sales-centers.view-buyer-register-dialog-title');
                viewRegixDataTitle = this.translationService.getValue('buyers-and-sales-centers.view-terminate-buyer-application-regix-data-dialog-title');
                editRegixDataTitle = this.translationService.getValue('buyers-and-sales-centers.edit-terminate-buyer-application-regix-data-dialog-title');
                editApplicationDialogTitle = this.translationService.getValue('buyers-and-sales-centers.edit-terminate-buyer-application-dialog-title');
                viewApplicationDataAndConfrimRegularityTitle = this.translationService.getValue('buyers-and-sales-centers.view-terminate-buyer-appl-and-confirm-regularity-title');
            } break;
            case PageCodeEnum.ChangeFirstSaleCenter: {
                editDialog = forRegister
                    ? new TLMatDialog<EditBuyersComponent>(this.matDialog)
                    : new TLMatDialog<BuyerChangeOfCircumstancesComponent>(this.matDialog);
                editDialogTCtor = forRegister
                    ? EditBuyersComponent
                    : BuyerChangeOfCircumstancesComponent;
                viewDialogTitle = this.translationService.getValue('buyers-and-sales-centers.view-first-sale-application-dialog-title');
                viewRegisterTitle = this.translationService.getValue('buyers-and-sales-centers.view-buyer-register-dialog-title');
                viewRegixDataTitle = this.translationService.getValue('buyers-and-sales-centers.view-change-first-sale-application-regix-data-dialog-title');
                editRegixDataTitle = this.translationService.getValue('buyers-and-sales-centers.edit-change-first-sale-application-regix-data-dialog-title');
                editApplicationDialogTitle = this.translationService.getValue('buyers-and-sales-centers.edit-change-first-sale-application-dialog-title');
                viewApplicationDataAndConfrimRegularityTitle = this.translationService.getValue('buyers-and-sales-centers.view-first-sale-center-appl-and-confirm-regularity-title');
            } break;
            case PageCodeEnum.TermFirstSaleCenter: {
                editDialog = forRegister
                    ? new TLMatDialog<EditBuyersComponent>(this.matDialog)
                    : new TLMatDialog<BuyerTerminationComponent>(this.matDialog);
                editDialogTCtor = forRegister
                    ? EditBuyersComponent
                    : BuyerTerminationComponent;
                viewDialogTitle = this.translationService.getValue('buyers-and-sales-centers.view-terminate-center-application-dialog-title');
                viewRegisterTitle = this.translationService.getValue('buyers-and-sales-centers.view-buyer-register-dialog-title');
                viewRegixDataTitle = this.translationService.getValue('buyers-and-sales-centers.view-terminate-center-application-regix-data-dialog-title');
                editRegixDataTitle = this.translationService.getValue('buyers-and-sales-centers.edit-terminate-center-application-regix-data-dialog-title');
                editApplicationDialogTitle = this.translationService.getValue('buyers-and-sales-centers.edit-terminate-center-application-dialog-title');
                viewApplicationDataAndConfrimRegularityTitle = this.translationService.getValue('buyers-and-sales-centers.view-terminate-center-appl-and-confirm-regularity-title');
            } break;
            case PageCodeEnum.DupFirstSaleCenter: {
                editDialog = forRegister
                    ? new TLMatDialog<EditBuyersComponent>(this.matDialog)
                    : new TLMatDialog<DuplicatesApplicationComponent>(this.matDialog);
                editDialogTCtor = forRegister
                    ? EditBuyersComponent
                    : DuplicatesApplicationComponent;
                viewDialogTitle = this.translationService.getValue('duplicates.view-center-application-dialog-title');
                viewRegisterTitle = this.translationService.getValue('duplicates.view-center-register-dialog-title');
                viewRegixDataTitle = this.translationService.getValue('duplicates.view-center-application-regix-data-dialog-title');
                editRegixDataTitle = this.translationService.getValue('duplicates.edit-center-application-regix-data-dialog-title');
                editApplicationDialogTitle = this.translationService.getValue('duplicates.edit-center-application-dialog-title');
                viewApplicationDataAndConfrimRegularityTitle = this.translationService.getValue('duplicates.view-center-appl-and-confirm-regularity-title');
            } break;
            case PageCodeEnum.DupFirstSaleBuyer: {
                editDialog = forRegister
                    ? new TLMatDialog<EditBuyersComponent>(this.matDialog)
                    : new TLMatDialog<DuplicatesApplicationComponent>(this.matDialog);
                editDialogTCtor = forRegister
                    ? EditBuyersComponent
                    : DuplicatesApplicationComponent;
                viewDialogTitle = this.translationService.getValue('duplicates.view-buyer-application-dialog-title');
                viewRegisterTitle = this.translationService.getValue('duplicates.view-buyer-register-dialog-title');
                viewRegixDataTitle = this.translationService.getValue('duplicates.view-buyer-application-regix-data-dialog-title');
                editRegixDataTitle = this.translationService.getValue('duplicates.edit-buyer-application-regix-data-dialog-title');
                editApplicationDialogTitle = this.translationService.getValue('duplicates.edit-buyer-application-dialog-title');
                viewApplicationDataAndConfrimRegularityTitle = this.translationService.getValue('duplicates.view-buyer-appl-and-confirm-regularity-title');
            } break;
            case PageCodeEnum.StatFormAquaFarm: {
                editDialog = new TLMatDialog<StatisticalFormsAquaFarmComponent>(this.matDialog);
                editDialogTCtor = StatisticalFormsAquaFarmComponent;
                viewDialogTitle = this.translationService.getValue('statistical-forms.view-statistical-form-aqua-farm-dialog-title');
                viewRegisterTitle = this.translationService.getValue('statistical-forms.aqua-farm-view-register-dialog-title');
                viewRegixDataTitle = this.translationService.getValue('statistical-forms.aqua-farm-view-regix-data-dialog-title');
                editRegixDataTitle = this.translationService.getValue('statistical-forms.aqua-farm-edit-regix-data-dialog-title');
                editApplicationDialogTitle = this.translationService.getValue('statistical-forms.aqua-farm-edit-application-dialog-title');
                viewApplicationDataAndConfrimRegularityTitle = this.translationService.getValue('statistical-forms.aqua-farm-view-and-confrim-data-regularity-title');
            } break;
            case PageCodeEnum.StatFormRework: {
                editDialog = new TLMatDialog<StatisticalFormsReworkComponent>(this.matDialog);
                editDialogTCtor = StatisticalFormsReworkComponent;
                viewDialogTitle = this.translationService.getValue('statistical-forms.view-statistical-form-rework-dialog-title');
                viewRegisterTitle = this.translationService.getValue('statistical-forms.rework-view-register-dialog-title');
                viewRegixDataTitle = this.translationService.getValue('statistical-forms.rework-view-regix-data-dialog-title');
                editRegixDataTitle = this.translationService.getValue('statistical-forms.rework-edit-regix-data-dialog-title');
                editApplicationDialogTitle = this.translationService.getValue('statistical-forms.rework-edit-application-dialog-title');
                viewApplicationDataAndConfrimRegularityTitle = this.translationService.getValue('statistical-forms.rework-view-and-confrim-data-regularity-title');
            } break;
            case PageCodeEnum.StatFormFishVessel: {
                editDialog = new TLMatDialog<StatisticalFormsFishVesselComponent>(this.matDialog);
                editDialogTCtor = StatisticalFormsFishVesselComponent;
                viewDialogTitle = this.translationService.getValue('statistical-forms.view-statistical-form-fish-vessel-dialog-title');
                viewRegisterTitle = this.translationService.getValue('statistical-forms.view-stat-form-fish-vessel-dialog-title');
                viewRegixDataTitle = this.translationService.getValue('statistical-forms.add-stat-form-fish-vessel-regix-data-dialog-title');
                editRegixDataTitle = this.translationService.getValue('statistical-forms.edit-stat-form-fish-vessel-regix-data-dialog-title');
                editApplicationDialogTitle = this.translationService.getValue('statistical-forms.edit-stat-form-fish-vessel-application-dialog-title');
                viewApplicationDataAndConfrimRegularityTitle = this.translationService.getValue('statistical-forms.view-and-confirm-stat-form-fish-vessel-dialog-title');
            } break;
            default:
                return null;
        }

        return new EditDialogInfo({
            editDialog: editDialog,
            editDialogTCtor: editDialogTCtor,
            viewTitle: viewDialogTitle,
            viewRegisterTitle: viewRegisterTitle,
            viewRegixDataTitle: viewRegixDataTitle,
            editRegixDataTitle: editRegixDataTitle,
            editApplicationTitle: editApplicationDialogTitle,
            viewApplicationDataAndConfrimRegularityTitle: viewApplicationDataAndConfrimRegularityTitle
        });
    }

    private getServiceInstanceForPageCode(pageCode: PageCodeEnum): IApplicationsActionsService {
        let service: IApplicationsActionsService | undefined = undefined;

        switch (pageCode) {
            case PageCodeEnum.SciFi: {
                if (this.pageType === 'PublicPage') {
                    service = this.injector.get(ScientificFishingPublicService);
                }
                else {
                    service = this.injector.get(ScientificFishingAdministrationService);
                }
            } break;
            case PageCodeEnum.CommFish:
            case PageCodeEnum.RightToFishThirdCountry:
            case PageCodeEnum.PoundnetCommFish:
            case PageCodeEnum.RightToFishResource:
            case PageCodeEnum.PoundnetCommFishLic:
            case PageCodeEnum.CatchQuataSpecies: {
                if (this.pageType === 'PublicPage') {
                    service = this.injector.get(CommercialFishingPublicService);
                }
                else {
                    service = this.injector.get(CommercialFishingAdministrationService);
                }
            } break;
            case PageCodeEnum.LE: {
                if (this.pageType === 'PublicPage') {
                    service = this.injector.get(LegalEntitiesPublicService);
                }
                else {
                    service = this.injector.get(LegalEntitiesAdministrationService);
                }
            } break;
            case PageCodeEnum.Assocs: {
                if (this.pageType === 'PublicPage') {
                    service = this.injector.get(RecreationalFishingAssociationPublicService);
                }
                else {
                    service = this.injector.get(RecreationalFishingAssociationService);
                }
            } break;
            case PageCodeEnum.RegVessel:
            case PageCodeEnum.ShipRegChange:
            case PageCodeEnum.DeregShip: {
                if (this.pageType === 'PublicPage') {
                    service = this.injector.get(ShipsRegisterPublicService);
                }
                else {
                    service = this.injector.get(ShipsRegisterAdministrationService);
                }
            } break;
            case PageCodeEnum.IncreaseFishCap:
            case PageCodeEnum.ReduceFishCap:
            case PageCodeEnum.TransferFishCap:
            case PageCodeEnum.CapacityCertDup: {
                if (this.pageType === 'PublicPage') {
                    service = this.injector.get(FishingCapacityPublicService);
                }
                else {
                    service = this.injector.get(FishingCapacityAdministrationService);
                }
            } break;
            case PageCodeEnum.CommFishLicense: {
                if (this.pageType === 'PublicPage') {
                    service = this.injector.get(QualifiedFishersPublicService);
                }
                else {
                    service = this.injector.get(QualifiedFishersService);
                }
            } break;
            case PageCodeEnum.RegFirstSaleCenter:
            case PageCodeEnum.RegFirstSaleBuyer:
            case PageCodeEnum.ChangeFirstSaleBuyer:
            case PageCodeEnum.TermFirstSaleBuyer:
            case PageCodeEnum.ChangeFirstSaleCenter:
            case PageCodeEnum.TermFirstSaleCenter: {
                if (this.pageType === 'PublicPage') {
                    service = this.injector.get(BuyersPublicService);
                }
                else {
                    service = this.injector.get(BuyersAdministrationService);
                }
            } break;
            case PageCodeEnum.StatFormAquaFarm:
            case PageCodeEnum.StatFormRework:
            case PageCodeEnum.StatFormFishVessel: {
                if (this.pageType === 'PublicPage') {
                    service = this.injector.get(StatisticalFormsPublicService);
                }
                else {
                    service = this.injector.get(StatisticalFormsAdministrationService);
                }
            } break;
            case PageCodeEnum.AquaFarmReg:
            case PageCodeEnum.AquaFarmChange:
            case PageCodeEnum.AquaFarmDereg: {
                if (this.pageType === 'PublicPage') {
                    service = this.injector.get(AquacultureFacilitiesPublicService);
                }
                else {
                    service = this.injector.get(AquacultureFacilitiesAdministrationService);
                }
            } break;
            case PageCodeEnum.DupFirstSaleBuyer:
            case PageCodeEnum.DupFirstSaleCenter:
            case PageCodeEnum.DupCommFish:
            case PageCodeEnum.DupRightToFishThirdCountry:
            case PageCodeEnum.DupPoundnetCommFish:
            case PageCodeEnum.DupRightToFishResource:
            case PageCodeEnum.DupPoundnetCommFishLic:
            case PageCodeEnum.DupCatchQuataSpecies:
            case PageCodeEnum.CompetencyDup: {
                if (this.pageType === 'PublicPage') {
                    service = this.injector.get(DuplicatesRegisterPublicService);
                }
                else {
                    service = this.injector.get(DuplicatesRegisterAdministrationService);
                }
            } break;
            default: {
                throw new Error(`pageCode ${pageCode} is not implemented in getServiceInstanceForPageCode.`);
            } break;
        }

        return service!;
    }

    private closePaymentDialogBtnClicked(closeFn: HeaderCloseFunction): void {
        closeFn();
    }

    private navigateByUrl(url: string, accessCode: string): void {
        this.router.navigateByUrl(url, { state: { accessCode: accessCode } });
    }
}