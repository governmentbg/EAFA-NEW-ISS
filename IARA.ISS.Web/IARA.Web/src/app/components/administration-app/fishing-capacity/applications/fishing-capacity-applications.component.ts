import { Component } from '@angular/core';
import { Router } from '@angular/router';

import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { PageCodeEnum } from '@app/enums/page-code.enum';
import { ApplicationProcessingPermissions } from '@app/models/common/application-processing-permissions.model';
import { ApplicationsRegisterData } from '@app/models/common/applications-register-data.model';
import { ApplicationsAdministrationService } from '@app/services/administration-app/applications-administration.service';
import { FishingCapacityAdministrationService } from '@app/services/administration-app/fishing-capacity-administration.service';
import { TLMatDialog } from '@app/shared/components/dialog-wrapper/tl-mat-dialog';
import { IncreaseFishingCapacityComponent } from '@app/components/common-app/fishing-capacity/increase-fishing-capacity/increase-fishing-capacity.component';
import { ReduceFishingCapacityComponent } from '@app/components/common-app/fishing-capacity/reduce-fishing-capacity/reduce-fishing-capacity.component';
import { TransferFishingCapacityComponent } from '@app/components/common-app/fishing-capacity/transfer-fishing-capacity/transfer-fishing-capacity.component';
import { PermissionsEnum } from '@app/shared/enums/permissions.enum';
import { ApplicationRegisterDTO } from '@app/models/generated/dtos/ApplicationRegisterDTO';
import { CapacityCertificateDuplicateComponent } from '@app/components/common-app/fishing-capacity/capacity-certificate-duplicate/capacity-certificate-duplicate.component';

type FishingCapacityRegisterDataType =
    ApplicationsRegisterData<IncreaseFishingCapacityComponent> |
    ApplicationsRegisterData<ReduceFishingCapacityComponent> |
    ApplicationsRegisterData<TransferFishingCapacityComponent> |
    ApplicationsRegisterData<CapacityCertificateDuplicateComponent>;

@Component({
    selector: 'fishing-capacity-application',
    templateUrl: './fishing-capacity-applications.component.html'
})
export class FishingCapacityApplicationsComponent {
    public service: FishingCapacityAdministrationService;
    public applicationsService: ApplicationsAdministrationService;
    public processingPermissions: Map<PageCodeEnum, ApplicationProcessingPermissions>;
    public applicationsRegisterData!: Map<PageCodeEnum, FishingCapacityRegisterDataType>;

    private router: Router;

    public constructor(
        service: FishingCapacityAdministrationService,
        applicationsService: ApplicationsAdministrationService,
        translationService: FuseTranslationLoaderService,
        increaseDialog: TLMatDialog<IncreaseFishingCapacityComponent>,
        reduceDialog: TLMatDialog<ReduceFishingCapacityComponent>,
        transferDialog: TLMatDialog<TransferFishingCapacityComponent>,
        certDupDialog: TLMatDialog<CapacityCertificateDuplicateComponent>,
        router: Router
    ) {
        this.service = service;
        this.applicationsService = applicationsService;
        this.router = router;

        const processingPermissions: ApplicationProcessingPermissions = new ApplicationProcessingPermissions({
            addPermission: PermissionsEnum.FishingCapacityApplicationsAddRecords,
            editPermission: PermissionsEnum.FishingCapacityApplicationsEditRecords,
            deletePermission: PermissionsEnum.FishingCapacityApplicationsDeleteRecords,
            restorePermission: PermissionsEnum.FishingCapacityApplicationsRestoreRecords,
            cancelPermssion: PermissionsEnum.FishingCapacityApplicationsCancel,
            inspectAndCorrectPermssion: PermissionsEnum.FishingCapacityApplicationsInspectAndCorrectRegiXData,
            processPaymentDataPermission: PermissionsEnum.FishingCapacityApplicationsProcessPaymentData,
            checkDataRegularityPermission: PermissionsEnum.FishingCapacityApplicationsCheckDataRegularity,
            addAdministrativeActPermission: PermissionsEnum.FishingCapacityAddRecords,
            readAdministrativeActPermission: PermissionsEnum.FishingCapacityRead
        });

        this.processingPermissions = new Map<PageCodeEnum, ApplicationProcessingPermissions>([
            [
                PageCodeEnum.IncreaseFishCap,
                processingPermissions
            ],
            [
                PageCodeEnum.ReduceFishCap,
                processingPermissions
            ],
            [
                PageCodeEnum.TransferFishCap,
                processingPermissions
            ],
            [
                PageCodeEnum.CapacityCertDup,
                processingPermissions
            ]
        ]);

        this.applicationsRegisterData = new Map<PageCodeEnum, FishingCapacityRegisterDataType>([
            [
                PageCodeEnum.IncreaseFishCap,
                new ApplicationsRegisterData<IncreaseFishingCapacityComponent>({
                    editDialog: increaseDialog,
                    editDialogTCtor: IncreaseFishingCapacityComponent,
                    addRegisterDialogTitle: translationService.getValue('fishing-capacity.complete-increase-capacity-application-dialog-title'),
                    editApplicationDialogTitle: translationService.getValue('fishing-capacity.edit-increase-capacity-application-dialog-title'),
                    editRegixDataDialogTitle: translationService.getValue('fishing-capacity.edit-increase-capacity-regix-data-dialog-title'),
                    viewApplicationDialogTitle: translationService.getValue('fishing-capacity.view-increase-capacity-application-dialog-title'),
                    viewRegixDataDialogTitle: translationService.getValue('fishing-capacity.view-increase-capacity-regix-data-dialog-title'),
                    viewAndConfrimDataRegularityTitle: translationService.getValue('fishing-capacity.view-increase-capacity-appl-and-confirm-regularity-title'),
                    createRegisterCallback: this.openShipEditForIncreaseCapacity.bind(this)
                })
            ],
            [
                PageCodeEnum.ReduceFishCap,
                new ApplicationsRegisterData<ReduceFishingCapacityComponent>({
                    editDialog: reduceDialog,
                    editDialogTCtor: ReduceFishingCapacityComponent,
                    addRegisterDialogTitle: translationService.getValue('fishing-capacity.complete-reduce-capacity-application-dialog-title'),
                    editApplicationDialogTitle: translationService.getValue('fishing-capacity.edit-reduce-capacity-application-dialog-title'),
                    editRegixDataDialogTitle: translationService.getValue('fishing-capacity.edit-reduce-capacity-regix-data-dialog-title'),
                    viewApplicationDialogTitle: translationService.getValue('fishing-capacity.view-reduce-capacity-application-dialog-title'),
                    viewRegixDataDialogTitle: translationService.getValue('fishing-capacity.view-reduce-capacity-regix-data-dialog-title'),
                    viewAndConfrimDataRegularityTitle: translationService.getValue('fishing-capacity.view-reduce-capacity-appl-and-confirm-regularity-title'),
                    createRegisterCallback: this.openShipEditForReduceCapacity.bind(this)
                })
            ],
            [
                PageCodeEnum.TransferFishCap,
                new ApplicationsRegisterData<TransferFishingCapacityComponent>({
                    editDialog: transferDialog,
                    editDialogTCtor: TransferFishingCapacityComponent,
                    addRegisterDialogTitle: translationService.getValue('fishing-capacity.complete-transfer-capacity-application-dialog-title'),
                    editApplicationDialogTitle: translationService.getValue('fishing-capacity.edit-transfer-capacity-application-dialog-title'),
                    editRegixDataDialogTitle: translationService.getValue('fishing-capacity.edit-transfer-capacity-regix-data-dialog-title'),
                    viewApplicationDialogTitle: translationService.getValue('fishing-capacity.view-transfer-capacity-application-dialog-title'),
                    viewRegixDataDialogTitle: translationService.getValue('fishing-capacity.view-transfer-capacity-regix-data-dialog-title'),
                    viewAndConfrimDataRegularityTitle: translationService.getValue('fishing-capacity.view-transfer-capacity-appl-and-confirm-regularity-title')
                })
            ],
            [
                PageCodeEnum.CapacityCertDup,
                new ApplicationsRegisterData<CapacityCertificateDuplicateComponent>({
                    editDialog: certDupDialog,
                    editDialogTCtor: CapacityCertificateDuplicateComponent,
                    addRegisterDialogTitle: translationService.getValue('fishing-capacity.complete-duplicate-capacity-application-dialog-title'),
                    editApplicationDialogTitle: translationService.getValue('fishing-capacity.edit-duplicate-capacity-application-dialog-title'),
                    editRegixDataDialogTitle: translationService.getValue('fishing-capacity.edit-duplicate-capacity-regix-data-dialog-title'),
                    viewApplicationDialogTitle: translationService.getValue('fishing-capacity.view-duplicate-capacity-application-dialog-title'),
                    viewRegixDataDialogTitle: translationService.getValue('fishing-capacity.view-duplicate-capacity-regix-data-dialog-title'),
                    viewAndConfrimDataRegularityTitle: translationService.getValue('fishing-capacity.view-duplicate-capacity-appl-and-confirm-regularity-title')
                })
            ]
        ]);
    }

    private openShipEditForIncreaseCapacity(application: ApplicationRegisterDTO): void {
        this.router.navigateByUrl('/fishing-vessels/edit', {
            state: {
                id: undefined,
                applicationId: application.id,
                isChangeOfCircumstancesApplication: false,
                isDeregistrationApplication: false,
                isIncreaseCapacityApplication: true,
                isReduceCapacityApplication: false,
                viewMode: false,
                isThirdPartyShip: false
            }
        });
    }

    private openShipEditForReduceCapacity(application: ApplicationRegisterDTO): void {
        this.router.navigateByUrl('/fishing-vessels/edit', {
            state: {
                id: undefined,
                applicationId: application.id,
                isChangeOfCircumstancesApplication: false,
                isDeregistrationApplication: false,
                isIncreaseCapacityApplication: false,
                isReduceCapacityApplication: true,
                viewMode: false,
                isThirdPartyShip: false
            }
        });
    }
}