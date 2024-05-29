import { Component, Input, OnChanges, OnInit, Optional, Self, SimpleChanges, ViewChild } from '@angular/core';
import { AbstractControl, FormControl, NgControl } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { HttpErrorResponse } from '@angular/common/http';
import { Observable } from 'rxjs';

import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { SuspensionDataDTO } from '@app/models/generated/dtos/SuspensionDataDTO';
import { PermissionsEnum } from '@app/shared/enums/permissions.enum';
import { PermissionsService } from '@app/shared/services/permissions.service';
import { TLDataTableComponent } from '@app/shared/components/data-table/tl-data-table.component';
import { TLMatDialog } from '@app/shared/components/dialog-wrapper/tl-mat-dialog';
import { EditSuspensionComponent } from './components/edit-suspension/edit-suspension.component';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { SuspnesionDataDialogParams } from '@app/components/common-app/commercial-fishing/models/suspnesion-data-dialog-params.model';
import { IHeaderAuditButton } from '@app/shared/components/dialog-wrapper/interfaces/header-audit-button.interface';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { PageCodeEnum } from '@app/enums/page-code.enum';
import { ISuspensionService } from '@app/interfaces/common-app/suspension.interface';
import { IS_PUBLIC_APP } from '@app/shared/modules/application.modules';
import { SimpleAuditDTO } from '@app/models/generated/dtos/SimpleAuditDTO';
import { HeaderCloseFunction } from '@app/shared/components/dialog-wrapper/interfaces/header-cancel-button.interface';
import { GridRow } from '@app/shared/components/data-table/models/row.model';
import { TLConfirmDialog } from '@app/shared/components/confirmation-dialog/tl-confirm-dialog';
import { SuspensionsDialogParams } from './models/suspensions-dialog-params.model';
import { CustomFormControl } from '@app/shared/utils/custom-form-control';
import { ErrorCode } from '@app/models/common/exception.model';
import { DateUtils } from '@app/shared/utils/date.utils';
import { RequestProperties } from '@app/shared/services/request-properties';

@Component({
    selector: 'suspensions',
    templateUrl: './suspensions.component.html'
})
export class SuspensionsComponent extends CustomFormControl<SuspensionDataDTO[]> implements OnChanges, OnInit, IDialogComponent {
    @Input()
    public isPermitLicense: boolean = false;

    @Input()
    public postOnAdd: boolean = false;

    /**
     * Id of permit license or permit
     */
    @Input()
    public recordId: number | undefined;

    @Input()
    public pageCode!: PageCodeEnum;

    @Input()
    public viewMode: boolean = true;

    @Input()
    public service!: ISuspensionService;

    @Input()
    public showAddButton: boolean = true;

    public suspensions: SuspensionDataDTO[] = [];

    public canAddSuspensionRecords: boolean = false;
    public canEditSuspensionRecords: boolean = false;
    public canDeleteSuspensionRecords: boolean = false;
    public canRestoreSuspensionRecords: boolean = false;

    /**
     * Used only when component in dialog mode (a.k.a independent)
     */
    public hasShipEventExistsOnSameDateError: boolean = false;

    @ViewChild('suspensionsTable')
    private suspensionsTable!: TLDataTableComponent;

    private readonly isPublicApp: boolean;
    private readonly editSuspensionDialog: TLMatDialog<EditSuspensionComponent>;
    private readonly confirmDialog: TLConfirmDialog;
    private readonly translationService: FuseTranslationLoaderService;
    private readonly permissionsService: PermissionsService;
    private readonly snackbar: MatSnackBar;

    public constructor(
        @Self() @Optional() ngControl: NgControl,
        permissionsService: PermissionsService,
        editSuspensionDialog: TLMatDialog<EditSuspensionComponent>,
        confirmDialog: TLConfirmDialog,
        translationService: FuseTranslationLoaderService,
        snackbar: MatSnackBar
    ) {
        super(ngControl);

        this.editSuspensionDialog = editSuspensionDialog;
        this.confirmDialog = confirmDialog;
        this.translationService = translationService;
        this.permissionsService = permissionsService;
        this.snackbar = snackbar;

        this.isPublicApp = IS_PUBLIC_APP;
    }

    public ngOnChanges(changes: SimpleChanges): void {
        if ('isPermitLicense' in changes) {
            this.setSuspensionPermission();
        }
    }

    public ngOnInit(): void {
        if (this.ngControl === null || this.ngControl === undefined) { // if not used as custom form control, get data from server
            this.service.getSuspensions(this.recordId!, this.pageCode).subscribe({
                next: (results: SuspensionDataDTO[]) => {
                    this.suspensions = results;
                }
            });
        }
        else {
            this.initCustomFormControl();
        }
    }

    public setData(data: SuspensionsDialogParams, wrapperData: DialogWrapperData): void {
        this.isPermitLicense = data.isPermitLicense;
        this.recordId = data.recordId;
        this.pageCode = data.pageCode;
        this.viewMode = data.viewMode;
        this.service = data.service;
        this.postOnAdd = data.postOnAdd;

        this.setSuspensionPermission();
    }

    public dialogButtonClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public saveBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        this.suspensions = this.getSuspensionsFromTable();

        if (this.suspensions !== null && this.suspensions !== undefined && this.suspensions.length > 0) { // save data to DB
            this.service.addEditSuspensions(
                this.recordId!,
                this.suspensions,
                this.pageCode
            ).subscribe({
                next: () => {
                    dialogClose(this.suspensions);
                },
                error: (errorResponse: HttpErrorResponse) => {
                    this.handleAddEditErrorResponse(errorResponse);
                }
            });
        }
    }

    public cancelBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public writeValue(value: SuspensionDataDTO[]): void {
        if (value !== null && value !== undefined) {
            this.suspensions = value.slice();
        }
        else {
            this.suspensions = [];

            if (this.ngControl) {
                this.control.updateValueAndValidity();
            }
        }
    }

    protected getValue(): SuspensionDataDTO[] {
        return this.getSuspensionsFromTable();
    }

    protected buildForm(): AbstractControl {
        return new FormControl();
    }

    public addEditSuspension(suspension?: SuspensionDataDTO, viewMode: boolean = false): void {
        let data: SuspnesionDataDialogParams;
        let headerAuditBtn: IHeaderAuditButton | undefined;
        let headerTitle: string = '';

        if (suspension !== null && suspension !== undefined) { // edit
            data = new SuspnesionDataDialogParams({
                model: suspension!,
                isPermit: !this.isPermitLicense,
                recordId: this.recordId,
                pageCode: this.pageCode,
                viewMode: viewMode,
                service: this.service
            });

            if (suspension.id !== undefined && !this.isPublicApp) {
                let getAuditRecordDataMethod: (id: number) => Observable<SimpleAuditDTO>;
                let tableName: string = '';

                switch (this.pageCode) {
                    case PageCodeEnum.CommFish:
                    case PageCodeEnum.RightToFishThirdCountry:
                    case PageCodeEnum.PoundnetCommFish: {
                        getAuditRecordDataMethod = this.service.getPermitSuspensionAudit.bind(this.service);
                        tableName = 'PermitSuspensionChangeHistory';
                    } break;
                    case PageCodeEnum.RightToFishResource:
                    case PageCodeEnum.CatchQuataSpecies:
                    case PageCodeEnum.PoundnetCommFishLic: {
                        getAuditRecordDataMethod = this.service.getPermitLicenseSuspensionAudit.bind(this.service);
                        tableName = 'PermitLicenseSuspensionChangeHistory';
                    } break;
                }

                headerAuditBtn = {
                    id: suspension.id,
                    getAuditRecordData: getAuditRecordDataMethod!,
                    tableName: tableName
                };
            }

            if (viewMode) {
                headerTitle = this.translationService.getValue('suspensions.view-suspension-dialog-title');
            }
            else {
                headerTitle = this.translationService.getValue('suspensions.edit-suspension-dialog-title');
            }
        }
        else { // add
            data = new SuspnesionDataDialogParams({
                viewMode: viewMode,
                isPermit: !this.isPermitLicense,
                recordId: this.recordId,
                pageCode: this.pageCode,
                service: this.service,
                postOnAdd: this.postOnAdd
            });

            headerTitle = this.translationService.getValue('suspensions.add-suspension-dialog-title');
        }

        const dialog = this.editSuspensionDialog.openWithTwoButtons({
            title: headerTitle,
            TCtor: EditSuspensionComponent,
            headerAuditButton: headerAuditBtn,
            headerCancelButton: {
                cancelBtnClicked: (closeFn: HeaderCloseFunction) => { closeFn(); }
            },
            componentData: data,
            translteService: this.translationService,
            viewMode: viewMode
        }, '850px');

        dialog.subscribe({
            next: (result: SuspensionDataDTO | undefined) => {
                if (result !== null && result !== undefined) {
                    if (suspension !== null && suspension !== undefined) { // edit
                        suspension = result;
                    }
                    else {
                        this.suspensions.push(result);
                    }

                    this.suspensions = this.suspensions.slice();

                    if (this.ngControl) { // custom form control mode
                        this.control.updateValueAndValidity();
                    }
                    else { // dialog mode
                        this.hasShipEventExistsOnSameDateError = false;
                    }
                }
            }
        });
    }

    public deleteSuspension(suspensionRow: GridRow<SuspensionDataDTO>): void {
        this.confirmDialog.open({
            title: this.translationService.getValue('suspensions.delete-suspension'),
            message: this.translationService.getValue('suspensions.confirm-delete-suspension-message'),
            okBtnLabel: this.translationService.getValue('suspensions.delete-btn-label')
        }).subscribe({
            next: (ok: boolean) => {
                if (ok) {
                    this.suspensionsTable.softDelete(suspensionRow);

                    this.suspensions = this.suspensionsTable.rows.slice();

                    if (this.ngControl) { // custrom form control mode
                        this.control.updateValueAndValidity();
                    }
                    else { // dialog mode
                        this.hasShipEventExistsOnSameDateError = false;
                    }
                }
            }
        });
    }

    public undoDeleteSuspension(suspension: GridRow<SuspensionDataDTO>): void {
        this.confirmDialog.open().subscribe({
            next: (ok: boolean) => {
                if (ok) {
                    this.suspensionsTable.softUndoDelete(suspension);

                    this.suspensions = this.suspensionsTable.rows.slice();

                    if (this.ngControl) {
                        this.control.updateValueAndValidity();
                    }
                }
            }
        });
    }

    private getSuspensionsFromTable(): SuspensionDataDTO[] {
        const result: SuspensionDataDTO[] = [];

        if (this.suspensionsTable.rows !== undefined && this.suspensionsTable.rows !== null && this.suspensionsTable.rows.length > 0) {
            const suspensions: SuspensionDataDTO[] = this.suspensionsTable.rows.filter(x => x.isActive === true || (x.id !== null && x.id !== undefined));

            for (const suspension of suspensions) {
                if (suspension.validTo !== undefined && suspension.validTo !== null) {
                    if (result.findIndex(x => x.validTo?.getFullYear() === suspension.validTo?.getFullYear()
                        && x.validTo?.getMonth() === suspension.validTo?.getMonth()
                        && x.validTo?.getDate() === suspension.validTo?.getDate()) === -1
                    ) {
                        const original = suspensions.filter(x => x.validTo?.getFullYear() === suspension.validTo?.getFullYear()
                            && x.validTo?.getMonth() === suspension.validTo?.getMonth()
                            && x.validTo?.getDate() === suspension.validTo?.getDate());

                        if (original.length === 1) {
                            result.push(suspension);
                        }
                        else {
                            result.push(original.find(x => x.isActive === true)!);
                        }
                    }
                }
                else {
                    result.push(suspension);
                }
            }
        }

        return result;
    }

    private handleAddEditErrorResponse(errorResponse: HttpErrorResponse): void {
        if (errorResponse.error?.code === ErrorCode.ShipEventExistsOnSameDate) {
            this.hasShipEventExistsOnSameDateError = true;
        }
        else if (errorResponse.error?.code === ErrorCode.PermitSuspensionValidToExists || errorResponse.error?.code === ErrorCode.PermitLicenseSuspensionValidToExists) {
            let existingValidTo: Date | undefined;
            if (errorResponse.error?.messages !== null && errorResponse.error?.messages !== undefined && errorResponse.error.messages.length > 0) {
                const timestamp: number = Date.parse(errorResponse.error.messages[0]);

                if (isNaN(timestamp) === false) {
                    existingValidTo = new Date(timestamp);
                }
            }

            let message: string = '';

            if (existingValidTo !== null && existingValidTo !== undefined && existingValidTo) {
                let msg1: string = '';
                if (this.isPermitLicense) {
                    msg1 = this.translationService.getValue('suspensions.permit-suspension-valid-to-already-exists');
                }
                else {
                    msg1 = this.translationService.getValue('suspensions.permit-license-suspension-valid-to-already-exists');
                }

                message = `${msg1}: ${DateUtils.ToDisplayDateString(existingValidTo)}`;
            }
            else {
                message = this.translationService.getValue('suspensions.there-is-a-suspension-with-already-existing-valid-to-error');
            }

            this.snackbar.open(message, undefined, {
                duration: RequestProperties.DEFAULT.showExceptionDurationErr,
                panelClass: RequestProperties.DEFAULT.showExceptionColorClassErr
            });
        }
    }

    private setSuspensionPermission(): void {
        if (this.isPermitLicense) {
            this.canAddSuspensionRecords = this.permissionsService.has(PermissionsEnum.PermitLicenseSuspensionAdd);
            this.canEditSuspensionRecords = this.permissionsService.has(PermissionsEnum.PermitLicenseSuspensionEdit);
            this.canDeleteSuspensionRecords = this.permissionsService.has(PermissionsEnum.PermitLicenseSuspensionDelete);
            this.canRestoreSuspensionRecords = this.permissionsService.has(PermissionsEnum.PermitLicenseSuspensionRestore);
        }
        else {
            this.canAddSuspensionRecords = this.permissionsService.has(PermissionsEnum.PermitSuspensionAdd);
            this.canEditSuspensionRecords = this.permissionsService.has(PermissionsEnum.PermitSuspensionEdit);
            this.canDeleteSuspensionRecords = this.permissionsService.has(PermissionsEnum.PermitSuspensionDelete);
            this.canRestoreSuspensionRecords = this.permissionsService.has(PermissionsEnum.PermitSuspensionRestore);
        }
    }
}