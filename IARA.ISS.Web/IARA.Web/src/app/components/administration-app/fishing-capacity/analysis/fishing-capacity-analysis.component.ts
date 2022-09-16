import { HttpErrorResponse } from '@angular/common/http';
import { AfterViewInit, Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';

import { IFishingCapacityService } from '@app/interfaces/common-app/fishing-capacity.interface';
import { ErrorCode, ErrorModel } from '@app/models/common/exception.model';
import { FishingCapacityStatisticsDTO } from '@app/models/generated/dtos/FishingCapacityStatistics';
import { FishingCapacityAdministrationService } from '@app/services/administration-app/fishing-capacity-administration.service';

@Component({
    selector: 'fishing-capacity-analysis',
    templateUrl: './fishing-capacity-analysis.component.html'
})
export class FishingCapacityAnalysisComponent implements OnInit, AfterViewInit {
    public readonly today: Date = new Date();

    public form!: FormGroup;

    public noMaximumCapacityError: boolean = false;
    public dataLoaded: boolean = false;

    private statistics: FishingCapacityStatisticsDTO | undefined;
    private service: IFishingCapacityService;

    public constructor(service: FishingCapacityAdministrationService) {
        this.service = service;

        this.buildForm();
    }

    public ngOnInit(): void {
        this.loadData();
    }

    public ngAfterViewInit(): void {
        this.form.get('dateControl')!.valueChanges.subscribe({
            next: (date: Date) => {
                this.loadData();
            }
        });
    }

    private buildForm(): void {
        this.form = new FormGroup({
            dateControl: new FormControl(new Date(), Validators.required),
            maximumCapacityTonnageControl: new FormControl({ value: undefined, disabled: true }),
            maximumCapacityPowerControl: new FormControl({ value: undefined, disabled: true }),
            totalUnusedCapacityTonnageControl: new FormControl({ value: undefined, disabled: true }),
            totalUnusedCapacityPowerControl: new FormControl({ value: undefined, disabled: true }),
            activeCertificatesCapacityTonnageControl: new FormControl({ value: undefined, disabled: true }),
            activeCertificatesCapacityPowerControl: new FormControl({ value: undefined, disabled: true }),
            activeShipCapacityTonnageControl: new FormControl({ value: undefined, disabled: true }),
            activeShipCapacityPowerControl: new FormControl({ value: undefined, disabled: true })
        });
    }

    private loadData(): void {
        const date: Date = this.form.get('dateControl')!.value;

        this.service.getFishingCapacityStatistics(date).subscribe({
            next: (statistics: FishingCapacityStatisticsDTO) => {
                this.statistics = statistics;
                this.fillForm();

                this.noMaximumCapacityError = false;
                this.dataLoaded = true;
            },
            error: (response: HttpErrorResponse) => {
                if (response.error !== undefined && response.error !== null) {
                    const error: ErrorModel = response.error as ErrorModel;

                    if (error.code === ErrorCode.NoMaximumFishingCapacityToDate) {
                        this.noMaximumCapacityError = true;
                    }
                    this.dataLoaded = true;
                }
            }
        });
    }

    private fillForm(): void {
        this.form.get('maximumCapacityTonnageControl')!.setValue(this.statistics!.maximumFishingCapacity!.grossTonnage);
        this.form.get('maximumCapacityPowerControl')!.setValue(this.statistics!.maximumFishingCapacity!.enginePower);
        this.form.get('totalUnusedCapacityTonnageControl')!.setValue(this.statistics!.totalUnusedFishingCapacity!.grossTonnage);
        this.form.get('totalUnusedCapacityPowerControl')!.setValue(this.statistics!.totalUnusedFishingCapacity!.enginePower);
        this.form.get('activeCertificatesCapacityTonnageControl')!.setValue(this.statistics!.totalCapacityFromActiveCertificates!.grossTonnage);
        this.form.get('activeCertificatesCapacityPowerControl')!.setValue(this.statistics!.totalCapacityFromActiveCertificates!.enginePower);
        this.form.get('activeShipCapacityTonnageControl')!.setValue(this.statistics!.totalActiveShipFishingCapacity!.grossTonnage);
        this.form.get('activeShipCapacityPowerControl')!.setValue(this.statistics!.totalActiveShipFishingCapacity!.enginePower);
    }
}