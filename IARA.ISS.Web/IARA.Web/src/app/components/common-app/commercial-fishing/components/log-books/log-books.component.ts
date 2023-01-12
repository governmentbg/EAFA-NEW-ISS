import { Component, EventEmitter, Input, OnChanges, OnInit, Optional, Output, Self, SimpleChanges, ViewChild } from '@angular/core';
import { AbstractControl, FormControl, NgControl, ValidationErrors, Validator } from '@angular/forms';
import { forkJoin, Observable, Subscription } from 'rxjs';
import { HttpErrorResponse } from '@angular/common/http';
import { MatSnackBar } from '@angular/material/snack-bar';

import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { LogBookEditDTO } from '@app/models/generated/dtos/LogBookEditDTO';
import { TLDataTableComponent } from '@app/shared/components/data-table/tl-data-table.component';
import { TLConfirmDialog } from '@app/shared/components/confirmation-dialog/tl-confirm-dialog';
import { EditLogBookComponent } from '../edit-log-book/edit-log-book.component';
import { TLMatDialog } from '@app/shared/components/dialog-wrapper/tl-mat-dialog';
import { CommercialFishingLogBookEditDTO } from '@app/models/generated/dtos/CommercialFishingLogBookEditDTO';
import { IHeaderAuditButton } from '@app/shared/components/dialog-wrapper/interfaces/header-audit-button.interface';
import { ICatchesAndSalesService } from '@app/interfaces/common-app/catches-and-sales.interface';
import { GridRow } from '@app/shared/components/data-table/models/row.model';
import { HeaderCloseFunction } from '@app/shared/components/dialog-wrapper/interfaces/header-cancel-button.interface';
import { EditLogBookDialogParamsModel } from './models/edit-log-book-dialog-params.model';
import { SimpleAuditDTO } from '@app/models/generated/dtos/SimpleAuditDTO';
import { LogBookGroupsEnum } from '@app/enums/log-book-groups.enum';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { FormControlDataLoader } from '@app/shared/utils/form-control-data-loader';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { CommonNomenclatures } from '@app/services/common-app/common-nomenclatures.service';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { LogBookPagePersonTypesEnum } from '@app/enums/log-book-page-person-types.enum';
import { LogBookStatusesEnum } from '@app/enums/log-book-statuses.enum';
import { ChooseLogBookForRenewalComponent } from './components/choose-log-book-for-renewal/choose-log-book-for-renewal.component';
import { ChooseLogBookForRenewalDialogParams } from './models/choose-log-book-for-renewal-dialog-params.model';
import { ILogBookService } from '../edit-log-book/interfaces/log-book.interface';
import { LogBookForRenewalDTO } from '@app/models/generated/dtos/LogBookForRenewalDTO';
import { CustomFormControl } from '@app/shared/utils/custom-form-control';
import { OverlappingLogBooksParameters } from '@app/shared/components/overlapping-log-books/models/overlapping-log-books-parameters.model';
import { OverlappingLogBooksDialogParamsModel } from '@app/shared/components/overlapping-log-books/models/overlapping-log-books-dialog-params.model';
import { OverlappingLogBooksComponent } from '@app/shared/components/overlapping-log-books/overlapping-log-books.component';
import { ErrorCode, ErrorModel } from '@app/models/common/exception.model';
import { RequestProperties } from '@app/shared/services/request-properties';
import { CommercialFishingRegisterCacheService } from '@app/components/administration-app/commercial-fishing-register/services/commercial-fishing-register-cache.service';
import { ShipLogBookPageRegisterDTO } from '@app/models/generated/dtos/ShipLogBookPageRegisterDTO';
import { AdmissionLogBookPageRegisterDTO } from '@app/models/generated/dtos/AdmissionLogBookPageRegisterDTO';
import { TransportationLogBookPageRegisterDTO } from '@app/models/generated/dtos/TransportationLogBookPageRegisterDTO';
import { FirstSaleLogBookPageRegisterDTO } from '@app/models/generated/dtos/FirstSaleLogBookPageRegisterDTO';
import { AquacultureLogBookPageRegisterDTO } from '@app/models/generated/dtos/AquacultureLogBookPageRegisterDTO';
import { LogBookTypesEnum } from '@app/enums/log-book-types.enum';
import { LogBooksCacheService } from './services/log-books-cache.service';

export type SimpleAuditMethod = (id: number) => Observable<SimpleAuditDTO>;
export type OnActionEndedType = 'add' | 'edit' | 'delete' | 'remove' | 'restore';

@Component({
    selector: 'log-books',
    templateUrl: './log-books.component.html',
    providers: [LogBooksCacheService]
})
export class LogBooksComponent extends CustomFormControl<LogBookEditDTO[] | CommercialFishingLogBookEditDTO[]> implements OnInit, OnChanges {
    @Input()
    public isReadonly: boolean = false;

    @Input()
    public logBooksPerPages: number = 5;

    @Input()
    public getLogBookSimpleAuditMethod: SimpleAuditMethod | undefined;

    @Input()
    public logBookGroup!: LogBookGroupsEnum;

    @Input()
    public ownerType: LogBookPagePersonTypesEnum | undefined;

    @Input()
    public hasRenewalValidator: boolean = false;

    @Input()
    public isForPermitLicense: boolean = false;

    /**
     * Only for ship log books - it is used in the `choose log book for renewal` button funcionallity
     * */
    @Input()
    public permitLicenseId: number | undefined;

    /**
     * Service needs to be provided when the `choose log book for renewal` funcionallity is wanted or when there is NO ngControl
     * */
    @Input()
    public service: ILogBookService | undefined;

    /**
     * Needed only when there is NO ngControl
     * */
    @Input()
    public permitLicenseLogBookCacheService: CommercialFishingRegisterCacheService | undefined;

    /**
     * Length of ship, for which are these log books (in cases of LogBookGroupsEnum = ship). 
     * Needed in order to know if the log books are online or on paper.
     **/
    @Input()
    public isOnline: boolean | undefined;

    @Input()
    public showInactivePagesAndDocuments: boolean = false;

    @Output()
    public onActionEnded: EventEmitter<OnActionEndedType> = new EventEmitter<OnActionEndedType>();

    public readonly icIconSize: number = CommonUtils.IC_ICON_SIZE;
    public readonly logBookGroupsEnum: typeof LogBookGroupsEnum = LogBookGroupsEnum;

    public logBooks: LogBookEditDTO[] | CommercialFishingLogBookEditDTO[] = [];
    public logBookStatuses: NomenclatureDTO<number>[] = [];
    public logBookTypes: NomenclatureDTO<number>[] = [];
    public hasLogBooksForRenewal: boolean = false;
    public hasNotTouchedRenewals: boolean = false; // needed only for Ship log books

    @ViewChild('logBooksTable')
    private logBooksTable!: TLDataTableComponent;

    private readonly translate: FuseTranslationLoaderService;
    private readonly confirmDialog: TLConfirmDialog;
    private readonly editLogBookDialog: TLMatDialog<EditLogBookComponent>;
    private readonly loader: FormControlDataLoader;
    private readonly nomenclaturesService: CommonNomenclatures;
    private readonly chooseLogBookForRenewalDialog: TLMatDialog<ChooseLogBookForRenewalComponent>;
    private readonly overlappingLogBooksDialog: TLMatDialog<OverlappingLogBooksComponent>;
    private readonly snackbar: MatSnackBar;
    private readonly logBooksCacheService: LogBooksCacheService;

    public constructor(
        @Self() @Optional() ngControl: NgControl,
        translate: FuseTranslationLoaderService,
        confirmDialog: TLConfirmDialog,
        editLogBookDialog: TLMatDialog<EditLogBookComponent>,
        nomenclaturesService: CommonNomenclatures,
        chooseLogBookForRenewalDialog: TLMatDialog<ChooseLogBookForRenewalComponent>,
        overlappingLogBooksDialog: TLMatDialog<OverlappingLogBooksComponent>,
        snackbar: MatSnackBar,
        logBooksCacheService: LogBooksCacheService
    ) {
        super(ngControl, false);

        this.translate = translate;
        this.confirmDialog = confirmDialog;
        this.editLogBookDialog = editLogBookDialog;
        this.nomenclaturesService = nomenclaturesService;
        this.chooseLogBookForRenewalDialog = chooseLogBookForRenewalDialog;
        this.overlappingLogBooksDialog = overlappingLogBooksDialog;
        this.snackbar = snackbar;
        this.logBooksCacheService = logBooksCacheService;

        this.loader = new FormControlDataLoader(this.getNomenclatures.bind(this));
    }

    public ngOnInit(): void {
        this.initCustomFormControl();
        this.loader.load();

        if (!this.ngControl) {
            if (this.service === null || this.service === undefined) {
                throw new Error('The service @Input() must be provided, when there si NO ngControl');
            }

            let cachedLogBooks: CommercialFishingLogBookEditDTO[] | undefined = undefined;

            if (this.permitLicenseLogBookCacheService !== null && this.permitLicenseLogBookCacheService !== undefined) {
                cachedLogBooks = this.permitLicenseLogBookCacheService.getLogBooks(this.permitLicenseId!);
            }

            if (cachedLogBooks !== null && cachedLogBooks !== undefined) {
                this.writeValue(cachedLogBooks.slice());
            }
            else {
                this.service!.getLogBooksForTable(this.permitLicenseId!).subscribe({
                    next: (logBooks: CommercialFishingLogBookEditDTO[]) => {
                        if (this.permitLicenseLogBookCacheService !== null && this.permitLicenseLogBookCacheService !== undefined) {
                            this.permitLicenseLogBookCacheService?.cacheLogBooks(this.permitLicenseId!, logBooks);
                        }
                        else {
                            console.warn('There is no cache service provided for log-books component! This means that the log books will not be cached...');
                        }

                        this.writeValue(logBooks.slice());
                    }
                });
            }

        }
    }

    public ngOnChanges(changes: SimpleChanges): void {
        const hasRenewalValidator: boolean | undefined = changes['hasRenewalValidator']?.currentValue;

        if (hasRenewalValidator !== null && hasRenewalValidator !== undefined) {
            this.hasRenewalValidator = hasRenewalValidator;
            this.checkLogBooksForRenewal();
        }
    }

    public buildForm(): AbstractControl {
        return new FormControl();
    }

    public writeValue(value: LogBookEditDTO[] | CommercialFishingLogBookEditDTO[]): void {
        if (value !== null && value !== undefined) {
            this.loader.load(() => {
                this.logBooks = value.slice();

                if (this.logBooks.length > 0 && this.logBooks[0] instanceof CommercialFishingLogBookEditDTO) {
                    this.hasLogBooksForRenewal = (this.logBooks as CommercialFishingLogBookEditDTO[]).some(x => x.isForRenewal);

                    if (this.ngControl) {
                        this.control.updateValueAndValidity();
                        this.onChanged(this.getValue());
                    }
                }
                else {
                    if (this.ngControl) {
                        this.control.updateValueAndValidity();
                        this.onChanged(this.getValue());
                    }
                }
            });
        }
        else {
            this.logBooks = [];

            if (this.ngControl) {
                this.control.updateValueAndValidity();
                this.onChanged(this.getValue());
            }
        }
    }

    public getValue(): CommercialFishingLogBookEditDTO[] | LogBookEditDTO[] {
        return this.logBooks?.slice() ?? [];
    }

    public setDisabledState(isDisabled: boolean): void {
        if (isDisabled) {
            this.isReadonly = true;
        }
        else {
            this.isReadonly = false;
        }
    }

    public validate(control: AbstractControl): ValidationErrors | null {
        const errors: ValidationErrors = {};

        if (this.hasLogBooksForRenewal && this.logBookGroup === LogBookGroupsEnum.Ship) {
            this.checkLogBooksForRenewal();

            if (this.hasNotTouchedRenewals) {
                errors['hasLogBooksForRenewal'] = true;
            }
        }

        this.checkLocalLogBookRanges();

        if (this.logBooks.some(x => x.hasError && x.isActive)) {
            errors['hasOverlappingRanges'] = true;
        }

        const result = Object.keys(errors).length > 0 ? errors : null;

        this.control.setErrors(result);

        return result;
    }

    public addLogBookFromOldPermitLicense(): void {
        const dialog = this.chooseLogBookForRenewalDialog.openWithTwoButtons({
            title: this.translate.getValue('catches-and-sales.choose-log-book-for-renewal-title'),
            componentData: new ChooseLogBookForRenewalDialogParams({
                permitLicenseId: this.permitLicenseId!,
                service: this.service
            }),
            headerCancelButton: {
                cancelBtnClicked: (closeFn: HeaderCloseFunction) => { closeFn(); }
            },
            translteService: this.translate,
            TCtor: ChooseLogBookForRenewalComponent
        }, '1500px');

        dialog.subscribe({
            next: (chosenLogBooks: LogBookForRenewalDTO[] | undefined) => {
                if (chosenLogBooks !== null && chosenLogBooks !== undefined && chosenLogBooks.length > 0) {
                    let chosenLogBookIds: number[] = chosenLogBooks.map(x => x.logBookPermitLicenseId!);
                    const logBookPermitLicenseIds: number[] = (this.logBooks as CommercialFishingLogBookEditDTO[]).map(x => x.logBookLicenseId!);
                    chosenLogBookIds = chosenLogBookIds.filter(x => !logBookPermitLicenseIds.includes(x));

                    if (chosenLogBookIds.length > 0) {
                        this.service!.getLogBooksForRenewalByIds(chosenLogBookIds).subscribe({
                            next: (results: CommercialFishingLogBookEditDTO[]) => {
                                if (this.ngControl) {
                                    this.handleAddLogBooksFromOldPermitLicenses(results);

                                    this.control.updateValueAndValidity();
                                    this.control.markAsTouched();
                                    this.onChanged(this.getValue());
                                }
                                else {
                                    const renewedStatusId: number = this.logBookStatuses.find(x => x.code === LogBookStatusesEnum[LogBookStatusesEnum.Renewed])!.value!;
                                    for (const logBook of results) {
                                        let permitLicenseStartPageProposal: number;

                                        if (logBook.lastPageNumber !== null && logBook.lastPageNumber !== undefined && logBook.lastPageNumber > 0) {
                                            permitLicenseStartPageProposal = logBook.lastPageNumber + 1;
                                        }
                                        else {
                                            permitLicenseStartPageProposal = logBook.startPageNumber!;
                                        }

                                        if (permitLicenseStartPageProposal > logBook.endPageNumber!) {
                                            permitLicenseStartPageProposal = logBook.endPageNumber!;
                                        }

                                        logBook.permitLicenseStartPageNumber = permitLicenseStartPageProposal;
                                        logBook.permitLicenseEndPageNumber = logBook.endPageNumber;

                                        logBook.statusId = renewedStatusId;
                                    }

                                    this.service!.addLogBooksFromOldPermitLicenses(results, this.permitLicenseId!, false).subscribe({
                                        next: () => {
                                            this.handleAddLogBooksFromOldPermitLicenses(results);
                                            this.onActionEnded.emit('add');
                                        },
                                        error: (errorResponse: HttpErrorResponse) => {
                                            const error = errorResponse.error as ErrorModel;

                                            if (error?.code === ErrorCode.InvalidLogBookLicensePagesRange
                                                || error?.code === ErrorCode.InvalidLogBookPagesRange
                                            ) {
                                                const logBookNumber: string = error!.messages[0];

                                                const logBook: CommercialFishingLogBookEditDTO = results.find(x => x.logbookNumber === logBookNumber)!;
                                                this.handleInvalidLogBookLicensePagesRangeError(logBook, false, logBook, error.messages[0], results);
                                            }
                                        }
                                    });
                                }
                            }
                        });
                    }
                }
            }
        });
    }

    public addEditLogBook(logBook?: LogBookEditDTO | CommercialFishingLogBookEditDTO, viewMode: boolean = false): void {
        if (logBook !== null && logBook !== undefined) { // edit
            // first get log book relared pages and declarations (from in memory cache or the server)

            const logBookType: LogBookTypesEnum = LogBookTypesEnum[this.logBookTypes.find(x => x.value === logBook.logBookTypeId)!.code as keyof typeof LogBookTypesEnum];
            let hasCachedData: boolean = false;

            switch (logBookType) {
                case LogBookTypesEnum.Ship: {
                    if (logBook instanceof CommercialFishingLogBookEditDTO) {
                        logBook.shipPagesAndDeclarations = this.logBooksCacheService.getShipLogBookPages(logBook.logBookId!, this.permitLicenseId!);

                        if (logBook.shipPagesAndDeclarations !== null && (logBook as CommercialFishingLogBookEditDTO).shipPagesAndDeclarations !== undefined) {
                            hasCachedData = true;
                        }
                    }

                } break;
                case LogBookTypesEnum.FirstSale: {
                    logBook.firstSalePages = this.logBooksCacheService.getFirstSaleLogBookPages(logBook.logBookId!);

                    if (logBook.firstSalePages !== null && logBook.firstSalePages !== undefined) {
                        hasCachedData = true;
                    }
                } break;
                case LogBookTypesEnum.Admission: {
                    switch (logBook.ownerType) {
                        case LogBookPagePersonTypesEnum.RegisteredBuyer: {
                            logBook.admissionPagesAndDeclarations = this.logBooksCacheService.getAdmissionLogBookPages(logBook.logBookId!);
                        } break;
                        case LogBookPagePersonTypesEnum.Person:
                        case LogBookPagePersonTypesEnum.LegalPerson: {
                            logBook.admissionPagesAndDeclarations = this.logBooksCacheService.getShipAdmissionLogBookPages(logBook.logBookId!, this.permitLicenseId!);
                        } break;
                    }

                    if (logBook.admissionPagesAndDeclarations !== null && logBook.admissionPagesAndDeclarations !== undefined) {
                        hasCachedData = true;
                    }
                } break;
                case LogBookTypesEnum.Transportation: {
                    switch (logBook.ownerType) {
                        case LogBookPagePersonTypesEnum.RegisteredBuyer: {
                            logBook.transportationPagesAndDeclarations = this.logBooksCacheService.getTransportationLogBookPages(logBook.logBookId!);
                        } break;
                        case LogBookPagePersonTypesEnum.Person:
                        case LogBookPagePersonTypesEnum.LegalPerson: {
                            logBook.transportationPagesAndDeclarations = this.logBooksCacheService.getShipTransportationLogBookPages(logBook.logBookId!, this.permitLicenseId!);
                        } break;
                    }
                    
                    if (logBook.transportationPagesAndDeclarations !== null && logBook.transportationPagesAndDeclarations !== undefined) {
                        hasCachedData = true;
                    }
                } break;
                case LogBookTypesEnum.Aquaculture: {
                    logBook.aquaculturePages = this.logBooksCacheService.getAquacultureLogBookPages(logBook.logBookId!);

                    if (logBook.aquaculturePages !== null && logBook.aquaculturePages !== undefined) {
                        hasCachedData = true;
                    }
                } break;
            }

            if (!hasCachedData) { // Ако не са кеширани все още данните за този дневник, вземаме ги от сървъра и ги кешираме
                if (logBook instanceof CommercialFishingLogBookEditDTO) {
                    this.service!.getLogBookPagesAndDeclarations(logBook.logBookId!, this.permitLicenseId!, logBookType).subscribe({
                        next: (pages: ShipLogBookPageRegisterDTO[] | AdmissionLogBookPageRegisterDTO[] | TransportationLogBookPageRegisterDTO[]) => {
                            if (pages !== null && pages !== undefined && pages.length > 0) {
                                if (pages[0] instanceof ShipLogBookPageRegisterDTO) {
                                    logBook.shipPagesAndDeclarations = pages as ShipLogBookPageRegisterDTO[];
                                    this.logBooksCacheService.cacheShipLogBookPages(logBook.logBookId!, this.permitLicenseId!, logBook.shipPagesAndDeclarations.slice());
                                }
                                else if (pages[0] instanceof AdmissionLogBookPageRegisterDTO) {
                                    logBook.admissionPagesAndDeclarations = pages as AdmissionLogBookPageRegisterDTO[];
                                    this.logBooksCacheService.cacheShipAdmissionLogBookPages(logBook.logBookId!, this.permitLicenseId!, logBook.admissionPagesAndDeclarations.slice());
                                }
                                else if (pages[0] instanceof TransportationLogBookPageRegisterDTO) {
                                    logBook.transportationPagesAndDeclarations = pages as TransportationLogBookPageRegisterDTO[];
                                    this.logBooksCacheService.cacheShipTransportationLogBookPages(logBook.logBookId!, this.permitLicenseId!, logBook.transportationPagesAndDeclarations.slice());
                                }
                            }

                            this.handleOpenLogBookDialog(logBook, viewMode);
                        }
                    });
                }
                else {
                    this.service!.getLogBookPages(logBook.logBookId!, logBookType).subscribe({
                        next: (
                            pages: AdmissionLogBookPageRegisterDTO[]
                                | TransportationLogBookPageRegisterDTO[]
                                | FirstSaleLogBookPageRegisterDTO[]
                                | AquacultureLogBookPageRegisterDTO[]
                        ) => {
                            if (pages !== null && pages !== undefined && pages.length > 0) {
                                if (pages[0] instanceof AdmissionLogBookPageRegisterDTO) {
                                    logBook.admissionPagesAndDeclarations = pages;
                                    this.logBooksCacheService.cacheAdmissionLogBookPages(logBook.logBookId!, logBook.admissionPagesAndDeclarations.slice());
                                }
                                else if (pages[0] instanceof TransportationLogBookPageRegisterDTO) {
                                    logBook.transportationPagesAndDeclarations = pages;
                                    this.logBooksCacheService.cacheTransportationLogBookPages(logBook.logBookId!, logBook.transportationPagesAndDeclarations.slice());
                                }
                                else if (pages[0] instanceof FirstSaleLogBookPageRegisterDTO) {
                                    logBook.firstSalePages = pages;
                                    this.logBooksCacheService.cacheFirstSaleLogBookPages(logBook.logBookId!, logBook.firstSalePages.slice());
                                }
                                else if (pages[0] instanceof AquacultureLogBookPageRegisterDTO) {
                                    logBook.aquaculturePages = pages;
                                    this.logBooksCacheService.cacheAquacultureLogBookPages(logBook.logBookId!, logBook.aquaculturePages.slice());
                                }
                            }

                            this.handleOpenLogBookDialog(logBook, viewMode);
                        }
                    });
                }
            }
            else {
                this.handleOpenLogBookDialog(logBook, viewMode);
            }
        }
        else { // add
            this.handleOpenLogBookDialog(logBook, viewMode);
        }
    }

    public removeLogBook(row: GridRow<CommercialFishingLogBookEditDTO>): void {
        this.logBooks = this.logBooks.filter(x => x.logBookId !== row.data.logBookId);

        this.checkLogBooksForRenewal();
        this.checkLocalLogBookRanges();

        if (this.ngControl) {
            this.control.updateValueAndValidity();
            this.control.markAsTouched();
            this.onChanged(this.getValue());
        }

        this.onActionEnded.emit('remove');
    }

    public deleteLogBook(row: GridRow<LogBookEditDTO | CommercialFishingLogBookEditDTO>): void {
        this.confirmDialog.open({
            title: this.translate.getValue('catches-and-sales.delete-log-book-dialog-label'),
            message: this.translate.getValue('catches-and-sales.confirm-delete-log-book-message'),
            okBtnLabel: this.translate.getValue('catches-and-sales.delete-log-book-btn-label')
        }).subscribe({
            next: (ok: boolean) => {
                if (ok) {
                    if (this.ngControl) {
                        this.handleDeleteLogBook(row);

                        this.control.updateValueAndValidity();
                        this.control.markAsTouched();
                        this.onChanged(this.getValue());
                    }
                    else {
                        this.service!.deleteLogBookPermitLicense((row.data as CommercialFishingLogBookEditDTO).logBookLicenseId!).subscribe({
                            next: () => {
                                this.handleDeleteLogBook(row);
                                this.onActionEnded.emit('delete');
                            },
                            error: (httpErrorResponse: HttpErrorResponse) => {
                                if ((httpErrorResponse.error as ErrorModel)?.code === ErrorCode.LogBookHasSubmittedPages) {
                                    const message: string = this.translate.getValue('catches-and-sales.cannot-delete-log-book-with-submitted-pages');
                                    this.snackbar.open(message, undefined, {
                                        duration: RequestProperties.DEFAULT.showExceptionDurationErr,
                                        panelClass: RequestProperties.DEFAULT.showExceptionColorClassErr
                                    });
                                }
                            }
                        });
                    }
                }
            }
        });
    }

    public undoDeleteLogBook(row: GridRow<LogBookEditDTO | CommercialFishingLogBookEditDTO>): void {
        this.confirmDialog.open().subscribe({
            next: (ok: boolean) => {
                if (ok) {
                    if (this.ngControl) {
                        this.handleUndoDeleteLogBook(row);

                        this.control.updateValueAndValidity();
                        this.control.markAsTouched();
                        this.onChanged(this.getValue());
                    }
                    else {
                        this.service!.undoDeleteLogBookPermitLicense((row.data as CommercialFishingLogBookEditDTO).logBookLicenseId!).subscribe({
                            next: () => {
                                this.handleUndoDeleteLogBook(row);
                                this.onActionEnded.emit('restore');
                            }
                        });
                    }
                }
            }
        });
    }

    private handleOpenLogBookDialog(logBook: LogBookEditDTO | CommercialFishingLogBookEditDTO | undefined, viewMode: boolean): void {
        let data: EditLogBookDialogParamsModel | undefined;
        let headerAuditBtn: IHeaderAuditButton | undefined;
        let title: string;

        if (logBook !== null && logBook !== undefined) { // edit
            data = new EditLogBookDialogParamsModel({
                model: logBook,
                readOnly: this.isReadonly || viewMode,
                logBookGroup: this.logBookGroup,
                ownerType: this.ownerType,
                pagesRangeError: logBook.hasError ?? false,
                isOnline: this.isOnline,
                isForPermitLicense: this.isForPermitLicense
            });

            if (logBook.logBookId !== undefined && logBook.logBookId !== null && this.getLogBookSimpleAuditMethod !== null && this.getLogBookSimpleAuditMethod !== undefined) {
                headerAuditBtn = {
                    id: logBook.logBookId,
                    getAuditRecordData: this.getLogBookSimpleAuditMethod,
                    tableName: 'LogBook'
                }
            }

            if (this.isReadonly || viewMode) {
                title = this.translate.getValue('catches-and-sales.view-log-book-dialog-title');
            }
            else {
                title = this.translate.getValue('catches-and-sales.edit-log-book-dialog-title');
            }
        }
        else { // add
            data = new EditLogBookDialogParamsModel({
                logBookGroup: this.logBookGroup,
                isOnline: this.isOnline,
                ownerType: this.ownerType,
                isForPermitLicense: this.isForPermitLicense
            });

            title = this.translate.getValue('catches-and-sales.add-log-book-dialog-title');
        }

        const dialog = this.editLogBookDialog.openWithTwoButtons({
            title: title,
            TCtor: EditLogBookComponent,
            headerAuditButton: headerAuditBtn,
            headerCancelButton: {
                cancelBtnClicked: this.closeEditLogBookDialogBtnClicked.bind(this)
            },
            componentData: data,
            translteService: this.translate,
            disableDialogClose: true,
            viewMode: this.isReadonly || viewMode
        }, '1400px');

        dialog.subscribe((result: LogBookEditDTO | CommercialFishingLogBookEditDTO | null | undefined) => {
            if (result !== null && result !== undefined) {
                let isEdit: boolean = false;

                if (logBook !== null && logBook !== undefined) {
                    isEdit = true;
                }
                else {
                    isEdit = false;
                }

                if (this.ngControl) {
                    this.handleAddOrEditLogBook(logBook, result);

                    this.control.updateValueAndValidity();
                    this.control.markAsTouched();
                    this.onChanged(this.getValue());
                }
                else {
                    this.saveLogBook(logBook, isEdit, result, this.permitLicenseId!, false);
                }
            }
        });
    }

    private handleAddLogBooksFromOldPermitLicenses(results: CommercialFishingLogBookEditDTO[]): void {
        this.logBooks.push(...results);
        this.hasLogBooksForRenewal = true;

        this.checkLogBooksForRenewal();
        this.checkLocalLogBookRanges();

        this.logBooks = this.logBooks.slice();
    }

    private handleAddOrEditLogBook(logBook: LogBookEditDTO | CommercialFishingLogBookEditDTO | undefined, result: LogBookEditDTO | CommercialFishingLogBookEditDTO): void {
        if (logBook !== null && logBook !== undefined) {
            result.hasError = false;
            logBook = result;
        }
        else {
            this.logBooks.push(result);
        }

        this.checkLogBooksForRenewal();
        this.checkLocalLogBookRanges();


        this.logBooks = this.logBooks.slice();
    }

    private handleDeleteLogBook(row: GridRow<LogBookEditDTO | CommercialFishingLogBookEditDTO>): void {
        if (row.data instanceof CommercialFishingLogBookEditDTO) {
            (this.logBooks as CommercialFishingLogBookEditDTO[]).find(x => x === row.data)!.permitLicenseIsActive = false;
        }
        else {
            (this.logBooks as LogBookEditDTO[]).find(x => x === row.data)!.logBookIsActive = false;
        }

        this.logBooksTable.softDelete(row);

        this.checkLogBooksForRenewal();
        this.checkLocalLogBookRanges();
    }

    private handleUndoDeleteLogBook(row: GridRow<LogBookEditDTO | CommercialFishingLogBookEditDTO>): void {
        if (row.data instanceof CommercialFishingLogBookEditDTO) {
            (this.logBooks as CommercialFishingLogBookEditDTO[]).find(x => x === row.data)!.permitLicenseIsActive = true;
        }
        else {
            (this.logBooks as LogBookEditDTO[]).find(x => x === row.data)!.logBookIsActive = true;
        }

        this.logBooksTable.softUndoDelete(row);

        this.checkLogBooksForRenewal();
        this.checkLocalLogBookRanges();
    }

    private getNomenclatures(): Subscription {
        const subscription: Subscription = forkJoin([
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.LogBookStatuses, this.nomenclaturesService.getLogBookStatuses.bind(this.nomenclaturesService), false),
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.LogBookTypes, this.nomenclaturesService.getLogBookTypes.bind(this.nomenclaturesService), false)
        ]).subscribe({
            next: (nomenclatures: NomenclatureDTO<number>[][]) => {
                this.logBookStatuses = nomenclatures[0];
                this.logBookTypes = nomenclatures[1];

                this.loader.complete();
            }
        });

        return subscription;
    }

    private checkLogBooksForRenewal(): void {
        if (this.hasRenewalValidator) {
            this.hasNotTouchedRenewals = (this.logBooks as CommercialFishingLogBookEditDTO[]).some(x => x.isForRenewal && (x.statusId === null || x.statusId === undefined));
        }
        else {
            this.hasNotTouchedRenewals = false;
        }
    }

    private checkLocalLogBookRanges(): void {
        this.checkLogBooksForOverlappingLogBookRanges();
    }

    private checkLogBooksForOverlappingLogBookRanges(): void {
        for (const logBook of this.logBooks) {
            const finishedStatusIds: (number | undefined)[] = this.logBookStatuses.filter(x => LogBookStatusesEnum[x.code as keyof typeof LogBookStatusesEnum] === LogBookStatusesEnum.Finished).map(x => x.value);

            let overlappingLogBooks: (LogBookEditDTO | CommercialFishingLogBookEditDTO)[] = [];

            if (logBook instanceof CommercialFishingLogBookEditDTO) {
                if (logBook.permitLicenseStartPageNumber !== null
                    && logBook.permitLicenseStartPageNumber !== undefined
                    && logBook.permitLicenseEndPageNumber !== null
                    && logBook.permitLicenseEndPageNumber !== undefined
                ) {
                    overlappingLogBooks = this.logBooks.filter((x: CommercialFishingLogBookEditDTO) => {
                        if (x.logBookTypeId === logBook.logBookTypeId
                            && x.isActive
                            && x.permitLicenseStartPageNumber !== null
                            && x.permitLicenseStartPageNumber !== undefined
                            && x.permitLicenseEndPageNumber !== null
                            && x.permitLicenseEndPageNumber !== undefined
                            && !(x.permitLicenseEndPageNumber! < logBook.permitLicenseStartPageNumber! || logBook.permitLicenseEndPageNumber! < x.permitLicenseStartPageNumber!)
                            && !finishedStatusIds.includes(x.statusId)
                        ) {
                            return true;
                        }

                        return false;
                    });
                }
            }
            else {
                overlappingLogBooks = this.logBooks.filter((x: LogBookEditDTO) => {
                    if (x.logBookTypeId === logBook.logBookTypeId
                        && x.isActive
                        && !(x.endPageNumber! < logBook.startPageNumber! || logBook.endPageNumber! < x.startPageNumber!)
                        && !finishedStatusIds.includes(x.statusId)
                    ) {
                        return true;
                    }

                    return false;
                });
            }

            if (overlappingLogBooks.length > 1) {
                logBook.hasError = true;
            }
            else {
                logBook.hasError = false;
            }
        }

        this.logBooks = this.logBooks.slice();
    }

    private handleInvalidLogBookLicensePagesRangeError(
        logBook: CommercialFishingLogBookEditDTO | LogBookEditDTO | undefined,
        isEdit: boolean,
        model: CommercialFishingLogBookEditDTO | LogBookEditDTO,
        logBookNumber: string,
        logBookForRenew: CommercialFishingLogBookEditDTO[] | undefined
    ): void {
        let ranges: OverlappingLogBooksParameters[] = [];
        let ignoreLogBookConflicts = false;

        if (model instanceof CommercialFishingLogBookEditDTO) {
            ranges.push(
                new OverlappingLogBooksParameters({
                    logBookId: model.logBookId,
                    typeId: model.logBookTypeId,
                    OwnerType: model.ownerType,
                    startPage: model.permitLicenseStartPageNumber,
                    endPage: model.permitLicenseEndPageNumber
                })
            );
        }
        else {
            ranges.push(
                new OverlappingLogBooksParameters({
                    logBookId: model.logBookId,
                    typeId: model.logBookTypeId,
                    OwnerType: model.ownerType,
                    startPage: model.startPageNumber,
                    endPage: model.endPageNumber
                })
            );
        }

        const editDialogData: OverlappingLogBooksDialogParamsModel = new OverlappingLogBooksDialogParamsModel({
            service: this.service,
            logBookGroup: this.logBookGroup,
            ranges: ranges
        });

        this.overlappingLogBooksDialog.open({
            title: this.translate.getValue('catches-and-sales.overlapping-log-books-dialog-title'),
            TCtor: OverlappingLogBooksComponent,
            headerCancelButton: {
                cancelBtnClicked: (closeFn: HeaderCloseFunction) => { closeFn(); }
            },
            componentData: editDialogData,
            translteService: this.translate,
            disableDialogClose: true,
            cancelBtn: {
                id: 'cancel',
                color: 'primary',
                translateValue: 'common.cancel',
            },
            saveBtn: {
                id: 'save',
                color: 'error',
                translateValue: 'catches-and-sales.overlapping-log-books-save-despite-conflicts'
            }
        }, '1300px').subscribe({
            next: (save: boolean | undefined) => {
                if (save) {
                    ignoreLogBookConflicts = true;
                    if (logBookForRenew) {
                        this.service!.addLogBooksFromOldPermitLicenses(logBookForRenew, this.permitLicenseId!, ignoreLogBookConflicts).subscribe({
                            next: () => {
                                this.handleAddLogBooksFromOldPermitLicenses(logBookForRenew);
                                this.onActionEnded.emit('add');
                            }
                        });
                    }
                    else {
                        this.saveLogBook(logBook, isEdit, model, this.permitLicenseId!, ignoreLogBookConflicts);
                    }   
                }
            }
        });
    }

    private saveLogBook(
        logBook: CommercialFishingLogBookEditDTO | LogBookEditDTO | undefined,
        isEdit: boolean,
        model: CommercialFishingLogBookEditDTO,
        registerId: number,
        ignoreLogBookConflicts: boolean
    ): void {
        if (isEdit) {
            this.service!.editLogBook(model, registerId, ignoreLogBookConflicts).subscribe({
                next: () => {
                    this.handleAddOrEditLogBook(logBook, model);
                    this.onActionEnded.emit('edit');
                },
                error: (errorResponse: HttpErrorResponse) => {
                    const error = errorResponse.error as ErrorModel;
                    if (error?.code === ErrorCode.InvalidLogBookLicensePagesRange
                        || error?.code === ErrorCode.InvalidLogBookPagesRange
                    ) {
                        this.handleInvalidLogBookLicensePagesRangeError(logBook, isEdit, model, error.messages[0], undefined);
                    }
                }
            });
        }
        else {
            this.service!.addLogBook(model, registerId, ignoreLogBookConflicts).subscribe({
                next: () => {
                    this.handleAddOrEditLogBook(logBook, model);
                    this.onActionEnded.emit('add');
                },
                error: (errorResponse: HttpErrorResponse) => {
                    const error = errorResponse.error as ErrorModel;
                    if (error?.code === ErrorCode.InvalidLogBookLicensePagesRange
                        || error?.code === ErrorCode.InvalidLogBookPagesRange
                    ) {
                        this.handleInvalidLogBookLicensePagesRangeError(logBook, isEdit, model, error.messages[0], undefined);
                    }
                }
            });
        }
    }

    private closeEditLogBookDialogBtnClicked(closeFn: HeaderCloseFunction): void {
        closeFn();
    }
}