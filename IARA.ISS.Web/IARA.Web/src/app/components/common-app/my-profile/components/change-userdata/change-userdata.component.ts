import { AfterViewInit, Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { ChangeUserDataDTO } from '@app/models/generated/dtos/ChangeUserDataDTO';
import { EgnLncDTO } from '@app/models/generated/dtos/EgnLncDTO';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { PersonDocumentDTO } from '@app/models/generated/dtos/PersonDocumentDTO';
import { CommonNomenclatures } from '@app/services/common-app/common-nomenclatures.service';
import { UsersService } from '@app/services/common-app/users.service';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { TLValidators } from '@app/shared/utils/tl-validators';
import { forkJoin } from 'rxjs';

@Component({
    selector: 'change-userdata',
    templateUrl: './change-userdata.component.html'
})
export class ChangeUserDataComponent implements IDialogComponent, OnInit, AfterViewInit {

    public showDistricts: boolean = false;
    public showAllDistricts: boolean = false;
    public passwordIcon: string = "fa-eye";
    public passwordConfirmationIcon: string = "fa-eye";
    public userMustChangePassword: boolean;

    private userModel!: ChangeUserDataDTO;

    private userService!: UsersService;
    private nomenclaturesService: CommonNomenclatures;

    public documentTypes: NomenclatureDTO<number>[] = [];
    public countries: NomenclatureDTO<number>[] = [];
    public genders: NomenclatureDTO<number>[] = [];
    public districts!: NomenclatureDTO<number>[];
    public changeUserDataForm!: FormGroup;

    public constructor(nomenclaturesService: CommonNomenclatures, userService: UsersService) {
        this.userService = userService;
        this.nomenclaturesService = nomenclaturesService;
        this.buildForm();
        this.userMustChangePassword = true;
    }

    public async ngOnInit(): Promise<void> {
        const nomenclatures: NomenclatureDTO<number>[][] = await forkJoin(
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.Countries, this.nomenclaturesService.getCountries.bind(this.nomenclaturesService), false),
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.DocumentTypes, this.nomenclaturesService.getDocumentTypes.bind(this.nomenclaturesService), false),
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.Genders, this.nomenclaturesService.getGenders.bind(this.nomenclaturesService), true),
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.Districts, this.nomenclaturesService.getDistricts.bind(this.nomenclaturesService), false)
        ).toPromise();

        this.countries = nomenclatures[0];
        this.documentTypes = nomenclatures[1];
        this.genders = nomenclatures[2];
        this.districts = nomenclatures[3];
    }

    public ngAfterViewInit(): void {
        this.changeUserDataForm.controls.checkboxControl.valueChanges.subscribe({
            next: (checked: boolean) => {
                if (checked) {
                    this.changeUserDataForm.controls.citizenshipControl.setValue(this.countries.find(x => x.code === 'BGR'));
                }
                else {
                    this.changeUserDataForm.controls.birthdateControl.setValidators(Validators.required);
                }
            }
        });

        this.changeUserDataForm.controls.hasNoDistrictControl.valueChanges.subscribe({
            next: (hasNoDistrictNewState: boolean) => {
                if (hasNoDistrictNewState) {
                    this.showDistricts = false;
                    this.changeUserDataForm.controls.districtsControl.clearValidators();
                    this.changeUserDataForm.controls.districtsControl.setValue(null);
                    this.changeUserDataForm.controls.districtsControl.disable();
                    this.changeUserDataForm.controls.districtsControl.setValue(this.districts);
                }
                else {
                    this.showDistricts = true;
                    this.changeUserDataForm.controls.districtsControl.setValidators(Validators.required);
                    this.changeUserDataForm.controls.districtsControl.enable();
                    this.changeUserDataForm.controls.districtsControl.setValue(null);
                }

                this.changeUserDataForm.controls.districtsControl.markAsPending();
            }
        });
    }

    public setData(data: { userData: ChangeUserDataDTO, userMustChangePassword: boolean }, wrapperData: DialogWrapperData): void {
        this.userModel = data.userData;
        //this.userMustChangePassword = data.userMustChangePassword;
        this.fillForm(this.userModel);
    }

    public dialogButtonClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
      
        dialogClose();
    }

    public saveBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        if (actionInfo.id === 'save') {
            this.changeUserDataForm.markAllAsTouched();
            if (this.changeUserDataForm.valid) {

                this.userModel = this.fillModel(this.changeUserDataForm);
                this.userService.updateUserData(this.userModel).subscribe(() => {
                    dialogClose(this.userModel);
                });
            }
        }
    }

    public cancelBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        if (!actionInfo.disabled) {
            dialogClose();
        }
    }

    public showOrHidePassword(formControlName: string): void {
        if (formControlName === 'password') {
            this.passwordIcon === "fa-eye" ? this.passwordIcon = "fa-eye-slash" : this.passwordIcon = "fa-eye";
        } else if (formControlName === 'passwordConfirmation') {
            this.passwordConfirmationIcon === "fa-eye" ? this.passwordConfirmationIcon = "fa-eye-slash" : this.passwordConfirmationIcon = "fa-eye";
        }
    }

    private buildForm(): void {
        this.changeUserDataForm = new FormGroup({
            addressesControl: new FormControl(),
            usernameControl: new FormControl('', [Validators.required, Validators.email]),
            egnControl: new FormControl('', [Validators.required, Validators.minLength(10), Validators.maxLength(10), Validators.pattern('[0-9]+')]),
            birthdateControl: new FormControl(),
            checkboxControl: new FormControl(),
            citizenshipControl: new FormControl(),
            genderControl: new FormControl(),
            documentTypeControl: new FormControl(),
            documentIssueDateControl: new FormControl(),
            documentNumControl: new FormControl('', [Validators.maxLength(50)]),
            documentIssuerControl: new FormControl('', [Validators.maxLength(50)]),
            firstNameControl: new FormControl('', [Validators.required, Validators.maxLength(200)]),
            middleNameControl: new FormControl('', Validators.maxLength(200)),
            lastNameControl: new FormControl('', [Validators.required, Validators.maxLength(200)]),
            phoneControl: new FormControl('', Validators.maxLength(50)),
            notificationNewsControl: new FormControl(),
            hasNoDistrictControl: new FormControl(),
            districtsControl: new FormControl(),
            passwordControl: new FormControl('', [
                Validators.required,
                Validators.maxLength(64),
                TLValidators.passwordComplexityValidator()]),
            passwordConfirmationControl: new FormControl('', [
                Validators.required,
                TLValidators.confirmPasswordValidator,
                Validators.maxLength(64),
                TLValidators.passwordComplexityValidator()]),
        });
    }

    private fillForm(model: ChangeUserDataDTO): void {
        this.changeUserDataForm.controls.usernameControl.setValue(model.username);
        this.changeUserDataForm.controls.birthdateControl.setValue(model.birthDate);
        this.changeUserDataForm.controls.firstNameControl.setValue(model.firstName);
        this.changeUserDataForm.controls.middleNameControl.setValue(model.middleName);
        this.changeUserDataForm.controls.lastNameControl.setValue(model.lastName);
        this.changeUserDataForm.controls.phoneControl.setValue(model.phone);
        this.changeUserDataForm.controls.egnControl.setValue(model.egnLnc?.egnLnc);
        this.changeUserDataForm.controls.addressesControl.setValue(model.userAddresses);

        this.changeUserDataForm.controls.citizenshipControl.setValue(this.countries.find(x => x.value === model.citizenshipCountryId));
        this.changeUserDataForm.controls.documentTypeControl.setValue(this.documentTypes.find(x => x.value === model.document?.documentTypeID));
        this.changeUserDataForm.controls.genderControl.setValue(this.genders.find(x => x.value === model.genderId));

        this.changeUserDataForm.controls.documentNumControl.setValue(model.document?.documentNumber);
        this.changeUserDataForm.controls.documentIssueDateControl.setValue(model.document?.documentIssuedOn);
        this.changeUserDataForm.controls.documentIssuerControl.setValue(model.document?.documentIssuedBy);
        this.changeUserDataForm.controls.checkboxControl.setValue(model.hasBulgarianAddressRegistration);
    }

    private fillModel(form: FormGroup): ChangeUserDataDTO {
        this.userModel.userAddresses = form.controls.addressesControl.value;
        this.userModel.birthDate = form.controls.birthdateControl.value;
        this.userModel.phone = form.controls.phoneControl.value;
        this.userModel.firstName = form.controls.firstNameControl.value;
        this.userModel.middleName = form.controls.middleNameControl.value;
        this.userModel.lastName = form.controls.lastNameControl.value;
        this.userModel.citizenshipCountryId = form.controls.citizenshipControl.value?.value;
        this.userModel.hasBulgarianAddressRegistration = form.controls.checkboxControl.value;
        this.userModel.genderId = form.controls.genderControl.value?.value;
        this.userModel.password = form.controls.passwordControl.value;
        this.userModel.email = form.controls.usernameControl.value;
        this.userModel.username = form.controls.usernameControl.value;


        const personDoc: PersonDocumentDTO = new PersonDocumentDTO();
        personDoc.documentTypeID = form.controls.documentTypeControl?.value?.value ?? undefined;
        personDoc.documentNumber = form.controls.documentNumControl?.value ?? undefined;
        personDoc.documentIssuedOn = form.controls.documentIssueDateControl?.value ?? undefined;
        personDoc.documentIssuedBy = form.controls.documentIssuerControl?.value ?? undefined;
        this.userModel.document = personDoc;
        this.userModel.egnLnc = form.controls.egnControl.value as EgnLncDTO;

        return this.userModel;
    }


}