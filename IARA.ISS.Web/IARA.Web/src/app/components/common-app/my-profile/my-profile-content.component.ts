import { AfterViewInit, Component, Input, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { NewsSubscriptionTypes } from '@app/enums/news-subscription-types.enum';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { UserLegalStatusEnum } from '@app/enums/user-legal-status.enum';
import { IMyProfileService } from '@app/interfaces/common-app/my-profile.interface';
import { DialogParamsModel } from '@app/models/common/dialog-params.model';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { MyProfileDTO } from '@app/models/generated/dtos/MyProfileDTO';
import { PersonDocumentDTO } from '@app/models/generated/dtos/PersonDocumentDTO';
import { RoleDTO } from '@app/models/generated/dtos/RoleDTO';
import { UserAuthDTO } from '@app/models/generated/dtos/UserAuthDTO';
import { UserLegalDTO } from '@app/models/generated/dtos/UserLegalDTO';
import { UserNewsDistrictSubscriptionDTO } from '@app/models/generated/dtos/UserNewsDistrictSubscriptionDTO';
import { UserPasswordDTO } from '@app/models/generated/dtos/UserPasswordDTO';
import { CommonNomenclatures } from '@app/services/common-app/common-nomenclatures.service';
import { DialogCloseCallback } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { TLMatDialog } from '@app/shared/components/dialog-wrapper/tl-mat-dialog';
import { TLPictureRequestMethod } from '@app/shared/components/tl-picture-uploader/tl-picture-uploader.component';
import { AuthService } from '@app/shared/services/auth.service';
import { MessageService } from '@app/shared/services/message.service';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { forkJoin } from 'rxjs';
import { BasePageComponent } from '../base-page.component';
import { ChangePasswordComponent } from './components/change-password/change-password.component';

@Component({
    selector: 'my-profile-content',
    templateUrl: './my-profile-content.component.html',
    styleUrls: ['./my-profile-content.component.scss']
})
export class MyProfileContentComponent extends BasePageComponent implements OnInit, AfterViewInit {
    @Input()
    public isPublicApp!: boolean;

    @Input()
    private service!: IMyProfileService;

    public translationService: FuseTranslationLoaderService;
    public userForm!: FormGroup;

    public photoRequestMethod?: TLPictureRequestMethod;

    public roles: RoleDTO[] = [];
    public legals: UserLegalDTO[] = [];

    public documentTypes: NomenclatureDTO<number>[] = [];
    public countries: NomenclatureDTO<number>[] = [];
    public genders: NomenclatureDTO<number>[] = [];
    public districts!: NomenclatureDTO<number>[];

    public showDistricts: boolean = false;
    public showAllDistricts: boolean = false;

    private authService: AuthService;
    private nomenclaturesService: CommonNomenclatures;
    private changePasswordDialog: TLMatDialog<ChangePasswordComponent>;

    private userProfileModel!: MyProfileDTO;
    private personId!: number;

    private hasChanges: boolean = false;

    public readonly legalStatusEnum: typeof UserLegalStatusEnum = UserLegalStatusEnum;

    public constructor(
        translationService: FuseTranslationLoaderService,
        authService: AuthService,
        messageService: MessageService,
        nomenclaturesService: CommonNomenclatures,
        changePasswordDialog: TLMatDialog<ChangePasswordComponent>
    ) {
        super(messageService);

        this.translationService = translationService;
        this.authService = authService;
        this.nomenclaturesService = nomenclaturesService;
        this.changePasswordDialog = changePasswordDialog;

        this.buildForm();
        this.messageService.sendMessage(this.translationService.getValue('navigation.my-profile'));
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

        this.authService.userRegistrationInfoEvent.subscribe({
            next: (userInfo: UserAuthDTO | null) => {
                if (userInfo !== null) {
                    this.service.getUserProfile(this.authService.userRegistrationInfo!.id!).subscribe({
                        next: (result: MyProfileDTO) => {
                            this.userProfileModel = result;

                            setTimeout(() => {
                                if (result.roles) {
                                    this.roles = result.roles;
                                }

                                if (result.legals) {
                                    this.legals = result.legals;
                                    for (const legal of this.legals) {
                                        legal.statusName = this.translationService.getValue(`my-profile.${UserLegalStatusEnum[legal.status!].toLowerCase()}`);
                                    }
                                }
                            });

                            if (result.id) {
                                this.personId = result.id;
                                this.photoRequestMethod = this.service.getUserPhoto.bind(this.service, result.id);
                            }

                            this.fillForm(this.userProfileModel);

                            setTimeout(() => {
                                this.userForm.valueChanges.subscribe({
                                    next: () => {
                                        this.hasChanges = true;
                                    }
                                });
                            }, 1000);
                        }
                    })
                }
            }
        });
    }

    public ngAfterViewInit(): void {
        this.userForm.controls.checkboxControl.valueChanges.subscribe({
            next: (checked: boolean) => {
                if (checked) {
                    this.userForm.controls.citizenshipControl.setValue(this.countries.find(x => x.code === 'BGR'));
                }
                else {
                    this.userForm.controls.birthdateControl.setValidators(Validators.required);
                }
            }
        });

        this.userForm.controls.notificationNewsControl.valueChanges.subscribe({
            next: (checked: boolean) => {
                if (checked) {
                    this.showDistricts = true;
                    this.showAllDistricts = true;
                    this.userForm.controls.districtsControl.setValidators(Validators.required);
                    this.userForm.controls.districtsControl.setValue(null);
                }
                else {
                    this.showDistricts = false;
                    this.showAllDistricts = false;
                    this.userForm.controls.districtsControl.clearValidators();
                    this.userForm.controls.districtsControl.setValue(null);
                }
            }
        });

        this.userForm.controls.hasNoDistrictControl.valueChanges.subscribe({
            next: (hasNoDistrictNewState: boolean) => {
                if (hasNoDistrictNewState) {
                    this.showDistricts = false;
                    this.userForm.controls.districtsControl.clearValidators();
                    this.userForm.controls.districtsControl.setValue(null);
                    this.userForm.controls.districtsControl.disable();
                    this.userForm.controls.districtsControl.setValue(this.districts);
                }
                else {
                    this.showDistricts = true;
                    this.userForm.controls.districtsControl.setValidators(Validators.required);
                    this.userForm.controls.districtsControl.enable();
                    this.userForm.controls.districtsControl.setValue(null);
                }
                this.userForm.controls.districtsControl.markAsPending();
            }
        });
    }

    public updateProfileData(): void {
        if (this.hasChanges === true) {
            this.userForm.markAllAsTouched();

            if (this.userForm.valid) {
                this.fillModel(this.userForm);
                CommonUtils.sanitizeModelStrings(this.userProfileModel);

                this.service.updateUserProfile(this.userProfileModel).subscribe({
                    next: () => {
                        this.hasChanges = false;
                    }
                });
            }
        }
    }

    public changePassword(): void {
        const dialog = this.changePasswordDialog.open({
            title: this.translationService.getValue('my-profile.change-password-dialog-title'),
            TCtor: ChangePasswordComponent,
            headerCancelButton: {
                cancelBtnClicked: this.closeDialogBtnClicked.bind(this)
            },
            componentData: new DialogParamsModel({
                id: this.personId,
                isApplication: false,
                isReadonly: false,
                service: this.service
            }),
            translteService: this.translationService,
            disableDialogClose: true,
            saveBtn: {
                id: 'save',
                color: 'accent',
                translateValue: this.translationService.getValue('my-profile.change')
            },
            cancelBtn: {
                id: 'cancel',
                color: 'primary',
                translateValue: this.translationService.getValue('common.cancel'),
            },
        }, '800px');

        dialog.subscribe({
            next: (password: UserPasswordDTO | undefined) => {
                // nothing to do
            }
        });
    }

    private buildForm(): void {
        this.userForm = new FormGroup({
            photoControl: new FormControl(null),
            addressesControl: new FormControl(null),
            usernameControl: new FormControl({ value: null, disabled: true }),
            egnControl: new FormControl({ value: null, disabled: true }),
            birthdateControl: new FormControl(null),
            checkboxControl: new FormControl(null),
            citizenshipControl: new FormControl(null),
            genderControl: new FormControl(null),
            documentTypeControl: new FormControl(null),
            documentIssueDateControl: new FormControl(null),
            documentNumControl: new FormControl(null, [Validators.maxLength(50)]),
            documentIssuerControl: new FormControl(null, [Validators.maxLength(50)]),
            firstNameControl: new FormControl(null, [Validators.required, Validators.maxLength(200)]),
            middleNameControl: new FormControl(null, Validators.maxLength(200)),
            lastNameControl: new FormControl(null, [Validators.required, Validators.maxLength(200)]),
            phoneControl: new FormControl(null, Validators.maxLength(50)),
            notificationNewsControl: new FormControl(),
            hasNoDistrictControl: new FormControl(),
            districtsControl: new FormControl()
        });
    }

    private fillForm(model: MyProfileDTO): void {
        this.userForm.controls.usernameControl.setValue(model.username);
        this.userForm.controls.birthdateControl.setValue(model.birthDate);
        this.userForm.controls.firstNameControl.setValue(model.firstName);
        this.userForm.controls.middleNameControl.setValue(model.middleName);
        this.userForm.controls.lastNameControl.setValue(model.lastName);
        this.userForm.controls.phoneControl.setValue(model.phone);
        this.userForm.controls.egnControl.setValue(model.egnLnc!.egnLnc);
        this.userForm.controls.photoControl.setValue(model.photo);
        this.userForm.controls.addressesControl.setValue(model.userAddresses);

        this.userForm.controls.citizenshipControl.setValue(this.countries.find(x => x.value === model.citizenshipCountryId));
        this.userForm.controls.documentTypeControl.setValue(this.documentTypes.find(x => x.value === model.document?.documentTypeID));
        this.userForm.controls.genderControl.setValue(this.genders.find(x => x.value === model.genderId));

        this.userForm.controls.documentNumControl.setValue(model.document?.documentNumber);
        this.userForm.controls.documentIssueDateControl.setValue(model.document?.documentIssuedOn);
        this.userForm.controls.documentIssuerControl.setValue(model.document?.documentIssuedBy);
        this.userForm.controls.checkboxControl.setValue(model.hasBulgarianAddressRegistration);

        if (model.newsSubscription === NewsSubscriptionTypes.Districts) {
            this.showDistricts = true;
            const selectedDistricts: NomenclatureDTO<number>[] = [];
            this.userForm.controls.notificationNewsControl.setValue(true);
            this.userForm.controls.districtsControl.enable();
            for (const nds of model.newsDistrictSubscriptions!) {
                const modelDistrict = this.districts.find(district => nds.id === district.value);
                selectedDistricts.push(modelDistrict!);

            }
            this.userForm.controls.districtsControl.setValue(selectedDistricts);
        } else if (model.newsSubscription === NewsSubscriptionTypes.ALL) {
            this.showDistricts = true;
            this.userForm.controls.notificationNewsControl.setValue(true);
            this.userForm.controls.hasNoDistrictControl.setValue(true);
            this.userForm.controls.districtsControl.enable();
            this.userForm.controls.districtsControl.setValue(this.districts);
        } else if (model.newsSubscription === NewsSubscriptionTypes.None) {
            this.userForm.controls.notificationNewsControl.setValue(false);
        }
    }

    private fillModel(form: FormGroup): void {
        this.userProfileModel.photo = form.controls.photoControl.value;
        this.userProfileModel.userAddresses = form.controls.addressesControl.value;
        this.userProfileModel.birthDate = form.controls.birthdateControl.value;
        this.userProfileModel.phone = form.controls.phoneControl.value;
        this.userProfileModel.firstName = form.controls.firstNameControl.value;
        this.userProfileModel.middleName = form.controls.middleNameControl.value;
        this.userProfileModel.lastName = form.controls.lastNameControl.value;
        this.userProfileModel.citizenshipCountryId = form.controls.citizenshipControl.value?.value;
        this.userProfileModel.hasBulgarianAddressRegistration = form.controls.checkboxControl.value;
        this.userProfileModel.genderId = form.controls.genderControl.value?.value;

        const personDoc: PersonDocumentDTO = new PersonDocumentDTO();
        personDoc.documentTypeID = form.controls.documentTypeControl?.value?.value ?? undefined;
        personDoc.documentNumber = form.controls.documentNumControl?.value ?? undefined;
        personDoc.documentIssuedOn = form.controls.documentIssueDateControl?.value ?? undefined;
        personDoc.documentIssuedBy = form.controls.documentIssuerControl?.value ?? undefined;

        this.userProfileModel.document = personDoc;

        const newsDistrictSubscriptions: UserNewsDistrictSubscriptionDTO[] = [];
        if (!CommonUtils.isNullOrEmpty(form.controls.districtsControl.value)) {
            for (const district of form.controls.districtsControl.value) {
                const newsDistrictSubscription = new UserNewsDistrictSubscriptionDTO();
                newsDistrictSubscription.id = district.value;
                newsDistrictSubscriptions.push(newsDistrictSubscription);
            }
            if (form.controls.hasNoDistrictControl.value) {
                this.userProfileModel.newsSubscription = NewsSubscriptionTypes.ALL;
            } else {
                this.userProfileModel.newsSubscription = NewsSubscriptionTypes.Districts;
            }
        } else {
            this.userProfileModel.newsSubscription = NewsSubscriptionTypes.None;
        }

        this.userProfileModel.newsDistrictSubscriptions = newsDistrictSubscriptions;
    }

    private closeDialogBtnClicked(closeFn: DialogCloseCallback): void {
        closeFn();
    }
}