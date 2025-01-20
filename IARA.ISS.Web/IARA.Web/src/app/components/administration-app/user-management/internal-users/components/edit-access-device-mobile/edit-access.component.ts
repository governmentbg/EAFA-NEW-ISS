import { Component, forwardRef, Input, ViewChild } from '@angular/core';
import { AbstractControl, ControlValueAccessor, FormControl, FormGroup, NG_VALUE_ACCESSOR, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { MobileDeviceStatusEnum } from '@app/enums/mobile-device-status.enum';
import { MobileDeviceDTO } from '@app/models/generated/dtos/MobileDeviceDTO';
import { TLConfirmDialog } from '@app/shared/components/confirmation-dialog/tl-confirm-dialog';
import { GridRow } from '@app/shared/components/data-table/models/row.model';
import { TLDataTableComponent } from '@app/shared/components/data-table/tl-data-table.component';
import { RecordChangedEventArgs } from '@app/shared/components/data-table/models/record-changed-event.model';
import { CommandTypes } from '@app/shared/components/data-table/enums/command-type.enum';
import { GetControlErrorLabelTextCallback } from '@app/shared/components/input-controls/base-tl-control';
import { TLError } from '@app/shared/components/input-controls/models/tl-error.model';

@Component({
    selector: 'edit-access',
    templateUrl: './edit-access.component.html',
    providers: [{
        provide: NG_VALUE_ACCESSOR,
        useExisting: forwardRef(() => EditAccessComponent),
        multi: true
    }]
})
export class EditAccessComponent implements ControlValueAccessor {
    @Input() public matCardTitleLabel!: string;
    @Input() public userFullName!: string;
    @Input() public readOnly!: boolean;

    public translationService: FuseTranslationLoaderService;
    public userDevices: MobileDeviceDTO[] = [];
    public userMobileDevicesForm!: FormGroup;
    public mobileDeviceStatusEnum: typeof MobileDeviceStatusEnum = MobileDeviceStatusEnum;

    public isDisabled: boolean = false;

    public getControlErrorLabelTextMethod: GetControlErrorLabelTextCallback = this.getControlErrorLabelText.bind(this);

    @ViewChild(TLDataTableComponent)
    private datatable!: TLDataTableComponent;

    private onChange!: (value: MobileDeviceDTO[]) => void;
    private onTouched!: (value: MobileDeviceDTO[]) => void;
    private confirmDialog: TLConfirmDialog;

    public constructor(
        translationService: FuseTranslationLoaderService,
        confirmDialog: TLConfirmDialog
    ) {
        this.translationService = translationService;
        this.confirmDialog = confirmDialog;

        this.userMobileDevicesForm = new FormGroup({
            imeiControl: new FormControl(null, [Validators.required, Validators.maxLength(50)]),
            descriptionControl: new FormControl({ value: null, disabled: true }),
            requestAccessDateControl: new FormControl({ value: null, disabled: true })
        }, this.uniqueImeiValidator());
    }

    public writeValue(value: MobileDeviceDTO[]): void {
        this.userDevices = value;
    }

    public registerOnChange(fn: (value: MobileDeviceDTO[]) => void): void {
        this.onChange = fn;
    }

    public registerOnTouched(fn: (value: unknown) => void): void {
        this.onTouched = fn;
    }

    public userDevicesChanged(recordChangedEvent: RecordChangedEventArgs<MobileDeviceDTO>): void {
        if (recordChangedEvent.Command === CommandTypes.Add) {
            if (recordChangedEvent.Record.accessStatus !== null
                || recordChangedEvent.Record.accessStatus !== undefined) {
                recordChangedEvent.Record.accessStatus = this.mobileDeviceStatusEnum.APPROVED;
            }
            recordChangedEvent.Record.requestAccessDate = new Date();
        }

        this.userMobileDevicesForm.get('imeiControl')!.setValidators([Validators.required, Validators.maxLength(50)]);
        this.userMobileDevicesForm.get('imeiControl')!.updateValueAndValidity({ emitEvent: false });
    }

    public onAddRecord(): void {
        this.userMobileDevicesForm.get('imeiControl')!.setValidators([Validators.required, Validators.maxLength(50), this.uniqueImeiValidator()]);
        this.userMobileDevicesForm.get('imeiControl')!.updateValueAndValidity({ emitEvent: false });
    }

    public allowAccess(id: number): void {
        if (this.userDevices.some(x => x.accessStatus === this.mobileDeviceStatusEnum.APPROVED)) {
            let approvedDevicesCount: number = 0;
            this.userDevices.forEach((d) => {
                if (d.accessStatus === this.mobileDeviceStatusEnum.APPROVED) {
                    approvedDevicesCount++;
                }
            });

            this.confirmDialog.open({
                title: this.translationService.getValue('users-page.restrict-access-to-previous-devices-confirm-dialog-title'),
                message: `${this.translationService.getValue('users-page.user')} ${this.userFullName} 
                          ${this.translationService.getValue('users-page.has-approved')} ${approvedDevicesCount} 
                          ${this.translationService.getValue('users-page.other-devices')}. 
                          ${this.translationService.getValue('users-page.do-you-want-to-detach')} ${approvedDevicesCount} 
                          ${this.translationService.getValue('users-page.these-devices')}`,
                okBtnLabel: this.translationService.getValue('users-page.ok-btn-label'),
                cancelBtnLabel: this.translationService.getValue('users-page.cancel-btn-label')
            }).subscribe((result: boolean) => {
                if (result) {
                    this.userDevices.forEach((x) => {
                        if (x.accessStatus === this.mobileDeviceStatusEnum.APPROVED) {
                            x.accessStatus = this.mobileDeviceStatusEnum.BLOCKED;
                            this.onChange(this.userDevices);
                        }
                    });

                    const device = this.userDevices.find(x => x.id === id) as MobileDeviceDTO;
                    device.accessStatus = this.mobileDeviceStatusEnum.APPROVED;

                    this.userDevices = this.userDevices.slice();
                    this.onChange(this.userDevices);
                }
                else {
                    const device = this.userDevices.find(x => x.id === id) as MobileDeviceDTO;
                    device.accessStatus = this.mobileDeviceStatusEnum.APPROVED;

                    this.userDevices = this.userDevices.slice();
                    this.onChange(this.userDevices);
                }
            });
        }
        else {
            const device = this.userDevices.find(x => x.id === id) as MobileDeviceDTO;
            device.accessStatus = this.mobileDeviceStatusEnum.APPROVED;

            this.userDevices = this.userDevices.slice();
            this.onChange(this.userDevices);
        }
    }

    public denyAccess(id: number): void {
        const device = this.userDevices.find(x => x.id === id) as MobileDeviceDTO;
        device.accessStatus = this.mobileDeviceStatusEnum.BLOCKED;

        this.userDevices = this.userDevices.slice();
        this.onChange(this.userDevices);
    }

    public reloadAppDatabase(id: number, mustReload: boolean): void {
        const device = this.userDevices.find(x => x.id === id) as MobileDeviceDTO;

        if (mustReload) {
            this.confirmDialog.open({
                title: this.translationService.getValue('users-page.reload-database-confirm-dialog-title'),
                message: `${this.translationService.getValue('users-page.user')} ${this.userFullName}
                          ${this.translationService.getValue('users-page.user-must-reload-database-confirmation')}`,
                okBtnLabel: this.translationService.getValue('users-page.user-must-reload-database-ok-btn-label'),
                cancelBtnLabel: this.translationService.getValue('users-page.user-must-reload-database-cancel-btn-label')
            }).subscribe((yes: boolean) => {
                if (yes) {
                    device.userMustReloadAppDatabase = mustReload;
                }
            });
        }
        else {
            device.userMustReloadAppDatabase = mustReload;
        }

        this.userDevices = this.userDevices.slice();
        this.onChange(this.userDevices);
    }

    public onSaveRow(row: GridRow<unknown>): void {
        const success = this.datatable.saveRow(row);
        if (success) {
            this.userDevices = this.datatable.rows;
            this.onChange(this.userDevices);
        }

        this.userMobileDevicesForm.updateValueAndValidity({ emitEvent: false });
    }

    public onUndoAddEditRow(row: GridRow<unknown>): void {
        this.datatable.undoAddEditRow(row);
    }

    public setDisabledState(isDisabled: boolean): void {
        this.isDisabled = isDisabled;
        if (this.isDisabled) {
            this.userMobileDevicesForm.disable();
        }
        else {
            this.userMobileDevicesForm.enable();
        }
    }
    public getControlErrorLabelText(controlName: string, errorValue: unknown, errorCode: string): TLError | undefined {
        if (errorValue === true) {
            if (controlName === 'imeiControl') {
                if (errorCode === 'uniqueImei') {
                    return new TLError({ text: this.translationService.getValue('users-page.unique-mobile-device-imei-error'), type: 'error' });
                }
            }
        }
        return undefined;
    }

    private uniqueImeiValidator(): ValidatorFn {
        return (control: AbstractControl): ValidationErrors | null => {
            const imei: string | undefined = control.value;

            if (this.datatable) {
                const rows = this.datatable.rows as MobileDeviceDTO[];
                const invalidRows = rows.filter(x => x.imei === imei);

                if (invalidRows.length > 0) {
                    return { uniqueImei: true };
                }
            }

            return null;
        };
    }
}