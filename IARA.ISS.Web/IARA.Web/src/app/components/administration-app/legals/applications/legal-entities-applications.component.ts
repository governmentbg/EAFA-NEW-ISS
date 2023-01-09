import { Component } from '@angular/core';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { PageCodeEnum } from '@app/enums/page-code.enum';
import { ApplicationProcessingPermissions } from '@app/models/common/application-processing-permissions.model';
import { ApplicationsRegisterData } from '@app/models/common/applications-register-data.model';
import { ApplicationsAdministrationService } from '@app/services/administration-app/applications-administration.service';
import { LegalEntitiesAdministrationService } from '@app/services/administration-app/legal-entities-administration.service';
import { TLMatDialog } from '@app/shared/components/dialog-wrapper/tl-mat-dialog';
import { PermissionsEnum } from '@app/shared/enums/permissions.enum';
import { EditLegalEntityComponent } from '@app/components/common-app/legals/edit-legal-entity/edit-legal-entity.component';
import { EditLegalAssociationComponent } from '@app/components/common-app/legal-associations/edit-legal-association/edit-legal-association.component';

type LegalsApplicationsRegisterDataType =
    ApplicationsRegisterData<EditLegalEntityComponent>
    | ApplicationsRegisterData<EditLegalAssociationComponent>;

@Component({
    selector: 'legal-entities-applications',
    templateUrl: './legal-entities-applications.component.html'
})
export class LegalEntitiesApplicationsComponent {
    public service: LegalEntitiesAdministrationService;
    public applicationsService: ApplicationsAdministrationService;
    public processingPermissions: Map<PageCodeEnum, ApplicationProcessingPermissions>;
    public applicationsRegisterData!: Map<PageCodeEnum, LegalsApplicationsRegisterDataType>;

    public constructor(
        service: LegalEntitiesAdministrationService,
        applicationsService: ApplicationsAdministrationService,
        editDialog: TLMatDialog<EditLegalEntityComponent>,
        assocEditDialog: TLMatDialog<EditLegalAssociationComponent>,
        translationService: FuseTranslationLoaderService
    ) {
        this.service = service;
        this.applicationsService = applicationsService;

        const processingPermissions: ApplicationProcessingPermissions = new ApplicationProcessingPermissions({
            addPermission: PermissionsEnum.LegalEntitiesApplicationsAddRecords,
            editPermission: PermissionsEnum.LegalEntitiesApplicationsEditRecords,
            deletePermission: PermissionsEnum.LegalEntitiesApplicationsDeleteRecords,
            restorePermission: PermissionsEnum.LegalEntitiesApplicationsRestoreRecords,
            cancelPermssion: PermissionsEnum.LegalEntitiesApplicationsCancel,
            inspectAndCorrectPermssion: PermissionsEnum.LegalEntitiesApplicationsInspectAndCorrectRegiXData,
            processPaymentDataPermission: PermissionsEnum.LegalEntitiesApplicationsProcessPaymentData,
            checkDataRegularityPermission: PermissionsEnum.LegalEntitiesApplicationsCheckDataRegularity,
            addAdministrativeActPermission: PermissionsEnum.LegalEntitiesAddRecords,
            readAdministrativeActPermission: PermissionsEnum.LegalEntitiesRead,
            canReAssignApplicationsPermission: PermissionsEnum.ReAssignApplication
        });

        this.processingPermissions = new Map<PageCodeEnum, ApplicationProcessingPermissions>([
            [
                PageCodeEnum.LE,
                processingPermissions
            ],
            [
                PageCodeEnum.Assocs,
                processingPermissions
            ]
        ]);

        this.applicationsRegisterData = new Map<PageCodeEnum, LegalsApplicationsRegisterDataType>([
            [
                PageCodeEnum.LE,
                new ApplicationsRegisterData<EditLegalEntityComponent>({
                    editDialog: editDialog,
                    editDialogTCtor: EditLegalEntityComponent,
                    addRegisterDialogTitle: translationService.getValue('legal-entities-page.add-legal-entity-dialog-title'),
                    editApplicationDialogTitle: translationService.getValue('legal-entities-page.edit-legal-entity-application-dialog-title'),
                    editRegixDataDialogTitle: translationService.getValue('legal-entities-page.edit-legal-entity-application-regix-data-dialog-title'),
                    viewApplicationDialogTitle: translationService.getValue('legal-entities-page.view-legal-entity-application-dialog-title'),
                    viewRegisterDialogTitle: translationService.getValue('legal-entities-page.view-legal-entity-dialog-title'),
                    viewRegixDataDialogTitle: translationService.getValue('legal-entities-page.view-legal-entity-application-regix-data-dialog-title'),
                    viewAndConfrimDataRegularityTitle: translationService.getValue('legal-entities-page.view-legal-entity-appl-and-confirm-regularity-title')
                })
            ],
            [
                PageCodeEnum.Assocs,
                new ApplicationsRegisterData<EditLegalAssociationComponent>({
                    editDialog: assocEditDialog,
                    editDialogTCtor: EditLegalAssociationComponent,
                    addRegisterDialogTitle: translationService.getValue('legal-association.add-association-dialog-title'),
                    editApplicationDialogTitle: translationService.getValue('legal-association.edit-association-application-dialog-title'),
                    editRegixDataDialogTitle: translationService.getValue('legal-association.edit-association-application-regix-data-dialog-title'),
                    viewApplicationDialogTitle: translationService.getValue('legal-association.view-association-application-dialog-title'),
                    viewRegisterDialogTitle: translationService.getValue('legal-association.view-association-dialog-title'),
                    viewRegixDataDialogTitle: translationService.getValue('legal-association.view-association-application-regix-data-dialog-title'),
                    viewAndConfrimDataRegularityTitle: translationService.getValue('legal-association.view-association-appl-and-confirm-regularity-title')
                })
            ]
        ]);
    }
}