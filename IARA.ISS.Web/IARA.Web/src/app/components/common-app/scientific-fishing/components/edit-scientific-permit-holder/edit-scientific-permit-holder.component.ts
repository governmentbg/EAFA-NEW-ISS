import { AfterViewInit, Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';

import { IScientificFishingService } from '@app/interfaces/common-app/scientific-fishing.interface';
import { RegixPersonDataDTO } from '@app/models/generated/dtos/RegixPersonDataDTO';
import { ScientificFishingPermitHolderDTO } from '@app/models/generated/dtos/ScientificFishingPermitHolderDTO';
import { ScientificFishingPermitHolderRegixDataDTO } from '@app/models/generated/dtos/ScientificFishingPermitHolderRegixDataDTO';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { TLPictureRequestMethod } from '@app/shared/components/tl-picture-uploader/tl-picture-uploader.component';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { AddressTypesEnum } from '@app/enums/address-types.enum';
import { AddressRegistrationDTO } from '@app/models/generated/dtos/AddressRegistrationDTO';
import { FileInfoDTO } from '@app/models/generated/dtos/FileInfoDTO';
import { Notifier } from '@app/shared/directives/notifier/notifier.class';
import { EditScientificPermitHolderDialogParams } from '../../models/edit-scientific-permit-holder-dialog-params';
import { PersonFullDataDTO } from '@app/models/generated/dtos/PersonFullDataDTO';

@Component({
    selector: 'edit-scientific-permit-holder',
    templateUrl: './edit-scientific-permit-holder.component.html',
    styleUrls: ['./edit-scientific-permit-holder.component.scss']
})
export class EditScientificPermitHolderComponent implements OnInit, AfterViewInit, IDialogComponent {
    public readonly addressTypes: typeof AddressTypesEnum = AddressTypesEnum;

    public form!: FormGroup;
    public model!: ScientificFishingPermitHolderDTO | ScientificFishingPermitHolderRegixDataDTO;
    public isEditing!: boolean;
    public showOnlyRegiXData: boolean = false;
    public photoRequestMethod?: TLPictureRequestMethod;

    public notifier: Notifier = new Notifier();
    public expectedResultPerson: RegixPersonDataDTO | undefined;
    public expectedResultAddress: AddressRegistrationDTO | undefined;

    public showAddress: boolean = true;

    private service!: IScientificFishingService;
    private requestDate?: Date;
    private readOnly: boolean = false;
    private isTouched: boolean = false;

    public constructor() {
        this.buildForm();
    }

    public ngOnInit(): void {
        if (this.model?.id !== undefined) {
            this.photoRequestMethod = this.service.getPermitHolderPhoto.bind(this.service, this.model.id);
        }
    }

    public ngAfterViewInit(): void {
        this.fillForm();
    }

    public saveBtnClicked(action: IActionInfo, dialogClose: DialogCloseCallback): void {
        if (this.readOnly) {
            dialogClose({ holder: this.model, isTouched: this.isTouched });
        }
        else {
            this.form.markAllAsTouched();

            if (this.form.valid || this.showOnlyRegiXData) {
                this.fillModel();
                CommonUtils.sanitizeModelStrings(this.model);
                dialogClose({ holder: this.model, isTouched: this.isTouched });
            }
        }
    }

    public cancelBtnClicked(action: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public dialogButtonClicked(action: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public setData(data: EditScientificPermitHolderDialogParams, buttons: DialogWrapperData): void {
        this.service = data.service;
        this.readOnly = data.readOnly;
        this.isEditing = data.isEgnLncReadOnly;
        this.requestDate = data.requestDate;
        this.showOnlyRegiXData = data.showOnlyRegiXData;

        if (data.expectedResults !== null && data.expectedResults !== undefined) {
            this.expectedResultPerson = data.expectedResults?.regixPersonData;
            this.expectedResultAddress = data.expectedResults?.addressRegistrations ? data.expectedResults?.addressRegistrations[0] : undefined;
        }

        if (data.model === undefined) {
            if (this.showOnlyRegiXData) {
                this.model = new ScientificFishingPermitHolderRegixDataDTO({ isActive: true });
            }
            else {
                this.model = new ScientificFishingPermitHolderDTO({ isActive: true });
            }
        }
        else {
            this.model = data.model;
        }

        if (this.readOnly) {
            this.form.disable();
        }
    }

    public downloadedPersonData(person: PersonFullDataDTO): void {
        this.form.get('regixDataControl')!.setValue(person.person);

        if (person.addresses && person.addresses.length !== 0) {
            this.form.get('noAddressProvidedControl')!.setValue(false);
            this.form.get('addressControl')!.setValue(person.addresses[0]);
        }

        this.form.get('photoControl')!.setValue(person.photo);
    }

    private buildForm(): void {
        if (!this.showOnlyRegiXData) {
            this.form = new FormGroup({
                permitNumberControl: new FormControl({ value: '', disabled: true }),
                permitCreatedDateControl: new FormControl({ value: new Date(), disabled: true }),
                organizationPositionControl: new FormControl(null, [Validators.maxLength(500), Validators.required]),
                noAddressProvidedControl: new FormControl(false),
                addressControl: new FormControl(null, Validators.required),
                regixDataControl: new FormControl(null, Validators.required),
                photoControl: new FormControl(null, Validators.required)
            });
        }
        else {
            this.form = new FormGroup({
                addressControl: new FormControl(null, Validators.required),
                regixDataControl: new FormControl(null, Validators.required),
                photoControl: new FormControl(null)
            });
        }

        this.form.get('noAddressProvidedControl')?.valueChanges.subscribe({
            next: (value: boolean) => {
                this.showAddress = !value;

                if (value) {
                    this.form.get('addressControl')!.clearValidators();
                }
                else {
                    this.form.get('addressControl')!.setValidators(Validators.required);
                }

                this.form.get('addressControl')!.updateValueAndValidity({ emitEvent: false });
            }
        });
    }

    private fillForm(): void {
        if (this.requestDate !== null && this.requestDate !== undefined && this.model instanceof ScientificFishingPermitHolderDTO) {
            this.form.get('permitCreatedDateControl')!.setValue(this.requestDate);
        }

        if (this.model !== undefined) {
            if (this.model instanceof ScientificFishingPermitHolderDTO) {
                this.form.get('permitNumberControl')!.setValue(this.model.permitNumber);
                this.form.get('organizationPositionControl')!.setValue(this.model.scientificPosition);

                if (this.model.photoBase64 && this.model.photoBase64.length > 0) {
                    this.form.get('photoControl')!.setValue(this.model.photoBase64);
                }
                else {
                    this.form.get('photoControl')!.setValue(this.model.photo);
                }
            }
            else {
                if (!this.readOnly) {
                    this.notifier.start();
                    this.notifier.onNotify.subscribe({
                        next: () => {
                            this.form.markAllAsTouched();
                            this.notifier.stop();

                            this.form.valueChanges.subscribe({
                                next: () => {
                                    this.isTouched = true;
                                }
                            });
                        }
                    });
                }
            }

            this.form.get('regixDataControl')!.setValue(this.model.regixPersonData);

            const addresses: AddressRegistrationDTO[] = this.model.addressRegistrations ?? [];
            if (addresses.length > 0) {
                this.form.get('addressControl')!.setValue(addresses[0]);
                this.showAddress = true;
            }
            else {
                this.form.get('noAddressProvidedControl')!.setValue(true);
            }
        }
    }

    private fillModel(): void {
        if (this.model instanceof ScientificFishingPermitHolderDTO) {
            this.model.scientificPosition = this.form.get('organizationPositionControl')!.value;

            const photo: FileInfoDTO | string | null = this.form.get('photoControl')!.value;
            if (photo !== undefined && photo !== null) {
                if (typeof photo === 'string') {
                    this.model.photoBase64 = photo;
                }
                else {
                    this.model.photo = photo;
                }
            }
            else {
                this.model.photo = undefined;
                this.model.photoBase64 = undefined;
            }
        }

        this.model.regixPersonData = this.form.get('regixDataControl')!.value;

        const hasAddress: boolean = this.form.get('noAddressProvidedControl')!.value !== true;
        if (hasAddress) {
            const address: AddressRegistrationDTO | undefined = this.form.get('addressControl')!.value;
            if (address !== undefined && address !== null) {
                this.model.addressRegistrations = [address];
            }
            else {
                this.model.addressRegistrations = [];
            }
        }
        else {
            this.model.addressRegistrations = [];
        }

        this.model.hasValidationErrors = !this.form.valid;

        this.fixHolderNames();
    }

    private fixHolderNames(): void {
        if (this.model.regixPersonData) {
            this.model.name = '';

            if (this.model.regixPersonData!.firstName !== undefined && this.model.regixPersonData!.firstName !== null) {
                this.model.name = `${this.model.regixPersonData!.firstName}`;
            }
            if (this.model.regixPersonData!.middleName !== undefined && this.model.regixPersonData!.middleName !== null) {
                this.model.name = `${this.model.name} ${this.model.regixPersonData!.middleName}`;
            }
            if (this.model.regixPersonData!.lastName !== undefined && this.model.regixPersonData!.lastName !== null) {
                this.model.name = `${this.model.name} ${this.model.regixPersonData!.lastName}`;
            }

            this.model.egn = this.model.regixPersonData!.egnLnc?.egnLnc;
        }
    }
}