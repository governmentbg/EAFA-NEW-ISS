import { Component } from '@angular/core';

import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { PageCodeEnum } from '@app/enums/page-code.enum';
import { IStatisticalFormsService } from '@app/interfaces/common-app/statistical-forms.interface';
import { ApplicationProcessingPermissions } from '@app/models/common/application-processing-permissions.model';
import { ApplicationsRegisterData } from '@app/models/common/applications-register-data.model';
import { ApplicationsAdministrationService } from '@app/services/administration-app/applications-administration.service';
import { StatisticalFormsAdministrationService } from '@app/services/administration-app/statistical-forms-administration.service';
import { TLMatDialog } from '@app/shared/components/dialog-wrapper/tl-mat-dialog';
import { StatisticalFormsAquaFarmComponent } from '@app/components/common-app/statistical-forms/components/statistical-forms-aqua-farm/statistical-forms-aqua-farm.component';
import { PermissionsEnum } from '@app/shared/enums/permissions.enum';
import { StatisticalFormsReworkComponent } from '@app/components/common-app/statistical-forms/components/statistical-forms-rework/statistical-forms-rework.component';
import { StatisticalFormsFishVesselComponent } from '@app/components/common-app/statistical-forms/components/statistical-forms-fish-vessel/statistical-forms-fish-vessel.component';

type StatisticalFormsApplicationsDataType =
    ApplicationsRegisterData<StatisticalFormsAquaFarmComponent>
    | ApplicationsRegisterData<StatisticalFormsReworkComponent>
    | ApplicationsRegisterData<StatisticalFormsFishVesselComponent>;

@Component({
    selector: 'statistical-forms-applications',
    templateUrl: './statistical-forms-applications.component.html'
})
export class StatisticalFormsApplicationsComponent {
    public service: IStatisticalFormsService;
    public applicationsService: ApplicationsAdministrationService;
    public processingPermissions: Map<PageCodeEnum, ApplicationProcessingPermissions>;
    public applicationsRegisterData: Map<PageCodeEnum, StatisticalFormsApplicationsDataType>;

    private translate: FuseTranslationLoaderService;

    public constructor(
        service: StatisticalFormsAdministrationService,
        applicationsService: ApplicationsAdministrationService,
        aquaFarmEditDialog: TLMatDialog<StatisticalFormsAquaFarmComponent>,
        reworkEditDialog: TLMatDialog<StatisticalFormsReworkComponent>,
        fishVesselEditDialog: TLMatDialog<StatisticalFormsFishVesselComponent>,
        translationService: FuseTranslationLoaderService
    ) {
        this.service = service;
        this.applicationsService = applicationsService;
        this.translate = translationService;

        const aquaFarmProcessingPermissions: ApplicationProcessingPermissions = new ApplicationProcessingPermissions({
            addPermission: PermissionsEnum.StatisticalFormsAquaFarmApplicationsAddRecords,
            editPermission: PermissionsEnum.StatisticalFormsAquaFarmApplicationsEditRecords,
            deletePermission: PermissionsEnum.StatisticalFormsAquaFarmApplicationsDeleteRecords,
            restorePermission: PermissionsEnum.StatisticalFormsAquaFarmApplicationsRestoreRecords,
            cancelPermssion: PermissionsEnum.StatisticalFormsAquaFarmApplicationsCancel,
            inspectAndCorrectPermssion: PermissionsEnum.StatisticalFormsAquaFarmApplicationsInspectAndCorrectRegiXData,
            processPaymentDataPermission: PermissionsEnum.StatisticalFormsAquaFarmApplicationsProcessPaymentData,
            checkDataRegularityPermission: PermissionsEnum.StatisticalFormsAquaFarmApplicationsCheckDataRegularity,
            addAdministrativeActPermission: PermissionsEnum.StatisticalFormsAquaFarmAddRecords,
            readAdministrativeActPermission: PermissionsEnum.StatisticalFormsAquaFarmRead,
            canReAssignApplicationsPermission: PermissionsEnum.ReAssignApplication,
            canInspectCorrectAndAddAdmActPermission: PermissionsEnum.StatisticalFormsAquaFarmAddRecords,
            canReassingToDifferentTerritoryUnitPermission: PermissionsEnum.ApplicationsReassingToDifferentTerritoryUnit
        });

        const reworkProcessingPermissiosn: ApplicationProcessingPermissions = new ApplicationProcessingPermissions({
            addPermission: PermissionsEnum.StatisticalFormsReworkApplicationsAddRecords,
            editPermission: PermissionsEnum.StatisticalFormsReworkApplicationsEditRecords,
            deletePermission: PermissionsEnum.StatisticalFormsReworkApplicationsDeleteRecords,
            restorePermission: PermissionsEnum.StatisticalFormsReworkApplicationsRestoreRecords,
            cancelPermssion: PermissionsEnum.StatisticalFormsReworkApplicationsCancel,
            inspectAndCorrectPermssion: PermissionsEnum.StatisticalFormsReworkApplicationsInspectAndCorrectRegiXData,
            processPaymentDataPermission: PermissionsEnum.StatisticalFormsReworkApplicationsProcessPaymentData,
            checkDataRegularityPermission: PermissionsEnum.StatisticalFormsReworkApplicationsCheckDataRegularity,
            addAdministrativeActPermission: PermissionsEnum.StatisticalFormsReworkAddRecords,
            readAdministrativeActPermission: PermissionsEnum.StatisticalFormsReworkRead,
            canReAssignApplicationsPermission: PermissionsEnum.ReAssignApplication,
            canInspectCorrectAndAddAdmActPermission: PermissionsEnum.StatisticalFormsReworkAddRecords,
            canReassingToDifferentTerritoryUnitPermission: PermissionsEnum.ApplicationsReassingToDifferentTerritoryUnit
        });

        const fishVesselProcessingPermissions: ApplicationProcessingPermissions = new ApplicationProcessingPermissions({
            addPermission: PermissionsEnum.StatisticalFormsFishVesselsApplicationsAddRecords,
            editPermission: PermissionsEnum.StatisticalFormsFishVesselsApplicationsEditRecords,
            deletePermission: PermissionsEnum.StatisticalFormsFishVesselsApplicationsDeleteRecords,
            restorePermission: PermissionsEnum.StatisticalFormsFishVesselsApplicationsRestoreRecords,
            cancelPermssion: PermissionsEnum.StatisticalFormsFishVesselsApplicationsCancel,
            inspectAndCorrectPermssion: PermissionsEnum.StatisticalFormsFishVesselsApplicationsInspectAndCorrectRegiXData,
            processPaymentDataPermission: PermissionsEnum.StatisticalFormsFishVesselsApplicationsProcessPaymentData,
            checkDataRegularityPermission: PermissionsEnum.StatisticalFormsFishVesselsApplicationsCheckDataRegularity,
            addAdministrativeActPermission: PermissionsEnum.StatisticalFormsFishVesselAddRecords,
            readAdministrativeActPermission: PermissionsEnum.StatisticalFormsFishVesselRead,
            canReAssignApplicationsPermission: PermissionsEnum.ReAssignApplication,
            canInspectCorrectAndAddAdmActPermission: PermissionsEnum.StatisticalFormsFishVesselAddRecords,
            canReassingToDifferentTerritoryUnitPermission: PermissionsEnum.ApplicationsReassingToDifferentTerritoryUnit
        });

        this.processingPermissions = new Map<PageCodeEnum, ApplicationProcessingPermissions>([
            [
                PageCodeEnum.StatFormAquaFarm,
                aquaFarmProcessingPermissions
            ],
            [
                PageCodeEnum.StatFormRework,
                reworkProcessingPermissiosn
            ],
            [
                PageCodeEnum.StatFormFishVessel,
                fishVesselProcessingPermissions
            ]
        ]);

        this.applicationsRegisterData = new Map<PageCodeEnum, StatisticalFormsApplicationsDataType>([
            [
                PageCodeEnum.StatFormAquaFarm,
                new ApplicationsRegisterData<StatisticalFormsAquaFarmComponent>({
                    editDialog: aquaFarmEditDialog,
                    editDialogTCtor: StatisticalFormsAquaFarmComponent,
                    addRegisterDialogTitle: this.translate.getValue('statistical-forms.aqua-farm-add-register-dialog-title'),
                    editApplicationDialogTitle: this.translate.getValue('statistical-forms.aqua-farm-edit-application-dialog-title'),
                    editRegixDataDialogTitle: this.translate.getValue('statistical-forms.aqua-farm-edit-regix-data-dialog-title'),
                    viewApplicationDialogTitle: this.translate.getValue('statistical-forms.aqua-farm-view-application-dialog-title'),
                    viewRegisterDialogTitle: this.translate.getValue('statistical-forms.aqua-farm-view-register-dialog-title'),
                    viewRegixDataDialogTitle: this.translate.getValue('statistical-forms.aqua-farm-view-regix-data-dialog-title'),
                    viewAndConfrimDataRegularityTitle: this.translate.getValue('statistical-forms.aqua-farm-view-and-confrim-data-regularity-title')
                })
            ],
            [
                PageCodeEnum.StatFormRework,
                new ApplicationsRegisterData<StatisticalFormsReworkComponent>({
                    editDialog: reworkEditDialog,
                    editDialogTCtor: StatisticalFormsReworkComponent,
                    addRegisterDialogTitle: this.translate.getValue('statistical-forms.rework-add-register-dialog-title'),
                    editApplicationDialogTitle: this.translate.getValue('statistical-forms.rework-edit-application-dialog-title'),
                    editRegixDataDialogTitle: this.translate.getValue('statistical-forms.rework-edit-regix-data-dialog-title'),
                    viewApplicationDialogTitle: this.translate.getValue('statistical-forms.rework-view-application-dialog-title'),
                    viewRegisterDialogTitle: this.translate.getValue('statistical-forms.rework-view-register-dialog-title'),
                    viewRegixDataDialogTitle: this.translate.getValue('statistical-forms.rework-view-regix-data-dialog-title'),
                    viewAndConfrimDataRegularityTitle: this.translate.getValue('statistical-forms.rework-view-and-confrim-data-regularity-title')
                })
            ],
            [
                PageCodeEnum.StatFormFishVessel,
                new ApplicationsRegisterData<StatisticalFormsFishVesselComponent>({
                    editDialog: fishVesselEditDialog,
                    editDialogTCtor: StatisticalFormsFishVesselComponent,
                    addRegisterDialogTitle: translationService.getValue('statistical-forms.add-stat-form-fish-vessel-dialog-title'),
                    editApplicationDialogTitle: translationService.getValue('statistical-forms.edit-stat-form-fish-vessel-application-dialog-title'),
                    editRegixDataDialogTitle: translationService.getValue('statistical-forms.edit-stat-form-fish-vessel-regix-data-dialog-title'),
                    viewApplicationDialogTitle: translationService.getValue('statistical-forms.view-stat-form-fish-vessel-dialog-application-title'),
                    viewRegisterDialogTitle: translationService.getValue('statistical-forms.view-stat-form-fish-vessel-dialog-title'),
                    viewRegixDataDialogTitle: translationService.getValue('statistical-forms.add-stat-form-fish-vessel-regix-data-dialog-title'),
                    viewAndConfrimDataRegularityTitle: translationService.getValue('statistical-forms.view-and-confirm-stat-form-fish-vessel-dialog-title')
                })
            ]
        ]);
    }
}