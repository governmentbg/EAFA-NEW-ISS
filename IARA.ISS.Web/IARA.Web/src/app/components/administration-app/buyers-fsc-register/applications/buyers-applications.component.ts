import { Component } from '@angular/core';
import { Observable, Subject } from 'rxjs';

import { IApplicationsService } from '@app/interfaces/administration-app/applications.interface';
import { ApplicationProcessingPermissions } from '@app/models/common/application-processing-permissions.model';
import { BuyersAdministrationService } from '@app/services/administration-app/buyers-administration.service';
import { ApplicationsAdministrationService } from '@app/services/administration-app/applications-administration.service';
import { TLMatDialog } from '@app/shared/components/dialog-wrapper/tl-mat-dialog';
import { PermissionsEnum } from '@app/shared/enums/permissions.enum';
import { EditBuyersComponent } from '@app/components/common-app/buyers/edit-buyers.component';
import { PageCodeEnum } from '@app/enums/page-code.enum';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { ApplicationsRegisterData } from '@app/models/common/applications-register-data.model';
import { BuyerChangeOfCircumstancesComponent } from '@app/components/common-app/buyers/buyer-change-of-circumstances/buyer-change-of-circumstances.component';
import { ApplicationRegisterDTO } from '@app/models/generated/dtos/ApplicationRegisterDTO';
import { DialogParamsModel } from '@app/models/common/dialog-params.model';
import { BuyerEditDTO } from '@app/models/generated/dtos/BuyerEditDTO';
import { IHeaderAuditButton } from '@app/shared/components/dialog-wrapper/interfaces/header-audit-button.interface';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { HeaderCloseFunction } from '@app/shared/components/dialog-wrapper/interfaces/header-cancel-button.interface';
import { BuyerTerminationComponent } from '@app/components/common-app/buyers/buyer-termination/buyer-termination.component';
import { BuyerStatusesEnum } from '@app/enums/buyer-statuses.enum';
import { DuplicatesApplicationComponent } from '@app/components/common-app/duplicates/duplicates-application.component';
import { IDuplicatesRegisterService } from '@app/interfaces/common-app/duplicates-register.interface';
import { DuplicatesRegisterAdministrationService } from '@app/services/administration-app/duplicates-register-administration.service';

type BuyersFirstSaleCenterApplicationsRegisterDataType =
    ApplicationsRegisterData<EditBuyersComponent> |
    ApplicationsRegisterData<BuyerChangeOfCircumstancesComponent> |
    ApplicationsRegisterData<BuyerTerminationComponent> |
    ApplicationsRegisterData<DuplicatesApplicationComponent>;

@Component({
    selector: 'buyers-applications',
    templateUrl: './buyers-applications.component.html'
})
export class BuyersFSCApplicationsComponent {
    public readonly service: BuyersAdministrationService;
    public readonly applicationsService: IApplicationsService;
    public readonly processingPermissions: Map<PageCodeEnum, ApplicationProcessingPermissions>;
    public readonly applicationsRegisterData: Map<PageCodeEnum, BuyersFirstSaleCenterApplicationsRegisterDataType>;

    private readonly translationService: FuseTranslationLoaderService;
    private readonly duplicatesService: IDuplicatesRegisterService;
    private readonly buyerEditDialog: TLMatDialog<EditBuyersComponent>;
    private readonly dupDialog: TLMatDialog<DuplicatesApplicationComponent>;

    public constructor(
        service: BuyersAdministrationService,
        applicationsService: ApplicationsAdministrationService,
        duplicatesService: DuplicatesRegisterAdministrationService,
        editDialog: TLMatDialog<EditBuyersComponent>,
        dupDialog: TLMatDialog<DuplicatesApplicationComponent>,
        translationService: FuseTranslationLoaderService,
        cocDialog: TLMatDialog<BuyerChangeOfCircumstancesComponent>,
        termDialog: TLMatDialog<BuyerTerminationComponent>
    ) {
        this.service = service;
        this.applicationsService = applicationsService;
        this.translationService = translationService;
        this.duplicatesService = duplicatesService;
        this.buyerEditDialog = editDialog;
        this.dupDialog = dupDialog;

        const processingPermissions: ApplicationProcessingPermissions = new ApplicationProcessingPermissions({
            addPermission: PermissionsEnum.BuyersApplicationsAddRecords,
            editPermission: PermissionsEnum.BuyersApplicationsEditRecords,
            deletePermission: PermissionsEnum.BuyersApplicationsDeleteRecords,
            restorePermission: PermissionsEnum.BuyersApplicationsRestoreRecords,
            cancelPermssion: PermissionsEnum.BuyersApplicationsCancel,
            inspectAndCorrectPermssion: PermissionsEnum.BuyersApplicationsInspectAndCorrectRegiXData,
            processPaymentDataPermission: PermissionsEnum.BuyersApplicationsProcessPaymentData,
            checkDataRegularityPermission: PermissionsEnum.BuyersApplicationsCheckDataRegularity,
            addAdministrativeActPermission: PermissionsEnum.BuyersAddRecords,
            readAdministrativeActPermission: PermissionsEnum.BuyersRead,
            canReAssignApplicationsPermission: PermissionsEnum.ReAssignApplication
        });

        this.processingPermissions = new Map<PageCodeEnum, ApplicationProcessingPermissions>([
            [ PageCodeEnum.RegFirstSaleBuyer, processingPermissions ],
            [ PageCodeEnum.RegFirstSaleCenter, processingPermissions ],
            [ PageCodeEnum.ChangeFirstSaleBuyer, processingPermissions ],
            [ PageCodeEnum.ChangeFirstSaleCenter, processingPermissions ],
            [ PageCodeEnum.TermFirstSaleBuyer, processingPermissions ],
            [ PageCodeEnum.TermFirstSaleCenter, processingPermissions ],
            [ PageCodeEnum.DupFirstSaleBuyer, processingPermissions ],
            [ PageCodeEnum.DupFirstSaleCenter, processingPermissions ]
        ]);

        this.applicationsRegisterData = new Map<PageCodeEnum, BuyersFirstSaleCenterApplicationsRegisterDataType>([
            [
                PageCodeEnum.RegFirstSaleBuyer,
                new ApplicationsRegisterData<EditBuyersComponent>({
                    editDialog: editDialog,
                    editDialogTCtor: EditBuyersComponent,
                    addRegisterDialogTitle: this.translationService.getValue('buyers-and-sales-centers.add-buyer-dialog-title'),
                    editApplicationDialogTitle: this.translationService.getValue('buyers-and-sales-centers.edit-buyer-application-dialog-title'),
                    editRegixDataDialogTitle: this.translationService.getValue('buyers-and-sales-centers.edit-buyer-application-regix-data-dialog-title'),
                    viewApplicationDialogTitle: this.translationService.getValue('buyers-and-sales-centers.view-buyer-application-dialog-title'),
                    viewRegisterDialogTitle: this.translationService.getValue('buyers-and-sales-centers.view-buyer-dialog-title'),
                    viewRegixDataDialogTitle: this.translationService.getValue('buyers-and-sales-centers.view-buyer-application-regix-data-dialog-title'),
                    viewAndConfrimDataRegularityTitle: this.translationService.getValue('buyers-and-sales-centers.view-buyer-appl-and-confirm-regularity-title')
                })
            ],
            [
                PageCodeEnum.RegFirstSaleCenter,
                new ApplicationsRegisterData<EditBuyersComponent>({
                    editDialog: editDialog,
                    editDialogTCtor: EditBuyersComponent,
                    addRegisterDialogTitle: this.translationService.getValue('buyers-and-sales-centers.add-first-sale-dialog-title'),
                    editApplicationDialogTitle: this.translationService.getValue('buyers-and-sales-centers.edit-first-sale-application-dialog-title'),
                    editRegixDataDialogTitle: this.translationService.getValue('buyers-and-sales-centers.edit-first-sale-application-regix-data-dialog-title'),
                    viewApplicationDialogTitle: this.translationService.getValue('buyers-and-sales-centers.view-first-sale-application-dialog-title'),
                    viewRegisterDialogTitle: this.translationService.getValue('buyers-and-sales-centers.view-first-sale-dialog-title'),
                    viewRegixDataDialogTitle: this.translationService.getValue('buyers-and-sales-centers.view-first-sale-application-regix-data-dialog-title'),
                    viewAndConfrimDataRegularityTitle: this.translationService.getValue('buyers-and-sales-centers.view-center-appl-and-confirm-regularity-title')
                })
            ],
            [
                PageCodeEnum.ChangeFirstSaleBuyer,
                new ApplicationsRegisterData<BuyerChangeOfCircumstancesComponent>({
                    editDialog: cocDialog,
                    editDialogTCtor: BuyerChangeOfCircumstancesComponent,
                    editApplicationDialogTitle: this.translationService.getValue('buyers-and-sales-centers.edit-change-buyer-application-dialog-title'),
                    editRegixDataDialogTitle: this.translationService.getValue('buyers-and-sales-centers.edit-change-buyer-application-regix-data-dialog-title'),
                    viewApplicationDialogTitle: this.translationService.getValue('buyers-and-sales-centers.view-change-buyer-application-dialog-title'),
                    viewRegixDataDialogTitle: this.translationService.getValue('buyers-and-sales-centers.view-change-buyer-application-regix-data-dialog-title'),
                    viewAndConfrimDataRegularityTitle: this.translationService.getValue('buyers-and-sales-centers.view-change-buyer-appl-and-confirm-regularity-title'),
                    createRegisterCallback: this.openEditBuyerForCircumstances.bind(this)
                })
            ],
            [
                PageCodeEnum.ChangeFirstSaleCenter,
                new ApplicationsRegisterData<BuyerChangeOfCircumstancesComponent>({
                    editDialog: cocDialog,
                    editDialogTCtor: BuyerChangeOfCircumstancesComponent,
                    editApplicationDialogTitle: this.translationService.getValue('buyers-and-sales-centers.edit-change-first-sale-application-dialog-title'),
                    editRegixDataDialogTitle: this.translationService.getValue('buyers-and-sales-centers.edit-change-first-sale-application-regix-data-dialog-title'),
                    viewApplicationDialogTitle: this.translationService.getValue('buyers-and-sales-centers.view-change-first-sale-application-dialog-title'),
                    viewRegixDataDialogTitle: this.translationService.getValue('buyers-and-sales-centers.view-change-first-sale-application-regix-data-dialog-title'),
                    viewAndConfrimDataRegularityTitle: this.translationService.getValue('buyers-and-sales-centers.view-change-first-sale-appl-and-confirm-regularity-title'),
                    createRegisterCallback: this.openEditBuyerForCircumstances.bind(this)
                })
            ],
            [
                PageCodeEnum.TermFirstSaleBuyer,
                new ApplicationsRegisterData<BuyerTerminationComponent>({
                    editDialog: termDialog,
                    editDialogTCtor: BuyerTerminationComponent,
                    editApplicationDialogTitle: this.translationService.getValue('buyers-and-sales-centers.edit-terminate-buyer-application-dialog-title'),
                    editRegixDataDialogTitle: this.translationService.getValue('buyers-and-sales-centers.edit-terminate-buyer-application-regix-data-dialog-title'),
                    viewApplicationDialogTitle: this.translationService.getValue('buyers-and-sales-centers.view-terminate-buyer-application-dialog-title'),
                    viewRegixDataDialogTitle: this.translationService.getValue('buyers-and-sales-centers.view-terminate-buyer-application-regix-data-dialog-title'),
                    viewAndConfrimDataRegularityTitle: this.translationService.getValue('buyers-and-sales-centers.view-terminate-buyer-appl-and-confirm-regularity-title'),
                    createRegisterCallback: this.openEditBuyerForTermination.bind(this)
                })
            ],
            [
                PageCodeEnum.TermFirstSaleCenter,
                new ApplicationsRegisterData<BuyerTerminationComponent>({
                    editDialog: termDialog,
                    editDialogTCtor: BuyerTerminationComponent,
                    editApplicationDialogTitle: this.translationService.getValue('buyers-and-sales-centers.edit-terminate-center-application-dialog-title'),
                    editRegixDataDialogTitle: this.translationService.getValue('buyers-and-sales-centers.edit-terminate-center-application-regix-data-dialog-title'),
                    viewApplicationDialogTitle: this.translationService.getValue('buyers-and-sales-centers.view-terminate-center-application-dialog-title'),
                    viewRegixDataDialogTitle: this.translationService.getValue('buyers-and-sales-centers.view-terminate-center-application-regix-data-dialog-title'),
                    viewAndConfrimDataRegularityTitle: this.translationService.getValue('buyers-and-sales-centers.view-terminate-center-appl-and-confirm-regularity-title'),
                    createRegisterCallback: this.openEditBuyerForTermination.bind(this)
                })
            ],
            [
                PageCodeEnum.DupFirstSaleBuyer,
                new ApplicationsRegisterData<DuplicatesApplicationComponent>({
                    editDialog: dupDialog,
                    editDialogTCtor: DuplicatesApplicationComponent,
                    addRegisterDialogTitle: this.translationService.getValue('duplicates.add-buyer-dialog-title'),
                    editApplicationDialogTitle: this.translationService.getValue('duplicates.edit-buyer-application-dialog-title'),
                    editRegixDataDialogTitle: this.translationService.getValue('duplicates.edit-buyer-application-regix-data-dialog-title'),
                    viewApplicationDialogTitle: this.translationService.getValue('duplicates.view-buyer-application-dialog-title'),
                    viewRegisterDialogTitle: this.translationService.getValue('duplicates.view-buyer-dialog-title'),
                    viewRegixDataDialogTitle: this.translationService.getValue('duplicates.view-buyer-application-regix-data-dialog-title'),
                    viewAndConfrimDataRegularityTitle: this.translationService.getValue('duplicates.view-buyer-appl-and-confirm-regularity-title'),
                    createRegisterCallback: this.openBuyerDuplicateDialog.bind(this)
                })
            ],
            [
                PageCodeEnum.DupFirstSaleCenter,
                new ApplicationsRegisterData<DuplicatesApplicationComponent>({
                    editDialog: dupDialog,
                    editDialogTCtor: DuplicatesApplicationComponent,
                    addRegisterDialogTitle: this.translationService.getValue('duplicates.add-first-sale-dialog-title'),
                    editApplicationDialogTitle: this.translationService.getValue('duplicates.edit-first-sale-application-dialog-title'),
                    editRegixDataDialogTitle: this.translationService.getValue('duplicates.edit-first-sale-application-regix-data-dialog-title'),
                    viewApplicationDialogTitle: this.translationService.getValue('duplicates.view-first-sale-application-dialog-title'),
                    viewRegisterDialogTitle: this.translationService.getValue('duplicates.view-first-sale-dialog-title'),
                    viewRegixDataDialogTitle: this.translationService.getValue('duplicates.view-first-sale-application-regix-data-dialog-title'),
                    viewAndConfrimDataRegularityTitle: this.translationService.getValue('duplicates.view-first-sale-appl-and-confirm-regularity-title'),
                    createRegisterCallback: this.openFirstSaleDuplicateDialog.bind(this)
                })
            ]
        ]);
    }

    private openEditBuyerForCircumstances(application: ApplicationRegisterDTO): Observable<any> {
        const saveOrEditDone: Subject<any> = new Subject<any>();

        const data: DialogParamsModel = new DialogParamsModel({
            id: undefined,
            applicationId: application.id,
            service: this.service,
            pageCode: application.pageCode
        });

        let title: string = '';

        if (application.pageCode === PageCodeEnum.ChangeFirstSaleBuyer) {
            title = this.translationService.getValue('buyers-and-sales-centers.coc-buyer-dialog-title');
        }
        else if (application.pageCode === PageCodeEnum.ChangeFirstSaleCenter) {
            title = this.translationService.getValue('buyers-and-sales-centers.coc-first-sale-center-dialog-title');
        }
        else {
            throw new Error('Invalid application page code for buyer change of circumstances.');
        }

        this.service.getBuyerFromChangeOfCircumstancesApplication(data.applicationId!).subscribe({
            next: (model: BuyerEditDTO) => {
                data.model = model;
                data.model.applicationId = application.id;
                this.openBuyerEditDialog(data, model.buyerStatus!, title).subscribe((result: any) => {
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

    private openEditBuyerForTermination(application: ApplicationRegisterDTO): Observable<any> {
        const saveOrEditDone: Subject<any> = new Subject<any>();

        const data: DialogParamsModel = new DialogParamsModel({
            id: undefined,
            applicationId: application.id,
            service: this.service,
            pageCode: application.pageCode
        });

        let title: string = '';

        if (application.pageCode === PageCodeEnum.TermFirstSaleBuyer) {
            title = this.translationService.getValue('buyers-and-sales-centers.term-buyer-dialog-title');
        }
        else if (application.pageCode === PageCodeEnum.TermFirstSaleCenter) {
            title = this.translationService.getValue('buyers-and-sales-centers.term-first-sale-center-dialog-title');
        }
        else {
            throw new Error('Invalid application page code for buyer termination.');
        }

        this.service.getBuyerFromTerminationApplication(data.applicationId!).subscribe({
            next: (model: BuyerEditDTO) => {
                data.model = model;
                data.model.applicationId = application.id;
                this.openBuyerEditDialog(data, model.buyerStatus!, title).subscribe((result: any) => {
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

    private openBuyerEditDialog(data: DialogParamsModel, status: BuyerStatusesEnum, title: string): Observable<any> {
        const auditBtn: IHeaderAuditButton = {
            id: data.model!.id!,
            getAuditRecordData: this.service.getSimpleAudit.bind(this.service),
            tableName: 'BuyerRegister'
        };

        const rightButtons: IActionInfo[] = [];

        if (status === BuyerStatusesEnum.Canceled) {
            rightButtons.push({
                id: 'activate',
                color: 'accent',
                translateValue: 'buyers-and-sales-centers.activate',
                isVisibleInViewMode: true
            });
        }
        else {
            rightButtons.push({
                id: 'cancel',
                color: 'warn',
                translateValue: 'buyers-and-sales-centers.cancel'
            });
        }

        return this.buyerEditDialog.open({
            TCtor: EditBuyersComponent,
            title: title,
            translteService: this.translationService,
            componentData: data,
            disableDialogClose: true,
            headerAuditButton: auditBtn,
            headerCancelButton: {
                cancelBtnClicked: this.closeBuyerEditBtnClicked.bind(this)
            },
            saveBtn: {
                id: 'save',
                color: 'accent',
                translateValue: this.translationService.getValue('buyers-and-sales-centers.complete-application')
            },
            cancelBtn: {
                id: 'cancel',
                color: 'primary',
                translateValue: this.translationService.getValue('common.cancel'),
            },
            rightSideActionsCollection: rightButtons,
            viewMode: false
        }, '1400px');
    }

    private closeBuyerEditBtnClicked(closeFn: HeaderCloseFunction): void {
        closeFn();
    }

    private openBuyerDuplicateDialog(application: ApplicationRegisterDTO): Observable<any> {
        const title: string = this.translationService.getValue('duplicates.add-buyer-dialog-title');
        return this.openDuplicateDialog(application, title);
    }

    private openFirstSaleDuplicateDialog(application: ApplicationRegisterDTO): Observable<any> {
        const title: string = this.translationService.getValue('duplicates.add-first-sale-dialog-title');
        return this.openDuplicateDialog(application, title);
    }

    private openDuplicateDialog(application: ApplicationRegisterDTO, title: string): Observable<any> {
        return this.dupDialog.openWithTwoButtons({
            TCtor: DuplicatesApplicationComponent,
            title: title,
            translteService: this.translationService,
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
