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

@Component({
    selector: 'inspected-ship-with-personnel',
    templateUrl: './inspected-ship-with-personnel.component.html'
})
export class InspectedShipWithPersonnelComponent extends CustomFormControl<ShipWithPersonnelModel> implements OnInit, OnChanges {

    @Output()
    public shipSelected: EventEmitter<VesselDuringInspectionDTO> = new EventEmitter<VesselDuringInspectionDTO>();

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
    public ships: NomenclatureDTO<number>[] = [];

    @Input()
    public countries: NomenclatureDTO<number>[] = [];

    @Input()
    public ports: NomenclatureDTO<number>[] = [];

    @Input()
    public vesselTypes: NomenclatureDTO<number>[] = [];

    public owners: InspectionShipSubjectNomenclatureDTO[] = [];
    public users: InspectionShipSubjectNomenclatureDTO[] = [];
    public representatives: InspectionShipSubjectNomenclatureDTO[] = [];
    public captains: InspectionShipSubjectNomenclatureDTO[] = [];

    public readonly inspectedPersonTypeEnum: typeof InspectedPersonTypeEnum = InspectedPersonTypeEnum;

    private readonly service: InspectionsService;

    public constructor(@Self() ngControl: NgControl,
        service: InspectionsService,
        translate: FuseTranslationLoaderService,
    ) {
        super(ngControl);

        this.shipLabel = translate.getValue('inspections.ship-data');

        this.service = service;
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
                            const personnel = value.personnel ?? [];

                            this.form.get('shipOwnerControl')!.setValue(
                                personnel.find(f => f.type === InspectedPersonTypeEnum.OwnerLegal || f.type === InspectedPersonTypeEnum.OwnerPers)
                            );
                            this.form.get('shipUserControl')!.setValue(
                                personnel.find(f => f.type === InspectedPersonTypeEnum.LicUsrLgl || f.type === InspectedPersonTypeEnum.LicUsrPers)
                            );
                            this.form.get('shipRepresentativeControl')!.setValue(
                                personnel.find(f => f.type === InspectedPersonTypeEnum.ReprsPers)
                            );
                            this.form.get('shipCaptainControl')!.setValue(
                                personnel.find(f => f.type === InspectedPersonTypeEnum.CaptFshmn)
                            );
                        });
                    }
                });
            }
            else {
                const personnel = value.personnel ?? [];

                this.form.get('shipOwnerControl')!.setValue(
                    personnel.find(f => f.type === InspectedPersonTypeEnum.OwnerLegal || f.type === InspectedPersonTypeEnum.OwnerPers)
                );
                this.form.get('shipUserControl')!.setValue(
                    personnel.find(f => f.type === InspectedPersonTypeEnum.LicUsrLgl || f.type === InspectedPersonTypeEnum.LicUsrPers)
                );
                this.form.get('shipRepresentativeControl')!.setValue(
                    personnel.find(f => f.type === InspectedPersonTypeEnum.ReprsPers)
                );
                this.form.get('shipCaptainControl')!.setValue(
                    personnel.find(f => f.type === InspectedPersonTypeEnum.CaptFshmn)
                );
            }
        }
    }

    public async onShipSelected(ship: VesselDuringInspectionDTO): Promise<void> {
        this.shipSelected.emit(ship);
        const personnel = await this.service.getShipPersonnel(ship.shipId!).toPromise();

        this.assignPersonnel(personnel);
    }

    public assignPersonnel(personnel: InspectionShipSubjectNomenclatureDTO[]): void {
        this.owners = personnel.filter(f => f.type === InspectedPersonTypeEnum.OwnerLegal || f.type === InspectedPersonTypeEnum.OwnerPers);

        this.users = personnel.filter(f => f.type === InspectedPersonTypeEnum.LicUsrLgl || f.type === InspectedPersonTypeEnum.LicUsrPers);

        const persons = personnel.filter(f => f.type !== InspectedPersonTypeEnum.OwnerLegal && f.type !== InspectedPersonTypeEnum.LicUsrLgl);

        // nomenclature value is Person.ID (this is for removing duplicates)
        this.representatives = persons.filter((f, index) => persons.findIndex(s => s.value === f.value) === index);

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
            observationControl: new FormControl(undefined),
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
}