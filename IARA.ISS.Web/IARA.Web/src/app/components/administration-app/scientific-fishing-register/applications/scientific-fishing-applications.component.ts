import { Component } from '@angular/core';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { PageCodeEnum } from '@app/enums/page-code.enum';
import { IApplicationsService } from '@app/interfaces/administration-app/applications.interface';
import { ApplicationProcessingPermissions } from '@app/models/common/application-processing-permissions.model';
import { ApplicationsRegisterData } from '@app/models/common/applications-register-data.model';
import { ApplicationsAdministrationService } from '@app/services/administration-app/applications-administration.service';
import { ScientificFishingAdministrationService } from '@app/services/administration-app/scientific-fishing-administration.service';
import { TLMatDialog } from '@app/shared/components/dialog-wrapper/tl-mat-dialog';
import { PermissionsEnum } from '@app/shared/enums/permissions.enum';
import { EditScientificPermitComponent } from '../../../common-app/scientific-fishing/components/edit-scientific-permit/edit-scientific-permit.component';

type SciFiApplicationsRegisterDataType = ApplicationsRegisterData<EditScientificPermitComponent>;

@Component({
    selector: 'scientific-fishing-applications',
    templateUrl: './scientific-fishing-applications.component.html'
})
export class ScientificFishingApplicationsComponent {
    public service: ScientificFishingAdministrationService;
    public applicationsService: IApplicationsService;
    public processingPermissions: Map<PageCodeEnum, ApplicationProcessingPermissions>;
    public applicationsRegisterData!: Map<PageCodeEnum, SciFiApplicationsRegisterDataType>;

    public constructor(
        service: ScientificFishingAdministrationService,
        applicationsService: ApplicationsAdministrationService,
        editDialog: TLMatDialog<EditScientificPermitComponent>,
        translationService: FuseTranslationLoaderService
    ) {
        this.service = service;
        this.applicationsService = applicationsService;

        const processingPermissions: ApplicationProcessingPermissions = new ApplicationProcessingPermissions({
            addPermission: PermissionsEnum.ScientificFishingApplicationsAddRecords,
            editPermission: PermissionsEnum.ScientificFishingApplicationsEditRecords,
            deletePermission: PermissionsEnum.ScientificFishingApplicationsDeleteRecords,
            restorePermission: PermissionsEnum.ScientificFishingApplicationsRestoreRecords,
            cancelPermssion: PermissionsEnum.ScientificFishingApplicationsCancel,
            inspectAndCorrectPermssion: PermissionsEnum.ScientificFishingApplicationsInspectAndCorrectRegiXData,
            processPaymentDataPermission: PermissionsEnum.ScientificFishingApplicationsProcessPaymentData,
            checkDataRegularityPermission: PermissionsEnum.ScientificFishingApplicationsCheckDataRegularity,
            addAdministrativeActPermission: PermissionsEnum.ScientificFishingAddRecords,
            readAdministrativeActPermission: PermissionsEnum.ScientificFishingRead,
            canReAssignApplicationsPermission: PermissionsEnum.ReAssignApplication
        });

        this.processingPermissions = new Map<PageCodeEnum, ApplicationProcessingPermissions>([
            [
                PageCodeEnum.SciFi,
                processingPermissions
            ]
        ]);

        this.applicationsRegisterData = new Map<PageCodeEnum, SciFiApplicationsRegisterDataType>([
            [
                PageCodeEnum.SciFi,
                new ApplicationsRegisterData<EditScientificPermitComponent>({
                    editDialog: editDialog,
                    editDialogTCtor: EditScientificPermitComponent,
                    addRegisterDialogTitle: translationService.getValue('scientific-fishing.add-permit-dialog-title'),
                    editApplicationDialogTitle: translationService.getValue('scientific-fishing.edit-permit-application-dialog-title'),
                    editRegixDataDialogTitle: translationService.getValue('scientific-fishing.edit-permit-application-regix-data-dialog-title'),
                    viewApplicationDialogTitle: translationService.getValue('scientific-fishing.view-permit-application-dialog-title'),
                    viewRegisterDialogTitle: translationService.getValue('scientific-fishing.view-permit-dialog-title'),
                    viewRegixDataDialogTitle: translationService.getValue('scientific-fishing.view-permit-application-regix-data-dialog-title'),
                    viewAndConfrimDataRegularityTitle: translationService.getValue('scientific-fishing.view-permit-appl-and-confirm-regularity-title')
                })
            ]
        ]);
    }
}
