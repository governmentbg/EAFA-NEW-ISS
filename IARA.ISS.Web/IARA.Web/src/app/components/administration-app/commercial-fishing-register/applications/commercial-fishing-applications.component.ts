﻿import { Component } from '@angular/core';
import { Observable, Subject } from 'rxjs';

import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { PageCodeEnum } from '@app/enums/page-code.enum';
import { IApplicationsService } from '@app/interfaces/administration-app/applications.interface';
import { ICommercialFishingService } from '@app/interfaces/common-app/commercial-fishing.interface';
import { ApplicationProcessingPermissions } from '@app/models/common/application-processing-permissions.model';
import { ApplicationsRegisterData } from '@app/models/common/applications-register-data.model';
import { ApplicationsAdministrationService } from '@app/services/administration-app/applications-administration.service';
import { CommercialFishingAdministrationService } from '@app/services/administration-app/commercial-fishing-administration.service';
import { TLMatDialog } from '@app/shared/components/dialog-wrapper/tl-mat-dialog';
import { PermissionsEnum } from '@app/shared/enums/permissions.enum';
import { EditCommercialFishingComponent } from '@app/components/common-app/commercial-fishing/components/edit-commercial-fishing/edit-commercial-fishing.component';
import { DuplicatesApplicationComponent } from '@app/components/common-app/duplicates/duplicates-application.component';
import { IDuplicatesRegisterService } from '@app/interfaces/common-app/duplicates-register.interface';
import { DuplicatesRegisterAdministrationService } from '@app/services/administration-app/duplicates-register-administration.service';
import { ApplicationRegisterDTO } from '@app/models/generated/dtos/ApplicationRegisterDTO';
import { DialogParamsModel } from '@app/models/common/dialog-params.model';
import { HeaderCloseFunction } from '@app/shared/components/dialog-wrapper/interfaces/header-cancel-button.interface';
import { EditPermitLicenseFishingGearsComponent } from '@app/components/common-app/commercial-fishing/components/permit-license-fishing-gears/edit-permit-license-fishing-gears.component';
import { CommercialFishingEditDTO } from '@app/models/generated/dtos/CommercialFishingEditDTO';
import { IHeaderAuditButton } from '@app/shared/components/dialog-wrapper/interfaces/header-audit-button.interface';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { CommercialFishingDialogParamsModel } from '@app/components/common-app/commercial-fishing/models/commercial-fishing-dialog-params.model';

type CommFishingApplicationsRegisterDataType =
    ApplicationsRegisterData<EditCommercialFishingComponent> |
    ApplicationsRegisterData<DuplicatesApplicationComponent> |
    ApplicationsRegisterData<EditPermitLicenseFishingGearsComponent>;

@Component({
    selector: 'commercial-fishing-applications',
    templateUrl: './commercial-fishing-applications.component.html'
})
export class CommercialFishingApplicationsComponent {
    public service: ICommercialFishingService;
    public applicationsService: IApplicationsService;
    public processingPermissions: Map<PageCodeEnum, ApplicationProcessingPermissions>;
    public applicationsRegisterData!: Map<PageCodeEnum, CommFishingApplicationsRegisterDataType>;

    private readonly duplicatesService: IDuplicatesRegisterService;
    private readonly translate: FuseTranslationLoaderService;
    private readonly dupDialog: TLMatDialog<DuplicatesApplicationComponent>;
    private readonly editDialog: TLMatDialog<EditCommercialFishingComponent>;

    public constructor(
        service: CommercialFishingAdministrationService,
        duplicatesService: DuplicatesRegisterAdministrationService,
        translate: FuseTranslationLoaderService,
        applicationsService: ApplicationsAdministrationService,
        editDialog: TLMatDialog<EditCommercialFishingComponent>,
        dupDialog: TLMatDialog<DuplicatesApplicationComponent>,
        fishingGearsDialog: TLMatDialog<EditPermitLicenseFishingGearsComponent>,
        translationService: FuseTranslationLoaderService
    ) {
        this.service = service;
        this.applicationsService = applicationsService;
        this.translate = translate;
        this.duplicatesService = duplicatesService;
        this.dupDialog = dupDialog;
        this.editDialog = editDialog;

        const permitProcessingPermissions: ApplicationProcessingPermissions = new ApplicationProcessingPermissions({
            addPermission: PermissionsEnum.CommercialFishingPermitApplicationsAddRecords,
            editPermission: PermissionsEnum.CommercialFishingPermitApplicationsEditRecords,
            deletePermission: PermissionsEnum.CommercialFishingPermitApplicationsDeleteRecords,
            restorePermission: PermissionsEnum.CommercialFishingPermitApplicationsRestoreRecords,
            cancelPermssion: PermissionsEnum.CommercialFishingPermitApplicationsCancel,
            inspectAndCorrectPermssion: PermissionsEnum.CommercialFishingPermitApplicationsInspectAndCorrectRegiXData,
            processPaymentDataPermission: PermissionsEnum.CommercialFishingPermitApplicationsProcessPaymentData,
            checkDataRegularityPermission: PermissionsEnum.CommercialFishingPermitApplicationsCheckDataRegularity,
            addAdministrativeActPermission: PermissionsEnum.CommercialFishingPermitRegisterAddRecords,
            readAdministrativeActPermission: PermissionsEnum.CommercialFishingPermitRegisterRead,
            canReAssignApplicationsPermission: PermissionsEnum.ReAssignApplication,
            canReassingToDifferentTerritoryUnitPermission: PermissionsEnum.ApplicationsReassingToDifferentTerritoryUnit
        });

        const permitLicenseProcessingPermissions: ApplicationProcessingPermissions = new ApplicationProcessingPermissions({
            addPermission: PermissionsEnum.CommercialFishingPermitLicenseApplicationsAddRecords,
            editPermission: PermissionsEnum.CommercialFishingPermitLicenseApplicationsEditRecords,
            deletePermission: PermissionsEnum.CommercialFishingPermitLicenseApplicationsDeleteRecords,
            restorePermission: PermissionsEnum.CommercialFishingPermitLicenseApplicationsRestoreRecords,
            cancelPermssion: PermissionsEnum.CommercialFishingPermitLicenseApplicationsCancel,
            inspectAndCorrectPermssion: PermissionsEnum.CommercialFishingPermitLicenseApplicationsInspectAndCorrectRegiXData,
            processPaymentDataPermission: PermissionsEnum.CommercialFishingPermitLicenseApplicationsProcessPaymentData,
            checkDataRegularityPermission: PermissionsEnum.CommercialFishingPermitLicenseApplicationsCheckDataRegularity,
            addAdministrativeActPermission: PermissionsEnum.CommercialFishingPermitLicenseRegisterAddRecords,
            readAdministrativeActPermission: PermissionsEnum.CommercialFishingPermitLicenseRegisterRead,
            canReAssignApplicationsPermission: PermissionsEnum.ReAssignApplication,
            canReassingToDifferentTerritoryUnitPermission: PermissionsEnum.ApplicationsReassingToDifferentTerritoryUnit
        });

        this.processingPermissions = new Map<PageCodeEnum, ApplicationProcessingPermissions>([
            [
                PageCodeEnum.CommFish,
                permitProcessingPermissions
            ],
            [
                PageCodeEnum.PoundnetCommFish,
                permitProcessingPermissions
            ],
            [
                PageCodeEnum.RightToFishThirdCountry,
                permitProcessingPermissions
            ],
            [
                PageCodeEnum.DupCommFish,
                permitProcessingPermissions
            ],
            [
                PageCodeEnum.DupRightToFishThirdCountry,
                permitProcessingPermissions
            ],
            [
                PageCodeEnum.DupPoundnetCommFish,
                permitProcessingPermissions
            ],
            [
                PageCodeEnum.RightToFishResource,
                permitLicenseProcessingPermissions
            ],
            [
                PageCodeEnum.PoundnetCommFishLic,
                permitLicenseProcessingPermissions
            ],
            [
                PageCodeEnum.CatchQuataSpecies,
                permitLicenseProcessingPermissions
            ],
            [
                PageCodeEnum.DupRightToFishResource,
                permitLicenseProcessingPermissions
            ],
            [
                PageCodeEnum.DupPoundnetCommFishLic,
                permitLicenseProcessingPermissions
            ],
            [
                PageCodeEnum.DupCatchQuataSpecies,
                permitLicenseProcessingPermissions
            ],
            [
                PageCodeEnum.FishingGearsCommFish,
                permitLicenseProcessingPermissions
            ]
        ]);

        this.applicationsRegisterData = new Map<PageCodeEnum, CommFishingApplicationsRegisterDataType>([
            [
                PageCodeEnum.CommFish,
                new ApplicationsRegisterData<EditCommercialFishingComponent>({
                    editDialog: editDialog,
                    editDialogTCtor: EditCommercialFishingComponent,
                    addRegisterDialogTitle: translationService.getValue('commercial-fishing.add-permit-dialog-title'),
                    editApplicationDialogTitle: translationService.getValue('commercial-fishing.edit-permit-application-dialog-title'),
                    editRegixDataDialogTitle: translationService.getValue('commercial-fishing.edit-permit-application-regix-data-dialog-title'),
                    viewApplicationDialogTitle: translationService.getValue('commercial-fishing.view-permit-application-dialog-title'),
                    viewRegisterDialogTitle: translationService.getValue('commercial-fishing.view-permit-dialog-title'),
                    viewRegixDataDialogTitle: translationService.getValue('commercial-fishing.view-permit-application-regix-data-dialog-title'),
                    viewAndConfrimDataRegularityTitle: translationService.getValue('commercial-fishing.view-permit-appl-and-confirm-regularity-title'),
                    customRightSideActionButtons: [{
                        id: 'save-and-start-permit-license',
                        color: 'accent',
                        translateValue: 'commercial-fishing.save-and-start-permit-license'
                    }]
                })
            ],
            [
                PageCodeEnum.RightToFishResource,
                new ApplicationsRegisterData<EditCommercialFishingComponent>({
                    editDialog: editDialog,
                    editDialogTCtor: EditCommercialFishingComponent,
                    addRegisterDialogTitle: translationService.getValue('commercial-fishing.add-permit-license-dialog-title'),
                    editApplicationDialogTitle: translationService.getValue('commercial-fishing.edit-permit-license-application-dialog-title'),
                    editRegixDataDialogTitle: translationService.getValue('commercial-fishing.edit-permit-license-application-regix-data-dialog-title'),
                    viewApplicationDialogTitle: translationService.getValue('commercial-fishing.view-permit-license-application-dialog-title'),
                    viewRegisterDialogTitle: translationService.getValue('commercial-fishing.view-permit-license-dialog-title'),
                    viewRegixDataDialogTitle: translationService.getValue('commercial-fishing.view-permit-license-application-regix-data-dialog-title'),
                    viewAndConfrimDataRegularityTitle: translationService.getValue('commercial-fishing.view-permit-license-appl-and-confirm-regularity-title'),
                    customRightSideActionButtons: [{
                        id: 'copy-data-from-old-permit-license',
                        color: 'accent',
                        translateValue: 'commercial-fishing.copy-data-from-old-permit-license'
                    }]
                })
            ],
            [
                PageCodeEnum.PoundnetCommFish,
                new ApplicationsRegisterData<EditCommercialFishingComponent>({
                    editDialog: editDialog,
                    editDialogTCtor: EditCommercialFishingComponent,
                    addRegisterDialogTitle: translationService.getValue('commercial-fishing.add-poundnet-permit-dialog-title'),
                    editApplicationDialogTitle: translationService.getValue('commercial-fishing.edit-poundnet-permit-application-dialog-title'),
                    editRegixDataDialogTitle: translationService.getValue('commercial-fishing.edit-poundnet-permit-application-regix-data-dialog-title'),
                    viewApplicationDialogTitle: translationService.getValue('commercial-fishing.view-poundnet-permit-application-dialog-title'),
                    viewRegisterDialogTitle: translationService.getValue('commercial-fishing.view-poundnet-permit-dialog-title'),
                    viewRegixDataDialogTitle: translationService.getValue('commercial-fishing.view-poundnet-permit-application-regix-data-dialog-title'),
                    viewAndConfrimDataRegularityTitle: translationService.getValue('commercial-fishing.view-poundnet-permit-appl-and-confirm-regularity-title')
                })
            ],
            [
                PageCodeEnum.PoundnetCommFishLic,
                new ApplicationsRegisterData<EditCommercialFishingComponent>({
                    editDialog: editDialog,
                    editDialogTCtor: EditCommercialFishingComponent,
                    addRegisterDialogTitle: translationService.getValue('commercial-fishing.add-poundnet-permit-license-dialog-title'),
                    editApplicationDialogTitle: translationService.getValue('commercial-fishing.edit-poundnet-permit-license-application-dialog-title'),
                    editRegixDataDialogTitle: translationService.getValue('commercial-fishing.edit-poundnet-permit-license-application-regix-data-dialog-title'),
                    viewApplicationDialogTitle: translationService.getValue('commercial-fishing.view-poundnet-permit-license-application-dialog-title'),
                    viewRegisterDialogTitle: translationService.getValue('commercial-fishing.view-poundnet-permit-license-dialog-title'),
                    viewRegixDataDialogTitle: translationService.getValue('commercial-fishing.view-poundnet-permit-license-application-regix-data-dialog-title'),
                    viewAndConfrimDataRegularityTitle: translationService.getValue('commercial-fishing.view-poundnet-permit-license-appl-and-confirm-regularity-title'),
                    customRightSideActionButtons: [{
                        id: 'copy-data-from-old-permit-license',
                        color: 'accent',
                        translateValue: 'commercial-fishing.copy-data-from-old-permit-license'
                    }, {
                        id: 'copy-data-from-permit',
                        color: 'accent',
                        translateValue: 'commercial-fishing.copy-data-from-permit'
                    }]
                })
            ],
            [
                PageCodeEnum.CatchQuataSpecies,
                new ApplicationsRegisterData<EditCommercialFishingComponent>({
                    editDialog: editDialog,
                    editDialogTCtor: EditCommercialFishingComponent,
                    addRegisterDialogTitle: translationService.getValue('commercial-fishing.add-quata-species-permit-license-dialog-title'),
                    editApplicationDialogTitle: translationService.getValue('commercial-fishing.edit-quata-species-permit-license-application-dialog-title'),
                    editRegixDataDialogTitle: translationService.getValue('commercial-fishing.edit-quata-species-permit-license-application-regix-data-dialog-title'),
                    viewApplicationDialogTitle: translationService.getValue('commercial-fishing.view-quata-species-permit-license-application-dialog-title'),
                    viewRegisterDialogTitle: translationService.getValue('commercial-fishing.view-quata-species-permit-license-dialog-title'),
                    viewRegixDataDialogTitle: translationService.getValue('commercial-fishing.view-quata-species-permit-license-application-regix-data-dialog-title'),
                    viewAndConfrimDataRegularityTitle: translationService.getValue('commercial-fishing.view-quata-species-permit-appl-and-confirm-regularity-title'),
                    customRightSideActionButtons: [{
                        id: 'copy-data-from-old-permit-license',
                        color: 'accent',
                        translateValue: 'commercial-fishing.copy-data-from-old-permit-license'
                    }]
                })
            ],
            [
                PageCodeEnum.RightToFishThirdCountry,
                new ApplicationsRegisterData<EditCommercialFishingComponent>({
                    editDialog: editDialog,
                    editDialogTCtor: EditCommercialFishingComponent,
                    addRegisterDialogTitle: translationService.getValue('commercial-fishing.add-3rd-country-permit-dialog-title'),
                    editApplicationDialogTitle: translationService.getValue('commercial-fishing.edit-3rd-country-permit-application-dialog-title'),
                    editRegixDataDialogTitle: translationService.getValue('commercial-fishing.edit-3rd-country-permit-application-regix-data-dialog-title'),
                    viewApplicationDialogTitle: translationService.getValue('commercial-fishing.view-3rd-country-permit-application-dialog-title'),
                    viewRegisterDialogTitle: translationService.getValue('commercial-fishing.view-3rd-country-permit-dialog-title'),
                    viewRegixDataDialogTitle: translationService.getValue('commercial-fishing.view-3rd-country-permit-application-regix-data-dialog-title'),
                    viewAndConfrimDataRegularityTitle: translationService.getValue('commercial-fishing.view-3rd-country-permit-appl-and-confirm-regularity-title')
                })
            ],
            [
                PageCodeEnum.DupCommFish,
                new ApplicationsRegisterData<DuplicatesApplicationComponent>({
                    editDialog: dupDialog,
                    editDialogTCtor: DuplicatesApplicationComponent,
                    addRegisterDialogTitle: translationService.getValue('duplicates.add-permit-dialog-title'),
                    editApplicationDialogTitle: translationService.getValue('duplicates.edit-permit-application-dialog-title'),
                    editRegixDataDialogTitle: translationService.getValue('duplicates.edit-permit-application-regix-data-dialog-title'),
                    viewApplicationDialogTitle: translationService.getValue('duplicates.view-permit-application-dialog-title'),
                    viewRegisterDialogTitle: translationService.getValue('duplicates.view-permit-dialog-title'),
                    viewRegixDataDialogTitle: translationService.getValue('duplicates.view-permit-application-regix-data-dialog-title'),
                    viewAndConfrimDataRegularityTitle: translationService.getValue('duplicates.view-permit-appl-and-confirm-regularity-title'),
                    createRegisterCallback: this.openCommFishDuplicateDialog.bind(this)
                })
            ],
            [
                PageCodeEnum.DupRightToFishThirdCountry,
                new ApplicationsRegisterData<DuplicatesApplicationComponent>({
                    editDialog: dupDialog,
                    editDialogTCtor: DuplicatesApplicationComponent,
                    addRegisterDialogTitle: translationService.getValue('duplicates.add-3rd-country-permit-dialog-title'),
                    editApplicationDialogTitle: translationService.getValue('duplicates.edit-3rd-country-permit-application-dialog-title'),
                    editRegixDataDialogTitle: translationService.getValue('duplicates.edit-3rd-country-permit-application-regix-data-dialog-title'),
                    viewApplicationDialogTitle: translationService.getValue('duplicates.view-3rd-country-permit-application-dialog-title'),
                    viewRegisterDialogTitle: translationService.getValue('duplicates.view-3rd-country-permit-dialog-title'),
                    viewRegixDataDialogTitle: translationService.getValue('duplicates.view-3rd-country-permit-application-regix-data-dialog-title'),
                    viewAndConfrimDataRegularityTitle: translationService.getValue('duplicates.view-3rd-country-permit-appl-and-confirm-regularity-title'),
                    createRegisterCallback: this.openRightToFishThirdDuplicateDialog.bind(this)
                })
            ],
            [
                PageCodeEnum.DupPoundnetCommFish,
                new ApplicationsRegisterData<DuplicatesApplicationComponent>({
                    editDialog: dupDialog,
                    editDialogTCtor: DuplicatesApplicationComponent,
                    addRegisterDialogTitle: translationService.getValue('duplicates.add-poundnet-permit-dialog-title'),
                    editApplicationDialogTitle: translationService.getValue('duplicates.edit-poundnet-permit-application-dialog-title'),
                    editRegixDataDialogTitle: translationService.getValue('duplicates.edit-poundnet-permit-application-regix-data-dialog-title'),
                    viewApplicationDialogTitle: translationService.getValue('duplicates.view-poundnet-permit-application-dialog-title'),
                    viewRegisterDialogTitle: translationService.getValue('duplicates.view-poundnet-permit-dialog-title'),
                    viewRegixDataDialogTitle: translationService.getValue('duplicates.view-poundnet-permit-application-regix-data-dialog-title'),
                    viewAndConfrimDataRegularityTitle: translationService.getValue('duplicates.view-poundnet-permit-appl-and-confirm-regularity-title'),
                    createRegisterCallback: this.openPoundnetCommFishDuplicateDialog.bind(this)
                })
            ],
            [
                PageCodeEnum.DupRightToFishResource,
                new ApplicationsRegisterData<DuplicatesApplicationComponent>({
                    editDialog: dupDialog,
                    editDialogTCtor: DuplicatesApplicationComponent,
                    addRegisterDialogTitle: translationService.getValue('duplicates.add-permit-license-dialog-title'),
                    editApplicationDialogTitle: translationService.getValue('duplicates.edit-permit-license-application-dialog-title'),
                    editRegixDataDialogTitle: translationService.getValue('duplicates.edit-permit-license-application-regix-data-dialog-title'),
                    viewApplicationDialogTitle: translationService.getValue('duplicates.view-permit-license-application-dialog-title'),
                    viewRegisterDialogTitle: translationService.getValue('duplicates.view-permit-license-dialog-title'),
                    viewRegixDataDialogTitle: translationService.getValue('duplicates.view-permit-license-application-regix-data-dialog-title'),
                    viewAndConfrimDataRegularityTitle: translationService.getValue('duplicates.view-permit-license-appl-and-confirm-regularity-title'),
                    createRegisterCallback: this.openRightToFishResourceDuplicateDialog.bind(this)
                })
            ],
            [
                PageCodeEnum.DupPoundnetCommFishLic,
                new ApplicationsRegisterData<DuplicatesApplicationComponent>({
                    editDialog: dupDialog,
                    editDialogTCtor: DuplicatesApplicationComponent,
                    addRegisterDialogTitle: translationService.getValue('duplicates.add-poundnet-permit-license-dialog-title'),
                    editApplicationDialogTitle: translationService.getValue('duplicates.edit-poundnet-permit-license-application-dialog-title'),
                    editRegixDataDialogTitle: translationService.getValue('duplicates.edit-poundnet-permit-license-application-regix-data-dialog-title'),
                    viewApplicationDialogTitle: translationService.getValue('duplicates.view-poundnet-permit-license-application-dialog-title'),
                    viewRegisterDialogTitle: translationService.getValue('duplicates.view-poundnet-permit-license-dialog-title'),
                    viewRegixDataDialogTitle: translationService.getValue('duplicates.view-poundnet-permit-license-application-regix-data-dialog-title'),
                    viewAndConfrimDataRegularityTitle: translationService.getValue('duplicates.view-poundnet-permit-license-appl-and-confirm-regularity-title'),
                    createRegisterCallback: this.openPoundnetCommFishLicDuplicateDialog.bind(this)
                })
            ],
            [
                PageCodeEnum.DupCatchQuataSpecies,
                new ApplicationsRegisterData<DuplicatesApplicationComponent>({
                    editDialog: dupDialog,
                    editDialogTCtor: DuplicatesApplicationComponent,
                    addRegisterDialogTitle: translationService.getValue('duplicates.add-quata-species-permit-license-dialog-title'),
                    editApplicationDialogTitle: translationService.getValue('duplicates.edit-quata-species-permit-license-application-dialog-title'),
                    editRegixDataDialogTitle: translationService.getValue('duplicates.edit-quata-species-permit-license-application-regix-data-dialog-title'),
                    viewApplicationDialogTitle: translationService.getValue('duplicates.view-quata-species-permit-license-application-dialog-title'),
                    viewRegisterDialogTitle: translationService.getValue('duplicates.view-quata-species-permit-license-dialog-title'),
                    viewRegixDataDialogTitle: translationService.getValue('duplicates.view-quata-species-permit-license-application-regix-data-dialog-title'),
                    viewAndConfrimDataRegularityTitle: translationService.getValue('duplicates.view-quata-species-permit-license-appl-and-confirm-regularity-title'),
                    createRegisterCallback: this.openCatchQuataSpeciesDuplicateDialog.bind(this)
                })
            ],
            [
                PageCodeEnum.FishingGearsCommFish,
                new ApplicationsRegisterData<EditPermitLicenseFishingGearsComponent>({
                    editDialog: fishingGearsDialog,
                    editDialogTCtor: EditPermitLicenseFishingGearsComponent,
                    editApplicationDialogTitle: translationService.getValue('fishing-gears.edit-fishing-gears-permit-license-application-dialog-title'),
                    editRegixDataDialogTitle: translationService.getValue('fishing-gears.edit-fishing-gears-permit-license-application-regix-data-dialog-title'),
                    viewApplicationDialogTitle: translationService.getValue('fishing-gears.view-fishing-gears-permit-license-application-dialog-title'),
                    viewRegisterDialogTitle: translationService.getValue('fishing-gears.view-fishing-gears-permit-license-application-dialog-title'),
                    viewRegixDataDialogTitle: translationService.getValue('fishing-gears.view-fishing-gears-permit-license-application-regix-data-dialog-title'),
                    viewAndConfrimDataRegularityTitle: translationService.getValue('fishing-gears.view-fishing-gears-permit-license-appl-and-confirm-regularity-title'),
                    createRegisterCallback: this.openFishingGearsDialog.bind(this)
                })
            ]
        ]);
    }

    private openCommFishDuplicateDialog(application: ApplicationRegisterDTO): Observable<any> {
        const title: string = this.translate.getValue('duplicates.add-permit-dialog-title');
        return this.openDuplicateDialog(application, title);
    }

    private openRightToFishThirdDuplicateDialog(application: ApplicationRegisterDTO): Observable<any> {
        const title: string = this.translate.getValue('duplicates.add-3rd-country-permit-dialog-title');
        return this.openDuplicateDialog(application, title);
    }

    private openPoundnetCommFishDuplicateDialog(application: ApplicationRegisterDTO): Observable<any> {
        const title: string = this.translate.getValue('duplicates.add-poundnet-permit-dialog-title');
        return this.openDuplicateDialog(application, title);
    }

    private openRightToFishResourceDuplicateDialog(application: ApplicationRegisterDTO): Observable<any> {
        const title: string = this.translate.getValue('duplicates.add-permit-license-dialog-title');
        return this.openDuplicateDialog(application, title);
    }

    private openPoundnetCommFishLicDuplicateDialog(application: ApplicationRegisterDTO): Observable<any> {
        const title: string = this.translate.getValue('duplicates.add-poundnet-permit-license-dialog-title');
        return this.openDuplicateDialog(application, title);
    }

    private openCatchQuataSpeciesDuplicateDialog(application: ApplicationRegisterDTO): Observable<any> {
        const title: string = this.translate.getValue('duplicates.add-quata-species-permit-license-dialog-title');
        return this.openDuplicateDialog(application, title);
    }

    private openDuplicateDialog(application: ApplicationRegisterDTO, title: string): Observable<any> {
        return this.dupDialog.openWithTwoButtons({
            TCtor: DuplicatesApplicationComponent,
            title: title,
            translteService: this.translate,
            componentData: new DialogParamsModel({
                applicationId: application.id,
                isReadonly: false,
                isApplication: false,
                isApplicationHistoryMode: false,
                viewMode: false,
                showOnlyRegiXData: false,
                pageCode: application.pageCode,
                loadRegisterFromApplication: false,
                service: this.duplicatesService
            }),
            headerCancelButton: {
                cancelBtnClicked: this.closeDuplicateDialogBtnClicked.bind(this)
            },
            rightSideActionsCollection: [{
                id: 'save-print',
                color: 'accent',
                translateValue: 'duplicates.save-print'
            }]
        }, '1200px');
    }

    private openFishingGearsDialog(application: ApplicationRegisterDTO): Observable<any> {
        const saveOrEditDone: Subject<any> = new Subject<any>();

        const data: CommercialFishingDialogParamsModel = new CommercialFishingDialogParamsModel({
            id: undefined,
            service: this.service,
            applicationId: application.id
        });

        const title: string = this.translate.getValue('fishing-gears.edit-fishing-gears-permit-license-application-dialog-title');

        this.service.getPermitLicenseFromFishingGearsApplication(application.id!).subscribe({
            next: (model: CommercialFishingEditDTO) => {
                data.pageCode = model.pageCode!;
                data.model = model;
                data.model.applicationId = application.id;
                data.model.pageCode = application.pageCode;
                
                this.openEditDialog(data, title).subscribe((result: any) => {
                    saveOrEditDone.next(result);
                    saveOrEditDone.complete();
                });
            },
            error: () => {
                saveOrEditDone.next(undefined);
                saveOrEditDone.complete();
            }
        });

        return saveOrEditDone;
    }

    private openEditDialog(data: DialogParamsModel, title: string): Observable<any> {
        const auditButton: IHeaderAuditButton = {
            id: data.model!.id!,
            getAuditRecordData: this.service.getPermitLicenseSimpleAudit.bind(this.service),
            tableName: 'RStRib.PermitLicensesRegister' 
        };

        const rightButtons: IActionInfo[] = [];

        return this.editDialog.open({
            title: title,
            TCtor: EditCommercialFishingComponent,
            headerAuditButton: auditButton,
            headerCancelButton: {
                cancelBtnClicked: (closeFn: HeaderCloseFunction): void => {
                    closeFn();
                }
            },
            componentData: data,
            translteService: this.translate,
            disableDialogClose: true,
            saveBtn: {
                id: 'save',
                color: 'accent',
                translateValue: this.translate.getValue('common.save')
            },
            cancelBtn: {
                id: 'cancel',
                color: 'primary',
                translateValue: this.translate.getValue('common.cancel'),
            },
            rightSideActionsCollection: rightButtons,
            viewMode: false
        }, '1600px');
    }

    private closeDuplicateDialogBtnClicked(closeFn: HeaderCloseFunction): void {
        closeFn();
    }
}
