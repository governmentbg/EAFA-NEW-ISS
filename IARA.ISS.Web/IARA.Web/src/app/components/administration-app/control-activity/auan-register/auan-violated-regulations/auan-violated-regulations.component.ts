import { Component, Input, OnInit, Self, ViewChild } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, NgControl, Validators } from '@angular/forms';
import { CustomFormControl } from '@app/shared/utils/custom-form-control';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { TLDataTableComponent } from '@app/shared/components/data-table/tl-data-table.component';
import { GridRow } from '@app/shared/components/data-table/models/row.model';
import { RecordChangedEventArgs } from '@app/shared/components/data-table/models/record-changed-event.model';
import { AuanViolatedRegulationDTO } from '@app/models/generated/dtos/AuanViolatedRegulationDTO';
import { TLMatDialog } from '@app/shared/components/dialog-wrapper/tl-mat-dialog';
import { IHeaderAuditButton } from '@app/shared/components/dialog-wrapper/interfaces/header-audit-button.interface';
import { HeaderCloseFunction } from '@app/shared/components/dialog-wrapper/interfaces/header-cancel-button.interface';
import { AuanLawSectionDTO } from '@app/models/generated/dtos/AuanLawSectionDTO';
import { ChooseLawSectionsComponent } from '../choose-law-sections/choose-law-sections.component';
import { ChooseLawSectionDialogParams } from '../models/choose-law-section-dialog-params.model';

@Component({
    selector: 'auan-violated-regulations',
    templateUrl: './auan-violated-regulations.component.html'
})
export class AuanViolatedRegulationsComponent extends CustomFormControl<AuanViolatedRegulationDTO[]> implements OnInit {
    @Input()
    public viewMode!: boolean;

    @Input()
    public showInactiveRecords: boolean = true;

    public violatedRegulationsForm!: FormGroup;
    public violatedRegulations: AuanViolatedRegulationDTO[] = [];
    public translate: FuseTranslationLoaderService;

    public isDisabled: boolean = false;

    @ViewChild('violatedRegulationsTable')
    private violatedRegulationsTable!: TLDataTableComponent;

    private chooseLawSectionDialog: TLMatDialog<ChooseLawSectionsComponent>;

    public constructor(
        @Self() ngControl: NgControl,
        translate: FuseTranslationLoaderService,
        chooseLawSectionDialog: TLMatDialog<ChooseLawSectionsComponent>
    ) {
        super(ngControl);

        this.translate = translate;
        this.chooseLawSectionDialog = chooseLawSectionDialog;
    }

    public async ngOnInit(): Promise<void> {
        this.initCustomFormControl();
    }

    public writeValue(value: AuanViolatedRegulationDTO[]): void {
        if (value !== null && value !== undefined) {
            setTimeout(() => {
                this.violatedRegulations = value;
            });
        }
        else {
            setTimeout(() => {
                this.violatedRegulations = [];
            });
        }
    }

    public onUndoAddEditRow(row: GridRow<AuanViolatedRegulationDTO>): void {
        this.violatedRegulationsTable.undoAddEditRow(row);
    }

    public setDisabledState(isDisabled: boolean): void {
        this.isDisabled = isDisabled;
        if (this.isDisabled) {
            this.violatedRegulationsForm.disable();
        }
        else {
            this.violatedRegulationsForm.enable();
        }
    }

    public violatedRegulationsRecordChanged(event: RecordChangedEventArgs<AuanViolatedRegulationDTO>): void {
        this.violatedRegulations = this.violatedRegulationsTable.rows;

        this.onChanged(this.getValue());
        this.control.updateValueAndValidity();
    }

    public onEditRecord(row: GridRow<AuanViolatedRegulationDTO>): void {
        if (row !== undefined && row !== null) {
            this.violatedRegulationsForm.get('lawTextControl')!.setValue(row.data.lawText);
        }
        else {
            this.violatedRegulationsForm.get('lawTextControl')!.reset();
        }
    }

    public openLawSectionsDialog(row: GridRow<AuanViolatedRegulationDTO>): void {
        let auditButton: IHeaderAuditButton | undefined;
        const title: string = this.translate.getValue('auan-register.choose-law-section-dialog-title');
        const data: ChooseLawSectionDialogParams = new ChooseLawSectionDialogParams({
            id: row.data?.lawSectionId
        });

        const dialog = this.chooseLawSectionDialog.openWithTwoButtons({
            title: title,
            TCtor: ChooseLawSectionsComponent,
            headerAuditButton: auditButton,
            headerCancelButton: {
                cancelBtnClicked: this.closeDialogBtnClicked.bind(this)
            },
            componentData: data,
            translteService: this.translate,
            disableDialogClose: true,
            saveBtn: {
                id: 'save',
                color: 'accent',
                translateValue: this.translate.getValue('auan-register.choose')
            },
            cancelBtn: {
                id: 'cancel',
                color: 'primary',
                translateValue: this.translate.getValue('common.cancel'),
            }
        }, '1400px');

        dialog.subscribe((entry: AuanLawSectionDTO) => {
            if (entry !== undefined && entry !== null) {
                this.violatedRegulationsForm.get('articleControl')!.setValue(entry.article);
                this.violatedRegulationsForm.get('letterControl')!.setValue(entry.letter);
                this.violatedRegulationsForm.get('paragraphControl')!.setValue(entry.paragraph);
                this.violatedRegulationsForm.get('sectionControl')!.setValue(entry.section);

                row.data.lawSectionId = entry.id;
                row.data.lawText = entry.lawText;
                this.violatedRegulationsForm.get('lawTextControl')!.setValue(row.data.lawText);
            }
        });
    }

    protected getValue(): AuanViolatedRegulationDTO[] {
        return this.violatedRegulations.map(x => new AuanViolatedRegulationDTO({
            id: x.id,
            article: x.article,
            paragraph: x.paragraph,
            section: x.section,
            letter: x.letter,
            lawSectionId: x.lawSectionId ?? undefined,
            lawText: x.lawText ?? undefined,
            comments: x.comments,
            isActive: x.isActive ?? true
        }));;
    }

    protected buildForm(): AbstractControl {
        this.violatedRegulationsForm = new FormGroup({
            articleControl: new FormControl(null, [Validators.required, Validators.maxLength(10)]),
            paragraphControl: new FormControl(null, Validators.maxLength(10)),
            sectionControl: new FormControl(null, Validators.maxLength(10)),
            letterControl: new FormControl(null, Validators.maxLength(10)),
            commentsControl: new FormControl(null, Validators.maxLength(4000)),
            lawTextControl: new FormControl(null)
        });

        return new FormControl(null);
    }

    private closeDialogBtnClicked(closeFn: HeaderCloseFunction): void {
        closeFn();
    }
}