import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { IPenalDecreesService } from '@app/interfaces/administration-app/penal-decrees.interface';
import { PenalDecreeStatusTypesEnum } from '@app/enums/penal-decree-status-types.enum';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { TLDataTableComponent } from '@app/shared/components/data-table/tl-data-table.component';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { forkJoin } from 'rxjs';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { TLValidators } from '@app/shared/utils/tl-validators';
import { EditPenalDecreeStatusDialogParams } from '../models/edit-penal-decree-status-params.model';
import { PenalDecreePaymentScheduleDTO } from '@app/models/generated/dtos/PenalDecreePaymentScheduleDTO';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { PenalDecreeStatusEditDTO } from '@app/models/generated/dtos/PenalDecreeStatusEditDTO';
import { PenalDecreeTypeEnum } from '@app/enums/penal-decree-type.enum';
import { DateUtils } from '@app/shared/utils/date.utils';

@Component({
    selector: 'edit-penal-decree-status',
    templateUrl: './edit-penal-decree-status.component.html'
})
export class EditPenalDecreeStatusComponent implements OnInit, AfterViewInit, IDialogComponent {
    public form!: FormGroup;
    public paymentScheduleForm!: FormGroup;
    public viewMode: boolean = false;
    public atLeastOnePaymentScheduleError: boolean = false;

    public model!: PenalDecreeStatusEditDTO;

    public decreeType: PenalDecreeTypeEnum | undefined;
    public type: PenalDecreeStatusTypesEnum | undefined;
    public readonly types: typeof PenalDecreeStatusTypesEnum = PenalDecreeStatusTypesEnum;

    public statusTypes: NomenclatureDTO<number>[] = [];
    public authorityTypes: NomenclatureDTO<number>[] = [];
    public confiscationInstitutions: NomenclatureDTO<number>[] = [];
    public courts: NomenclatureDTO<number>[] = [];
    public payments: PenalDecreePaymentScheduleDTO[] = [];

    @ViewChild('paymentScheduleTable')
    private paymentScheduleTable!: TLDataTableComponent;

    private id: number | undefined;
    private penalDecreeId: number | undefined;
    private service!: IPenalDecreesService;
    private readonly translate: FuseTranslationLoaderService;
    private isAdding!: boolean;

    public constructor(
        translate: FuseTranslationLoaderService,
    ) {
        this.translate = translate;
        this.buildForm();
    }

    public async ngOnInit(): Promise<void> {
        const nomenclatures: (NomenclatureDTO<number>)[][] = await forkJoin(
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.PenalDecreeStatuses, this.service.getPenalDecreeStatusTypes.bind(this.service), false),
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.PenalDecreeAuthorityTypes, this.service.getPenalDecreeAuthorityTypes.bind(this.service), false),
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.Courts, this.service.getCourts.bind(this.service), false),
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.ConfiscatonInstitutions, this.service.getConfiscationInstitutions.bind(this.service), false)
        ).toPromise();

        this.statusTypes = nomenclatures[0];
        this.authorityTypes = nomenclatures[1];
        this.courts = nomenclatures[2];
        this.confiscationInstitutions = nomenclatures[3];

        if (!this.isAdding) {
            this.fillForm();
        }
    }

    public ngAfterViewInit(): void {
        this.form.get('statusTypeControl')!.valueChanges.subscribe({
            next: (type: NomenclatureDTO<number> | undefined) => {
                if (type !== undefined) {
                    this.type = PenalDecreeStatusTypesEnum[type.code as keyof typeof PenalDecreeStatusTypesEnum];
                }
                else {
                    this.type = undefined;
                }
            }
        });

        this.paymentScheduleForm.valueChanges.subscribe({
            next: () => {
                this.atLeastOnePaymentScheduleError = false;
            }
        });
    }

    public setData(data: EditPenalDecreeStatusDialogParams, wrapperData: DialogWrapperData): void {
        this.service = data.service as IPenalDecreesService;
        this.viewMode = data.viewMode;
        this.decreeType = data.decreeType;
        this.penalDecreeId = data.penalDecreeId;

        if (data.model === undefined) {
            this.model = new PenalDecreeStatusEditDTO({ isActive: true });
            this.isAdding = true;
        }
        else {
            if (this.viewMode) {
                this.form.disable();
            }
            if (data.model instanceof PenalDecreeStatusEditDTO) {
                this.model = data.model;
                this.isAdding = false;
                this.id = data.model.id!;
            }
        }
    }

    public saveBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        if (this.viewMode) {
            dialogClose(this.model);
        }
        else {
            this.form.markAllAsTouched();
            if (this.isFormValid()) {
                this.fillModel();
                CommonUtils.sanitizeModelStrings(this.model);

                if (this.id !== undefined && this.id !== null) {
                    this.service.editPenalDecreeStatus(this.model).subscribe({
                        next: () => {
                            dialogClose(this.model);
                        }
                    });
                }
                else {
                    this.service.addPenalDecreeStatus(this.model).subscribe({
                        next: (id: number) => {
                            this.model.id = id;
                            dialogClose(this.model);
                        }
                    });
                }
            }
        }
    }

    public dialogButtonClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public cancelBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }


    private buildForm(): void {
        this.form = new FormGroup({
            statusTypeControl: new FormControl(null, Validators.required),

            appealGroup: new FormGroup({
                courtControl: new FormControl(null, Validators.required),
                appealDateControl: new FormControl(null, Validators.required),
                caseNumControl: new FormControl(null, Validators.required)
            }),

            firstDecisionGroup: new FormGroup({
                courtControl: new FormControl(null, Validators.required),
                complaintDueDateControl: new FormControl(null, Validators.required),
                remunerationAmountControl: new FormControl(null, [Validators.required, TLValidators.number(0, undefined, 2)])
            }),

            secondDecisionGroup: new FormGroup({
                courtControl: new FormControl(null, Validators.required),
                remunerationAmountControl: new FormControl(null, [Validators.required, TLValidators.number(0, undefined, 2)])
            }),

            intoForceGroup: new FormGroup({
                enactmentDateControl: new FormControl(null, Validators.required)
            }),

            partiallyChangedGroup: new FormGroup({
                enactmentDateControl: new FormControl(null, Validators.required),
                courtControl: new FormControl(null, Validators.required),
                amendmentsControl: new FormControl(null, Validators.maxLength(4000))
            }),

            withdrawnGroup: new FormGroup({
                penalAuthorityTypeControl: new FormControl(null, Validators.required),
                enactmentDateControl: new FormControl(null, Validators.required)
            }),

            compulsoryGroup: new FormGroup({
                confiscationInstitutionControl: new FormControl(null, Validators.required),
                enactmentDateControl: new FormControl(null, Validators.required)
            }),

            partiallyPaidGroup: new FormGroup({
                paidAmountControl: new FormControl(null, [Validators.required, TLValidators.number(0, undefined, 2)])
            })
        });

        this.paymentScheduleForm = new FormGroup({
            dateControl: new FormControl(null, Validators.required),
            owedAmountControl: new FormControl(null, [Validators.required, TLValidators.number(0, undefined, 2)]),
            paidAmountControl: new FormControl(null, TLValidators.number(0, undefined, 2))
        });
    }

    private fillForm(): void {
        if (this.model?.statusType !== undefined && this.model.statusType !== null) {
            this.type = this.model.statusType;
            this.form.get('statusTypeControl')!.setValue(this.statusTypes.find(x => x.code === PenalDecreeStatusTypesEnum[this.model.statusType!]));
            this.penalDecreeId = this.model.penalDecreeId;

            switch (this.type) {
                case PenalDecreeStatusTypesEnum.FirstInstAppealed:
                case PenalDecreeStatusTypesEnum.SecondInstAppealed:
                    this.form.get('appealGroup')!.get('courtControl')!.setValue(this.courts.find(x => x.value === this.model.courtId));
                    this.form.get('appealGroup')!.get('appealDateControl')!.setValue(this.model.appealDate);
                    this.form.get('appealGroup')!.get('caseNumControl')!.setValue(this.model.caseNum);
                    break;
                case PenalDecreeStatusTypesEnum.FirstInstDecision:
                    this.form.get('firstDecisionGroup')!.get('courtControl')!.setValue(this.courts.find(x => x.value === this.model.courtId));
                    this.form.get('firstDecisionGroup')!.get('complaintDueDateControl')!.setValue(this.model.complaintDueDate);
                    this.form.get('firstDecisionGroup')!.get('remunerationAmountControl')!.setValue(this.model.remunerationAmount?.toFixed(2));
                    break;
                case PenalDecreeStatusTypesEnum.SecondInstDecision:
                    this.form.get('secondDecisionGroup')!.get('courtControl')!.setValue(this.courts.find(x => x.value === this.model.courtId));
                    this.form.get('secondDecisionGroup')!.get('remunerationAmountControl')!.setValue(this.model.remunerationAmount?.toFixed(2));
                    break;
                case PenalDecreeStatusTypesEnum.PartiallyChanged:
                    this.form.get('partiallyChangedGroup')!.get('enactmentDateControl')!.setValue(this.model.enactmentDate);
                    this.form.get('partiallyChangedGroup')!.get('courtControl')!.setValue(this.courts.find(x => x.value === this.model.courtId));
                    this.form.get('partiallyChangedGroup')!.get('amendmentsControl')!.setValue(this.model.amendments);
                    break;
                case PenalDecreeStatusTypesEnum.PartiallyPaid:
                    this.form.get('partiallyPaidGroup')!.get('paidAmountControl')!.setValue(this.model.paidAmount?.toFixed(2));
                    break;
                case PenalDecreeStatusTypesEnum.Valid:
                    this.form.get('intoForceGroup')!.get('enactmentDateControl')!.setValue(this.model.enactmentDate);
                    break;
                case PenalDecreeStatusTypesEnum.Withdrawn:
                    this.form.get('withdrawnGroup')!.get('penalAuthorityTypeControl')!.setValue(this.authorityTypes.find(x => x.value === this.model.penalAuthorityTypeId));
                    this.form.get('withdrawnGroup')!.get('enactmentDateControl')!.setValue(this.model.enactmentDate);
                    break;
                case PenalDecreeStatusTypesEnum.Compulsory:
                    this.form.get('compulsoryGroup')!.get('confiscationInstitutionControl')!.setValue(this.confiscationInstitutions.find(x => x.value === this.model.confiscationInstitutionId));
                    this.form.get('compulsoryGroup')!.get('enactmentDateControl')!.setValue(this.model.enactmentDate);
                    break;
                case PenalDecreeStatusTypesEnum.Rescheduled:
                    this.payments = this.model.paymentSchedule ?? [];
                    break;
                case PenalDecreeStatusTypesEnum.FullyPaid:
                    break;
                default:
                    throw new Error('Invalid penal decree status type');
            }
        }
    }

    private fillModel(): void {
        if (this.form.get('statusTypeControl')!.valid) {
            const statusType: NomenclatureDTO<PenalDecreeStatusTypesEnum> = this.form.get('statusTypeControl')!.value!;

            this.model.statusType = PenalDecreeStatusTypesEnum[statusType.code as keyof typeof PenalDecreeStatusTypesEnum];
            this.model.statusName = statusType.displayName;
            this.model.dateOfChange = new Date();
            this.model.penalDecreeId = this.penalDecreeId!;

            const from: string = this.translate.getValue('common.from');
            const instanceAppealDate: string = this.translate.getValue('penal-decrees.status-details-instance-appeal-date');
            const decisionDate: string = this.translate.getValue('penal-decrees.status-details-decision-date');
            const enactmentDate: string = this.translate.getValue('penal-decrees.status-details-enactment-date');
            const dateOfWithdraw: string = this.translate.getValue('penal-decrees.status-details-withdraw-date');
            const dateOfChange: string = this.translate.getValue('penal-decrees.status-details-change-date');
            const compulsory: string = this.translate.getValue('penal-decrees.status-details-compulsory');
            const partiallyPaid: string = this.translate.getValue('penal-decrees.status-details-partially-paid');

            switch (this.type) {
                case PenalDecreeStatusTypesEnum.FirstInstAppealed:
                case PenalDecreeStatusTypesEnum.SecondInstAppealed:
                    this.model.courtId = this.form.get('appealGroup')!.get('courtControl')!.value?.value;
                    this.model.appealDate = this.form.get('appealGroup')!.get('appealDateControl')!.value;
                    this.model.caseNum = this.form.get('appealGroup')!.get('caseNumControl')!.value;
                    this.model.details = `${instanceAppealDate}: ${DateUtils.ToDisplayDateString(this.model.appealDate!)} ${from} ${this.form.get('appealGroup')!.get('courtControl')!.value?.displayName}`;
                    break;
                case PenalDecreeStatusTypesEnum.FirstInstDecision:
                    this.model.courtId = this.form.get('firstDecisionGroup')!.get('courtControl')!.value?.value;
                    this.model.complaintDueDate = this.form.get('firstDecisionGroup')!.get('complaintDueDateControl')!.value;
                    this.model.remunerationAmount = this.form.get('firstDecisionGroup')!.get('remunerationAmountControl')!.value;
                    this.model.details = `${decisionDate}: ${DateUtils.ToDisplayDateString(this.model.complaintDueDate!)} ${from} ${this.form.get('firstDecisionGroup')!.get('courtControl')!.value?.displayName}`;
                    break;
                case PenalDecreeStatusTypesEnum.SecondInstDecision:
                    this.model.courtId = this.form.get('secondDecisionGroup')!.get('courtControl')!.value?.value;
                    this.model.remunerationAmount = this.form.get('secondDecisionGroup')!.get('remunerationAmountControl')!.value;
                    this.model.details = `${from} ${this.form.get('secondDecisionGroup')!.get('courtControl')!.value?.displayName}`;
                    break;
                case PenalDecreeStatusTypesEnum.PartiallyChanged:
                    this.model.enactmentDate = this.form.get('partiallyChangedGroup')!.get('enactmentDateControl')!.value;
                    this.model.courtId = this.form.get('partiallyChangedGroup')!.get('courtControl')!.value?.value;
                    this.model.amendments = this.form.get('partiallyChangedGroup')!.get('amendmentsControl')!.value;
                    this.model.details = `${dateOfChange}: ${DateUtils.ToDisplayDateString(this.model.enactmentDate!)} ${from} ${this.form.get('partiallyChangedGroup')!.get('courtControl')!.value?.displayName}`;
                    break;
                case PenalDecreeStatusTypesEnum.PartiallyPaid:
                    this.model.paidAmount = this.form.get('partiallyPaidGroup')!.get('paidAmountControl')!.value;
                    this.model.details = `${partiallyPaid}: ${this.model.paidAmount}`;
                    break;
                case PenalDecreeStatusTypesEnum.Valid:
                    this.model.enactmentDate = this.form.get('intoForceGroup')!.get('enactmentDateControl')!.value;
                    this.model.details = `${enactmentDate}: ${DateUtils.ToDisplayDateString(this.model.enactmentDate!)}`;
                    break;
                case PenalDecreeStatusTypesEnum.Withdrawn:
                    this.model.penalAuthorityTypeId = this.form.get('withdrawnGroup')!.get('penalAuthorityTypeControl')!.value?.value;
                    this.model.enactmentDate = this.form.get('withdrawnGroup')!.get('enactmentDateControl')!.value;
                    this.model.details = `${dateOfWithdraw}: ${DateUtils.ToDisplayDateString(this.model.enactmentDate!)} ${from} ${this.form.get('withdrawnGroup')!.get('penalAuthorityTypeControl')!.value?.displayName}`;
                    break;
                case PenalDecreeStatusTypesEnum.Compulsory:
                    this.model.confiscationInstitutionId = this.form.get('compulsoryGroup')!.get('confiscationInstitutionControl')!.value?.value;
                    this.model.enactmentDate = this.form.get('compulsoryGroup')!.get('enactmentDateControl')!.value;
                    this.model.details = `${compulsory} ${this.form.get('compulsoryGroup')!.get('confiscationInstitutionControl')!.value?.displayName}`;
                    break;
                case PenalDecreeStatusTypesEnum.Rescheduled:
                    this.model.paymentSchedule = this.getPaymentScheduleFromTable();
                    break;
                case PenalDecreeStatusTypesEnum.FullyPaid:
                    break;
                default:
                    throw new Error('Invalid penal decree status type');
            }
        }
    }

    private getPaymentScheduleFromTable(): PenalDecreePaymentScheduleDTO[] {
        const rows = this.paymentScheduleTable.rows as PenalDecreePaymentScheduleDTO[];

        return rows.map(x => new PenalDecreePaymentScheduleDTO({
            id: x.id,
            date: x.date,
            owedAmount: x.owedAmount,
            paidAmount: x.paidAmount,
            isActive: x.isActive ?? true
        }));
    }

    private isFormValid(): boolean {
        if (this.form.get('statusTypeControl')!.valid) {
            switch (this.type) {
                case PenalDecreeStatusTypesEnum.FirstInstAppealed:
                case PenalDecreeStatusTypesEnum.SecondInstAppealed:
                    return this.form.get('appealGroup')!.valid;
                case PenalDecreeStatusTypesEnum.FirstInstDecision:
                    return this.form.get('firstDecisionGroup')!.valid;
                case PenalDecreeStatusTypesEnum.SecondInstDecision:
                    return this.form.get('secondDecisionGroup')!.valid;
                case PenalDecreeStatusTypesEnum.PartiallyChanged:
                    return this.form.get('partiallyChangedGroup')!.valid;
                case PenalDecreeStatusTypesEnum.PartiallyPaid:
                    return this.form.get('partiallyPaidGroup')!.valid;
                case PenalDecreeStatusTypesEnum.Valid:
                    return this.form.get('intoForceGroup')!.valid;
                case PenalDecreeStatusTypesEnum.Withdrawn:
                    return this.form.get('withdrawnGroup')!.valid;
                case PenalDecreeStatusTypesEnum.Compulsory:
                    return this.form.get('compulsoryGroup')!.valid;
                case PenalDecreeStatusTypesEnum.Rescheduled:
                    this.atLeastOnePaymentScheduleError = !this.getPaymentScheduleFromTable().some(x => x.isActive !== false);
                    return !this.atLeastOnePaymentScheduleError;
                case PenalDecreeStatusTypesEnum.FullyPaid:
                    return true;
                default:
                    return false;
            }
        }
        return false;
    }
}