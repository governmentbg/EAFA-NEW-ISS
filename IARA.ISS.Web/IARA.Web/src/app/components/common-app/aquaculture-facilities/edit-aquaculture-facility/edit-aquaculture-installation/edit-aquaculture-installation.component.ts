import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { forkJoin } from 'rxjs';

import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { AquacultureInstallationTypeEnum } from '@app/enums/aquaculture-installation-type.enum';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { IAquacultureFacilitiesService } from '@app/interfaces/common-app/aquaculture-facilities.interface';
import { AquacultureInstallationEditDTO } from '@app/models/generated/dtos/AquacultureInstallationEditDTO';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { EditAquacultureInstallationDialogParams } from '../models/edit-aquaculture-installation-dialog-params.model';
import { AquacultureInstallationBasinDTO } from '@app/models/generated/dtos/AquacultureInstallationBasinDTO';
import { TLDataTableComponent } from '@app/shared/components/data-table/tl-data-table.component';
import { TLValidators } from '@app/shared/utils/tl-validators';
import { AquacultureInstallationNetCageDTO } from '@app/models/generated/dtos/AquacultureInstallationNetCageDTO';
import { AquacultureInstallationNetCageShapesEnum } from '@app/enums/aquaculture-installation-net-cage-shapes.enum';
import { EditInstallationNetCageDialogParams } from '../models/edit-installation-net-cage-dialog-params.model';
import { HeaderCloseFunction } from '@app/shared/components/dialog-wrapper/interfaces/header-cancel-button.interface';
import { GridRow } from '@app/shared/components/data-table/models/row.model';
import { TLConfirmDialog } from '@app/shared/components/confirmation-dialog/tl-confirm-dialog';
import { TLMatDialog } from '@app/shared/components/dialog-wrapper/tl-mat-dialog';
import { AquacultureInstallationCollectorDTO } from '@app/models/generated/dtos/AquacultureInstallationCollectorDTO';
import { AquacultureInstallationRaftDTO } from '@app/models/generated/dtos/AquacultureInstallationRaftDTO';
import { AquacultureInstallationRecirculatorySystemDTO } from '@app/models/generated/dtos/AquacultureInstallationRecirculatorySystemDTO';
import { AquacultureInstallationDamDTO } from '@app/models/generated/dtos/AquacultureInstallationDamDTO';
import { AquacultureInstallationAquariumDTO } from '@app/models/generated/dtos/AquacultureInstallationAquariumDTO';
import { EditInstallationNetCageComponent } from '../edit-installation-net-cage/edit-installation-net-cage.component';
import { IHeaderAuditButton } from '@app/shared/components/dialog-wrapper/interfaces/header-audit-button.interface';
import { IS_PUBLIC_APP } from '@app/shared/modules/application.modules';

@Component({
    selector: 'edit-aquaculture-installation',
    templateUrl: './edit-aquaculture-installation.component.html'
})
export class EditAquacultureInstallationComponent implements OnInit, AfterViewInit, IDialogComponent {
    public form!: FormGroup;
    public basinsGroup!: FormGroup;
    public collectorsGroup!: FormGroup;
    public raftsGroup!: FormGroup;
    public recirculatoryGroup!: FormGroup;

    public installationTypes: NomenclatureDTO<number>[] = [];
    public basinPurposeTypes: NomenclatureDTO<number>[] = [];
    public basinMaterialTypes: NomenclatureDTO<number>[] = [];
    public netCageTypes: NomenclatureDTO<number>[] = [];
    public netCageShapes: NomenclatureDTO<AquacultureInstallationNetCageShapesEnum>[] = [];
    public collectorTypes: NomenclatureDTO<number>[] = [];

    public type: AquacultureInstallationTypeEnum | undefined;
    public types: typeof AquacultureInstallationTypeEnum = AquacultureInstallationTypeEnum;

    public basins: AquacultureInstallationBasinDTO[] = [];
    public netCages: AquacultureInstallationNetCageDTO[] = [];
    public collectors: AquacultureInstallationCollectorDTO[] = [];
    public rafts: AquacultureInstallationRaftDTO[] = [];
    public recirculatorySystems: AquacultureInstallationRecirculatorySystemDTO[] = [];

    public readOnly: boolean = false;

    @ViewChild('basinsDescriptionTable')
    private basinsTable!: TLDataTableComponent;

    @ViewChild('netCagesTable')
    private netCagesTable!: TLDataTableComponent;

    @ViewChild('collectorsTable')
    private collectorsTable!: TLDataTableComponent;

    @ViewChild('raftsTable')
    private raftsTable!: TLDataTableComponent;

    @ViewChild('recirculatorySystemsTable')
    private recirculatorySystemsTable!: TLDataTableComponent;

    private service!: IAquacultureFacilitiesService;
    private translate: FuseTranslationLoaderService;
    private confirmDialog: TLConfirmDialog;
    private editNetCageDialog: TLMatDialog<EditInstallationNetCageComponent>;
    private isDraft: boolean = false;
    private isAdd!: boolean;

    private model!: AquacultureInstallationEditDTO;

    public constructor(
        translate: FuseTranslationLoaderService,
        confirmDialog: TLConfirmDialog,
        editNetCageDialog: TLMatDialog<EditInstallationNetCageComponent>
    ) {
        this.translate = translate;
        this.confirmDialog = confirmDialog;
        this.editNetCageDialog = editNetCageDialog;

        this.buildForm();
    }

    public async ngOnInit(): Promise<void> {
        const nomenclatures: NomenclatureDTO<number>[][] = await forkJoin(
            NomenclatureStore.instance.getNomenclature(
                NomenclatureTypes.AquacultureInstallationTypes, this.service.getInstallationTypes.bind(this.service), false
            ),
            NomenclatureStore.instance.getNomenclature(
                NomenclatureTypes.AquacultureInstallationBasinPurposeTypes, this.service.getInstallationBasinPurposeTypes.bind(this.service), false
            ),
            NomenclatureStore.instance.getNomenclature(
                NomenclatureTypes.AquacultureInstallationBasinMaterialTypes, this.service.getInstallationBasinMaterialTypes.bind(this.service), false
            ),
            NomenclatureStore.instance.getNomenclature(
                NomenclatureTypes.AquacultureInstallationNetCageTypes, this.service.getInstallationNetCageTypes.bind(this.service), false
            ),
            NomenclatureStore.instance.getNomenclature(
                NomenclatureTypes.AquacultureInstallationCollectorTypes, this.service.getInstallationCollectorTypes.bind(this.service), false
            )
        ).toPromise();

        this.installationTypes = nomenclatures[0];
        this.basinPurposeTypes = nomenclatures[1];
        this.basinMaterialTypes = nomenclatures[2];
        this.netCageTypes = nomenclatures[3];
        this.collectorTypes = nomenclatures[4];

        this.netCageShapes = [
            new NomenclatureDTO<number>({
                value: AquacultureInstallationNetCageShapesEnum.Rectangular,
                displayName: this.translate.getValue('aquacultures.installation-net-cage-rectangular'),
                isActive: true
            }),
            new NomenclatureDTO<number>({
                value: AquacultureInstallationNetCageShapesEnum.Circular,
                displayName: this.translate.getValue('aquacultures.installation-net-cage-circular'),
                isActive: true
            })
        ];

        if (!this.isAdd) {
            this.fillForm();
        }
    }

    public ngAfterViewInit(): void {
        this.form.get('installationTypeControl')!.valueChanges.subscribe({
            next: (type: NomenclatureDTO<number> | undefined) => {
                if (type !== undefined) {
                    this.type = AquacultureInstallationTypeEnum[type.code as keyof typeof AquacultureInstallationTypeEnum];
                }
                else {
                    this.type = undefined;
                }
            }
        });
    }

    public saveBtnClicked(action: IActionInfo, dialogClose: DialogCloseCallback): void {
        if (this.readOnly) {
            dialogClose(this.model);
        }
        else if (this.isDraft) {
            this.fillModel();
            CommonUtils.sanitizeModelStrings(this.model);
            dialogClose(this.model);
        }
        else {
            this.form.markAllAsTouched();
            if (this.isFormValid()) {
                this.fillModel();
                CommonUtils.sanitizeModelStrings(this.model);
                dialogClose(this.model);
            }
        }
    }

    public cancelBtnClicked(action: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public dialogButtonClicked(action: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public setData(data: EditAquacultureInstallationDialogParams, wrapperData: DialogWrapperData): void {
        this.service = data.service as IAquacultureFacilitiesService;
        this.readOnly = data.isReadOnly;
        this.isDraft = data.isDraft;

        if (data.model === undefined) {
            this.model = new AquacultureInstallationEditDTO({ isActive: true });
            this.isAdd = true;
        }
        else {
            if (this.readOnly) {
                this.form.disable();
            }
            if (data.model instanceof AquacultureInstallationEditDTO) {
                this.model = data.model;
                this.isAdd = false;
            }
            else {
                throw new Error('Incorrect model type provided to aquaculture installation edit dialog');
            }
        }
    }

    public addEditNetCage(netCage: AquacultureInstallationNetCageDTO | undefined, viewMode: boolean = false): void {
        let data: EditInstallationNetCageDialogParams | undefined;
        let auditBtn: IHeaderAuditButton | undefined;
        let title: string;

        if (netCage !== undefined) {
            data = new EditInstallationNetCageDialogParams({
                model: netCage,
                netCageTypes: this.netCageTypes,
                netCageShapes: this.netCageShapes,
                readOnly: this.readOnly || viewMode
            });

            if (netCage.id !== undefined && !IS_PUBLIC_APP) {
                auditBtn = {
                    id: netCage.id,
                    getAuditRecordData: this.service.getInstallationNetCageAudit.bind(this.service),
                    tableName: 'RAquaSt.AquacultureInstallationNetCages'
                };
            }

            if (this.readOnly || viewMode) {
                title = this.translate.getValue('aquacultures.view-installation-net-cage-dialog-title');
            }
            else {
                title = this.translate.getValue('aquacultures.edit-installation-net-cage-dialog-title');
            }
        }
        else {
            data = new EditInstallationNetCageDialogParams({
                model: undefined,
                netCageTypes: this.netCageTypes,
                netCageShapes: this.netCageShapes,
                readOnly: false
            });

            title = this.translate.getValue('aquacultures.add-installation-net-cage-dialog-title');
        }
        const dialog = this.editNetCageDialog.openWithTwoButtons({
            title: title,
            TCtor: EditInstallationNetCageComponent,
            headerAuditButton: auditBtn,
            headerCancelButton: {
                cancelBtnClicked: this.closeEditInstallationNetCageDialogBtnClicked.bind(this)
            },
            componentData: data,
            translteService: this.translate,
            viewMode: viewMode
        }, '1000px');

        dialog.subscribe({
            next: (result: AquacultureInstallationNetCageDTO | undefined) => {
                if (result !== undefined) {
                    if (netCage !== undefined) {
                        netCage = result;
                    }
                    else {
                        this.netCages.push(result);
                    }

                    this.netCages = this.netCages.slice();
                }
            }
        });
    }

    public deleteNetCage(installation: GridRow<AquacultureInstallationNetCageDTO>): void {
        this.confirmDialog.open({
            title: this.translate.getValue('aquacultures.delete-installation-net-cage-dialog-title'),
            message: this.translate.getValue('aquacultures.delete-installation-net-cage-dialog-message'),
            okBtnLabel: this.translate.getValue('aquacultures.delete-installation-net-cage-dialog-ok-btn-label')
        }).subscribe({
            next: (ok: boolean) => {
                if (ok) {
                    this.netCagesTable.softDelete(installation);
                }
            }
        });
    }

    public undoDeleteNetCage(installation: GridRow<AquacultureInstallationNetCageDTO>): void {
        this.confirmDialog.open().subscribe({
            next: (ok: boolean) => {
                if (ok) {
                    this.netCagesTable.softUndoDelete(installation);
                }
            }
        });
    }

    public calculateRaftArea(row: AquacultureInstallationRaftDTO): string {
        if (row.length !== undefined && row.width !== undefined) {
            const l: number = Number(row.length);
            const w: number = Number(row.width);
            if (!isNaN(l) && !isNaN(w)) {
                return (l * w).toFixed(2);
            }
        }
        return '';
    }

    private closeEditInstallationNetCageDialogBtnClicked(closeFn: HeaderCloseFunction): void {
        closeFn();
    }

    private buildForm(): void {
        this.form = new FormGroup({
            installationTypeControl: new FormControl(null, Validators.required),

            basinsGroup: new FormGroup({
                commentsControl: new FormControl(null, Validators.maxLength(4000))
            }),

            netCagesGroup: new FormGroup({
                commentsControl: new FormControl(null, Validators.maxLength(4000))
            }),

            aquariumsGroup: new FormGroup({
                countControl: new FormControl(null, [Validators.required, TLValidators.number(1, undefined, 0)]),
                volumeControl: new FormControl(null, [Validators.required, TLValidators.number(1)]),
                commentsControl: new FormControl(null, Validators.maxLength(4000))
            }),

            collectorsGroup: new FormGroup({
                commentsControl: new FormControl(null, Validators.maxLength(4000))
            }),

            raftsGroup: new FormGroup({
                commentsControl: new FormControl(null, Validators.maxLength(4000))
            }),

            damsGroup: new FormGroup({
                areaControl: new FormControl(null, [Validators.required, TLValidators.number(0)]),
                commentsControl: new FormControl(null, Validators.maxLength(4000))
            }),

            recirculatorySystemsGroup: new FormGroup({
                commentsControl: new FormControl(null, Validators.maxLength(4000))
            })
        });

        this.basinsGroup = new FormGroup({
            basinPurposeTypeIdControl: new FormControl(null, Validators.required),
            basinMaterialTypeIdControl: new FormControl(null, Validators.required),
            countControl: new FormControl(null, [Validators.required, TLValidators.number(1, undefined, 0)]),
            areaControl: new FormControl(null, [Validators.required, TLValidators.number(0)]),
            volumeControl: new FormControl(null, [Validators.required, TLValidators.number(0)])
        });

        this.collectorsGroup = new FormGroup({
            collectorTypeIdControl: new FormControl(null, Validators.required),
            totalCountControl: new FormControl(null, [Validators.required, TLValidators.number(0, undefined, 0)]),
            totalAreaControl: new FormControl(null, [Validators.required, TLValidators.number(0)])
        });

        this.raftsGroup = new FormGroup({
            lengthControl: new FormControl(null, [Validators.required, TLValidators.number(0)]),
            widthControl: new FormControl(null, [Validators.required, TLValidators.number(0)]),
            areaControl: new FormControl(null),
            countControl: new FormControl(null, [Validators.required, TLValidators.number(1, undefined, 0)])
        });

        this.recirculatoryGroup = new FormGroup({
            basinPurposeTypeIdControl: new FormControl(null, Validators.required),
            basinMaterialTypeIdControl: new FormControl(null, Validators.required),
            countControl: new FormControl(null, [Validators.required, TLValidators.number(1, undefined, 0)]),
            areaControl: new FormControl(null, [Validators.required, TLValidators.number(0)]),
            volumeControl: new FormControl(null, [Validators.required, TLValidators.number(0)])
        });
    }

    private fillForm(): void {
        if (this.model.installationType !== undefined && this.model.installationType !== null) {
            this.type = this.model.installationType;
            this.form.get('installationTypeControl')!.setValue(this.installationTypes.find(x => x.code === AquacultureInstallationTypeEnum[this.model.installationType!]));

            switch (this.model.installationType) {
                case AquacultureInstallationTypeEnum.Basins:
                    this.form.get('basinsGroup')!.get('commentsControl')!.setValue(this.model.comments);
                    this.basins = this.model.basins ?? [];
                    break;
                case AquacultureInstallationTypeEnum.NetCages:
                    this.form.get('netCagesGroup')!.get('commentsControl')!.setValue(this.model.comments);
                    this.netCages = this.model.netCages ?? [];
                    break;
                case AquacultureInstallationTypeEnum.Aquariums:
                    this.form.get('aquariumsGroup')!.get('commentsControl')!.setValue(this.model.comments);
                    this.form.get('aquariumsGroup')!.get('countControl')!.setValue(this.model.aquariums?.count);
                    this.form.get('aquariumsGroup')!.get('volumeControl')!.setValue(this.model.aquariums?.volume);
                    break;
                case AquacultureInstallationTypeEnum.Collectors:
                    this.form.get('collectorsGroup')!.get('commentsControl')!.setValue(this.model.comments);
                    this.collectors = this.model.collectors ?? [];
                    break;
                case AquacultureInstallationTypeEnum.Rafts:
                    this.form.get('raftsGroup')!.get('commentsControl')!.setValue(this.model.comments);
                    this.rafts = this.model.rafts ?? [];
                    break;
                case AquacultureInstallationTypeEnum.Dams:
                    this.form.get('damsGroup')!.get('commentsControl')!.setValue(this.model.comments);
                    this.form.get('damsGroup')!.get('areaControl')!.setValue(this.model.dams?.area);
                    break;
                case AquacultureInstallationTypeEnum.RecirculatorySystems:
                    this.form.get('recirculatorySystemsGroup')!.get('commentsControl')!.setValue(this.model.comments);
                    this.recirculatorySystems = this.model.recirculatorySystems ?? [];
                    break;
                default:
                    throw new Error('Invalid aquaculture installation type');
            }
        }
    }

    private fillModel(): void {
        if (this.form.get('installationTypeControl')!.valid) {
            const installationType: NomenclatureDTO<AquacultureInstallationTypeEnum> = this.form.get('installationTypeControl')!.value!;

            this.model.installationType = AquacultureInstallationTypeEnum[installationType.code as keyof typeof AquacultureInstallationTypeEnum];
            this.model.installationTypeName = installationType.displayName;

            switch (this.type) {
                case AquacultureInstallationTypeEnum.Basins:
                    this.model.basins = this.getBasinsFromTable();

                    this.model.totalArea = this.sum(this.model.basins.map(x => x.area! * x.count!));
                    this.model.totalVolume = this.sum(this.model.basins.map(x => x.volume! * x.count!));
                    this.model.totalCount = this.sum(this.model.basins.map(x => x.count!));
                    this.model.comments = this.form.get('basinsGroup')!.get('commentsControl')!.value;
                    break;
                case AquacultureInstallationTypeEnum.NetCages:
                    this.model.netCages = this.getNetCagesFromTable();

                    this.model.totalArea = this.sum(this.model.netCages.filter(x => x.isActive).map(x => x.area! * x.count!));
                    this.model.totalVolume = this.sum(this.model.netCages.filter(x => x.isActive).map(x => x.volume! * x.count!));
                    this.model.totalCount = this.sum(this.model.netCages.filter(x => x.isActive).map(x => x.count!));
                    this.model.comments = this.form.get('netCagesGroup')!.get('commentsControl')!.value;
                    break;
                case AquacultureInstallationTypeEnum.Aquariums:
                    this.model.aquariums = new AquacultureInstallationAquariumDTO({
                        id: this.model.aquariums?.id,
                        count: this.form.get('aquariumsGroup')!.get('countControl')!.value,
                        volume: this.form.get('aquariumsGroup')!.get('volumeControl')!.value
                    });

                    this.model.totalArea = undefined;
                    this.model.totalVolume = this.model.aquariums.count! * this.model.aquariums.volume!;
                    this.model.totalCount = this.model.aquariums.count;
                    this.model.comments = this.form.get('aquariumsGroup')!.get('commentsControl')!.value;
                    break;
                case AquacultureInstallationTypeEnum.Collectors:
                    this.model.collectors = this.getCollectorsFromTable();

                    this.model.totalArea = this.sum(this.model.collectors.map(x => x.totalArea!));
                    this.model.totalVolume = undefined;
                    this.model.totalCount = this.sum(this.model.collectors.filter(x => x.isActive).map(x => x.totalCount!));
                    this.model.comments = this.form.get('collectorsGroup')!.get('commentsControl')!.value;
                    break;
                case AquacultureInstallationTypeEnum.Rafts:
                    this.model.rafts = this.getRaftsFromTable();

                    this.model.totalArea = this.sum(this.model.rafts.map(x => x.area! * x.count!));
                    this.model.totalVolume = undefined;
                    this.model.totalCount = this.sum(this.model.rafts.map(x => x.count!));
                    this.model.comments = this.form.get('raftsGroup')!.get('commentsControl')!.value;
                    break;
                case AquacultureInstallationTypeEnum.Dams:
                    this.model.dams = new AquacultureInstallationDamDTO({
                        id: this.model.dams?.id,
                        area: this.form.get('damsGroup')!.get('areaControl')!.value
                    });

                    this.model.totalArea = this.model.dams.area;
                    this.model.totalVolume = undefined;
                    this.model.totalCount = 1;
                    this.model.comments = this.form.get('damsGroup')!.get('commentsControl')!.value;
                    break;
                case AquacultureInstallationTypeEnum.RecirculatorySystems:
                    this.model.recirculatorySystems = this.getRecirculatorySystemsFromTable();

                    this.model.totalArea = this.sum(this.model.recirculatorySystems.map(x => x.area! * x.count!));
                    this.model.totalVolume = this.sum(this.model.recirculatorySystems.map(x => x.volume! * x.count!));
                    this.model.totalCount = this.sum(this.model.recirculatorySystems.map(x => x.count!));
                    this.model.comments = this.form.get('recirculatorySystemsGroup')!.get('commentsControl')!.value;
                    break;
                default:
                    throw new Error('Invalid aquaculture installation type');
            }
        }

        this.model.hasValidationErrors = !this.isFormValid() || this.model.totalCount === 0;
    }

    private getBasinsFromTable(): AquacultureInstallationBasinDTO[] {
        const rows = this.basinsTable.rows as AquacultureInstallationBasinDTO[];

        return rows.map(x => new AquacultureInstallationBasinDTO({
            id: x.id,
            basinPurposeTypeId: x.basinPurposeTypeId,
            basinMaterialTypeId: x.basinMaterialTypeId,
            count: x.count,
            area: x.area,
            volume: x.volume,
            isActive: x.isActive ?? true
        }));
    }

    private getNetCagesFromTable(): AquacultureInstallationNetCageDTO[] {
        const rows = this.netCagesTable.rows as AquacultureInstallationNetCageDTO[];
        return rows.map(x => new AquacultureInstallationNetCageDTO({
            id: x.id,
            netCageTypeId: x.netCageTypeId,
            shape: x.shape,
            count: x.count,
            radius: x.radius,
            length: x.length,
            width: x.width,
            height: x.height,
            area: x.area,
            volume: x.volume,
            isActive: x.isActive ?? true
        }));
    }

    private getCollectorsFromTable(): AquacultureInstallationCollectorDTO[] {
        const rows = this.collectorsTable.rows as AquacultureInstallationCollectorDTO[];
        return rows.map(x => new AquacultureInstallationCollectorDTO({
            id: x.id,
            collectorTypeId: x.collectorTypeId,
            totalCount: x.totalCount,
            totalArea: x.totalArea,
            isActive: x.isActive ?? true
        }));
    }

    private getRaftsFromTable(): AquacultureInstallationRaftDTO[] {
        const rows = this.raftsTable.rows as AquacultureInstallationRaftDTO[];
        return rows.map(x => new AquacultureInstallationRaftDTO({
            id: x.id,
            length: x.length,
            width: x.width,
            area: x.area,
            count: x.count,
            isActive: x.isActive ?? true
        }));
    }

    private getRecirculatorySystemsFromTable(): AquacultureInstallationRecirculatorySystemDTO[] {
        const rows = this.recirculatorySystemsTable.rows as AquacultureInstallationRecirculatorySystemDTO[];
        return rows.map(x => new AquacultureInstallationRecirculatorySystemDTO({
            id: x.id,
            basinPurposeTypeId: x.basinPurposeTypeId,
            basinMaterialTypeId: x.basinMaterialTypeId,
            count: x.count,
            area: x.area,
            volume: x.volume,
            isActive: x.isActive ?? true
        }));
    }

    private isFormValid(): boolean {
        if (this.form.get('installationTypeControl')!.valid) {
            switch (this.type) {
                case AquacultureInstallationTypeEnum.Basins:
                    return this.form.get('basinsGroup')!.valid;
                case AquacultureInstallationTypeEnum.NetCages:
                    return this.form.get('netCagesGroup')!.valid;
                case AquacultureInstallationTypeEnum.Aquariums:
                    return this.form.get('aquariumsGroup')!.valid;
                case AquacultureInstallationTypeEnum.Collectors:
                    return this.form.get('collectorsGroup')!.valid;
                case AquacultureInstallationTypeEnum.Rafts:
                    return this.form.get('raftsGroup')!.valid;
                case AquacultureInstallationTypeEnum.Dams:
                    return this.form.get('damsGroup')!.valid;
                case AquacultureInstallationTypeEnum.RecirculatorySystems:
                    return this.form.get('recirculatorySystemsGroup')!.valid;
                default:
                    return false;
            }
        }
        return false;
    }

    private sum(nums: number[]): number {
        return nums.reduce((sum: number, current: number) => { return sum + current; }, 0);
    }
}