import { Component } from '@angular/core';
import { Observable } from 'rxjs';

import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { PageCodeEnum } from '@app/enums/page-code.enum';
import { IApplicationsService } from '@app/interfaces/administration-app/applications.interface';
import { ApplicationProcessingPermissions } from '@app/models/common/application-processing-permissions.model';
import { ApplicationsRegisterData } from '@app/models/common/applications-register-data.model';
import { ApplicationsAdministrationService } from '@app/services/administration-app/applications-administration.service';
import { QualifiedFishersService } from '@app/services/administration-app/qualified-fishers.service';
import { TLMatDialog } from '@app/shared/components/dialog-wrapper/tl-mat-dialog';
import { PermissionsEnum } from '@app/shared/enums/permissions.enum';
import { EditFisherComponent } from '@app/components/common-app/qualified-fishers/edit-fisher.component';
import { DuplicatesApplicationComponent } from '@app/components/common-app/duplicates/duplicates-application.component';
import { ApplicationRegisterDTO } from '@app/models/generated/dtos/ApplicationRegisterDTO';
import { IDuplicatesRegisterService } from '@app/interfaces/common-app/duplicates-register.interface';
import { DuplicatesRegisterAdministrationService } from '@app/services/administration-app/duplicates-register-administration.service';
import { DialogParamsModel } from '@app/models/common/dialog-params.model';
import { HeaderCloseFunction } from '@app/shared/components/dialog-wrapper/interfaces/header-cancel-button.interface';

type QualifiedFisherApplicationsRegisterDataType =
    ApplicationsRegisterData<EditFisherComponent> |
    ApplicationsRegisterData<DuplicatesApplicationComponent>;

@Component({
    selector: 'qualified-fishers-applications',
    templateUrl: './qualified-fishers-applications.component.html'
})
export class QualifiedFishersApplicationsComponent {
    public service: QualifiedFishersService;
    public applicationsService: IApplicationsService;
    public processingPermissions: Map<PageCodeEnum, ApplicationProcessingPermissions>;
    public applicationsRegisterData!: Map<PageCodeEnum, QualifiedFisherApplicationsRegisterDataType>;

    private translate: FuseTranslationLoaderService;
    private duplicatesService: IDuplicatesRegisterService;
    private dupDialog: TLMatDialog<DuplicatesApplicationComponent>;

    public constructor(
        service: QualifiedFishersService,
        applicationsService: ApplicationsAdministrationService,
        translate: FuseTranslationLoaderService,
        duplicatesService: DuplicatesRegisterAdministrationService,
        editDialog: TLMatDialog<EditFisherComponent>,
        dupDialog: TLMatDialog<DuplicatesApplicationComponent>,
        translationService: FuseTranslationLoaderService
    ) {
        this.service = service;
        this.applicationsService = applicationsService;
        this.translate = translate;
        this.duplicatesService = duplicatesService;
        this.dupDialog = dupDialog;

        const processingPermissions: ApplicationProcessingPermissions = new ApplicationProcessingPermissions({
            addPermission: PermissionsEnum.QualifiedFishersApplicationsAddRecords,
            editPermission: PermissionsEnum.QualifiedFishersApplicationsEditRecords,
            deletePermission: PermissionsEnum.QualifiedFishersApplicationsDeleteRecords,
            restorePermission: PermissionsEnum.QualifiedFishersApplicationsRestoreRecords,
            cancelPermssion: PermissionsEnum.QualifiedFishersApplicationsCancel,
            inspectAndCorrectPermssion: PermissionsEnum.QualifiedFishersApplicationsInspectAndCorrectRegiXData,
            processPaymentDataPermission: PermissionsEnum.QualifiedFishersApplicationsProcessPaymentData,
            checkDataRegularityPermission: PermissionsEnum.QualifiedFishersApplicationsCheckDataRegularity,
            addAdministrativeActPermission: PermissionsEnum.QualifiedFishersAddRecords,
            readAdministrativeActPermission: PermissionsEnum.QualifiedFishersRead,
            canReAssignApplicationsPermission: PermissionsEnum.ReAssignApplication
        });

        this.processingPermissions = new Map<PageCodeEnum, ApplicationProcessingPermissions>([
            [
                PageCodeEnum.CommFishLicense,
                processingPermissions
            ],
            [
                PageCodeEnum.CompetencyDup,
                processingPermissions
            ]
        ]);

        this.applicationsRegisterData = new Map<PageCodeEnum, QualifiedFisherApplicationsRegisterDataType>([
            [
                PageCodeEnum.CommFishLicense,
                new ApplicationsRegisterData<EditFisherComponent>({
                    editDialog: editDialog,
                    editDialogTCtor: EditFisherComponent,
                    addRegisterDialogTitle: translationService.getValue('qualified-fishers-page.add-qualified-fisher-dialog-title'),
                    editApplicationDialogTitle: translationService.getValue('qualified-fishers-page.edit-qualified-fisher-application-dialog-title'),
                    editRegixDataDialogTitle: translationService.getValue('qualified-fishers-page.edit-qualified-fisher-application-regix-data-dialog-title'),
                    viewApplicationDialogTitle: translationService.getValue('qualified-fishers-page.view-qualified-fisher-application-dialog-title'),
                    viewRegisterDialogTitle: translationService.getValue('qualified-fishers-page.view-qualified-fisher-dialog-title'),
                    viewRegixDataDialogTitle: translationService.getValue('qualified-fishers-page.view-qualified-fisher-application-regix-data-dialog-title'),
                    viewAndConfrimDataRegularityTitle: translationService.getValue('qualified-fishers-page.view-qualified-fisher-appl-and-confirm-regularity-title')
                })
            ],
            [
                PageCodeEnum.CompetencyDup,
                new ApplicationsRegisterData<DuplicatesApplicationComponent>({
                    editDialog: dupDialog,
                    editDialogTCtor: DuplicatesApplicationComponent,
                    addRegisterDialogTitle: translationService.getValue('duplicates.add-qualified-fisher-dialog-title'),
                    editApplicationDialogTitle: translationService.getValue('duplicates.edit-qualified-fisher-application-dialog-title'),
                    editRegixDataDialogTitle: translationService.getValue('duplicates.edit-qualified-fisher-application-regix-data-dialog-title'),
                    viewApplicationDialogTitle: translationService.getValue('duplicates.view-qualified-fisher-application-dialog-title'),
                    viewRegisterDialogTitle: translationService.getValue('duplicates.view-qualified-fisher-dialog-title'),
                    viewRegixDataDialogTitle: translationService.getValue('duplicates.view-qualified-fisher-application-regix-data-dialog-title'),
                    viewAndConfrimDataRegularityTitle: translationService.getValue('duplicates.view-qualified-fisher-appl-and-confirm-regularity-title'),
                    createRegisterCallback: this.openDuplicateDialog.bind(this)
                })
            ]
        ]);
    }

    private openDuplicateDialog(application: ApplicationRegisterDTO): Observable<any> {
        return this.dupDialog.openWithTwoButtons({
            TCtor: DuplicatesApplicationComponent,
            title: this.translate.getValue('duplicates.add-qualified-fisher-dialog-title'),
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

    private closeDuplicateDialogBtnClicked(closeFn: HeaderCloseFunction): void {
        closeFn();
    }
}
