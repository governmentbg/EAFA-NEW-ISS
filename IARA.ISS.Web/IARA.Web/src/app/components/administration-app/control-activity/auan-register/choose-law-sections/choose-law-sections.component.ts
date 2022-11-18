import { AfterViewInit, Component, OnInit } from '@angular/core';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { AuanRegisterService } from '@app/services/administration-app/auan-register.service';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { GridRow } from '@app/shared/components/data-table/models/row.model';
import { AuanLawSectionDTO } from '@app/models/generated/dtos/AuanLawSectionDTO';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { ChooseLawSectionDialogParams } from '../models/choose-law-section-dialog-params.model';
import { FormControl } from '@angular/forms';
import { CommonUtils } from '@app/shared/utils/common.utils';

@Component({
    selector: 'choose-law-sections',
    templateUrl: './choose-law-sections.component.html'
})
export class ChooseLawSectionsComponent implements OnInit, AfterViewInit, IDialogComponent {
    public allLawSections: AuanLawSectionDTO[] = [];
    public lawSections: AuanLawSectionDTO[] = [];
    public filteredLawSections: AuanLawSectionDTO[] = [];
    public laws: NomenclatureDTO<number>[] = [];

    public articleControl: FormControl;
    public textControl: FormControl;

    public noLawSectionValidation: boolean = false;

    private section: AuanLawSectionDTO | undefined;
    private sectionId: number | undefined;

    private readonly service: AuanRegisterService;

    public constructor(service: AuanRegisterService) {
        this.service = service;
        this.articleControl = new FormControl();
        this.textControl = new FormControl();
    }
    public ngOnInit(): void {
        this.service.getLaws().subscribe({
            next: (result: NomenclatureDTO<number>[]) => {
                this.laws = result;
            }
        });

        this.service.getAuanLawSections().subscribe({
            next: (result: AuanLawSectionDTO[]) => {
                setTimeout(() => {
                    this.allLawSections = result;
                    this.lawSections = [...this.allLawSections];

                    if (this.sectionId !== undefined && this.sectionId !== null) {
                        this.noLawSectionValidation = false;
                        this.lawSections.find(x => x.id === this.sectionId)!.isChecked = true;
                        this.section = this.lawSections.find(x => x.id === this.sectionId);
                    }
                });
            }
        });
    }

    public ngAfterViewInit(): void {
        this.articleControl.valueChanges.subscribe({
            next: (value: string) => {
                if (value?.length > 0) {
                    value = value.toLowerCase();

                    this.lawSections = this.lawSections.filter((section: AuanLawSectionDTO) => {
                        if (section.article?.includes(value)) {
                            return true;
                        }
                        return false;
                    });

                    if (this.textControl.value?.length === 0) {
                        this.filteredLawSections = this.lawSections;
                    }
                }
                else {
                    this.lawSections = this.allLawSections;

                    if (this.textControl.value?.length > 0) {
                        this.textControl.updateValueAndValidity();
                    }
                    else {
                        this.filteredLawSections = [];
                    }
                }
            }
        });

        this.textControl.valueChanges.subscribe({
            next: (value: string) => {
                if (value?.length > 0) {
                    value = value.toLowerCase();

                    this.lawSections = this.lawSections.filter((section: AuanLawSectionDTO) => {
                        if (section.lawText?.toLowerCase().includes(value)) {
                            return true;
                        }
                        return false;
                    });

                    if (this.articleControl.value?.length === 0) {
                        this.filteredLawSections = this.lawSections;
                    }
                }
                else {
                    this.lawSections = this.allLawSections;

                    if (this.articleControl.value?.length > 0) {
                        this.articleControl.updateValueAndValidity();
                    }
                    else {
                        this.filteredLawSections = [];
                    }
                }
            }
        });
    }

    public setData(data: ChooseLawSectionDialogParams, wrapperData: DialogWrapperData): void {
        this.sectionId = data.id;
    }

    public saveBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        if (!CommonUtils.isNullOrEmpty(this.section) && !this.noLawSectionValidation) {
            this.noLawSectionValidation = false;
            dialogClose(this.section);
        }
        else {
            this.noLawSectionValidation = true;
        }
    }

    public cancelBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public dialogButtonClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public checkedRow(row: GridRow<AuanLawSectionDTO>): void {
        const element: AuanLawSectionDTO | undefined = this.lawSections?.find(x => x.id === row.data.id);
        if (element !== null && element !== undefined) {
            element.isChecked = !element.isChecked;

            if (element.isChecked) {
                this.noLawSectionValidation = false;
                const elementsToUpdate: AuanLawSectionDTO[] = this.lawSections.filter(x => x.id !== element.id);
                this.section = element;

                for (const el of elementsToUpdate) {
                    el.isChecked = false;
                }
            }
            else {
                this.noLawSectionValidation = true;

                if (this.lawSections !== null && this.lawSections !== undefined) {
                    for (const el of this.lawSections) {
                        el.isChecked = false;
                    }
                }
                this.section = undefined
            }

            this.lawSections = this.lawSections.slice();
        }
    }

    public getRowClass = (row: GridRow<AuanLawSectionDTO>): Record<string, boolean> => {
        return {
            'row-selected': row.data.isChecked === true,
            'row-not-selected': row.data.isChecked === false
        };
    }
}