import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { forkJoin, Observable } from 'rxjs';
import { HttpErrorResponse } from '@angular/common/http';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Moment } from 'moment';
import moment from 'moment';
import { DatePipe } from '@angular/common';

import { EditOriginDeclarationComponent } from '../edit-origin-declaration/edit-origin-declaration.component';
import { OriginDeclarationDialogParamsModel } from '../../models/origin-declaration-dialog-params.model';
import { EditCatchRecordComponent } from '../catch-record/edit-catch-record.component';
import { CatchRecordDialogParamsModel } from '../../models/catch-record-dialog-params.model';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { ICatchesAndSalesService } from '@app/interfaces/common-app/catches-and-sales.interface';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { ShipLogBookPageEditDTO } from '@app/models/generated/dtos/ShipLogBookPageEditDTO';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { PageCodeEnum } from '@app/enums/page-code.enum';
import { TLDataTableComponent } from '@app/shared/components/data-table/tl-data-table.component';
import { CatchRecordDTO } from '@app/models/generated/dtos/CatchRecordDTO';
import { GridRow } from '@app/shared/components/data-table/models/row.model';
import { TLMatDialog } from '@app/shared/components/dialog-wrapper/tl-mat-dialog';
import { HeaderCloseFunction } from '@app/shared/components/dialog-wrapper/interfaces/header-cancel-button.interface';
import { IS_PUBLIC_APP } from '@app/shared/modules/application.modules';
import { IHeaderAuditButton } from '@app/shared/components/dialog-wrapper/interfaces/header-audit-button.interface';
import { CatchRecordFishDTO } from '@app/models/generated/dtos/CatchRecordFishDTO';
import { OriginDeclarationFishDTO } from '@app/models/generated/dtos/OriginDeclarationFishDTO';
import { CommonNomenclatures } from '@app/services/common-app/common-nomenclatures.service';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { TLConfirmDialog } from '@app/shared/components/confirmation-dialog/tl-confirm-dialog';
import { TLValidators } from '@app/shared/utils/tl-validators';
import { ShipNomenclatureDTO } from '@app/models/generated/dtos/ShipNomenclatureDTO';
import { DateUtils } from '@app/shared/utils/date.utils';
import { DateDifference } from '@app/models/common/date-difference.model';
import { TLDateDifferencePipe } from '@app/shared/pipes/tl-date-difference.pipe';
import { LogBookPageStatusesEnum } from '@app/enums/log-book-page-statuses.enum';
import { EditShipLogBookPageDialogParams } from './models/edit-ship-log-book-page-dialog-params.model';
import { ErrorCode, ErrorModel } from '@app/models/common/exception.model';
import { RequestProperties } from '@app/shared/services/request-properties';
import { FishingGearRegisterNomenclatureDTO } from '@app/models/generated/dtos/FishingGearRegisterNomenclatureDTO';
import { ValidityCheckerGroupDirective } from '@app/shared/directives/validity-checker/validity-checker-group.directive';
import { FishNomenclatureDTO } from '@app/models/generated/dtos/FishNomenclatureDTO';
import { FishPresentationCodesEnum } from '@app/enums/fish-presentation-codes.enum';
import { FishCatchStateCodesEnum } from '@app/enums/fish-catch-state-codes.enum';
import { TLError } from '@app/shared/components/input-controls/models/tl-error.model';
import { PreviousTripsCatchRecordsComponent } from '../previous-trips-catch-records/previous-trips-catch-records.component';
import { PreviousTripsCatchRecordsDialogParams } from '../previous-trips-catch-records/models/previous-trips-catch-records-dialog-params.model';
import { OnBoardCatchRecordFishDTO } from '@app/models/generated/dtos/OnBoardCatchRecordFishDTO';
import { CatchZoneNomenclatureDTO } from '@app/models/generated/dtos/CatchZoneNomenclatureDTO';
import { ShipsUtils } from '@app/shared/utils/ships.utils';

const PERCENT_TOLERANCE: number = 10;
const QUALITY_DIFF_VALIDATOR_NAME: string = 'quantityDifferences';
export const DEFAULT_PRESENTATION_CODE: FishPresentationCodesEnum = FishPresentationCodesEnum.WHL;
export const DEFAULT_CATCH_STATE_CODE: FishCatchStateCodesEnum = FishCatchStateCodesEnum.E;

@Component({
    selector: 'edit-ship-log-book-page',
    templateUrl: './edit-ship-log-book-page.component.html'
})
export class EditShipLogBookPageComponent implements OnInit, AfterViewInit, IDialogComponent {
    public readonly today: Date = new Date();
    public readonly pageCode: PageCodeEnum = PageCodeEnum.ShipLogBookPage;
    public readonly icIconSize: number = CommonUtils.IC_ICON_SIZE;

    public form!: FormGroup;
    public viewMode!: boolean;
    public model!: ShipLogBookPageEditDTO;
    public service!: ICatchesAndSalesService;
    public catchRecords: CatchRecordDTO[] = [];

    public declarationOfOriginCatchRecords: OriginDeclarationFishDTO[] = [];

    public ships: ShipNomenclatureDTO[] = [];
    public fishingGearsRegister: FishingGearRegisterNomenclatureDTO[] = [];
    public ports: NomenclatureDTO<number>[] = [];
    public aquaticOrganisms: FishNomenclatureDTO[] = [];
    public catchStates: NomenclatureDTO<number>[] = [];
    public catchPresentations: NomenclatureDTO<number>[] = [];
    public catchZones: CatchZoneNomenclatureDTO[] = [];

    public showHooksCountField: boolean = false;
    public showPartnerShipField: boolean = false;

    public isEditing: boolean = false;
    public isAdd: boolean = false;

    @ViewChild(ValidityCheckerGroupDirective)
    private validityCheckerGroup!: ValidityCheckerGroupDirective;

    @ViewChild('catchRecordsTable')
    private catchRecordsTable!: TLDataTableComponent;

    @ViewChild('declarationOfOriginCatchRecordsTable')
    private declarationOfOriginCatchRecordsTable!: TLDataTableComponent;

    private id: number | undefined;
    private translationService: FuseTranslationLoaderService;

    private editCatchRecordDialog: TLMatDialog<EditCatchRecordComponent>;
    private editDeclarationOfOriginDialog: TLMatDialog<EditOriginDeclarationComponent>;
    private previousTripCatchRecordsDialog: TLMatDialog<PreviousTripsCatchRecordsComponent>;
    private confirmDialog: TLConfirmDialog;
    private snackbar: MatSnackBar;

    private commonNomenclaturesService: CommonNomenclatures;
    private dateDifferencePipe: TLDateDifferencePipe;
    private datePipe: DatePipe;
    private selectedCatchesFromPreviousTrips: OnBoardCatchRecordFishDTO[] = [];

    public constructor(
        translate: FuseTranslationLoaderService,
        editCatchRecordDialog: TLMatDialog<EditCatchRecordComponent>,
        editDeclarationOfOriginDialog: TLMatDialog<EditOriginDeclarationComponent>,
        previousTripCatchRecordsDialog: TLMatDialog<PreviousTripsCatchRecordsComponent>,
        commonNomenclaturesService: CommonNomenclatures,
        confirmDialog: TLConfirmDialog,
        snackbar: MatSnackBar,
        dateDifferencePipe: TLDateDifferencePipe,
        datePipe: DatePipe
    ) {
        this.translationService = translate;
        this.commonNomenclaturesService = commonNomenclaturesService;
        this.snackbar = snackbar;
        this.dateDifferencePipe = dateDifferencePipe;
        this.datePipe = datePipe;

        this.editCatchRecordDialog = editCatchRecordDialog;
        this.editDeclarationOfOriginDialog = editDeclarationOfOriginDialog;
        this.confirmDialog = confirmDialog;
        this.previousTripCatchRecordsDialog = previousTripCatchRecordsDialog;
    }

    public async ngOnInit(): Promise<void> {
        const nomenclatures: (NomenclatureDTO<number> | ShipNomenclatureDTO | FishingGearRegisterNomenclatureDTO | FishNomenclatureDTO)[][] = await forkJoin(
            NomenclatureStore.instance.getNomenclature(
                NomenclatureTypes.Ships, this.commonNomenclaturesService.getShips.bind(this.commonNomenclaturesService), false
            ),
            NomenclatureStore.instance.getNomenclature(
                NomenclatureTypes.Ports, this.commonNomenclaturesService.getPorts.bind(this.commonNomenclaturesService), false
            ),
            NomenclatureStore.instance.getNomenclature(
                NomenclatureTypes.Fishes, this.commonNomenclaturesService.getFishTypes.bind(this.commonNomenclaturesService), false
            ),
            NomenclatureStore.instance.getNomenclature<number>(
                NomenclatureTypes.CatchStates, this.service.getCatchStates.bind(this.service), false),
            NomenclatureStore.instance.getNomenclature<number>(
                NomenclatureTypes.CatchPresentations, this.commonNomenclaturesService.getCatchPresentations.bind(this.commonNomenclaturesService), false),
            NomenclatureStore.instance.getNomenclature(
                NomenclatureTypes.CatchZones, this.commonNomenclaturesService.getCatchZones.bind(this.commonNomenclaturesService), false)
        ).toPromise();

        this.ships = nomenclatures[0] as ShipNomenclatureDTO[];
        this.ports = nomenclatures[1];
        this.aquaticOrganisms = nomenclatures[2] as FishNomenclatureDTO[];
        this.catchStates = nomenclatures[3] as NomenclatureDTO<number>[];
        this.catchPresentations = nomenclatures[4] as NomenclatureDTO<number>[];
        this.catchZones = nomenclatures[5] as CatchZoneNomenclatureDTO[];

        if (this.id !== null && this.id !== undefined) {
            this.isAdd = false;
            this.isEditing = true;

            this.service.getShipLogBookPage(this.id).subscribe({
                next: async (value: ShipLogBookPageEditDTO) => {
                    this.model = value;
                    this.model.status = this.service.getPageStatusTranslation(this.model.statusCode!);

                    this.fishingGearsRegister = await this.service.getFishingGearsRegister(this.model.permitLicenseId!).toPromise();

                    this.fillForm();
                }
            });
        }
        else {
            this.isAdd = true;
            this.fishingGearsRegister = await this.service.getFishingGearsRegister(this.model.permitLicenseId!).toPromise();
            this.fillForm();
        }
    }

    public ngAfterViewInit(): void {
        this.form.get('fishingGearRegisterControl')!.valueChanges.subscribe({
            next: (value: FishingGearRegisterNomenclatureDTO | string | undefined) => {
                if (value instanceof FishingGearRegisterNomenclatureDTO) {
                    const fishingGearsCount: number | undefined = this.form.get('fishingGearCountControl')!.value;
                    if (fishingGearsCount === null || fishingGearsCount === undefined) {
                        this.form.get('fishingGearCountControl')!.setValue(value.gearCount);
                    }

                    if (value.hasHooks === true) {
                        this.showHooksCountField = true;
                        this.form.get('fishingGearHookCountControl')!.setValidators([Validators.required, TLValidators.number(0)]);
                        this.form.get('fishingGearHookCountControl')!.markAsPending();
                        this.form.get('fishingGearHookCountControl')!.setValue(value.hooksCount);

                        if (this.viewMode) {
                            this.form.get('fishingGearHookCountControl')!.disable();
                        }
                    }
                    else {
                        this.showHooksCountField = false;
                        this.form.get('fishingGearHookCountControl')!.clearValidators();
                        this.form.get('fishingGearHookCountControl')!.reset();
                    }

                    if (value.isForMutualFishing === true) {
                        this.showPartnerShipField = true;
                        this.form.get('partnerShipControl')!.setValidators(Validators.required);
                        this.form.get('partnerShipControl')!.markAsPending();

                        if (this.viewMode) {
                            this.form.get('partnerShipControl')!.disable();
                        }
                    }
                    else {
                        this.showPartnerShipField = false;
                        this.form.get('partnerShipControl')!.clearValidators();
                    }
                }
                else {
                    if (value === null || value === undefined) {
                        this.form.get('fishingGearCountControl')!.reset();
                        this.form.get('fishingGearHookCountControl')!.reset();
                    }

                    this.showHooksCountField = false;
                    this.showPartnerShipField = false;
                }
            }
        });

        this.form.get('fishTripStartDateTimeControl')!.valueChanges.subscribe({
            next: (startDate: Moment | null | undefined) => {
                if (startDate !== null && startDate !== undefined) {
                    const endDate: Moment | null | undefined = this.form.get('fishTripEndDateTimeControl')!.value;
                    if (endDate !== null && endDate !== undefined) {
                        const difference: DateDifference | undefined = DateUtils.getDateDifference(startDate.toDate(), endDate.toDate());
                        const daysAtSeaValue: string = this.getDaysAtSeaValue(difference);

                        this.form.get('daysAtSeaCountControl')!.setValue(daysAtSeaValue);
                    }
                    else {
                        this.form.get('daysAtSeaCountControl')!.setValue(undefined);
                    }
                }
                else {
                    this.form.get('daysAtSeaCountControl')!.setValue(undefined);
                }
            }
        });

        this.form.get('fishTripEndDateTimeControl')!.valueChanges.subscribe({
            next: (endDate: Moment | null | undefined) => {
                if (endDate !== null && endDate !== undefined) {
                    const startDate: Moment | null | undefined = this.form.get('fishTripStartDateTimeControl')!.value;
                    if (startDate !== null && startDate !== undefined) {
                        const difference: DateDifference | undefined = DateUtils.getDateDifference(startDate.toDate(), endDate.toDate());
                        const daysAtSeaValue: string = this.getDaysAtSeaValue(difference);

                        this.form.get('daysAtSeaCountControl')!.setValue(daysAtSeaValue);
                    }
                    else {
                        this.form.get('daysAtSeaCountControl')!.setValue(undefined);
                    }
                }
                else {
                    this.form.get('daysAtSeaCountControl')!.setValue(undefined);
                }
            }
        });
    }

    public setData(data: EditShipLogBookPageDialogParams, buttons: DialogWrapperData): void {
        if (data.id !== undefined && data.id !== null) {
            this.id = data.id;
        }
        else if (data.model !== undefined && data.model !== null) {
            this.model = data.model;
        }

        this.service = data.service as ICatchesAndSalesService;
        this.viewMode = data.viewMode;

        this.buildForm();

        if (this.viewMode === true) {
            this.form.disable();
        }
    }

    public saveBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        this.form.markAllAsTouched();
        this.validityCheckerGroup.validate();

        if (this.isFormValid()) {
            this.model = this.fillModel();
            this.model = CommonUtils.sanitizeModelStrings(this.model);

            if (this.id === null || this.id === undefined) {
                this.service.addShipLogBookPage(this.model).subscribe({
                    next: (id: number) => {
                        this.model.id = id;
                        dialogClose(this.model);
                    },
                    error: (response: HttpErrorResponse) => {
                        const error: ErrorModel | undefined = response.error;

                        if (error?.code === ErrorCode.PageNumberNotInLogbook) {
                            this.snackbar.open(this.translationService.getValue('catches-and-sales.ship-log-book-page-not-in-range-error'), undefined, {
                                duration: RequestProperties.DEFAULT.showExceptionDurationErr,
                                panelClass: RequestProperties.DEFAULT.showExceptionColorClassErr
                            });
                        }
                        else if (error?.code === ErrorCode.PageNumberNotInLogBookLicense) {
                            this.snackbar.open(
                                this.translationService.getValue('catches-and-sales.ship-log-book-page-not-in-log-book-license-range-error'),
                                undefined,
                                {
                                    duration: RequestProperties.DEFAULT.showExceptionDurationErr,
                                    panelClass: RequestProperties.DEFAULT.showExceptionColorClassErr
                                });
                        }
                        else if (error?.code === ErrorCode.LogBookPageAlreadySubmitted) {
                            this.snackbar.open(this.translationService.getValue('catches-and-sales.ship-log-book-page-already-submitted-error'), undefined, {
                                duration: RequestProperties.DEFAULT.showExceptionDurationErr,
                                panelClass: RequestProperties.DEFAULT.showExceptionColorClassErr
                            });
                        }
                    }
                });
            }
            else {
                this.service.editShipLogBookPage(this.model).subscribe({
                    next: () => {
                        dialogClose(this.model);
                    }
                });
            }
        }
    }

    private isFormValid(): boolean {
        let isValid: boolean = this.form.valid;

        if (isValid === false) {
            const innerErrors: Record<string, unknown> = {};
            for (const controlName in this.form.controls) {
                if (this.form.get(controlName)!.errors !== null && this.form.get(controlName)!.errors !== undefined) {
                    for (const key in this.form.get(controlName)!.errors) {
                        innerErrors[key] = this.form.get(controlName)!.errors![key];
                    }
                }
            }

            const innerErrorKeys = Object.keys(innerErrors);
            if (innerErrorKeys.length === 0) {
                if (this.form.errors !== null && this.form.errors !== undefined) {
                    const errorKeys = Object.keys(this.form.errors);

                    if (errorKeys.length === 1 && errorKeys[0] === QUALITY_DIFF_VALIDATOR_NAME) {
                        isValid = true;
                    }
                }
            }
        }

        return isValid;
    }

    public cancelBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public dialogButtonClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public addEditCatchRecord(catchRecord?: CatchRecordDTO, viewMode: boolean = false): void {
        let title: string = '';
        let headerAuditBtn: IHeaderAuditButton | undefined = undefined;
        const data: CatchRecordDialogParamsModel = new CatchRecordDialogParamsModel({
            model: catchRecord ? this.copyCatchRecord(catchRecord) : undefined,
            service: this.service,
            viewMode: viewMode,
            waterType: this.model.permitLicenseWaterType!
        });

        if (catchRecord === undefined) {
            title = this.translationService.getValue('catches-and-sales.add-ship-page-catch-record');
        }
        else if (viewMode === true) {
            title = this.translationService.getValue('catches-and-sales.view-ship-page-catch-record');
        }
        else {
            title = this.translationService.getValue('catches-and-sales.edit-ship-page-catch-record');
        }

        if (!IS_PUBLIC_APP && catchRecord !== undefined) {
            headerAuditBtn = {
                id: catchRecord.id!,
                tableName: 'CatchRecord',
                tooltip: '',
                getAuditRecordData: this.service.getCatchRecordSimpleAudit.bind(this.service)
            };
        }

        this.editCatchRecordDialog.openWithTwoButtons({
            title: title,
            TCtor: EditCatchRecordComponent,
            translteService: this.translationService,
            headerCancelButton: {
                cancelBtnClicked: this.closeEditCatchRecordDialogBtnClicked.bind(this)
            },
            headerAuditButton: headerAuditBtn,
            componentData: data,
            viewMode: viewMode,
            disableDialogClose: true
        }, '1350px').subscribe({
            next: (result?: CatchRecordDTO) => {
                if (result !== undefined && result !== null) {
                    const difference: DateDifference | undefined = DateUtils.getDateDifference(result.gearEntryTime!, result.gearExitTime!);
                    result.totalTime = this.dateDifferencePipe.transform(difference);
                    result.catchRecordFishesSummary = result.catchRecordFishes?.map(x => `${x.fishName} ${x.quantityKg}kg (${x.catchQuadrant})`).join(';') ?? '';

                    if (catchRecord !== undefined) {
                        const index: number = this.catchRecords.findIndex(x => x.id === result.id);
                        this.catchRecords[index] = result;
                    }
                    else {
                        this.catchRecords.push(result);
                    }

                    this.catchRecords = this.catchRecords.slice();
                    this.form.updateValueAndValidity({ emitEvent: false });
                }
            }
        });
    }

    public deleteCatchRecord(row: GridRow<CatchRecordDTO>): void {
        this.confirmDialog.open({
            title: this.translationService.getValue('catches-and-sales.delete-catch-record-dialog-label'),
            message: this.translationService.getValue('catches-and-sales.confirm-delete-catch-record-message'),
            okBtnLabel: this.translationService.getValue('catches-and-sales.delete-catch-record-btn-label')
        }).subscribe({
            next: (ok: boolean) => {
                if (ok) {
                    this.catchRecordsTable.softDelete(row);
                    this.catchRecords = this.catchRecordsTable.rows;

                    const catchRecord: CatchRecordDTO = this.catchRecords.find(x => x === row.data)!;
                    for (const catchRecordFish of catchRecord.catchRecordFishes!) {
                        catchRecordFish.isActive = false;
                    }

                    this.catchRecords = this.catchRecords.slice();
                    this.form.updateValueAndValidity({ emitEvent: false });
                }
            }
        });
    }

    public undoDeleteCatchRecord(row: GridRow<CatchRecordDTO>): void {
        this.confirmDialog.open().subscribe({
            next: (ok: boolean) => {
                if (ok) {
                    this.catchRecordsTable.softUndoDelete(row);
                    this.catchRecords = this.catchRecordsTable.rows;
                    this.catchRecords = this.catchRecords.slice();
                    this.form.updateValueAndValidity({ emitEvent: false });
                }
            }
        });
    }

    public addCatchFromPreviousTrip(): void {
        const data: PreviousTripsCatchRecordsDialogParams = new PreviousTripsCatchRecordsDialogParams({
            service: this.service,
            shipId: this.model.shipId
        });

        this.previousTripCatchRecordsDialog.openWithTwoButtons({
            title: this.translationService.getValue('catches-and-sales.previous-trip-catch-records-dialog-title'),
            TCtor: PreviousTripsCatchRecordsComponent,
            translteService: this.translationService,
            headerCancelButton: {
                cancelBtnClicked: this.closePreviousTripCatchRecordsDialogBtnClicked.bind(this)
            },
            componentData: data,
            viewMode: false,
            disableDialogClose: true
        }).subscribe({
            next: (selectedCatches: OnBoardCatchRecordFishDTO[] | undefined) => {
                if (selectedCatches !== null && selectedCatches !== undefined) {
                    this.selectedCatchesFromPreviousTrips = selectedCatches.slice();
                    this.addOriginDeclarationFishesFromPreviousTripCatches();
                }
            }
        });
    }

    public generateOriginDeclarationFromCatchRecordFishes(): void {
        // TODO better
        const catchRecordFishes: CatchRecordFishDTO[][] = this.catchRecords.map(x => x.catchRecordFishes) as CatchRecordFishDTO[][];
        const fishesFlattened: CatchRecordFishDTO[] = ([] as CatchRecordFishDTO[]).concat(...catchRecordFishes);

        this.declarationOfOriginCatchRecords = this.declarationOfOriginCatchRecords.filter(x => x.id !== null && x.id !== undefined);
        for (const declarationFish of this.declarationOfOriginCatchRecords) {
            declarationFish.isActive = false;
        }

        this.declarationOfOriginCatchRecords = [...this.declarationOfOriginCatchRecords, ...this.getOriginDeclarationCatchRecords(fishesFlattened)];
        this.selectedCatchesFromPreviousTrips = [];

        this.form.updateValueAndValidity({ emitEvent: false });
    }

    public editDeclarationOfOriginCatchRecord(declarationOfOriginCatchRecord: OriginDeclarationFishDTO, viewMode: boolean = false): void {
        let title: string = '';
        let headerAuditBtn: IHeaderAuditButton | undefined = undefined;
        const data: OriginDeclarationDialogParamsModel = new OriginDeclarationDialogParamsModel({
            model: declarationOfOriginCatchRecord,
            service: this.service,
            viewMode: viewMode
        });

        if (viewMode === true) {
            title = this.translationService.getValue('catches-and-sales.ship-page-view-origin-declaration-dialog-title');
        }
        else {
            title = this.translationService.getValue('catches-and-sales.ship-page-edit-origin-declaration-dialog-title');
        }

        if (!IS_PUBLIC_APP
            && declarationOfOriginCatchRecord !== null
            && declarationOfOriginCatchRecord !== undefined
            && declarationOfOriginCatchRecord.id !== null
            && declarationOfOriginCatchRecord.id !== undefined
        ) {
            headerAuditBtn = {
                id: declarationOfOriginCatchRecord.id!,
                tableName: 'OriginDeclarationFish',
                tooltip: '',
                getAuditRecordData: this.service.getOriginDeclarationFishSimpleAudit.bind(this.service)
            };
        }

        const dialogClosed = this.openDeclarationOfOriginDialog(declarationOfOriginCatchRecord, title, headerAuditBtn, data, viewMode);

        dialogClosed.subscribe({
            next: (result: OriginDeclarationFishDTO | undefined) => {
                if (result !== undefined) {
                    declarationOfOriginCatchRecord = result;
                    this.declarationOfOriginCatchRecords = this.declarationOfOriginCatchRecords.slice();
                    this.form.updateValueAndValidity({ emitEvent: false });
                }
            }
        });
    }

    public addOriginDeclarationFishesFromPreviousTripCatches(): void {
        const defaultCatchFishPresentation: NomenclatureDTO<number> | undefined = this.catchPresentations.find(x => x.code === FishPresentationCodesEnum[DEFAULT_PRESENTATION_CODE] && x.isActive);
        const defaultCatchFishState: NomenclatureDTO<number> | undefined = this.catchStates.find(x => x.code === FishCatchStateCodesEnum[DEFAULT_CATCH_STATE_CODE] && x.isActive);

        const notAddedCatches: OnBoardCatchRecordFishDTO[] = this.selectedCatchesFromPreviousTrips.filter(x => !this.declarationOfOriginCatchRecords.some(y => y.catchRecordFishId === x.id));

        for (const catchFish of notAddedCatches) {
            const catchQuadrant: CatchZoneNomenclatureDTO = this.catchZones.find(x => x.value === catchFish.catchQuadrantId)!;

            this.declarationOfOriginCatchRecords.push(
                new OriginDeclarationFishDTO({
                    catchRecordFishId: catchFish.id,
                    fishId: catchFish.fishId,
                    quantityKg: catchFish.quantityKg,
                    catchZone: catchQuadrant.zone?.toString(),
                    catchQuadrantId: catchFish.catchQuadrantId,
                    catchQuadrant: catchFish.catchQuadrant,
                    isActive: catchFish.isActive,
                    catchFishPresentationId: defaultCatchFishPresentation?.value,
                    catchFishStateId: defaultCatchFishState?.value,
                    isProcessedOnBoard: false,
                    isValid: true
                })
            );
        }

        this.declarationOfOriginCatchRecords = this.declarationOfOriginCatchRecords.slice();
        this.form.updateValueAndValidity({ emitEvent: false });
    }

    public copyDeclarationOfOriginCatchRecord(declarationOfOriginCatchRecord: OriginDeclarationFishDTO): void {
        const title: string = this.translationService.getValue('catches-and-sales.ship-page-add-origin-declaration-dialog-title');
        const declarationOfOriginCatchRecordCopy = new OriginDeclarationFishDTO({
            catchRecordFishId: declarationOfOriginCatchRecord.id,
            fishId: declarationOfOriginCatchRecord.fishId,
            fishName: declarationOfOriginCatchRecord.fishName,
            originDeclarationId: declarationOfOriginCatchRecord.originDeclarationId,
            catchQuadrantId: declarationOfOriginCatchRecord.catchQuadrantId,
            catchQuadrant: declarationOfOriginCatchRecord.catchQuadrant,
            catchZone: declarationOfOriginCatchRecord.catchZone,
            isActive: true,
            isValid: false
        });
        const data: OriginDeclarationDialogParamsModel = new OriginDeclarationDialogParamsModel({
            model: declarationOfOriginCatchRecordCopy,
            service: this.service,
            viewMode: false
        });

        const dialogClosed = this.openDeclarationOfOriginDialog(declarationOfOriginCatchRecordCopy, title, undefined, data, false);

        dialogClosed.subscribe({
            next: (result: OriginDeclarationFishDTO | undefined) => {
                if (result !== undefined) {
                    this.declarationOfOriginCatchRecords.push(result);
                    this.declarationOfOriginCatchRecords = this.declarationOfOriginCatchRecords.slice();
                    this.form.updateValueAndValidity({ emitEvent: false });
                }
            }
        });
    }

    public removeOriginDeclarationFish(declarationOfOriginCatchRecord: OriginDeclarationFishDTO): void {
        const indexToDelete: number = this.declarationOfOriginCatchRecords.findIndex(x => x === declarationOfOriginCatchRecord);
        const deletedItems: OriginDeclarationFishDTO[] = this.declarationOfOriginCatchRecords.splice(indexToDelete, 1);
        this.declarationOfOriginCatchRecords = this.declarationOfOriginCatchRecords.slice();

        const fishFromPreviousTripIndex: number = this.selectedCatchesFromPreviousTrips.findIndex(x => x.id === deletedItems[0].catchRecordFishId);
        if (fishFromPreviousTripIndex !== -1) {
            this.selectedCatchesFromPreviousTrips.splice(fishFromPreviousTripIndex, 1);
        }

        this.form.updateValueAndValidity({ emitEvent: false });
    }

    public getControlErrorLabelText(controlName: string, errorValue: unknown, errorCode: string): TLError | undefined {
        if (controlName === 'fishTripStartDateTimeControl') {
            if (errorCode === 'matDatetimePickerParse') {
                const message: string = `${this.translationService.getValue('validation.pattern')}: ${this.translationService.getValue('common.date-time-control-format-hint')}`;
                return new TLError({ text: message });
            }
            else if (errorCode === 'required') {
                const message: string = `${this.translationService.getValue('validation.required')} (${this.translationService.getValue('catches-and-sales.with-pattern')}: ${this.translationService.getValue('common.date-time-control-format-hint')})`;
                return new TLError({ text: message });
            }
            else if (errorCode === 'matDatetimePickerMax') {
                if (this.form.get('fishTripEndDateTimeControl')!.value !== null && this.form.get('fishTripEndDateTimeControl')!.value !== undefined) {
                    const maxDate: Date = (this.form.get('fishTripEndDateTimeControl')!.value as Moment).toDate();
                    const dateString: string = this.datePipe.transform(maxDate, 'dd.MM.YYYY HH:mm') ?? "";
                    return new TLError({ text: `${this.translationService.getValue('validation.max')}: ${dateString}` });
                }
            }
        }
        else if (controlName === 'fishTripEndDateTimeControl') {
            if (errorCode === 'matDatetimePickerParse') {
                const message: string = `${this.translationService.getValue('validation.pattern')}: ${this.translationService.getValue('common.date-time-control-format-hint')}`;
                return new TLError({ text: message });
            }
            else if (errorCode === 'required') {
                const message: string = `${this.translationService.getValue('validation.required')} (${this.translationService.getValue('catches-and-sales.with-pattern')}: ${this.translationService.getValue('common.date-time-control-format-hint')})`;
                return new TLError({ text: message });
            }
            else if (errorCode === 'matDatetimePickerMin') {
                if (this.form.get('fishTripStartDateTimeControl')!.value !== null && this.form.get('fishTripStartDateTimeControl')!.value !== undefined) {
                    const maxDate: Date = (this.form.get('fishTripStartDateTimeControl')!.value as Moment).toDate();
                    const dateString: string = this.datePipe.transform(maxDate, 'dd.MM.YYYY HH:mm') ?? "";
                    return new TLError({ text: `${this.translationService.getValue('validation.min')}: ${dateString}` });
                }
            }
        }
        else if (controlName === 'unloadDateTimeControl') {
            if (errorCode === 'matDatetimePickerParse') {
                const message: string = `${this.translationService.getValue('validation.pattern')}: ${this.translationService.getValue('common.date-time-control-format-hint')}`;
                return new TLError({ text: message });
            }
            else if (errorCode === 'required') {
                const message: string = `${this.translationService.getValue('validation.required')} (${this.translationService.getValue('catches-and-sales.with-pattern')}: ${this.translationService.getValue('common.date-time-control-format-hint')})`;
                return new TLError({ text: message });
            }
            else if (errorCode === 'matDatetimePickerMin') {
                if (this.form.get('fishTripEndDateTimeControl')!.value !== null && this.form.get('fishTripEndDateTimeControl')!.value !== undefined) {
                    const maxDate: Date = (this.form.get('fishTripEndDateTimeControl')!.value as Moment).toDate();
                    const dateString: string = this.datePipe.transform(maxDate, 'dd.MM.YYYY HH:mm') ?? "";
                    return new TLError({ text: `${this.translationService.getValue('validation.min')}: ${dateString}` });
                }
            }
        }

        return undefined;
    }

    private fillForm(): void {
        this.form.get('pageNumberControl')!.setValue(this.model.pageNumber);
        this.form.get('fillDateControl')!.setValue(this.model.fillDate);
        this.form.get('iaraAcceptanceDateTimeControl')!.setValue(this.model.iaraAcceptanceDateTime);

        if (this.isEditing) {
            this.form.get('statusControl')!.setValue(this.model.status);
        }

        this.form.get('shipControl')!.setValue(this.model.shipName);
        this.form.get('permitLicenseControl')!.setValue(this.model.permitLicenseName);

        if (this.model.fishingGearRegisterId !== null && this.model.fishingGearRegisterId !== undefined) {
            const fishingGearRegister: FishingGearRegisterNomenclatureDTO = this.fishingGearsRegister.find(x => x.value === this.model.fishingGearRegisterId)!;
            this.form.get('fishingGearRegisterControl')!.setValue(fishingGearRegister);

            if (this.model.fishingGearCount !== null && this.model.fishingGearCount !== undefined) {
                this.form.get('fishingGearCountControl')!.setValue(this.model.fishingGearCount);
            }
            else {
                this.form.get('fishingGearCountControl')!.setValue(fishingGearRegister.gearCount);
            }

            if (this.model.fishingGearHookCount !== null && this.model.fishingGearHookCount !== undefined) {
                this.form.get('fishingGearHookCountControl')!.setValue(fishingGearRegister.hooksCount);
            }
        }
        else {
            this.form.get('fishingGearCountControl')!.setValue(this.model.fishingGearCount);
            this.form.get('fishingGearHookCountControl')!.setValue(this.model.fishingGearHookCount);
        }

        if (this.model.partnerShipId !== null && this.model.partnerShipId !== undefined) {
            this.form.get('partnerShipControl')!.setValue(ShipsUtils.get(this.ships, this.model.partnerShipId!));
        }

        this.form.get('fishTripStartDateTimeControl')!.setValue(moment(this.model.fishTripStartDateTime));

        if (this.model.departurePortId !== null && this.model.departurePortId !== undefined) {
            const departurePort: NomenclatureDTO<number> = this.ports.find(x => x.value === this.model.departurePortId)!;
            this.form.get('departurePortControl')!.setValue(departurePort);
        }

        this.form.get('fishTripEndDateTimeControl')!.setValue(moment(this.model.fishTripEndDateTime));

        if (this.model.arrivalPortId !== null && this.model.arrivalPortId !== undefined) {
            const arrivalPort: NomenclatureDTO<number> = this.ports.find(x => x.value === this.model.arrivalPortId)!;
            this.form.get('arrivalPortControl')!.setValue(arrivalPort);
        }

        if (this.model.unloadPortId !== null && this.model.unloadPortId !== undefined) {
            const unloadPort: NomenclatureDTO<number> = this.ports.find(x => x.value === this.model.unloadPortId)!;
            this.form.get('unloadPortControl')!.setValue(unloadPort);
        }
        else if (this.id !== null && this.id !== undefined && this.model.statusCode === LogBookPageStatusesEnum.Submitted) {
            this.form.get('allCatchIsTransboardedControl')!.setValue(true);
        }

        this.form.get('unloadDateTimeControl')!.setValue(moment(this.model.unloadDateTime));

        this.form.get('filesControl')!.setValue(this.model.files);

        setTimeout(() => {
            this.catchRecords = this.model.catchRecords ?? [];
            this.declarationOfOriginCatchRecords = this.model.originDeclarationFishes ?? [];
            this.form.updateValueAndValidity({ emitEvent: false });

            for (const record of this.catchRecords) {
                const difference: DateDifference | undefined = DateUtils.getDateDifference(record.gearEntryTime!, record.gearExitTime!);
                record.totalTime = this.dateDifferencePipe.transform(difference);
            }
        });
    }

    private buildForm(): void {
        this.form = new FormGroup({
            pageNumberControl: new FormControl(undefined, Validators.required),
            fillDateControl: new FormControl(undefined, Validators.required),
            iaraAcceptanceDateTimeControl: new FormControl(),
            statusControl: new FormControl(),

            shipControl: new FormControl(undefined, Validators.required),
            permitLicenseControl: new FormControl(undefined, Validators.required),
            fishingGearRegisterControl: new FormControl(undefined, Validators.required),
            fishingGearCountControl: new FormControl(undefined, [Validators.required, TLValidators.number(0)]),
            fishingGearHookCountControl: new FormControl(undefined),
            partnerShipControl: new FormControl(),

            fishTripStartDateTimeControl: new FormControl(undefined, Validators.required),
            departurePortControl: new FormControl(undefined, Validators.required),
            fishTripEndDateTimeControl: new FormControl(undefined, Validators.required),
            arrivalPortControl: new FormControl(undefined, Validators.required),
            daysAtSeaCountControl: new FormControl(undefined, Validators.required),
            unloadDateTimeControl: new FormControl(undefined, Validators.required),
            unloadPortControl: new FormControl(undefined, Validators.required),
            allCatchIsTransboardedControl: new FormControl(false),

            filesControl: new FormControl()
        }, [this.originDeclarationFishesValidator(),
            this.delcarationOfOriginCatchRecordQuantitiesValidator(),
            this.originDeclarationCatchRecordFishesDatesValidator()]);

        this.form.get('allCatchIsTransboardedControl')!.valueChanges.subscribe({
            next: (value: boolean | undefined) => {
                if (value) {
                    this.form.get('unloadDateTimeControl')!.clearValidators();
                    this.form.get('unloadPortControl')!.clearValidators();
                }
                else {
                    this.form.get('unloadDateTimeControl')!.setValidators(Validators.required);
                    this.form.get('unloadPortControl')!.setValidators(Validators.required);
                }

                this.form.get('unloadDateTimeControl')!.markAsPending({ emitEvent: false });
                this.form.get('unloadPortControl')!.markAsPending({ emitEvent: false });

                if (this.viewMode) {
                    this.form.get('unloadPortControl')!.disable();
                }
            }
        });

        this.form.get('unloadDateTimeControl')!.valueChanges.subscribe({
            next: (value: Moment | undefined) => {
                this.form.updateValueAndValidity({ emitEvent: false });
            }
        });
    }

    private fillModel(): ShipLogBookPageEditDTO {
        const selectedFishingGear: FishingGearRegisterNomenclatureDTO = this.form.get('fishingGearRegisterControl')!.value;
        const iaraAcceptanceDateTime: Moment | undefined = this.form.get('iaraAcceptanceDateTimeControl')!.value as Moment;

        const model: ShipLogBookPageEditDTO = new ShipLogBookPageEditDTO({
            id: this.model.id,
            pageNumber: this.form.get('pageNumberControl')!.value,
            logBookId: this.model.logBookId,
            fillDate: this.form.get('fillDateControl')!.value,
            originDeclarationId: this.model.originDeclarationId,

            iaraAcceptanceDateTime: iaraAcceptanceDateTime?.toDate(),

            shipId: this.model.shipId,
            shipName: this.model.shipName,
            logBookPermitLicenseId: this.model.logBookPermitLicenseId,
            permitLicenseName: this.model.permitLicenseName,

            fishingGearRegisterId: selectedFishingGear.value,
            fishingGearCount: this.form.get('fishingGearCountControl')!.value,

            fishTripStartDateTime: (this.form.get('fishTripStartDateTimeControl')!.value as Moment)?.toDate(),
            departurePortId: this.form.get('departurePortControl')!.value?.value,
            fishTripEndDateTime: (this.form.get('fishTripEndDateTimeControl')!.value as Moment)?.toDate(),
            arrivalPortId: this.form.get('arrivalPortControl')!.value?.value,
            unloadDateTime: (this.form.get('unloadDateTimeControl')!.value as Moment)?.toDate(),
            unloadPortId: this.form.get('unloadPortControl')!.value?.value,
            allCatchIsTransboarded: this.form.get('allCatchIsTransboardedControl')!.value,

            files: this.form.get('filesControl')!.value
        });

        if (selectedFishingGear.hasHooks === true) {
            model.fishingGearHookCount = this.form.get('fishingGearHookCountControl')!.value;
        }

        if (selectedFishingGear.isForMutualFishing === true) {
            model.partnerShipId = this.form.get('partnerShipControl')!.value!.value;
        }

        model.catchRecords = this.catchRecords;
        model.originDeclarationFishes = this.declarationOfOriginCatchRecords;

        return model;
    }

    private getOriginDeclarationCatchRecords(catchRecordFishes: CatchRecordFishDTO[]): OriginDeclarationFishDTO[] {
        const originDeclarationFishes: OriginDeclarationFishDTO[] = [];
        const defaultCatchFishPresentation: NomenclatureDTO<number> | undefined = this.catchPresentations.find(x => x.code === FishPresentationCodesEnum[DEFAULT_PRESENTATION_CODE] && x.isActive);
        const defaultCatchFishState: NomenclatureDTO<number> | undefined = this.catchStates.find(x => x.code === FishCatchStateCodesEnum[DEFAULT_CATCH_STATE_CODE] && x.isActive);

        for (const catchRecord of catchRecordFishes) {
            originDeclarationFishes.push(
                new OriginDeclarationFishDTO({
                    catchRecordFishId: catchRecord.id,
                    fishId: catchRecord.fishId,
                    fishName: catchRecord.fishName,
                    quantityKg: catchRecord.quantityKg,
                    catchZone: catchRecord.catchZone,
                    catchQuadrantId: catchRecord.catchQuadrantId,
                    catchQuadrant: catchRecord.catchQuadrant,
                    isActive: catchRecord.isActive,
                    catchFishPresentationId: defaultCatchFishPresentation?.value,
                    catchFishPresentationName: defaultCatchFishPresentation?.displayName ?? '',
                    catchFishStateId: defaultCatchFishState?.value,
                    catchFishStateName: defaultCatchFishState?.displayName ?? '',
                    isProcessedOnBoard: false,
                    isValid: true
                })
            );
        }

        return originDeclarationFishes;
    }

    private openDeclarationOfOriginDialog(
        declarationOfOriginCatchRecord: OriginDeclarationFishDTO,
        title: string,
        headerAuditBtn: IHeaderAuditButton | undefined,
        data: OriginDeclarationDialogParamsModel,
        viewMode: boolean
    ): Observable<any> {
        return this.editDeclarationOfOriginDialog.openWithTwoButtons({
            title: title,
            TCtor: EditOriginDeclarationComponent,
            translteService: this.translationService,
            headerCancelButton: {
                cancelBtnClicked: this.closeEditOriginDeclarationDialogBtnClicked.bind(this)
            },
            headerAuditButton: headerAuditBtn,
            componentData: data,
            viewMode: viewMode,
            disableDialogClose: true
        }, '1000px');
    }

    private getDaysAtSeaValue(difference: DateDifference | undefined): string {
        let daysAtSeaValue: string;

        if (difference !== null
            && difference !== undefined
            && (difference.hours! > 0 || difference.minutes! > 0)
        ) {
            const differenceDaysCeiled: number = difference.days! + 1;
            const differenceCeiled: DateDifference = new DateDifference({
                days: differenceDaysCeiled,
                hours: 0,
                minutes: 0,
                seconds: 0
            });
            daysAtSeaValue = `${this.dateDifferencePipe.transform(difference)} (${this.dateDifferencePipe.transform(differenceCeiled)})`;
        }
        else {
            daysAtSeaValue = this.dateDifferencePipe.transform(difference);
        }

        return daysAtSeaValue;
    }

    private copyCatchRecord(originalCatchRecord: CatchRecordDTO): CatchRecordDTO {
        const catchRecord: CatchRecordDTO = new CatchRecordDTO(originalCatchRecord);
        catchRecord.catchRecordFishes = [];
        for (const record of originalCatchRecord.catchRecordFishes ?? []) {
            catchRecord.catchRecordFishes.push(new CatchRecordFishDTO(record));
        }

        return catchRecord;
    }

    private delcarationOfOriginCatchRecordQuantitiesValidator(): ValidatorFn {
        return (control: AbstractControl): ValidationErrors | null => {

            if (control === null || control === undefined) {
                return null;
            }

            const differences: Map<number, number> | undefined = this.validateDeclarationOfOriginCatchRecords();

            if (differences !== null && differences !== undefined) {
                return { 'quantityDifferences': differences };
            }

            return null;
        }
    }

    private validateDeclarationOfOriginCatchRecords(): Map<number, number> | undefined {
        let differences: Map<number, number> | undefined = undefined;
        const quantitiesMapDeclarationOfOrigin: Map<number, number> = new Map<number, number>();

        const unloadedCatchesGroupedByFishType: Record<number, OriginDeclarationFishDTO[]> = CommonUtils.groupBy(this.declarationOfOriginCatchRecords.filter(x => x.isActive), x => x.fishId!);

        for (const fishGroupId in unloadedCatchesGroupedByFishType) {
            const quantity: number = unloadedCatchesGroupedByFishType[fishGroupId].reduce((sum, current) => sum + current.quantityKg!, 0);
            quantitiesMapDeclarationOfOrigin.set(Number(fishGroupId), quantity);
        }

        const catchRecordFishes: CatchRecordFishDTO[][] = this.catchRecords.filter(x => x.isActive).map(x => x.catchRecordFishes) as CatchRecordFishDTO[][];
        const fishesFlattened: CatchRecordFishDTO[] = ([] as CatchRecordFishDTO[]).concat(...catchRecordFishes);
        const catchRecordsGroupedByFishType: Record<number, CatchRecordFishDTO[]> = CommonUtils.groupBy(fishesFlattened, x => x.fishId!);

        const quantitiesMapCatchRecords: Map<number, number> = new Map<number, number>();
        for (const fishGroupId in catchRecordsGroupedByFishType) {
            const quantity: number = catchRecordsGroupedByFishType[fishGroupId].reduce((sum, current) => sum + current.quantityKg!, 0);
            quantitiesMapCatchRecords.set(Number(fishGroupId), quantity);
        }

        for (const [key, value] of quantitiesMapCatchRecords) {
            const originDeclarationQuantity: number | undefined = quantitiesMapDeclarationOfOrigin.get(key);
            if (originDeclarationQuantity !== null && originDeclarationQuantity !== undefined) {
                const diff: number = ((originDeclarationQuantity / value) * 100) - 100; // percents less (-) or more (+) than the original fish quantity;
                if (diff < -PERCENT_TOLERANCE || diff > PERCENT_TOLERANCE) {
                    if (differences === null || differences === undefined) {
                        differences = new Map<number, number>();
                    }

                    differences.set(key, diff);
                }
            }
        }
        
        return differences;
    }

    private originDeclarationFishesValidator(): ValidatorFn {
        return (control: AbstractControl): ValidationErrors | null => {
            if (control === null || control === undefined) {
                return null;
            }

            if (this.declarationOfOriginCatchRecords === null
                || this.declarationOfOriginCatchRecords === undefined
                || this.declarationOfOriginCatchRecords.length === 0
            ) {
                return null;
            }

            const isValid: boolean = !this.declarationOfOriginCatchRecords.some(x => x.isValid !== true && x.isActive);

            if (isValid) {
                return null;
            }
            else {
                return { 'invalidOriginDeclarationCatchRecords': true };
            }
        }
    }

    private originDeclarationCatchRecordFishesDatesValidator(): ValidatorFn {
        return (control: AbstractControl): ValidationErrors | null => {
            if (control === null || control === undefined) {
                return null;
            }

            if (this.declarationOfOriginCatchRecords === null
                || this.declarationOfOriginCatchRecords === undefined
                || this.declarationOfOriginCatchRecords.length === 0
            ) {
                return null;
            }

            const fishesFromPreviousTrips = this.declarationOfOriginCatchRecords.filter(x => this.selectedCatchesFromPreviousTrips.some(y => y.id === x.catchRecordFishId));

            if (fishesFromPreviousTrips.length === 0) {
                return null;
            }
            
            const tripUnloadDateTime: Date | undefined = ((control as FormGroup).get('unloadDateTimeControl')?.value as Moment)?.toDate();

            if (tripUnloadDateTime === null || tripUnloadDateTime === undefined) {
                return null;
            }
            
            const previousTripFishIds: number[] = fishesFromPreviousTrips.map(x => x.catchRecordFishId!);

            const hasInvalidFishRecordDates: boolean = this.selectedCatchesFromPreviousTrips.some((x: OnBoardCatchRecordFishDTO) => {
                return previousTripFishIds.includes(x.id!) && x.tripEndDateTime!.getTime() > tripUnloadDateTime.getTime();
            });

            if (hasInvalidFishRecordDates) {
                return { 'hasInvalidCatchRecordDates': true };
            }

            return null;
        }
    }

    private closeEditCatchRecordDialogBtnClicked(closeFn: HeaderCloseFunction): void {
        closeFn();
    }

    private closeEditOriginDeclarationDialogBtnClicked(closeFn: HeaderCloseFunction): void {
        closeFn();
    }

    private closePreviousTripCatchRecordsDialogBtnClicked(closeFn: HeaderCloseFunction): void {
        closeFn();
    }
}
