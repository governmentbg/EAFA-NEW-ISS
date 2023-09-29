import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';

import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { FluxVmsRequestsService } from '@app/services/administration-app/flux-vms-requests.service';
import { FluxVesselQueryRequestEditDTO } from '@app/models/generated/dtos/FluxVesselQueryRequestEditDTO';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';

@Component({
    selector: 'flux-vessel-query',
    templateUrl: './flux-vessel-query.component.html'
})
export class FluxVesselQueryComponent implements OnInit, IDialogComponent {
    public readonly form: FormGroup;

    public flagStates: NomenclatureDTO<number>[] = [];
    public histOptions: NomenclatureDTO<boolean>[];
    public vesselActiveOptions: NomenclatureDTO<boolean>[];
    public dataOptions: NomenclatureDTO<boolean>[];

    private model: FluxVesselQueryRequestEditDTO = new FluxVesselQueryRequestEditDTO();

    private readonly service: FluxVmsRequestsService;
    private readonly translate: FuseTranslationLoaderService;

    public constructor(service: FluxVmsRequestsService, translate: FuseTranslationLoaderService) {
        this.service = service;
        this.translate = translate;

        this.form = this.buildForm();

        this.histOptions = [
            new NomenclatureDTO<boolean>({
                value: false,
                displayName: this.translate.getValue('flux-vms-requests.vessel-query-hist-no'),
                isActive: true
            }),
            new NomenclatureDTO<boolean>({
                value: true,
                displayName: this.translate.getValue('flux-vms-requests.vessel-query-hist-yes'),
                isActive: true
            })
        ];

        this.vesselActiveOptions = [
            new NomenclatureDTO<boolean>({
                value: true,
                displayName: this.translate.getValue('flux-vms-requests.vessel-query-vessel-active-yes'),
                isActive: true
            }),
            new NomenclatureDTO<boolean>({
                value: false,
                displayName: this.translate.getValue('flux-vms-requests.vessel-query-vessel-active-no'),
                isActive: true
            }),
        ];

        this.dataOptions = [
            new NomenclatureDTO<boolean>({
                value: false,
                displayName: this.translate.getValue('flux-vms-requests.vessel-query-data-no'),
                isActive: true
            }),
            new NomenclatureDTO<boolean>({
                value: true,
                displayName: this.translate.getValue('flux-vms-requests.vessel-query-data-yes'),
                isActive: true
            })
        ];
    }

    public ngOnInit(): void {
        this.service.getTerritories().subscribe({
            next: (territories: NomenclatureDTO<number>[]) => {
                this.flagStates = territories;
            }
        });
    }

    public setData(data: unknown, wrapperData: DialogWrapperData): void {
        // nothing to do
    }

    public saveBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        this.form.markAllAsTouched();

        if (this.form.valid) {
            this.fillModel();
            CommonUtils.sanitizeModelStrings(this.model);

            this.service.addVesselQueryRequest(this.model).subscribe({
                next: () => {
                    dialogClose(true);
                },
                error: () => {
                    dialogClose(false);
                }
            });
        }
    }

    public cancelBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose(false);
    }

    public dialogButtonClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose(false);
    }

    private buildForm(): FormGroup {
        return new FormGroup({
            dateTimeFromControl: new FormControl(undefined, Validators.required),
            dateTimeToControl: new FormControl(undefined, Validators.required),
            cfrControl: new FormControl(undefined),
            uviControl: new FormControl(undefined),
            ircsControl: new FormControl(undefined),
            mmsiControl: new FormControl(undefined),
            flagStateControl: new FormControl(undefined),
            histControl: new FormControl(undefined, Validators.required),
            vesselActiveControl: new FormControl(undefined, Validators.required),
            dataControl: new FormControl(undefined, Validators.required)
        });
    }

    private fillModel(): void {
        this.model.dateTimeFrom = this.form.get('dateTimeFromControl')!.value;
        this.model.dateTimeTo = this.form.get('dateTimeToControl')!.value;
        this.model.cfr = this.form.get('cfrControl')!.value;
        this.model.uvi = this.form.get('uviControl')!.value;
        this.model.ircs = this.form.get('ircsControl')!.value;
        this.model.mmsi = this.form.get('mmsiControl')!.value;
        this.model.flagStateCode = this.form.get('flagStateControl')!.value?.code;
        this.model.histYes = this.form.get('histControl')!.value.value === true;
        this.model.histNo = this.form.get('histControl')!.value.value === false;
        this.model.vesselActive = this.form.get('vesselActiveControl')!.value.value === true;
        this.model.vesselAll = this.form.get('vesselActiveControl')!.value.value === false;
        this.model.dataVcd = this.form.get('dataControl')!.value.value === false;
        this.model.dataAll = this.form.get('dataControl')!.value.value === true;
    }
}