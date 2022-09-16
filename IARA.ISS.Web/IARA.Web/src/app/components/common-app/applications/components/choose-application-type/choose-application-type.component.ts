import { Component, OnInit, ViewChild } from '@angular/core';
import { IApplicationsService } from '@app/interfaces/administration-app/applications.interface';
import { ApplicationTypeDTO } from '@app/models/generated/dtos/ApplicationTypeDTO';
import { ApplicationsPublicService } from '@app/services/public-app/applications-public.service';
import { GridRow } from '@app/shared/components/data-table/models/row.model';
import { TLDataTableComponent } from '@app/shared/components/data-table/tl-data-table.component';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';


@Component({
    selector: 'choose-application-type',
    templateUrl: './choose-application-type.component.html',
    styleUrls: ['./choose-application-type.component.scss']
})
export class ChooseApplicationTypeComponent implements OnInit, IDialogComponent {

    public applicationTypes: ApplicationTypeDTO[] = [];
    public applicationsService: IApplicationsService;
    public noApplicationTypeChosenValidation: boolean = false;

    @ViewChild('table')
    private table!: TLDataTableComponent;

    public constructor(applicationsService: ApplicationsPublicService) {
        this.applicationsService = applicationsService;
    }

    public ngOnInit(): void {
        this.applicationsService.getApplicationTypesForChoice().subscribe((result: ApplicationTypeDTO[]) => {
            setTimeout(() => {
                this.applicationTypes = result;
            });
        });
    }

    public setData(data: unknown, buttons: DialogWrapperData): void {
        // nothing to do
    }

    public dialogButtonClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public saveBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        const selectedElement: ApplicationTypeDTO | undefined = this.applicationTypes.find(x => x.isChecked);
        if (selectedElement === undefined) {
            this.noApplicationTypeChosenValidation = true;
        }
        else {
            dialogClose(selectedElement);
        }
    }

    public cancelBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public toggleExpandGroup(group: { key: number, value: ApplicationTypeDTO[] }): void {
        this.table.toggleExandGroup(group);
    }

    public getRowClass = (row: GridRow<ApplicationTypeDTO>): Record<string, boolean> => {
        return {
            'row-selected': row.data.isChecked === true,
            'row-not-selected': row.data.isChecked === false
        }
    }

    public checkedGroup(event: Event, group: { key: number, value: GridRow<ApplicationTypeDTO>[] }): void {
        const element: ApplicationTypeDTO | undefined = this.applicationTypes.find(x => x.groupId === group.key);
        if (element !== undefined) {
            element.isChecked = !element.isChecked;

            if (element.isChecked) {
                this.noApplicationTypeChosenValidation = false;
                const elementsToUpdate: ApplicationTypeDTO[] = this.applicationTypes.filter(x => x.groupId !== group.key);
                for (const el of elementsToUpdate) {
                    el.isChecked = false;
                }
            }
            else {
                this.noApplicationTypeChosenValidation = true;
            }
            this.applicationTypes = this.applicationTypes.slice();
            this.table.rows = this.table.rows.slice();
        }
    }

    public checkedRow(event: Event, row: GridRow<ApplicationTypeDTO>): void {
        const element: ApplicationTypeDTO | undefined = this.applicationTypes.find(x => x.value === row.data.value);
        if (element !== undefined) {
            element.isChecked = !element.isChecked;

            if (element.isChecked) {
                this.noApplicationTypeChosenValidation = false;
                const elementsToUpdate = this.applicationTypes.filter(x => x.value !== row.data.value);
                for (const el of elementsToUpdate) {
                    el.isChecked = false;
                }
            }
            else {
                this.noApplicationTypeChosenValidation = true;
            }
            this.applicationTypes = this.applicationTypes.slice();
        }
    }
}
