import { AquacultureInstallationNetCageDTO } from '@app/models/generated/dtos/AquacultureInstallationNetCageDTO';
import { AquacultureInstallationNetCageShapesEnum } from '@app/enums/aquaculture-installation-net-cage-shapes.enum';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';

export class EditInstallationNetCageDialogParams {
    public model: AquacultureInstallationNetCageDTO | undefined;
    public netCageTypes: NomenclatureDTO<number>[] = [];
    public netCageShapes: NomenclatureDTO<AquacultureInstallationNetCageShapesEnum>[] = [];
    public readOnly: boolean = false;

    public constructor(params?: Partial<EditInstallationNetCageDialogParams>) {
        Object.assign(this, params);
    }
}