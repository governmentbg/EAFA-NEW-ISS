import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormControl, Validators } from '@angular/forms';
import { Subscription } from 'rxjs';

import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { ICatchesAndSalesService } from '@app/interfaces/common-app/catches-and-sales.interface';
import { ShipLogBookPageEditDTO } from '@app/models/generated/dtos/ShipLogBookPageEditDTO';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { AddRelatedDeclarationDialogParams } from '../../models/add-related-declaration-dialog-params.model';

@Component({
    selector: 'add-related-declaration',
    templateUrl: './add-related-declaration.component.html'
})
export class AddRelatedDeclarationComponent implements OnInit, OnDestroy, IDialogComponent {
    public readonly control: FormControl;

    public previousPages: NomenclatureDTO<number>[] = [];

    private service!: ICatchesAndSalesService;
    private page!: ShipLogBookPageEditDTO;

    private readonly subs: Subscription[] = [];

    public constructor() {
        this.control = new FormControl(undefined, [Validators.required]);
    }

    public ngOnInit(): void {
        this.subs.push(this.service.getPreviousRelatedLogBookPages(this.page.id!).subscribe({
            next: (pages: NomenclatureDTO<number>[]) => {
                this.previousPages = pages ?? [];
            }
        }));
    }

    public ngOnDestroy(): void {
        for (const sub of this.subs) {
            sub.unsubscribe();
        }
    }

    public setData(data: AddRelatedDeclarationDialogParams, wrapperData: DialogWrapperData): void {
        this.service = data.service;
        this.page = data.shipLogBookPage;
    }

    public saveBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        this.control.markAsTouched();

        if (this.control.valid) {
            const selectedPage: NomenclatureDTO<number> = this.control.value;

            this.subs.push(this.service.addRelatedDeclaration(this.page.id!, selectedPage.value!).subscribe({
                next: () => {
                    dialogClose(true);
                }
            }));
        }
    }

    public cancelBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public dialogButtonClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }
}
