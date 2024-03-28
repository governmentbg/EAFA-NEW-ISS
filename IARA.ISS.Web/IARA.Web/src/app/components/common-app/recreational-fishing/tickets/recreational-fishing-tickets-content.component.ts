import { CdkStep, StepperSelectionEvent } from '@angular/cdk/stepper';
import { DatePipe } from '@angular/common';
import { AfterViewInit, Component, Input, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { AbstractControl, FormArray, FormControl, FormGroup, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { MatStepper } from '@angular/material/stepper';
import { Router } from '@angular/router';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { PaymentTypesEnum } from '@app/enums/payment-types.enum';
import { TicketPeriodEnum } from '@app/enums/ticket-period.enum';
import { TicketTypeEnum } from '@app/enums/ticket-type.enum';
import { IRecreationalFishingService } from '@app/interfaces/common-app/recreational-fishing.interface';
import { EgnLncDTO } from '@app/models/generated/dtos/EgnLncDTO';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { PaymentDataDTO } from '@app/models/generated/dtos/PaymentDataDTO';
import { RecreationalFishingAddTicketsResultDTO } from '@app/models/generated/dtos/RecreationalFishingAddTicketsResultDTO';
import { RecreationalFishingTicketDTO } from '@app/models/generated/dtos/RecreationalFishingTicketDTO';
import { RecreationalFishingTicketPriceDTO } from '@app/models/generated/dtos/RecreationalFishingTicketPriceDTO';
import { RecreationalFishingTicketsDTO } from '@app/models/generated/dtos/RecreationalFishingTicketsDTO';
import { RecreationalFishingTicketValidationDTO } from '@app/models/generated/dtos/RecreationalFishingTicketValidationDTO';
import { RecreationalFishingTicketValidationResultDTO } from '@app/models/generated/dtos/RecreationalFishingTicketValidationResultDTO';
import { RecreationalFishingTicketViewDTO } from '@app/models/generated/dtos/RecreationalFishingTicketViewDTO';
import { RecreationalFishingUserTicketDataDTO } from '@app/models/generated/dtos/RecreationalFishingUserTicketDataDTO';
import { SystemPropertiesDTO } from '@app/models/generated/dtos/SystemPropertiesDTO';
import { CommonNomenclatures } from '@app/services/common-app/common-nomenclatures.service';
import { SystemPropertiesService } from '@app/services/common-app/system-properties.service';
import { RecreationalFishingPublicService } from '@app/services/public-app/recreational-fishing-public.service';
import { GetControlErrorLabelTextCallback } from '@app/shared/components/input-controls/base-tl-control';
import { TLError } from '@app/shared/components/input-controls/models/tl-error.model';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { TLValidators } from '@app/shared/utils/tl-validators';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { forkJoin, Subscription } from 'rxjs';


@Component({
    selector: 'recreational-fishing-tickets-content',
    templateUrl: './recreational-fishing-tickets-content.component.html',
    styleUrls: ['./recreational-fishing-tickets-content.component.scss']
})
export class RecreationalFishingTicketsContentComponent implements OnInit, AfterViewInit, OnDestroy {
    @Input()
    public isPublicApp!: boolean;

    @Input()
    public isPersonal: boolean = false;

    @Input()
    public isAssociation: boolean = false;

    @Input()
    public service!: IRecreationalFishingService;

    public currentDate: Date = new Date();

    public ticketTypeGroup!: FormGroup;
    public under14CountControl!: FormControl;
    public ticketPeriodGroup!: FormGroup;
    public ticketNumsArray!: FormArray;
    public ticketsGroup!: FormGroup;
    public paymentDataControl!: FormControl;

    public ticketTypes!: NomenclatureDTO<number>[];
    public visibleTicketPeriods!: NomenclatureDTO<number>[];
    public territoryUnits!: NomenclatureDTO<number>[];

    public showValidityStep: boolean = false;
    public showPaymentStep: boolean = false;
    public showDeclarationFileValidation: boolean = false;
    public ticketNumsApproved: boolean = false;

    public ticketTypeComment: string | null = null;
    public validityPeriodComment: string | null = null;
    public ticketNumsComment: string | null = null;
    public ticketNumsLabels: string[] = [];
    public ticketPeriodsOverlapComment: string | null = null;

    public maxUnder14Tickets!: number;
    public blockAssociationsAddTicketsDate: Date | undefined;

    public ticketsTypes: NomenclatureDTO<number>[] = [];
    public ticketsPeriods: NomenclatureDTO<number>[] = [];
    public childTicketsTypes: NomenclatureDTO<number>[] = [];
    public childTicketsPeriods: NomenclatureDTO<number>[] = [];

    public tickets: RecreationalFishingTicketDTO[] = [];
    public childTickets: RecreationalFishingTicketDTO[] = [];

    public totalPrice: number = 0;
    public ticketsSaved: boolean = false;
    public ticketsPayed: boolean = false;

    public deliveryTerritoryUnitControl: FormControl | undefined;

    public paidTicketApplicationId: number | undefined;
    public paidTicketPaymentRequestNum: string | undefined;

    public adultTicketType: TicketTypeEnum = TicketTypeEnum.STANDARD;

    public getControlErrorLabelTextForTicketNumberMethod: GetControlErrorLabelTextCallback = this.getControlErrorLabelTextForTicketNumber.bind(this);

    public get ticketsArray(): FormArray {
        return this.ticketsGroup.get('ticketsArray') as FormArray;
    }

    public get childTicketsArray(): FormArray {
        return this.ticketsGroup.get('childTicketsArray') as FormArray;
    }

    public get valid(): boolean {
        let result: boolean = (this.ticketTypeGroup.valid || this.ticketTypeGroup.disabled) && (this.ticketsGroup.valid || this.ticketsGroup.disabled);

        if (this.showValidityStep) {
            result &&= (this.ticketPeriodGroup.valid || this.ticketPeriodGroup.disabled);
        }

        if (this.isPublicApp && !this.isAssociation && this.deliveryTerritoryUnitControl) {
            result &&= (this.deliveryTerritoryUnitControl.valid || this.deliveryTerritoryUnitControl.disabled);
        }

        return result;
    }

    @ViewChild('stepper')
    private stepper!: MatStepper;

    @ViewChild('viewAndConfirmStep')
    private viewAndConfirmStep!: CdkStep;

    @ViewChild('paymentStep')
    private paymentStep: CdkStep | undefined;

    private ticketPeriods!: NomenclatureDTO<number>[];
    private ticketPrices: [number, number, number][] = []; // array of tuples [periodId, typeId, price]

    private shouldUpdatePersonalData: boolean = true;
    private ticketNumbersAvailability: Map<string, boolean> = new Map<string, boolean>();

    private cannotPurchase: boolean = false;
    private cannotPurchaseUnder14: boolean = false;

    private translate: FuseTranslationLoaderService;
    private router: Router;
    private systemPropertiesService: SystemPropertiesService;
    private systemProperties!: SystemPropertiesDTO;
    private nomenclatures: CommonNomenclatures;
    private datePipe: DatePipe;

    private dataSubscriptions: Subscription[] = [];

    private readonly nonPickablePeriods: TicketPeriodEnum[] = [TicketPeriodEnum.DISABILITY, TicketPeriodEnum.UNTIL14, TicketPeriodEnum.NOPERIOD];

    public constructor(
        translate: FuseTranslationLoaderService,
        systemPropertiesService: SystemPropertiesService,
        nomenclatures: CommonNomenclatures,
        datePipe: DatePipe,
        router: Router
    ) {
        this.buildForm();

        this.translate = translate;
        this.router = router;
        this.systemPropertiesService = systemPropertiesService;
        this.nomenclatures = nomenclatures;
        this.datePipe = datePipe;
    }

    public async ngOnInit(): Promise<void> {
        this.systemProperties = await this.systemPropertiesService.properties.toPromise();
        this.maxUnder14Tickets = this.systemProperties.maxNumberOfUnder14Tickets!;

        if (this.isPublicApp && this.isAssociation) {
            this.blockAssociationsAddTicketsDate = this.systemProperties.blockAssociationsAddTickets;
        }

        if (this.isPublicApp && !this.isAssociation) {
            this.deliveryTerritoryUnitControl = new FormControl(undefined, Validators.required);
        }

        forkJoin([
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.TicketPeriods, this.service.getTicketPeriods.bind(this.service), false),
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.TicketTypes, this.service.getTicketTypes.bind(this.service), false),
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.TerritoryUnits, this.nomenclatures.getTerritoryUnits.bind(this.nomenclatures), false)
        ]).subscribe({
            next: ([periods, types, tus]: [NomenclatureDTO<number>[], NomenclatureDTO<number>[], NomenclatureDTO<number>[]]) => {
                this.ticketTypes = types.filter(x => x.isActive);
                this.ticketPeriods = periods.filter(x => x.isActive);
                this.territoryUnits = tus;
                this.visibleTicketPeriods = periods.filter(x => !this.nonPickablePeriods.includes(TicketPeriodEnum[x.code! as keyof typeof TicketPeriodEnum]));

                this.buildTicketTypeControls();

                this.service.getTicketPrices().subscribe({
                    next: (prices: RecreationalFishingTicketPriceDTO[]) => {
                        for (const price of prices) {
                            const periodId: number | undefined = this.ticketPeriods.find(x => TicketPeriodEnum[x.code as keyof typeof TicketPeriodEnum] === price.ticketPeriod)?.value;
                            const typeId: number | undefined = this.ticketTypes.find(x => TicketTypeEnum[x.code as keyof typeof TicketTypeEnum] === price.ticketType)?.value;

                            if (periodId !== undefined && typeId !== undefined && price.price !== undefined) {
                                this.ticketPrices.push([periodId, typeId, price.price]);
                            }
                        }
                    }
                });
            }
        }).add(() => {
            if (this.isPublicApp && !this.isAssociation) {
                (this.service as RecreationalFishingPublicService).currentUserTickets.subscribe({
                    next: (tickets: RecreationalFishingTicketViewDTO[]) => {
                        const under14TypeId: number = this.ticketTypes.find(x => x.code === TicketTypeEnum[TicketTypeEnum.UNDER14])!.value!;

                        let under14Tickets: number = 0;
                        for (const ticket of tickets) {
                            if (ticket.typeId === under14TypeId) {
                                ++under14Tickets;
                            }
                        }

                        this.maxUnder14Tickets = this.systemProperties.maxNumberOfUnder14Tickets! - under14Tickets;

                        if (this.maxUnder14Tickets < 0) {
                            this.maxUnder14Tickets = 0;
                        }

                        if (this.maxUnder14Tickets === 0) {
                            const control: AbstractControl | null = this.ticketTypeGroup.get(TicketTypeEnum[TicketTypeEnum.UNDER14]);
                            control?.disable();
                        }
                        else if (this.maxUnder14Tickets > 0) {
                            const control: AbstractControl | null = this.ticketTypeGroup.get(TicketTypeEnum[TicketTypeEnum.UNDER14]);
                            control?.enable();
                        }
                    }
                });
            }
        });
    }

    public ngAfterViewInit(): void {
        this.registerValueChangesSubscribers();
    }

    public ngOnDestroy(): void {
        this.unsubscribeOnDataChange();
    }

    public checkTicketNumbers(): void {
        const check: boolean = (this.ticketNumsArray.value as string[]).some((num: string) => {
            return !this.ticketNumbersAvailability.has(num);
        });

        if (check) {
            this.service.checkTicketNumbersAvailability(this.ticketNumsArray.value).subscribe({
                next: (result: boolean[]) => {
                    this.ticketNumsApproved = true;

                    for (let i = 0; i < result.length; ++i) {
                        this.ticketNumbersAvailability.set(this.ticketNumsArray.value[i], result[i]);
                    }

                    this.updateTicketNumsArrayAndMoveStep();
                }
            });
        }
        else {
            this.ticketNumsApproved = true;
            this.updateTicketNumsArrayAndMoveStep();
        }
    }

    public save(print: boolean): void {
        if (this.ticketsSaved && this.showPaymentStep) {
            setTimeout(() => {
                this.stepper.next();
            });
        }

        if (this.isAssociation || !this.isPublicApp) {
            this.showDeclarationFileValidation = true;
        }

        if (this.checkIfValid()) {
            if (this.isPublicApp && !this.isAssociation) {
                this.service.addTickets(this.buildTicketModels()).subscribe({
                    next: (result: RecreationalFishingAddTicketsResultDTO) => {
                        this.setAddTicketsResult(result);

                        const service = this.service as RecreationalFishingPublicService;

                        if (this.shouldUpdatePersonalData === true && this.tickets.length === 1) {
                            const ticket: RecreationalFishingTicketDTO = this.tickets[0];

                            service.updateUserDataFromTicket(new RecreationalFishingUserTicketDataDTO({
                                person: ticket.person,
                                addressRegistrations: ticket.personAddressRegistrations,
                                photo: ticket.personPhoto
                            })).subscribe();
                        }

                        service.getUserRequestedOrActiveTickets().subscribe();
                    }
                }).add(this.ticketsSavedHandler.bind(this, print));
            }
            else {
                this.ticketsSavedHandler(print);
            }
        }
    }

    public pay(): Subscription | undefined {
        if (!this.isPublicApp || this.isAssociation) {
            this.paymentDataControl.markAllAsTouched();

            if (this.paymentDataControl.valid) {
                return this.service.addTickets(this.buildTicketModels()).subscribe({
                    next: (result: RecreationalFishingAddTicketsResultDTO) => {
                        this.setAddTicketsResult(result);

                        this.ticketsPayed = true;
                        this.paymentDataControl.disable();
                    }
                });
            }
        }
        else {
            this.ticketsPayed = true;
            this.navigateToMyTickets();
        }

        return undefined;
    }

    public payAndPrint(): void {
        const result: Subscription | undefined = this.pay();

        if (result !== undefined) {
            result.add(() => {
                this.downloadAllFishingTickets();
            });
        }
    }

    public reset(): void {
        this.stepper.reset();

        this.ticketTypeGroup.reset();
        this.ticketTypeGroup.enable();
        this.ticketTypeComment = null;

        this.under14CountControl.reset();
        this.under14CountControl.enable();

        this.ticketPeriodGroup.reset();
        this.ticketPeriodGroup.enable();
        this.validityPeriodComment = null;

        this.ticketNumsArray.clear();
        this.ticketNumsApproved = false;
        this.ticketNumsLabels = [];
        this.ticketNumsComment = null;

        this.showValidityStep = false;
        this.showPaymentStep = false;
        this.showDeclarationFileValidation = false;
        this.ticketPeriodsOverlapComment = null;

        this.paymentDataControl.reset();
        this.paymentDataControl.enable();

        this.totalPrice = 0;
        this.ticketsSaved = false;
        this.ticketsPayed = false;
        this.paidTicketApplicationId = undefined;
        this.paidTicketPaymentRequestNum = undefined;

        this.clearTickets();
        this.tickets = [];
        this.childTickets = [];

        this.registerValueChangesSubscribers();
    }

    public updatePersonalData(yes: boolean): void {
        this.shouldUpdatePersonalData = yes;
    }

    public onStepChange(step: StepperSelectionEvent): void {
        let firstIdx: number = this.showValidityStep === true ? 2 : 1;
        if (!this.isPublicApp || this.isAssociation) {
            ++firstIdx;
        }

        let shouldUpdateTicketPrices: boolean = false;
        if (this.ticketPrices === undefined || this.ticketPrices === null || this.ticketPrices.length === 0) {
            shouldUpdateTicketPrices = true;

            this.service.getTicketPrices().subscribe({
                next: (prices: RecreationalFishingTicketPriceDTO[]) => {
                    for (const price of prices) {
                        const periodId: number | undefined = this.ticketPeriods.find(x => TicketPeriodEnum[x.code as keyof typeof TicketPeriodEnum] === price.ticketPeriod)?.value;
                        const typeId: number | undefined = this.ticketTypes.find(x => TicketTypeEnum[x.code as keyof typeof TicketTypeEnum] === price.ticketType)?.value;

                        if (periodId !== undefined && typeId !== undefined && price.price !== undefined) {
                            this.ticketPrices.push([periodId, typeId, price.price]);
                        }
                    }
                }
            });
        }

        if (this.showPaymentStep === true && step.selectedStep === this.paymentStep) {
            const payment: PaymentDataDTO | undefined = this.paymentDataControl.value;
            if (payment) {
                payment.totalPaidPrice = this.totalPrice;

                this.paymentDataControl.setValue(payment);
            }
            else {
                this.paymentDataControl.setValue(new PaymentDataDTO({
                    paymentType: PaymentTypesEnum.CASH,
                    paymentDateTime: this.currentDate,
                    totalPaidPrice: this.totalPrice
                }));
            }
            return;
        }

        if (step.selectedIndex >= firstIdx + 1) {
            const idx: number = step.selectedIndex - firstIdx - 1;

            if (this.ticketsArray.length <= idx) {
                this.childTicketsArray.at(idx - this.ticketsArray.length).markAllAsTouched();
            }
            else {
                this.ticketsArray.at(idx).markAllAsTouched();
            }
        }

        if (step.selectedStep === this.viewAndConfirmStep) {
            if (!this.isPublicApp || this.isAssociation) {
                for (let i = 0; i < this.ticketNumsArray.length; ++i) {
                    const ticketNum: string = this.ticketNumsArray.controls[i].value;

                    if (i < this.tickets.length) {
                        this.tickets[i].ticketNum = ticketNum;

                        if(shouldUpdateTicketPrices) {
                            this.tickets[i].price = this.calculateTicketPrice(this.tickets[i]);
                            this.calculateTotalPrice();
                        }
                    }
                    else {
                        this.childTickets[i - this.tickets.length].ticketNum = ticketNum;
                    }
                }
            }

            this.checkPurchaseAbility();
        }
    }

    public onStepAnimationDone(): void {
        if (this.stepper.selected === this.viewAndConfirmStep) {
            if (this.ticketsGroup.invalid && !this.ticketsGroup.disabled) {
                this.stepper.previous();
            }
        }
    }

    public getControlErrorLabelTextForTicketNumber(controlName: string, error: unknown, errorCode: string): TLError | undefined {
        if (errorCode === 'alreadyInUse') {
            return new TLError({
                text: this.translate.getValue('recreational-fishing.ticket-number-already-in-use'),
                type: 'error'
            });
        }
        return undefined;
    }

    private buildForm(): void {
        this.ticketTypeGroup = new FormGroup({}, this.atLeastOneTicketTypeSelectedValidator());
        this.under14CountControl = new FormControl(1);

        this.ticketPeriodGroup = new FormGroup({
            ticketPeriodControl: new FormControl(null, Validators.required)
        });

        this.ticketNumsArray = new FormArray([], this.ticketNumsValidator());

        this.ticketsGroup = new FormGroup({
            ticketsArray: new FormArray([]),
            childTicketsArray: new FormArray([])
        });

        this.paymentDataControl = new FormControl();
    }

    private buildTicketTypeControls(): void {
        for (const type of this.ticketTypes) {
            this.ticketTypeGroup.addControl(`${type.code}`, new FormControl(false));
        }
    }

    private updateTicketNumsArrayAndMoveStep(): void {
        for (const control of this.ticketNumsArray.controls) {
            control.markAsTouched();
            control.updateValueAndValidity({ emitEvent: false });
        }

        this.ticketNumsArray.markAllAsTouched();
        this.ticketNumsArray.updateValueAndValidity({ emitEvent: false });

        if (this.ticketNumsArray.valid) {
            setTimeout(() => {
                this.stepper.next();
            });
        }
    }

    private atLeastOneTicketTypeSelectedValidator(): ValidatorFn {
        return (control: AbstractControl): ValidationErrors | null => {
            const group: FormGroup = control as FormGroup;

            const valid: boolean = Object.keys(group.controls).some((key: string) => {
                return group.controls[key].value === true;
            });

            if (!valid) {
                return { atLeastOne: true };
            }
            return null;
        };
    }

    private ticketNumberAlreadyInUse(): ValidatorFn {
        return (control: AbstractControl): ValidationErrors | null => {
            if (this.ticketNumsApproved === true) {
                const available: boolean | undefined = this.ticketNumbersAvailability.get(control.value);
                if (available !== undefined && available === false) {
                    return { alreadyInUse: true };
                }
            }

            return null;
        };
    }

    private ticketNumsValidator(): ValidatorFn {
        return (control: AbstractControl): ValidationErrors | null => {
            const ticketNumsArray: FormArray = control as FormArray;
            const ticketNums: string[] = ticketNumsArray?.value ?? [];

            if (ticketNums.length !== new Set(ticketNums).size) {
                this.ticketNumsArray.setErrors(null);
                for (const ticketNumControl of this.ticketNumsArray.controls) {
                    ticketNumControl.setErrors(null);
                }

                return { ticketNumDuplicates: true };
            }

            return null;
        };
    }

    private buildTicketsFormArray(values: { [key: string]: boolean }): void {
        for (const key in values) {
            const type: TicketTypeEnum = TicketTypeEnum[key as keyof typeof TicketTypeEnum];
            const typeId: number = this.ticketTypes.find(x => x.code === key)!.value!;

            // adding ticket control
            if (this.ticketTypeGroup.get(key)!.value === true) {
                if (type !== TicketTypeEnum.UNDER14 && type !== TicketTypeEnum.DISABILITY) {
                    this.showValidityStep = true;
                }

                if (type !== TicketTypeEnum.UNDER14) {
                    let needToAddTicket: boolean = true;

                    for (let i = 0; i < this.ticketsArray.length; ++i) {
                        const ticket: RecreationalFishingTicketDTO = this.ticketsArray.at(i).value;

                        if (ticket.typeId === typeId) {
                            needToAddTicket = false;
                            break;
                        }
                    }

                    if (needToAddTicket) {
                        const ticket = new RecreationalFishingTicketDTO({ typeId: typeId });

                        if (type === TicketTypeEnum.DISABILITY) {
                            ticket.periodId = this.ticketPeriods.find(x => x.code === TicketPeriodEnum[TicketPeriodEnum.DISABILITY])!.value;
                            ticket.price = this.calculateTicketPrice(ticket);
                        }

                        this.ticketsArray.push(new FormControl(ticket, Validators.required));
                    }
                }
                else {
                    this.addOrRemoveUnder14Tickets(this.under14CountControl.value);
                }
            }
            // removing ticket control
            else {
                if (type !== TicketTypeEnum.UNDER14) {
                    let index: number = -1;

                    for (let i = 0; i < this.ticketsArray.length; ++i) {
                        const ticket: RecreationalFishingTicketDTO = this.ticketsArray.at(i).value;

                        if (ticket.typeId === typeId) {
                            index = i;
                            break;
                        }
                    }

                    if (index !== -1) {
                        this.ticketsArray.removeAt(index);
                        this.ticketPeriodGroup.get('ticketPeriodControl')?.reset();
                    }
                }
                else {
                    this.childTicketsArray.clear();
                }
            }
        }
    }

    private refreshTicketsTypesAndPeriodsArray(): void {
        this.ticketsTypes = [];
        this.childTicketsTypes = [];
        this.ticketsPeriods = [];

        for (const ticket of this.tickets) {
            if (ticket.typeId !== undefined) {
                this.ticketsTypes.push(this.getTypeNomenclatureById(ticket.typeId));
            }
            if (ticket.periodId !== undefined) {
                this.ticketsPeriods.push(this.getPeriodNomenclatureById(ticket.periodId));
            }
        }

        for (const ticket of this.childTickets) {
            if (ticket.typeId !== undefined) {
                this.childTicketsTypes.push(this.getTypeNomenclatureById(ticket.typeId));
            }
            if (ticket.periodId !== undefined) {
                this.childTicketsPeriods.push(this.getPeriodNomenclatureById(ticket.periodId));
            }
        }
    }

    private getTypeNomenclatureById(id: number): NomenclatureDTO<number> {
        return this.ticketTypes.find(x => x.value === id)!;
    }

    private getPeriodNomenclatureById(id: number): NomenclatureDTO<number> {
        return this.ticketPeriods.find(x => x.value === id)!;
    }

    private clearTickets(): void {
        this.ticketsArray.clear();
        this.childTicketsArray.clear();
    }

    private overwriteTicketsArray(src: RecreationalFishingTicketDTO[], dest: RecreationalFishingTicketDTO[]): void {
        while (src.length > dest.length) {
            dest.push(new RecreationalFishingTicketDTO());
        }

        while (src.length < dest.length) {
            dest.pop();
        }

        for (let i = 0; i < src.length; ++i) {
            Object.assign(dest[i], src[i]);
        }
    }

    private calculateTicketPrice(ticket: RecreationalFishingTicketDTO): number {
        const ticketPrice = this.ticketPrices.find(x => x[0] === ticket.periodId && x[1] === ticket.typeId);
        return ticketPrice !== undefined ? ticketPrice[2] : 0;
    }

    private calculateTotalPrice(): void {
        this.totalPrice = 0;

        for (const ticket of this.tickets) {
            if (ticket.price !== undefined && ticket.price !== null) {
                this.totalPrice += ticket.price;
            }
        }
        for (const ticket of this.childTickets) {
            if (ticket.price !== undefined && ticket.price !== null) {
                this.totalPrice += ticket.price;
            }
        }
    }

    private buildTicketModels(): RecreationalFishingTicketsDTO {
        let associationId: number | undefined;
        if (this.isAssociation) {
            associationId = (this.service as RecreationalFishingPublicService).currentUserChosenAssociation?.value;
        }

        let hasPaymentData: boolean = false;
        let payment: PaymentDataDTO | undefined;

        if ((!this.isPublicApp || this.isAssociation) && this.showPaymentStep) {
            payment = this.paymentDataControl.value;
            hasPaymentData = true;
        }

        const tickets: RecreationalFishingTicketDTO[] = this.tickets.concat(this.childTickets);

        if (this.isPublicApp && !this.isAssociation && this.deliveryTerritoryUnitControl) {
            for (const ticket of tickets) {
                ticket.deliveryTerritoryUnitId = this.deliveryTerritoryUnitControl.value.value;
            }
        }

        return new RecreationalFishingTicketsDTO({
            tickets: tickets,
            associationId: associationId,
            hasPaymentData: hasPaymentData,
            paymentData: payment
        });
    }

    private disableAllTypesExcept(...types: TicketTypeEnum[]): void {
        for (const key of Object.keys(this.ticketTypeGroup.controls)) {
            if (!types.some(type => TicketTypeEnum[type] === key)) {
                this.ticketTypeGroup.get(key)!.disable({ onlySelf: true, emitEvent: false });
            }
        }
    }

    private enableAllTypes(): void {
        for (const key of Object.keys(this.ticketTypeGroup.controls)) {
            this.ticketTypeGroup.get(key)!.enable({ onlySelf: true, emitEvent: false });
        }
        if (this.isPublicApp && !this.isAssociation && this.maxUnder14Tickets === 0) {
            this.ticketTypeGroup.get(TicketTypeEnum[TicketTypeEnum.UNDER14])!.disable({ emitEvent: false });
        }
    }

    private buildTicketTypeComment(): void {
        this.ticketTypeComment = null;

        for (const ticket of this.tickets) {
            const type: string = this.ticketTypes.find(x => x.value === ticket.typeId)!.displayName!;
            this.adultTicketType = TicketTypeEnum[this.ticketTypes.find(x => x.value === ticket.typeId)!.code! as keyof typeof TicketTypeEnum];

            if (this.ticketTypeComment === null) {
                this.ticketTypeComment = `${type}`;
            }
            else {
                this.ticketTypeComment = `${this.ticketTypeComment}; ${type}`;
            }
        }

        if (this.childTickets.length !== 0) {
            const type: string = this.ticketTypes.find(x => x.code === TicketTypeEnum[TicketTypeEnum.UNDER14])!.displayName!;

            if (this.ticketTypeComment === null) {
                this.ticketTypeComment = `${type}`;
            }
            else {
                this.ticketTypeComment = `${this.ticketTypeComment}; ${type}`;
            }

            if (this.childTickets.length > 1) {
                this.ticketTypeComment = `${this.ticketTypeComment} (x${this.childTickets.length})`;
            }
        }
    }

    private ticketsSavedHandler(print: boolean): void {
        this.showPaymentStep = this.totalPrice > 0;

        this.ticketsSaved = true;
        this.unsubscribeOnDataChange();
        this.disableEditing();

        if (this.showPaymentStep) {
            setTimeout(() => {
                this.stepper.next();
            });
        }
        else {
            if (this.isPublicApp && !this.isAssociation) {
                this.navigateToMyTickets();
            }
            else {
                this.service.addTickets(this.buildTicketModels()).subscribe({
                    next: (result: RecreationalFishingAddTicketsResultDTO) => {
                        this.setAddTicketsResult(result);

                        if (print) {
                            this.downloadAllFishingTickets();
                        }
                    }
                });
            }
        }
    }

    private setAddTicketsResult(result: RecreationalFishingAddTicketsResultDTO): void {
        this.paidTicketApplicationId = result.paidTicketApplicationId;
        this.paidTicketPaymentRequestNum = result.paidTicketPaymentRequestNum;

        for (let i = 0; i < this.tickets.length; ++i) {
            this.tickets[i].id = result.ticketIds![i];
        }
        for (let i = 0; i < this.childTickets.length; ++i) {
            this.childTickets[i].id = result.childTicketIds![i];
        }
    }

    private downloadAllFishingTickets(): void {
        for (const ticket of this.tickets) {
            this.service.downloadFishingTicket(ticket.id!).subscribe({
                next: () => {
                    // nothing to do
                }
            });
        }

        for (const ticket of this.childTickets) {
            this.service.downloadFishingTicket(ticket.id!).subscribe({
                next: () => {
                    // nothing to do
                }
            });
        }
    }

    private disableEditing(): void {
        this.ticketTypeGroup.disable();
        this.under14CountControl.disable();
        this.ticketPeriodGroup.disable();
        this.ticketNumsArray.disable();
        this.ticketsGroup.disable();
    }

    private unsubscribeOnDataChange(): void {
        for (const sub of this.dataSubscriptions) {
            sub.unsubscribe();
        }
    }

    private checkIfValid(): boolean {
        this.ticketTypeGroup.markAllAsTouched();
        this.under14CountControl.markAsTouched();
        this.ticketPeriodGroup.markAllAsTouched();
        this.ticketNumsArray.markAsTouched();
        this.ticketsGroup.markAllAsTouched();

        let declarationFile: boolean = true;
        if (this.isAssociation || !this.isPublicApp) {
            declarationFile &&= this.tickets.every(x => x.declarationFile !== undefined && x.declarationFile !== null);
        }

        return this.ticketTypeGroup.valid
            && this.under14CountControl.valid
            && (this.ticketPeriodGroup.valid || !this.showValidityStep)
            && (this.ticketNumsArray.valid || (this.isPublicApp && !this.isAssociation))
            && this.ticketsGroup.valid
            && declarationFile;
    }

    private checkPurchaseAbility(): void {
        this.ticketPeriodsOverlapComment = null;

        let data: RecreationalFishingTicketValidationDTO | undefined;

        if (this.tickets.length > 0) {
            const ticket: RecreationalFishingTicketDTO = this.tickets[0];

            if (ticket.person && ticket.person.egnLnc && ticket.person.egnLnc.egnLnc && ticket.person.egnLnc.identifierType !== undefined && ticket.person.egnLnc.identifierType !== null) {
                data = new RecreationalFishingTicketValidationDTO({
                    personEgnLnc: ticket.person!.egnLnc,
                    ticketType: TicketTypeEnum[this.ticketTypes.find(x => x.value === ticket.typeId)!.code as keyof typeof TicketTypeEnum],
                    ticketPeriod: TicketPeriodEnum[this.ticketPeriods.find(x => x.value === ticket.periodId)!.code as keyof typeof TicketPeriodEnum],
                    birthDate: ticket.person!.birthDate,
                    telkValidTo: ticket.telkData?.validTo,
                    validFrom: ticket.validFrom
                });
            }
        }
        else if (this.childTickets.length > 0) {
            const ticket: RecreationalFishingTicketDTO = this.childTickets[0];

            if (ticket.person && ticket.person.egnLnc && ticket.person.egnLnc.egnLnc && ticket.person.egnLnc.identifierType !== undefined && ticket.person.egnLnc.identifierType !== null) {
                data = new RecreationalFishingTicketValidationDTO({
                    personEgnLnc: ticket.person!.egnLnc,
                    representativePersonEgnLnc: ticket.representativePerson!.egnLnc,
                    validFrom: ticket.validFrom
                });
            }
        }

        if (data !== undefined) {
            this.service.checkEgnLncPurchaseAbility(data).subscribe({
                next: (result: RecreationalFishingTicketValidationResultDTO) => {
                    this.cannotPurchase = result.cannotPurchaseTicket ?? false;

                    if (this.cannotPurchase) {
                        const validFrom: string = this.datePipe.transform(result.ticketValidFrom!, 'dd.MM.yyyy')!;
                        const validTo: string = this.datePipe.transform(result.ticketValidTo!, 'dd.MM.yyyy')!;
                        const isIndefinite: boolean = result.telkisIndefinite ?? false;

                        this.ticketPeriodsOverlapComment = isIndefinite
                            ? `${this.translate.getValue('recreational-fishing.person-already-has-indefinite-ticket')} ${validFrom}`
                            : `${this.translate.getValue('recreational-fishing.person-already-has-ticket-from-date')} ${validFrom} ${this.translate.getValue('recreational-fishing.to-date')} ${validTo}`;
                    }
                    else {
                        if (result.representativeCount) {
                            const egnLnc: EgnLncDTO = result.representativeCount.egnLnc!;
                            const current: number = result.representativeCount.count!;

                            const attempted: number = this.childTickets
                                .filter(x => x.representativePerson?.egnLnc?.egnLnc === egnLnc.egnLnc
                                    && x.representativePerson?.egnLnc?.identifierType === egnLnc.identifierType)
                                .length;

                            this.cannotPurchaseUnder14 = current + attempted > this.maxUnder14Tickets;

                            if (this.cannotPurchaseUnder14) {
                                this.ticketPeriodsOverlapComment = `${this.translate.getValue('recreational-fishing.person-already-has-child-tickets')}: ${current}`;
                            }
                            else {
                                this.ticketPeriodsOverlapComment = null;
                            }
                        }
                        else {
                            this.cannotPurchaseUnder14 = false;
                            this.ticketPeriodsOverlapComment = null;
                        }
                    }
                }
            });
        }
    }

    private navigateToMyTickets(): void {
        this.router.navigateByUrl('/recreational-fishing', {
            state: {
                buyTicket: false
            }
        });
    }

    private registerValueChangesSubscribers(): void {
        this.dataSubscriptions = [];

        this.dataSubscriptions.push(
            this.ticketTypeGroup.valueChanges.subscribe({
                next: (values: { [key: string]: boolean }) => {
                    this.showValidityStep = false;
                    this.validityPeriodComment = null;

                    this.calculateTotalPrice();
                    this.buildTicketsFormArray(values);
                    this.refreshTicketsTypesAndPeriodsArray();
                    this.buildTicketNums();

                    this.enableOrDisableTypes(values);
                    this.buildTicketTypeComment();
                }
            })
        );

        this.dataSubscriptions.push(
            this.under14CountControl.valueChanges.subscribe({
                next: (value: number) => {
                    this.addOrRemoveUnder14Tickets(value);
                    this.calculateTotalPrice();
                    this.buildTicketNums();

                    this.buildTicketTypeComment();
                    this.refreshTicketsTypesAndPeriodsArray();
                }
            })
        );

        this.dataSubscriptions.push(
            this.ticketPeriodGroup.get('ticketPeriodControl')!.valueChanges.subscribe({
                next: (period: NomenclatureDTO<number>) => {
                    if (period !== null && period !== undefined) {
                        for (const ticket of this.tickets) {
                            ticket.periodId = period.value!;
                            ticket.price = this.calculateTicketPrice(ticket);

                            this.validityPeriodComment = period.displayName ?? null;

                            this.calculateTotalPrice();
                            this.refreshTicketsTypesAndPeriodsArray();
                        }
                    }
                }
            })
        );

        this.dataSubscriptions.push(
            this.ticketNumsArray.valueChanges.subscribe({
                next: (values: string[]) => {
                    if (values?.length !== 0) {
                        const vals: string[] = values.filter(x => x !== null && x !== undefined && x !== '');
                        if (vals.length !== 0) {
                            this.ticketNumsComment = vals.join(', ');
                        }
                        else {
                            this.ticketNumsComment = null;
                        }
                    }
                    else {
                        this.ticketNumsComment = null;
                    }
                }
            })
        );

        this.dataSubscriptions.push(
            this.ticketsGroup.get('ticketsArray')!.valueChanges.subscribe({
                next: (tickets: RecreationalFishingTicketDTO[]) => {
                    this.overwriteTicketsArray(tickets, this.tickets);
                    this.refreshTicketsTypesAndPeriodsArray();
                }
            })
        );

        this.dataSubscriptions.push(
            this.ticketsGroup.get('childTicketsArray')!.valueChanges.subscribe({
                next: (tickets: RecreationalFishingTicketDTO[]) => {
                    this.overwriteTicketsArray(tickets, this.childTickets);
                    this.refreshTicketsTypesAndPeriodsArray();
                }
            })
        );

        this.dataSubscriptions.push(
            this.ticketNumsArray.valueChanges.subscribe({
                next: (ticketNumbers: string[]) => {
                    this.ticketNumsApproved = false;
                }
            })
        );
    }

    private buildTicketNums(): void {
        if (this.isAssociation || !this.isPublicApp) {
            this.ticketNumsArray.clear();
            this.ticketNumsLabels = [];

            for (const ticket of this.tickets) {
                this.ticketNumsArray.push(new FormControl(null, [
                    Validators.required, Validators.maxLength(50), TLValidators.number(1, undefined, 0), this.ticketNumberAlreadyInUse()
                ]));

                const type: string = this.ticketTypes.find(x => x.value === ticket.typeId)!.displayName!;
                this.ticketNumsLabels.push(type);
            }

            for (let i = 0; i < this.childTickets.length; ++i) {
                this.ticketNumsArray.push(new FormControl(null, [
                    Validators.required, Validators.maxLength(50), TLValidators.number(1, undefined, 0), this.ticketNumberAlreadyInUse()
                ]));

                const type: string = this.ticketTypes.find(x => x.code === TicketTypeEnum[TicketTypeEnum.UNDER14])!.displayName!;

                if (this.childTickets.length > 1) {
                    this.ticketNumsLabels.push(`${type} ${this.translate.getValue('recreational-fishing.issue-ticket-num-no')}${i}`);
                }
                else {
                    this.ticketNumsLabels.push(type);
                }
            }
        }
    }

    private enableOrDisableTypes(values: { [key: string]: boolean }): void {
        let selected!: TicketTypeEnum;

        const disable: boolean = Object.keys(values).some((key: string) => {
            const type: TicketTypeEnum = TicketTypeEnum[key as keyof typeof TicketTypeEnum];

            if (values[key] === true) {
                if (TicketTypeEnum.UNDER14 !== type) {
                    selected = type;
                    return true;
                }
            }
            return false;
        });

        if (disable === true) {
            this.disableAllTypesExcept(TicketTypeEnum.UNDER14, selected);
        }
        else {
            this.enableAllTypes();
        }
    }

    private addOrRemoveUnder14Tickets(value: number): void {
        if (value > this.childTickets.length) {
            const ticket = new RecreationalFishingTicketDTO({
                typeId: this.ticketTypes.find(x => x.code === TicketTypeEnum[TicketTypeEnum.UNDER14])!.value!,
                periodId: this.ticketPeriods.find(x => x.code === TicketPeriodEnum[TicketPeriodEnum.UNTIL14])!.value!
            });

            ticket.price = this.calculateTicketPrice(ticket);

            while (value > this.childTickets.length) {
                this.childTicketsArray.push(new FormControl(ticket, Validators.required));
            }
        }

        while (value < this.childTickets.length) {
            this.childTicketsArray.removeAt(this.childTickets.length - 1);
        }
    }
}
