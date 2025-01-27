﻿import { AfterViewInit, Component, EventEmitter, Input, OnChanges, OnInit, Output, Self, SimpleChanges } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, NgControl, Validators } from '@angular/forms';

import { CustomFormControl } from '@app/shared/utils/custom-form-control';
import { ShipWithPersonnelModel } from '../../models/ship-with-personnel.model';
import { InspectedPersonTypeEnum } from '@app/enums/inspected-person-type.enum';
import { InspectionShipSubjectNomenclatureDTO } from '@app/models/generated/dtos/InspectionShipSubjectNomenclatureDTO';
import { InspectionsService } from '@app/services/administration-app/inspections.service';
import { InspectionCheckModel } from '../../models/inspection-check.model';
import { InspectionObservationCategoryEnum } from '@app/enums/inspection-observation-category.enum';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { InspectionObservationTextDTO } from '@app/models/generated/dtos/InspectionObservationTextDTO';
import { VesselDuringInspectionDTO } from '@app/models/generated/dtos/VesselDuringInspectionDTO';
import { InspectionCheckDTO } from '@app/models/generated/dtos/InspectionCheckDTO';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { ValidityCheckerDirective } from '@app/shared/directives/validity-checker/validity-checker.directive';
import { ShipNomenclatureDTO } from '@app/models/generated/dtos/ShipNomenclatureDTO';
import { InspectionSubjectPersonnelDTO } from '@app/models/generated/dtos/InspectionSubjectPersonnelDTO';
import { PortNomenclatureDTO } from '@app/models/generated/dtos/PortNomenclatureDTO';
import { ShipsUtils } from '@app/shared/utils/ships.utils';

@Component({
    selector: 'inspected-ship-with-personnel',
    templateUrl: './inspected-ship-with-personnel.component.html'
})
export class InspectedShipWithPersonnelComponent extends CustomFormControl<ShipWithPersonnelModel> implements OnInit, AfterViewInit, OnChanges {
    @Input()
    public hasMap: boolean = true;

    @Input()
    public hasPort: boolean = true;

    @Input()
    public toggles: InspectionCheckModel[] = [];

    @Input()
    public shipLabel: string;

    @Input()
    public shipObservationCategory: InspectionObservationCategoryEnum = InspectionObservationCategoryEnum.ShipData;

    @Input()
    public ships: ShipNomenclatureDTO[] = [];

    @Input()
    public countries: NomenclatureDTO<number>[] = [];

    @Input()
    public ports: PortNomenclatureDTO[] = [];

    @Input()
    public vesselTypes: NomenclatureDTO<number>[] = [];

    @Output()
    public shipSelected: EventEmitter<VesselDuringInspectionDTO> = new EventEmitter<VesselDuringInspectionDTO>();

    public owners: InspectionShipSubjectNomenclatureDTO[] = [];
    public users: InspectionShipSubjectNomenclatureDTO[] = [];
    public representatives: InspectionShipSubjectNomenclatureDTO[] = [];
    public captains: InspectionShipSubjectNomenclatureDTO[] = [];
    public permittedPortIds: number[] = [];

    public hasDanubePermit: boolean = false;
    public hasBlackSeaPermit: boolean = false;

    public readonly inspectedPersonTypeEnum: typeof InspectedPersonTypeEnum = InspectedPersonTypeEnum;

    private readonly service: InspectionsService;
    private readonly translate: FuseTranslationLoaderService;

    public constructor(
        @Self() ngControl: NgControl,
        @Self() validityChecker: ValidityCheckerDirective,
        service: InspectionsService,
        translate: FuseTranslationLoaderService,
    ) {
        super(ngControl, true, validityChecker);

        this.shipLabel = translate.getValue('inspections.ship-data');

        this.service = service;
        this.translate = translate;

        this.onMarkAsTouched.subscribe({
            next: () => {
                this.control.updateValueAndValidity();
            }
        });
    }

    public ngOnChanges(changes: SimpleChanges): void {
        const hasPort = changes['hasPort'];

        if (hasPort !== null && hasPort !== undefined) {
            if (this.hasPort && this.isDisabled === false) {
                this.form.get('portControl')!.enable();
            }
            else {
                this.form.get('portControl')!.disable();
            }
        }
    }

    public ngOnInit(): void {
        this.initCustomFormControl();
    }

    public ngAfterViewInit(): void {
        if (!this.isDisabled) {
            this.form.get('isRepresentativeSameAsOwnerControl')!.valueChanges.subscribe({
                next: (yes: boolean) => {
                    if (yes) {
                        const owner: InspectionSubjectPersonnelDTO | undefined = this.form.get('shipOwnerControl')!.value;

                        if (owner !== undefined && owner !== null && !owner.isLegal) {
                            this.form.get('shipRepresentativeControl')!.setValue(owner);
                            this.form.get('shipRepresentativeControl')!.disable();
                        }
                    }
                    else {
                        this.form.get('shipRepresentativeControl')!.enable();
                    }

                    this.form.get('shipRepresentativeControl')!.updateValueAndValidity({ emitEvent: false });
                    this.form.get('shipRepresentativeControl')!.markAsPending();
                }
            });

            this.form.get('isUserSameAsOwnerControl')!.valueChanges.subscribe({
                next: (yes: boolean) => {
                    if (yes) {
                        const owner: InspectionSubjectPersonnelDTO | undefined = this.form.get('shipOwnerControl')!.value;

                        if (owner !== undefined && owner !== null) {
                            const type: InspectedPersonTypeEnum = owner.isLegal ? InspectedPersonTypeEnum.LicUsrLgl : InspectedPersonTypeEnum.LicUsrPers;
                            const shipSubject: InspectionSubjectPersonnelDTO = this.mapSubjectToOwner(owner, type);

                            this.form.get('shipUserControl')!.setValue(shipSubject);
                            this.form.get('shipUserControl')!.disable();
                        }
                    }
                    else {
                        this.form.get('shipUserControl')!.enable();
                    }

                    this.form.get('shipUserControl')!.updateValueAndValidity({ emitEvent: false });
                    this.form.get('shipUserControl')!.markAsPending();
                }
            });
        }
    }

    public writeValue(value: ShipWithPersonnelModel): void {
        if (value !== undefined && value !== null) {
            this.form.get('shipControl')!.setValue(value.ship);
            this.form.get('togglesControl')!.setValue(value.toggles);
            this.form.get('portControl')!.setValue(value.port);
            this.filterPortsByShipPermits(value.ship?.shipId);

            const violation = value.observationTexts?.find(f => f.category === this.shipObservationCategory);

            if (violation !== null && violation !== undefined) {
                this.form.get('observationControl')!.setValue(violation.text);
            }

            if (!this.isDisabled && value.ship?.shipId !== null && value.ship?.shipId !== undefined) {
                this.service.getShipPersonnel(value.ship.shipId).subscribe({
                    next: (personnel: InspectionShipSubjectNomenclatureDTO[]) => {
                        this.assignPersonnel(personnel);

                        setTimeout(() => {
                            this.fillPersonnelControls(value.personnel ?? []);
                        });
                    }
                });
            }
            else {
                this.fillPersonnelControls(value.personnel ?? []);
            }
        }
        else {
            this.form.reset();
            this.owners = [];
            this.users = [];
            this.representatives = [];
            this.captains = [];
            this.permittedPortIds = [];
        }

        this.onChanged(this.getValue());
    }

    public async onShipSelected(ship: VesselDuringInspectionDTO): Promise<void> {
        this.shipSelected.emit(ship);

        if (ship !== undefined && ship !== null) {
            if (ship.shipId !== undefined && ship.shipId !== null) {
                const personnel: InspectionShipSubjectNomenclatureDTO[] = await this.service.getShipPersonnel(ship.shipId).toPromise();
                this.assignPersonnel(personnel);
                this.filterPortsByShipPermits(ship.shipId);
            }
        }
        else {
            this.form.get('shipOwnerControl')!.setValue(undefined);
            this.form.get('shipUserControl')!.setValue(undefined);
            this.form.get('shipRepresentativeControl')!.setValue(undefined);
            this.form.get('shipCaptainControl')!.setValue(undefined);

            this.owners = [];
            this.users = [];
            this.representatives = [];
            this.captains = [];
            this.permittedPortIds = [];
        }
    }

    public assignPersonnel(personnel: InspectionShipSubjectNomenclatureDTO[]): void {
        this.owners = personnel.filter(f => f.type === InspectedPersonTypeEnum.OwnerLegal || f.type === InspectedPersonTypeEnum.OwnerPers);
        this.users = personnel.filter(f => f.type === InspectedPersonTypeEnum.LicUsrLgl || f.type === InspectedPersonTypeEnum.LicUsrPers);
        const persons = personnel.filter(f => f.type !== InspectedPersonTypeEnum.OwnerLegal && f.type !== InspectedPersonTypeEnum.LicUsrLgl);

        // nomenclature value is Person.ID (this is for removing duplicates)
        this.representatives = persons.filter((f, index) => persons.findIndex(s => s.value === f.value) === index);

        for (let i = 0; i < this.representatives.length; i++) {
            const rep = this.representatives[i];

            let typeName = '';

            switch (rep.type!) {
                case InspectedPersonTypeEnum.ReprsPers:
                    typeName = this.translate.getValue('inspections.ship-representative');
                    break;
                case InspectedPersonTypeEnum.ActualOwn:
                case InspectedPersonTypeEnum.OwnerBuyer:
                case InspectedPersonTypeEnum.OwnerLegal:
                case InspectedPersonTypeEnum.OwnerPers:
                    typeName = this.translate.getValue('inspections.ship-owner');
                    break;
                case InspectedPersonTypeEnum.CaptFshmn:
                    typeName = this.translate.getValue('inspections.ship-captain');
                    break;
                case InspectedPersonTypeEnum.LicUsrLgl:
                case InspectedPersonTypeEnum.LicUsrPers:
                    typeName = this.translate.getValue('inspections.ship-user');
                    break;
            }

            this.representatives[i] = new InspectionShipSubjectNomenclatureDTO({
                address: rep.address,
                code: rep.code,
                countryId: rep.countryId,
                description: rep.description,
                displayName: rep.displayName + ` (${typeName})`,
                egnLnc: rep.egnLnc,
                eik: rep.eik,
                entryId: rep.entryId,
                firstName: rep.firstName,
                isActive: rep.isActive,
                isLegal: rep.isLegal,
                lastName: rep.lastName,
                middleName: rep.middleName,
                type: rep.type,
                value: rep.value
            });
        }

        this.captains = personnel.filter(f => f.type === InspectedPersonTypeEnum.CaptFshmn);
    }

    protected buildForm(): AbstractControl {
        const form = new FormGroup({
            shipControl: new FormControl(undefined, Validators.required),
            shipOwnerControl: new FormControl(undefined, Validators.required),
            shipUserControl: new FormControl(undefined, Validators.required),
            shipRepresentativeControl: new FormControl(undefined, Validators.required),
            shipCaptainControl: new FormControl(undefined, Validators.required),
            togglesControl: new FormControl([]),
            portControl: new FormControl(undefined),
            observationControl: new FormControl(undefined, Validators.maxLength(4000)),
            isUserSameAsOwnerControl: new FormControl(false),
            isRepresentativeSameAsOwnerControl: new FormControl(false)
        });

        return form;
    }

    protected getValue(): ShipWithPersonnelModel {
        const observation: string = this.form.get('observationControl')!.value;
        const ship: VesselDuringInspectionDTO = this.form.get('shipControl')!.value;
        const toggles: InspectionCheckDTO[] = this.form.get('togglesControl')!.value ?? [];

        return new ShipWithPersonnelModel({
            personnel: [
                this.form.get('shipOwnerControl')!.value,
                this.form.get('shipUserControl')!.value,
                this.form.get('shipRepresentativeControl')!.value,
                this.form.get('shipCaptainControl')!.value,
            ].filter(f => f !== undefined && f !== null),
            toggles: toggles,
            ship: ship,
            port: this.form.get('portControl')!.value,
            observationTexts: !CommonUtils.isNullOrWhiteSpace(observation)
                ? [new InspectionObservationTextDTO({
                    category: this.shipObservationCategory,
                    text: observation
                })] : [],
        })
    }

    private filterPortsByShipPermits(shipId: number | undefined): void {
        if (shipId !== undefined && shipId !== null) {
            const ship: ShipNomenclatureDTO = ShipsUtils.get(this.ships, shipId);
            this.hasDanubePermit = ShipsUtils.hasDanubePermit(ship);
            this.hasBlackSeaPermit = ShipsUtils.hasBlackSeaPermit(ship);
        }

        this.permittedPortIds = this.ports.filter(x => (this.hasDanubePermit && x.isDanube) || (this.hasBlackSeaPermit && x.isBlackSea)).map(x => x.value!);
    }

    private fillPersonnelControls(personnel: InspectionSubjectPersonnelDTO[]): void {
        const shipOwner: InspectionSubjectPersonnelDTO | undefined = personnel.find(x => x.type === InspectedPersonTypeEnum.OwnerLegal || x.type === InspectedPersonTypeEnum.OwnerPers);
        const shipUser: InspectionSubjectPersonnelDTO | undefined = personnel.find(x => x.type === InspectedPersonTypeEnum.LicUsrLgl || x.type === InspectedPersonTypeEnum.LicUsrPers);
        const shipRepresentative: InspectionSubjectPersonnelDTO | undefined = personnel.find(x => x.type === InspectedPersonTypeEnum.ReprsPers);
        const shipCaptain: InspectionSubjectPersonnelDTO | undefined = personnel.find(x => x.type === InspectedPersonTypeEnum.CaptFshmn);

        this.form.get('shipOwnerControl')!.setValue(shipOwner);
        this.form.get('shipUserControl')!.setValue(shipUser);
        this.form.get('shipRepresentativeControl')!.setValue(shipRepresentative);
        this.form.get('shipCaptainControl')!.setValue(shipCaptain);

        if (shipOwner !== undefined && shipOwner !== null) {
            if (shipUser !== undefined && shipUser !== null) {
                const isSameAsOwner: boolean = this.isInspectedSubjectSameAsShipOwner(shipOwner, shipUser);
                this.form.get('isUserSameAsOwnerControl')!.setValue(isSameAsOwner);
            }

            if (shipRepresentative !== undefined && shipRepresentative !== null) {
                const isSameAsOwner: boolean = this.isInspectedSubjectSameAsShipOwner(shipOwner, shipRepresentative);
                this.form.get('isRepresentativeSameAsOwnerControl')!.setValue(isSameAsOwner);
            }
        }
    }

    private isInspectedSubjectSameAsShipOwner(shipOwner: InspectionSubjectPersonnelDTO, inspectedSubject: InspectionSubjectPersonnelDTO): boolean {
        if (shipOwner.isLegal === inspectedSubject.isLegal) {
            if (shipOwner.entryId === undefined || shipOwner.entryId === null) {
                if (shipOwner.isLegal && inspectedSubject.isLegal) {
                    if (shipOwner.eik === inspectedSubject.eik) {
                        return true;
                    }
                }
                else {
                    if (shipOwner.egnLnc?.identifierType === inspectedSubject.egnLnc?.identifierType && shipOwner.egnLnc?.egnLnc === inspectedSubject.egnLnc?.egnLnc) {
                        return true;
                    }
                }
            }
            else if (shipOwner.entryId === inspectedSubject.entryId) {
                return true;
            }
        }

        return false;
    }

    private mapSubjectToOwner(owner: InspectionSubjectPersonnelDTO, type: InspectedPersonTypeEnum): InspectionSubjectPersonnelDTO {
        const shipSubject: InspectionSubjectPersonnelDTO = new InspectionSubjectPersonnelDTO({
            isLegal: owner.isLegal,
            isRegistered: owner.isRegistered,
            citizenshipId: owner.citizenshipId,
            entryId: owner.entryId,
            address: owner.address,
            comment: owner.comment,
            egnLnc: owner.egnLnc,
            eik: owner.eik,
            firstName: owner.firstName,
            middleName: owner.middleName,
            hasBulgarianAddressRegistration: owner.hasBulgarianAddressRegistration,
            registeredAddress: owner.registeredAddress,
            lastName: owner.lastName,
            type: type
        });

        return shipSubject;
    }
}