import { Component, OnInit } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, ValidationErrors, ValidatorFn } from '@angular/forms';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { AuanDeliveryDataDTO } from '@app/models/generated/dtos/AuanDeliveryDataDTO';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { HttpErrorResponse } from '@angular/common/http';
import { ErrorCode } from '@app/models/common/exception.model';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { Subject } from 'rxjs';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { InspDeliveryTypesNomenclatureDTO } from '@app/models/generated/dtos/InspDeliveryTypesNomenclatureDTO';
import { InspDeliveryTypesEnum } from '@app/enums/insp-delivery-types.enum';
import { InspDeliveryDataDialogParams } from '../models/insp-delivery-data-dialog-params.model';
import { PageCodeEnum } from '@app/enums/page-code.enum';
import { IInspDeliveryService } from '@app/interfaces/administration-app/insp-delivery.interface';

@Component({
    selector: 'auan-delivery',
    templateUrl: './auan-delivery.component.html'
})
export class AuanDeliveryComponent implements IDialogComponent, OnInit {
    public form!: FormGroup;
    public currentDate: Date = new Date();

    public refreshFileTypes: Subject<void> = new Subject<void>();
    public hasNoEDeliveryRegistrationError: boolean = false;
    public isEDeliveryChosen: boolean = false;
    public isAuan: boolean = false;

    public pageCodesEnum: typeof PageCodeEnum = PageCodeEnum;

    public deliveryForm: FormGroup = new FormGroup(
        {
            deliveryFilesControl: new FormControl()
        }, this.userHasEDelivery());


    private registerId: number | undefined;

    private deliveryId: number | undefined;
    private service!: IInspDeliveryService;
    private model!: AuanDeliveryDataDTO;
    private rightSideButtons: IActionInfo[] = [];

    public constructor() {
        this.buildForm();
    }

    public async ngOnInit(): Promise<void> {
        if (this.deliveryId !== null && this.deliveryId !== undefined) {
            this.service.getDeliveryData(this.deliveryId).subscribe({
                next: (result: AuanDeliveryDataDTO) => {
                    this.model = result;
                    this.fillForm();
                    this.refreshFileTypes.next();
                }
            });
        }
        else {
            this.model = new AuanDeliveryDataDTO();
        }
    }

    public setData(data: InspDeliveryDataDialogParams, buttons: DialogWrapperData): void {
        this.registerId = data.registerId;
        this.deliveryId = data.id;
        this.isAuan = data.isAuan;
        this.service = data.service;

        if (buttons.rightSideActions !== undefined) {
            buttons.rightSideActions.push({
                id: 'save-and-send',
                color: 'accent',
                translateValue: 'register-delivery.save-and-send-btn',
                hidden: true
            });

            this.rightSideButtons = buttons.rightSideActions;
        }

        if (data.viewMode) {
            this.form.disable();
        }
    }

    public dialogButtonClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        if (actionInfo.id === 'save-and-send') {
            this.fillModelAndSave(true, dialogClose);
        }
        else {
            dialogClose();
        }
    }

    public saveBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        this.fillModelAndSave(false, dialogClose);
    }

    public cancelBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    private buildForm(): void {
        this.form = new FormGroup({
            deliveryDataControl: new FormControl()
        });

        this.form.get('deliveryDataControl')!.valueChanges.subscribe({
            next: async (data: AuanDeliveryDataDTO) => {
                const saveAndSendBtn: IActionInfo | undefined = this.rightSideButtons.find(x => x.id === 'save-and-send');

                if (saveAndSendBtn !== null && saveAndSendBtn !== undefined) {
                    if (data !== undefined && data !== null) {
                        const deliveryTypes: InspDeliveryTypesNomenclatureDTO[] = await NomenclatureStore.instance.getNomenclature(
                            NomenclatureTypes.InspDeliveryTypes, this.service.getDeliveryTypes.bind(this.service), false
                        ).toPromise();

                        const deliveryType: InspDeliveryTypesNomenclatureDTO | undefined = deliveryTypes.find(x => x.code === InspDeliveryTypesEnum[data.deliveryType!]);
                        const isEDeliveryRequested: boolean = data.isEDeliveryRequested ?? false;

                        if (isEDeliveryRequested || this.isEDeliveryRequested(deliveryType)) {
                            if (this.deliveryId !== undefined && this.deliveryId !== null) {
                                saveAndSendBtn.hidden = false;
                                this.isEDeliveryChosen = true;
                            }
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
    }

    private fillForm(): void {
        this.form.get('deliveryDataControl')!.setValue(this.model);
        this.deliveryForm.get('deliveryFilesControl')!.setValue(this.model.files);
    }

    private fillModel(): void {
        this.model = this.form.get('deliveryDataControl')!.value;
        this.model.files = this.deliveryForm.get('deliveryFilesControl')!.value;
    }

    private fillModelAndSave(send: boolean, dialogClose: DialogCloseCallback): void {
        this.form.markAllAsTouched();

        if (this.isEDeliveryChosen && send) {
            this.deliveryForm.get('deliveryFilesControl')!.markAsTouched();
        }

        if (this.isFormValid(send)) {
            this.fillModel();
            CommonUtils.sanitizeModelStrings(this.model);

            if (this.deliveryId !== undefined && this.deliveryId !== null) {
                this.service.editDeliveryData(this.registerId!, this.model, send).subscribe({
                    next: () => {
                        dialogClose(this.model);
                    },
                    error: (response: HttpErrorResponse) => {
                        this.handleSaveErrorResponse(response);
                    }
                });
            }
            else {
                this.service.addDeliveryData(this.registerId!, this.model).subscribe({
                    next: (id: number) => {
                        this.model.id = id;
                        dialogClose(this.model);
                    },
                    error: (response: HttpErrorResponse) => {
                        this.handleSaveErrorResponse(response);
                    }
                });
            }
        }
    }

    private isFormValid(send: boolean): boolean {
        return this.form.valid
            && (!send || this.deliveryForm.get('deliveryFilesControl')!.valid)
            && (!this.isEDeliveryChosen || !this.hasNoEDeliveryRegistrationError);
    }

    private isEDeliveryRequested(deliveryType: InspDeliveryTypesNomenclatureDTO | undefined): boolean {
        return (deliveryType !== undefined && deliveryType !== null)
            && (deliveryType.code === InspDeliveryTypesEnum[InspDeliveryTypesEnum.EDelivery]
                || deliveryType.code === InspDeliveryTypesEnum[InspDeliveryTypesEnum.DecreeEDelivery]);
    }

    private handleSaveErrorResponse(response: HttpErrorResponse): void {
        if (response.error !== null && response.error !== undefined) {
            if (response.error?.code === ErrorCode.NoEDeliveryRegistration) {
                this.hasNoEDeliveryRegistrationError = true;
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
}