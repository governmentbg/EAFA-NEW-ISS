import { Component, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';

import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { DialogParamsModel } from '@app/models/common/dialog-params.model';
import { IApplicationsRegisterService } from '@app/interfaces/administration-app/applications-register.interface';
import { AssignedApplicationInfoDTO } from '@app/models/generated/dtos/AssignedApplicationInfoDTO';
import { PrintUserNomenclatureDTO } from '@app/models/generated/dtos/PrintUserNomenclatureDTO';
import { FormControlDataLoader } from '@app/shared/utils/form-control-data-loader';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';

@Component({
    selector: 'assign-application-by-user',
    templateUrl: './assign-application-by-user.component.html'
})
export class AssignApplicationByUserComponent implements OnInit, IDialogComponent {
    public form!: FormGroup;
    public users: PrintUserNomenclatureDTO[] = [];

    private readonly nomenclatureLoader: FormControlDataLoader;
    private service!: IApplicationsRegisterService;
    private applicationId!: number;

    public constructor() {
        this.nomenclatureLoader = new FormControlDataLoader(this.getNomenclatures.bind(this));

        this.buildForm();
    }

    public ngOnInit(): void {
        this.nomenclatureLoader.load();
    }

    public setData(data: DialogParamsModel, wrapperData: DialogWrapperData): void {
        this.service = data.service! as IApplicationsRegisterService;
        this.applicationId = data.applicationId!;
    }

    public dialogButtonClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public saveBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        this.form.markAllAsTouched();

        if (this.form.valid) {
            const userId: number = this.form.get('userControl')!.value?.value;
            this.service.assignApplicationViaUserId(this.applicationId, userId).subscribe({
                next: (applicationData: AssignedApplicationInfoDTO) => {
                    dialogClose(applicationData);
                }
            });
        }
    }

    public cancelBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    private buildForm(): void {
        this.form = new FormGroup({
            userControl: new FormControl(undefined, Validators.required),
            onlyMyTerritoryUnitControl: new FormControl(true)
        });

        this.form.get('onlyMyTerritoryUnitControl')!.valueChanges.subscribe({
            next: (value: boolean | undefined) => {
                this.form.get('userControl')!.reset();

                this.getUsers();
            }
        });
    }

    private getNomenclatures(): Subscription {
        const subscription: Subscription = this.getUsers();
        return subscription;
    }

    private getUsers(): Subscription {
        let subscription: Subscription;

        if (this.form.get('onlyMyTerritoryUnitControl')!.value) {
            subscription = NomenclatureStore.instance.getNomenclature(
                NomenclatureTypes.MyTerritoryUnitUsers,
                this.service.getMyTerritoryUnitUsersNomenclature.bind(this.service),
                false
            ).subscribe({
                next: (results: PrintUserNomenclatureDTO[]) => {
                    this.users = results;
                    this.nomenclatureLoader.complete();
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
                    this.nomenclatureLoader.complete();
                }
            });
        }

        return subscription;
    }
}