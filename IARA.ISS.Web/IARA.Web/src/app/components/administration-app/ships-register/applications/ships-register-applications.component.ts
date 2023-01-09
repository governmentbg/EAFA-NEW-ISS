import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';

import { PageCodeEnum } from '@app/enums/page-code.enum';
import { ApplicationProcessingPermissions } from '@app/models/common/application-processing-permissions.model';
import { ApplicationsRegisterData } from '@app/models/common/applications-register-data.model';
import { ApplicationsAdministrationService } from '@app/services/administration-app/applications-administration.service';
import { ShipsRegisterAdministrationService } from '@app/services/administration-app/ships-register-administration.service';
import { TLMatDialog } from '@app/shared/components/dialog-wrapper/tl-mat-dialog';
import { PermissionsEnum } from '@app/shared/enums/permissions.enum';
import { EditShipComponent } from '@app/components/common-app/ships-register/edit-ship/edit-ship.component';
import { ShipChangeOfCircumstancesComponent } from '@app/components/common-app/ships-register/ship-change-of-circumstances/ship-change-of-circumstances.component';
import { ApplicationRegisterDTO } from '@app/models/generated/dtos/ApplicationRegisterDTO';
import { ShipDeregistrationComponent } from '@app/components/common-app/ships-register/ship-deregistration/ship-deregistration.component';

type ShipApplicationsRegisterDataType =
    ApplicationsRegisterData<EditShipComponent> |
    ApplicationsRegisterData<ShipChangeOfCircumstancesComponent> |
    ApplicationsRegisterData<ShipDeregistrationComponent>;

@Component({
    selector: 'ships-register-applications',
    templateUrl: './ships-register-applications.component.html'
})
export class ShipsRegisterApplicationsComponent {
    public service: ShipsRegisterAdministrationService;
    public applicationsService: ApplicationsAdministrationService;
    public processingPermissions: Map<PageCodeEnum, ApplicationProcessingPermissions>;
    public applicationsRegisterData!: Map<PageCodeEnum, ShipApplicationsRegisterDataType>;

    private router: Router;

    public constructor(
        service: ShipsRegisterAdministrationService,
        applicationsService: ApplicationsAdministrationService,
        editDialog: TLMatDialog<EditShipComponent>,
        cocDialog: TLMatDialog<ShipChangeOfCircumstancesComponent>,
        deregDialog: TLMatDialog<ShipDeregistrationComponent>,
        translationService: FuseTranslationLoaderService,
        router: Router
    ) {
        this.service = service;
        this.applicationsService = applicationsService;
        this.router = router;

        const processingPermissions: ApplicationProcessingPermissions = new ApplicationProcessingPermissions({
            addPermission: PermissionsEnum.ShipsRegisterApplicationsAddRecords,
            editPermission: PermissionsEnum.ShipsRegisterApplicationsEditRecords,
            deletePermission: PermissionsEnum.ShipsRegisterApplicationsDeleteRecords,
            restorePermission: PermissionsEnum.ShipsRegisterApplicationsRestoreRecords,
            cancelPermssion: PermissionsEnum.ShipsRegisterApplicationsCancel,
            inspectAndCorrectPermssion: PermissionsEnum.ShipsRegisterApplicationsInspectAndCorrectRegiXData,
            processPaymentDataPermission: PermissionsEnum.ShipsRegisterApplicationsProcessPaymentData,
            checkDataRegularityPermission: PermissionsEnum.ShipsRegisterApplicationsCheckDataRegularity,
            addAdministrativeActPermission: PermissionsEnum.ShipsRegisterAddRecords,
            readAdministrativeActPermission: PermissionsEnum.ShipsRegisterRead,
            canReAssignApplicationsPermission: PermissionsEnum.ReAssignApplication
        });

        this.processingPermissions = new Map<PageCodeEnum, ApplicationProcessingPermissions>([
            [
                PageCodeEnum.RegVessel,
                processingPermissions
            ],
            [
                PageCodeEnum.ShipRegChange,
                processingPermissions
            ],
            [
                PageCodeEnum.DeregShip,
                processingPermissions
            ]
        ]);

        this.applicationsRegisterData = new Map<PageCodeEnum, ShipApplicationsRegisterDataType>([
            [
                PageCodeEnum.RegVessel,
                new ApplicationsRegisterData<EditShipComponent>({
                    editDialog: editDialog,
                    editDialogTCtor: EditShipComponent,
                    addRegisterDialogTitle: translationService.getValue('ships-register.add-ship-dialog-title'),
                    editApplicationDialogTitle: translationService.getValue('ships-register.edit-ship-application-dialog-title'),
                    editRegixDataDialogTitle: translationService.getValue('ships-register.edit-ship-application-regix-data-dialog-title'),
                    viewApplicationDialogTitle: translationService.getValue('ships-register.view-ship-application-dialog-title'),
                    viewRegisterDialogTitle: translationService.getValue('ships-register.view-ship-dialog-title'),
                    viewRegixDataDialogTitle: translationService.getValue('ships-register.view-ship-application-regix-data-dialog-title'),
                    viewAndConfrimDataRegularityTitle: translationService.getValue('ships-register.view-ship-appl-and-confirm-regularity-title')
                })
            ],
            [
                PageCodeEnum.ShipRegChange,
                new ApplicationsRegisterData<ShipChangeOfCircumstancesComponent>({
                    editDialog: cocDialog,
                    editDialogTCtor: ShipChangeOfCircumstancesComponent,
                    editApplicationDialogTitle: translationService.getValue('ships-register.edit-change-of-circumstances-dialog-title'),
                    editRegixDataDialogTitle: translationService.getValue('ships-register.edit-change-of-circumstances-regix-data-dialog-title'),
                    viewApplicationDialogTitle: translationService.getValue('ships-register.view-change-of-circumstances-dialog-title'),
                    viewRegixDataDialogTitle: translationService.getValue('ships-register.view-change-of-circumstances-regix-data-dialog-title'),
                    viewAndConfrimDataRegularityTitle: translationService.getValue('ships-register.view-change-of-circumstances-appl-and-confirm-regularity-title'),
                    createRegisterCallback: this.openShipEditForCircumstances.bind(this)
                })
            ],
            [
                PageCodeEnum.DeregShip,
                new ApplicationsRegisterData<ShipDeregistrationComponent>({
                    editDialog: deregDialog,
                    editDialogTCtor: ShipDeregistrationComponent,
                    editApplicationDialogTitle: translationService.getValue('ships-register.edit-deregistration-dialog-title'),
                    editRegixDataDialogTitle: translationService.getValue('ships-register.edit-deregistration-regix-data-dialog-title'),
                    viewApplicationDialogTitle: translationService.getValue('ships-register.view-deregistration-dialog-title'),
                    viewRegixDataDialogTitle: translationService.getValue('ships-register.view-deregistration-regix-data-dialog-title'),
                    viewAndConfrimDataRegularityTitle: translationService.getValue('ships-register.view-deregistration-appl-and-confirm-regularity-title'),
                    createRegisterCallback: this.openShipEditForDeregistration.bind(this)
                })
            ]
        ]);
    }

    private openShipEditForCircumstances(application: ApplicationRegisterDTO): void {
        this.router.navigateByUrl('/fishing-vessels/edit', {
            state: {
                id: undefined,
                applicationId: application.id,
                isChangeOfCircumstancesApplication: true,
                isDeregistrationApplication: false,
                isIncreaseCapacityApplication: false,
                isReduceCapacityApplication: false,
                viewMode: false,
                isThirdPartyShip: false
            }
        });
    }

    private openShipEditForDeregistration(application: ApplicationRegisterDTO): void {
        this.router.navigateByUrl('/fishing-vessels/edit', {
            state: {
                id: undefined,
                applicationId: application.id,
                isChangeOfCircumstancesApplication: false,
                isDeregistrationApplication: true,
                isIncreaseCapacityApplication: false,
                isReduceCapacityApplication: false,
                viewMode: false,
                isThirdPartyShip: false
            }
        });
    }
}