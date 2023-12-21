import { Component, Input, OnInit, Self, ViewChild } from '@angular/core';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { FishingGearDTO } from '@app/models/generated/dtos/FishingGearDTO';
import { TLConfirmDialog } from '@app/shared/components/confirmation-dialog/tl-confirm-dialog';
import { TLDataTableComponent } from '@app/shared/components/data-table/tl-data-table.component';
import { GridRow } from '@app/shared/components/data-table/models/row.model';
import { EditFishingGearComponent } from './components/edit-fishing-gear.component';
import { IHeaderAuditButton } from '@app/shared/components/dialog-wrapper/interfaces/header-audit-button.interface';
import { ICommercialFishingService } from '@app/interfaces/common-app/commercial-fishing.interface';
import { TLMatDialog } from '@app/shared/components/dialog-wrapper/tl-mat-dialog';
import { HeaderCloseFunction } from '@app/shared/components/dialog-wrapper/interfaces/header-cancel-button.interface';
import { AbstractControl, FormControl, NgControl, ValidationErrors, ValidatorFn } from '@angular/forms';
import { PageCodeEnum } from '@app/enums/page-code.enum';
import { EditFishingGearDialogParamsModel } from './models/edit-fishing-gear-dialog-params.model';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { IS_PUBLIC_APP } from '@app/shared/modules/application.modules';
import { CustomFormControl } from '@app/shared/utils/custom-form-control';
import { ChoosePermitLicenseFromInspectionComponent } from './components/choose-permit-license-from-inspection/choose-permit-license-from-inspection.component';
import { ChoosePermitLicenseFromInspectionDialogParams } from './components/choose-permit-license-from-inspection/models/choose-permit-license-from-inspection-dialog-params.model';
import { ShipNomenclatureDTO } from '@app/models/generated/dtos/ShipNomenclatureDTO';

@Component({
    selector: 'fishing-gears',
    templateUrl: './fishing-gears.component.html'
})
export class FishingGearsComponent extends CustomFormControl<FishingGearDTO[]> implements OnInit {
    @Input()
    public isReadonly: boolean = false;

    @Input()
    public isApplication: boolean = true; // = false;

    @Input()
    public service!: ICommercialFishingService;

    @Input()
    public maxNumberOfFishingGears: number | undefined;

    @Input()
    public pageCode: PageCodeEnum | undefined;

    @Input()
    public shipId: number | undefined;

    @Input()
    public ships: ShipNomenclatureDTO[] = [];

    @Input()
    public showInspectedGears: boolean = false;

    @Input()
    public isDunabe: boolean = false;

    @Input()
    public appliedTariffs: string[] = [];

    public fishingGears: FishingGearDTO[] = [];
    public isPublicApp: boolean;

    @ViewChild('fishingGearsTable')
    private fishingGearsTable!: TLDataTableComponent;

    private translate: FuseTranslationLoaderService;
    private confirmDialog: TLConfirmDialog;
    private editFishingGearDialog: TLMatDialog<EditFishingGearComponent>;
    private choosePermitLicenseFromInspectionDialog: TLMatDialog<ChoosePermitLicenseFromInspectionComponent>;

    public constructor(
        @Self() ngControl: NgControl,
        translate: FuseTranslationLoaderService,
        confirmDialog: TLConfirmDialog,
        editFishingGearDialog: TLMatDialog<EditFishingGearComponent>,
        choosePermitLicenseFromInspectionDialog: TLMatDialog<ChoosePermitLicenseFromInspectionComponent>
    ) {
        super(ngControl, false);

        this.translate = translate;
        this.confirmDialog = confirmDialog;
        this.editFishingGearDialog = editFishingGearDialog;
        this.choosePermitLicenseFromInspectionDialog = choosePermitLicenseFromInspectionDialog;
        this.isPublicApp = IS_PUBLIC_APP;
    }

    public ngOnInit(): void {
        this.initCustomFormControl();
    }

    protected getValue(): FishingGearDTO[] {
        return this.fishingGears.slice();
    }

    protected buildForm(): AbstractControl {
        return new FormControl(undefined, [this.permittedNumberFishingGearsValidator()]);
    }

    public writeValue(value: FishingGearDTO[]): void {
        if (value !== null && value !== undefined) {
            this.fishingGears = value;
        }
        else {
            this.fishingGears = [];
        }

        this.onChanged(this.getValue());
    }

    public registerOnChange(fn: (value: FishingGearDTO[]) => void): void {
        this.onChanged = fn;
    }

    public registerOnTouched(fn: (value: FishingGearDTO[]) => void): void {
        this.onTouched = fn;
    }

    public addEditFishingGear(fishingGear?: FishingGearDTO, viewMode: boolean = false): void {
        let data: EditFishingGearDialogParamsModel | undefined;
        let headerAuditBtn: IHeaderAuditButton | undefined;
        let title: string;

        if (fishingGear !== undefined) {
            data = new EditFishingGearDialogParamsModel({
                model: fishingGear,
                readOnly: this.isReadonly || viewMode,
                pageCode: this.pageCode,
                isDunabe: this.isDunabe,
                appliedTariffCodes: this.appliedTariffs
            });

            if (fishingGear.id !== undefined && !this.isPublicApp) {
                headerAuditBtn = {
                    id: fishingGear.id,
                    getAuditRecordData: this.service.getFishingGearAudit.bind(this.service),
                    tableName: 'FishingGearRegister'
                };
            }

            if (this.isReadonly || viewMode) {
                title = this.translate.getValue('fishing-gears.view-fishing-gear-dialog-title');
            }
            else {
                title = this.translate.getValue('fishing-gears.edit-fishing-gear-dialog-title');
            }
        }
        else {
            data = new EditFishingGearDialogParamsModel({
                pageCode: this.pageCode,
                isDunabe: this.isDunabe,
                appliedTariffCodes: this.appliedTariffs
            });

            title = this.translate.getValue('fishing-gears.add-fishing-gear-dialog-title');
        }

        const dialog = this.editFishingGearDialog.openWithTwoButtons({
            title: title,
            TCtor: EditFishingGearComponent,
            headerAuditButton: headerAuditBtn,
            headerCancelButton: {
                cancelBtnClicked: (closeFn: HeaderCloseFunction) => { closeFn(); }
            },
            componentData: data,
            translteService: this.translate,
            disableDialogClose: true,
            viewMode: this.isReadonly || viewMode
        }, '1200px');

        dialog.subscribe((result: FishingGearDTO | null | undefined) => {
            if (result !== null && result !== undefined) {
                if (fishingGear !== null && fishingGear !== undefined) {
                    fishingGear = result;
                }
                else {
                    this.fishingGears.push(result);
                }

                this.fishingGears = this.fishingGears.slice();
                this.control.markAsTouched();
                this.control.updateValueAndValidity();
                this.onChanged(this.getValue());
            }
        });
    }

    public deleteFishingGear(row: GridRow<FishingGearDTO>): void {
        this.confirmDialog.open({
            title: this.translate.getValue('fishing-gears.delete-fishing-gear-dialog-label'),
            message: this.translate.getValue('fishing-gears.confirm-delete-fishing-gear-message'),
            okBtnLabel: this.translate.getValue('fishing-gears.delete-fishing-gear-btn-label')
        }).subscribe({
            next: (ok: boolean) => {
                if (ok) {
                    this.fishingGearsTable.softDelete(row);
                    this.control.markAsTouched();
                    this.control.updateValueAndValidity();
                    this.onChanged(this.getValue());
                }
            }
        });
    }

    public undoDeleteFishingGear(row: GridRow<FishingGearDTO>): void {
        this.confirmDialog.open().subscribe({
            next: (ok: boolean) => {
                if (ok) {
                    this.fishingGearsTable.softUndoDelete(row);
                    this.control.markAsTouched();
                    this.control.updateValueAndValidity();
                    this.onChanged(this.getValue());
                }
            }
        });
    }

    public choosePermitLicenseFromInspection(): void {
        const dialog = this.choosePermitLicenseFromInspectionDialog.openWithTwoButtons({
            title: this.translate.getValue('fishing-gears.choose-permit-license-from-inspection-dialog-title'),
            componentData: new ChoosePermitLicenseFromInspectionDialogParams({
                shipId: this.shipId,
                service: this.service,
                ships: this.ships
            }),
            headerCancelButton: {
                cancelBtnClicked: (closeFn: HeaderCloseFunction) => { closeFn(); }
            },
            translteService: this.translate,
            TCtor: ChoosePermitLicenseFromInspectionComponent
        }, '1500px');

        dialog.subscribe((gearIds: number[] | null | undefined) => {
            if (gearIds !== null && gearIds !== undefined && gearIds.length > 0) {
                this.service.getFishingGearsForIds(gearIds).subscribe({
                    next: (gears: FishingGearDTO[]) => {
                        if (gears.length > 0) {
                            this.fishingGears.push(...gears);
                            this.fishingGears = this.fishingGears.slice();

                            this.control.markAsTouched();
                            this.control.updateValueAndValidity();
                            this.onChanged(this.getValue());
                        }
                    }
                });
            }
        });
    }

    private permittedNumberFishingGearsValidator(): ValidatorFn {
        return (control: AbstractControl): ValidationErrors | null => {
            if (this.maxNumberOfFishingGears !== null && this.maxNumberOfFishingGears !== undefined) {
                let numberOfFishingGears: number = 0;

                if (this.fishingGears !== null && this.fishingGears !== undefined && this.fishingGears.length > 0) {
                    const groupedFishingGearsByType = CommonUtils.groupByKey(this.fishingGears.filter(x => x.isActive), 'typeId');
                    numberOfFishingGears = Object.getOwnPropertyNames(groupedFishingGearsByType).length;
                }

                if (numberOfFishingGears > this.maxNumberOfFishingGears) {
                    return { 'moreThanPermittedFishingGears': true };
                }
                else {
                    return null;
                }
            }
            else {
                return null;
            }
        }
    }
}