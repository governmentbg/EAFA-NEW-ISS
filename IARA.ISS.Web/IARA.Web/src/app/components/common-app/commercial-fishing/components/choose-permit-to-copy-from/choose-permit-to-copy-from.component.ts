import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';

import { PageCodeEnum } from '@app/enums/page-code.enum';
import { ICommercialFishingService } from '@app/interfaces/common-app/commercial-fishing.interface';
import { PermitNomenclatureDTO } from '@app/models/generated/dtos/PermitNomenclatureDTO';
import { ShipNomenclatureDTO } from '@app/models/generated/dtos/ShipNomenclatureDTO';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { IS_PUBLIC_APP } from '@app/shared/modules/application.modules';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { ChoosePermitToCopyFromDialogParams } from './models/choose-permit-to-copy-from-dialog-params.model';
import { ChoosePermitToCopyFromDialogResult } from './models/choose-permit-to-copy-from-dialog-result.model';
import { ShipsUtils } from '@app/shared/utils/ships.utils';

@Component({
    selector: 'choose-permit-to-copy-from',
    templateUrl: './choose-permit-to-copy-from.component.html'
})
export class ChoosePermitToCopyFromComponent implements OnInit, IDialogComponent {
    public formGroup: FormGroup;

    public isPublicApp: boolean = false;
    public noShipSelected: boolean = true;

    public ships: ShipNomenclatureDTO[] = [];
    public permits: PermitNomenclatureDTO[] = [];

    private pageCode!: PageCodeEnum;
    private service!: ICommercialFishingService;
    private shipId: number | undefined;
    private permitId: number | undefined;
    private permitNumber: string | undefined;

    public constructor() {
        this.isPublicApp = IS_PUBLIC_APP;

        this.formGroup = new FormGroup({
            shipControl: new FormControl(null, Validators.required),
        });

        if (this.isPublicApp) {
            this.formGroup.addControl('permitRegistrationNumberControl', new FormControl(null, Validators.required));
        }
        else {
            this.formGroup.addControl('permitControl', new FormControl(null, Validators.required));
        }
    }

    public ngOnInit(): void {
        if (!this.isPublicApp) {
            this.formGroup.get('shipControl')!.valueChanges.subscribe({
                next: (ship: ShipNomenclatureDTO | undefined | string) => {
                    if (ship !== null && ship !== undefined && ship instanceof NomenclatureDTO) {
                        this.service.getPermitNomenclatures(ship.value!, this.pageCode === PageCodeEnum.PoundnetCommFishLic).subscribe({
                            next: (values: PermitNomenclatureDTO[]) => {
                                this.permits = values;
                                this.noShipSelected = false;
                                if (this.permitId !== null && this.permitId !== undefined) {
                                    const permit: PermitNomenclatureDTO = this.permits.find(x => x.value === this.permitId)!;
                                    this.formGroup.get('permitControl')!.setValue(permit);
                                }
                            }
                        });
                    }
                    else {
                        this.permits = [];
                        this.permitId = undefined;
                        this.formGroup.get('permitControl')!.setValue(this.permitId);
                        this.noShipSelected = true;
                    }
                }
            });

            this.formGroup.get('permitControl')!.valueChanges.subscribe({
                next: (permit: PermitNomenclatureDTO | undefined | string) => {
                    if (permit === null || permit === undefined || typeof (permit) === 'string') {
                        this.permitId = undefined;
                    }
                }
            });
        }
        else {
            this.formGroup.get('permitRegistrationNumberControl')!.setValue(this.permitNumber);
        }

        if (this.shipId !== null && this.shipId !== undefined) {
            if (!this.isPublicApp) {
                this.formGroup.get('shipControl')!.setValue(ShipsUtils.get(this.ships, this.shipId));
            }
        }
        else {
            this.noShipSelected = true;
        }
    }

    public setData(data: ChoosePermitToCopyFromDialogParams, buttons: DialogWrapperData): void {
        this.service = data.service;
        this.shipId = data.shipId;
        this.permitId = data.permitId;
        this.permitNumber = data.permitNumber;
        this.pageCode = data.pageCode;
        this.ships = data.ships;
    }

    public dialogButtonClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public saveBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        if (this.formGroup.valid) {
            const result: ChoosePermitToCopyFromDialogResult = new ChoosePermitToCopyFromDialogResult();

            if (this.isPublicApp) {
                this.permitNumber = this.formGroup.get('permitRegistrationNumberControl')!.value;
                result.permitNumber = this.permitNumber;
            }
            else {
                this.permitId = this.formGroup.get('permitControl')!.value!.value;
                result.permitId = this.permitId;
            }

            dialogClose(result);
        }
    }

    public cancelBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }
}