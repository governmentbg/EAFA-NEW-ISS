import { Component, EventEmitter, Input, OnChanges, OnInit, Output, Self, SimpleChanges } from '@angular/core';
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

@Component({
    selector: 'inspected-ship-with-personnel',
    templateUrl: './inspected-ship-with-personnel.component.html'
})
export class InspectedShipWithPersonnelComponent extends CustomFormControl<ShipWithPersonnelModel> implements OnInit, OnChanges {
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
    public ports: NomenclatureDTO<number>[] = [];

    @Input()
    public vesselTypes: NomenclatureDTO<number>[] = [];

    @Output()
    public shipSelected: EventEmitter<VesselDuringInspectionDTO> = new EventEmitter<VesselDuringInspectionDTO>();

    public owners: InspectionShipSubjectNomenclatureDTO[] = [];
    public users: InspectionShipSubjectNomenclatureDTO[] = [];
    public representatives: InspectionShipSubjectNomenclatureDTO[] = [];
    public captains: InspectionShipSubjectNomenclatureDTO[] = [];

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

    public async ngOnInit(): Promise<void> {
        this.initCustomFormControl();
    }

    public writeValue(value: ShipWithPersonnelModel): void {
        if (value !== undefined && value !== null) {
            this.form.get('shipControl')!.setValue(value.ship);
            this.form.get('togglesControl')!.setValue(value.toggles);
            this.form.get('portControl')!.setValue(value.port);

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
        }

        this.onChanged(this.getValue());
    }

    public async onShipSelected(ship: VesselDuringInspectionDTO): Promise<void> {
        this.shipSelected.emit(ship);

        if (ship !== undefined && ship !== null) {
            if (ship.shipId !== undefined && ship.shipId !== null) {
                const personnel: InspectionShipSubjectNomenclatureDTO[] = await this.service.getShipPersonnel(ship.shipId).toPromise();

                this.assignPersonnel(personnel);
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
            observationControl: new FormControl(undefined, Validators.maxLength(4000))
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

    private fillPersonnelControls(personnel: InspectionSubjectPersonnelDTO[]): void {
        this.form.get('shipOwnerControl')!.setValue(personnel.find(x => x.type === InspectedPersonTypeEnum.OwnerLegal || x.type === InspectedPersonTypeEnum.OwnerPers));
        this.form.get('shipUserControl')!.setValue(personnel.find(x => x.type === InspectedPersonTypeEnum.LicUsrLgl || x.type === InspectedPersonTypeEnum.LicUsrPers));
        this.form.get('shipRepresentativeControl')!.setValue(personnel.find(x => x.type === InspectedPersonTypeEnum.ReprsPers));
        this.form.get('shipCaptainControl')!.setValue(personnel.find(x => x.type === InspectedPersonTypeEnum.CaptFshmn));
    }
}