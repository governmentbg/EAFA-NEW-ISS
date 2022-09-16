import { Component, DoCheck, Input, OnChanges, OnInit, Self, SimpleChanges, ViewChild } from '@angular/core';
import { AbstractControl, ControlValueAccessor, NgControl, ValidationErrors, Validator } from '@angular/forms';
import { forkJoin, Observable, Subscription } from 'rxjs';

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

export type SimpleAuditMethod = (id: number) => Observable<SimpleAuditDTO>;

@Component({
    selector: 'log-books',
    templateUrl: './log-books.component.html'
})
export class LogBooksComponent implements OnInit, OnChanges, DoCheck, ControlValueAccessor, Validator {
    @Input()
    public isReadonly: boolean = false;

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
     * Length of ship, for which are these log books (in cases of LogBookGroupsEnum = ship). 
     * Needed in order to know if the log books are online or on paper.
     **/
    @Input()
    public isOnline: boolean | undefined;

    @Input()
    public showInactivePagesAndDocuments: boolean = false;

    public readonly icIconSize: number = CommonUtils.IC_ICON_SIZE;
    public readonly logBookGroupsEnum: typeof LogBookGroupsEnum = LogBookGroupsEnum;

    public logBooks: LogBookEditDTO[] | CommercialFishingLogBookEditDTO[] = [];
    public logBookStatuses: NomenclatureDTO<number>[] = [];
    public logBookTypes: NomenclatureDTO<number>[] = [];
    public hasLogBooksForRenewal: boolean = false;
    public isTouched: boolean = false;
    public hasNotTouchedRenewals: boolean = false; // needed only for Ship log books

    @ViewChild('logBooksTable')
    private logBooksTable!: TLDataTableComponent;

    private ngControl: NgControl;
    private onChanged: (value: LogBookEditDTO[] | CommercialFishingLogBookEditDTO[]) => void = (value: LogBookEditDTO[] | CommercialFishingLogBookEditDTO[]) => { return; };
    private onTouched: (value: LogBookEditDTO[] | CommercialFishingLogBookEditDTO[]) => void = (value: LogBookEditDTO[] | CommercialFishingLogBookEditDTO[]) => { return; };

    private translate: FuseTranslationLoaderService;
    private confirmDialog: TLConfirmDialog;
    private editLogBookDialog: TLMatDialog<EditLogBookComponent>;
    private readonly loader: FormControlDataLoader;
    private nomenclaturesService: CommonNomenclatures;

    public constructor(
        @Self() ngControl: NgControl,
        translate: FuseTranslationLoaderService,
        confirmDialog: TLConfirmDialog,
        editLogBookDialog: TLMatDialog<EditLogBookComponent>,
        nomenclaturesService: CommonNomenclatures
    ) {
        this.ngControl = ngControl;
        this.ngControl.valueAccessor = this;

        this.translate = translate;
        this.confirmDialog = confirmDialog;
        this.editLogBookDialog = editLogBookDialog;
        this.nomenclaturesService = nomenclaturesService;

        this.loader = new FormControlDataLoader(this.getNomenclatures.bind(this));
    }

    public ngOnInit(): void {
        if (this.ngControl.control) {
            this.ngControl.control.validator = this.validate.bind(this);
        }

        this.loader.load();
    }

    public ngDoCheck(): void {
        if (this.ngControl?.control?.touched) {
            this.isTouched = true;
            this.validate(this.ngControl.control!);
        }
        else {
            this.isTouched = false;
        }
    }

    public ngOnChanges(changes: SimpleChanges): void {
        const hasRenewalValidator: boolean | undefined = changes['hasRenewalValidator']?.currentValue;

        if (hasRenewalValidator !== null && hasRenewalValidator !== undefined) {
            this.hasRenewalValidator = hasRenewalValidator;
            this.checkLogBooksForRenewal();
        }
    }

    public writeValue(value: LogBookEditDTO[] | CommercialFishingLogBookEditDTO[]): void {
        if (value !== null && value !== undefined) {
            this.loader.load(() => {
                setTimeout(() => {
                    this.logBooks = value;

                    if (this.logBooks.length > 0 && this.logBooks[0] instanceof CommercialFishingLogBookEditDTO) {
                        setTimeout(() => {
                            this.hasLogBooksForRenewal = (this.logBooks as CommercialFishingLogBookEditDTO[]).some(x => x.isForRenewal);
                            this.onChanged(this.logBooks);
                        });
                    }
                    else {
                        this.onChanged(value);
                    }
                });
            });
        }
        else {
            this.logBooks = [];
            this.onChanged(this.logBooks);
        }
    }

    public registerOnChange(fn: (value: LogBookEditDTO[] | CommercialFishingLogBookEditDTO[]) => void): void {
        this.onChanged = fn;
    }

    public registerOnTouched(fn: (value: LogBookEditDTO[] | CommercialFishingLogBookEditDTO[]) => void): void {
        this.onTouched = fn;
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

        this.checkLocalLogBookRagnes();

        if (this.logBooks.some(x => x.hasError && x.isActive)) {
            errors['hasOverlappingRanges'] = true;
        }

        return Object.keys(errors).length > 0 ? errors : null;
    }

    public addEditLogBook(logBook?: LogBookEditDTO | CommercialFishingLogBookEditDTO, viewMode: boolean = false): void {
        let data: EditLogBookDialogParamsModel | undefined;
        let headerAuditBtn: IHeaderAuditButton | undefined;
        let title: string;

        if (logBook !== undefined) {
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
        else {
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
        }, '1200px');

        dialog.subscribe((result: LogBookEditDTO | CommercialFishingLogBookEditDTO | null | undefined) => {
            if (result !== null && result !== undefined) {
                if (logBook !== null && logBook !== undefined) {
                    result.hasError = false;
                    logBook = result;
                }
                else {
                    this.logBooks.push(result);
                }

                this.checkLocalLogBookRagnes();

                setTimeout(() => {
                    this.logBooks = this.logBooks.slice();
                    this.onChanged(this.logBooks);
                });
            }
        });
    }

    public removeLogBook(row: GridRow<CommercialFishingLogBookEditDTO>): void {
        this.logBooks = this.logBooks.filter(x => x.logBookId !== row.data.logBookId);
        this.checkLogBooksForRenewal();
        this.checkLocalLogBookRagnes();
        this.onChanged(this.logBooks);
    }

    public deleteLogBook(row: GridRow<LogBookEditDTO | CommercialFishingLogBookEditDTO>): void {
        this.confirmDialog.open({
            title: this.translate.getValue('catches-and-sales.delete-log-book-dialog-label'),
            message: this.translate.getValue('catches-and-sales.confirm-delete-log-book-message'),
            okBtnLabel: this.translate.getValue('catches-and-sales.delete-log-book-btn-label')
        }).subscribe({
            next: (ok: boolean) => {
                if (ok) {
                    if (row.data instanceof CommercialFishingLogBookEditDTO) {
                        (this.logBooks as CommercialFishingLogBookEditDTO[]).find(x => x === row.data)!.permitLicenseIsActive = false;
                    }
                    else {
                        (this.logBooks as LogBookEditDTO[]).find(x => x === row.data)!.logBookIsActive = false;
                    }

                    this.logBooksTable.softDelete(row);
                    this.checkLogBooksForRenewal();
                    this.checkLocalLogBookRagnes();
                    this.onChanged(this.logBooks);
                }
            }
        });
    }

    public undoDeleteLogBook(row: GridRow<LogBookEditDTO | CommercialFishingLogBookEditDTO>): void {
        this.confirmDialog.open().subscribe({
            next: (ok: boolean) => {
                if (ok) {
                    if (row.data instanceof CommercialFishingLogBookEditDTO) {
                        (this.logBooks as CommercialFishingLogBookEditDTO[]).find(x => x === row.data)!.permitLicenseIsActive = true;
                    }
                    else {
                        (this.logBooks as LogBookEditDTO[]).find(x => x === row.data)!.logBookIsActive = true;
                    }

                    this.logBooksTable.softUndoDelete(row);
                    this.checkLogBooksForRenewal();
                    this.checkLocalLogBookRagnes();
                    this.onChanged(this.logBooks);
                }
            }
        });
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

    private checkLocalLogBookRagnes(): void {
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

    private closeEditLogBookDialogBtnClicked(closeFn: HeaderCloseFunction): void {
        closeFn();
    }
}