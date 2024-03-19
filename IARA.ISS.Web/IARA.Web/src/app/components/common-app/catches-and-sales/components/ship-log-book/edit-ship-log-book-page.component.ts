import { Component, OnInit, ViewChild } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { forkJoin, Observable, of } from 'rxjs';
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
import { ShipLogBookPageDataService } from './services/ship-log-book-page-data.service';
import { GetControlErrorLabelTextCallback } from '@app/shared/components/input-controls/base-tl-control';
import { SystemParametersService } from '@app/services/common-app/system-parameters.service';
import { LockShipLogBookPeriodsModel } from './models/lock-ship-log-book-periods.model';
import { SystemPropertiesDTO } from '@app/models/generated/dtos/SystemPropertiesDTO';
import { CatchesAndSalesUtils } from '@app/components/common-app/catches-and-sales/utils/catches-and-sales.utils';
import { LogBookPageEditExceptionDTO } from '@app/models/generated/dtos/LogBookPageEditExceptionDTO';
import { FishPreservationCodesEnum } from '@app/enums/fish-preservation-codes.enum';
import { SecurityService } from '@app/services/common-app/security.service';

const PERCENT_TOLERANCE: number = 10;
const QUALITY_DIFF_VALIDATOR_NAME: string = 'quantityDifferences';
export const DEFAULT_PRESENTATION_CODE: FishPresentationCodesEnum = FishPresentationCodesEnum.WHL;
export const DEFAULT_PRESERVATION_CODE: FishPreservationCodesEnum = FishPreservationCodesEnum.FRE;
export const DEFAULT_CATCH_STATE_CODE: FishCatchStateCodesEnum = FishCatchStateCodesEnum.E;

@Component({
    selector: 'edit-ship-log-book-page',
    templateUrl: './edit-ship-log-book-page.component.html',
    providers: [ShipLogBookPageDataService]
})
export class EditShipLogBookPageComponent implements OnInit, IDialogComponent {
    public readonly today: Date = new Date();
    public readonly pageCode: PageCodeEnum = PageCodeEnum.ShipLogBookPage;
    public readonly icIconSize: number = CommonUtils.IC_ICON_SIZE;

    public ogirinDeclarationHasCatchFromPreviousTripLabel: string = '';

    public form!: FormGroup;
    public viewMode!: boolean;
    public model!: ShipLogBookPageEditDTO;
    public service!: ICatchesAndSalesService;

    public catchRecords: CatchRecordDTO[] = [];

    public declarationOfOriginCatchRecords: OriginDeclarationFishDTO[] = [];
    public declarationOfOriginHasCatchFromPreviousTrip: boolean = false;

    public ships: ShipNomenclatureDTO[] = [];
    public fishingGearsRegister: FishingGearRegisterNomenclatureDTO[] = [];
    public ports: NomenclatureDTO<number>[] = [];
    public aquaticOrganisms: FishNomenclatureDTO[] = [];
    public catchStates: NomenclatureDTO<number>[] = [];
    public catchPresentations: NomenclatureDTO<number>[] = [];
    public catchPreservations: NomenclatureDTO<number>[] = [];
    public catchZones: CatchZoneNomenclatureDTO[] = [];
    public catchRecordQuantities: Map<number, number> = new Map<number, number>();
    public declarationOfOriginCatchRecordsQuantities: Map<number, number> = new Map<number, number>();

    public showHooksCountField: boolean = false;
    public showPartnerShipField: boolean = false;

    public isEditing: boolean = false;
    public isAdd: boolean = false;

    public allCatchIsTransboardedValue: boolean = false;
    public isLogBookPageDateLockedError: boolean = false;

    public getControlErrorLabelTextMethod: GetControlErrorLabelTextCallback = this.getControlErrorLabelText.bind(this);

    @ViewChild(ValidityCheckerGroupDirective)
    private validityCheckerGroup!: ValidityCheckerGroupDirective;

    @ViewChild('catchRecordsTable')
    private catchRecordsTable!: TLDataTableComponent;

    @ViewChild('declarationOfOriginCatchRecordsTable')
    private declarationOfOriginCatchRecordsTable!: TLDataTableComponent;

    private id: number | undefined;
    private translationService: FuseTranslationLoaderService;

    private selectedCatchesFromPreviousTrips: OnBoardCatchRecordFishDTO[] = [];
    private hasMissingPagesRangePermission: boolean = false;

    private lockShipLogBookPeriods!: LockShipLogBookPeriodsModel;
    private logBookPageEditExceptions: LogBookPageEditExceptionDTO[] = [];
    private currentUserId: number;

    private readonly editCatchRecordDialog: TLMatDialog<EditCatchRecordComponent>;
    private readonly editDeclarationOfOriginDialog: TLMatDialog<EditOriginDeclarationComponent>;
    private readonly previousTripCatchRecordsDialog: TLMatDialog<PreviousTripsCatchRecordsComponent>;
    private readonly confirmDialog: TLConfirmDialog;
    private readonly snackbar: MatSnackBar;
    private readonly commonNomenclaturesService: CommonNomenclatures;
    private readonly dateDifferencePipe: TLDateDifferencePipe;
    private readonly datePipe: DatePipe;
    private readonly shipLogBookPageDataService: ShipLogBookPageDataService;
    private readonly systemParametersService: SystemParametersService;

    public constructor(
        translate: FuseTranslationLoaderService,
        editCatchRecordDialog: TLMatDialog<EditCatchRecordComponent>,
        editDeclarationOfOriginDialog: TLMatDialog<EditOriginDeclarationComponent>,
        previousTripCatchRecordsDialog: TLMatDialog<PreviousTripsCatchRecordsComponent>,
        commonNomenclaturesService: CommonNomenclatures,
        confirmDialog: TLConfirmDialog,
        snackbar: MatSnackBar,
        dateDifferencePipe: TLDateDifferencePipe,
        datePipe: DatePipe,
        shipLogBookPageDataService: ShipLogBookPageDataService,
        systemParametersService: SystemParametersService,
        authService: SecurityService
    ) {
        this.translationService = translate;
        this.commonNomenclaturesService = commonNomenclaturesService;
        this.snackbar = snackbar;
        this.dateDifferencePipe = dateDifferencePipe;
        this.datePipe = datePipe;
        this.shipLogBookPageDataService = shipLogBookPageDataService;
        this.systemParametersService = systemParametersService;

        this.editCatchRecordDialog = editCatchRecordDialog;
        this.editDeclarationOfOriginDialog = editDeclarationOfOriginDialog;
        this.confirmDialog = confirmDialog;
        this.previousTripCatchRecordsDialog = previousTripCatchRecordsDialog;

        this.currentUserId = authService.User!.userId;
    }

    public async ngOnInit(): Promise<void> {
        const subscriptions: Observable<(NomenclatureDTO<number> | ShipNomenclatureDTO | FishingGearRegisterNomenclatureDTO | FishNomenclatureDTO)[] | LogBookPageEditExceptionDTO[]>[] = [
            NomenclatureStore.instance.getNomenclature(
                NomenclatureTypes.Ships, this.commonNomenclaturesService.getShips.bind(this.commonNomenclaturesService), false),
            NomenclatureStore.instance.getNomenclature(
                NomenclatureTypes.Ports, this.commonNomenclaturesService.getPorts.bind(this.commonNomenclaturesService), false),
            NomenclatureStore.instance.getNomenclature(
                NomenclatureTypes.Fishes, this.commonNomenclaturesService.getFishTypes.bind(this.commonNomenclaturesService), false),
            NomenclatureStore.instance.getNomenclature<number>(
                NomenclatureTypes.CatchStates, this.service.getCatchStates.bind(this.service), false),
            NomenclatureStore.instance.getNomenclature<number>(
                NomenclatureTypes.CatchPresentations, this.commonNomenclaturesService.getCatchPresentations.bind(this.commonNomenclaturesService), false),
            NomenclatureStore.instance.getNomenclature<number>(
                NomenclatureTypes.CatchPreservations, this.commonNomenclaturesService.getCatchPreservations.bind(this.commonNomenclaturesService), false),
            NomenclatureStore.instance.getNomenclature(
                NomenclatureTypes.CatchZones, this.commonNomenclaturesService.getCatchZones.bind(this.commonNomenclaturesService), false),
        ];

        if (!this.viewMode) {
            subscriptions.push(this.service.getLogBookPageEditExceptions());
        }
        const nomenclatures = await forkJoin(subscriptions).toPromise();

        this.ships = nomenclatures[0] as ShipNomenclatureDTO[];
        this.ports = nomenclatures[1] as NomenclatureDTO<number>[];
        this.aquaticOrganisms = nomenclatures[2] as FishNomenclatureDTO[];
        this.catchStates = nomenclatures[3] as NomenclatureDTO<number>[];
        this.catchPresentations = nomenclatures[4] as NomenclatureDTO<number>[];
        this.catchPreservations = nomenclatures[5] as NomenclatureDTO<number>[];
        this.catchZones = nomenclatures[6] as CatchZoneNomenclatureDTO[];

        if (!this.viewMode) {
            this.logBookPageEditExceptions = nomenclatures[7] as LogBookPageEditExceptionDTO[];
        }

        const systemParameters: SystemPropertiesDTO = await this.systemParametersService.systemParameters();

        this.lockShipLogBookPeriods = new LockShipLogBookPeriodsModel({
            lockShipUnder10MLogBookAfterDays: systemParameters.lockShipUnder10MLogBookAfterDays,
            lockShip10M12MLogBookAfterHours: systemParameters.lockShip10M12MLogBookAfterHours,
            lockShipOver12MLogBookAfterHours: systemParameters.lockShipOver12MLogBookAfterHours,
            lockPageAfterDays: systemParameters.addShipPagesDaysTolerance
        });

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

        this.form.get('departurePortControl')!.valueChanges.subscribe({
            next: (departurePort: NomenclatureDTO<number> | string | null | undefined) => {
                this.onDeparturePortChanged(departurePort);
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
                this.addShipLogBookPage(dialogClose);
            }
            else {
                this.service.editShipLogBookPage(this.model).subscribe({
                    next: () => {
                        dialogClose(this.model);
                    },
                    error: (response: HttpErrorResponse) => {
                        this.addOrEditShipLogBookPageErrorResponseHandler(response);
                    }
                });
            }
        }
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
            waterType: this.model.permitLicenseWaterType!,
            permitLicenseAquaticOrganismTypeIds: this.model.permitLicenseAquaticOrganismTypeIds ?? [],
            shipLogBookPageDataService: this.shipLogBookPageDataService
        });

        const tripStartDate: Moment | null | undefined = this.form.get('fishTripStartDateTimeControl')!.value;
        const tripEndDate: Moment | null | undefined = this.form.get('fishTripEndDateTimeControl')!.value;

        if (tripStartDate !== null && tripStartDate !== undefined) {
            data.tripStartDateTime = tripStartDate.toDate();
        }

        if (tripEndDate !== null && tripEndDate !== undefined) {
            data.tripEndDateTime = tripEndDate.toDate();
        }

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
        }, '1400px').subscribe({
            next: (result?: CatchRecordDTO) => {
                if (result !== undefined && result !== null) {
                    const difference: DateDifference | undefined = DateUtils.getDateDifference(result.gearEntryTime!, result.gearExitTime!);
                    result.totalTime = this.dateDifferencePipe.transform(difference);
                    result.catchRecordFishesSummary = result.catchRecordFishes?.map(x => `${x.fishName} ${x.quantityKg}kg (${x.catchQuadrant !== undefined && x.catchQuadrant !== null ? x.catchQuadrant : ''})`).join(';') ?? '';

                    if (catchRecord !== undefined) {
                        const index: number = this.catchRecords.findIndex(x => x.id === result.id);
                        this.catchRecords[index] = result;
                    }
                    else {
                        this.catchRecords.push(result);
                    }

                    this.catchRecords = this.catchRecords.slice();
                    this.recalculateCatchRecordsQuantitySums();
                    this.form.updateValueAndValidity({ onlySelf: true, emitEvent: false });
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
                    this.recalculateCatchRecordsQuantitySums();
                    this.form.updateValueAndValidity({ onlySelf: true, emitEvent: false });
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
                    this.recalculateCatchRecordsQuantitySums();
                    this.form.updateValueAndValidity({ onlySelf: true, emitEvent: false });
                }
            }
        });
    }

    public addCatchFromPreviousTrip(): void {
        const data: PreviousTripsCatchRecordsDialogParams = new PreviousTripsCatchRecordsDialogParams({
            service: this.service,
            shipId: this.model.shipId,
            currentPageId: this.model.id
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
        }, '1200px').subscribe({
            next: (selectedCatches: OnBoardCatchRecordFishDTO[] | undefined) => {
                if (selectedCatches !== null && selectedCatches !== undefined) {
                    this.selectedCatchesFromPreviousTrips = selectedCatches.slice();
                    this.addOriginDeclarationFishesFromPreviousTripCatches();
                    this.recalculateCatchRecordsQuantitySums();
                }
            }
        });
    }

    public generateOriginDeclarationFromCatchRecordFishes(): void {
        for (const declarationFish of this.declarationOfOriginCatchRecords.filter(x => x.isActive)) {
            declarationFish.isActive = false;
        }

        this.declarationOfOriginCatchRecords = this.declarationOfOriginCatchRecords.filter(x => x.id !== null && x.id !== undefined);

        const possibleCatchRecordFishes: CatchRecordFishDTO[] = this.getPossibleCatchRecordFishesForOriginDeclaration();
        this.declarationOfOriginCatchRecords = [...this.declarationOfOriginCatchRecords, ...this.getOriginDeclarationCatchRecords(possibleCatchRecordFishes)];
        this.setDeclarationOfOriginHasCatchFromPreviousTripFlag();
        this.selectedCatchesFromPreviousTrips = [];
        this.recalculateDeclarationOfOriginCatchRecordsQuantitySums();

        this.form.markAsTouched();
        this.form.updateValueAndValidity({ onlySelf: true, emitEvent: false });
    }

    public editDeclarationOfOriginCatchRecord(declarationOfOriginCatchRecord: OriginDeclarationFishDTO, viewMode: boolean = false): void {
        let title: string = '';
        let headerAuditBtn: IHeaderAuditButton | undefined = undefined;
        const data: OriginDeclarationDialogParamsModel = new OriginDeclarationDialogParamsModel({
            model: declarationOfOriginCatchRecord,
            service: this.service,
            viewMode: viewMode,
            isAllCatchTransboarded: this.allCatchIsTransboardedValue
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
                    this.recalculateUnloadedQuantities();
                    this.recalculateDeclarationOfOriginCatchRecordsQuantitySums();

                    this.form.markAsTouched();
                    this.form.updateValueAndValidity({ onlySelf: true, emitEvent: false });
                }
            }
        });
    }

    public addOriginDeclarationFishesFromPreviousTripCatches(): void {
        const defaultCatchFishPresentation: NomenclatureDTO<number> | undefined = this.catchPresentations.find(x => x.code === FishPresentationCodesEnum[DEFAULT_PRESENTATION_CODE] && x.isActive);
        const defaultCatchFishPreservation: NomenclatureDTO<number> | undefined = this.catchPreservations.find(x => x.code === FishPreservationCodesEnum[DEFAULT_PRESERVATION_CODE] && x.isActive);
        const defaultCatchFishState: NomenclatureDTO<number> | undefined = this.catchStates.find(x => x.code === FishCatchStateCodesEnum[DEFAULT_CATCH_STATE_CODE] && x.isActive);

        const notAddedCatches: OnBoardCatchRecordFishDTO[] = this.selectedCatchesFromPreviousTrips.filter(x => !this.declarationOfOriginCatchRecords.some(y => y.isActive));

        for (const catchFish of notAddedCatches) {
            const catchQuadrant: CatchZoneNomenclatureDTO = this.catchZones.find(x => x.value === catchFish.catchQuadrantId)!;

            this.declarationOfOriginCatchRecords.push(
                new OriginDeclarationFishDTO({
                    fishId: catchFish.fishId,
                    quantityKg: catchFish.quantityKg,
                    catchZone: catchQuadrant.zone?.toString(),
                    catchQuadrantId: catchFish.catchQuadrantId,
                    catchQuadrant: catchFish.catchQuadrant,
                    catchSizeId: catchFish.catchSizeId,
                    isActive: catchFish.isActive,
                    catchFishPresentationId: defaultCatchFishPresentation?.value,
                    catchFishPreservationId: defaultCatchFishPreservation?.value,
                    catchFishStateId: defaultCatchFishState?.value,
                    isProcessedOnBoard: false,
                    fromPreviousTrip: true,
                    isValid: true
                })
            );
        }

        this.setDeclarationOfOriginHasCatchFromPreviousTripFlag();
        this.declarationOfOriginCatchRecords = this.declarationOfOriginCatchRecords.slice();
        this.recalculateDeclarationOfOriginCatchRecordsQuantitySums();

        this.form.updateValueAndValidity({ onlySelf: true, emitEvent: false });
    }

    public copyDeclarationOfOriginCatchRecord(declarationOfOriginCatchRecord: OriginDeclarationFishDTO): void {
        const title: string = this.translationService.getValue('catches-and-sales.ship-page-add-origin-declaration-dialog-title');
        const declarationOfOriginCatchRecordCopy = new OriginDeclarationFishDTO({
            fishId: declarationOfOriginCatchRecord.fishId,
            fishName: declarationOfOriginCatchRecord.fishName,
            originDeclarationId: declarationOfOriginCatchRecord.originDeclarationId,
            catchQuadrantId: declarationOfOriginCatchRecord.catchQuadrantId,
            catchSizeId: declarationOfOriginCatchRecord.catchSizeId,
            catchQuadrant: declarationOfOriginCatchRecord.catchQuadrant,
            catchZone: declarationOfOriginCatchRecord.catchZone,
            fromPreviousTrip: declarationOfOriginCatchRecord.fromPreviousTrip,
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
                    this.recalculateDeclarationOfOriginCatchRecordsQuantitySums();

                    this.form.markAsTouched();
                    this.form.updateValueAndValidity({ onlySelf: true, emitEvent: false });
                }
            }
        });
    }

    public removeOriginDeclarationFish(declarationOfOriginCatchRecord: OriginDeclarationFishDTO): void {
        if (declarationOfOriginCatchRecord.id !== null && declarationOfOriginCatchRecord.id !== undefined) {
            declarationOfOriginCatchRecord.isActive = false;
        }
        else {
            const indexToDelete: number = this.declarationOfOriginCatchRecords.findIndex(x => x === declarationOfOriginCatchRecord);
            const deletedItems: OriginDeclarationFishDTO[] = this.declarationOfOriginCatchRecords.splice(indexToDelete, 1);
            this.setDeclarationOfOriginHasCatchFromPreviousTripFlag();
        }

        this.declarationOfOriginCatchRecords = this.declarationOfOriginCatchRecords.slice();
        this.recalculateDeclarationOfOriginCatchRecordsQuantitySums();

        this.form.updateValueAndValidity({ onlySelf: true, emitEvent: false });
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
                    let messageText: string = this.translationService.getValue('validation.max');
                    messageText = messageText[0].toUpperCase() + messageText.substr(1);
                    return new TLError({ text: `${messageText}: ${dateString}` });
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
            else if (errorCode === 'endDateSameAsStartDate') {
                const message: string = `${this.translationService.getValue('catches-and-sales.end-date-should-be-different-from-start-date')}`;
                return new TLError({ text: message });
            }
            else if (errorCode === 'matDatetimePickerMin') {
                if (this.form.get('fishTripStartDateTimeControl')!.value !== null && this.form.get('fishTripStartDateTimeControl')!.value !== undefined) {
                    const minDate: Date = (this.form.get('fishTripStartDateTimeControl')!.value as Moment).toDate();
                    const dateString: string = this.datePipe.transform(minDate, 'dd.MM.YYYY HH:mm') ?? "";
                    let messageText: string = this.translationService.getValue('validation.min');
                    messageText = messageText[0].toUpperCase() + messageText.substr(1);
                    return new TLError({ text: `${messageText}: ${dateString}` });
                }
            }
            else if (errorCode === 'logBookPageDateLocked') {
                const messageText: string = this.translationService.getValue('catches-and-sales.ship-page-date-cannot-be-chosen-error');
                if (this.isLogBookPageDateLockedError) {
                    return new TLError({ text: messageText, type: 'error' });
                }
                else {
                    return new TLError({ text: messageText, type: 'warn' });
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
                    const minDate: Date = (this.form.get('fishTripEndDateTimeControl')!.value as Moment).toDate();
                    const dateString: string = this.datePipe.transform(minDate, 'dd.MM.YYYY HH:mm') ?? "";
                    return new TLError({ text: `${this.translationService.getValue('validation.min')}: ${dateString}` });
                }
            }
            else if (errorCode === 'logBookPageDateLocked') {
                const messageText: string = this.translationService.getValue('catches-and-sales.ship-page-date-cannot-be-chosen-error');
                if (this.isLogBookPageDateLockedError) {
                    return new TLError({ text: messageText, type: 'error' });
                }
                else {
                    return new TLError({ text: messageText, type: 'warn' });
                }
            }
        }

        return undefined;
    }

    private fillForm(): void {
        this.form.get('pageNumberControl')!.setValue(this.model.pageNumber);
        this.form.get('fillDateControl')!.setValue(this.model.fillDate);
        //this.form.get('iaraAcceptanceDateTimeControl')!.setValue(this.model.iaraAcceptanceDateTime);

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
                this.form.get('fishingGearHookCountControl')!.setValue(this.model.fishingGearHookCount);
            }
            else {
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

        if (this.model.fishTripStartDateTime !== undefined && this.model.fishTripStartDateTime !== null) {
            this.form.get('fishTripStartDateTimeControl')!.setValue(moment(this.model.fishTripStartDateTime));
        }
        else {
            if (!this.viewMode) {
                const today: Date = new Date();
                const fishTripStartDateTime = new Date(today.setHours(6, 0, 0));
                this.form.get('fishTripStartDateTimeControl')!.setValue(moment(fishTripStartDateTime));
            }
        }

        if (this.model.departurePortId !== null && this.model.departurePortId !== undefined) {
            const departurePort: NomenclatureDTO<number> = this.ports.find(x => x.value === this.model.departurePortId)!;
            this.form.get('departurePortControl')!.setValue(departurePort);
        }

        if (this.model.fishTripEndDateTime !== undefined && this.model.fishTripEndDateTime !== null) {
            this.form.get('fishTripEndDateTimeControl')!.setValue(moment(this.model.fishTripEndDateTime));
        }

        if (this.model.arrivalPortId !== null && this.model.arrivalPortId !== undefined) {
            const arrivalPort: NomenclatureDTO<number> = this.ports.find(x => x.value === this.model.arrivalPortId)!;
            this.form.get('arrivalPortControl')!.setValue(arrivalPort);
        }

        if (this.model.unloadPortId !== null && this.model.unloadPortId !== undefined) {
            const unloadPort: NomenclatureDTO<number> = this.ports.find(x => x.value === this.model.unloadPortId)!;
            this.form.get('unloadPortControl')!.setValue(unloadPort);
        }

        this.form.get('allCatchIsTransboardedControl')!.setValue(this.model.allCatchIsTransboarded ?? false);

        this.form.get('noCatchUnloadedControl')!.setValue(this.model.hasNoUnloadedCatch ?? false);

        if (this.model.unloadDateTime !== undefined && this.model.unloadDateTime !== null) {
            this.form.get('unloadDateTimeControl')!.setValue(moment(this.model.unloadDateTime));
        }

        this.form.get('filesControl')!.setValue(this.model.files);

        this.catchRecords.push(...this.model.catchRecords ?? []);
        for (const record of this.catchRecords) {
            const difference: DateDifference | undefined = DateUtils.getDateDifference(record.gearEntryTime!, record.gearExitTime!);
            record.totalTime = this.dateDifferencePipe.transform(difference);
        }

        this.declarationOfOriginCatchRecords.push(...this.model.originDeclarationFishes ?? []);
        this.setDeclarationOfOriginHasCatchFromPreviousTripFlag();

        setTimeout(() => {
            this.catchRecords = this.catchRecords.slice();

            if (this.declarationOfOriginHasCatchFromPreviousTrip) {
                this.ogirinDeclarationHasCatchFromPreviousTripLabel = this.translationService.getValue('catches-and-sales.ship-page-origin-declaration-has-catch-from-previous-trip');
            }
            else {
                this.ogirinDeclarationHasCatchFromPreviousTripLabel = '';
            }

            this.declarationOfOriginCatchRecords = this.declarationOfOriginCatchRecords.slice();
            this.recalculateDeclarationOfOriginCatchRecordsQuantitySums();
            this.recalculateCatchRecordsQuantitySums();
        });

        this.form.updateValueAndValidity({ onlySelf: true, emitEvent: false });
    }

    private buildForm(): void {
        this.form = new FormGroup({
            pageNumberControl: new FormControl(undefined, Validators.required),
            fillDateControl: new FormControl(undefined, Validators.required),
            //iaraAcceptanceDateTimeControl: new FormControl(),
            statusControl: new FormControl(),

            shipControl: new FormControl(undefined, Validators.required),
            permitLicenseControl: new FormControl(undefined, Validators.required),
            fishingGearRegisterControl: new FormControl(undefined, Validators.required),
            fishingGearNetEyeSizeControl: new FormControl({ value: null, disabled: true }),
            fishingGearCountControl: new FormControl(undefined, [Validators.required, TLValidators.number(0, undefined, 0)]),
            fishingGearHookCountControl: new FormControl(undefined),
            partnerShipControl: new FormControl(),

            fishTripStartDateTimeControl: new FormControl(), // the validators are set right after the form is instantiated
            departurePortControl: new FormControl(undefined, Validators.required),
            fishTripEndDateTimeControl: new FormControl(), // the validators are set right after the form is instantiated
            arrivalPortControl: new FormControl(undefined, Validators.required),
            daysAtSeaCountControl: new FormControl(undefined, Validators.required),
            unloadDateTimeControl: new FormControl(), // the validators are set right after the form is instantiated
            unloadPortControl: new FormControl(undefined, Validators.required),

            allCatchIsTransboardedControl: new FormControl(false),
            noCatchUnloadedControl: new FormControl(false),

            filesControl: new FormControl()
        }, [
            this.originDeclarationFishesValidator(),
            this.delcarationOfOriginCatchRecordQuantitiesValidator(),
            this.allOriginDeclarationFishTransboardedIfMarkedValidator(),
            this.requiredOriginDeclarationIfCatchOnBoard(),
            this.gearEntryTimeValidator()
        ]);

        // set validators

        this.setFishTripStartDateTimeControlValidators();
        this.setFishTripEndDateTimeControlInitialValidators();
        this.setUnloadDateTimeControlValidators();

        // value changes

        this.form.get('fishingGearRegisterControl')!.valueChanges.subscribe({
            next: (value: FishingGearRegisterNomenclatureDTO | string | undefined) => {
                this.onFishingGearRegisterChanged(value);
            }
        });

        this.form.get('fishTripStartDateTimeControl')!.valueChanges.subscribe({
            next: (startDate: Moment | null | undefined) => {
                this.onFishTripStartDateTimeChanged(startDate);
            }
        });

        this.form.get('fishTripEndDateTimeControl')!.valueChanges.subscribe({
            next: (endDate: Moment | null | undefined) => {
                this.onFishTripEndDateTimeChanged(endDate);
            }
        });

        this.form.get('allCatchIsTransboardedControl')!.valueChanges.subscribe({
            next: (value: boolean | undefined) => {
                this.allCatchIsTransboardedValue = value ?? false;
                this.changeUnloadingControlsValidators(this.allCatchIsTransboardedValue);
            }
        });

        this.form.get('noCatchUnloadedControl')!.valueChanges.subscribe({
            next: (value: boolean) => {
                this.onNoCatchUnloadedChanged(value);
            }
        });

        this.form.get('unloadDateTimeControl')!.valueChanges.subscribe({
            next: (unloadDate: Moment | null | undefined) => {
                this.onUnloadDateTimeChanged(unloadDate);
                this.form.updateValueAndValidity({ onlySelf: true, emitEvent: false });
            }
        });
    }

    private fillModel(): ShipLogBookPageEditDTO {
        const selectedFishingGear: FishingGearRegisterNomenclatureDTO = this.form.get('fishingGearRegisterControl')!.value;
        //const iaraAcceptanceDateTime: Moment | undefined = this.form.get('iaraAcceptanceDateTimeControl')!.value as Moment;

        const model: ShipLogBookPageEditDTO = new ShipLogBookPageEditDTO({
            id: this.model.id,
            pageNumber: this.form.get('pageNumberControl')!.value,
            logBookId: this.model.logBookId,
            logBookTypeId: this.model.logBookTypeId,
            fillDate: this.form.get('fillDateControl')!.value,
            originDeclarationId: this.model.originDeclarationId,

            //iaraAcceptanceDateTime: iaraAcceptanceDateTime?.toDate(),

            shipId: this.model.shipId,
            shipName: this.model.shipName,
            logBookPermitLicenseId: this.model.logBookPermitLicenseId,

            permitLicenseName: this.model.permitLicenseName,
            permitLicenseWaterType: this.model.permitLicenseWaterType,
            permitLicenseWaterTypeName: this.model.permitLicenseWaterTypeName,
            permitLicenseId: this.model.permitLicenseId,
            permitLicenseNumber: this.model.permitLicenseNumber,

            status: this.model.status,
            statusCode: this.model.statusCode,

            fishingGearRegisterId: selectedFishingGear.value,
            fishingGearCount: this.form.get('fishingGearCountControl')!.value,

            fishTripStartDateTime: (this.form.get('fishTripStartDateTimeControl')!.value as Moment)?.toDate(),
            departurePortId: this.form.get('departurePortControl')!.value?.value,
            fishTripEndDateTime: (this.form.get('fishTripEndDateTimeControl')!.value as Moment)?.toDate(),
            arrivalPortId: this.form.get('arrivalPortControl')!.value?.value,
            unloadDateTime: (this.form.get('unloadDateTimeControl')!.value as Moment)?.toDate(),
            unloadPortId: this.form.get('unloadPortControl')!.value?.value,

            allCatchIsTransboarded: this.form.get('allCatchIsTransboardedControl')!.value,
            hasNoUnloadedCatch: this.form.get('noCatchUnloadedControl')!.value ?? false,

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

    private isFormValid(): boolean {
        let isValid: boolean = this.form.valid;

        if (isValid === false) {
            const innerErrors: Record<string, unknown> = {};
            for (const controlName in this.form.controls) {
                if (this.form.get(controlName)!.errors !== null && this.form.get(controlName)!.errors !== undefined) {
                    for (const key in this.form.get(controlName)!.errors) {
                        if (controlName === 'fishTripEndDateTimeControl' || controlName === 'unloadDateTimeControl') {
                            if (key !== 'logBookPageDateLocked' && !this.isLogBookPageDateLockedError) {
                                innerErrors[key] = this.form.get(controlName)!.errors![key];
                            }
                        }
                        else {
                            innerErrors[key] = this.form.get(controlName)!.errors![key];
                        }
                    }
                }
            }

            const innerErrorKeys = Object.keys(innerErrors);
            if (innerErrorKeys.length === 0) {
                if (this.form.errors !== null && this.form.errors !== undefined) {
                    let errorKeys = Object.keys(this.form.errors);

                    const logBookPageDateLocked: string | undefined = errorKeys.find((key: string) => key === 'logBookPageDateLocked');
                    if (logBookPageDateLocked !== undefined && logBookPageDateLocked !== null && !this.isLogBookPageDateLockedError) {
                        errorKeys.splice(errorKeys.indexOf(logBookPageDateLocked), 1);
                        errorKeys = errorKeys.slice();
                    }

                    if (errorKeys.length === 1 && errorKeys[0] === QUALITY_DIFF_VALIDATOR_NAME) {
                        isValid = true;
                    }

                    if (errorKeys.length === 0) {
                        isValid = true;
                    }
                }
                else {
                    isValid = true;
                }
            }
        }

        return isValid;
    }

    private addShipLogBookPage(dialogClose: DialogCloseCallback): void {
        this.service.addShipLogBookPage(this.model, this.hasMissingPagesRangePermission).subscribe({
            next: (id: number) => {
                this.model.id = id;
                dialogClose(this.model);
            },
            error: (response: HttpErrorResponse) => {
                this.addOrEditShipLogBookPageErrorResponseHandler(response, dialogClose);
            }
        });
    }

    private addOrEditShipLogBookPageErrorResponseHandler(response: HttpErrorResponse, dialogClose?: DialogCloseCallback): void {
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
        else if (error?.code === ErrorCode.LogBookPageAlreadySubmittedOtherLogBook) {
            const message: string = this.translationService.getValue('catches-and-sales.ship-log-book-page-already-submitted-other-logbook-error');
            this.snackbar.open(
                `${message}: ${error.messages[0]}`,
                undefined,
                {
                    duration: RequestProperties.DEFAULT.showExceptionDurationErr,
                    panelClass: RequestProperties.DEFAULT.showExceptionColorClassErr
                });
        }
        else if (error?.code === ErrorCode.MaxNumberMissingPagesExceeded) {
            if (error!.messages === null || error!.messages === undefined || error!.messages.length < 2) {
                throw new Error('In MaxNumberMissingPagesExceeded exception at least the last used page number and a number saying the difference should be passed in the messages property.');
            }

            const lastUsedPageNum: number = Number(error!.messages[0]);
            const diff: number = Number(error!.messages[1]);
            const pageToAdd: string = this.model.pageNumber!;

            // confirmation message

            let message: string = '';

            if (lastUsedPageNum === 0) { // няма добавени страници все още към този дневник
                const currentStartPage: number = Number(error!.messages[2]);

                const msg1: string = this.translationService.getValue('catches-and-sales.transportation-page-generate-missing-pages-permission-no-pages-first-message');
                const msg2: string = this.translationService.getValue('catches-and-sales.transportation-page-generate-missing-pages-permission-second-message');
                const msg3: string = this.translationService.getValue('catches-and-sales.transportation-page-generate-missing-pages-permission-third-message');
                const msg4: string = this.translationService.getValue('catches-and-sales.transportation-page-generate-missing-pages-permission-forth-message');
                const msg5: string = this.translationService.getValue('catches-and-sales.transportation-page-generate-missing-pages-permission-fifth-message');
                const msg6: string = this.translationService.getValue('catches-and-sales.transportation-page-generate-missing-pages-permission-sixth-message');

                message = `${msg1} ${currentStartPage} ${msg2} ${pageToAdd} ${msg3} ${diff} ${msg4}.\n\n${msg5} ${diff} ${msg6}.`;
            }
            else {
                const msg1: string = this.translationService.getValue('catches-and-sales.transportation-page-generate-missing-pages-permission-first-message');
                const msg2: string = this.translationService.getValue('catches-and-sales.transportation-page-generate-missing-pages-permission-second-message');
                const msg3: string = this.translationService.getValue('catches-and-sales.transportation-page-generate-missing-pages-permission-third-message');
                const msg4: string = this.translationService.getValue('catches-and-sales.transportation-page-generate-missing-pages-permission-forth-message');
                const msg5: string = this.translationService.getValue('catches-and-sales.transportation-page-generate-missing-pages-permission-fifth-message');
                const msg6: string = this.translationService.getValue('catches-and-sales.transportation-page-generate-missing-pages-permission-sixth-message');

                message = `${msg1} ${lastUsedPageNum} ${msg2} ${pageToAdd} ${msg3} ${diff} ${msg4}.\n\n${msg5} ${diff} ${msg6}.`;
            }

            // button label

            const btnMsg1: string = this.translationService.getValue('catches-and-sales.ship-log-book-page-permit-generate-missing-pages-first-part');
            const btnMsg2: string = this.translationService.getValue('catches-and-sales.ship-log-book-page-permit-generate-missing-pages-second-part');

            this.confirmDialog.open({
                title: this.translationService.getValue('catches-and-sales.ship-log-book-page-generate-missing-pages-permission-dialog-title'),
                message: message,
                okBtnLabel: `${btnMsg1} ${diff} ${btnMsg2}`,
                okBtnColor: 'warn'
            }).subscribe({
                next: (ok: boolean | undefined) => {
                    this.hasMissingPagesRangePermission = ok ?? false;

                    if (this.hasMissingPagesRangePermission) {
                        this.addShipLogBookPage(dialogClose!); // start add method again
                    }
                }
            });
        }
        else if (error?.code === ErrorCode.CannotAddEditPageForShipUnder10M) {
            let msg: string = '';
            const hasNoUnloadedCatch: boolean = this.form.get('noCatchUnloadedControl')!.value ?? false;

            if (hasNoUnloadedCatch) {
                const msg1: string = this.translationService.getValue('catches-and-sales.ship-page-cannot-add-for-chosen-trip-end-date');
                const msg2: string = this.translationService.getValue('catches-and-sales.ship-page-days-have-past-from-previous-month');
                const msg3: string = this.translationService.getValue('catches-and-sales.ship-page-to-add-page-after-locked-period-contanct-admin');
                msg = `${msg1} ${this.lockShipLogBookPeriods.lockShipUnder10MLogBookAfterDays} ${msg2}. ${msg3}.`;
            }
            else {
                const msg1: string = this.translationService.getValue('catches-and-sales.ship-page-cannot-add-for-chosen-unloading-date');
                const msg2: string = this.translationService.getValue('catches-and-sales.ship-page-days-have-past-from-previous-month');
                const msg3: string = this.translationService.getValue('catches-and-sales.ship-page-to-add-page-after-locked-period-contanct-admin');
                msg = `${msg1} ${this.lockShipLogBookPeriods.lockShipUnder10MLogBookAfterDays} ${msg2}. ${msg3}.`;
            }

            this.snackbar.open(msg, undefined, {
                duration: RequestProperties.DEFAULT.showExceptionDurationErr,
                panelClass: RequestProperties.DEFAULT.showExceptionColorClassErr
            });
        }
        else if (error?.code === ErrorCode.CannotAddEditPageForShip10M12M) {
            let msg: string = '';
            const hasNoUnloadedCatch: boolean = this.form.get('noCatchUnloadedControl')!.value ?? false;

            if (hasNoUnloadedCatch) {
                const msg1: string = this.translationService.getValue('catches-and-sales.ship-page-cannot-add-for-chosen-trip-end-date');
                const msg2: string = this.translationService.getValue('catches-and-sales.ship-page-hours-have-past-from-trip-end-date');
                const msg3: string = this.translationService.getValue('catches-and-sales.ship-page-to-add-page-after-locked-period-contanct-admin');
                msg = `${msg1} ${this.lockShipLogBookPeriods.lockShip10M12MLogBookAfterHours} ${msg2}. ${msg3}.`;
            }
            else {
                const msg1: string = this.translationService.getValue('catches-and-sales.ship-page-cannot-add-for-chosen-unloading-date');
                const msg2: string = this.translationService.getValue('catches-and-sales.ship-page-hours-have-past-from-unloading');
                const msg3: string = this.translationService.getValue('catches-and-sales.ship-page-to-add-page-after-locked-period-contanct-admin');
                msg = `${msg1} ${this.lockShipLogBookPeriods.lockShip10M12MLogBookAfterHours} ${msg2}. ${msg3}.`;
            }

            this.snackbar.open(msg, undefined, {
                duration: RequestProperties.DEFAULT.showExceptionDurationErr,
                panelClass: RequestProperties.DEFAULT.showExceptionColorClassErr
            });
        }
        else if (error?.code === ErrorCode.CannotAddEditPageForShipOver12M) {
            let msg: string = '';
            const hasNoUnloadedCatch: boolean = this.form.get('noCatchUnloadedControl')!.value ?? false;

            if (hasNoUnloadedCatch) {
                const msg1: string = this.translationService.getValue('catches-and-sales.ship-page-cannot-add-for-chosen-trip-end-date');
                const msg2: string = this.translationService.getValue('catches-and-sales.ship-page-hours-have-past-from-trip-end-date');
                const msg3: string = this.translationService.getValue('catches-and-sales.ship-page-to-add-page-after-locked-period-contanct-admin');
                msg = `${msg1} ${this.lockShipLogBookPeriods.lockShipOver12MLogBookAfterHours} ${msg2}. ${msg3}.`;
            }
            else {
                const msg1: string = this.translationService.getValue('catches-and-sales.ship-page-cannot-add-for-chosen-unloading-date');
                const msg2: string = this.translationService.getValue('catches-and-sales.ship-page-hours-have-past-from-unloading');
                const msg3: string = this.translationService.getValue('catches-and-sales.ship-page-to-add-page-after-locked-period-contanct-admin');
                msg = `${msg1} ${this.lockShipLogBookPeriods.lockShipOver12MLogBookAfterHours} ${msg2}. ${msg3}.`;
            }

            this.snackbar.open(msg, undefined, {
                duration: RequestProperties.DEFAULT.showExceptionDurationErr,
                panelClass: RequestProperties.DEFAULT.showExceptionColorClassErr
            });
        }
    }

    private getOriginDeclarationCatchRecords(catchRecordFishes: CatchRecordFishDTO[]): OriginDeclarationFishDTO[] {
        const originDeclarationFishes: OriginDeclarationFishDTO[] = [];
        const defaultCatchFishPresentation: NomenclatureDTO<number> | undefined = this.catchPresentations.find(x => x.code === FishPresentationCodesEnum[DEFAULT_PRESENTATION_CODE] && x.isActive);
        const defaultCatchFishState: NomenclatureDTO<number> | undefined = this.catchStates.find(x => x.code === FishCatchStateCodesEnum[DEFAULT_CATCH_STATE_CODE] && x.isActive);
        const defaultCatchFishPreservation: NomenclatureDTO<number> | undefined = this.catchPreservations.find(x => x.code === FishPreservationCodesEnum[DEFAULT_PRESERVATION_CODE] && x.isActive);

        for (const catchRecordFish of catchRecordFishes) {
            const quantityForUnloading: number = (catchRecordFish.quantityKg ?? 0) - (catchRecordFish.unloadedQuantityKg ?? 0) - (catchRecordFish.unloadedInOtherTripQuantityKg ?? 0);
            catchRecordFish.unloadedQuantityKg = quantityForUnloading;

            if (catchRecordFish.unloadedQuantityKg < (catchRecordFish.unloadedInOtherTripQuantityKg ?? 0)) {
                catchRecordFish.unloadedQuantityKg = catchRecordFish.unloadedInOtherTripQuantityKg ?? 0;
            }

            originDeclarationFishes.push(
                new OriginDeclarationFishDTO({
                    fishId: catchRecordFish.fishId,
                    fishName: catchRecordFish.fishName,
                    quantityKg: quantityForUnloading,
                    unloadedProcessedQuantityKg: quantityForUnloading,
                    catchZone: catchRecordFish.catchZone,
                    catchQuadrantId: catchRecordFish.catchQuadrantId,
                    catchQuadrant: catchRecordFish.catchQuadrant,
                    catchSizeId: catchRecordFish.catchSizeId,
                    isActive: catchRecordFish.isActive,
                    catchFishPresentationId: defaultCatchFishPresentation?.value,
                    catchFishStateId: defaultCatchFishState?.value,
                    catchFishPreservationId: defaultCatchFishPreservation?.value,
                    isProcessedOnBoard: false,
                    fromPreviousTrip: false,
                    isValid: true
                })
            );
        }

        return originDeclarationFishes;
    }

    private recalculateUnloadedQuantities(): void {
        const catchRecordFishes: CatchRecordFishDTO[][] = this.catchRecords.map(x => (x.catchRecordFishes ?? []).filter(x => x.isActive)) as CatchRecordFishDTO[][];
        const fishesFlattened: CatchRecordFishDTO[] = ([] as CatchRecordFishDTO[]).concat(...catchRecordFishes);

        for (const catchRecordFish of fishesFlattened) { // update each quantity
            catchRecordFish.unloadedQuantityKg = this.declarationOfOriginCatchRecords
                .filter(x => x.isActive)
                .reduce((sum: number, current: OriginDeclarationFishDTO) => sum + (current.quantityKg ?? 0), 0);
        }
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
        }, '1300px');
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
            const newCatchRecordFish = new CatchRecordFishDTO(JSON.parse(JSON.stringify(record))); // copy catch record data
            newCatchRecordFish.unloadedQuantityKg = 0;
            newCatchRecordFish.unloadedInOtherTripQuantityKg = 0;

            catchRecord.catchRecordFishes.push(newCatchRecordFish);
        }

        return catchRecord;
    }

    private getPossibleCatchRecordFishesForOriginDeclaration(): CatchRecordFishDTO[] {
        const catchRecordFishes: CatchRecordFishDTO[][] = this.catchRecords.map(x => (x.catchRecordFishes ?? []).filter(x => x.isActive));
        const fishesFlattened: CatchRecordFishDTO[] = ([] as CatchRecordFishDTO[]).concat(...catchRecordFishes);

        return fishesFlattened;
    }

    private setDeclarationOfOriginHasCatchFromPreviousTripFlag(): void {
        this.declarationOfOriginHasCatchFromPreviousTrip = this.declarationOfOriginCatchRecords.some(x => x.fromPreviousTrip);
    }

    private onFishingGearRegisterChanged(value: FishingGearRegisterNomenclatureDTO | string | undefined): void {
        if (value instanceof FishingGearRegisterNomenclatureDTO) {
            const fishingGearsCount: number | undefined = this.form.get('fishingGearCountControl')!.value;
            if (fishingGearsCount === null || fishingGearsCount === undefined) {
                this.form.get('fishingGearCountControl')!.setValue(value.gearCount);
            }

            if (value.hasHooks === true) {
                this.showHooksCountField = true;
                this.form.get('fishingGearHookCountControl')!.setValidators([Validators.required, TLValidators.number(0, undefined, 0)]);
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
                this.form.get('partnerShipControl')!.updateValueAndValidity({ emitEvent: false });

                if (this.viewMode) {
                    this.form.get('partnerShipControl')!.disable();
                }
            }
            else {
                this.showPartnerShipField = false;
                this.form.get('partnerShipControl')!.clearValidators();
            }

            if (value.netEyeSize !== undefined && value.netEyeSize !== null) {
                this.form.get('fishingGearNetEyeSizeControl')!.setValue(value.netEyeSize);
            }
        }
        else {
            if (value === null || value === undefined) {
                this.form.get('fishingGearCountControl')!.reset();
                this.form.get('fishingGearHookCountControl')!.reset();
                this.form.get('fishingGearNetEyeSizeControl')!.reset();
            }

            this.showHooksCountField = false;
            this.showPartnerShipField = false;
        }
    }

    private onFishTripStartDateTimeChanged(startDate: Moment | null | undefined): void {
        if (startDate !== null && startDate !== undefined && startDate.isValid()) {
            if (!this.viewMode) {
                this.form.get('daysAtSeaCountControl')!.setValue(undefined);
                this.form.get('fishTripEndDateTimeControl')!.setValue(startDate);
                this.form.get('unloadDateTimeControl')!.setValue(startDate);
            }
        }

        this.form.get('fishTripEndDateTimeControl')!.updateValueAndValidity({ emitEvent: false });
    }

    private onUnloadDateTimeChanged(unloadDate: Moment | null | undefined): void {
        if (!this.viewMode) {
            if (unloadDate !== null && unloadDate !== undefined && unloadDate.isValid()) {
                const fillDate: Date = new Date(unloadDate.toDate());
                this.form.get('fillDateControl')!.setValue(fillDate);
                this.form.get('fillDateControl')!.updateValueAndValidity({ emitEvent: false });
            }
        }
    }

    private onDeparturePortChanged(departurePort: NomenclatureDTO<number> | string | null | undefined): void {
        if (departurePort !== null && departurePort !== undefined && departurePort instanceof NomenclatureDTO) {
            const arrivalPort: NomenclatureDTO<number> | string | null | undefined = this.form.get('arrivalPortControl')!.value;
            if (arrivalPort === null || arrivalPort === undefined || typeof arrivalPort === 'string') {
                this.form.get('arrivalPortControl')!.setValue(departurePort);
            }

            const noUnloading: boolean = this.form.get('noCatchUnloadedControl')!.value ?? false;
            if (!noUnloading) {
                const unloadingPort: NomenclatureDTO<number> | string | null | undefined = this.form.get('unloadPortControl')!.value;

                if (unloadingPort === null || unloadingPort === undefined || typeof unloadingPort === 'string') {
                    this.form.get('unloadPortControl')!.setValue(departurePort);
                }
            }
        }
    }

    private onFishTripEndDateTimeChanged(endDate: Moment | null | undefined): void {
        if (endDate !== null && endDate !== undefined && endDate.isValid()) {
            const startDate: Moment | null | undefined = this.form.get('fishTripStartDateTimeControl')!.value;
            if (startDate !== null && startDate !== undefined && startDate.isValid()) {
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

        this.form.get('fishTripStartDateTimeControl')!.updateValueAndValidity({ emitEvent: false });
        this.form.get('unloadDateTimeControl')!.updateValueAndValidity({ emitEvent: false });
    }

    private onNoCatchUnloadedChanged(value: boolean): void {
        this.changeUnloadingControlsValidators(value ?? false);
        this.form.updateValueAndValidity({ onlySelf: true, emitEvent: false });

        if (value) { // без разтоварване
            const validators = this.form.get('fishTripEndDateTimeControl')!.validator;
            if (validators !== null && validators !== undefined) {
                this.form.get('fishTripEndDateTimeControl')!.setValidators([validators, this.checkDateValidityVsLockPeriodsValidator()]);
            }
            else {
                this.form.get('fishTripEndDateTimeControl')!.setValidators(this.checkDateValidityVsLockPeriodsValidator());
            }

            this.form.get('fishTripEndDateTimeControl')!.updateValueAndValidity({ emitEvent: false });
        }
        else { // има разтоварване
            this.setFishTripEndDateTimeControlInitialValidators();
        }
    }

    private setFishTripEndDateTimeControlInitialValidators(): void {
        this.form.get('fishTripEndDateTimeControl')!.setValidators([
            Validators.required,
            TLValidators.minDate(this.form.get('fishTripStartDateTimeControl')!),
            this.endDateDifferentFromStartDateValidator()
        ]);

        this.form.get('fishTripEndDateTimeControl')!.markAsPending({ emitEvent: false });
        this.form.get('fishTripEndDateTimeControl')!.updateValueAndValidity({ emitEvent: false });
    }

    private setFishTripStartDateTimeControlValidators(): void {
        this.form.get('fishTripStartDateTimeControl')!.setValidators([
            Validators.required,
            TLValidators.maxDate(this.form.get('fishTripEndDateTimeControl')!)
        ]);

        this.form.get('fishTripStartDateTimeControl')!.markAsPending({ emitEvent: false });
        this.form.get('fishTripStartDateTimeControl')!.updateValueAndValidity({ emitEvent: false });
    }

    private setUnloadDateTimeControlValidators(): void {
        this.form.get('unloadDateTimeControl')!.setValidators([
            Validators.required,
            TLValidators.minDate(this.form.get('fishTripEndDateTimeControl')!),
            this.checkDateValidityVsLockPeriodsValidator()
        ]);

        this.form.get('unloadDateTimeControl')!.markAsPending({ emitEvent: false });
        this.form.get('unloadDateTimeControl')!.updateValueAndValidity({ emitEvent: false });

        const unloadDateTime: Moment | null | undefined = this.form.get('unloadDateTimeControl')!.value;
        if (unloadDateTime !== null && unloadDateTime !== undefined && unloadDateTime.isValid() === false) {
            this.form.get('unloadDateTimeControl')!.setValue(undefined);
        }
    }

    private changeUnloadingControlsValidators(noUnloading: boolean): void {
        if (noUnloading === true) {
            this.form.get('unloadDateTimeControl')!.setValidators([
                TLValidators.minDate(this.form.get('fishTripEndDateTimeControl')!),
            ]);
            this.form.get('unloadPortControl')!.clearValidators();

            this.form.get('unloadDateTimeControl')!.setValue(undefined);
            this.form.get('unloadPortControl')!.setValue(undefined);
        }
        else {
            this.form.get('unloadDateTimeControl')!.setValidators([
                Validators.required,
                TLValidators.minDate(this.form.get('fishTripEndDateTimeControl')!),
                this.checkDateValidityVsLockPeriodsValidator()
            ]);
            this.form.get('unloadPortControl')!.setValidators(Validators.required);
        }

        this.form.get('unloadDateTimeControl')!.markAsPending({ emitEvent: false });
        this.form.get('unloadPortControl')!.markAsPending({ emitEvent: false });

        this.form.get('unloadDateTimeControl')!.updateValueAndValidity({ emitEvent: false });
        this.form.get('unloadPortControl')!.updateValueAndValidity({ emitEvent: false });

        const unloadDateTime: Moment | null | undefined = this.form.get('unloadDateTimeControl')!.value;
        if (unloadDateTime !== null && unloadDateTime !== undefined && unloadDateTime.isValid() === false) {
            this.form.get('unloadDateTimeControl')!.setValue(undefined);
        }

        if (this.viewMode) {
            this.form.get('unloadDateTimeControl')!.disable();
            this.form.get('unloadPortControl')!.disable();
        }
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
            const quantity: number = catchRecordsGroupedByFishType[fishGroupId].reduce((sum, current) => sum + (current.quantityKg! - (current.unloadedInOtherTripQuantityKg ?? 0)), 0);
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

    private allOriginDeclarationFishTransboardedIfMarkedValidator(): ValidatorFn {
        return (form: AbstractControl): ValidationErrors | null => {
            if (form === null || form === undefined) {
                return null;
            }

            if (!this.allCatchIsTransboardedValue) {
                return null;
            }
            else {
                const originDeclarationFishes: OriginDeclarationFishDTO[] = (this.declarationOfOriginCatchRecordsTable.rows as OriginDeclarationFishDTO[]) ?? [];
                if (originDeclarationFishes.some(x => x.transboradDateTime === null || x.transboradDateTime === undefined)) { // Ако има улов, който няма данни за трансбордиране
                    return { 'noTransboardDataForTransboardedCatch': true };
                }
                else {
                    return null;
                }
            }
        }
    }

    private requiredOriginDeclarationIfCatchOnBoard(): ValidatorFn {
        return (form: AbstractControl): ValidationErrors | null => {
            if (form === null || form === undefined) {
                return null;
            }

            const noCatchUnloaded: boolean = form.get('noCatchUnloadedControl')!.value ?? false;

            if (noCatchUnloaded === true) {
                const hasUnloadedFishesInfo: boolean =
                    this.declarationOfOriginCatchRecords !== null
                    && this.declarationOfOriginCatchRecords !== undefined
                    && this.declarationOfOriginCatchRecords.some(x => x.isActive);

                if (hasUnloadedFishesInfo === true) {
                    return { 'originDeclarationFishesNotNeeded': true };
                }
                else {
                    return null;
                }
            }

            if (this.catchRecords === null || this.catchRecords === undefined) {
                return null;
            }

            const tripHasCatchFishes: boolean = this.catchRecords.some(x =>
                x.isActive
                && x.catchRecordFishes !== null
                && x.catchRecordFishes !== undefined
                && x.catchRecordFishes.some(x => x.isActive)
            );

            if (tripHasCatchFishes === true) {
                const hasUnloadedFishesInfo: boolean =
                    this.declarationOfOriginCatchRecords !== null
                    && this.declarationOfOriginCatchRecords !== undefined
                    && this.declarationOfOriginCatchRecords.some(x => x.isActive);

                if (hasUnloadedFishesInfo === true) {
                    return null;
                }
                else {
                    return { 'originDeclarationFishesRequired': true };
                }
            }
            else {
                return null;
            }

            return null;
        }
    }

    private checkDateValidityVsLockPeriodsValidator(): ValidatorFn {
        return (control: AbstractControl): ValidationErrors | null => {
            if (control === null || control === undefined) {
                return null;
            }

            if (this.form === null || this.form === undefined) {
                return null;
            }

            if (control.value === null || control.value === undefined) {
                return null;
            }

            const date: Date = (control.value as Moment).toDate();
            const now: Date = new Date();

            if (this.model === null || this.model === undefined) {
                return null;
            }

            const logBookId: number = this.model.logBookId!;
            const logBookTypeId: number = this.model.logBookTypeId!;

            const difference: DateDifference | undefined = DateUtils.getDateDifference(date, now);

            if (difference === null || difference === undefined) {
                return null;
            }

            if (difference.minutes === 0 && difference.hours === 0 && difference.days === 0) {
                return null;
            }

            const ship = ShipsUtils.get(this.ships, this.model.shipId!);

            if (CatchesAndSalesUtils.pageHasLogBookPageDateLockedViaDaysAfterMonth(date, now, this.lockShipLogBookPeriods.lockPageAfterDays)
                && !CatchesAndSalesUtils.checkIfPageDateIsUnlocked(this.logBookPageEditExceptions, this.currentUserId, logBookTypeId, logBookId, date, now)
            ) { // този дневник го няма в изключение за избраната дата за потребител и/или тип дневник, и/или дневник

                return {
                    logBookPageDatePeriodLocked: {
                        shipLength: ship.totalLength!,
                        lockedPeriod: this.lockShipLogBookPeriods.lockShipUnder10MLogBookAfterDays,
                        periodType: 'days-after-month'
                    }
                };
            }

            //само предупреждения
            if (ship.totalLength! < 10) { // При кораби под 10 метра
                if (CatchesAndSalesUtils.pageHasLogBookPageDateLockedViaDaysAfterMonth(date, now, this.lockShipLogBookPeriods.lockShipUnder10MLogBookAfterDays)) {
                    return {
                        logBookPageDateLocked: {
                            shipLength: ship.totalLength!,
                            lockedPeriod: this.lockShipLogBookPeriods.lockShipUnder10MLogBookAfterDays,
                            periodType: 'days-after-month'
                        }
                    };
                }
            }
            else if (ship.totalLength! >= 10 && ship.totalLength! <= 12) { // При кораби от 10 до 12 метра
                const hoursDifference: number = CatchesAndSalesUtils.convertDateDifferenceToHours(difference);
                if (hoursDifference > this.lockShipLogBookPeriods.lockShip10M12MLogBookAfterHours) {
                    return {
                        logBookPageDateLocked: {
                            shipLength: ship.totalLength!,
                            lockedPeriod: this.lockShipLogBookPeriods.lockShip10M12MLogBookAfterHours,
                            periodType: 'hours'
                        }
                    }
                }
            }
            else if (ship.totalLength! > 12) { // При кораби над 12 метра
                const hoursDifference: number = CatchesAndSalesUtils.convertDateDifferenceToHours(difference);
                if (hoursDifference > this.lockShipLogBookPeriods.lockShipOver12MLogBookAfterHours) {
                    return {
                        logBookPageDateLocked: {
                            shipLength: ship.totalLength!,
                            lockedPeriod: this.lockShipLogBookPeriods.lockShipOver12MLogBookAfterHours,
                            periodType: 'hours'
                        }
                    };
                }
            }

            return null;
        }
    }

    private gearEntryTimeValidator(): ValidatorFn {
        return (control: AbstractControl): ValidationErrors | null => {
            if (control === null || control === undefined) {
                return null;
            }

            if (this.form === null || this.form === undefined) {
                return null;
            }

            if (control.value === null || control.value === undefined) {
                return null;
            }

            if (this.catchRecords === null || this.catchRecords === undefined || this.catchRecords.length === 0) {
                return null;
            }

            for (const catchRecord of this.catchRecords) {
                const invalidRows = this.catchRecords.filter(x => {
                    if (x.isActive !== false
                        && x.gearEntryTime?.getFullYear() === catchRecord.gearEntryTime?.getFullYear()
                        && x.gearEntryTime?.getDate() === catchRecord.gearEntryTime?.getDate()
                        && x.gearEntryTime?.getHours() === catchRecord.gearEntryTime?.getHours()
                        && x.gearEntryTime?.getMinutes() === catchRecord.gearEntryTime?.getMinutes()
                    ) {
                        return true;
                    }

                    return false;
                });

                if (invalidRows.length > 1) {
                    return { 'uniqueGearEntryTime': true };
                }
            }

            return null;
        }
    }

    private endDateDifferentFromStartDateValidator(): ValidatorFn {
        return (control: AbstractControl): ValidationErrors | null => {
            if (control.value !== null && control.value !== undefined) {

                const startDate: Moment | null | undefined = this.form.get('fishTripStartDateTimeControl')!.value;

                if (startDate !== null && startDate !== undefined && startDate.isValid) {
                    const endDate: Moment = control.value;

                    if (endDate.isSame(startDate)) {
                        return { 'endDateSameAsStartDate': true };
                    }
                }
            }

            return null;
        }
    }

    private recalculateDeclarationOfOriginCatchRecordsQuantitySums(): void {
        const recordsGroupedByFishType: Record<number, OriginDeclarationFishDTO[]> = CommonUtils.groupBy(this.declarationOfOriginCatchRecords.filter(x => x.isActive), x => x.fishId!);
        this.declarationOfOriginCatchRecordsQuantities.clear();

        for (const fishGroupId in recordsGroupedByFishType) {
            const quantity = recordsGroupedByFishType[fishGroupId].reduce((sum, current) => sum + current.quantityKg!, 0);
            this.declarationOfOriginCatchRecordsQuantities.set(Number(fishGroupId), quantity);
        }
    }

    private recalculateCatchRecordsQuantitySums(): void {
        const catchRecordFishes: CatchRecordFishDTO[] = [];

        for (const catchRecord of this.catchRecords.filter(x => x.isActive)) {
            catchRecord.catchRecordFishes?.map(x => catchRecordFishes.push(x));
        }

        const recordsGroupedByFishType: Record<number, CatchRecordFishDTO[]> = CommonUtils.groupBy(catchRecordFishes, x => x.fishId!);
        this.catchRecordQuantities.clear();

        for (const fishGroupId in recordsGroupedByFishType) {
            const quantity = recordsGroupedByFishType[fishGroupId].reduce((sum, current) => sum + current.quantityKg!, 0);
            this.catchRecordQuantities.set(Number(fishGroupId), quantity);
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
