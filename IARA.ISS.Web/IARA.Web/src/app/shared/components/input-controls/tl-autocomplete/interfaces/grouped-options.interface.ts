import { NomenclatureDTO } from "@app/models/generated/dtos/GenericNomenclatureDTO";

export interface IGroupedOptions<T> {
    name: string;
    options: NomenclatureDTO<T>[] | string[];
}