import { Component } from '@angular/core';

import { IApplicationsService } from '@app/interfaces/administration-app/applications.interface';
import { ApplicationProcessingPermissions } from '@app/models/common/application-processing-permissions.model';
import { ApplicationsPublicService } from '@app/services/public-app/applications-public.service';
import { SubmittedApplicationsProcessingService } from '@app/services/public-app/submitted-applications-processing.service';
import { PermissionsEnum } from '@app/shared/enums/permissions.enum';
import { PageCodeEnum } from '@app/enums/page-code.enum';
import { ApplicationTablePageType } from '@app/components/common-app/applications/applications-table/applications-table.component';


@Component({
    selector: 'submitted-applications',
    templateUrl: './submitted-applications.component.html'
})
export class SubmittedApplicationsComponent {

    public pageType: ApplicationTablePageType = 'PublicPage';
    public service: SubmittedApplicationsProcessingService;
    public applicationsService: IApplicationsService;
    public processingPermissions: Map<PageCodeEnum, ApplicationProcessingPermissions>;

    public constructor(service: SubmittedApplicationsProcessingService, applicationsService: ApplicationsPublicService) {
        this.service = service;
        this.applicationsService = applicationsService;

        const onlineProcessingPermissions: ApplicationProcessingPermissions = new ApplicationProcessingPermissions({
            addPermission: PermissionsEnum.OnlineSubmittedApplicationsAddRecords,
            cancelPermssion: PermissionsEnum.OnlineSubmittedApplicationsCancelRecords,
            editPermission: PermissionsEnum.OnlineSubmittedApplicationsEditRecords,
            processPaymentDataPermission: PermissionsEnum.OnlineSubmittedApplicationsProcessPaymentData,
            downloadOnlineApplicationsPermission: PermissionsEnum.OnlineSubmittedApplicationsDownloadRecords,
            uploadOnlineApplicationsPermission: PermissionsEnum.OnlineSubmittedApplicationsUploadRecords,
            readAdministrativeActPermission: PermissionsEnum.OnlineSubmittedApplicationsReadRegister
        });

        this.processingPermissions = new Map<PageCodeEnum, ApplicationProcessingPermissions>(
            [
                [PageCodeEnum.SciFi, onlineProcessingPermissions],
                [PageCodeEnum.LE, onlineProcessingPermissions],
                [PageCodeEnum.Assocs, onlineProcessingPermissions],
                [PageCodeEnum.CommFish, onlineProcessingPermissions],
                [PageCodeEnum.DupCommFish, onlineProcessingPermissions],
                [PageCodeEnum.AquaFarmReg, onlineProcessingPermissions],
                [PageCodeEnum.AquaFarmChange, onlineProcessingPermissions],
                [PageCodeEnum.AquaFarmDereg, onlineProcessingPermissions],
                [PageCodeEnum.ShipRegChange, onlineProcessingPermissions],
                [PageCodeEnum.RightToFishResource, onlineProcessingPermissions],
                [PageCodeEnum.DupRightToFishResource, onlineProcessingPermissions],
                [PageCodeEnum.RightToFishThirdCountry, onlineProcessingPermissions],
                [PageCodeEnum.DupRightToFishThirdCountry, onlineProcessingPermissions],
                [PageCodeEnum.TransferFishCap, onlineProcessingPermissions],
                [PageCodeEnum.CapacityCertDup, onlineProcessingPermissions],
                [PageCodeEnum.PoundnetCommFish, onlineProcessingPermissions],
                [PageCodeEnum.DupPoundnetCommFish, onlineProcessingPermissions],
                [PageCodeEnum.PoundnetCommFishLic, onlineProcessingPermissions],
                [PageCodeEnum.DupPoundnetCommFishLic, onlineProcessingPermissions],
                [PageCodeEnum.FishingGearsCommFish, onlineProcessingPermissions],
                [PageCodeEnum.ReduceFishCap, onlineProcessingPermissions],
                [PageCodeEnum.RegVessel, onlineProcessingPermissions],
                [PageCodeEnum.CommFishLicense, onlineProcessingPermissions],
                [PageCodeEnum.CatchQuataSpecies, onlineProcessingPermissions],
                [PageCodeEnum.DupCatchQuataSpecies, onlineProcessingPermissions],
                [PageCodeEnum.CompetencyDup, onlineProcessingPermissions],
                [PageCodeEnum.IncreaseFishCap, onlineProcessingPermissions],
                [PageCodeEnum.RegFirstSaleCenter, onlineProcessingPermissions],
                [PageCodeEnum.RegFirstSaleBuyer, onlineProcessingPermissions],
                [PageCodeEnum.DeregShip, onlineProcessingPermissions],
                [PageCodeEnum.ChangeFirstSaleBuyer, onlineProcessingPermissions],
                [PageCodeEnum.ChangeFirstSaleCenter, onlineProcessingPermissions],
                [PageCodeEnum.TermFirstSaleBuyer, onlineProcessingPermissions],
                [PageCodeEnum.TermFirstSaleCenter, onlineProcessingPermissions],
                [PageCodeEnum.DupFirstSaleBuyer, onlineProcessingPermissions],
                [PageCodeEnum.DupFirstSaleCenter, onlineProcessingPermissions],
                [PageCodeEnum.StatFormAquaFarm, onlineProcessingPermissions],
                [PageCodeEnum.StatFormRework, onlineProcessingPermissions],
                [PageCodeEnum.StatFormFishVessel, onlineProcessingPermissions]
            ]
        );
    }
}
