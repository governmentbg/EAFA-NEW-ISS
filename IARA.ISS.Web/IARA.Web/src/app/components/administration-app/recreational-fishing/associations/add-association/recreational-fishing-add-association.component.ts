import { AfterViewInit, Component, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { RecreationalFishingPossibleAssociationLegalDTO } from '@app/models/generated/dtos/RecreationalFishingPossibleAssociationLegalDTO';
import { RecreationalFishingAssociationService } from '@app/services/administration-app/recreational-fishing-association.service';
import { GridRow } from '@app/shared/components/data-table/models/row.model';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { IRecreationalFishingAssociationService } from '@app/interfaces/common-app/recreational-fishing-association.interface';

@Component({
    selector: 'recreational-fishing-add-association',
    templateUrl: './recreational-fishing-add-association.component.html',
    styleUrls: ['./recreational-fishing-add-association.component.scss']
})
export class RecreationalFishingAddAssociationComponent implements OnInit, AfterViewInit, IDialogComponent {
    public filterControl: FormControl;
    public showNoPermissionsControl: FormControl;

    public associations: RecreationalFishingPossibleAssociationLegalDTO[] = [];
    public allAssociations: RecreationalFishingPossibleAssociationLegalDTO[] = [];

    private service: IRecreationalFishingAssociationService;

    public constructor(service: RecreationalFishingAssociationService) {
        this.service = service;

        this.filterControl = new FormControl();
        this.showNoPermissionsControl = new FormControl();
    }

    public ngOnInit(): void {
        this.service.getPossibleAssociationLegals().subscribe({
            next: (legals: RecreationalFishingPossibleAssociationLegalDTO[]) => {
                setTimeout(() => {
                    this.allAssociations = legals;

                    this.associations = this.allAssociations.filter(x => x.hasPermissions === true);
                });
            }
        });
    }

    public ngAfterViewInit(): void {
        this.filterControl.valueChanges.subscribe({
            next: (value: string) => {
                this.deselectAll();

                if (value?.length > 0) {
                    value = value.toLowerCase();

                    this.associations = this.allAssociations.filter(x => x.hasPermissions === !this.showNoPermissionsControl.value);

                    this.associations = this.associations.filter((association: RecreationalFishingPossibleAssociationLegalDTO) => {
                        if (association.eik!.toLowerCase().includes(value)) {
                            return true;
                        }
                        if (association.name!.toLowerCase().includes(value)) {
                            return true;
                        }
                        return false;
                    });
                }
            }
        });

        this.showNoPermissionsControl.valueChanges.subscribe({
            next: (checked: boolean) => {
                this.associations = this.allAssociations.filter(x => x.hasPermissions === !checked);
            }
        });
    }

    public setData(data: unknown, buttons: DialogWrapperData): void {
        // nothing to do
    }

    public saveBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        const selected: RecreationalFishingPossibleAssociationLegalDTO | undefined = this.getSelectedAssociation();

        if (selected !== undefined) {
            dialogClose(selected);
        }
    }

    public cancelBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public dialogButtonClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public getRowClass(row: GridRow<RecreationalFishingPossibleAssociationLegalDTO>): Record<string, boolean> {
        return {
            'row-selected': row.data.isChecked === true,
            'row-not-selected': row.data.isChecked === false
        };
    }

    public checkRow(row: RecreationalFishingPossibleAssociationLegalDTO): void {
        for (const association of this.associations) {
            association.isChecked = association.id === row.id && !association.isChecked;
        }
    }

    public deselectAll(): void {
        for (const association of this.associations) {
            association.isChecked = false;
        }
    }

    public getSelectedAssociation(): RecreationalFishingPossibleAssociationLegalDTO | undefined {
        return this.associations.find(x => x.isChecked === true);
    }
}