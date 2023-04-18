import { AfterViewInit, Component, EventEmitter, Input, OnChanges, OnInit, Optional, Output, Self, SimpleChanges } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, NgControl, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Observable, of, Subject } from 'rxjs';
import { map } from 'rxjs/operators';

import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { GenderEnum } from '@app/enums/gender.enum';
import { TicketTypeEnum } from '@app/enums/ticket-type.enum';
import { IRecreationalFishingService } from '@app/interfaces/common-app/recreational-fishing.interface';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { PermittedFileTypeDTO } from '@app/models/generated/dtos/PermittedFileTypeDTO';
import { RecreationalFishingMembershipCardDTO } from '@app/models/generated/dtos/RecreationalFishingMembershipCardDTO';
import { RecreationalFishingTelkDTO } from '@app/models/generated/dtos/RecreationalFishingTelkDTO';
import { RecreationalFishingTicketBaseRegixDataDTO } from '@app/models/generated/dtos/RecreationalFishingTicketBaseRegixDataDTO';
import { RecreationalFishingTicketDTO } from '@app/models/generated/dtos/RecreationalFishingTicketDTO';
import { RecreationalFishingTicketHolderDTO } from '@app/models/generated/dtos/RecreationalFishingTicketHolderDTO';
import { RegixChecksWrapperDTO } from '@app/models/generated/dtos/RegixChecksWrapperDTO';
import { SystemPropertiesDTO } from '@app/models/generated/dtos/SystemPropertiesDTO';
import { RecreationalFishingAdministrationService } from '@app/services/administration-app/recreational-fishing-administration.service';
import { SystemPropertiesService } from '@app/services/common-app/system-properties.service';
import { RecreationalFishingPublicService } from '@app/services/public-app/recreational-fishing-public.service';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { TLError } from '@app/shared/components/input-controls/models/tl-error.model';
import { RegixDateOfBirthProperties } from '@app/shared/components/regix-data/regix-data.component';
import { TLPictureRequestMethod } from '@app/shared/components/tl-picture-uploader/tl-picture-uploader.component';
import { ApplicationDialogData, ApplicationUtils } from '@app/shared/utils/application.utils';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { TLValidators } from '@app/shared/utils/tl-validators';
import { EditTicketDialogParams } from '@app/components/common-app/recreational-fishing/applications/models/edit-ticket-dialog-params.model';
import { PageCodeEnum } from '@app/enums/page-code.enum';
import { CommonNomenclatures } from '@app/services/common-app/common-nomenclatures.service';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { RecreationalFishingTicketsDTO } from '@app/models/generated/dtos/RecreationalFishingTicketsDTO';
import { FileTypeEnum } from '@app/enums/file-types.enum';
import { RequestProperties } from '@app/shared/services/request-properties';
import { RecreationalFishingAddTicketsResultDTO } from '@app/models/generated/dtos/RecreationalFishingAddTicketsResultDTO';
import { CustomFormControl } from '@app/shared/utils/custom-form-control';
import { RecreationalFishingTicketDuplicateTableDTO } from '@app/models/generated/dtos/RecreationalFishingTicketDuplicateTableDTO';
import { TLMatDialog } from '@app/shared/components/dialog-wrapper/tl-mat-dialog';
import { HeaderCloseFunction } from '@app/shared/components/dialog-wrapper/interfaces/header-cancel-button.interface';
import { Notifier } from '@app/shared/directives/notifier/notifier.class';
import { ApplicationRegiXCheckDTO } from '@app/models/generated/dtos/ApplicationRegiXCheckDTO';
import { TicketPeriodEnum } from '@app/enums/ticket-period.enum';
import { PersonFullDataDTO } from '@app/models/generated/dtos/PersonFullDataDTO';
import { IssueDuplicateTicketComponent } from '../../../applications/components/issue-duplicate-ticket/issue-duplicate-ticket.component';
import { IssueDuplicateTicketDialogParams } from '../../../applications/models/issue-duplicate-ticket-dialog-params.model';

@Component({
    selector: 'recreational-fishing-ticket',
    templateUrl: './recreational-fishing-ticket.component.html'
})
export class RecreationalFishingTicketComponent extends CustomFormControl<RecreationalFishingTicketDTO> implements OnInit, AfterViewInit, OnChanges, IDialogComponent {
    @Input() public service!: IRecreationalFishingService;

    @Input() public type!: NomenclatureDTO<number>;
    @Input() public period!: NomenclatureDTO<number>;
    @Input() public price!: number;

    @Input() public isEgnEditable: boolean = true;
    @Input() public isPersonal!: boolean;
    @Input() public isAssociation!: boolean;

    @Output() public updatePersonalData: EventEmitter<boolean> = new EventEmitter<boolean>();

    public notifier: Notifier = new Notifier();

    public isRenewal: boolean = false;
    public isDialog: boolean = false;
    public refreshFileTypes: Subject<void> = new Subject<void>();
    public isRegisterEntry: boolean = false;
    public isDuplicate: boolean = false;

    public currentDate: Date = new Date();
    public validFrom: Date | undefined;

    public fishingAssociations!: NomenclatureDTO<number>[];

    public showFilesPanel: boolean = false;
    public personPhotoMethod: TLPictureRequestMethod | undefined;
    public personPhotoRequired: boolean = true;

    public dateOfBirthProperties!: RegixDateOfBirthProperties;
    public dateOfBirthRequiredTicketTypes: string[] = [];

    public regixChecksData: RecreationalFishingTicketBaseRegixDataDTO | undefined;
    public regixChecks: ApplicationRegiXCheckDTO[] = [];
    public dialogData: EditTicketDialogParams | undefined;

    public ticketDuplicates: RecreationalFishingTicketDuplicateTableDTO[] = [];

    public pageCode!: PageCodeEnum;

    private buttons: DialogWrapperData | undefined;

    private periods: NomenclatureDTO<number>[] = [];

    private translate: FuseTranslationLoaderService;
    private nomenclatures: CommonNomenclatures;
    private snackbar: MatSnackBar;
    private systemPropertiesService: SystemPropertiesService;
    private issueDuplicateDialog: TLMatDialog<IssueDuplicateTicketComponent>

    private systemProperties!: SystemPropertiesDTO;

    private readonly notRequiredPhotoPeriods: string[] = [
        TicketPeriodEnum[TicketPeriodEnum.WEEKLY],
        TicketPeriodEnum[TicketPeriodEnum.MONTHLY]
    ];

    public constructor(
        @Optional() @Self() ngControl: NgControl,
        translate: FuseTranslationLoaderService,
        nomenclatures: CommonNomenclatures,
        snackbar: MatSnackBar,
        systemPropertiesService: SystemPropertiesService,
        issueDuplicateDialog: TLMatDialog<IssueDuplicateTicketComponent>
    ) {
        super(ngControl);
        this.translate = translate;
        this.nomenclatures = nomenclatures;
        this.snackbar = snackbar;
        this.systemPropertiesService = systemPropertiesService;
        this.issueDuplicateDialog = issueDuplicateDialog;

        this.dateOfBirthRequiredTicketTypes = [
            TicketTypeEnum[TicketTypeEnum.UNDER14],
            TicketTypeEnum[TicketTypeEnum.BETWEEN14AND18],
            TicketTypeEnum[TicketTypeEnum.ELDER],
            TicketTypeEnum[TicketTypeEnum.BETWEEN14AND18ASSOCIATION],
            TicketTypeEnum[TicketTypeEnum.ELDERASSOCIATION]
        ];

        this.form.get('telkIsIndefiniteControl')?.valueChanges.subscribe({
            next: (isIndefinite: boolean) => {
                if (this.type.code === TicketTypeEnum[TicketTypeEnum.DISABILITY]) {
                    this.getTicketPeriods().subscribe({
                        next: () => {
                            if (isIndefinite) {
                                this.period = this.periods.find(x => x.code === TicketPeriodEnum[TicketPeriodEnum.NOPERIOD])!;
                            }
                            else {
                                this.period = this.periods.find(x => x.code === TicketPeriodEnum[TicketPeriodEnum.DISABILITY])!;
                            }
                        }
                    });
                }
            }
        });
    }

    public async ngOnInit(): Promise<void> {
        this.initCustomFormControl();

        this.systemProperties = await this.systemPropertiesService.properties.toPromise();
        this.getFishingAssociations().subscribe();
        this.pageCode = this.getPageCodeFromTicketType();

        NomenclatureStore.instance.getNomenclature(
            NomenclatureTypes.PermittedFileTypes, this.nomenclatures.getPermittedFileTypes.bind(this.nomenclatures), false
        ).subscribe({
            next: (types: PermittedFileTypeDTO[]) => {
                this.showFilesPanel = !this.isPersonal || types.some(x => x.isRequired === true);
            }
        });

        if (this.isPersonal) {
            this.getPersonalDataFromProfile();
        }

        if (this.dialogData) {
            if (this.dialogData.showOnlyRegiXData) {
                (this.service as RecreationalFishingAdministrationService).getRegixData(this.dialogData.id).subscribe({
                    next: (result: RegixChecksWrapperDTO<RecreationalFishingTicketBaseRegixDataDTO>) => {
                        this.regixChecksData = result.regiXDataModel;
                        this.setupValidators();

                        this.writeValue(result.dialogDataModel!);

                        this.setRegiXData();

                        if (this.dialogData!.viewMode) {
                            this.setDisabledState(true);
                        }
                    }
                });
            }
            else {
                this.service.getTicket(this.dialogData.id, this.dialogData.showRegiXData).subscribe({
                    next: (result: RecreationalFishingTicketDTO) => {
                        if (this.isRenewal) {
                            result.validFrom = this.currentDate;
                        }
                        else {
                            this.isRegisterEntry = true;
                            this.currentDate = result.issuedOn!;
                            this.form.get('validFromControl')!.clearValidators();
                        }

                        if (this.dialogData!.showRegiXData) {
                            this.regixChecksData = new RecreationalFishingTicketBaseRegixDataDTO(result.regiXDataModel);
                            result.regiXDataModel = undefined;
                            this.setupValidators();
                        }

                        this.writeValue(result);

                        this.setRegiXData();

                        if (result.personPhoto !== null && result.personPhoto !== undefined) {
                            this.personPhotoMethod = this.service.getPhoto.bind(this.service, result.personPhoto.id!);
                        }

                        this.price = result.price!;
                        this.validFrom = result.validFrom!;

                        if (result.duplicateOfTicketNum && result.duplicateOfTicketNum.length > 0) {
                            this.isDuplicate = true;
                        }
                        else {
                            setTimeout(() => {
                                this.ticketDuplicates = result.ticketDuplicates ?? [];
                            });
                        }

                        if (this.dialogData!.viewMode) {
                            this.setDisabledState(true);
                        }
                    }
                });
            }
        }

        if (this.isDisabled) {
            this.form.disable();
        }
    }

    public ngAfterViewInit(): void {
        this.form.get('updatePersonalDataControl')?.valueChanges.subscribe({
            next: (checked: boolean) => {
                this.updatePersonalData.emit(checked);
            }
        });
    }

    public ngOnChanges(changes: SimpleChanges): void {
        this.dateOfBirthProperties = new RegixDateOfBirthProperties({
            getControlErrorLabelText: this.getControlErrorLabelText.bind(this)
        });

        const type: TicketTypeEnum = TicketTypeEnum[this.type.code as keyof typeof TicketTypeEnum];

        if (type === TicketTypeEnum.UNDER14) {
            const min: Date = new Date();
            min.setFullYear(min.getFullYear() - 14);
            min.setDate(min.getDate() + 1);

            this.dateOfBirthProperties.min = min;
            this.dateOfBirthProperties.max = this.currentDate;

            this.dateOfBirthProperties.validators = [Validators.required, this.isPersonUnder14Validator()];

            // always set to true because it doesn't exist in this case
            this.form.get('guaranteeTrueDataControl')?.setValue(true);
        }
        else if (type === TicketTypeEnum.BETWEEN14AND18 || type === TicketTypeEnum.BETWEEN14AND18ASSOCIATION) {
            const min: Date = new Date();
            min.setFullYear(min.getFullYear() - 18);
            min.setDate(min.getDate() + 1);

            const max: Date = new Date();
            max.setFullYear(max.getFullYear() - 14);
            max.setDate(max.getDate() + 1);

            this.dateOfBirthProperties.min = min;
            this.dateOfBirthProperties.max = max;

            this.dateOfBirthProperties.validators = [Validators.required, this.isPersonBetween14And18Validator()];
        }
        else if (type === TicketTypeEnum.ELDER || type === TicketTypeEnum.ELDERASSOCIATION) {
            const max: Date = new Date();
            max.setFullYear(max.getFullYear() - 60);
            max.setDate(max.getDate() + 1);

            this.dateOfBirthProperties.max = max;

            this.dateOfBirthProperties.validators = [Validators.required, this.isPersonElder()];
        }

        this.personPhotoRequired = !this.notRequiredPhotoPeriods.includes(this.period.code!);
        if (this.personPhotoRequired) {
            this.form.get('photoControl')?.setValidators(Validators.required);
            this.form.get('photoControl')?.updateValueAndValidity();
        }
        else {
            this.form.get('photoControl')?.clearValidators();
            this.form.get('photoControl')?.updateValueAndValidity();
        }

        this.setupValidators();
    }

    public writeValue(value: RecreationalFishingTicketDTO): void {
        if (value !== null && value !== undefined) {
            if (this.hasProperty(value, 'validFrom')) {
                this.form.get('validFromControl')?.setValue(value.validFrom);
            }
            if (this.hasProperty(value, 'validTo')) {
                this.form.get('validToControl')?.setValue(value.validTo);
            }
            if (this.hasProperty(value, 'ticketNum')) {
                this.form.get('ticketNumControl')?.setValue(value.ticketNum);
            }
            if (this.hasProperty(value, 'duplicateOfTicketNum')) {
                this.form.get('duplicateOfTicketNumControl')?.setValue(value.duplicateOfTicketNum);
            }
            if (this.hasProperty(value, 'person')) {
                this.form.get('regixDataControl')?.setValue(value.person);
            }
            if (this.hasProperty(value, 'personPhoto')) {
                this.form.get('photoControl')?.setValue(value.personPhoto);
            }
            if (this.hasProperty(value, 'personAddressRegistrations')) {
                this.form.get('addressControl')?.setValue(value.personAddressRegistrations);
            }
            if (this.hasProperty(value, 'representativePerson')) {
                this.form.get('representativeRegixDataControl')?.setValue(value.representativePerson);
            }
            if (this.hasProperty(value, 'representativePersonAddressRegistrations')) {
                this.form.get('representativeAddressControl')?.setValue(value.representativePersonAddressRegistrations);
            }

            if (this.hasProperty(value, 'membershipCard')) {
                if (this.hasProperty(value.membershipCard, 'cardNum')) {
                    this.form.get('membershipCardNumberControl')?.setValue(value.membershipCard?.cardNum);
                }
                if (this.hasProperty(value.membershipCard, 'issueDate')) {
                    this.form.get('membershipCardIssueDateControl')?.setValue(value.membershipCard?.issueDate);
                }
                if (this.hasProperty(value.membershipCard, 'associationId')) {
                    this.getFishingAssociations().subscribe({
                        next: () => {
                            this.form.get('membershipCardIssuedByControl')?.setValue(this.fishingAssociations.find(x => x.value === value.membershipCard?.associationId));
                        }
                    });
                }
            }

            if (this.hasProperty(value, 'telkData')) {
                if (this.hasProperty(value.telkData, 'num')) {
                    this.form.get('telkNumControl')?.setValue(value.telkData?.num);
                }
                if (this.hasProperty(value.telkData, 'isIndefinite')) {
                    this.form.get('telkIsIndefiniteControl')?.setValue(value.telkData?.isIndefinite);
                }
                if (this.hasProperty(value.telkData, 'validTo')) {
                    this.form.get('telkValidToControl')?.setValue(value.telkData?.validTo);
                }
            }

            if (this.hasProperty(value, 'comment')) {
                this.form.get('commentsControl')?.setValue(value.comment);
            }
            if (this.hasProperty(value, 'files')) {
                this.form.get('filesControl')?.setValue(value.files);
            }
        }
    }

    public setData(data: EditTicketDialogParams, buttons: DialogWrapperData): void {
        this.isDialog = true;
        this.dialogData = data;
        this.buttons = buttons;
        this.type = this.dialogData.type;
        this.period = this.dialogData.period;
        this.isPersonal = this.dialogData.isPersonal ?? false;
        this.isAssociation = this.dialogData.isAssociation ?? false;
        this.isRenewal = this.dialogData.isRenewal ?? false;
        this.service = this.dialogData.service as IRecreationalFishingService;

        if (this.type.code === TicketTypeEnum[TicketTypeEnum.UNDER14]) {
            // always set to true because it doesn't exist in this case
            this.form.get('guaranteeTrueDataControl')?.setValue(true);
        }

        this.personPhotoRequired = !this.notRequiredPhotoPeriods.includes(this.period.code!);
        if (this.personPhotoRequired) {
            this.form.get('photoControl')?.setValidators(Validators.required);
            this.form.get('photoControl')?.updateValueAndValidity();
        }
        else {
            this.form.get('photoControl')?.clearValidators();
            this.form.get('photoControl')?.updateValueAndValidity();
        }
    }

    public dialogButtonClicked(action: IActionInfo, dialogClose: DialogCloseCallback): void {
        if (action.id === 'print') {
            this.service.downloadFishingTicket(this.dialogData!.id).subscribe({
                next: () => {
                    // nothing to do
                }
            });
        }
        else if (action.id === 'issue-duplicate') {
            this.openIssueDuplicateDialog().subscribe({
                next: (success: boolean) => {
                    if (success === true) {
                        dialogClose(true);
                    }
                }
            });
        }
        else if (this.dialogData!.viewMode) {
            dialogClose(undefined);
        }
        else {
            const model: RecreationalFishingTicketBaseRegixDataDTO = this.getTicketBaseRegixData();
            CommonUtils.sanitizeModelStrings(model);

            ApplicationUtils.applicationDialogButtonClicked(new ApplicationDialogData({
                action: action,
                applicationId: this.dialogData!.applicationId,
                dialogClose: dialogClose,
                editForm: this.form,
                model: model
            }));
        }
    }

    public saveBtnClicked(action: IActionInfo, dialogClose: DialogCloseCallback): void {
        if (this.dialogData!.viewMode || this.dialogData!.isReadonly) {
            dialogClose(undefined);
        }
        else {
            this.form.markAllAsTouched();

            if (this.isFormValid()) {
                const model: RecreationalFishingTicketDTO = this.getValue();
                CommonUtils.sanitizeModelStrings(model);

                if (this.isRenewal) {
                    const tickets = new RecreationalFishingTicketsDTO({
                        associationId: model.membershipCard?.associationId,
                        tickets: [model],
                        paymentData: undefined
                    });
                    this.service.addTickets(tickets).subscribe({
                        next: (result: RecreationalFishingAddTicketsResultDTO) => {
                            model.id = result.ticketIds ? result.ticketIds[0] : result.childTicketIds![0];
                            model.applicationId = result.paidTicketApplicationId;
                            dialogClose(model);
                        }
                    });
                }
                else {
                    this.service.editTicket(model).subscribe({
                        next: () => {
                            dialogClose(model);
                        }
                    });
                }
            }
        }
    }

    public cancelBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose(undefined);
    }

    public getControlErrorLabelText(controlName: string, errorValue: unknown, errorCode: string): TLError | undefined {
        const result: TLError | undefined = CommonUtils.getControlErrorLabelTextForRegixExpectedValueValidator(controlName, errorValue, errorCode);
        if (result) {
            return result;
        }

        if (errorCode === 'personUnder14') {
            if (errorValue === false) {
                return new TLError({ text: this.translate.getValue('recreational-fishing.person-already-over-14'), type: 'error' });
            }
            else if (errorValue === true) {
                return new TLError({ text: this.translate.getValue('recreational-fishing.person-is-not-14-yet'), type: 'error' });
            }
        }
        else if (errorCode === 'personAbove18') {
            if (errorValue === true) {
                return new TLError({ text: this.translate.getValue('recreational-fishing.person-already-over-18'), type: 'error' });
            }
        }
        else if (errorCode === 'personAbove60') {
            if (errorValue === false) {
                return new TLError({ text: this.translate.getValue('recreational-fishing.person-is-not-60-yet'), type: 'error' });
            }
        }
        else if (errorCode === 'personAbove65') {
            if (errorValue === false) {
                return new TLError({ text: this.translate.getValue('recreational-fishing.person-is-not-65-yet'), type: 'error' });
            }
        }
        return undefined;
    }

    public downloadPersonalData(person: PersonFullDataDTO, representative: boolean): void {
        if (!this.isPersonal) {
            let associationId: number | undefined;
            if (this.isAssociation) {
                associationId = (this.service as RecreationalFishingPublicService).currentUserChosenAssociation!.value!;
            }

            this.setTicketHolderData(new RecreationalFishingTicketHolderDTO({
                person: person.person,
                addresses: person.addresses,
                photo: person.photo
            }), representative, associationId);
        }
    }

    public fileTypeFilterFn(options: PermittedFileTypeDTO[]): PermittedFileTypeDTO[] {
        const excludedFileTypes: FileTypeEnum[] = [FileTypeEnum.TICKET_DECLARATION];

        let result: PermittedFileTypeDTO[] = options;

        if (!this.isDialog) {
            result = result.filter(x => !excludedFileTypes.includes(FileTypeEnum[x.code as keyof typeof FileTypeEnum]));
        }

        return result;
    }

    protected buildForm(): AbstractControl {
        return new FormGroup({
            validFromControl: new FormControl(new Date(), Validators.required),
            validToControl: new FormControl(null),
            ticketNumControl: new FormControl(null),
            duplicateOfTicketNumControl: new FormControl(null),
            regixDataControl: new FormControl(null, Validators.required),
            photoControl: new FormControl(null),
            addressControl: new FormControl(null, Validators.required),
            representativeRegixDataControl: new FormControl(null),
            representativeAddressControl: new FormControl(null),
            membershipCardNumberControl: new FormControl(null),
            membershipCardIssueDateControl: new FormControl(null),
            membershipCardIssuedByControl: new FormControl(null),
            telkNumControl: new FormControl(null),
            telkIsIndefiniteControl: new FormControl(false),
            telkValidToControl: new FormControl(null),
            commentsControl: new FormControl(null, Validators.maxLength(4000)),
            filesControl: new FormControl(),
            guaranteeTrueDataControl: new FormControl(false, Validators.requiredTrue),
            updatePersonalDataControl: new FormControl(true)
        });
    }

    protected getValue(): RecreationalFishingTicketDTO {
        const result: RecreationalFishingTicketDTO = new RecreationalFishingTicketDTO({
            id: this.dialogData?.id,
            applicationId: this.dialogData?.applicationId,
            typeId: this.type?.value,
            periodId: this.period?.value,
            price: this.price,
            validFrom: this.form.get('validFromControl')?.value ?? undefined,
            person: this.form.get('regixDataControl')?.value,
            personPhoto: this.form.get('photoControl')?.value ?? undefined,
            personAddressRegistrations: this.form.get('addressControl')?.value,
            comment: this.form.get('commentsControl')?.value ?? undefined,
            hasUserConfirmed: this.form.get('guaranteeTrueDataControl')?.value ?? undefined,
            files: this.form.get('filesControl')?.value ?? undefined
        });

        if (result.validFrom !== undefined && result.validFrom !== null) {
            if (result.validFrom.getFullYear() === this.currentDate.getFullYear()
                && result.validFrom.getMonth() === this.currentDate.getMonth()
                && result.validFrom.getDate() === this.currentDate.getDate()) {
                result.validFrom = new Date();
            }
        }

        const type: TicketTypeEnum = TicketTypeEnum[this.type.code as keyof typeof TicketTypeEnum];
        if (type === TicketTypeEnum.UNDER14) {
            result.representativePerson = this.form.get('representativeRegixDataControl')?.value ?? undefined;
            result.representativePersonAddressRegistrations = this.form.get('representativeAddressControl')?.value ?? undefined;
        }
        else if (type === TicketTypeEnum.ASSOCIATION || type === TicketTypeEnum.BETWEEN14AND18ASSOCIATION || type === TicketTypeEnum.ELDERASSOCIATION) {
            result.membershipCard = new RecreationalFishingMembershipCardDTO({
                associationId: this.form.get('membershipCardIssuedByControl')?.value?.value ?? undefined,
                cardNum: this.form.get('membershipCardNumberControl')?.value ?? undefined,
                issueDate: this.form.get('membershipCardIssueDateControl')?.value ?? undefined
            });
        }
        else if (type === TicketTypeEnum.DISABILITY) {
            result.telkData = new RecreationalFishingTelkDTO({
                isIndefinite: this.form.get('telkIsIndefiniteControl')?.value ?? undefined,
                num: this.form.get('telkNumControl')?.value ?? undefined,
                validTo: this.form.get('telkValidToControl')?.value ?? undefined,
            });
        }

        return result;
    }

    private isFormValid(): boolean {
        if (this.form.valid) {
            return true;
        }
        else {
            const errors: ValidationErrors = {};

            for (const key of Object.keys(this.form.controls)) {
                if (this.form.controls[key].errors !== null && this.form.controls[key].errors !== undefined) {
                    for (const error in this.form.controls[key].errors) {
                        if (!['expectedValueNotMatching'].includes(error)) {
                            errors[error] = this.form.controls[key].errors![error];
                        }
                    }
                }
            }

            return Object.keys(errors).length === 0;
        }
    }

    private setupValidators(): void {
        const type: TicketTypeEnum = TicketTypeEnum[this.type.code as keyof typeof TicketTypeEnum];

        // Representative
        if (type === TicketTypeEnum.UNDER14) {
            this.form.get('representativeRegixDataControl')!.setValidators(Validators.required);
            this.form.get('representativeAddressControl')!.setValidators(Validators.required);
        }
        else {
            this.form.get('representativeRegixDataControl')!.clearValidators();
            this.form.get('representativeAddressControl')!.clearValidators();
        }

        // Membership card
        if (type === TicketTypeEnum.ASSOCIATION || type === TicketTypeEnum.BETWEEN14AND18ASSOCIATION || type === TicketTypeEnum.ELDERASSOCIATION) {
            this.form.get('membershipCardNumberControl')!.setValidators([Validators.maxLength(50), Validators.required]);
            this.form.get('membershipCardIssueDateControl')!.setValidators(Validators.required);
            this.form.get('membershipCardIssuedByControl')!.setValidators(Validators.required);

            this.form.get('membershipCardNumberControl')!.markAsPending({ emitEvent: false });
            this.form.get('membershipCardIssueDateControl')!.markAsPending({ emitEvent: false });
            this.form.get('membershipCardIssuedByControl')!.markAsPending({ emitEvent: false });
        }
        else {
            this.form.get('membershipCardNumberControl')!.clearValidators();
            this.form.get('membershipCardIssueDateControl')!.clearValidators();
            this.form.get('membershipCardIssuedByControl')!.clearValidators();
        }

        // TELK disability
        if (type === TicketTypeEnum.DISABILITY) {
            if (this.dialogData?.showOnlyRegiXData === true || this.dialogData?.showRegiXData) {
                this.form.get('telkNumControl')!.setValidators([
                    Validators.required,
                    Validators.maxLength(50),
                    TLValidators.expectedValueMatch(this.regixChecksData?.telkData?.num)
                ]);
                this.form.get('telkValidToControl')!.setValidators([
                    Validators.required,
                    TLValidators.expectedValueMatch(this.regixChecksData?.telkData?.validTo)
                ]);
                this.form.get('telkIsIndefiniteControl')!.setValidators(
                    TLValidators.expectedValueMatch(this.regixChecksData?.telkData?.isIndefinite)
                );
            }
            else {
                this.form.get('telkNumControl')!.setValidators([Validators.required, Validators.maxLength(50)]);
                this.form.get('telkValidToControl')!.setValidators(Validators.required);
            }

            this.form.get('telkNumControl')?.markAsPending({ emitEvent: false });

            this.form.get('telkIsIndefiniteControl')?.valueChanges.subscribe({
                next: (yes: boolean) => {
                    if (yes === true) {
                        this.form.get('telkValidToControl')?.reset();
                        this.form.get('telkValidToControl')?.disable();
                        this.form.get('telkValidToControl')?.clearValidators();
                    }
                    else {
                        this.form.get('telkValidToControl')?.enable();

                        if (this.dialogData?.showOnlyRegiXData === true || this.dialogData?.showRegiXData) {
                            this.form.get('telkValidToControl')?.setValidators([
                                Validators.required,
                                TLValidators.expectedValueMatch(this.regixChecksData?.telkData?.validTo)
                            ]);
                        }
                        else {
                            this.form.get('telkValidToControl')?.setValidators(Validators.required);
                        }
                        this.form.get('telkValidToControl')?.markAsPending({ emitEvent: false });
                    }
                }
            });
        }
        else {
            this.form.get('telkNumControl')!.clearValidators();
            this.form.get('telkValidToControl')!.clearValidators();
        }

        this.form.updateValueAndValidity({ emitEvent: false });
    }

    private setTicketHolderData(data: RecreationalFishingTicketHolderDTO | null, representative: boolean, associationId: number | undefined): void {
        if (data !== null && data !== undefined) {
            if (representative) {
                this.form.get('representativeRegixDataControl')!.setValue(data.person);
                this.form.get('representativeAddressControl')!.setValue(data.addresses);
            }
            else {
                this.form.get('regixDataControl')!.setValue(data.person);
                this.form.get('addressControl')!.setValue(data.addresses);

                if (this.personPhotoRequired) {
                    this.personPhotoMethod = this.service.getPersonPhoto.bind(this.service, data.person!.egnLnc!, associationId);
                    setTimeout(() => {
                        this.form.get('photoControl')!.setValue(data.photo);
                    });
                }
            }
        }
        else {
            if (representative) {
                this.form.get('representativeRegixDataControl')!.setValue(null);
                this.form.get('representativeAddressControl')!.setValue(null);
            }
            else {
                this.form.get('regixDataControl')!.setValue(null);
                this.form.get('addressControl')!.setValue(null);

                if (this.personPhotoRequired) {
                    this.personPhotoMethod = undefined;
                    setTimeout(() => {
                        this.form.get('photoControl')!.setValue(null);
                    });
                }

                this.snackbar.open(this.translate.getValue('recreational-fishing.no-person-found-snackbar-err'), undefined, {
                    duration: RequestProperties.DEFAULT.showExceptionDurationErr,
                    panelClass: RequestProperties.DEFAULT.showExceptionColorClassErr
                });
            }
        }
    }

    private getTicketBaseRegixData(): RecreationalFishingTicketBaseRegixDataDTO {
        const result: RecreationalFishingTicketBaseRegixDataDTO = new RecreationalFishingTicketBaseRegixDataDTO({
            id: this.dialogData!.id,
            applicationId: this.dialogData!.applicationId,
            person: this.form.get('regixDataControl')?.value,
            personAddressRegistrations: this.form.get('addressControl')?.value
        });

        const type: TicketTypeEnum = TicketTypeEnum[this.type.code as keyof typeof TicketTypeEnum];
        if (type === TicketTypeEnum.UNDER14) {
            result.representativePerson = this.form.get('representativeRegixDataControl')?.value ?? undefined;
            result.representativePersonAddressRegistrations = this.form.get('representativeAddressControl')?.value ?? undefined;
        }
        else if (type === TicketTypeEnum.DISABILITY) {
            result.telkData = new RecreationalFishingTelkDTO({
                isIndefinite: this.form.get('telkIsIndefiniteControl')?.value ?? undefined,
                num: this.form.get('telkNumControl')?.value ?? undefined,
                validTo: this.form.get('telkValidToControl')?.value ?? undefined,
            });
        }

        return result;
    }

    private openIssueDuplicateDialog(): Observable<boolean> {
        const data: IssueDuplicateTicketDialogParams = new IssueDuplicateTicketDialogParams({
            ticketId: this.dialogData!.id,
            service: this.service
        });

        if (this.isAssociation) {
            data.associationId = (this.service as RecreationalFishingPublicService).currentUserChosenAssociation!.value!;
        }

        return this.issueDuplicateDialog.openWithTwoButtons({
            title: this.translate.getValue('recreational-fishing.issue-duplicate-ticket-dialog-title'),
            TCtor: IssueDuplicateTicketComponent,
            headerCancelButton: {
                cancelBtnClicked: this.closeIssueDuplicateDialogBtnClicked.bind(this)
            },
            rightSideActionsCollection: [{
                id: 'save-and-print',
                color: 'accent',
                translateValue: this.translate.getValue('recreational-fishing.save-and-print-duplicate')
            }],
            translteService: this.translate,
            componentData: data
        }, '1000px');
    }

    private setRegiXData(): void {
        if (this.regixChecksData?.applicationRegiXChecks) {
            const applicationRegiXChecks: ApplicationRegiXCheckDTO[] = this.regixChecksData.applicationRegiXChecks;

            setTimeout(() => {
                this.regixChecks = applicationRegiXChecks;
            });
        }

        this.notifier.start();
        this.notifier.onNotify.subscribe({
            next: () => {
                this.form.markAllAsTouched();

                if (this.dialogData?.showOnlyRegiXData) {
                    ApplicationUtils.enableOrDisableRegixCheckButtons(this.form, this.buttons!.rightSideActions);
                }

                this.notifier.stop();
            }
        });
    }

    private isPersonUnder14Validator(): ValidatorFn {
        return (control: AbstractControl): ValidationErrors | null => {
            const value: Date = control.value;
            if (value !== undefined && value !== null && value.getTime() === value.getTime()) {
                if (this.getPersonAge(value) >= 14) {
                    return { personUnder14: false };
                }
            }
            return null;
        };
    }

    private isPersonBetween14And18Validator(): ValidatorFn {
        return (control: AbstractControl): ValidationErrors | null => {
            const value: Date = control.value;
            if (value !== undefined && value !== null && value.getTime() === value.getTime()) {
                const age: number = this.getPersonAge(value);
                if (age < 14) {
                    return { personUnder14: true };
                }
                if (age > 18) {
                    return { personAbove18: true };
                }
            }
            return null;
        };
    }

    private isPersonElder(): ValidatorFn {
        return (control: AbstractControl): ValidationErrors | null => {
            const value: Date = control.value;

            if (value !== undefined && value !== null) {
                const parent: FormGroup = control.parent as FormGroup;

                if (parent.get('genderControl')?.valid) {
                    const age: number = this.getPersonAge(value);
                    const genderCode: string = parent.get('genderControl')!.value!.code;
                    const gender: GenderEnum = GenderEnum[genderCode as keyof typeof GenderEnum];

                    if (gender === GenderEnum.M && age < this.systemProperties.elderTicketMaleAge!) {
                        return { personAbove65: false };
                    }
                    if (gender === GenderEnum.F && age < this.systemProperties.elderTicketFemaleAge!) {
                        return { personAbove60: false };
                    }
                }
            }
            return null;
        }
    }

    private getPersonAge(birthDate: Date): number {
        const difference: number = Date.now() - birthDate.getTime();
        const age: Date = new Date(difference);

        return Math.abs(age.getUTCFullYear() - 1970);
    }

    private getPersonalDataFromProfile(): void {
        if (this.isPersonal === true) {
            const service = this.service as RecreationalFishingPublicService;

            service.getUserPersonData().subscribe({
                next: (data: RecreationalFishingTicketHolderDTO) => {
                    if (this.type.code === TicketTypeEnum[TicketTypeEnum.UNDER14]) {
                        this.form.get('representativeRegixDataControl')!.setValue(data.person);
                        this.form.get('representativeAddressControl')!.setValue(data.addresses);
                    }
                    else {
                        this.form.get('regixDataControl')!.setValue(data.person);
                        this.form.get('addressControl')!.setValue(data.addresses);
                        this.form.get('photoControl')!.setValue(data.photo);
                    }
                }
            });

            this.personPhotoMethod = service.getUserPhoto.bind(service);
        }
    }

    private hasProperty<T>(value: T, property: string): boolean {
        if (value !== undefined && value !== null) {
            const keys: string[] = Object.keys(value).map(x => `${x[0].toLowerCase()}${x.slice(1)}`);
            return keys.includes(property);
        }
        return false;
    }

    private getFishingAssociations(): Observable<void> {
        const assocTypes: TicketTypeEnum[] = [TicketTypeEnum.ASSOCIATION, TicketTypeEnum.BETWEEN14AND18ASSOCIATION, TicketTypeEnum.ELDERASSOCIATION];

        if (assocTypes.includes(TicketTypeEnum[this.type.code as keyof typeof TicketTypeEnum])) {
            return NomenclatureStore.instance.getNomenclature(
                NomenclatureTypes.FishingAssociations, this.service.getAllFishingAssociations.bind(this.service), false
            ).pipe(map((associations: NomenclatureDTO<number>[]) => {
                this.fishingAssociations = associations;
            }));
        }
        return of();
    }

    private getTicketPeriods(): Observable<void> {
        return NomenclatureStore.instance.getNomenclature(
            NomenclatureTypes.TicketPeriods, this.service.getTicketPeriods.bind(this.service), false
        ).pipe(map((periods: NomenclatureDTO<number>[]) => {
            this.periods = periods;
        }));
    }

    private getPageCodeFromTicketType(): PageCodeEnum {
        if (this.isAssociation || !this.isPersonal) {
            switch (TicketTypeEnum[this.type.code as keyof typeof TicketTypeEnum]) {
                case TicketTypeEnum.STANDARD:
                    return PageCodeEnum.RecFishStd;
                case TicketTypeEnum.UNDER14:
                    return PageCodeEnum.RecFishUnd14;
                case TicketTypeEnum.BETWEEN14AND18:
                    return PageCodeEnum.RecFish14And18;
                case TicketTypeEnum.ELDER:
                    return PageCodeEnum.RecFishElder;
                case TicketTypeEnum.DISABILITY:
                    return PageCodeEnum.RecFishDisbl;
                case TicketTypeEnum.ASSOCIATION:
                    return PageCodeEnum.RecFishAssoc;
                case TicketTypeEnum.BETWEEN14AND18ASSOCIATION:
                    return PageCodeEnum.RecFish14And18Assoc;
                case TicketTypeEnum.ELDERASSOCIATION:
                    return PageCodeEnum.RecFishElderAssoc;
            }
        }
        else {
            switch (TicketTypeEnum[this.type.code as keyof typeof TicketTypeEnum]) {
                case TicketTypeEnum.STANDARD:
                    return PageCodeEnum.OnlineRecFishStd;
                case TicketTypeEnum.UNDER14:
                    return PageCodeEnum.OnlineRecFishUnd14;
                case TicketTypeEnum.BETWEEN14AND18:
                    return PageCodeEnum.OnlineRecFish14And18;
                case TicketTypeEnum.ELDER:
                    return PageCodeEnum.OnlineRecFishElder;
                case TicketTypeEnum.DISABILITY:
                    return PageCodeEnum.OnlineRecFishDisbl;
                case TicketTypeEnum.ASSOCIATION:
                    return PageCodeEnum.OnlineRecFishAssoc;
                case TicketTypeEnum.BETWEEN14AND18ASSOCIATION:
                    return PageCodeEnum.OnlineRecFish14And18Assoc;
                case TicketTypeEnum.ELDERASSOCIATION:
                    return PageCodeEnum.OnlineRecFishElderAssoc;
            }
        }
    }

    private closeIssueDuplicateDialogBtnClicked(closeFn: HeaderCloseFunction): void {
        closeFn();
    }
}