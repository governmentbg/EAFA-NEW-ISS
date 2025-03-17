import { Component, OnInit } from '@angular/core';

import { IApplicationsRegisterService } from '@app/interfaces/administration-app/applications-register.interface';
import { IApplicationsService } from '@app/interfaces/administration-app/applications.interface';
import { ApplicationProcessingPermissions } from '@app/models/common/application-processing-permissions.model';
import { ApplicationsAdministrationService } from '@app/services/administration-app/applications-administration.service';
import { ApplicationsProcessingService } from '@app/services/administration-app/applications-processing.service';
import { PermissionsEnum } from '@app/shared/enums/permissions.enum';
import { PageCodeEnum } from '@app/enums/page-code.enum';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { ApplicationsNotAssignedDTO } from '@app/models/generated/dtos/ApplicationsNotAssignedDTO';

@Component({
    selector: 'applications-processing',
    templateUrl: './applications-processing.component.html'
})
export class ApplicationsProcessingComponent implements OnInit {
    public service: IApplicationsRegisterService;
    public processingPermissions: Map<PageCodeEnum, ApplicationProcessingPermissions>;
    public applicationsService: IApplicationsService;
    public translate: FuseTranslationLoaderService;

    public onlineApplicationsLoaded: boolean = false;
    public paperNotAssigned: number = 0;
    public onlineNotAssigned: number = 0;

    public constructor(service: ApplicationsProcessingService,
        applicationsService: ApplicationsAdministrationService,
        translate: FuseTranslationLoaderService
    ) {
        this.service = service;
        this.translate = translate;
        this.applicationsService = applicationsService;

        const applicationProcessingPermissions: ApplicationProcessingPermissions = new ApplicationProcessingPermissions({
            addPermission: PermissionsEnum.ApplicationsAddRecords,
            editPermission: PermissionsEnum.ApplicationsEditRecords,
            deletePermission: PermissionsEnum.ApplicationsDeleteRecords,
            restorePermission: PermissionsEnum.ApplicationsRestoreRecords,
            enterEventisNumberPermission: PermissionsEnum.ApplicationsEnterEventisNumber,
            cancelPermssion: PermissionsEnum.ApplicationsCancelRecords,
            inspectAndCorrectPermssion: PermissionsEnum.ApplicationsInspectAndCorrectRegiXData,
            canReAssignApplicationsPermission: PermissionsEnum.ReAssignApplication,
            processPaymentDataPermission: PermissionsEnum.ApplicationsProcessPaymentData,
            canReassingToDifferentTerritoryUnitPermission: PermissionsEnum.ApplicationsReassingToDifferentTerritoryUnit
        });

        this.processingPermissions = new Map<PageCodeEnum, ApplicationProcessingPermissions>(
            [
                [PageCodeEnum.SciFi, applicationProcessingPermissions],
                [PageCodeEnum.LE, applicationProcessingPermissions],
                [PageCodeEnum.Assocs, applicationProcessingPermissions],
                [PageCodeEnum.CommFish, applicationProcessingPermissions],
                [PageCodeEnum.DupCommFish, applicationProcessingPermissions],
                [PageCodeEnum.AquaFarmReg, applicationProcessingPermissions],
                [PageCodeEnum.AquaFarmChange, applicationProcessingPermissions],
                [PageCodeEnum.AquaFarmDereg, applicationProcessingPermissions],
                [PageCodeEnum.ShipRegChange, applicationProcessingPermissions],
                [PageCodeEnum.RightToFishResource, applicationProcessingPermissions],
                [PageCodeEnum.DupRightToFishResource, applicationProcessingPermissions],
                [PageCodeEnum.RightToFishThirdCountry, applicationProcessingPermissions],
                [PageCodeEnum.DupRightToFishThirdCountry, applicationProcessingPermissions],
                [PageCodeEnum.TransferFishCap, applicationProcessingPermissions],
                [PageCodeEnum.CapacityCertDup, applicationProcessingPermissions],
                [PageCodeEnum.PoundnetCommFish, applicationProcessingPermissions],
                [PageCodeEnum.DupPoundnetCommFish, applicationProcessingPermissions],
                [PageCodeEnum.PoundnetCommFishLic, applicationProcessingPermissions],
                [PageCodeEnum.DupPoundnetCommFishLic, applicationProcessingPermissions],
                [PageCodeEnum.FishingGearsCommFish, applicationProcessingPermissions],
                [PageCodeEnum.ReduceFishCap, applicationProcessingPermissions],
                [PageCodeEnum.RegVessel, applicationProcessingPermissions],
                [PageCodeEnum.CommFishLicense, applicationProcessingPermissions],
                [PageCodeEnum.CatchQuataSpecies, applicationProcessingPermissions],
                [PageCodeEnum.DupCatchQuataSpecies, applicationProcessingPermissions],
                [PageCodeEnum.CompetencyDup, applicationProcessingPermissions],
                [PageCodeEnum.IncreaseFishCap, applicationProcessingPermissions],
                [PageCodeEnum.RegFirstSaleCenter, applicationProcessingPermissions],
                [PageCodeEnum.RegFirstSaleBuyer, applicationProcessingPermissions],
                [PageCodeEnum.DeregShip, applicationProcessingPermissions],
                [PageCodeEnum.ChangeFirstSaleBuyer, applicationProcessingPermissions],
                [PageCodeEnum.ChangeFirstSaleCenter, applicationProcessingPermissions],
                [PageCodeEnum.TermFirstSaleBuyer, applicationProcessingPermissions],
                [PageCodeEnum.TermFirstSaleCenter, applicationProcessingPermissions],
                [PageCodeEnum.DupFirstSaleBuyer, applicationProcessingPermissions],
                [PageCodeEnum.DupFirstSaleCenter, applicationProcessingPermissions],
                [PageCodeEnum.StatFormAquaFarm, applicationProcessingPermissions],
                [PageCodeEnum.StatFormRework, applicationProcessingPermissions],
                [PageCodeEnum.StatFormFishVessel, applicationProcessingPermissions]
            ]
        );
    }

    public ngOnInit(): void {
        this.service.getNotAssignedApplications().subscribe({
            next: (data: ApplicationsNotAssignedDTO) => {
                this.paperNotAssigned = data.paperApplicationsCount!;
                this.onlineNotAssigned = data.onlineApplicationsCount!;
            }
        });
    }

    public onTabChanged(index: number): void {
        if (index === 1) {
            this.onlineApplicationsLoaded = true;
        }
    }
}
