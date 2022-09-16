import { NomenclatureDTO } from "@app/models/generated/dtos/GenericNomenclatureDTO";
import { Pipe, PipeTransform } from "@angular/core";

export type NomeclatureParameter = NomenclatureDTO<any>[] | undefined;

@Pipe({ name: 'nomenclatureDisplay' })
export class NomenclatureDisplayPipe implements PipeTransform {
    transform(nomenclature: any, ...args: NomeclatureParameter[]): string {
        if (args != undefined && args.length == 1) {
            const collection = args[0] as NomenclatureDTO<any>[];
            if (collection != undefined && nomenclature instanceof NomenclatureDTO) {
                const item = collection.find(x => x.value == nomenclature.value) as NomenclatureDTO<any>;
                return this.getNomenclatureValue(nomenclature, item);
            } else if (collection != undefined) {
                const item = collection.find(x => x.value == nomenclature) as NomenclatureDTO<any>;
                return this.getNomenclatureValue(nomenclature, item);
            }
        }

        return nomenclature.toString();
    }

    private getNomenclatureValue(value: any, item: NomenclatureDTO<any>): string {
        if (item != undefined) {
            return item.displayName != undefined ? item.displayName : '';
        } else {
            return value != undefined ? value.toString() : '';
        }
    }
}