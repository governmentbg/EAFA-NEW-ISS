import { Observable } from "rxjs";
import { NomenclatureDTO } from "@app/models/generated/dtos/GenericNomenclatureDTO";

export interface IRolesService {
    getInternalActiveRoles(): Observable<NomenclatureDTO<number>[]>;

    getPublicActiveRoles(): Observable<NomenclatureDTO<number>[]>;
}