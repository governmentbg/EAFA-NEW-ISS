import { Component, Input } from '@angular/core';

import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { DuplicatesEntryDTO } from '@app/models/generated/dtos/DuplicatesEntryDTO';
import { IDeliveryService } from '@app/interfaces/common-app/delivery.interface';
import { ApplicationDeliveryDTO } from '@app/models/generated/dtos/ApplicationDeliveryDTO';
import { DeliveryAdministrationService } from '@app/services/administration-app/delivery-administration.service';
import { HeaderCloseFunction } from '@app/shared/components/dialog-wrapper/interfaces/header-cancel-button.interface';
import { TLMatDialog } from '@app/shared/components/dialog-wrapper/tl-mat-dialog';
import { RegisterDeliveryDialogParams } from '@app/shared/components/register-delivery/models/register-delivery-dialog-params.model';
import { RegisterDeliveryComponent } from '@app/shared/components/register-delivery/register-delivery.component';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { DialogParamsModel } from '@app/models/common/dialog-params.model';
import { IDuplicatesRegisterService } from '@app/interfaces/common-app/duplicates-register.interface';
import { DuplicatesRegisterAdministrationService } from '@app/services/administration-app/duplicates-register-administration.service';
import { DuplicatesApplicationComponent } from '../duplicates-application.component';

@Component({
    selector: 'duplicate-entries-table',
    templateUrl: './duplicate-entries-table.component.html'
})
export class DuplicateEntriesTableComponent {
    @Input()
    public entries: DuplicatesEntryDTO[] = [];

    public readonly icIconSize: number = CommonUtils.IC_ICON_SIZE;

    private readonly translate: FuseTranslationLoaderService;
    private readonly deliveryService: IDeliveryService;
    private readonly duplicateService: IDuplicatesRegisterService;
    private readonly deliveryDialog: TLMatDialog<RegisterDeliveryComponent>;
    private readonly dupDialog: TLMatDialog<DuplicatesApplicationComponent>;

    public constructor(
        translate: FuseTranslationLoaderService,
        deliveryService: DeliveryAdministrationService,
        duplicateService: DuplicatesRegisterAdministrationService,
        deliveryDialog: TLMatDialog<RegisterDeliveryComponent>,
        dupDialog: TLMatDialog<DuplicatesApplicationComponent>
    ) {
        this.translate = translate;
        this.deliveryService = deliveryService;
        this.duplicateService = duplicateService;
        this.deliveryDialog = deliveryDialog;
        this.dupDialog = dupDialog;
    }

    public viewDuplicateApplication(entry: DuplicatesEntryDTO): void {
        this.dupDialog.openWithTwoButtons({
            TCtor: DuplicatesApplicationComponent,
            title: this.translate.getValue('duplicates.table-entry-view-application-dialog-title'),
            translteService: this.translate,
            componentData: new DialogParamsModel({
                id: entry.id,
                applicationId: entry.applicationId,
                isReadonly: true,
                isApplication: false,
                isApplicationHistoryMode: false,
                viewMode: true,
                showOnlyRegiXData: false,
                pageCode: entry.pageCode,
                loadRegisterFromApplication: false,
                service: this.duplicateService
            }),
            headerCancelButton: {
                cancelBtnClicked: this.closeViewApplicationDialogBtnClicked.bind(this)
            },
            headerAuditButton: {
                id: entry.id!,
                getAuditRecordData: this.duplicateService.getSimpleAudit.bind(this.duplicateService),
                tableName: 'Appl.DuplicatesRegister'
            },
            rightSideActionsCollection: [{
                id: 'print',
                color: 'accent',
                translateValue: 'duplicates.print',
                isVisibleInViewMode: true
            }],
            viewMode: true
        }, '1200px').subscribe({
            next: () => {
                // nothing to do
            }
        });
    }

    public openDeliveryDialog(entry: DuplicatesEntryDTO): void {
        this.deliveryDialog.openWithTwoButtons({
            TCtor: RegisterDeliveryComponent,
            title: this.translate.getValue('duplicates.table-entry-delivery-data-dialog-title'),
            translteService: this.translate,
            componentData: new RegisterDeliveryDialogParams({
                deliveryId: entry.deliveryId,
                isPublicApp: false,
                service: this.deliveryService,
                pageCode: entry.pageCode,
                registerId: entry.id
            }),
            headerCancelButton: {
                cancelBtnClicked: this.closeDeliveryDataDialogBtnClicked.bind(this)
            },
            headerAuditButton: {
                id: entry.deliveryId!,
                getAuditRecordData: this.deliveryService.getSimpleAudit.bind(this.deliveryService),
                tableName: 'Appl.ApplicationDelivery'
            }
        }, '1200px').subscribe({
            next: (model: ApplicationDeliveryDTO | undefined) => {
                // nothing to do
            }
        });
    }

    private closeViewApplicationDialogBtnClicked(closeFn: HeaderCloseFunction): void {
        closeFn();
    }

    private closeDeliveryDataDialogBtnClicked(closeFn: HeaderCloseFunction): void {
        closeFn();
    }
}