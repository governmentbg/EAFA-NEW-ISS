import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';

import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { MarketCatchTableParams } from '../market-catches-table/models/market-catch-table-params';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { CommonNomenclatures } from '@app/services/common-app/common-nomenclatures.service';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { InspectionsService } from '@app/services/administration-app/inspections.service';
import { DeclarationLogBookTypeEnum } from '@app/enums/declaration-log-book-type.enum';
import { InspectedDeclarationCatchDTO } from '@app/models/generated/dtos/InspectedDeclarationCatchDTO';
import { forkJoin } from 'rxjs';
import { TLError } from '@app/shared/components/input-controls/models/tl-error.model';
import { ValidityCheckerGroupDirective } from '@app/shared/directives/validity-checker/validity-checker-group.directive';
import { InspectionLogBookPageNomenclatureDTO } from '@app/models/generated/dtos/InspectionLogBookPageNomenclatureDTO';
import { ShipNomenclatureDTO } from '@app/models/generated/dtos/ShipNomenclatureDTO';
import { VesselDuringInspectionDTO } from '@app/models/generated/dtos/VesselDuringInspectionDTO';
import { InspectedLogBookPageDataDTO } from '@app/models/generated/dtos/InspectedLogBookPageDataDTO';
import { InspectionLogBookPageDTO } from '@app/models/generated/dtos/InspectionLogBookPageDTO';

@Component({
    selector: 'edit-market-catch',
    styleUrls: ['./edit-market-catch.component.scss'],
    templateUrl: './edit-market-catch.component.html',
})
export class EditMarketCatchComponent implements OnInit, AfterViewInit, IDialogComponent {
    public form!: FormGroup;

    protected model: InspectionLogBookPageDTO = new InspectionLogBookPageDTO();

    public hasCatchType: boolean = true;
    public aquacultureRegistered: boolean = true;
    public hasUndersizedCheck: boolean = false;
    public hasDeclaration: boolean = false;
    public readOnly: boolean = false;
    public showTurbotControl: boolean = false;
    public isFirstSaleInspection: boolean = false;
    public showAdmissionPageControls: boolean = false;
    public showTransportationPageControls: boolean = false;
    public showShipPageControls: boolean = false;

    public permitTypeSelected: DeclarationLogBookTypeEnum | undefined;
    public pageDateLabel: string | undefined;

    public readonly declarationLogBookTypeEnum = DeclarationLogBookTypeEnum;

    public ships: ShipNomenclatureDTO[] = [];
    public countries: NomenclatureDTO<number>[] = [];
    public vesselTypes: NomenclatureDTO<number>[] = [];
    public fishes: NomenclatureDTO<number>[] = [];
    public types: NomenclatureDTO<number>[] = [];
    public presentations: NomenclatureDTO<number>[] = [];
    public permitTypes: NomenclatureDTO<DeclarationLogBookTypeEnum>[] = [];
    public aquacultures: NomenclatureDTO<number>[] = [];
    public turbotSizeGroups: NomenclatureDTO<number>[] = [];
    public declarationPages: InspectionLogBookPageNomenclatureDTO[] = [];

    public logBookPageProductQuantities: Map<number, number> = new Map<number, number>();
    public logBookPageFishes: InspectedDeclarationCatchDTO[] = [];
    public fishErrors: TLError[] = [];

    @ViewChild(ValidityCheckerGroupDirective)
    private validityCheckerGroup!: ValidityCheckerGroupDirective;

    private shipLogBookPageId: number | undefined;
    private transportationLogBookPageId: number | undefined;
    private admissionLogBookPageId: number | undefined;

    private readonly service: InspectionsService;
    private readonly nomenclatures: CommonNomenclatures;
    private readonly translate: FuseTranslationLoaderService;

    public constructor(
        service: InspectionsService,
        nomenclatures: CommonNomenclatures,
        translate: FuseTranslationLoaderService
    ) {
        this.buildForm();

        this.service = service;
        this.nomenclatures = nomenclatures;
        this.translate = translate;

        this.permitTypes = [
            new NomenclatureDTO({
                value: DeclarationLogBookTypeEnum.FirstSaleLogBook,
                displayName: translate.getValue('inspections.market-first-sale-log-book'),
                isActive: true,
            }),
            new NomenclatureDTO({
                value: DeclarationLogBookTypeEnum.AdmissionLogBook,
                displayName: translate.getValue('inspections.market-admission-log-book'),
                isActive: true,
            }),
            new NomenclatureDTO({
                value: DeclarationLogBookTypeEnum.TransportationLogBook,
                displayName: translate.getValue('inspections.market-transport-log-book'),
                isActive: true,
            }),
            new NomenclatureDTO({
                value: DeclarationLogBookTypeEnum.ShipLogBook,
                displayName: translate.getValue('inspections.market-ship-log-book'),
                isActive: true,
            }),
            new NomenclatureDTO({
                value: DeclarationLogBookTypeEnum.AquacultureLogBook,
                displayName: translate.getValue('inspections.market-aquaculture-log-book'),
                isActive: true,
            }),
            new NomenclatureDTO({
                value: DeclarationLogBookTypeEnum.Invoice,
                displayName: translate.getValue('inspections.market-other'),
                isActive: true,
            }),
            new NomenclatureDTO({
                value: DeclarationLogBookTypeEnum.NNN,
                displayName: translate.getValue('inspections.market-ship-nnn'),
                isActive: true,
            }),
        ];
    }

    public async ngOnInit(): Promise<void> {
        if (this.readOnly) {
            this.form.disable();
        }

        const nomenclatureTables = await forkJoin([
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.Ships, this.nomenclatures.getShips.bind(this.nomenclatures), false),
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.Countries, this.nomenclatures.getCountries.bind(this.nomenclatures), false),
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.VesselTypes, this.nomenclatures.getVesselTypes.bind(this.nomenclatures), false),
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.TurbotSizeGroups, this.nomenclatures.getTurbotSizeGroups.bind(this.nomenclatures), false),
            this.service.getAquacultures()
        ]).toPromise();

        this.ships = nomenclatureTables[0];
        this.countries = nomenclatureTables[1];
        this.vesselTypes = nomenclatureTables[2];
        this.turbotSizeGroups = nomenclatureTables[3];
        this.aquacultures = nomenclatureTables[4];

        if (this.model !== undefined && this.model !== null) {
            this.fillLogBookPageData();
        }

        setTimeout(() => {
            this.fillForm();
        });
    }

    public ngAfterViewInit(): void {
        this.form.get('permitControl')!.valueChanges.subscribe({
            next: (value: NomenclatureDTO<DeclarationLogBookTypeEnum> | undefined) => {
                if (value !== undefined && value !== null) {
                    this.permitTypeSelected = value?.value;

                    this.getPageDateLabel();
                    this.pullDeclarations();

                    if (this.permitTypeSelected === DeclarationLogBookTypeEnum.AquacultureLogBook) {
                        this.form.get('aquacultureRegisteredControl')!.setValue(this.aquacultureRegistered);
                    }
                    else {
                        this.form.get('aquacultureControl')!.disable();
                        this.form.get('aquacultureTextControl')!.disable();
                    }

                    if (this.permitTypeSelected === DeclarationLogBookTypeEnum.AquacultureLogBook || this.permitTypeSelected === DeclarationLogBookTypeEnum.Invoice || this.permitTypeSelected === DeclarationLogBookTypeEnum.NNN) {
                        this.form.get('shipControl')!.disable();
                    }
                    else if (!this.readOnly) {
                        this.form.get('shipControl')!.enable();
                    }
                }
                else {
                    this.declarationPages = [];

                    this.form.get('pageNumberControl')!.setValue(undefined);
                    this.form.get('pageNumberControl')!.updateValueAndValidity({ onlySelf: true });

                    this.form.get('shipControl')!.reset();
                    this.form.get('shipControl')!.disable();
                }
            }
        });

        this.form.get('pageNumberControl')!.valueChanges.subscribe({
            next: (value: NomenclatureDTO<number> | string | undefined) => {
                this.clearLogBookPageData();

                if (value !== undefined && value !== null) {
                    if (typeof value === 'string') {
                        this.form.get('pageDateControl')!.enable();
                        this.form.get('logBookNumberControl')!.enable();
                    }
                    else if (value instanceof InspectionLogBookPageNomenclatureDTO) {
                        this.form.get('pageDateControl')!.setValue(value.logBookPageDate);
                        this.form.get('logBookNumberControl')!.setValue(value.logBookNum);

                        this.getLogBookPageDataFromLogBookPageId(value.value!);
                        this.disablePageControls();
                    }
                }
                else {
                    this.form.get('pageDateControl')!.setValue(undefined);
                    this.form.get('logBookNumberControl')!.setValue(undefined);
                }

                if (this.readOnly === true) {
                    this.disablePageControls();
                }
            }
        });

        this.form.get('aquacultureRegisteredControl')!.valueChanges.subscribe({
            next: (value: boolean) => {
                this.aquacultureRegistered = value;

                if (!this.readOnly) {
                    if (value) {
                        this.form.get('aquacultureControl')!.setValidators(Validators.required);
                        this.form.get('aquacultureControl')!.enable();
                        this.form.get('aquacultureTextControl')!.disable();
                        this.form.get('pageDateControl')!.disable();
                        this.form.get('logBookNumberControl')!.disable();
                    }
                    else {
                        this.declarationPages = [];
                        this.form.get('aquacultureTextControl')!.setValidators(Validators.required);
                        this.form.get('aquacultureControl')!.disable();
                        this.form.get('aquacultureTextControl')!.enable();
                        this.form.get('pageDateControl')!.enable();
                        this.form.get('logBookNumberControl')!.enable();
                    }
                }
            }
        });

        this.form.get('aquacultureControl')!.valueChanges.subscribe({
            next: (value: NomenclatureDTO<number> | undefined) => {
                if (value !== undefined && value !== null) {
                    this.pullDeclarations();
                }
            }
        });
    }

    public setData(data: MarketCatchTableParams, wrapperData: DialogWrapperData): void {
        if (data.model !== undefined && data.model !== null) {
            this.model = data.model;
        }

        this.readOnly = data.readOnly;
        this.fishes = data.fishes;
        this.types = data.types;
        this.hasUndersizedCheck = data.hasUndersizedCheck;
        this.presentations = data.presentations;
        this.hasCatchType = data.hasCatchType;
    }

    public dialogButtonClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public saveBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        if (this.readOnly) {
            dialogClose(this.model);
        }
        else {
            this.form.markAllAsTouched();
            this.validityCheckerGroup.validate();

            if (this.form.valid) {
                this.fillModel();
                CommonUtils.sanitizeModelStrings(this.model);

                dialogClose(this.model);
            }
        }
    }

    public cancelBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public generateInspectedProducts(): void {
        this.form.get('inspectedCatchesControl')!.setValue(this.logBookPageFishes);
    }

    protected buildForm(): void {
        this.form = new FormGroup({
            permitControl: new FormControl(undefined, [Validators.required]),
            shipControl: new FormControl(undefined),
            aquacultureRegisteredControl: new FormControl(true),
            aquacultureControl: new FormControl(undefined),
            aquacultureTextControl: new FormControl(undefined, Validators.maxLength(4000)),
            invoiceDataControl: new FormControl(undefined, Validators.maxLength(4000)),
            pageNumberControl: new FormControl(undefined),
            logBookNumberControl: new FormControl(undefined),
            pageDateControl: new FormControl(undefined),
            inspectedCatchesControl: new FormControl(undefined),

            shipPageNumberControl: new FormControl(undefined),
            shipPageDateControl: new FormControl(undefined),
            transportationPageNumberControl: new FormControl(undefined),
            transportationPageDateControl: new FormControl(undefined),
            admissionPageNumberControl: new FormControl(undefined),
            admissionPageDateControl: new FormControl(undefined)
        }, this.fishQuantityValidator());

        this.form.get('shipControl')!.disable();
        this.form.get('aquacultureControl')!.disable();
        this.form.get('aquacultureTextControl')!.disable();
    }

    protected fillForm(): void {
        this.permitTypeSelected = this.model.logBookType;

        this.form.get('shipControl')!.setValue(this.model.originShip);

        if (this.model.aquacultureId) {
            this.form.get('aquacultureControl')!.setValue(this.aquacultures.find(f => f.value == this.model.aquacultureId));
        }
        else if (this.model.unregisteredEntityData != undefined && this.model.logBookType === DeclarationLogBookTypeEnum.AquacultureLogBook) {
            this.aquacultureRegistered = false;
            this.form.get('aquacultureRegisteredControl')!.setValue(false);
        }

        if (this.model.logBookType === DeclarationLogBookTypeEnum.Invoice) {
            this.form.get('invoiceDataControl')!.setValue(this.model.unregisteredEntityData);
        }
        else {
            this.form.get('aquacultureTextControl')!.setValue(this.model.unregisteredEntityData);
        }

        this.form.get('inspectedCatchesControl')!.setValue(this.model.inspectionCatchMeasures);
    }

    protected fillModel(): void {
        this.model.originShip = this.form.get('shipControl')!.value;
        this.model.logBookType = this.form.get('permitControl')!.value?.value;
        this.model.aquacultureId = this.form.get('aquacultureControl')!.value?.value;

        this.model.unregisteredEntityData = this.model.logBookType === DeclarationLogBookTypeEnum.Invoice
            ? this.form.get('invoiceDataControl')!.value
            : this.form.get('aquacultureTextControl')!.value;

        this.model.inspectionCatchMeasures = this.form.get('inspectedCatchesControl')!.value;

        this.model.shipLogBookPageId = undefined;
        this.model.transportationLogBookPageId = undefined;
        this.model.admissionLogBookPageId = undefined;
        this.model.firstSaleLogBookPageId = undefined;
        this.model.aquacultureLogBookPageId = undefined;

        const logBookPage: InspectionLogBookPageNomenclatureDTO | string = this.form.get('pageNumberControl')!.value;

        if (typeof logBookPage === 'string') {
            this.model.unregisteredPageNum = logBookPage;
            this.model.unregisteredLogBookNum = this.form.get('logBookNumberControl')!.value;
            this.model.unregisteredPageDate = this.form.get('pageDateControl')!.value;
        }
        else if (logBookPage !== null && logBookPage !== undefined) {
            const page = this.declarationPages.find(x => x.value === logBookPage.value);

            switch (this.permitTypeSelected) {
                case DeclarationLogBookTypeEnum.ShipLogBook:
                    this.model.shipLogBookPageId = this.form.get('pageNumberControl')!.value!.value;
                    this.model.unregisteredPageNum = page?.originDeclarationNum;
                    break;
                case DeclarationLogBookTypeEnum.TransportationLogBook:
                    this.model.transportationLogBookPageId = this.form.get('pageNumberControl')!.value!.value;
                    this.model.shipLogBookPageId = this.shipLogBookPageId;
                    this.model.unregisteredPageNum = page?.logPageNum?.toString();
                    break;
                case DeclarationLogBookTypeEnum.AdmissionLogBook:
                    this.model.admissionLogBookPageId = this.form.get('pageNumberControl')!.value!.value;
                    this.model.shipLogBookPageId = this.shipLogBookPageId;
                    this.model.transportationLogBookPageId = this.transportationLogBookPageId;
                    this.model.unregisteredPageNum = page?.logPageNum?.toString();
                    break;
                case DeclarationLogBookTypeEnum.FirstSaleLogBook:
                    this.model.firstSaleLogBookPageId = this.form.get('pageNumberControl')!.value!.value;
                    this.model.shipLogBookPageId = this.shipLogBookPageId;
                    this.model.transportationLogBookPageId = this.transportationLogBookPageId;
                    this.model.admissionLogBookPageId = this.admissionLogBookPageId;
                    this.model.unregisteredPageNum = page?.logPageNum?.toString();
                    break;
                case DeclarationLogBookTypeEnum.AquacultureLogBook:
                    this.model.aquacultureLogBookPageId = this.form.get('pageNumberControl')!.value!.value;
                    this.model.unregisteredPageNum = page?.logPageNum?.toString();
                    break;
            }

            this.model.unregisteredPageDate = page?.logBookPageDate;
            this.model.unregisteredLogBookNum = page?.logBookNum;
        }
        else {
            this.model.unregisteredLogBookNum = this.form.get('logBookNumberControl')!.value;
            this.model.unregisteredPageDate = this.form.get('pageDateControl')!.value;
            this.model.unregisteredPageNum = undefined;
            this.model.shipLogBookPageId = undefined;
            this.model.transportationLogBookPageId = undefined;
            this.model.admissionLogBookPageId = undefined;
            this.model.firstSaleLogBookPageId = undefined;
            this.model.aquacultureLogBookPageId = undefined;
        }
    }

    private fillLogBookPageData(): void {
        this.form.get('permitControl')!.setValue(this.permitTypes.find(f => f.value === this.model.logBookType));

        if (this.model.logBookType !== null && this.model.logBookType !== undefined
            && this.model.logBookType !== DeclarationLogBookTypeEnum.Invoice
            && this.model.logBookType !== DeclarationLogBookTypeEnum.NNN
        ) {

            if (this.hasLogBookPageId()
                && ((this.model.originShip?.shipId !== null && this.model.originShip?.shipId !== undefined)
                    || (this.model.aquacultureId !== null && this.model.aquacultureId !== undefined))
            ) {
                this.service.getLogBookPages(this.model.logBookType, this.model.originShip?.shipId, this.model.aquacultureId).subscribe({
                    next: (pages: InspectionLogBookPageNomenclatureDTO[]) => {
                        this.declarationPages = pages;
                        const page: InspectionLogBookPageNomenclatureDTO | undefined = pages.find(f => f.value === this.model.shipLogBookPageId);

                        this.form.get('pageNumberControl')!.setValue(page);
                        this.form.get('logBookNumberControl')!.setValue(page?.logBookNum);
                        this.form.get('pageDateControl')!.setValue(page?.logBookPageDate);

                        this.fillPageNumControlsByPageId();

                        this.disablePageControls();
                    }
                });
            }
            else {
                this.form.get('pageNumberControl')!.setValue(this.model.unregisteredPageNum);
                this.form.get('logBookNumberControl')!.setValue(this.model.unregisteredLogBookNum);
                this.form.get('pageDateControl')!.setValue(this.model.unregisteredPageDate);
            }
        }
        else {
            this.form.get('pageNumberControl')!.setValue(this.model.unregisteredPageNum);
            this.form.get('logBookNumberControl')!.setValue(this.model.unregisteredLogBookNum);
            this.form.get('pageDateControl')!.setValue(this.model.unregisteredPageDate);
        }
    }

    private getLogBookPageDataFromLogBookPageId(logBookPageId: number): void {
        if (this.permitTypeSelected !== undefined && this.permitTypeSelected !== null && this.permitTypeSelected !== DeclarationLogBookTypeEnum.NNN && this.permitTypeSelected !== DeclarationLogBookTypeEnum.Invoice) {
            this.service.getInspectedLogBookPageData(logBookPageId, this.permitTypeSelected).subscribe({
                next: (logBookPageData: InspectedLogBookPageDataDTO | undefined) => {
                    if (logBookPageData !== undefined && logBookPageData !== null) {

                        if (logBookPageData.shipLogBookPageId !== undefined
                            && logBookPageData.shipLogBookPageId !== null
                            && (this.permitTypeSelected === DeclarationLogBookTypeEnum.AdmissionLogBook
                                || this.permitTypeSelected === DeclarationLogBookTypeEnum.FirstSaleLogBook
                                || this.permitTypeSelected === DeclarationLogBookTypeEnum.TransportationLogBook)
                        ) {
                            this.showShipPageControls = true;
                            this.form.get('shipPageNumberControl')!.setValue(logBookPageData.shipLogBookPageNumber);
                            this.form.get('shipPageDateControl')!.setValue(logBookPageData.shipPageFillDate);
                        }

                        if (logBookPageData.transportationLogBookPageId !== undefined
                            && logBookPageData.transportationLogBookPageId !== null
                            && (this.permitTypeSelected === DeclarationLogBookTypeEnum.AdmissionLogBook
                                || this.permitTypeSelected === DeclarationLogBookTypeEnum.FirstSaleLogBook)
                        ) {
                            this.showTransportationPageControls = true;
                            this.form.get('transportationPageNumberControl')!.setValue(logBookPageData.transportationLogBookPageNumber);
                            this.form.get('transportationPageDateControl')!.setValue(logBookPageData.transportationPageLoadingDate);
                        }

                        if (logBookPageData.admissionLogBookPageId !== undefined
                            && logBookPageData.admissionLogBookPageId !== null
                            && this.permitTypeSelected === DeclarationLogBookTypeEnum.FirstSaleLogBook
                        ) {
                            this.showAdmissionPageControls = true;
                            this.form.get('admissionPageNumberControl')!.setValue(logBookPageData.admissionLogBookPageNumber);
                            this.form.get('admissionPageDateControl')!.setValue(logBookPageData.admissionPageHandoverDate);
                        }

                        this.shipLogBookPageId = logBookPageData.shipLogBookPageId;
                        this.transportationLogBookPageId = logBookPageData.transportationLogBookPageId;
                        this.admissionLogBookPageId = logBookPageData.admissionLogBookPageId;

                        this.logBookPageFishes = logBookPageData.inspectionCatchMeasures ?? [];
                        this.logBookPageProductQuantities = this.getFishQuantities(this.logBookPageFishes);
                    }
                }
            });
        }
    }

    private pullDeclarations(vessel: VesselDuringInspectionDTO | undefined = undefined): void {
        if (this.permitTypeSelected !== undefined
            && this.permitTypeSelected !== null
            && this.permitTypeSelected !== DeclarationLogBookTypeEnum.Invoice
            && this.permitTypeSelected !== DeclarationLogBookTypeEnum.NNN
        ) {
            let ship: VesselDuringInspectionDTO | undefined = this.form.get('shipControl')!.value;
            const aquaculture: number | undefined = this.form.get('aquacultureControl')!.value?.value;

            if (vessel !== undefined && vessel !== null) {
                ship = vessel;
            }

            if ((ship !== undefined && ship !== null && ship.shipId !== undefined && ship.shipId !== null)
                || (aquaculture !== undefined && aquaculture !== null)
            ) {
                this.service.getLogBookPages(this.permitTypeSelected, ship?.shipId, aquaculture).subscribe({
                    next: (result: InspectionLogBookPageNomenclatureDTO[]) => {
                        if (result !== undefined && result !== null) {
                            this.declarationPages = result;
                        }
                    }
                });
            }
        }
    }

    private disablePageControls(): void {
        this.form.get('logBookNumberControl')!.disable();
        this.form.get('pageDateControl')!.disable();
    }

    private clearLogBookPageData(): void {
        this.hasDeclaration = false;
        this.showTransportationPageControls = false;
        this.showAdmissionPageControls = false;
        this.showShipPageControls = false;

        this.shipLogBookPageId = undefined;
        this.transportationLogBookPageId = undefined;
        this.admissionLogBookPageId = undefined;

        this.logBookPageFishes = [];
        this.logBookPageProductQuantities.clear();

        this.form.get('shipPageNumberControl')!.setValue(undefined);
        this.form.get('shipPageDateControl')!.setValue(undefined);
        this.form.get('transportationPageNumberControl')!.setValue(undefined);
        this.form.get('transportationPageDateControl')!.setValue(undefined);
        this.form.get('admissionPageNumberControl')!.setValue(undefined);
        this.form.get('admissionPageDateControl')!.setValue(undefined);
    }

    private fillPageNumControlsByPageId(): void {
        switch (this.model.logBookType) {
            case DeclarationLogBookTypeEnum.ShipLogBook:
                this.shipLogBookPageId = this.model.shipLogBookPageId;
                this.form.get('pageNumberControl')!.setValue(this.declarationPages.find(x => x.value === this.model.shipLogBookPageId));
                break;
            case DeclarationLogBookTypeEnum.TransportationLogBook:
                this.transportationLogBookPageId = this.model.transportationLogBookPageId;
                this.form.get('pageNumberControl')!.setValue(this.declarationPages.find(x => x.value === this.model.transportationLogBookPageId));
                break;
            case DeclarationLogBookTypeEnum.AdmissionLogBook:
                this.admissionLogBookPageId = this.model.admissionLogBookPageId;
                this.form.get('pageNumberControl')!.setValue(this.declarationPages.find(x => x.value === this.model.admissionLogBookPageId));
                break;
            case DeclarationLogBookTypeEnum.FirstSaleLogBook:
                this.form.get('pageNumberControl')!.setValue(this.declarationPages.find(x => x.value === this.model.firstSaleLogBookPageId));
                break;
            case DeclarationLogBookTypeEnum.AquacultureLogBook:
                this.form.get('pageNumberControl')!.setValue(this.declarationPages.find(x => x.value === this.model.aquacultureLogBookPageId));
                break;
            case DeclarationLogBookTypeEnum.NNN:
            case DeclarationLogBookTypeEnum.Invoice:
            default:
                this.form.get('pageNumberControl')!.setValue(this.model.unregisteredPageNum);
                this.form.get('pageDateControl')!.setValue(this.model.unregisteredPageDate);
                this.form.get('logBookNumberControl')!.setValue(this.model.unregisteredLogBookNum);
                break;
        }
    }

    private getPageDateLabel(): void {
        switch (this.permitTypeSelected) {
            case DeclarationLogBookTypeEnum.AdmissionLogBook:
                this.pageDateLabel = this.translate.getValue('inspections.market-handover-date');
                break;
            case DeclarationLogBookTypeEnum.FirstSaleLogBook:
                this.pageDateLabel = this.translate.getValue('inspections.market-sale-date');
                break;
            case DeclarationLogBookTypeEnum.TransportationLogBook:
                this.pageDateLabel = this.translate.getValue('inspections.market-loading-date');
                break;
            case DeclarationLogBookTypeEnum.ShipLogBook:
                this.pageDateLabel = this.translate.getValue('inspections.market-declaration-date');
                break;
            case DeclarationLogBookTypeEnum.AquacultureLogBook:
            default:
                this.pageDateLabel = this.translate.getValue('inspections.market-filling-date');
                break;
        }
    }

    private hasLogBookPageId(): boolean {
        if (this.model.shipLogBookPageId !== undefined && this.model.shipLogBookPageId !== null) {
            return true;
        }

        if (this.model.transportationLogBookPageId !== undefined && this.model.transportationLogBookPageId !== null) {
            return true;
        }

        if (this.model.admissionLogBookPageId !== undefined && this.model.admissionLogBookPageId !== null) {
            return true;
        }

        if (this.model.firstSaleLogBookPageId !== undefined && this.model.firstSaleLogBookPageId !== null) {
            return true;
        }

        if (this.model.aquacultureLogBookPageId !== undefined && this.model.aquacultureLogBookPageId !== null) {
            return true;
        }

        return false;
    }

    private fishQuantityValidator(): ValidatorFn {
        return (form: AbstractControl): ValidationErrors | null => {
            const permit: NomenclatureDTO<number> | string | undefined = form.get('pageNumberControl')!.value;

            if (permit !== undefined && permit !== null && typeof permit !== 'string') {
                this.fishErrors = [];
                const inspectedProducts: InspectedDeclarationCatchDTO[] = form.get('inspectedCatchesControl')!.value ?? [];

                if (inspectedProducts.length > 0) {
                    const recordQuantities: Map<number, number> = this.getFishQuantities(inspectedProducts);

                    for (const fishGroupId of recordQuantities) {
                        const logBookProductQuantity: number = this.logBookPageProductQuantities.get(Number(fishGroupId[0])) ?? 0;
                        const inspectedProductQuantity: number = fishGroupId[1] ?? 0;
                        const fishName: string | undefined = this.fishes.find(x => x.value === Number(fishGroupId[0]))?.displayName;

                        if (fishName !== undefined && fishName !== null) {
                            if (inspectedProductQuantity > logBookProductQuantity) {
                                this.fishErrors.push({
                                    text: this.translate.getValue('inspections.declaration-fish-quantity-error')
                                        .replace('{0}', `${fishName} : ${logBookProductQuantity}`)
                                        .replace('{1}', (inspectedProductQuantity - logBookProductQuantity).toString()),
                                    type: 'warn'
                                });
                            }
                            else {
                                this.fishErrors.push({
                                    text: this.translate.getValue('inspections.declaration-fish-quantity-warning')
                                        .replace('{0}', `${fishName} : ${logBookProductQuantity}`),
                                    type: 'warn'
                                });
                            }
                        }
                    }
                }
            }

            return null;
        }
    }

    private getFishQuantities(fishes: InspectedDeclarationCatchDTO[]): Map<number, number> {
        const recordsGroupedByFish: Record<number, InspectedDeclarationCatchDTO[]> = CommonUtils.groupBy(fishes, x => x.fishTypeId!);
        const fishQuantities: Map<number, number> = new Map<number, number>();

        for (const fishGroupId in recordsGroupedByFish) {
            const quantity: number = recordsGroupedByFish[fishGroupId].reduce((sum, current) => Number(sum) + Number(current.catchQuantity!), 0);
            fishQuantities.set(Number(fishGroupId), quantity);
        }

        return fishQuantities;
    }
}