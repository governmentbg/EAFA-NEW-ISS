import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { Component, Input, OnChanges, SimpleChange, SimpleChanges } from '@angular/core';

import { HeaderCloseFunction } from '../dialog-wrapper/interfaces/header-cancel-button.interface';
import { TLMatDialog } from '../dialog-wrapper/tl-mat-dialog';
import { EditTranslationComponent } from '@app/components/administration-app/translation-management/edit-translation.component';
import { EditTranslationDialogParams } from '@app/components/administration-app/translation-management/models/edit-translation-dialog-params.model';

@Component({
    selector: 'tl-help',
    templateUrl: './tl-help.component.html',
})
export class TLHelpComponent implements OnChanges {
    @Input()
    public tooltipResource!: string;

    public alwaysShow: boolean = false;
    public text: string = '';

    public static alwaysShowHelpers: boolean = false;

    private static helpers: TLHelpComponent[] = [];

    private editDialog: TLMatDialog<EditTranslationComponent>;
    private translationService: FuseTranslationLoaderService;

    public constructor(
        translationService: FuseTranslationLoaderService,
        editDialog: TLMatDialog<EditTranslationComponent>
    ) {
        this.editDialog = editDialog;
        this.translationService = translationService;

        this.alwaysShow = TLHelpComponent.alwaysShowHelpers;

        TLHelpComponent.helpers.push(this);
    }

    public ngOnChanges(changes: SimpleChanges): void {
        const resource: SimpleChange = changes['tooltipResource'];

        if (resource !== null && resource !== undefined) {
            const value: string = resource.currentValue;

            if (value !== undefined && value !== null) {
                this.text = this.translationService.getValue(value) ?? '';

                if (this.text === this.tooltipResource) {
                    this.text = '';
                }
            }
        }
    }

    public onTogglePopover(): void {
        if (TLHelpComponent.alwaysShowHelpers) {
            if (this.tooltipResource !== null && this.tooltipResource !== undefined && this.tooltipResource !== '') {
                this.openEditDialogFor(this.tooltipResource);
            }
        }
    }

    public static toggleShowAllHelpers(): void {
        TLHelpComponent.alwaysShowHelpers = !TLHelpComponent.alwaysShowHelpers;

        for (const helper of TLHelpComponent.helpers) {
            helper.alwaysShow = TLHelpComponent.alwaysShowHelpers;
        }
    }

    private openEditDialogFor(key: string): void {
        this.editDialog.openWithTwoButtons({
            title: this.translationService.getValue('translation-management.edit-dialog'),
            TCtor: EditTranslationComponent,
            headerCancelButton: {
                cancelBtnClicked: this.closeDialogBtnClicked.bind(this)
            },
            componentData: new EditTranslationDialogParams({ key: key }),
            translteService: this.translationService
        }, '50em');
    }

    private closeDialogBtnClicked(closeFn: HeaderCloseFunction): void {
        closeFn();
    }
}