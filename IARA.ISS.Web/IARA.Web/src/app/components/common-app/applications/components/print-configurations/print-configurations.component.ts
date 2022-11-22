import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Subscription } from 'rxjs';

import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { PrintConfigurationParameters } from '@app/components/common-app/applications/models/print-configuration-parameters.model';
import { FormControlDataLoader } from '@app/shared/utils/form-control-data-loader';
import { IPrintConfigurationsService } from '../../interfaces/print-cofigurations.interface';
import { ApplicationsProcessingService } from '@app/services/administration-app/applications-processing.service';
import { PrintUserNomenclatureDTO } from '@app/models/generated/dtos/PrintUserNomenclatureDTO';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';

@Component({
    selector: 'print-configurations',
    templateUrl: './print-configurations.component.html'
})
export class PrintConfigurationsComponent implements IDialogComponent, OnInit {
    public form!: FormGroup;
    public users: any[] = [];

    private readonly nomenclatureLoader: FormControlDataLoader;
    private readonly service: IPrintConfigurationsService;

    public constructor(service: ApplicationsProcessingService) {
        this.service = service;

        this.buildForm();

        this.nomenclatureLoader = new FormControlDataLoader(this.getNomenclatures.bind(this));
    }

    public ngOnInit(): void {
        this.nomenclatureLoader.load();
    }

    public setData(data: never, wrapperData: DialogWrapperData): void {
        //
    }

    public dialogButtonClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public saveBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        this.form.markAllAsTouched();

        if (this.form.valid) {
            const data = this.fillModel();
            dialogClose(data);
        }
    }

    public cancelBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    private buildForm(): void {
        this.form = new FormGroup({
            userControl: new FormControl(undefined, [Validators.required]),
            showOnlyMyTUControl: new FormControl(true),
            positionControl: new FormControl(undefined, [Validators.required])
        });

        this.form.get('showOnlyMyTUControl')!.valueChanges.subscribe({
            next: (value: boolean | undefined) => {
                this.form.get('userControl')!.reset();
                this.getUsers();
            }
        });

        this.form.get('userControl')!.valueChanges.subscribe({
            next: (value: string | PrintUserNomenclatureDTO | undefined | null) => {
                if (value instanceof NomenclatureDTO) {
                    this.form.get('positionControl')!.setValue(value.position);
                }
            }
        });
    }

    private fillModel(): PrintConfigurationParameters {
        return new PrintConfigurationParameters({
            userId: this.form.get('userControl')!.value?.value,
            position: this.form.get('positionControl')!.value
        });
    }

    private getNomenclatures(): Subscription {
        const subscription: Subscription = this.getUsers();

        this.nomenclatureLoader.complete();
        return subscription;
    }

    private getUsers(): Subscription {
        let subscription: Subscription;

        if (this.form.get('showOnlyMyTUControl')!.value) {
            subscription = NomenclatureStore.instance.getNomenclature(
                NomenclatureTypes.MyTerritoryUnitUsers,
                this.service.getMyTerritoryUnitUsersNomenclature.bind(this.service),
                false
            ).subscribe({
                next: (results: PrintUserNomenclatureDTO[]) => {
                    this.users = results;
                }
            });
        }
        else {
            subscription = NomenclatureStore.instance.getNomenclature(
                NomenclatureTypes.AllUsers,
                this.service.getUsersNomenclature.bind(this.service),
                false
            ).subscribe({
                next: (results: PrintUserNomenclatureDTO[]) => {
                    this.users = results;
                }
            });
        }

        return subscription;
    }

}