import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, ValidatorFn, Validators } from '@angular/forms';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { DialogParamsModel } from '@app/models/common/dialog-params.model';
import { ColumnDTO } from '@app/models/generated/dtos/ColumnDTO';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { NomenclaturesRegisterService } from '@app/services/administration-app/nomenclatures-register.service';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { INomenclaturesService } from '@app/interfaces/administration-app/nomenclatures-register.interface';
import { TLValidators } from '@app/shared/utils/tl-validators';

class ExtendedColumn extends ColumnDTO {
    public constructor(obj?: Partial<ColumnDTO>) {
        super(obj);
    }

    public options?: NomenclatureDTO<number>[];
}

@Component({
    selector: 'edit-nomenclatures-component',
    templateUrl: './edit-nomenclatures.component.html',
})
export class EditNomenclatureComponent implements IDialogComponent, OnInit {
    public form: FormGroup;
    public translate: FuseTranslationLoaderService;

    public record: Record<string, unknown>;
    public dataColumns: ExtendedColumn[] = [];
    public readOnly: boolean = false;

    private id: number | undefined;
    private adding: boolean = true;
    private childNomenclatures: Record<string, NomenclatureDTO<number>[]>;

    public service: INomenclaturesService;

    public constructor(service: NomenclaturesRegisterService, translate: FuseTranslationLoaderService) {
        this.service = service;
        this.translate = translate;
        this.record = {};
        this.childNomenclatures = {};
        this.form = new FormGroup({});
    }

    public async ngOnInit(): Promise<void> {
        await this.getColumns().toPromise();
        await this.getChildNomenclatures().toPromise();

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

    public setData(data: DialogParamsModel | undefined, buttons: DialogWrapperData): void {
        if (data !== undefined && data !== null) {
            this.adding = false;
            this.id = data.id;
            this.readOnly = data.isReadonly;
        }
    }

    public saveBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        if (this.readOnly) {
            dialogClose(this.record);
        }

        this.form.markAllAsTouched();
        this.form.updateValueAndValidity();
        if (this.form.valid) {
            for (const key of Object.keys(this.form.controls)) {
                this.record[key] = this.form.controls[key].value;
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
                this.service.edit(this.record).subscribe({
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

    private getColumns(): Observable<ColumnDTO[]> {
        return this.service.getColumns().pipe(map((columns: ColumnDTO[]) => {
            this.dataColumns = [];

            for (const column of columns) {
                const extColumn: ExtendedColumn = new ExtendedColumn(column);
                this.dataColumns.push(extColumn);

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

                this.form.addControl(column.propertyName, new FormControl(undefined, validators));
            }

            if (this.readOnly) {
                this.form.disable();
            }

            return columns;
        }));
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
}