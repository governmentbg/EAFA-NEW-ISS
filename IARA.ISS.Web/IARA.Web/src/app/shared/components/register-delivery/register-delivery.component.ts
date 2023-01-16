import { Component, OnInit } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { Subject } from 'rxjs';
import { HttpErrorResponse } from '@angular/common/http';

import { IDeliveryService } from '@app/interfaces/common-app/delivery.interface';
import { ApplicationDeliveryDTO } from '@app/models/generated/dtos/ApplicationDeliveryDTO';
import { PageCodeEnum } from '@app/enums/page-code.enum';
import { ApplicationBaseDeliveryDTO } from '@app/models/generated/dtos/ApplicationBaseDeliveryDTO';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { RegisterDeliveryDialogParams } from './models/register-delivery-dialog-params.model';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { ApplicationDeliveryTypeDTO } from '@app/models/generated/dtos/ApplicationDeliveryTypeDTO';
import { CommonNomenclatures } from '@app/services/common-app/common-nomenclatures.service';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { DeliveryTypesEnum } from '@app/enums/delivery-types.enum';
import { ErrorCode, ErrorModel } from '@app/models/common/exception.model';
import { ApplicationValidationErrorsEnum } from '@app/enums/application-validation-errors.enum';

@Component({
    selector: 'register-delivery',
    templateUrl: './register-delivery.component.html'
})
export class RegisterDeliveryComponent implements IDialogComponent, OnInit {
    public form!: FormGroup;
    public pageCode!: PageCodeEnum;
    public currentDate: Date = new Date();
    public isDialog: boolean = false;

    public isEDeliveryChosen: boolean = false;
    public refreshFileTypes: Subject<void> = new Subject<void>();
    public pageCodesEnum: typeof PageCodeEnum = PageCodeEnum;
    public deliveryForm: FormGroup = new FormGroup(
        {
            deliveryFilesControl: new FormControl()
        },
        [
            this.userHasEDelivery(),
            this.validUploadedFile()
        ]);

    private hasNoEDeliveryRegistrationError: boolean = false;
    private hasInvalidFile: boolean = false;

    private registerId!: number;

    private deliveryId!: number;
    private model!: ApplicationDeliveryDTO;
    private deliveryService!: IDeliveryService;
    private isPublicApp: boolean = false;
    private rightSideButtons: IActionInfo[] = [];

    private readonly nomenclatures: CommonNomenclatures;

    public constructor(nomenclatures: CommonNomenclatures) {
        this.nomenclatures = nomenclatures;

        this.buildForm();

        this.deliveryForm.get('deliveryFilesControl')!.valueChanges.subscribe({
            next: () => {
                this.hasInvalidFile = false;
                this.deliveryForm.updateValueAndValidity({ emitEvent: false });
            }
        });
    }

    public async ngOnInit(): Promise<void> {
        if (this.deliveryId !== null && this.deliveryId !== undefined) {
            this.deliveryService.getDeliveryData(this.deliveryId).subscribe({
                next: (result: ApplicationDeliveryDTO) => {
                    this.model = result;
                    this.fillForm();
                    this.refreshFileTypes.next();
                }
            });
        }
        else {
            this.model = new ApplicationDeliveryDTO();
        }

        if (this.isPublicApp) {
            this.form.disable();
        }
    }

    public setData(data: RegisterDeliveryDialogParams, buttons: DialogWrapperData): void {
        this.deliveryId = data.deliveryId;
        this.isPublicApp = data.isPublicApp;
        this.deliveryService = data.service;
        this.pageCode = data.pageCode;
        this.registerId = data.registerId;

        if (buttons.rightSideActions !== undefined) {
            buttons.rightSideActions.push({
                id: 'save-and-send',
                color: 'accent',
                translateValue: 'register-delivery.save-and-send-btn',
                hidden: true
            });

            this.rightSideButtons = buttons.rightSideActions;
        }

        this.isDialog = true;
    }

    public saveBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        if (this.isPublicApp) {
            dialogClose();
        }
        else {
            this.fillModelAndSave(false, dialogClose);
        }
    }

    public cancelBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public dialogButtonClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        if (this.isPublicApp) {
            dialogClose();
        }
        else {
            if (actionInfo.id === 'save-and-send') {
                this.fillModelAndSave(true, dialogClose);
            }
            else {
                dialogClose();
            }
        }
    }

    private buildForm(): void {
        this.form = new FormGroup({
            basicDeliveryDataControl: new FormControl(undefined),
            sentDateControl: new FormControl(undefined),
            deliveryDateControl: new FormControl(undefined),
            referenceNumberControl: new FormControl(undefined, Validators.maxLength(50))
        });

        this.form.get('basicDeliveryDataControl')!.valueChanges.subscribe({
            next: async (data: ApplicationBaseDeliveryDTO) => {
                const saveAndSendBtn: IActionInfo | undefined = this.rightSideButtons.find(x => x.id === 'save-and-send');

                if (saveAndSendBtn !== null && saveAndSendBtn !== undefined) {
                    if (data !== undefined && data !== null) {
                        const deliveryTypes: ApplicationDeliveryTypeDTO[] = await NomenclatureStore.instance.getNomenclature(
                            NomenclatureTypes.DeliveryTypes, this.nomenclatures.getDeliveryTypes.bind(this.nomenclatures), false
                        ).toPromise();

                        const deliveryType: ApplicationDeliveryTypeDTO | undefined = deliveryTypes.find(x => x.value === data.deliveryTypeId);

                        if (deliveryType !== undefined && deliveryType.code === DeliveryTypesEnum[DeliveryTypesEnum.eDelivery]) {
                            saveAndSendBtn.hidden = false;
                            this.isEDeliveryChosen = true;
                        }
                        else {
                            saveAndSendBtn.hidden = true;
                            this.isEDeliveryChosen = false;
                        }
                    }
                    else {
                        saveAndSendBtn.hidden = true;
                    }
                }
            }
        });

        this.form.get('deliveryDateControl')!.valueChanges.subscribe({
            next: () => {
                setTimeout(() => { // за да се смени реално стойността на deliveryDateControl и след това чак да се построи съобщението за валидация
                    this.form.get('sentDateControl')!.markAsTouched();
                });
            }
        });
    }

    private fillForm(): void {
        this.form.get('basicDeliveryDataControl')!.setValue(new ApplicationBaseDeliveryDTO({
            id: this.model.id,
            deliveryTypeId: this.model.deliveryTypeId,
            deliveryAddress: this.model.deliveryAddress,
            deliveryEmailAddress: this.model.deliveryEmailAddress,
            deliveryTeritorryUnitId: this.model.deliveryTeritorryUnitId
        }));

        this.form.get('sentDateControl')!.setValue(this.model.sentDate);
        this.form.get('deliveryDateControl')!.setValue(this.model.deliveryDate);
        this.form.get('referenceNumberControl')!.setValue(this.model.referenceNumber);

        this.deliveryForm.get('deliveryFilesControl')!.setValue(this.model.files);
    }

    private fillModel(): ApplicationDeliveryDTO {
        const basicDeliveryData: ApplicationBaseDeliveryDTO = this.form.get('basicDeliveryDataControl')!.value;
        this.model.deliveryTypeId = basicDeliveryData.deliveryTypeId;
        this.model.deliveryAddress = basicDeliveryData.deliveryAddress;
        this.model.deliveryEmailAddress = basicDeliveryData.deliveryEmailAddress;
        this.model.deliveryTeritorryUnitId = basicDeliveryData.deliveryTeritorryUnitId;

        this.model.deliveryDate = this.form.get('deliveryDateControl')!.value;
        this.model.sentDate = this.form.get('sentDateControl')!.value;
        this.model.referenceNumber = this.form.get('referenceNumberControl')!.value;
        this.model.isDelivered = this.model.deliveryDate !== undefined && this.model.deliveryDate !== null;

        this.model.registerId = this.registerId;

        this.model.files = this.deliveryForm.get('deliveryFilesControl')!.value;

        return this.model;
    }

    private fillModelAndSave(send: boolean, dialogClose: DialogCloseCallback): void {
        this.form.markAllAsTouched();

        if (this.isEDeliveryChosen && send) {
            this.deliveryForm.get('deliveryFilesControl')!.markAsTouched();
        }

        if (this.isFormValid(send)) {
            this.fillModel();
            CommonUtils.sanitizeModelStrings(this.model);

            this.deliveryService.editDeliveryData(this.model, send).subscribe({
                next: () => {
                    dialogClose(this.model);
                },
                error: (response: HttpErrorResponse) => {
                    this.handleSaveErrorResponse(response);
                }
            });
        }
    }

    private isFormValid(send: boolean): boolean {
        return this.form.valid
            && (!send || this.deliveryForm.get('deliveryFilesControl')!.valid)
            && (!this.isEDeliveryChosen || !this.hasNoEDeliveryRegistrationError);
    }

    private handleSaveErrorResponse(response: HttpErrorResponse): void {
        if (response.error !== null && response.error !== undefined) {
            const messages: string[] = response.error.messages;
            
            if (messages.find(message => message === ApplicationValidationErrorsEnum[ApplicationValidationErrorsEnum.NoEDeliveryRegistration])) {
                this.hasNoEDeliveryRegistrationError = true;
                this.deliveryForm.updateValueAndValidity({ emitEvent: false });
            }
            else if ((response.error as ErrorModel)?.code === ErrorCode.ApplicationFileInvalid) {
                this.hasInvalidFile = true;
                this.deliveryForm.updateValueAndValidity({ emitEvent: false });
            }
        }
    }

    private userHasEDelivery(): ValidatorFn {
        return (control: AbstractControl): ValidationErrors | null => {
            if (this.hasNoEDeliveryRegistrationError) {
                return { 'noEDeliveryRegistration': true };
            }
            else {
                return null;
            }
        }
    }

    private validUploadedFile(): ValidatorFn {
        return (control: AbstractControl): ValidationErrors | null => {
            if (this.hasInvalidFile) {
                return { 'userUploadedInvalidFile': true };
            }
            else {
                return null;
            }
        }
    }
}