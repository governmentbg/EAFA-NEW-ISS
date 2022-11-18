import { Component, OnInit, ViewChild } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';

import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { TLValidators } from '@app/shared/utils/tl-validators';
import { CommercialFishingLogBookEditDTO } from '@app/models/generated/dtos/CommercialFishingLogBookEditDTO';
import { EditLogBookDialogParamsModel } from '../log-books/models/edit-log-book-dialog-params.model';
import { LogBookEditDTO } from '@app/models/generated/dtos/LogBookEditDTO';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { LogBookGroupsEnum } from '@app/enums/log-book-groups.enum';
import { CommonNomenclatures } from '@app/services/common-app/common-nomenclatures.service';
import { LogBookPagePersonTypesEnum } from '@app/enums/log-book-page-person-types.enum';
import { LogBookTypesEnum } from '@app/enums/log-book-types.enum';
import { LogBookStatusesEnum } from '@app/enums/log-book-statuses.enum';
import { ShipLogBookPageRegisterDTO } from '@app/models/generated/dtos/ShipLogBookPageRegisterDTO';
import { LogBookPageStatusesEnum } from '@app/enums/log-book-page-statuses.enum';
import { FirstSaleLogBookPageRegisterDTO } from '@app/models/generated/dtos/FirstSaleLogBookPageRegisterDTO';
import { AdmissionLogBookPageRegisterDTO } from '@app/models/generated/dtos/AdmissionLogBookPageRegisterDTO';
import { TransportationLogBookPageRegisterDTO } from '@app/models/generated/dtos/TransportationLogBookPageRegisterDTO';
import { AquacultureLogBookPageRegisterDTO } from '@app/models/generated/dtos/AquacultureLogBookPageRegisterDTO';
import { ValidityCheckerGroupDirective } from '@app/shared/directives/validity-checker/validity-checker-group.directive';
import { ICatchesAndSalesService } from '@app/interfaces/common-app/catches-and-sales.interface';
import { ILogBookService } from './interfaces/log-book.interface';
import { HttpErrorResponse } from '@angular/common/http';
import { ErrorCode, ErrorModel } from '@app/models/common/exception.model';
import { OverlappingLogBooksParameters } from '@app/shared/components/overlapping-log-books/models/overlapping-log-books-parameters.model';
import { OverlappingLogBooksDialogParamsModel } from '@app/shared/components/overlapping-log-books/models/overlapping-log-books-dialog-params.model';
import { OverlappingLogBooksComponent } from '@app/shared/components/overlapping-log-books/overlapping-log-books.component';
import { HeaderCloseFunction } from '@app/shared/components/dialog-wrapper/interfaces/header-cancel-button.interface';
import { TLMatDialog } from '@app/shared/components/dialog-wrapper/tl-mat-dialog';


@Component({
    selector: 'edit-log-book',
    templateUrl: './edit-log-book.component.html'
})
export class EditLogBookComponent implements OnInit, IDialogComponent {
    public readonly logBookGroupsEnum: typeof LogBookGroupsEnum = LogBookGroupsEnum;
    public readonly logBookTypesEnum: typeof LogBookTypesEnum = LogBookTypesEnum;
    public readonly logBookPagePersonTypesEnum: typeof LogBookPagePersonTypesEnum = LogBookPagePersonTypesEnum;
    public readonly pagesAndDeclarationsForLastPermitLicenseLabel: string;
    public readonly declarationsOfOriginInPermitLicenseLabel: string;
    public readonly declarationsOfOriginLabel: string;

    public form!: FormGroup;
    public logBookGroup!: LogBookGroupsEnum;
    public readOnly: boolean = false;
    public isAdd: boolean = false;
    public logBookTypes: NomenclatureDTO<number>[] = [];
    public allLogBookStatuses: NomenclatureDTO<number>[] = [];
    public logBookStatuses: NomenclatureDTO<number>[] = [];

    public isIdUndefined: boolean = false;
    public pagesRangeError: boolean = false;
    public isOnline: boolean = false;
    public isForRenewal: boolean = false;
    public ignoreLogBookConflicts: boolean = false;

    /**
     * Indicates whether the dialog is opened from a permit license entry
     * */
    public isForPermitLicense: boolean = false;
    /**
    * For when LogBookGroupsEnum = Ship (passed as parameter in setData method)
    **/
    public ownerType: LogBookPagePersonTypesEnum | undefined;
    public selectedLogBookType: LogBookTypesEnum | undefined;

    private model!: LogBookEditDTO | CommercialFishingLogBookEditDTO;
    private permitLicenseLogBookId: number | undefined;

    private readonly translate: FuseTranslationLoaderService;
    private readonly nomenclaturesService: CommonNomenclatures;
    private readonly overlappingLogBooksDialog: TLMatDialog<OverlappingLogBooksComponent>;
    private service: ILogBookService | undefined;

    @ViewChild(ValidityCheckerGroupDirective)
    private validityCheckerGroup!: ValidityCheckerGroupDirective;

    public constructor(
        nomenclaturesService: CommonNomenclatures,
        translate: FuseTranslationLoaderService,
        overlappingLogBooksDialog: TLMatDialog<OverlappingLogBooksComponent>
    ) {
        this.nomenclaturesService = nomenclaturesService;
        this.translate = translate;
        this.overlappingLogBooksDialog = overlappingLogBooksDialog;

        this.pagesAndDeclarationsForLastPermitLicenseLabel = this.translate.getValue('catches-and-sales.log-book-pages-and-declarations-for-last-permit-license-panel');
        this.declarationsOfOriginInPermitLicenseLabel = this.translate.getValue('catches-and-sales.log-book-declarations-of-origin-in-permit-license-panel');
        this.declarationsOfOriginLabel = this.translate.getValue('catches-and-sales.log-book-declarations-of-origin-panel');
    }

    public async ngOnInit(): Promise<void> {
        this.logBookTypes = await NomenclatureStore.instance.getNomenclature<number>(
            NomenclatureTypes.LogBookTypes, this.nomenclaturesService.getLogBookTypes.bind(this.nomenclaturesService), false
        ).toPromise();

        const logBookStatusesNomenclature = await NomenclatureStore.instance.getNomenclature<number>(
            NomenclatureTypes.LogBookStatuses, this.nomenclaturesService.getLogBookStatuses.bind(this.nomenclaturesService), false
        ).toPromise();

        this.allLogBookStatuses = this.deepCopyLogBookStatuses(logBookStatusesNomenclature);

        if (this.permitLicenseLogBookId !== null && this.permitLicenseLogBookId !== undefined) {
            if (this.service !== null && this.service !== undefined) {
                this.model = await this.service.getPermitLicenseLogBook(this.permitLicenseLogBookId).toPromise();
            }
            else {
                throw new Error("service is null. If permitLicenseLogBookId is passed to the edit log book dialog, a service property should be present as well.");
            }
        }

        if (this.model instanceof CommercialFishingLogBookEditDTO) {
            if (this.model.isForRenewal) {
                this.logBookStatuses = this.allLogBookStatuses.filter(x => LogBookStatusesEnum[x.code as keyof typeof LogBookStatusesEnum] !== LogBookStatusesEnum.New);
            }
        }

        switch (this.logBookGroup) {
            case LogBookGroupsEnum.Ship: {
                this.model.ownerType = this.ownerType;
                const permittedLogBookTypes: LogBookTypesEnum[] = [LogBookTypesEnum.Ship, LogBookTypesEnum.Admission, LogBookTypesEnum.Transportation];
                this.logBookTypes = this.logBookTypes.filter(x => permittedLogBookTypes.some(y => y === LogBookTypesEnum[x.code as keyof typeof LogBookTypesEnum]));
            } break;
            case LogBookGroupsEnum.Aquaculture: {
                this.model.ownerType = undefined;
                this.logBookTypes = this.logBookTypes.filter(x => LogBookTypesEnum[x.code as keyof typeof LogBookTypesEnum] === LogBookTypesEnum.Aquaculture);
            } break;
            case LogBookGroupsEnum.DeclarationsAndDocuments: {
                const permittedLogBookTypes: LogBookTypesEnum[] = [
                    LogBookTypesEnum.FirstSale,
                    LogBookTypesEnum.Admission,
                    LogBookTypesEnum.Transportation
                ];
                this.model.ownerType = LogBookPagePersonTypesEnum.RegisteredBuyer; // защото от интерфейсът ще се добавят дневници само за регистрирани купувачи/ЦПП
                this.logBookTypes = this.logBookTypes.filter(x => permittedLogBookTypes.some(y => y === LogBookTypesEnum[x.code as keyof typeof LogBookTypesEnum]) === true);
            } break;
        }

        if (this.model.logBookId === null || this.model.logBookId === undefined) {
            this.isIdUndefined = true;
        }

        this.filterLogBookStatuses();

        if (this.isAdd) {
            if (this.model instanceof CommercialFishingLogBookEditDTO) {
                this.form.get('startPageNumberControl')!.setValidators([TLValidators.number(0), Validators.required]);
                this.form.get('endPageNumberControl')!.setValidators([TLValidators.number(0), Validators.required]);

                this.form.get('startPageNumberControl')!.markAsPending();
                this.form.get('endPageNumberControl')!.markAsPending();

                this.form.get('permitLicenseStartPageNumberControl')!.setValidators([TLValidators.number(0), Validators.required]);
                this.form.get('permitLicenseEndPageNumberControl')!.setValidators([TLValidators.number(0), Validators.required]);

                this.form.get('permitLicenseStartPageNumberControl')!.markAsPending();
                this.form.get('permitLicenseEndPageNumberControl')!.markAsPending();
            }

            if (this.model.isOnline) {
                this.clearAndDisableStartPageControl();
                this.clearAndDisableEndPageControl();

                if (this.model instanceof CommercialFishingLogBookEditDTO) {
                    this.clearAndDisableLicenseStartPageControl();
                    this.clearAndDisableLicenseEndPageControl();
                }
            }
            else {
                if (this.model instanceof CommercialFishingLogBookEditDTO) {
                    this.form.get('permitLicenseStartPageNumberControl')!.disable();
                    this.form.get('permitLicenseEndPageNumberControl')!.disable();

                    this.form.get('startPageNumberControl')!.valueChanges.subscribe({
                        next: (value: string | undefined) => {
                            this.form.get('permitLicenseStartPageNumberControl')!.setValue(value);
                        }
                    });

                    this.form.get('endPageNumberControl')!.valueChanges.subscribe({
                        next: (value: string | undefined) => {
                            this.form.get('permitLicenseEndPageNumberControl')!.setValue(value);
                        }
                    });
                }
            }
        }
        else {
            if (this.model.isOnline && !this.readOnly) {
                this.clearAndDisableStartPageControl();
                this.clearAndDisableEndPageControl();
            }

            if (this.model instanceof CommercialFishingLogBookEditDTO && this.model.isForRenewal) {
                const logBookStatus: NomenclatureDTO<number> | undefined = this.form.get('statusControl')!.value;
                let isLogBookFinished: boolean = false;

                if (logBookStatus !== null && logBookStatus !== undefined) {
                    isLogBookFinished = LogBookStatusesEnum[logBookStatus.code as keyof typeof LogBookStatusesEnum] === LogBookStatusesEnum.Finished;
                }
                else {
                    isLogBookFinished = false;
                }

                this.setLogBookForRenewalValidators(isLogBookFinished);
            }
        }

        this.form.get('typeControl')!.valueChanges.subscribe({
            next: (value: NomenclatureDTO<number> | undefined | string) => {
                if (typeof value !== 'string') {
                    if (value === null || value === undefined) {
                        this.selectedLogBookType = undefined;
                    }
                    else {
                        this.selectedLogBookType = LogBookTypesEnum[value!.code as keyof typeof LogBookTypesEnum];

                        if (!this.readOnly) {
                            if (this.selectedLogBookType === LogBookTypesEnum.Ship && this.isOnline) {
                                this.clearAndDisableStartPageControl();
                                this.clearAndDisableEndPageControl();

                                if (this.isAdd) {
                                    this.form.get('isOnlineControl')!.setValue(true);

                                    if (this.model instanceof CommercialFishingLogBookEditDTO) {
                                        this.clearAndDisableLicenseStartPageControl();
                                        this.clearAndDisableLicenseEndPageControl();
                                    }
                                }
                            }
                            else {
                                this.form.get('startPageNumberControl')!.enable();
                                this.form.get('endPageNumberControl')!.enable();

                                if (this.model instanceof CommercialFishingLogBookEditDTO) {
                                    this.form.get('permitLicenseStartPageNumberControl')!.enable();
                                    this.form.get('permitLicenseEndPageNumberControl')!.enable();
                                }
                            }
                        }
                    }
                }
            }
        });

        if (!this.readOnly) {
            this.form.get('finishDateControl')!.valueChanges.subscribe({
                next: (value: Date | undefined) => {
                    if (value !== null && value !== undefined) {
                        this.form.get('statusControl')!.setValue(
                            this.allLogBookStatuses.find(x =>
                                LogBookStatusesEnum[x.code as keyof typeof LogBookStatusesEnum] === LogBookStatusesEnum.Finished
                            )
                        );
                    }
                }
            });

            this.form.get('statusControl')!.valueChanges.subscribe({
                next: (value: NomenclatureDTO<number> | undefined) => {
                    if (value !== null && value !== undefined) {
                        if (LogBookStatusesEnum[value.code as keyof typeof LogBookStatusesEnum] === LogBookStatusesEnum.Finished) { // Приключен дневник
                            this.form.get('finishDateControl')!.setValidators(Validators.required);
                            if (this.model instanceof CommercialFishingLogBookEditDTO) {
                                if (this.model.isForRenewal) {
                                    this.setLogBookForRenewalValidators(true);
                                }
                            }

                            this.setMissingOrInProgressPagesValidator();
                        }
                        else { // Неприключен дневник
                            if (this.model instanceof CommercialFishingLogBookEditDTO) {
                                if (this.model.isForRenewal) {
                                    this.setLogBookForRenewalValidators(false);
                                }
                            }

                            this.form.get('finishDateControl')!.clearValidators();
                            this.form.get('finishDateControl')!.reset();

                            this.clearMissingOrInProgressPagesValidator();
                        }
                    }
                    else {
                        this.form.get('finishDateControl')!.clearValidators();
                        this.form.get('finishDateControl')!.reset();
                    }

                    this.form.get('finishDateControl')!.markAsPending({ emitEvent: false });
                    this.form.get('finishDateControl')!.updateValueAndValidity({ emitEvent: false });
                }
            });
        }

        this.fillForm();
    }

    public setData(data: EditLogBookDialogParamsModel, buttons: DialogWrapperData): void {
        this.readOnly = data.readOnly;
        this.logBookGroup = data.logBookGroup;
        this.pagesRangeError = data.pagesRangeError;
        this.isOnline = data.isOnline ?? false;
        this.ownerType = data.ownerType;
        this.isForPermitLicense = data.isForPermitLicense;
        this.permitLicenseLogBookId = data.permitLicenseLogBookId;
        this.service = data.service;

        this.buildForm();

        if (this.permitLicenseLogBookId === null || this.permitLicenseLogBookId === undefined) {
            if (data.model === null || data.model === undefined) {
                this.isAdd = true;

                if (this.logBookGroup === LogBookGroupsEnum.Ship) {
                    this.model = new CommercialFishingLogBookEditDTO({
                        isActive: true,
                        permitLicenseIsActive: true,
                        isOnline: false
                    });
                }
                else {
                    this.model = new LogBookEditDTO({
                        isActive: true,
                        logBookIsActive: true,
                        isOnline: false
                    });
                }
            }
            else {
                this.isAdd = false;

                if (this.readOnly) {
                    this.form.disable();
                }

                this.model = data.model;

                if (this.model instanceof CommercialFishingLogBookEditDTO) {
                    this.isForRenewal = this.model.isForRenewal ?? false;
                }
            }
        }
        else {
            if (this.readOnly) {
                this.form.disable();
            }
        }
    }

    public saveBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        if (this.readOnly) {
            dialogClose(this.model);
            return;
        }

        this.form.markAllAsTouched();
        this.validityCheckerGroup.validate();

        if (this.form.valid) {
            this.fillModel();
            CommonUtils.sanitizeModelStrings(this.model);

            if (this.permitLicenseLogBookId !== null && this.permitLicenseLogBookId !== undefined) { // should save in DB before closing the dialog
                this.saveEditLogBook(dialogClose);
            }
            else {
                dialogClose(this.model);
            }
        }
    }

    private saveEditLogBook(dialogClose: DialogCloseCallback): void {
        this.service!.editLogBook(this.model, this.ignoreLogBookConflicts).subscribe({
            next: () => {
                dialogClose(this.model);
            },
            error: (errorResponse: HttpErrorResponse) => {
                const error = errorResponse.error as ErrorModel;
                if (error?.code === ErrorCode.InvalidLogBookLicensePagesRange
                    || error?.code === ErrorCode.InvalidLogBookPagesRange
                ) {
                    this.handleInvalidLogBookLicensePagesRangeError(error.messages[0], dialogClose);
                }
            }
        });
    }

    private handleInvalidLogBookLicensePagesRangeError(logBookNumber: string, dialogClose: DialogCloseCallback): void {
        if (this.model instanceof CommercialFishingLogBookEditDTO) {
            this.ignoreLogBookConflicts = false;

            const ranges: OverlappingLogBooksParameters[] = [
                new OverlappingLogBooksParameters({
                    logBookId: this.model.logBookId,
                    typeId: this.model.logBookTypeId,
                    OwnerType: this.model.ownerType,
                    startPage: this.model.permitLicenseStartPageNumber,
                    endPage: this.model.permitLicenseEndPageNumber
                })
            ];

            const editDialogData: OverlappingLogBooksDialogParamsModel = new OverlappingLogBooksDialogParamsModel({
                service: this.service,
                logBookGroup: this.logBookGroup,
                ranges: ranges
            });

            this.overlappingLogBooksDialog.open({
                title: this.translate.getValue('commercial-fishing.overlapping-log-books-dialog-title'),
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
                    translateValue: 'commercial-fishing.overlapping-log-books-save-despite-conflicts'
                }
            }, '1300px').subscribe({
                next: (save: boolean | undefined) => {
                    if (save) {
                        this.ignoreLogBookConflicts = true;
                        this.saveEditLogBook(dialogClose);
                    }
                }
            });
        }
    }

    public cancelBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public dialogButtonClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    private buildForm(): void {
        this.form = new FormGroup({
            typeControl: new FormControl(undefined, Validators.required),
            numberControl: new FormControl(),
            statusControl: new FormControl(undefined, Validators.required),
            issueDateControl: new FormControl(undefined, Validators.required),
            finishDateControl: new FormControl(),
            startPageNumberControl: new FormControl(undefined, [TLValidators.number(0)]),
            endPageNumberControl: new FormControl(undefined, [TLValidators.number(0)]),
            priceControl: new FormControl(undefined, [Validators.required, TLValidators.number(0)]),
            notesControl: new FormControl(undefined, Validators.maxLength(4000))
        }, [EditLogBookComponent.endPageGreaterThanStartPageValidator]);

        if (this.logBookGroup === LogBookGroupsEnum.Ship
            || this.ownerType === LogBookPagePersonTypesEnum.Person
            || this.ownerType === LogBookPagePersonTypesEnum.LegalPerson
        ) {
            this.form.addControl('isOnlineControl', new FormControl({ value: false, disabled: true }));

            this.form.addControl('permitLicenseStartPageNumberControl', new FormControl());
            this.form.addControl('permitLicenseEndPageNumberControl', new FormControl());

            this.form.addControl('permitLicenseRegistrationNumberControl', new FormControl(undefined));
            this.form.addControl('logBookLicenseValidFormControl', new FormControl(undefined));
            this.form.addControl('logBookLicenseValidToControl', new FormControl(undefined));

            this.form.setValidators([
                EditLogBookComponent.endPageGreaterThanStartPageValidator,
                this.permitLicensePageRangeValidator
            ]);
        }
    }

    private fillForm(): void {
        if (this.model.logBookTypeId !== null && this.model.logBookTypeId !== undefined) {
            const logBookType: NomenclatureDTO<number> = this.logBookTypes.find(x => x.value === this.model.logBookTypeId)!;
            this.form.get('typeControl')!.setValue(logBookType);
        }

        if (this.model.statusId !== null && this.model.statusId !== undefined) {
            const logBookStatus: NomenclatureDTO<number> = this.allLogBookStatuses.find(x => x.value === this.model.statusId)!;
            this.form.get('statusControl')!.setValue(logBookStatus);
        }

        this.form.get('numberControl')!.setValue(this.model.logbookNumber);
        this.form.get('startPageNumberControl')!.setValue(this.model.startPageNumber);
        this.form.get('endPageNumberControl')!.setValue(this.model.endPageNumber);
        this.form.get('priceControl')!.setValue(this.model.price);
        this.form.get('notesControl')!.setValue(this.model.comment);

        if (this.model instanceof CommercialFishingLogBookEditDTO) {
            this.form.get('isOnlineControl')!.setValue(this.model.isOnline);

            if (this.model.permitLicenseStartPageNumber !== null && this.model.permitLicenseStartPageNumber !== undefined) {
                this.form.get('permitLicenseStartPageNumberControl')!.setValue(this.model.permitLicenseStartPageNumber);
            }

            if (this.model.permitLicenseEndPageNumber !== null && this.model.permitLicenseEndPageNumber !== undefined) {
                this.form.get('permitLicenseEndPageNumberControl')!.setValue(this.model.permitLicenseEndPageNumber);
            }

            this.form.get('permitLicenseRegistrationNumberControl')!.setValue(this.model.permitLicenseRegistrationNumber);
            this.form.get('logBookLicenseValidFormControl')!.setValue(this.model.logBookLicenseValidForm);
            this.form.get('logBookLicenseValidToControl')!.setValue(this.model.logBookLicenseValidTo);
        }

        if (this.isAdd) {
            this.form.get('issueDateControl')!.setValue(new Date());

            const status: NomenclatureDTO<number> | undefined = this.allLogBookStatuses.find(x => LogBookStatusesEnum[x.code as keyof typeof LogBookStatusesEnum] === LogBookStatusesEnum.New);
            this.form.get('statusControl')!.setValue(status);
            this.form.get('statusControl')!.disable();
        }
        else {
            this.form.get('issueDateControl')!.setValue(this.model.issueDate);
            this.form.get('finishDateControl')!.setValue(this.model.finishDate);
        }

        if (this.isAdd && this.model.isOnline && this.selectedLogBookType === LogBookTypesEnum.Ship) {
            this.form.get('isOnlineControl')!.setValue(true);
        }
    }

    private fillModel(): void {
        this.model.logBookTypeId = this.form.get('typeControl')!.value?.value;
        const logBookTypeCode: string = this.logBookTypes.find(x => x.value === this.model.logBookTypeId)!.code!;

        if (logBookTypeCode === LogBookTypesEnum[LogBookTypesEnum.Ship]) {
            this.model.ownerType = undefined;
        }

        this.model.statusId = this.form.get('statusControl')!.value?.value;
        this.model.issueDate = this.form.get('issueDateControl')!.value;
        this.model.startPageNumber = this.form.get('startPageNumberControl')!.value;
        this.model.endPageNumber = this.form.get('endPageNumberControl')!.value;
        this.model.price = this.form.get('priceControl')!.value as number;
        this.model.comment = this.form.get('notesControl')!.value;

        if (this.model instanceof CommercialFishingLogBookEditDTO) {
            this.model.permitLicenseStartPageNumber = this.form.get('permitLicenseStartPageNumberControl')!.value;
            this.model.permitLicenseEndPageNumber = this.form.get('permitLicenseEndPageNumberControl')!.value;
            this.model.isOnline = this.form.get('isOnlineControl')!.value;
        }

        if (!this.isAdd) {
            this.model.finishDate = this.form.get('finishDateControl')!.value;
        }
    }

    private filterLogBookStatuses(): void {
        this.logBookStatuses = this.allLogBookStatuses.slice();

        let logBookStatusesToDelete: NomenclatureDTO<number>[];

        if (this.isAdd) {
            logBookStatusesToDelete = this.logBookStatuses.filter(x => x.code !== LogBookStatusesEnum[LogBookStatusesEnum.New]);
        }
        else if (this.model instanceof CommercialFishingLogBookEditDTO && this.model.isForRenewal) {
            logBookStatusesToDelete = this.logBookStatuses.filter(x => x.code === LogBookStatusesEnum[LogBookStatusesEnum.New]);
        }
        else {
            logBookStatusesToDelete = this.logBookStatuses.filter(x => x.code === LogBookStatusesEnum[LogBookStatusesEnum.Renewed]);
        }

        for (const logBookStatus of logBookStatusesToDelete) {
            logBookStatus.isActive = false;
        }
    }

    private clearAndDisableStartPageControl(): void {
        this.form.get('startPageNumberControl')!.setValue(null);
        this.form.get('startPageNumberControl')!.clearValidators();
        this.form.get('startPageNumberControl')!.disable();
        this.form.get('startPageNumberControl')!.markAsPending();
        this.form.get('startPageNumberControl')!.updateValueAndValidity({ emitEvent: false });
    }

    private clearAndDisableEndPageControl(): void {
        this.form.get('endPageNumberControl')!.setValue(null);
        this.form.get('endPageNumberControl')!.clearValidators();
        this.form.get('endPageNumberControl')!.disable();
        this.form.get('endPageNumberControl')!.markAsPending();
        this.form.get('endPageNumberControl')!.updateValueAndValidity({ emitEvent: false });
    }

    private clearAndDisableLicenseStartPageControl(): void {
        this.form.get('permitLicenseStartPageNumberControl')!.setValue(null);
        this.form.get('permitLicenseStartPageNumberControl')!.clearValidators();
        this.form.get('permitLicenseStartPageNumberControl')!.disable();
        this.form.get('permitLicenseStartPageNumberControl')!.markAsPending();
        this.form.get('permitLicenseStartPageNumberControl')!.updateValueAndValidity({ emitEvent: false });
    }

    private clearAndDisableLicenseEndPageControl(): void {
        this.form.get('permitLicenseEndPageNumberControl')!.setValue(null);
        this.form.get('permitLicenseEndPageNumberControl')!.clearValidators();
        this.form.get('permitLicenseEndPageNumberControl')!.disable();
        this.form.get('permitLicenseEndPageNumberControl')!.markAsPending();
        this.form.get('permitLicenseEndPageNumberControl')!.updateValueAndValidity({ emitEvent: false });
    }

    private setLogBookForRenewalValidators(isLogBookFishined: boolean): void {
        if (this.model instanceof CommercialFishingLogBookEditDTO && this.model.isForRenewal) {
            if (isLogBookFishined) {
                this.form.get('permitLicenseStartPageNumberControl')!.clearValidators();
                this.form.get('permitLicenseEndPageNumberControl')!.clearValidators();

                this.form.get('permitLicenseStartPageNumberControl')!.markAsPending();
                this.form.get('permitLicenseEndPageNumberControl')!.markAsPending();

                this.form.get('permitLicenseStartPageNumberControl')!.disable();
                this.form.get('permitLicenseEndPageNumberControl')!.disable();

                this.form.get('permitLicenseStartPageNumberControl')!.updateValueAndValidity();
                this.form.get('permitLicenseEndPageNumberControl')!.updateValueAndValidity();

                // set validator that will check for missing and in progress pages SO the form will be invalid if there are missing or in progress pages
                this.setMissingOrInProgressPagesValidator();
            }
            else {
                if (!this.readOnly) {
                    this.form.get('permitLicenseStartPageNumberControl')!.enable();
                    this.form.get('permitLicenseEndPageNumberControl')!.enable();
                }

                if (this.model.lastPageNumber !== null && this.model.lastPageNumber !== undefined) {
                    let permitLicenseStartPageProposal: number;
                    if (this.model.lastPageNumber > 0) {
                        permitLicenseStartPageProposal = this.model.lastPageNumber + 1;
                    }
                    else {
                        permitLicenseStartPageProposal = this.model.startPageNumber!;
                    }

                    if (permitLicenseStartPageProposal > this.model.endPageNumber!) {
                        permitLicenseStartPageProposal = this.model.endPageNumber!;
                    }

                    this.form.get('permitLicenseStartPageNumberControl')!.setValue(permitLicenseStartPageProposal);
                    this.form.get('permitLicenseStartPageNumberControl')!.setValidators([
                        Validators.required,
                        TLValidators.number(permitLicenseStartPageProposal)
                    ]);

                    this.form.get('permitLicenseEndPageNumberControl')!.setValidators([TLValidators.number(0), Validators.required]);
                    this.form.get('permitLicenseEndPageNumberControl')!.setValue(this.model.endPageNumber);

                    this.form.get('permitLicenseStartPageNumberControl')!.markAsPending();
                    this.form.get('permitLicenseEndPageNumberControl')!.markAsPending();

                    this.form.get('permitLicenseStartPageNumberControl')!.updateValueAndValidity({ emitEvent: false });
                    this.form.get('permitLicenseEndPageNumberControl')!.updateValueAndValidity({ emitEvent: false });

                    this.form.get('startPageNumberControl')!.setValidators([TLValidators.number(0), Validators.required]);
                    this.form.get('endPageNumberControl')!.setValidators([TLValidators.number(0), Validators.required]);

                    this.form.get('startPageNumberControl')!.markAsPending();
                    this.form.get('endPageNumberControl')!.markAsPending();

                    this.form.get('startPageNumberControl')!.updateValueAndValidity({ emitEvent: false });
                    this.form.get('endPageNumberControl')!.updateValueAndValidity({ emitEvent: false });
                }
            }
        }
    }

    private setMissingOrInProgressPagesValidator(): void {
        const formValidators: ValidatorFn | null = this.form.validator;

        if (formValidators !== null && formValidators !== undefined) {
            this.form.setValidators([formValidators, this.logBookHasMissingOrInProgressPagesValidator()]);
        }
        else {
            this.form.setValidators(this.logBookHasMissingOrInProgressPagesValidator());
        }

        this.form.updateValueAndValidity({ emitEvent: false });
    }

    /**
     * Calls `clearValidators` function inside.
     * Cannot remore dinamically one 1 validator ... must add there other validators, if there were any except the `logBookHasMissingPagesValidator`
     * */
    private clearMissingOrInProgressPagesValidator(): void {
        this.form.clearValidators();
        this.form.updateValueAndValidity({ emitEvent: false });
    }

    private static endPageGreaterThanStartPageValidator: ValidatorFn = (form: AbstractControl): ValidationErrors | null => {
        if (!form) {
            return null;
        }

        const startPageNumberControl = form.get('startPageNumberControl');
        const endPageNumberControl = form.get('endPageNumberControl');

        if (!startPageNumberControl || !endPageNumberControl) {
            return null;
        }

        if (startPageNumberControl.value === null
            || startPageNumberControl.value === undefined
            || endPageNumberControl.value === null
            || endPageNumberControl.value === undefined
        ) {
            return null
        }

        const startPageNumber = Number(startPageNumberControl.value);
        const endPageNumber = Number(endPageNumberControl.value);

        if (startPageNumber <= endPageNumber) {
            return null;
        }

        return { endPageGreaterThanStartPage: true };
    };

    private permitLicensePageRangeValidator(): ValidatorFn {
        return (form: AbstractControl): ValidationErrors | null => {
            if (!form) {
                return null;
            }

            const logBookStatusControl = form.get('statusControl');
            const logBookStatus: NomenclatureDTO<number> | undefined = logBookStatusControl?.value;
            if (logBookStatus !== null
                && logBookStatus !== undefined
                && LogBookStatusesEnum[logBookStatus.code as keyof typeof LogBookStatusesEnum] === LogBookStatusesEnum.Finished
                && this.model instanceof CommercialFishingLogBookEditDTO
                && this.model?.isForRenewal
            ) {
                return null;
            }

            const startPageNumberControl = form.get('startPageNumberControl');
            const endPageNumberControl = form.get('endPageNumberControl');

            const permitLicenseStartPageNumberControl = form.get('permitLicenseStartPageNumberControl');
            const permitLicenseEndPageNumberControl = form.get('permitLicenseEndPageNumberControl');

            if (!startPageNumberControl || !endPageNumberControl || !permitLicenseStartPageNumberControl || !permitLicenseEndPageNumberControl) {
                return null;
            }

            if (startPageNumberControl.value === null
                || startPageNumberControl.value === undefined
                || endPageNumberControl.value === null
                || endPageNumberControl.value === undefined
            ) {
                return null;
            }

            if (permitLicenseStartPageNumberControl.value === null
                || permitLicenseStartPageNumberControl.value === undefined
                || permitLicenseEndPageNumberControl.value === null
                || permitLicenseEndPageNumberControl.value === undefined
            ) {
                return null;
            }

            const startPageNumber = Number(startPageNumberControl.value);
            const endPageNumber = Number(endPageNumberControl.value);
            const permitLicenseStartPageNumber = Number(permitLicenseStartPageNumberControl.value);
            const permitLicenseEndPageNumber = Number(permitLicenseEndPageNumberControl.value);

            if (permitLicenseStartPageNumber >= startPageNumber && permitLicenseEndPageNumber <= endPageNumber) {
                return null;
            }

            return { permitLicensePageRangeInvalid: true };
        };
    }

    private logBookHasMissingOrInProgressPagesValidator(): ValidatorFn {
        return (control: AbstractControl): ValidationErrors | null => {
            if (control === null || control === undefined) {
                return null;
            }

            let hasMissingOrInProgressPages: boolean = false;
            const logBookTypeCode: string = this.form.get('typeControl')!.value?.code;

            if (logBookTypeCode == null || logBookTypeCode === undefined) {
                return null;
            }

            switch (LogBookTypesEnum[logBookTypeCode as keyof typeof LogBookTypesEnum]) {
                case LogBookTypesEnum.Ship: {
                    const pages: ShipLogBookPageRegisterDTO[] = (this.model as CommercialFishingLogBookEditDTO).shipPagesAndDeclarations ?? [];
                    if (pages.some(x => x.status === LogBookPageStatusesEnum.Missing || x.status === LogBookPageStatusesEnum.InProgress)) {
                        hasMissingOrInProgressPages = true;
                    }
                } break;
                case LogBookTypesEnum.FirstSale: {
                    const pages: FirstSaleLogBookPageRegisterDTO[] = this.model.firstSalePages ?? [];
                    if (pages.some(x => x.status === LogBookPageStatusesEnum.Missing || x.status === LogBookPageStatusesEnum.InProgress)) {
                        hasMissingOrInProgressPages = true;
                    }
                } break;
                case LogBookTypesEnum.Admission: {
                    const pages: AdmissionLogBookPageRegisterDTO[] = this.model.admissionPagesAndDeclarations ?? [];
                    if (pages.some(x => x.status === LogBookPageStatusesEnum.Missing || x.status === LogBookPageStatusesEnum.InProgress)) {
                        hasMissingOrInProgressPages = true;
                    }
                } break;
                case LogBookTypesEnum.Transportation: {
                    const pages: TransportationLogBookPageRegisterDTO[] = this.model.transportationPagesAndDeclarations ?? [];
                    if (pages.some(x => x.status === LogBookPageStatusesEnum.Missing || x.status === LogBookPageStatusesEnum.InProgress)) {
                        hasMissingOrInProgressPages = true;
                    }
                } break;
                case LogBookTypesEnum.Aquaculture: {
                    const pages: AquacultureLogBookPageRegisterDTO[] = this.model.aquaculturePages ?? [];
                    if (pages.some(x => x.status === LogBookPageStatusesEnum.Missing || x.status === LogBookPageStatusesEnum.InProgress)) {
                        hasMissingOrInProgressPages = true;
                    }
                } break;
            }

            if (hasMissingOrInProgressPages) {
                return { 'hasMissingOrInProgressPages': true };
            }

            return null;
        }
    }

    // helpers

    private deepCopyLogBookStatuses(statuses: NomenclatureDTO<number>[]): NomenclatureDTO<number>[] {
        if (statuses !== null && statuses !== undefined) {
            const copiedStatuses: NomenclatureDTO<number>[] = [];

            for (const status of statuses) {
                const stringified: string = JSON.stringify(status);
                const newStatus: NomenclatureDTO<number> = new NomenclatureDTO<number>(JSON.parse(stringified));

                copiedStatuses.push(newStatus);
            }

            return copiedStatuses;
        }
        else {
            return [];
        }
    }
}