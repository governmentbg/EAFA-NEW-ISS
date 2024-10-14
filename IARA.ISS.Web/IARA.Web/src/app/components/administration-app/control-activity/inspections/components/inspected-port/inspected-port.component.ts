import { Component, Input, OnChanges, OnInit, Self, SimpleChanges } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, NgControl, Validators } from '@angular/forms';

import { CustomFormControl } from '@app/shared/utils/custom-form-control';
import { PortVisitDTO } from '@app/models/generated/dtos/PortVisitDTO';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { IGroupedOptions } from '@app/shared/components/input-controls/tl-autocomplete/interfaces/grouped-options.interface';
import { PortNomenclatureDTO } from '@app/models/generated/dtos/PortNomenclatureDTO';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';

@Component({
    selector: 'inspected-port',
    templateUrl: './inspected-port.component.html'
})
export class InspectedPortComponent extends CustomFormControl<PortVisitDTO | undefined> implements OnInit, OnChanges {
    @Input()
    public hasDate: boolean = true;

    @Input()
    public ports: PortNomenclatureDTO[] = [];

    @Input()
    public countries: NomenclatureDTO<number>[] = [];

    @Input()
    public portsGrouped: IGroupedOptions<number>[] = [];

    @Input()
    public permittedPortIds: number[] = [];

    public isFromRegister: boolean = true;

    private readonly translate: FuseTranslationLoaderService;

    public constructor(
        @Self() ngControl: NgControl,
        translate: FuseTranslationLoaderService
    ) {
        super(ngControl);

        this.translate = translate;

        this.onMarkAsTouched.subscribe({
            next: () => {
                this.control.updateValueAndValidity();
            }
        });
    }

    public ngOnInit(): void {
        this.initCustomFormControl();
    }

    public ngOnChanges(changes: SimpleChanges): void {
        if ('permittedPortIds' in this) {
            if (this.permittedPortIds === undefined || this.permittedPortIds === null) {
                this.permittedPortIds = [];
            }

            if (this.ports !== undefined && this.ports !== null) {
                this.filterPermittedPorts();
            }
        }
    }

    public writeValue(value: PortVisitDTO | undefined): void {
        if (value !== undefined && value !== null) {
            this.isFromRegister = value.portId !== undefined && value.portId !== null;
            this.form.get('portRegisteredControl')!.setValue(this.isFromRegister);
            this.form.get('dateControl')!.setValue(value.visitDate);

            if (this.isFromRegister) {
                const portGroup = this.portsGrouped.find(x => (x.options as PortNomenclatureDTO[]).some(x => x.value === value.portId))!;
                const port: PortNomenclatureDTO = (portGroup.options as PortNomenclatureDTO[]).find(x => x.value === value.portId)!;
                this.form.get('portControl')!.setValue(port);
            }
            else {
                this.form.get('nameControl')!.setValue(value.portName);
                this.form.get('countryControl')!.setValue(this.countries.find(x => x.value === value.portCountryId));
            }
        }
        else {
            this.form.get('countryControl')!.setValue(this.countries.find(x => x.code === CommonUtils.COUNTRIES_BG));
        }
    }

    protected buildForm(): AbstractControl {
        const form = new FormGroup({
            portRegisteredControl: new FormControl(true),
            portControl: new FormControl(undefined, Validators.required),
            nameControl: new FormControl(undefined, Validators.maxLength(200)),
            countryControl: new FormControl(undefined),
            dateControl: new FormControl(undefined)
        });

        form.get('portRegisteredControl')!.valueChanges.subscribe({
            next: this.onPortRegisteredChanged.bind(this)
        });

        return form;
    }

    protected getValue(): PortVisitDTO | undefined {
        if (this.isFromRegister) {
            const portControl = this.form.get('portControl')!;

            if (portControl.value !== undefined && portControl.value !== null) {
                return new PortVisitDTO({
                    portId: portControl.value.value,
                    visitDate: this.form.get('dateControl')!.value
                });
            }
        }
        else {
            return new PortVisitDTO({
                portCountryId: this.form.get('countryControl')!.value?.value,
                portName: this.form.get('nameControl')!.value,
                visitDate: this.form.get('dateControl')!.value
            });
        }

        return undefined;
    }

    private onPortRegisteredChanged(value: boolean): void {
        this.isFromRegister = value;

        if (value) {
            this.form.get('portControl')!.setValidators(Validators.required);
            this.form.get('nameControl')!.clearValidators();
            this.form.get('countryControl')!.clearValidators();
        }
        else {
            this.form.get('portControl')!.clearValidators();
            this.form.get('nameControl')!.setValidators([Validators.required, Validators.maxLength(500)]);
            this.form.get('countryControl')!.setValidators([Validators.required]);
        }

        this.form.get('portControl')!.updateValueAndValidity({ emitEvent: false });
        this.form.get('nameControl')!.updateValueAndValidity({ emitEvent: false });
        this.form.get('countryControl')!.updateValueAndValidity({ emitEvent: false });
    }

    public filterPermittedPorts(): void {
        const ports: PortNomenclatureDTO[] = this.ports.filter(x => x.displayName !== undefined && x.displayName !== null && x.displayName !== '');

        const newPorts: IGroupedOptions<number>[] = [
            {
                name: this.translate.getValue('inspections.permitted-ports'),
                options: ports.filter(x => this.permittedPortIds.includes(x.value!))
            },
            {
                name: this.translate.getValue('inspections.other-ports-danube'),
                options: ports.filter(x => !this.permittedPortIds.includes(x.value!) && x.isDanube)
            },
            {
                name: this.translate.getValue('inspections.other-ports-black-sea'),
                options: ports.filter(x => !this.permittedPortIds.includes(x.value!) && x.isBlackSea)
            },
            {
                name: this.translate.getValue('inspections.other-ports-country'),
                options: ports.filter(x => !this.permittedPortIds.includes(x.value!) && !x.isDanube && !x.isBlackSea)
            }
        ];

        if (newPorts[0].options.length === 0) {
            newPorts.splice(0, 1);
        }

        this.portsGrouped = newPorts;
    }
}