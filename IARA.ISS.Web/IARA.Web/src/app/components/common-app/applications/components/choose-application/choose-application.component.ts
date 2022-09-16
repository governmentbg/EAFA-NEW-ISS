import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { FormControl } from '@angular/forms';
import { SelectionType } from '@swimlane/ngx-datatable';
import { Observable } from 'rxjs';
import { PageCodeEnum } from '@app/enums/page-code.enum';
import { ApplicationForChoiceDTO } from '@app/models/generated/dtos/ApplicationForChoiceDTO';
import { ApplicationsAdministrationService } from '@app/services/administration-app/applications-administration.service';
import { GridRow } from '@app/shared/components/data-table/models/row.model';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { DateUtils } from '@app/shared/utils/date.utils';
import { ChooseApplicationDialogParams } from './models/choose-application-dialog-params.model';


@Component({
    selector: 'choose-application',
    templateUrl: './choose-application.component.html'
})
export class ChooseApplicationComponent implements OnInit, AfterViewInit, IDialogComponent {
    public allApplications: ApplicationForChoiceDTO[] = [];
    public applications: ApplicationForChoiceDTO[] | undefined = [];
    public SelectionType = SelectionType;
    public applicationsFilterControl: FormControl;
    public selectedApplications: GridRow<ApplicationForChoiceDTO>[] = [];
    public noApplicationChosenValidation: boolean = false;

    private pageCodes: PageCodeEnum[] = [];
    private service: ApplicationsAdministrationService;

    public constructor(service: ApplicationsAdministrationService) {
        this.service = service;
        this.applicationsFilterControl = new FormControl();
    }

    public ngOnInit(): void {
        this.service.getApplicationsForChoice(this.pageCodes).subscribe((results: ApplicationForChoiceDTO[]) => {
            setTimeout(() => {
                this.allApplications = results;
                this.applications = [...this.allApplications];
            });
        });
    }

    public ngAfterViewInit(): void {
        this.applicationsFilterControl.valueChanges.subscribe({
            next: (value: string) => {
                if (value?.length > 0) {
                    value = value.toLowerCase();

                    this.applications = this.allApplications.filter((application: ApplicationForChoiceDTO) => {
                        if (application.eventisNumber?.toLowerCase().includes(value)) {
                            return true;
                        }
                        if (application.submittedBy?.toLowerCase().includes(value)) {
                            return true;
                        }
                        if (application.submittedFor?.toLowerCase().includes(value)) {
                            return true;
                        }
                        if (CommonUtils.isNullOrUndefined(application.submitDateTime)
                            && DateUtils.ToDisplayDateString(application.submitDateTime!).includes(value)) {
                            return true;
                        }
                        if (application.type?.toLowerCase().includes(value)) {
                            return true;
                        }
                        return false;
                    });
                }
                else {
                    this.applications = this.allApplications;
                }

                if (this.applications.find(x => x.id === this.selectedApplications[0]?.data.id) === undefined) {
                    this.selectedApplications = [];
                }
                else { // doesn't update the UI so the selected row is not present TODO
                    const selectedApplicationRef = this.selectedApplications[0];
                    this.selectedApplications = [];
                    const applicationToSelect = this.applications.find(x => x.id === selectedApplicationRef?.data.id);
                    if (applicationToSelect !== undefined) {
                        this.selectedApplications = [new GridRow(applicationToSelect, false, false)];
                        this.selectedApplications = this.selectedApplications.slice();
                    }
                }
            }
        });
    }

    public setData(data: ChooseApplicationDialogParams, buttons: DialogWrapperData): void {
        this.pageCodes = data.pageCodes;
    }

    public dialogButtonClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public checkedRow(row: GridRow<ApplicationForChoiceDTO>): void {
        const element: ApplicationForChoiceDTO | undefined = this.applications?.find(x => x.id === row.data.id);
        if (element !== undefined) {
            element.isChecked = !element.isChecked;

            if (element.isChecked) {
                this.noApplicationChosenValidation = false;
                const elementsToUpdate: ApplicationForChoiceDTO[] = this.applications!.filter(x => x.id !== row.data.id);
                for (const el of elementsToUpdate) {
                    el.isChecked = false;
                }
                this.selectedApplications = [row];
            }
            else {
                this.noApplicationChosenValidation = true;
                if (this.applications !== null && this.applications !== undefined) {
                    for (const el of this.applications) {
                        el.isChecked = false;
                    }
                }
                this.selectedApplications = [];
            }

            this.applications = this.applications!.slice();
        }
    }

    public getRowClass = (row: GridRow<ApplicationForChoiceDTO>): Record<string, boolean> => {
        return {
            'row-selected': row.data.isChecked === true,
            'row-not-selected': row.data.isChecked === false
        };
    }

    public saveBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        if (!CommonUtils.isNullOrEmpty(this.selectedApplications) && !this.noApplicationChosenValidation) {
            this.noApplicationChosenValidation = false;
            dialogClose({ selectedApplication: (this.selectedApplications as GridRow<ApplicationForChoiceDTO>[])[0]?.data });
        }
        else {
            this.noApplicationChosenValidation = true;
        }
    }

    public cancelBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }
}