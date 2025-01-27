import { Component } from '@angular/core';
import { Observable, Subject } from 'rxjs';

import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { PageCodeEnum } from '@app/enums/page-code.enum';
import { ApplicationProcessingPermissions } from '@app/models/common/application-processing-permissions.model';
import { ApplicationsRegisterData } from '@app/models/common/applications-register-data.model';
import { ApplicationsAdministrationService } from '@app/services/administration-app/applications-administration.service';
import { AquacultureFacilitiesAdministrationService } from '@app/services/administration-app/aquaculture-facilities-administration.service';
import { TLMatDialog } from '@app/shared/components/dialog-wrapper/tl-mat-dialog';
import { PermissionsEnum } from '@app/shared/enums/permissions.enum';
import { AquacultureChangeOfCircumstancesComponent } from '@app/components/common-app/aquaculture-facilities/aquaculture-change-of-circumstances/aquaculture-change-of-circumstances.component';
import { AquacultureDeregistrationComponent } from '@app/components/common-app/aquaculture-facilities/aquaculture-deregistration/aquaculture-deregistration.component';
import { EditAquacultureFacilityComponent } from '@app/components/common-app/aquaculture-facilities/edit-aquaculture-facility/edit-aquaculture-facility.component';
import { ApplicationRegisterDTO } from '@app/models/generated/dtos/ApplicationRegisterDTO';
import { EditAquacultureFacilityDialogParams } from '@app/components/common-app/aquaculture-facilities/edit-aquaculture-facility/models/edit-aquaculture-facility-dialog-params.model';
import { IHeaderAuditButton } from '@app/shared/components/dialog-wrapper/interfaces/header-audit-button.interface';
import { AquacultureFacilityEditDTO } from '@app/models/generated/dtos/AquacultureFacilityEditDTO';
import { IAquacultureFacilitiesService } from '@app/interfaces/common-app/aquaculture-facilities.interface';
import { HeaderCloseFunction } from '@app/shared/components/dialog-wrapper/interfaces/header-cancel-button.interface';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { AquacultureStatusEnum } from '@app/enums/aquaculture-status.enum';

type AquacultureFacilitiesRegisterDataType =
    ApplicationsRegisterData<EditAquacultureFacilityComponent> |
    ApplicationsRegisterData<AquacultureChangeOfCircumstancesComponent> |
    ApplicationsRegisterData<AquacultureDeregistrationComponent>;

@Component({
    selector: 'aquaculture-facilities-applications',
    templateUrl: './aquaculture-facilities-applications.component.html'
})
export class AquacultureFacilitiesApplicationsComponent {
    public service: IAquacultureFacilitiesService;
    public applicationsService: ApplicationsAdministrationService;
    public processingPermissions: Map<PageCodeEnum, ApplicationProcessingPermissions>;
    public applicationsRegisterData: Map<PageCodeEnum, AquacultureFacilitiesRegisterDataType>;

    private aquacultureEditDialog: TLMatDialog<EditAquacultureFacilityComponent>;
    private translate: FuseTranslationLoaderService;

    public constructor(
        service: AquacultureFacilitiesAdministrationService,
        applicationsService: ApplicationsAdministrationService,
        editDialog: TLMatDialog<EditAquacultureFacilityComponent>,
        cocDialog: TLMatDialog<AquacultureChangeOfCircumstancesComponent>,
        deregDialog: TLMatDialog<AquacultureDeregistrationComponent>,
        aquacultureEditDialog: TLMatDialog<EditAquacultureFacilityComponent>,
        translationService: FuseTranslationLoaderService
    ) {
        this.service = service;
        this.applicationsService = applicationsService;
        this.aquacultureEditDialog = aquacultureEditDialog;
        this.translate = translationService;

        const processingPermissions: ApplicationProcessingPermissions = new ApplicationProcessingPermissions({
            addPermission: PermissionsEnum.AquacultureFacilitiesApplicationsAddRecords,
            editPermission: PermissionsEnum.AquacultureFacilitiesApplicationsEditRecords,
            deletePermission: PermissionsEnum.AquacultureFacilitiesApplicationsDeleteRecords,
            restorePermission: PermissionsEnum.AquacultureFacilitiesApplicationsRestoreRecords,
            cancelPermssion: PermissionsEnum.AquacultureFacilitiesApplicationsCancel,
            inspectAndCorrectPermssion: PermissionsEnum.AquacultureFacilitiesApplicationsInspectAndCorrectRegiXData,
            processPaymentDataPermission: PermissionsEnum.AquacultureFacilitiesApplicationsProcessPaymentData,
            checkDataRegularityPermission: PermissionsEnum.AquacultureFacilitiesApplicationsCheckDataRegularity,
            addAdministrativeActPermission: PermissionsEnum.AquacultureFacilitiesAddRecords,
            readAdministrativeActPermission: PermissionsEnum.AquacultureFacilitiesRead,
            canReAssignApplicationsPermission: PermissionsEnum.ReAssignApplication
        });

        this.processingPermissions = new Map<PageCodeEnum, ApplicationProcessingPermissions>([
            [
                PageCodeEnum.AquaFarmReg,
                processingPermissions
            ],
            [
                PageCodeEnum.AquaFarmChange,
                processingPermissions
            ],
            [
                PageCodeEnum.AquaFarmDereg,
                processingPermissions
            ]
        ]);

        this.applicationsRegisterData = new Map<PageCodeEnum, AquacultureFacilitiesRegisterDataType>([
            [
                PageCodeEnum.AquaFarmReg,
                new ApplicationsRegisterData<EditAquacultureFacilityComponent>({
                    editDialog: editDialog,
                    editDialogTCtor: EditAquacultureFacilityComponent,
                    addRegisterDialogTitle: this.translate.getValue('aquacultures.add-aquaculture-dialog-title'),
                    editApplicationDialogTitle: this.translate.getValue('aquacultures.edit-aquaculture-application-dialog-title'),
                    editRegixDataDialogTitle: this.translate.getValue('aquacultures.edit-aquaculture-application-regix-data-dialog-title'),
                    viewApplicationDialogTitle: this.translate.getValue('aquacultures.view-aquaculture-application-dialog-title'),
                    viewRegisterDialogTitle: this.translate.getValue('aquacultures.view-aquaculture-dialog-title'),
                    viewRegixDataDialogTitle: this.translate.getValue('aquacultures.view-aquaculture-application-regix-data-dialog-title'),
                    viewAndConfrimDataRegularityTitle: this.translate.getValue('aquacultures.view-aquaculture-appl-and-confirm-regularity-title')
                })
            ],
            [
                PageCodeEnum.AquaFarmChange,
                new ApplicationsRegisterData<AquacultureChangeOfCircumstancesComponent>({
                    editDialog: cocDialog,
                    editDialogTCtor: AquacultureChangeOfCircumstancesComponent,
                    editApplicationDialogTitle: this.translate.getValue('aquacultures.edit-change-of-circumstances-dialog-title'),
                    editRegixDataDialogTitle: this.translate.getValue('aquacultures.edit-change-of-circumstances-regix-data-dialog-title'),
                    viewApplicationDialogTitle: this.translate.getValue('aquacultures.view-change-of-circumstances-dialog-title'),
                    viewRegixDataDialogTitle: this.translate.getValue('aquacultures.view-change-of-circumstances-regix-data-dialog-title'),
                    viewAndConfrimDataRegularityTitle: this.translate.getValue('aquacultures.view-aquaculture-change-of-circumstances-appl-and-confirm-regularity-title'),
                    createRegisterCallback: this.openEditAquacultureForCircumstances.bind(this)
                })
            ],
            [
                PageCodeEnum.AquaFarmDereg,
                new ApplicationsRegisterData<AquacultureDeregistrationComponent>({
                    editDialog: deregDialog,
                    editDialogTCtor: AquacultureDeregistrationComponent,
                    editApplicationDialogTitle: this.translate.getValue('aquacultures.edit-deregistration-dialog-title'),
                    editRegixDataDialogTitle: this.translate.getValue('aquacultures.edit-deregistration-regix-data-dialog-title'),
                    viewApplicationDialogTitle: this.translate.getValue('aquacultures.view-deregistration-dialog-title'),
                    viewRegixDataDialogTitle: this.translate.getValue('aquacultures.view-deregistration-regix-data-dialog-title'),
                    viewAndConfrimDataRegularityTitle: this.translate.getValue('aquacultures.view-aquaculture-deregistration-appl-and-confirm-regularity-title'),
                    createRegisterCallback: this.openEditAquacultureForDeregistration.bind(this)
                })
            ]
        ]);
    }

    private openEditAquacultureForCircumstances(application: ApplicationRegisterDTO): Observable<any> {
        const saveOrEditDone: Subject<any> = new Subject<any>();

        const data: EditAquacultureFacilityDialogParams = new EditAquacultureFacilityDialogParams({
            id: undefined,
            applicationId: application.id,
            service: this.service,
            isChangeOfCircumstancesApplication: true,
            isDeregistrationApplication: false
        });

        const title: string = this.translate.getValue('aquacultures.coc-dialog-title');

        this.service.getAquacultureFromChangeOfCircumstancesApplication(data.applicationId!).subscribe({
            next: (model: AquacultureFacilityEditDTO) => {
                data.model = model;
                data.model.applicationId = application.id;
                this.openAquacultureEditDialog(data, title, true).subscribe((result: any) => {
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

    private openEditAquacultureForDeregistration(application: ApplicationRegisterDTO): Observable<any> {
        const saveOrEditDone: Subject<any> = new Subject<any>();

        const data: EditAquacultureFacilityDialogParams = new EditAquacultureFacilityDialogParams({
            id: undefined,
            applicationId: application.id,
            service: this.service,
            isChangeOfCircumstancesApplication: false,
            isDeregistrationApplication: true
        });

        const title: string = this.translate.getValue('aquacultures.dereg-dialog-title');

        this.service.getAquacultureFromDeregistrationApplication(data.applicationId!).subscribe({
            next: (model: AquacultureFacilityEditDTO) => {
                data.model = model;
                data.model.applicationId = application.id;
                this.openAquacultureEditDialog(data, title, false).subscribe((result: any) => {
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

    private openAquacultureEditDialog(data: EditAquacultureFacilityDialogParams, title: string, complete: boolean): Observable<any> {
        const auditBtn: IHeaderAuditButton = {
            id: data.model!.id!,
            getAuditRecordData: this.service.getSimpleAudit.bind(this.service),
            tableName: 'RAquaSt.AquacultureFacilityRegister'
        };

        const rightButtons: IActionInfo[] = [];
        if (data.model?.status === AquacultureStatusEnum.Canceled) {
            rightButtons.push({
                id: 'activate',
                color: 'accent',
                translateValue: 'aquacultures.activate'
            });
        }
        else {
            rightButtons.push({
                id: 'cancel',
                color: 'warn',
                translateValue: 'aquacultures.cancel'
            });
        }

        return this.aquacultureEditDialog.open({
            TCtor: EditAquacultureFacilityComponent,
            title: title,
            translteService: this.translate,
            componentData: data,
            disableDialogClose: true,
            headerAuditButton: auditBtn,
            headerCancelButton: {
                cancelBtnClicked: this.closeAquacultureEditBtnClicked.bind(this)
            },
            saveBtn: complete
                ? {
                    id: 'save',
                    color: 'accent',
                    translateValue: this.translate.getValue('aquacultures.complete-application')
                }
                : undefined,
            cancelBtn: {
                id: 'cancel',
                color: 'primary',
                translateValue: this.translate.getValue('common.cancel'),
            },
            rightSideActionsCollection: rightButtons,
            viewMode: false
        }, '1400px');
    }

    private closeAquacultureEditBtnClicked(closeFn: HeaderCloseFunction): void {
        closeFn();
    }
}