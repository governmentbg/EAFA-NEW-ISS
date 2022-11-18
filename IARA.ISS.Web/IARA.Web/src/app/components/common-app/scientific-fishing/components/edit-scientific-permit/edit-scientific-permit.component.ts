import { Component, EventEmitter, OnInit, ViewChild } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { combineLatest, forkJoin, Observable, Subject } from 'rxjs';

import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { ScientificPermitStatusEnum } from '@app/enums/scientific-permit-status.enum';
import { IApplicationsService } from '@app/interfaces/administration-app/applications.interface';
import { IScientificFishingService } from '@app/interfaces/common-app/scientific-fishing.interface';
import { DialogParamsModel } from '@app/models/common/dialog-params.model';
import { AddressRegistrationDTO } from '@app/models/generated/dtos/AddressRegistrationDTO';
import { ApplicationContentDTO } from '@app/models/generated/dtos/ApplicationContentDTO';
import { ApplicationRegiXCheckDTO } from '@app/models/generated/dtos/ApplicationRegiXCheckDTO';
import { CancellationDetailsDTO } from '@app/models/generated/dtos/CancellationDetailsDTO';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { RegixChecksWrapperDTO } from '@app/models/generated/dtos/RegixChecksWrapperDTO';
import { RegixLegalDataDTO } from '@app/models/generated/dtos/RegixLegalDataDTO';
import { RegixPersonDataDTO } from '@app/models/generated/dtos/RegixPersonDataDTO';
import { ScientificFishingApplicationEditDTO } from '@app/models/generated/dtos/ScientificFishingApplicationEditDTO';
import { ScientificFishingOutingDTO } from '@app/models/generated/dtos/ScientificFishingOutingDTO';
import { ScientificFishingPermitEditDTO } from '@app/models/generated/dtos/ScientificFishingPermitEditDTO';
import { ScientificFishingPermitHolderDTO } from '@app/models/generated/dtos/ScientificFishingPermitHolderDTO';
import { ScientificFishingPermitHolderRegixDataDTO } from '@app/models/generated/dtos/ScientificFishingPermitHolderRegixDataDTO';
import { ScientificFishingPermitRegixDataDTO } from '@app/models/generated/dtos/ScientificFishingPermitRegixDataDTO';
import { CancellationDialogParams } from '@app/shared/components/cancellation-dialog/cancellation-dialog-params.model';
import { CancellationDialogComponent } from '@app/shared/components/cancellation-dialog/cancellation-dialog.component';
import { TLConfirmDialog } from '@app/shared/components/confirmation-dialog/tl-confirm-dialog';
import { GridRow } from '@app/shared/components/data-table/models/row.model';
import { TLDataTableComponent } from '@app/shared/components/data-table/tl-data-table.component';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { IHeaderAuditButton } from '@app/shared/components/dialog-wrapper/interfaces/header-audit-button.interface';
import { HeaderCloseFunction } from '@app/shared/components/dialog-wrapper/interfaces/header-cancel-button.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { TLMatDialog } from '@app/shared/components/dialog-wrapper/tl-mat-dialog';
import { TLError } from '@app/shared/components/input-controls/models/tl-error.model';
import { IS_PUBLIC_APP } from '@app/shared/modules/application.modules';
import { ApplicationDialogData, ApplicationUtils } from '@app/shared/utils/application.utils';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { EditScientificFishingOutingComponent } from '../edit-scientific-fishing-outing/edit-scientific-fishing-outing.component';
import { EditScientificPermitHolderComponent } from '../edit-scientific-permit-holder/edit-scientific-permit-holder.component';
import { EditScientificPermitHolderDialogParams } from '../../models/edit-scientific-permit-holder-dialog-params';
import { EditScientificFishingOutingDialogParams } from '../../models/edit-scientific-fishing-outing-dialog-params.model';
import { PageCodeEnum } from '@app/enums/page-code.enum';
import { CommonNomenclatures } from '@app/services/common-app/common-nomenclatures.service';
import { ShipNomenclatureDTO } from '@app/models/generated/dtos/ShipNomenclatureDTO';
import { SciFiPrintTypesEnum } from '@app/enums/sci-fi-print-types.enum';
import { DateRangeData } from '@app/shared/components/input-controls/tl-date-range/tl-date-range.component';
import { FileTypeEnum } from '@app/enums/file-types.enum';
import { Notifier } from '@app/shared/directives/notifier/notifier.class';
import { ValidityCheckerGroupDirective } from '@app/shared/directives/validity-checker/validity-checker-group.directive';
import { ScientificFishingPublicService } from '@app/services/public-app/scientific-fishing-public.service';
import { ApplicationSubmittedByDTO } from '@app/models/generated/dtos/ApplicationSubmittedByDTO';
import { CancellationReasonGroupEnum } from '@app/enums/cancellation-reason-group.enum';
import { PermittedFileTypeDTO } from '@app/models/generated/dtos/PermittedFileTypeDTO';
import { ScientificFishingReasonNomenclatureDTO } from '@app/models/generated/dtos/ScientificFishingReasonNomenclatureDTO';
import { ErrorCode, ErrorModel } from '@app/models/common/exception.model';
import { ShipNomenclatureFilters, ShipsUtils } from '@app/shared/utils/ships.utils';

@Component({
    selector: 'edit-scientific-permit',
    templateUrl: './edit-scientific-permit.component.html',
    styleUrls: ['./edit-scientific-permit.component.scss']
})
export class EditScientificPermitComponent implements OnInit, IDialogComponent {
    public readonly pageCode: PageCodeEnum = PageCodeEnum.SciFi;
    public readonly icIconSize: number = CommonUtils.IC_ICON_SIZE;
    public readonly permitStatuses: typeof ScientificPermitStatusEnum = ScientificPermitStatusEnum;
    public readonly currentDate: Date = new Date();

    public form!: FormGroup;
    public service!: IScientificFishingService;

    public permitStatus: ScientificPermitStatusEnum | undefined;
    public permitHolders: ScientificFishingPermitHolderDTO[] = [];
    public permitOutings: ScientificFishingOutingDTO[] = [];
    public fishes: NomenclatureDTO<number>[] = [];
    public ships: ShipNomenclatureDTO[] = [];
    public permitReasons: NomenclatureDTO<number>[] = [];
    public permitLegalReasons: NomenclatureDTO<number>[] = [];
    public statuses: NomenclatureDTO<number>[] = [];

    public holdersTouched: boolean = false;

    public notifier: Notifier = new Notifier();
    public regixChecks: ApplicationRegiXCheckDTO[] = [];
    public expectedResults: ScientificFishingPermitRegixDataDTO;

    public isApplication!: boolean;
    public isApplicationHistoryMode: boolean = false;
    public showOnlyRegiXData: boolean = false;
    public showRegiXData: boolean = false;
    public isOnlineApplication: boolean = false;
    public refreshFileTypes: Subject<void> = new Subject<void>();
    public loadRegisterFromApplication: boolean = false;
    public hasDelivery: boolean = false;
    public dialogRightSideActions: IActionInfo[] | undefined;
    public hasNoEDeliveryRegistrationError: boolean = false;

    public permitId: number | undefined;
    public applicationId: number | undefined;
    public readOnly!: boolean;
    public viewMode!: boolean;
    public isEditing: boolean = false;
    public isRegisterEntry: boolean = false;
    public isPublicApp: boolean;

    @ViewChild('holdersTable')
    private holdersTable!: TLDataTableComponent;

    @ViewChild(ValidityCheckerGroupDirective)
    private validityCheckerGroup!: ValidityCheckerGroupDirective;

    private shipCaptains: Map<number, string> = new Map<number, string>();

    private translateService: FuseTranslationLoaderService;
    private nomenclatures: CommonNomenclatures;
    private applicationsService: IApplicationsService | undefined;

    private allStatuses: NomenclatureDTO<number>[] = [];

    private annulDialog: TLMatDialog<CancellationDialogComponent>;
    private editHolderDialog: TLMatDialog<EditScientificPermitHolderComponent>;
    private editOutingDialog: TLMatDialog<EditScientificFishingOutingComponent>;
    private confirmDialog: TLConfirmDialog;

    private model!: ScientificFishingPermitEditDTO | ScientificFishingApplicationEditDTO | ScientificFishingPermitRegixDataDTO;

    public constructor(
        annulDialog: TLMatDialog<CancellationDialogComponent>,
        editHolderDialog: TLMatDialog<EditScientificPermitHolderComponent>,
        editOutingDialog: TLMatDialog<EditScientificFishingOutingComponent>,
        confirmDialog: TLConfirmDialog,
        translate: FuseTranslationLoaderService,
        nomenclatures: CommonNomenclatures
    ) {
        this.annulDialog = annulDialog;
        this.editHolderDialog = editHolderDialog;
        this.editOutingDialog = editOutingDialog;
        this.confirmDialog = confirmDialog;
        this.translateService = translate;
        this.nomenclatures = nomenclatures;

        this.isPublicApp = IS_PUBLIC_APP;

        this.expectedResults = new ScientificFishingPermitRegixDataDTO({
            requester: new RegixPersonDataDTO(),
            receiver: new RegixLegalDataDTO(),
            holders: []
        });
    }

    public async ngOnInit(): Promise<void> {
        if (!this.showOnlyRegiXData) {
            const nomenclatures: (ScientificFishingReasonNomenclatureDTO[] | NomenclatureDTO<number>[])[] = await forkJoin(
                NomenclatureStore.instance.getNomenclature(NomenclatureTypes.SciPermitReasons, this.service.getPermitReasons.bind(this.service), false),
                NomenclatureStore.instance.getNomenclature(NomenclatureTypes.SciPermitStatuses, this.service.getPermitStatuses.bind(this.service), false),
                NomenclatureStore.instance.getNomenclature(NomenclatureTypes.Ships, this.nomenclatures.getShips.bind(this.nomenclatures), false),
                NomenclatureStore.instance.getNomenclature(NomenclatureTypes.Fishes, this.nomenclatures.getFishTypes.bind(this.nomenclatures), false)
            ).toPromise();

            this.permitReasons = (nomenclatures[0] as ScientificFishingReasonNomenclatureDTO[]).filter(x => !x.isLegalReason);
            this.permitLegalReasons = (nomenclatures[0] as ScientificFishingReasonNomenclatureDTO[]).filter(x => x.isLegalReason);
            this.allStatuses = this.statuses = nomenclatures[1];
            this.ships = nomenclatures[2];
            this.fishes = nomenclatures[3];

            this.statuses = this.statuses.filter((stat: NomenclatureDTO<number>) => {
                const notVisibleStatuses: ScientificPermitStatusEnum[] = [
                    ScientificPermitStatusEnum.Application,
                    ScientificPermitStatusEnum.Canceled,
                    ScientificPermitStatusEnum.Expired
                ];

                const status: ScientificPermitStatusEnum = ScientificPermitStatusEnum[stat.code as keyof typeof ScientificPermitStatusEnum];
                return !notVisibleStatuses.includes(status);
            });

            this.buildPermitReasonsFormGroup();

            if (!this.isApplication) {
                this.buildPermitLegalReasonsFormGroup();
            }

            this.ships = ShipsUtils.filter(this.ships, new ShipNomenclatureFilters({
                isDestOrDereg: false
            }));
        }

        if (this.isApplicationHistoryMode && this.applicationId !== undefined) {
            this.form.disable();

            if (this.applicationsService) {
                this.applicationsService.getApplicationChangeHistoryContent(this.applicationId).subscribe({
                    next: (content: ApplicationContentDTO) => {
                        const contentObject: Record<string, unknown> = JSON.parse(content.draftContent!);
                        const permit: ScientificFishingApplicationEditDTO = new ScientificFishingApplicationEditDTO(contentObject);
                        permit.files = content.files;
                        permit.applicationId = content.applicationId;

                        this.hasDelivery = permit.hasDelivery!;
                        this.isOnlineApplication = permit.isOnlineApplication!;
                        this.refreshFileTypes.next();

                        this.model = permit;
                        this.fillForm(this.model);
                    }
                });
            }
        }
        else if (this.permitId === undefined && this.applicationId !== undefined && !this.isApplication) {
            if (this.loadRegisterFromApplication === true) {
                this.isEditing = true;

                this.service.getRegisterByApplicationId(this.applicationId, this.pageCode).subscribe({
                    next: (permit: unknown) => {
                        this.model = permit as ScientificFishingPermitEditDTO;
                        this.permitStatus = (this.model as ScientificFishingPermitEditDTO).permitStatus;

                        this.isOnlineApplication = (this.model as ScientificFishingPermitEditDTO).isOnlineApplication!;
                        this.refreshFileTypes.next();
                        this.fillForm(this.model);
                    }
                });
            }
            // извличане на данни за създаване на регистров запис от заявление
            else {
                this.isEditing = false;

                this.service.getApplicationDataForRegister(this.applicationId!).subscribe((permit: ScientificFishingPermitEditDTO) => {
                    this.model = permit;
                    this.permitStatus = permit.permitStatus;

                    this.isOnlineApplication = permit.isOnlineApplication!;
                    this.refreshFileTypes.next();
                    this.fillForm(this.model);
                });
            }
        }
        else {
            if (this.isApplication && this.applicationId !== undefined) {
                // извличане на данни за RegiX сверка от служител
                this.isEditing = false;

                if (this.showOnlyRegiXData) {
                    this.service.getRegixData(this.applicationId!).subscribe({
                        next: (regixData: RegixChecksWrapperDTO<ScientificFishingPermitRegixDataDTO>) => {
                            this.model = new ScientificFishingPermitRegixDataDTO(regixData.dialogDataModel);
                            this.expectedResults = new ScientificFishingPermitRegixDataDTO(regixData.regiXDataModel);

                            for (const holder of this.model.holders as ScientificFishingPermitHolderRegixDataDTO[]) {
                                holder.hasRegixDataDiscrepancy = !this.holderEqualsRegixHolder(holder);
                            }

                            this.fillForm(this.model);
                        }
                    });
                }
                else {
                    // извличане на данни за заявление
                    this.isEditing = false;

                    this.service.getApplication(this.applicationId!, this.showRegiXData).subscribe({
                        next: (permit: ScientificFishingApplicationEditDTO) => {
                            permit.applicationId = this.applicationId!;
                            this.hasDelivery = permit.hasDelivery!;
                            this.isOnlineApplication = permit.isOnlineApplication!;
                            this.refreshFileTypes.next();

                            if (this.showRegiXData) {
                                this.expectedResults = new ScientificFishingPermitRegixDataDTO(permit.regiXDataModel);
                                permit.regiXDataModel = undefined;
                            }

                            if (this.isPublicApp && this.isOnlineApplication && (permit.requester === undefined || permit.requester === null)) {
                                const service = this.service as ScientificFishingPublicService;
                                service.getCurrentUserAsSubmittedBy().subscribe({
                                    next: (requester: ApplicationSubmittedByDTO) => {
                                        permit.requester = requester.person;
                                        this.model = permit;
                                        this.fillForm(this.model);
                                    }
                                });
                            }
                            else {
                                this.model = permit;
                                this.fillForm(this.model);
                            }
                        }
                    });
                }
            }
            else if (this.permitId !== undefined) {
                // извличане на данни за регистров запис
                this.isEditing = true;

                this.service.getPermit(this.permitId!).subscribe({
                    next: (permit: ScientificFishingPermitEditDTO) => {
                        this.model = permit;
                        this.permitStatus = permit.permitStatus;

                        this.isOnlineApplication = permit.isOnlineApplication!;
                        this.refreshFileTypes.next();
                        this.fillForm(this.model);
                    }
                });
            }
        }

        if (this.readOnly || this.viewMode) {
            this.form.disable();
        }
    }

    public setData(data: DialogParamsModel, buttons: DialogWrapperData): void {
        this.service = data.service as IScientificFishingService;
        this.applicationsService = data.applicationsService;
        this.isApplication = data.isApplication;
        this.applicationId = data.applicationId;
        this.permitId = data.id;
        this.readOnly = data.isReadonly;
        this.isApplicationHistoryMode = data.isApplicationHistoryMode;
        this.viewMode = data.viewMode;
        this.showOnlyRegiXData = data.showOnlyRegiXData;
        this.showRegiXData = data.showRegiXData;
        this.dialogRightSideActions = buttons.rightSideActions;
        this.loadRegisterFromApplication = data.loadRegisterFromApplication;

        this.buildForm();

        if (this.permitId !== undefined) {
            this.form.get('requestDateControl')!.disable();
        }
    }

    public saveBtnClicked(action: IActionInfo, dialogClose: DialogCloseCallback): void {
        this.form.markAllAsTouched();
        this.holdersTouched = true;
        this.validityCheckerGroup.validate();

        if (this.form.valid) {
            this.savePermit(dialogClose);
        }
    }

    public cancelBtnClicked(action: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public async dialogButtonClicked(action: IActionInfo, dialogClose: DialogCloseCallback): Promise<void> {
        let applicationAction: boolean = false;

        if (this.model instanceof ScientificFishingApplicationEditDTO || this.model instanceof ScientificFishingPermitRegixDataDTO) {
            this.fillModel(this.form);
            CommonUtils.sanitizeModelStrings(this.model);

            if (this.model instanceof ScientificFishingApplicationEditDTO) {
                if (action.id === 'save-draft-content' && (this.model.id === undefined || this.model.id === null)) {
                    const holders: ScientificFishingPermitHolderDTO[] = [];
                    const promises: Promise<string>[] = [];

                    for (const holder of this.model.holders ?? []) {
                        if (holder.photo && holder.photo.file) {
                            promises.push(CommonUtils.getFileAsBase64(holder.photo.file));
                            holders.push(holder);
                        }
                    }

                    if (promises.length > 0) {
                        const photos: string[] = await Promise.all(promises);

                        for (let i = 0; i < photos.length; ++i) {
                            holders[i].photoBase64 = photos[i];
                            holders[i].photo = undefined;
                        }
                    }
                }
            }

            applicationAction = ApplicationUtils.applicationDialogButtonClicked(new ApplicationDialogData({
                action: action,
                dialogClose: dialogClose,
                applicationId: this.applicationId as number,
                model: this.model,
                readOnly: this.readOnly,
                viewMode: this.viewMode,
                editForm: this.form,
                saveFn: this.savePermit.bind(this),
                onMarkAsTouched: () => {
                    this.holdersTouched = true;
                    this.validityCheckerGroup.validate();
                }
            }));
        }

        if (!applicationAction) {
            this.form.markAllAsTouched();
            this.validityCheckerGroup.validate();

            if (this.readOnly || this.viewMode) {
                switch (action.id) {
                    case 'print':
                        return this.print(SciFiPrintTypesEnum.Register);
                    case 'print-gov':
                        return this.print(SciFiPrintTypesEnum.Gov);
                    case 'print-gov-project':
                        return this.print(SciFiPrintTypesEnum.GovProject);
                }
            }
            else {
                if (this.form.valid) {
                    switch (action.id) {
                        case 'annul':
                            return this.openAnnulDialog(dialogClose, true);
                        case 'activate':
                            return this.openAnnulDialog(dialogClose, false);
                        case 'save':
                            return this.saveBtnClicked(action, dialogClose);
                        case 'save-print':
                            return this.saveAndPrint(dialogClose, SciFiPrintTypesEnum.Register);
                        case 'save-print-gov':
                            return this.saveAndPrint(dialogClose, SciFiPrintTypesEnum.Gov);
                        case 'save-print-gov-project':
                            return this.saveAndPrint(dialogClose, SciFiPrintTypesEnum.GovProject);
                    }
                }
            }
        }
    }

    public getCoordinationDateErrorText(controlName: string, error: Record<string, unknown>, errorCode: string): TLError | undefined {
        if (controlName === 'coordinationDateControl') {
            if (errorCode === 'coordinationdatelessthanrequestdate') {
                return new TLError({
                    text: this.translateService.getValue('scientific-fishing.coordination-date-less-than-request-date'),
                    type: 'error'
                });
            }
        }
        return undefined;
    }

    // permit holders table
    public addEditPermitHolder(holder: ScientificFishingPermitHolderDTO | undefined, viewMode: boolean = false): void {
        let data: EditScientificPermitHolderDialogParams | undefined;
        let headerAuditBtn: IHeaderAuditButton | undefined;
        let headerTitle: string;

        let registrationDate: Date | undefined;
        if (this.model instanceof ScientificFishingApplicationEditDTO || this.model instanceof ScientificFishingPermitEditDTO) {
            registrationDate = this.model.registrationDate!;
        }

        if (holder !== undefined) {
            const holderExpectedResult = this.expectedResults.holders?.find(x => {
                return x.regixPersonData?.egnLnc?.identifierType === holder?.regixPersonData?.egnLnc?.identifierType
                    && x.regixPersonData?.egnLnc?.egnLnc === holder?.regixPersonData?.egnLnc?.egnLnc;
            });

            data = new EditScientificPermitHolderDialogParams({
                service: this.service,
                model: holder,
                requestDate: registrationDate,
                isEgnLncReadOnly: this.isEditing,
                expectedResults: holderExpectedResult ?? new ScientificFishingPermitHolderRegixDataDTO(),
                readOnly: this.readOnly || viewMode,
                showOnlyRegiXData: this.showOnlyRegiXData
            });

            if (holder.id !== undefined && !IS_PUBLIC_APP) {
                headerAuditBtn = {
                    id: holder.id,
                    getAuditRecordData: this.service.getPermitHolderAudit.bind(this.service),
                    tableName: 'ScientificPermitOwner'
                };
            }

            if (this.readOnly || viewMode) {
                headerTitle = this.translateService.getValue('scientific-fishing.view-permit-holder-dialog-title');
            }
            else {
                headerTitle = this.translateService.getValue('scientific-fishing.edit-permit-holder-dialog-title');
            }
        }
        else {
            headerTitle = this.translateService.getValue('scientific-fishing.add-permit-holder-dialog-title');
            data = new EditScientificPermitHolderDialogParams({
                service: this.service,
                requestDate: registrationDate,
                showOnlyRegiXData: this.showOnlyRegiXData
            });
        }

        const dialog = this.editHolderDialog.openWithTwoButtons({
            title: headerTitle,
            TCtor: EditScientificPermitHolderComponent,
            headerAuditButton: headerAuditBtn,
            headerCancelButton: {
                cancelBtnClicked: this.closeEditHolderDialogBtnClicked.bind(this)
            },
            componentData: data,
            translteService: this.translateService,
            viewMode: viewMode ?? false
        }, '1200px');

        dialog.subscribe((result: { holder: ScientificFishingPermitHolderDTO | ScientificFishingPermitHolderRegixDataDTO | undefined, isTouched: boolean }) => {
            if (result !== undefined && result !== null && result.holder !== null && result.holder !== undefined) {
                if (holder !== undefined) {
                    holder = result.holder;
                }
                else {
                    this.permitHolders.push(result.holder);
                }

                this.permitHolders = this.permitHolders.slice();
                this.holdersTouched = true;
                this.form.updateValueAndValidity({ emitEvent: result.isTouched });
            }
        });
    }

    public closeEditHolderDialogBtnClicked(closeFn: HeaderCloseFunction): void {
        closeFn();
    }

    public deletePermitHolder(holder: GridRow<ScientificFishingPermitHolderDTO>): void {
        this.confirmDialog.open({
            title: this.translateService.getValue('scientific-fishing.delete-holder'),
            message: this.translateService.getValue('scientific-fishing.confirm-delete-holder-message'),
            okBtnLabel: this.translateService.getValue('scientific-fishing.delete')
        }).subscribe({
            next: (ok: boolean) => {
                if (ok) {
                    this.holdersTable.softDelete(holder);
                    this.holdersTouched = true;
                    this.form.updateValueAndValidity();
                }
            }
        });
    }

    public undoDeletePermitHolder(holder: GridRow<ScientificFishingPermitHolderDTO>): void {
        this.confirmDialog.open().subscribe({
            next: (ok: boolean) => {
                if (ok) {
                    this.holdersTable.softUndoDelete(holder);
                    this.holdersTouched = true;
                    this.form.updateValueAndValidity();
                }
            }
        });
    }

    // permit outings table
    public addEditPermitOuting(outingId: number | undefined, viewMode: boolean = false): void {
        let title: string;
        let data: EditScientificFishingOutingDialogParams;
        let headerAuditBtn: IHeaderAuditButton | undefined;

        if (outingId !== undefined) {
            const model = this.permitOutings.find((outing: ScientificFishingOutingDTO) => { return outing.id === outingId; });

            if (this.readOnly || viewMode) {
                title = this.translateService.getValue('scientific-fishing.view-outing-dialog-title');
            }
            else {
                title = this.translateService.getValue('scientific-fishing.edit-outing-dialog-title');
            }
            data = new EditScientificFishingOutingDialogParams(this.permitId!, this.service, model, false, outingId, this.readOnly || viewMode);

            headerAuditBtn = {
                id: outingId,
                getAuditRecordData: this.service.getPermitOutingAudit.bind(this.service),
                tableName: 'ScientificPermitOuting'
            };
        }
        else {
            title = this.translateService.getValue('scientific-fishing.add-outing-dialog-title');
            data = new EditScientificFishingOutingDialogParams(this.permitId!, this.service, undefined, false);
        }

        const dialog = this.editOutingDialog.openWithTwoButtons({
            title: title,
            TCtor: EditScientificFishingOutingComponent,
            headerAuditButton: headerAuditBtn,
            headerCancelButton: {
                cancelBtnClicked: this.closeEditOutingDialogBtnClicked.bind(this)
            },
            componentData: data,
            translteService: this.translateService,
            disableDialogClose: true,
            viewMode: viewMode ?? false
        }, '1400px');

        dialog.subscribe((entry?: ScientificFishingOutingDTO) => {
            if (entry !== undefined && entry !== null) {
                if (outingId !== undefined) {
                    const idx: number = this.permitOutings.findIndex((owner: ScientificFishingOutingDTO) => owner.id === outingId);
                    this.permitOutings[idx] = entry;
                }
                else {
                    this.permitOutings.push(entry);
                }
                this.permitOutings = this.permitOutings.slice();
            }
        });
    }

    public closeEditOutingDialogBtnClicked(closeFn: HeaderCloseFunction): void {
        closeFn();
    }

    public deletePermitOuting(outing: ScientificFishingOutingDTO): void {
        this.confirmDialog.open({
            title: this.translateService.getValue('scientific-fishing.delete-outing-confirm-title'),
            message: this.translateService.getValue('scientific-fishing.confirm-delete-outing-message'),
            okBtnLabel: this.translateService.getValue('scientific-fishing.delete')
        }).subscribe({
            next: (ok: boolean) => {
                if (ok) {
                    outing.isActive = false;
                }
            }
        });
    }

    public undoDeletePermitOuting(outing: ScientificFishingOutingDTO): void {
        this.confirmDialog.open().subscribe({
            next: (ok: boolean) => {
                if (ok) {
                    outing.isActive = true;
                }
            }
        });
    }

    public fileTypeFilterFn(options: PermittedFileTypeDTO[]): PermittedFileTypeDTO[] {
        const pdfs: FileTypeEnum[] = [FileTypeEnum.SIGNEDAPPL, FileTypeEnum.APPLICATION_PDF];

        let result: PermittedFileTypeDTO[] = options;

        if (this.isApplication || !this.isOnlineApplication) {
            result = result.filter(x => !pdfs.includes(FileTypeEnum[x.code as keyof typeof FileTypeEnum]));
        }

        return result;
    }

    // form handlers
    private buildForm(): void {
        if (this.showOnlyRegiXData) {
            this.form = new FormGroup({
                requesterRegixDataControl: new FormControl(null),

                receiverRegixDataControl: new FormControl(null, Validators.required)
            });
        }
        else {
            this.form = new FormGroup({
                letterOfAttorneyControl: new FormControl(null),

                receiverRegixDataControl: new FormControl(null, Validators.required),

                requestNumberControl: new FormControl({ value: '', disabled: true }),
                requestDateControl: new FormControl(new Date(), Validators.required),
                validityDateRangeControl: new FormControl('', Validators.required),

                researchPeriodControl: new FormControl('', Validators.required),
                researchWaterAreasControl: new FormControl('', [Validators.maxLength(4000), Validators.required]),
                researchGoalsControl: new FormControl('', [Validators.maxLength(4000), Validators.required]),

                fishTypesControl: new FormControl('', [Validators.maxLength(4000)]),
                fishTypesApp4ZBRDescControl: new FormControl('', [Validators.maxLength(4000)]),
                fishTypesCrayFishControl: new FormControl('', [Validators.maxLength(4000)]),

                fishingGearControl: new FormControl('', [Validators.maxLength(4000)]),

                shipIsNotRegisteredControl: new FormControl(false),
                existingShipNameControl: new FormControl(null),
                shipNameControl: new FormControl('', Validators.maxLength(500)),
                shipExternalMarkingControl: new FormControl('', Validators.maxLength(50)),
                shipCaptainNameControl: new FormControl('', Validators.maxLength(500)),

                deliveryDataControl: new FormControl(null),
                filesControl: new FormControl(null, Validators.required)
            }, this.validateHolders());

            if (this.isApplication) {
                this.form.addControl('requesterRegixDataControl', new FormControl(null));
                this.form.addControl('requesterOrganizationPositionControl', new FormControl('', Validators.required));
                this.form.addControl('requesterHasLetterOfAttorneyControl', new FormControl(null));
            }
            else {
                this.form.addControl('statusControl', new FormControl(null, Validators.required));
            }

            if (!this.isApplication && this.permitId !== undefined) {
                this.form.addControl('coordinationCommitteeControl', new FormControl(''));
                this.form.addControl('coordinationLetterNoControl', new FormControl(''));
                this.form.addControl('coordinationDateControl', new FormControl(null));
                this.form.addControl('coordinationCommentsControl', new FormControl('', Validators.maxLength(4000)));
            }

            this.setupValidatorHandlers();

            this.form.get('deliveryDataControl')?.valueChanges.subscribe({
                next: () => {
                    this.hasNoEDeliveryRegistrationError = false;
                }
            });
        }
    }

    private setupValidatorHandlers(): void {
        if (!this.isApplication && this.permitId !== undefined && !this.showOnlyRegiXData) {
            combineLatest(
                this.form.get('coordinationCommitteeControl')!.valueChanges,
                this.form.get('coordinationLetterNoControl')!.valueChanges,
                this.form.get('coordinationDateControl')!.valueChanges
            ).subscribe((values: string[]) => {
                if (values.some(x => x !== undefined && x !== null && x.length !== 0)) {
                    this.form.get('coordinationCommitteeControl')!.setValidators([Validators.required, Validators.maxLength(500)]);
                    this.form.get('coordinationLetterNoControl')!.setValidators([Validators.required, Validators.maxLength(50)]);
                    this.form.get('coordinationDateControl')!.setValidators([Validators.required, this.coordinationDateValidator()]);
                }
                else {
                    this.form.get('coordinationCommitteeControl')!.clearValidators();
                    this.form.get('coordinationLetterNoControl')!.clearValidators();
                    this.form.get('coordinationDateControl')!.clearValidators();
                }
                this.form.get('coordinationCommitteeControl')!.markAsPending({ emitEvent: false });
                this.form.get('coordinationLetterNoControl')!.markAsPending({ emitEvent: false });
                this.form.get('coordinationDateControl')!.markAsPending({ emitEvent: false });
            });
        }

        this.form.get('requesterHasLetterOfAttorneyControl')?.valueChanges.subscribe({
            next: (yes: boolean) => {
                if (yes === true) {
                    this.form.get('letterOfAttorneyControl')!.setValidators(Validators.required);
                }
                else {
                    this.form.get('letterOfAttorneyControl')!.clearValidators();
                    this.form.get('letterOfAttorneyControl')!.setErrors(null);
                }
                this.form.get('letterOfAttorneyControl')!.updateValueAndValidity({ emitEvent: false });
            }
        });
    }

    private buildPermitReasonsFormGroup(): void {
        const formGroup: FormGroup = new FormGroup({}, this.atLeastOnePermitReasonSelectedValidator());

        for (const reason of this.permitReasons) {
            formGroup.addControl(`${reason.value}`, new FormControl(false));
        }
        this.form.addControl('permitReasonsGroup', formGroup);
    }

    private buildPermitLegalReasonsFormGroup(): void {
        const formGroup: FormGroup = new FormGroup({}, this.atLeastOnePermitReasonSelectedValidator());

        for (const reason of this.permitLegalReasons) {
            formGroup.addControl(`${reason.value}`, new FormControl(false));
        }
        this.form.addControl('permitLegalReasonsGroup', formGroup);
    }

    private atLeastOnePermitReasonSelectedValidator(): ValidatorFn {
        return (control: AbstractControl): ValidationErrors | null => {
            const group: FormGroup = control as FormGroup;

            const valid: boolean = Object.keys(group.controls).some((key: string) => {
                return group.controls[key].value === true;
            });
            if (!valid) {
                return { atLeastOne: true };
            }
            return null;
        };
    }

    private coordinationDateValidator(): ValidatorFn {
        return (control: AbstractControl): ValidationErrors | null => {
            const requestDate = this.form?.get('requestDateControl')?.value;
            const coordinationDate = control?.value;

            if (requestDate && coordinationDate) {
                const request: Date = new Date(requestDate);
                const coordination: Date = new Date(coordinationDate);

                if (coordination < request) {
                    return { 'coordinationdatelessthanrequestdate': true };
                }
            }
            return null;
        };
    }

    private validateHolders(): ValidatorFn {
        return (control: AbstractControl): ValidationErrors | null => {
            const errors: ValidationErrors = {};

            if (this.permitHolders.some(holder => holder.hasValidationErrors)) {
                errors['holdersValidation'] = true;
            }

            if (!this.permitHolders.some(holder => holder.isActive)) {
                errors['noHolders'] = true;
            }

            return Object.keys(errors).length !== 0 ? errors : null;
        };
    }

    private fillForm(model: ScientificFishingPermitEditDTO | ScientificFishingApplicationEditDTO | ScientificFishingPermitRegixDataDTO): void {
        this.fillCommonFormFields(model);

        if (model instanceof ScientificFishingPermitRegixDataDTO) {
            this.fillFormRegiX(this.model);
        }
        else {
            this.form.get('requestNumberControl')!.setValue(model.eventisNum);
            this.form.get('requestDateControl')!.setValue(model.registrationDate);

            if (model.validFrom || model.validTo) {
                this.form.get('validityDateRangeControl')!.setValue(new DateRangeData({ start: model.validFrom, end: model.validTo }));
            }

            if (model.permitReasonsIds !== undefined && model.permitReasonsIds !== null) {
                for (const reasonId of model.permitReasonsIds) {
                    this.form.get('permitReasonsGroup')!.get(`${reasonId}`)!.setValue(true);
                }
            }

            if (model instanceof ScientificFishingApplicationEditDTO) {
                this.form.get('requesterOrganizationPositionControl')!.setValue(model.requesterPosition);
                if (model.requesterLetterOfAttorney !== null && model.requesterLetterOfAttorney !== undefined) {
                    this.form.get('requesterHasLetterOfAttorneyControl')!.setValue(true);
                }
                this.form.get('letterOfAttorneyControl')?.setValue(model.requesterLetterOfAttorney);

                if (this.hasDelivery) {
                    this.form.get('deliveryDataControl')!.setValue(model.deliveryData);
                }
            }
            else {
                if (model.permitStatus === ScientificPermitStatusEnum.Canceled || model.permitStatus === ScientificPermitStatusEnum.Expired) {
                    this.form.get('statusControl')!.disable();
                    this.statuses = this.allStatuses.slice();
                }

                if (model.permitStatus !== ScientificPermitStatusEnum.Application) {
                    const statusCode: string = ScientificPermitStatusEnum[model.permitStatus!];
                    this.form.get('statusControl')!.setValue(this.allStatuses.find(x => x.code === statusCode));
                }

                if (model.permitLegalReasonsIds !== undefined && model.permitLegalReasonsIds !== null) {
                    for (const reasonId of model.permitLegalReasonsIds) {
                        this.form.get('permitLegalReasonsGroup')!.get(`${reasonId}`)!.setValue(true);
                    }
                }
            }

            setTimeout(() => {
                if (model instanceof ScientificFishingPermitEditDTO) {
                    if (model.outings !== undefined && model.outings !== null) {
                        this.permitOutings = model.outings ?? [];

                        for (const outing of this.permitOutings) {
                            outing.catches ??= [];

                            for (const outingCatch of outing.catches) {
                                outingCatch.totalCatch = outingCatch.catchUnder100! + outingCatch.catch100To500! + outingCatch.catch500To1000! + outingCatch.catchOver1000!;
                            }
                        }
                    }
                }
            });

            if (model.researchPeriodFrom || model.researchPeriodTo) {
                this.form.get('researchPeriodControl')!.setValue(new DateRangeData({ start: model.researchPeriodFrom, end: model.researchPeriodTo }));
            }

            this.form.get('researchWaterAreasControl')!.setValue(model.researchWaterArea);
            this.form.get('researchGoalsControl')!.setValue(model.researchGoalsDescription);

            this.form.get('fishTypesControl')!.setValue(model.fishTypesDescription);
            this.form.get('fishTypesApp4ZBRDescControl')!.setValue(model.fishTypesApp4ZBRDesc);
            this.form.get('fishTypesCrayFishControl')!.setValue(model.fishTypesCrayFish);

            this.form.get('fishingGearControl')!.setValue(model.fishingGearDescription);

            if (model.isShipRegistered === true) {
                this.form.get('shipIsNotRegisteredControl')!.setValue(false);

                if (model.shipId !== undefined && model.shipId !== null) {
                    this.form.get('existingShipNameControl')?.setValue(ShipsUtils.get(this.ships, model.shipId));
                }
                this.form.get('shipCaptainNameControl')?.setValue(model.shipCaptainName);
            }
            else if (model.isShipRegistered === false) {
                this.form.get('shipIsNotRegisteredControl')!.setValue(true);

                this.form.get('shipNameControl')?.setValue(model.shipName);
                this.form.get('shipExternalMarkingControl')?.setValue(model.shipExternalMark);
                this.form.get('shipCaptainNameControl')?.setValue(model.shipCaptainName);
            }

            if (model instanceof ScientificFishingPermitEditDTO) {
                this.form.get('coordinationCommitteeControl')?.setValue(model.coordinationCommittee);
                this.form.get('coordinationLetterNoControl')?.setValue(model.coordinationLetterNo);
                this.form.get('coordinationDateControl')?.setValue(model.coordinationDate);
                this.form.get('coordinationCommentsControl')?.setValue(model.coordinationComments);
            }

            this.form.get('filesControl')!.setValue(model.files);

            this.form.markAsUntouched();

            if (!this.isPublicApp) {
                this.form.get('existingShipNameControl')!.valueChanges.subscribe({
                    next: (ship: ShipNomenclatureDTO | undefined) => {
                        if (ship && typeof ship !== 'string') {
                            const captain: string | undefined = this.shipCaptains.get(ship.value!);

                            if (captain !== undefined) {
                                this.form.get('shipCaptainNameControl')!.setValue(captain);
                            }
                            else {
                                this.service.getShipCaptainName(ship.value!).subscribe({
                                    next: (captain: string) => {
                                        this.form.get('shipCaptainNameControl')!.setValue(captain);
                                        this.shipCaptains.set(ship.value!, captain);
                                    }
                                });
                            }
                        }
                    }
                });
            }

            if (this.showRegiXData) {
                this.fillFormRegiX(this.model);
            }
        }
    }

    private fillFormRegiX(model: ScientificFishingApplicationEditDTO | ScientificFishingPermitRegixDataDTO): void {
        setTimeout(() => {
            this.regixChecks = model.applicationRegiXChecks ?? [];
        });

        if (!this.viewMode) {
            this.notifier.start();
            this.notifier.onNotify.subscribe({
                next: () => {
                    this.form.markAllAsTouched();

                    if (this.showOnlyRegiXData) {
                        ApplicationUtils.enableOrDisableRegixCheckButtons(this.form, this.dialogRightSideActions);
                    }

                    this.notifier.stop();
                }
            });
        }
    }

    private fillCommonFormFields(model: ScientificFishingPermitEditDTO | ScientificFishingApplicationEditDTO | ScientificFishingPermitRegixDataDTO): void {
        if (model instanceof ScientificFishingPermitRegixDataDTO || model instanceof ScientificFishingApplicationEditDTO) {
            this.form.get('requesterRegixDataControl')!.setValue(model.requester);
        }

        this.form.get('receiverRegixDataControl')!.setValue(model.receiver);

        setTimeout(() => {
            if (model.holders !== undefined && model.holders !== null) {
                this.permitHolders = model.holders;
                this.fixHoldersNames();
                this.form.updateValueAndValidity();
            }
        });
    }

    private fillModel(form: FormGroup): void {
        this.fillCommonModelFields(form);

        if (this.model instanceof ScientificFishingApplicationEditDTO || this.model instanceof ScientificFishingPermitEditDTO) {
            this.model.registrationDate = form.get('requestDateControl')!.value;
            this.model.validFrom = (form.get('validityDateRangeControl')!.value as DateRangeData)?.start;
            this.model.validTo = (form.get('validityDateRangeControl')!.value as DateRangeData)?.end;

            this.model.permitReasonsIds = [];
            for (const reason of this.permitReasons) {
                if (form.get('permitReasonsGroup')!.get(`${reason.value}`)!.value === true) {
                    this.model.permitReasonsIds.push(reason.value!);
                }
            }

            if (this.model instanceof ScientificFishingApplicationEditDTO) {
                this.model.requesterPosition = form.get('requesterOrganizationPositionControl')!.value;

                if (form.get('requesterHasLetterOfAttorneyControl')?.value === true) {
                    this.model.requesterLetterOfAttorney = form.get('letterOfAttorneyControl')?.value;
                }
                else {
                    this.model.requesterLetterOfAttorney = undefined;
                }

                if (this.hasDelivery) {
                    this.model.deliveryData = this.form.get('deliveryDataControl')!.value;
                }
            }

            this.model.holders = this.permitHolders;

            if (this.model instanceof ScientificFishingPermitEditDTO) {
                this.model.permitStatus = ScientificPermitStatusEnum[form.get('statusControl')!.value?.code as keyof typeof ScientificPermitStatusEnum];
                this.model.outings = this.permitOutings;

                this.model.permitLegalReasonsIds = [];
                for (const reason of this.permitLegalReasons) {
                    if (form.get('permitLegalReasonsGroup')!.get(`${reason.value}`)!.value === true) {
                        this.model.permitLegalReasonsIds.push(reason.value!);
                    }
                }

                this.model.coordinationCommittee = form.get('coordinationCommitteeControl')?.value;
                this.model.coordinationLetterNo = form.get('coordinationLetterNoControl')?.value;
                this.model.coordinationDate = form.get('coordinationDateControl')?.value;
                this.model.coordinationComments = form.get('coordinationCommentsControl')?.value;
            }

            this.model.researchPeriodFrom = (form.get('researchPeriodControl')!.value as DateRangeData)?.start;
            this.model.researchPeriodTo = (form.get('researchPeriodControl')!.value as DateRangeData)?.end;
            this.model.researchWaterArea = form.get('researchWaterAreasControl')!.value;
            this.model.researchGoalsDescription = form.get('researchGoalsControl')!.value;

            this.model.fishTypesDescription = form.get('fishTypesControl')!.value;
            this.model.fishTypesApp4ZBRDesc = form.get('fishTypesApp4ZBRDescControl')!.value;
            this.model.fishTypesCrayFish = form.get('fishTypesCrayFishControl')!.value;

            this.model.fishingGearDescription = form.get('fishingGearControl')!.value;

            this.model.isShipRegistered = !form.get('shipIsNotRegisteredControl')!.value;
            if (this.model.isShipRegistered) {
                this.model.shipId = form.get('existingShipNameControl')!.value?.value;
                this.model.shipCaptainName = form.get('shipCaptainNameControl')!.value;
            }
            else {
                this.model.shipName = form.get('shipNameControl')!.value;
                this.model.shipExternalMark = form.get('shipExternalMarkingControl')!.value;
                this.model.shipCaptainName = form.get('shipCaptainNameControl')!.value;
            }

            this.model.files = form.get('filesControl')!.value;
        }
    }

    private fillCommonModelFields(form: FormGroup): void {
        if (this.model instanceof ScientificFishingPermitRegixDataDTO || this.model instanceof ScientificFishingApplicationEditDTO) {
            this.model.requester = form.get('requesterRegixDataControl')!.value;
        }

        this.model.receiver = form.get('receiverRegixDataControl')!.value;
    }

    private fixHoldersNames(): void {
        for (const holder of this.permitHolders) {
            holder.name = '';

            if (holder.regixPersonData!.firstName !== undefined && holder.regixPersonData!.firstName !== null) {
                holder.name = `${holder.regixPersonData!.firstName}`;
            }
            if (holder.regixPersonData!.middleName !== undefined && holder.regixPersonData!.middleName !== null) {
                holder.name = `${holder.name} ${holder.regixPersonData!.middleName}`;
            }
            if (holder.regixPersonData!.lastName !== undefined && holder.regixPersonData!.lastName !== null) {
                holder.name = `${holder.name} ${holder.regixPersonData!.lastName}`;
            }

            holder.egn = holder.regixPersonData?.egnLnc?.egnLnc ?? '';
        }
    }

    // form button handlers
    private savePermit(dialogClose: DialogCloseCallback, fromSaveAsDraft: boolean = false): Observable<boolean> {
        const saveOrEditDone: EventEmitter<boolean> = new EventEmitter<boolean>();

        this.saveOrEdit(fromSaveAsDraft).subscribe({
            next: (id: number | void) => {
                this.hasNoEDeliveryRegistrationError = false;

                if (typeof id === 'number' && id !== undefined) {
                    this.model.id = id;
                    dialogClose(this.model);
                }
                else {
                    dialogClose(this.model);
                }
                saveOrEditDone.emit(true);
                saveOrEditDone.complete();
            },
            error: (response: HttpErrorResponse) => {
                this.handleSaveErrorResponse(response);
            }
        });

        return saveOrEditDone.asObservable();
    }

    private handleSaveErrorResponse(errorResponse: HttpErrorResponse): void {
        if (errorResponse.error !== undefined && errorResponse.error !== null) {
            const error: ErrorModel = errorResponse.error as ErrorModel;

            if (error.code === ErrorCode.NoEDeliveryRegistration) {
                this.hasNoEDeliveryRegistrationError = true;
                this.validityCheckerGroup.validate();
            }
        }
    }

    private openAnnulDialog(dialogClose: DialogCloseCallback, annuling: boolean): void {
        if (this.model instanceof ScientificFishingPermitEditDTO) {
            const title: string = annuling
                ? this.translateService.getValue('scientific-fishing.annul-scientific-fishing-permit')
                : this.translateService.getValue('scientific-fishing.activate-scientific-fishing-permit');

            const dialog = this.annulDialog.openWithTwoButtons({
                title: title,
                TCtor: CancellationDialogComponent,
                headerCancelButton: {
                    cancelBtnClicked: this.closeAnnulDialogBtnClicked.bind(this)
                },
                translteService: this.translateService,
                componentData: new CancellationDialogParams({
                    model: this.model.cancellationDetails,
                    group: annuling ? CancellationReasonGroupEnum.SciFiCancel : CancellationReasonGroupEnum.SciFiActivate
                })
            }, '1200px');

            dialog.subscribe((result: CancellationDetailsDTO | undefined) => {
                if (result !== undefined && result !== null) {
                    result.isActive = annuling;
                    (this.model as ScientificFishingPermitEditDTO).cancellationDetails = result;
                    this.savePermit(dialogClose);
                }
            });
        }
    }

    private closeAnnulDialogBtnClicked(closeFn: HeaderCloseFunction): void {
        closeFn();
    }

    private saveAndPrint(dialogClose: DialogCloseCallback, printType: SciFiPrintTypesEnum): void {
        if (this.model instanceof ScientificFishingPermitEditDTO) {
            this.fillModel(this.form);
            CommonUtils.sanitizeModelStrings(this.model);
            let saveOrEditObservable: Observable<boolean>;

            if (this.permitId !== null && this.permitId !== undefined) {
                saveOrEditObservable = this.service.editAndDownloadRegister(this.model, printType);
            }
            else {
                saveOrEditObservable = this.service.addAndDownloadRegister(this.model, printType);
            }

            saveOrEditObservable.subscribe({
                next: (downloaded: boolean) => {
                    if (downloaded === true) {
                        dialogClose(this.model);
                    }
                }
            });
        }
    }

    private print(printType: SciFiPrintTypesEnum): void {
        this.service.downloadRegister(this.model.id!, printType).subscribe({
            next: (downloaded: boolean) => {
                // nothing to do
            }
        });
    }

    private saveOrEdit(fromSaveAsDraft: boolean): Observable<number | void> {
        this.fillModel(this.form);
        CommonUtils.sanitizeModelStrings(this.model);
        let saveOrEditObservable: Observable<void | number>;

        if (this.model instanceof ScientificFishingPermitEditDTO) {
            if (this.permitId !== undefined) {
                saveOrEditObservable = this.service.editPermit(this.model);
            }
            else {
                saveOrEditObservable = this.service.addPermit(this.model);
            }
        }
        else {
            if (this.model.id !== undefined && this.model.id !== null) {
                saveOrEditObservable = this.service.editApplication(this.model, this.pageCode, fromSaveAsDraft);
            }
            else {
                saveOrEditObservable = this.service.addApplication(this.model);
            }
        }

        return saveOrEditObservable;
    }

    private holderEqualsRegixHolder(holder: ScientificFishingPermitHolderRegixDataDTO): boolean {
        const regixHolder: ScientificFishingPermitHolderRegixDataDTO | undefined = this.expectedResults.holders?.find(x => {
            return x.regixPersonData?.egnLnc?.identifierType === holder.regixPersonData?.egnLnc?.identifierType
                && x.regixPersonData?.egnLnc?.egnLnc === holder.regixPersonData?.egnLnc?.egnLnc;
        });

        if (regixHolder !== undefined) {
            if (!CommonUtils.objectsEqual(holder.regixPersonData, regixHolder.regixPersonData)) {
                return false;
            }

            if (holder.addressRegistrations !== undefined) {
                for (const address of holder.addressRegistrations) {
                    const regixAddress: AddressRegistrationDTO | undefined = regixHolder.addressRegistrations?.find(x => x.addressType === address.addressType);
                    if (!CommonUtils.objectsEqual(address, regixAddress)) {
                        return false;
                    }
                }
            }
        }
        return true;
    }
}
