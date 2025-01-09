import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, ValidatorFn, Validators } from '@angular/forms';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { ColumnDTO } from '@app/models/generated/dtos/ColumnDTO';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { NomenclaturesRegisterService } from '@app/services/administration-app/nomenclatures-register.service';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { INomenclaturesService } from '@app/interfaces/administration-app/nomenclatures-register.interface';
import { TLValidators } from '@app/shared/utils/tl-validators';
import { ExtendedColumn } from './models/extended-column.model';
import { EditNomenclatureParams } from './models/edit-nomenclature-params.model';
import { DateUtils } from '@app/shared/utils/date.utils';

@Component({
    selector: 'edit-nomenclatures-component',
    templateUrl: './edit-nomenclatures.component.html',
})
export class EditNomenclatureComponent implements IDialogComponent, OnInit {
    public form: FormGroup;
    public editCurrentControl: FormControl;
    public translate: FuseTranslationLoaderService;

    public record: Record<string, unknown>;
    public dataColumns: ExtendedColumn[] = [];
    public readOnly: boolean = false;

    public adding: boolean = true;
    public allReadOnly: boolean = false;
    public isValidityNomenclature: boolean = false;

    private id: number | undefined;
    private childNomenclatures: Record<string, NomenclatureDTO<number>[]>;

    private readonly service: INomenclaturesService;

    public constructor(service: NomenclaturesRegisterService, translate: FuseTranslationLoaderService) {
        this.service = service;
        this.translate = translate;
        this.record = {};
        this.childNomenclatures = {};

        this.form = new FormGroup({});
        this.editCurrentControl = new FormControl(true);
    }

    public ngOnInit(): void {
        this.getChildNomenclatures().subscribe({
            next: () => {
                if (this.id !== undefined && this.id !== null) {
                    this.service.get(this.id).subscribe({
                        next: (result: Record<string, unknown>) => {
                            this.record = result;

                            const properties: string[] = Object.keys(this.record);
                            for (const property of properties) {
                                const formControl: FormControl = this.form.get(property) as FormControl;
                                const column: ExtendedColumn | undefined = this.dataColumns.find(x => x.propertyName === property);

                                if (formControl && column) {
                                    if (column.isForeignKey) {
                                        formControl.setValue(column.options?.find(x => x.value === this.record[property]));
                                    }
                                    else {
                                        formControl.setValue(this.record[property]);
                                    }
                                }
                            }
                        }
                    });
                }
            }
        });
    }

    public setData(data: EditNomenclatureParams, buttons: DialogWrapperData): void {
        this.id = data.id;
        this.readOnly = data.viewMode;
        this.adding = this.id === undefined || this.id === null;

        this.setDataColumns(data.columns);

        const columns: string[] = this.dataColumns.map(x => x.propertyName.toLowerCase());
        this.isValidityNomenclature = columns.includes('validfrom') && columns.includes('validto');
    }

    public saveBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        if (this.readOnly) {
            dialogClose(this.record);
        }

        this.form.markAllAsTouched();
        this.form.updateValueAndValidity();
        if (this.form.valid) {
            for (const key of Object.keys(this.form.controls)) {
                if (this.form.controls[key].value instanceof NomenclatureDTO) {
                    this.record[key] = this.form.controls[key].value?.value;
                }
                else {
                    this.record[key] = this.form.controls[key].value;
                }
            }

            if (this.adding) {
                this.service.add(this.record).subscribe({
                    next: (id: number) => {
                        this.record.id = id;
                        dialogClose(this.record);
                    }
                });
            }
            else {
                this.service.edit(this.record, this.editCurrentControl.value).subscribe({
                    next: () => {
                        dialogClose(this.record);
                    }
                });
            }
        }
    }

    public cancelBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public dialogButtonClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    private setDataColumns(columns: ColumnDTO[]): void {
        this.dataColumns = [];

        for (const column of columns) {
            const validators: ValidatorFn[] = [];
            if (column.isRequired && column.dataType !== 'boolean') {
                validators.push(Validators.required);
            }
            if (column.maxLength !== null && column.maxLength !== undefined && column.maxLength > 0) {
                validators.push(Validators.maxLength(column.maxLength));
            }
            if (column.dataType === 'number') {
                validators.push(TLValidators.number());
            }

            if (this.isValidFromColumn(column.propertyName)) {
                this.form.addControl(column.propertyName, new FormControl(new Date(), validators));
                this.form.controls[column.propertyName].disable();
            }
            else if (this.isValidToColumn(column.propertyName)) {
                this.form.addControl(column.propertyName, new FormControl(DateUtils.MAX_DATE, validators));
                this.form.controls[column.propertyName].disable();
            }
            else {
                this.form.addControl(column.propertyName, new FormControl(undefined, validators));
            }

            if (!this.adding) {
                if (column.isReadOnly) {
                    this.form.get(column.propertyName)!.disable();
                }
                else if (this.isValidityColumn(column.propertyName) || this.isActiveColumn(column.propertyName)) {
                    column.isReadOnly = true;
                    this.form.get(column.propertyName)!.disable();
                }
            }

            const extColumn: ExtendedColumn = new ExtendedColumn(column);
            this.dataColumns.push(extColumn);
        }

        this.allReadOnly = !this.dataColumns.some(x => !x.isReadOnly);

        if (this.readOnly) {
            this.form.disable();
        }
    }

    private getChildNomenclatures(): Observable<Record<string, NomenclatureDTO<number>[]>> {
        return this.service.getChildNomenclatures().pipe(map((result: Record<string, NomenclatureDTO<number>[]>) => {
            this.childNomenclatures = result;

            for (const column of this.dataColumns) {
                if (column.isForeignKey) {
                    column.options = this.getForeignKeyNomenclature(column);
                }
            }

            return this.childNomenclatures;
        }));
    }

    private getForeignKeyNomenclature(column: ColumnDTO): NomenclatureDTO<number>[] {
        const nomenclature: NomenclatureDTO<number>[] = this.childNomenclatures[column.propertyName];
        return nomenclature?.map(x => new NomenclatureDTO<number>(x)) ?? [];
    }

    private isValidFromColumn(propertyName: string): boolean {
        return ['validFrom', 'ValidFrom'].includes(propertyName);
    }

    private isValidToColumn(propertyName: string): boolean {
        return ['validTo', 'ValidTo'].includes(propertyName);
    }

    private isValidityColumn(propertyName: string): boolean {
        return this.isValidFromColumn(propertyName) || this.isValidToColumn(propertyName);
    }

    private isActiveColumn(propertyName: string): boolean {
        return ['isActive', 'IsActive'].includes(propertyName);
    }
}