import { Component, Input, OnInit, Self, ViewChild } from '@angular/core';
import { AbstractControl, FormControl, NgControl, ValidationErrors, ValidatorFn } from '@angular/forms';

import { CustomFormControl } from '@app/shared/utils/custom-form-control';
import { GridRow } from '@app/shared/components/data-table/models/row.model';
import { TLMatDialog } from '@app/shared/components/dialog-wrapper/tl-mat-dialog';
import { TLConfirmDialog } from '@app/shared/components/confirmation-dialog/tl-confirm-dialog';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { TLDataTableComponent } from '@app/shared/components/data-table/tl-data-table.component';
import { HeaderCloseFunction } from '@app/shared/components/dialog-wrapper/interfaces/header-cancel-button.interface';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { EditMarketCatchComponent } from '../edit-market-catch/edit-market-catch.component';
import { MarketCatchTableParams } from './models/market-catch-table-params';
import { InspectedCatchTableModel } from '../../../../models/inspected-catch-table.model';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { InspectionLogBookPageDTO } from '@app/models/generated/dtos/InspectionLogBookPageDTO';
import { DeclarationLogBookTypeEnum } from '@app/enums/declaration-log-book-type.enum';
import { InspectedDeclarationCatchDTO } from '@app/models/generated/dtos/InspectedDeclarationCatchDTO';

@Component({
    selector: 'market-catches-table',
    templateUrl: './market-catches-table.component.html'
})
export class MarketCatchesTableComponent extends CustomFormControl<InspectionLogBookPageDTO[]> implements OnInit {
    @Input()
    public hasCatchType: boolean = true;

    @Input()
    public hasUndersizedCheck: boolean = false;

    @Input()
    public fishes: NomenclatureDTO<number>[] = [];

    @Input()
    public types: NomenclatureDTO<number>[] = [];

    @Input()
    public presentations: NomenclatureDTO<number>[] = [];

    public logBookTypes: NomenclatureDTO<DeclarationLogBookTypeEnum>[] = [];

    public catchQuantities: Map<number, number> = new Map<number, number>();

    public pages: InspectionLogBookPageDTO[] = [];

    @ViewChild(TLDataTableComponent)
    private datatable!: TLDataTableComponent;

    private readonly translate: FuseTranslationLoaderService;
    private readonly confirmDialog: TLConfirmDialog;
    private readonly editEntryDialog: TLMatDialog<EditMarketCatchComponent>;

    public constructor(
        @Self() ngControl: NgControl,
        translate: FuseTranslationLoaderService,
        confirmDialog: TLConfirmDialog,
        editEntryDialog: TLMatDialog<EditMarketCatchComponent>
    ) {
        super(ngControl);

        this.translate = translate;
        this.confirmDialog = confirmDialog;
        this.editEntryDialog = editEntryDialog;

        this.initLogBookTypesNomenclature();
    }

    public async ngOnInit(): Promise<void> {
        this.initCustomFormControl();
    }

    public writeValue(value: InspectionLogBookPageDTO[]): void {
        if (value !== undefined && value !== null && value.length !== 0) {
            setTimeout(() => {
                this.pages = value;
                this.recalculateCatchQuantitySums();
                this.onChanged(this.getValue());
                this.form.updateValueAndValidity();
            });
        }
        else {
            this.pages = [];
            this.recalculateCatchQuantitySums();
            this.onChanged(this.getValue());
        }
    }

    public addEditEntry(inspectedPage?: InspectionLogBookPageDTO, viewMode?: boolean): void {
        const readOnly: boolean = this.isDisabled || viewMode === true;

        let data: MarketCatchTableParams | undefined;
        let title: string;

        if (inspectedPage !== undefined && inspectedPage !== null) {
            data = new MarketCatchTableParams({
                model: inspectedPage,
                readOnly: readOnly,
                fishes: this.fishes,
                presentations: this.presentations,
                types: this.types,
                hasCatchType: this.hasCatchType,
                hasUndersizedCheck: this.hasUndersizedCheck,
            });

            if (readOnly) {
                title = this.translate.getValue('inspections.view-market-catches-dialog-title');
            }
            else {
                title = this.translate.getValue('inspections.edit-market-catches-dialog-title');
            }
        }
        else {
            data = new MarketCatchTableParams({
                fishes: this.fishes,
                presentations: this.presentations,
                types: this.types,
                hasCatchType: this.hasCatchType,
                hasUndersizedCheck: this.hasUndersizedCheck,
            });

            title = this.translate.getValue('inspections.add-market-catches-dialog-title');
        }

        const dialog = this.editEntryDialog.openWithTwoButtons({
            title: title,
            TCtor: EditMarketCatchComponent,
            headerCancelButton: {
                cancelBtnClicked: (closeFn: HeaderCloseFunction) => closeFn()
            },
            componentData: data,
            translteService: this.translate,
            disableDialogClose: !readOnly,
            viewMode: readOnly
        }, '1200px');

        dialog.subscribe((result: InspectionLogBookPageDTO) => {
            if (result !== undefined && result !== null) {
                result.catchMeasuresText = result.inspectionCatchMeasures?.map(x => `${x.fishName} ${(Number(x.catchQuantity)).toFixed(2)}kg`).join(';') ?? '';

                if (inspectedPage !== undefined) {
                    inspectedPage = result;
                }
                else {
                    this.pages.push(result);
                }

                this.pages = this.pages.slice();
                this.recalculateCatchQuantitySums();
                this.onChanged(this.getValue());
                this.form.updateValueAndValidity();
            }
        });
    }

    public deleteEntry(inspectedCatch: GridRow<InspectionLogBookPageDTO>): void {
        this.confirmDialog.open({
            title: this.translate.getValue('inspections.market-catches-table-delete-dialog-title'),
            message: this.translate.getValue('inspections.market-catches-table-delete-message'),
            okBtnLabel: this.translate.getValue('inspections.market-catches-table-delete-btn-label')
        }).subscribe({
            next: (ok: boolean) => {
                if (ok) {
                    this.datatable.softDelete(inspectedCatch);
                    this.pages.splice(this.pages.indexOf(inspectedCatch.data), 1);
                    this.onChanged(this.getValue());
                    this.recalculateCatchQuantitySums();
                    this.form.updateValueAndValidity();
                }
            }
        });
    }

    protected buildForm(): AbstractControl {
        return new FormControl(undefined, [this.minLengthValidator(), this.quantitiesValidator()]);
    }

    protected getValue(): InspectionLogBookPageDTO[] {
        return this.pages;
    }

    private minLengthValidator(): ValidatorFn {
        return (): ValidationErrors | null => {
            if (this.pages !== undefined && this.pages !== null) {
                if (this.pages.length === 0) {
                    return { 'minLength': true };
                }
            }
            return null;
        };
    }

    private quantitiesValidator(): ValidatorFn {
        return (): ValidationErrors | null => {
            if (this.pages !== undefined && this.pages !== null && this.pages.length > 0) {
                for (const page of this.pages) {
                    if (page.inspectionCatchMeasures !== null && page.inspectionCatchMeasures !== undefined
                        && page.inspectionCatchMeasures.some(x => x.catchQuantity === undefined || x.catchQuantity === null || Number(x.catchQuantity) === 0)) {
                        return { 'catchMeasureQuantityError': true };
                    }
                }
            }
            return null;
        };
    }

    private recalculateCatchQuantitySums(): void {
        const inspectedProducts: InspectedDeclarationCatchDTO[][] = this.pages.map(x => x.inspectionCatchMeasures!);
        const inspectedProductsFlattened: InspectedDeclarationCatchDTO[] = ([] as InspectedDeclarationCatchDTO[]).concat(...inspectedProducts);

        const recordsGroupedByFish: Record<number, InspectedDeclarationCatchDTO[]> = CommonUtils.groupBy(inspectedProductsFlattened, x => x.fishTypeId!);
        this.catchQuantities.clear();

        for (const fishGroupId in recordsGroupedByFish) {
            const quantity: number = recordsGroupedByFish[fishGroupId].reduce((sum, current) => Number(sum) + Number(current.catchQuantity!), 0);
            this.catchQuantities.set(Number(fishGroupId), quantity);
        }
    }

    private initLogBookTypesNomenclature(): void {
        this.logBookTypes = [
            new NomenclatureDTO({
                value: DeclarationLogBookTypeEnum.FirstSaleLogBook,
                displayName: this.translate.getValue('inspections.market-first-sale-log-book'),
                isActive: true
            }),
            new NomenclatureDTO({
                value: DeclarationLogBookTypeEnum.AdmissionLogBook,
                displayName: this.translate.getValue('inspections.market-admission-log-book'),
                isActive: true
            }),
            new NomenclatureDTO({
                value: DeclarationLogBookTypeEnum.TransportationLogBook,
                displayName: this.translate.getValue('inspections.market-transport-log-book'),
                isActive: true
            }),
            new NomenclatureDTO({
                value: DeclarationLogBookTypeEnum.ShipLogBook,
                displayName: this.translate.getValue('inspections.market-ship-log-book'),
                isActive: true
            }),
            new NomenclatureDTO({
                value: DeclarationLogBookTypeEnum.AquacultureLogBook,
                displayName: this.translate.getValue('inspections.market-aquaculture-log-book'),
                isActive: true
            }),
            new NomenclatureDTO({
                value: DeclarationLogBookTypeEnum.Invoice,
                displayName: this.translate.getValue('inspections.market-other'),
                isActive: true
            }),
            new NomenclatureDTO({
                value: DeclarationLogBookTypeEnum.NNN,
                displayName: this.translate.getValue('inspections.market-ship-nnn'),
                isActive: true
            })
        ];
    }
}