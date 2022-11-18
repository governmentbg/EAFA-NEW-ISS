import { Component, Input, OnInit, Self, ViewChild } from '@angular/core';
import { AbstractControl, FormGroup, NgControl } from '@angular/forms';

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
import { InspectedDeclarationCatchDTO } from '@app/models/generated/dtos/InspectedDeclarationCatchDTO';

@Component({
    selector: 'market-catches-table',
    templateUrl: './market-catches-table.component.html'
})
export class MarketCatchesTableComponent extends CustomFormControl<InspectedDeclarationCatchDTO[]> implements OnInit {

    public catches: InspectedDeclarationCatchDTO[] = [];

    @Input()
    public hasCatchType: boolean = true;

    @Input()
    public fishes: NomenclatureDTO<number>[] = [];

    @Input()
    public types: NomenclatureDTO<number>[] = [];

    @Input()
    public catchZones: NomenclatureDTO<number>[] = [];

    @Input()
    public presentations: NomenclatureDTO<number>[] = [];

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
    }

    public async ngOnInit(): Promise<void> {
        this.initCustomFormControl();
    }

    public writeValue(value: InspectedDeclarationCatchDTO[]): void {
        if (value !== undefined && value !== null && value.length !== 0) {
            setTimeout(() => {
                this.catches = value;
            });
        }
        else {
            this.catches = [];
        }
    }

    public addEditEntry(inspectedCatch?: InspectedDeclarationCatchDTO, viewMode?: boolean): void {
        const readOnly: boolean = this.isDisabled || viewMode === true;

        let data: MarketCatchTableParams | undefined;
        let title: string;

        if (inspectedCatch !== undefined && inspectedCatch !== null) {
            data = new MarketCatchTableParams({
                model: inspectedCatch,
                readOnly: readOnly,
                catchZones: this.catchZones,
                fishes: this.fishes,
                presentations: this.presentations,
                types: this.types,
                hasCatchType: this.hasCatchType,
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
                catchZones: this.catchZones,
                fishes: this.fishes,
                presentations: this.presentations,
                types: this.types,
                hasCatchType: this.hasCatchType
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
        }, '1000px');

        dialog.subscribe((result: InspectedDeclarationCatchDTO) => {
            if (result !== undefined && result !== null) {
                if (inspectedCatch !== undefined) {
                    inspectedCatch = result;
                }
                else {
                    this.catches.push(result);
                }

                this.catches = this.catches.slice();
                this.onChanged(this.getValue());
            }
        });
    }

    public deleteEntry(inspectedCatch: GridRow<InspectedDeclarationCatchDTO>): void {
        this.confirmDialog.open({
            title: this.translate.getValue('inspections.market-catches-table-delete-dialog-title'),
            message: this.translate.getValue('inspections.market-catches-table-delete-message'),
            okBtnLabel: this.translate.getValue('inspections.market-catches-table-delete-btn-label')
        }).subscribe({
            next: (ok: boolean) => {
                if (ok) {
                    this.datatable.softDelete(inspectedCatch);
                    this.catches.splice(this.catches.indexOf(inspectedCatch.data), 1);
                    this.onChanged(this.getValue());
                }
            }
        });
    }

    protected buildForm(): AbstractControl {
        return new FormGroup({});
    }

    protected getValue(): InspectedDeclarationCatchDTO[] {
        return this.catches;
    }
}