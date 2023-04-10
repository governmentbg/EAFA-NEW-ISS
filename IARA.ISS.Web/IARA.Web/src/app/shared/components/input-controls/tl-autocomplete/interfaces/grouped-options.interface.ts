import { NomenclatureDTO } from "@app/models/generated/dtos/GenericNomenclatureDTO";

export interface IGroupedOptions<T> {
    name: string;
    code?: string | undefined;
    options: NomenclatureDTO<T>[] | string[];
}