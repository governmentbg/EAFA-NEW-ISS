import { AfterViewInit, Component, OnInit } from '@angular/core';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { ICommercialFishingService } from '@app/interfaces/common-app/commercial-fishing.interface';
import { ShipNomenclatureDTO } from '@app/models/generated/dtos/ShipNomenclatureDTO';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { StepperSelectionEvent } from '@angular/cdk/stepper';
import { GridRow } from '@app/shared/components/data-table/models/row.model';
import { FishingGearForChoiceDTO } from '@app/models/generated/dtos/FishingGearForChoiceDTO';
import { CommonUtils } from '@app/shared/components/search-panel/utils';
import { InspectedPermitLicenseNomenclatureDTO } from '@app/models/generated/dtos/InspectedPermitLicenseNomenclatureDTO';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { ShipsUtils } from '@app/shared/utils/ships.utils';
import { ChooseFromInspectionDialogParams } from './models/choose-from-inspection-dialog-params.model';

@Component({
    selector: 'choose-permit-license-from-inspection',
    templateUrl: './choose-permit-license-from-inspection.component.html',
})
export class ChoosePermitLicenseFromInspectionComponent implements IDialogComponent, OnInit, AfterViewInit {
    public chooseShipAndYearGroup: FormGroup;
    public choosePermitLicenseGroup: FormGroup;
    public filterControl: FormControl;

    public fishingGears: FishingGearForChoiceDTO[] = [];
    public allFishingGears: FishingGearForChoiceDTO[] = [];
    public selectedFishingGears: FishingGearForChoiceDTO[] = [];
    public allPermitLicenses: InspectedPermitLicenseNomenclatureDTO[] = [];
    public permitLicenses: InspectedPermitLicenseNomenclatureDTO[] = [];
    public ships: ShipNomenclatureDTO[] = [];

    public noShipSelected: boolean = true;
    public noFishingGearsChosenValidation: boolean = false;
    public touched: boolean = false;
    public numberOfSelectedFishingGears: number = 0;

    private service!: ICommercialFishingService;
    private shipId: number | undefined;

    public constructor() {
        this.filterControl = new FormControl(null);

        this.chooseShipAndYearGroup = new FormGroup({
            shipControl: new FormControl(null, Validators.required),
            yearControl: new FormControl(null, Validators.required)
        });

        this.choosePermitLicenseGroup = new FormGroup({
            permitLicenseControl: new FormControl(null, Validators.required)
        });
    }

    public ngOnInit(): void {
        this.chooseShipAndYearGroup.get('shipControl')!.valueChanges.subscribe({
            next: (ship: ShipNomenclatureDTO | undefined | string) => {
                this.allPermitLicenses = [];
                this.resetPermitLicenseData();
                this.choosePermitLicenseGroup.reset();

                if (ship !== null && ship !== undefined && ship instanceof NomenclatureDTO) {
                    this.getShipPermitLicenseNomenclature(ship.value!);
                }
                else {
                    this.noShipSelected = true;
                }
            }
        });

        this.chooseShipAndYearGroup.get('yearControl')!.valueChanges.subscribe({
            next: (year: Date | undefined) => {
                this.resetPermitLicenseData();
                this.choosePermitLicenseGroup.reset();

                if (year !== null && year !== undefined) {
                    if (this.allPermitLicenses.length > 0) {
                        this.permitLicenses = this.allPermitLicenses.filter(x => x.year === year.getFullYear());
                    }
                }
            }
        });

        this.choosePermitLicenseGroup.get('permitLicenseControl')!.valueChanges.subscribe({
            next: (license: InspectedPermitLicenseNomenclatureDTO | undefined) => {
                this.fishingGears = [];
                this.allFishingGears = [];
                this.selectedFishingGears = [];

                if (license !== null && license !== undefined) {
                    this.service.getShipFishingGearsFromInspection(license.value!).subscribe({
                        next: (gears: FishingGearForChoiceDTO[]) => {
                            this.allFishingGears = gears;
                            this.fishingGears = gears;
                        }
                    });
                }
            }
        });

        if (this.shipId !== null && this.shipId !== undefined) {
            this.chooseShipAndYearGroup.get('yearControl')!.setValue(new Date());
            this.chooseShipAndYearGroup.get('shipControl')!.setValue(ShipsUtils.get(this.ships, this.shipId));
        }
        else {
            this.noShipSelected = true;
        }
    }

    public ngAfterViewInit(): void {
        this.filterControl.valueChanges.subscribe({
            next: (value: string) => {
                if (value?.length > 0) {
                    value = value.toLowerCase();
                    const numValue: number | undefined = Number(value);

                    this.fishingGears = this.allFishingGears.filter((fishingGear: FishingGearForChoiceDTO) => {
                        if (fishingGear.type!.toLowerCase().includes(value)) {
                            return true;
                        }

                        if (fishingGear.description?.toLocaleLowerCase().includes(value)) {
                            return true;
                        }

                        if (fishingGear.marksNumbers?.toLocaleLowerCase().includes(value)) {
                            return true;
                        }

                        if (fishingGear.count === numValue) {
                            return true;
                        }

                        if (fishingGear.netEyeSize === numValue) {
                            return true;
                        }

                        if (fishingGear.length === numValue) {
                            return true;
                        }

                        return false;
                    });
                }
                else {
                    this.fishingGears = this.allFishingGears;
                }
            }
        });
    }

    public setData(data: ChooseFromInspectionDialogParams, wrapperData: DialogWrapperData): void {
        this.service = data.service;
        this.shipId = data.shipId;
        this.ships = data.ships;
    }

    public saveBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        this.touched = true;

        if (!CommonUtils.isNullOrEmpty(this.selectedFishingGears) && !this.noFishingGearsChosenValidation) {
            this.noFishingGearsChosenValidation = false;
            const gearIds: number[] = this.selectedFishingGears.map(x => x.id!) ?? [];

            dialogClose(gearIds)
        }
        else {
            this.noFishingGearsChosenValidation = true;
        }
    }

    public cancelBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public dialogButtonClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public checkedRow(row: GridRow<FishingGearForChoiceDTO>): void {
        this.touched = true;

        const element: FishingGearForChoiceDTO | undefined = this.fishingGears?.find(x => x.id === row.data.id);

        if (element !== undefined && element !== null) {
            element.isChecked = !element.isChecked;

            if (element.isChecked) {
                this.noFishingGearsChosenValidation = false;
                this.selectedFishingGears.push(element);
            }
            else {
                this.selectedFishingGears.splice(this.selectedFishingGears.findIndex(x => x.id === element.id), 1);

                if (this.fishingGears.filter(x => x.isChecked).length === 0) {
                    this.noFishingGearsChosenValidation = true;
                }
            }

            this.numberOfSelectedFishingGears = this.selectedFishingGears.length;
            this.selectedFishingGears = this.selectedFishingGears!.slice();
        }
    }

    public getRowClass = (row: GridRow<FishingGearForChoiceDTO>): Record<string, boolean> => {
        return {
            'row-selected': row.data.isChecked === true,
            'row-not-selected': row.data.isChecked === false
        };
    }

    private getShipPermitLicenseNomenclature(shipId: number): void {
        this.service.getShipPermitLicensesFromInspection(shipId).subscribe({
            next: (result: InspectedPermitLicenseNomenclatureDTO[]) => {
                this.allPermitLicenses = result;
                const year: number | undefined = this.chooseShipAndYearGroup.get('yearControl')!.value?.getFullYear();

                if (year !== undefined && year !== null) {
                    this.permitLicenses = this.allPermitLicenses.filter(x => x.year === year);
                }
            }
        });
    }

    private resetPermitLicenseData(): void {
        this.permitLicenses = [];
        this.fishingGears = [];
        this.allFishingGears = [];
        this.selectedFishingGears = [];
    }
}