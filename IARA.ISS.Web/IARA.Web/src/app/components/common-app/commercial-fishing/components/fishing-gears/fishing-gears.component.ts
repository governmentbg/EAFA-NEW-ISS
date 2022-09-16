import { Component, DoCheck, Input, OnInit, Self, ViewChild } from '@angular/core';
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
import { AbstractControl, ControlValueAccessor, NgControl, ValidationErrors, Validator } from '@angular/forms';
import { PageCodeEnum } from '@app/enums/page-code.enum';
import { EditFishingGearDialogParamsModel } from './models/edit-fishing-gear-dialog-params.model';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { IS_PUBLIC_APP } from '@app/shared/modules/application.modules';

@Component({
    selector: 'fishing-gears',
    templateUrl: './fishing-gears.component.html'
})
export class FishingGearsComponent implements OnInit, DoCheck, ControlValueAccessor, Validator {
    @Input()
    public isReadonly: boolean = false;

    @Input()
    public isApplication: boolean = true; // = false;

    @Input()
    public isDraftMode: boolean = false;

    @Input()
    public service!: ICommercialFishingService;

    @Input()
    public maxNumberOfFishingGears: number | undefined;

    @Input()
    public pageCode: PageCodeEnum | undefined;

    public fishingGears: FishingGearDTO[] = [];

    @ViewChild('fishingGearsTable')
    private fishingGearsTable!: TLDataTableComponent;

    public hasMoreThanPermittedNumberOfFishingGears: boolean = false;

    private ngControl: NgControl;
    private onChanged: (value: FishingGearDTO[]) => void = (value: FishingGearDTO[]) => { return; };
    private onTouched: (value: FishingGearDTO[]) => void = (value: FishingGearDTO[]) => { return; };

    private translate: FuseTranslationLoaderService;
    private confirmDialog: TLConfirmDialog;
    private editFishingGearDialog: TLMatDialog<EditFishingGearComponent>;
    private isPublicApp: boolean;

    public constructor(
        @Self() ngControl: NgControl,
        translate: FuseTranslationLoaderService,
        confirmDialog: TLConfirmDialog,
        editFishingGearDialog: TLMatDialog<EditFishingGearComponent>
    ) {
        this.ngControl = ngControl;
        this.ngControl.valueAccessor = this;

        this.translate = translate;
        this.confirmDialog = confirmDialog;
        this.editFishingGearDialog = editFishingGearDialog;
        this.isPublicApp = IS_PUBLIC_APP;
    }

    public ngOnInit(): void {
        if (this.ngControl.control) {
            this.ngControl.control.validator = this.validate.bind(this);
        }
    }

    public ngDoCheck(): void {
        if (this.ngControl?.control?.touched) {
            this.validate(this.ngControl.control!);
        }
    }

    public writeValue(value: FishingGearDTO[]): void {
        if (value !== null && value !== undefined) {
            setTimeout(() => {
                this.fishingGears = value;
                this.onChanged(value);
            });
        }
    }

    public registerOnChange(fn: (value: FishingGearDTO[]) => void): void {
        this.onChanged = fn;
    }

    public registerOnTouched(fn: (value: FishingGearDTO[]) => void): void {
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

        this.checkForMoreThanPermittedNumberFishingGearsError();
        if (this.hasMoreThanPermittedNumberOfFishingGears === true) {
            errors['moreThanPermittedFishingGears'] = true;
        }

        return Object.keys(errors).length > 0 ? errors : null;
    }

    public addEditFishingGear(fishingGear?: FishingGearDTO, viewMode: boolean = false): void {
        let data: EditFishingGearDialogParamsModel | undefined;
        let headerAuditBtn: IHeaderAuditButton | undefined;
        let title: string;

        if (fishingGear !== undefined) {
            data = new EditFishingGearDialogParamsModel({
                model: fishingGear,
                readOnly: this.isReadonly || viewMode,
                isDraft: this.isDraftMode,
                pageCode: this.pageCode
            });

            if (fishingGear.id !== undefined && !this.isPublicApp) {
                headerAuditBtn = {
                    id: fishingGear.id,
                    getAuditRecordData: this.service.getFishingGearAudit.bind(this.service),
                    tableName: 'FishingGearRegister'
                }
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
                isDraft: this.isDraftMode,
                pageCode: this.pageCode
            });

            title = this.translate.getValue('fishing-gears.add-fishing-gear-dialog-title');
        }

        const dialog = this.editFishingGearDialog.openWithTwoButtons({
            title: title,
            TCtor: EditFishingGearComponent,
            headerAuditButton: headerAuditBtn,
            headerCancelButton: {
                cancelBtnClicked: this.closeEditFishingGearDialogBtnClicked.bind(this)
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

                setTimeout(() => {
                    this.fishingGears = this.fishingGears.slice();

                    this.onChanged(this.fishingGears);
                    this.checkForMoreThanPermittedNumberFishingGearsError();
                });
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
                    this.onChanged(this.fishingGears);
                    this.checkForMoreThanPermittedNumberFishingGearsError();
                }
            }
        });
    }

    public undoDeleteFishingGear(row: GridRow<FishingGearDTO>): void {
        this.confirmDialog.open().subscribe({
            next: (ok: boolean) => {
                if (ok) {
                    this.fishingGearsTable.softUndoDelete(row);
                    this.onChanged(this.fishingGears);
                    this.checkForMoreThanPermittedNumberFishingGearsError();
                }
            }
        });
    }

    private checkForMoreThanPermittedNumberFishingGearsError(): void {
        if (this.maxNumberOfFishingGears !== null && this.maxNumberOfFishingGears !== undefined) {
            let numberOfFishingGears: number = 0;
            if (this.fishingGears !== null && this.fishingGears !== undefined && this.fishingGears.length > 0) {
                const groupedFishingGearsByType = CommonUtils.groupByKey(this.fishingGears.filter(x => x.isActive), 'typeId');
                numberOfFishingGears = Object.getOwnPropertyNames(groupedFishingGearsByType).length;
            }

            if (numberOfFishingGears > this.maxNumberOfFishingGears) {
                this.hasMoreThanPermittedNumberOfFishingGears = true;
            }
            else {
                this.hasMoreThanPermittedNumberOfFishingGears = false;
            }
        }
        else {
            this.hasMoreThanPermittedNumberOfFishingGears = false;
        }
    }

    private closeEditFishingGearDialogBtnClicked(closeFn: HeaderCloseFunction): void {
        closeFn();
    }
}