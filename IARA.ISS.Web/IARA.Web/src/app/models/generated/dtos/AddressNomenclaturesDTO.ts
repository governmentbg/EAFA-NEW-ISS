
import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { NomenclatureDTO } from './GenericNomenclatureDTO';
import { MunicipalityNomenclatureExtendedDTO } from './MunicipalityNomenclatureExtendedDTO';
import { PopulatedAreaNomenclatureExtendedDTO } from './PopulatedAreaNomenclatureExtendedDTO';

export class AddressNomenclaturesDTO {
    public constructor(obj?: Partial<AddressNomenclaturesDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(NomenclatureDTO)
    public countries?: NomenclatureDTO<number>[];

    @StrictlyTyped(NomenclatureDTO)
    public districts?: NomenclatureDTO<number>[];

    @StrictlyTyped(MunicipalityNomenclatureExtendedDTO)
    public municipalities?: MunicipalityNomenclatureExtendedDTO[];

    @StrictlyTyped(PopulatedAreaNomenclatureExtendedDTO)
    public populatedAreas?: PopulatedAreaNomenclatureExtendedDTO[];
}