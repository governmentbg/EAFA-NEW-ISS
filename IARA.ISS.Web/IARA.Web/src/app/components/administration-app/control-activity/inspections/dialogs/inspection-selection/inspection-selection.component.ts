import { Component, OnInit } from '@angular/core';
import { MatDialogRef } from '@angular/material/dialog';

import { InspectionTypesEnum } from '@app/enums/inspection-types.enum';
import { DialogWrapperComponent } from '@app/shared/components/dialog-wrapper/dialog-wrapper.component';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';

class InspectionTypeExtended {
    public icon!: string;
    public iconSize!: string;
    public displayCode!: string;
    public type!: InspectionTypesEnum;
}

@Component({
    selector: 'inspection-selection',
    templateUrl: './inspection-selection.component.html',
})
export class InspectionSelectionComponent implements OnInit, IDialogComponent {
    public inspectionTypes: InspectionTypeExtended[] = [];

    private dialogRef!: MatDialogRef<DialogWrapperComponent<IDialogComponent>>;

    public ngOnInit(): void {
        this.inspectionTypes = [
            {
                icon: 'fa-binoculars',
                iconSize: '2.1',
                displayCode: 'ofs',
                type: InspectionTypesEnum.OFS
            },
            {
                icon: 'fa-ship',
                iconSize: '2.1',
                displayCode: 'ibs',
                type: InspectionTypesEnum.IBS
            },
            {
                icon: 'fa-anchor',
                iconSize: '2.1',
                displayCode: 'ibp',
                type: InspectionTypesEnum.IBP
            },
            {
                icon: 'fa-exchange-alt',
                iconSize: '2.1',
                displayCode: 'itb',
                type: InspectionTypesEnum.ITB
            },
            {
                icon: 'fa-shipping-fast',
                iconSize: '2.1',
                displayCode: 'ivh',
                type: InspectionTypesEnum.IVH
            },
            {
                icon: 'fa-store',
                iconSize: '2.1',
                displayCode: 'ifs',
                type: InspectionTypesEnum.IFS
            },
            {
                icon: 'ic-fishbowl',
                iconSize: '33',
                displayCode: 'iaq',
                type: InspectionTypesEnum.IAQ
            },
            {
                icon: 'fa-vest',
                iconSize: '2.1',
                displayCode: 'ifp',
                type: InspectionTypesEnum.IFP
            },
            {
                icon: 'fa-water',
                iconSize: '2.1',
                displayCode: 'cwo-report',
                type: InspectionTypesEnum.CWO
            },
            {
                icon: 'ic-hook',
                iconSize: '33',
                displayCode: 'igm',
                type: InspectionTypesEnum.IGM
            }
        ];
    }

    public inspectionSelect(inspectionType: InspectionTypesEnum): void {
        this.dialogRef.close(inspectionType);
    }

    public setData(data: unknown, wrapperData: DialogWrapperData): void {
        this.dialogRef = wrapperData.dialogRef!;
    }

    public saveBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public cancelBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public dialogButtonClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }
}