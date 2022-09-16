

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { NomenclatureDTO } from './GenericNomenclatureDTO';

export class PermissionGroupDTO { 
    public constructor(obj?: Partial<PermissionGroupDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(String)
    public name?: string;

    @StrictlyTyped(String)
    public parentGroup?: string;

    @StrictlyTyped(NomenclatureDTO)
    public readAllPermission?: NomenclatureDTO<number>;

    @StrictlyTyped(NomenclatureDTO)
    public readPermission?: NomenclatureDTO<number>;

    @StrictlyTyped(NomenclatureDTO)
    public addPermission?: NomenclatureDTO<number>;

    @StrictlyTyped(NomenclatureDTO)
    public editPermission?: NomenclatureDTO<number>;

    @StrictlyTyped(NomenclatureDTO)
    public deletePermission?: NomenclatureDTO<number>;

    @StrictlyTyped(NomenclatureDTO)
    public restorePermission?: NomenclatureDTO<number>;

    @StrictlyTyped(NomenclatureDTO)
    public otherPermissions?: NomenclatureDTO<number>[];
}