import { InspectionSubjectAddressDTO } from '@app/models/generated/dtos/InspectionSubjectAddressDTO';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { InspectionToggleTypesEnum } from '../../enums/inspection-toggle-types.enum';
import { NomenclatureDTO } from '../../models/generated/dtos/GenericNomenclatureDTO';
import { CommonUtils } from './common.utils';

export class InspectionUtils {
    public static buildAddress(
        address: InspectionSubjectAddressDTO | undefined,
        translate: FuseTranslationLoaderService
    ): string | undefined {

        if (address === undefined || address === null) {
            return undefined;
        }

        return [
            address.populatedArea,
            address.region,
            !CommonUtils.isNullOrWhiteSpace(address.street) && !CommonUtils.isNullOrWhiteSpace(address.streetNum)
                ? (address.street + ' ' + address.streetNum)
                : address.street,
            !CommonUtils.isNullOrWhiteSpace(address.blockNum)
                ? (translate.getValue('inspections.block') + ' ' + address.blockNum)
                : null,
            !CommonUtils.isNullOrWhiteSpace(address.entranceNum)
                ? (translate.getValue('inspections.entrance') + ' ' + address.entranceNum)
                : null,
            !CommonUtils.isNullOrWhiteSpace(address.floorNum)
                ? (translate.getValue('inspections.floor') + ' ' + address.floorNum)
                : null,
            !CommonUtils.isNullOrWhiteSpace(address.apartmentNum)
                ? (translate.getValue('inspections.apartment') + ' ' + address.apartmentNum)
                : null,
        ].filter(f => !CommonUtils.isNullOrWhiteSpace(f)).join(', ');
    }

    public static getToggleBoolOptions(translate: FuseTranslationLoaderService): NomenclatureDTO<InspectionToggleTypesEnum>[] {
        return [
            new NomenclatureDTO<InspectionToggleTypesEnum>({
                value: InspectionToggleTypesEnum.Y,
                displayName: translate.getValue('inspections.toggle-yes'),
            }),
            new NomenclatureDTO<InspectionToggleTypesEnum>({
                value: InspectionToggleTypesEnum.N,
                displayName: translate.getValue('inspections.toggle-no'),
            }),
        ];
    }

    public static getToggleTripleOptions(translate: FuseTranslationLoaderService): NomenclatureDTO<InspectionToggleTypesEnum>[] {
        return [
            new NomenclatureDTO<InspectionToggleTypesEnum>({
                value: InspectionToggleTypesEnum.Y,
                displayName: translate.getValue('inspections.toggle-yes'),
            }),
            new NomenclatureDTO<InspectionToggleTypesEnum>({
                value: InspectionToggleTypesEnum.N,
                displayName: translate.getValue('inspections.toggle-no'),
            }),
            new NomenclatureDTO<InspectionToggleTypesEnum>({
                value: InspectionToggleTypesEnum.X,
                displayName: translate.getValue('inspections.toggle-not-applicable'),
            }),
        ];
    }

    public static getToggleCheckOptions(translate: FuseTranslationLoaderService): NomenclatureDTO<InspectionToggleTypesEnum>[] {
        return [
            new NomenclatureDTO<InspectionToggleTypesEnum>({
                value: InspectionToggleTypesEnum.Y,
                displayName: translate.getValue('inspections.toggle-matches'),
            }),
            new NomenclatureDTO<InspectionToggleTypesEnum>({
                value: InspectionToggleTypesEnum.N,
                displayName: translate.getValue('inspections.toggle-does-not-match'),
            }),
            new NomenclatureDTO<InspectionToggleTypesEnum>({
                value: InspectionToggleTypesEnum.X,
                displayName: translate.getValue('inspections.toggle-not-available'),
            }),
        ];
    }

    public static getToggleMatchesOptions(translate: FuseTranslationLoaderService): NomenclatureDTO<InspectionToggleTypesEnum>[] {
        return [
            new NomenclatureDTO<InspectionToggleTypesEnum>({
                value: InspectionToggleTypesEnum.Y,
                displayName: translate.getValue('inspections.toggle-yes'),
            }),
            new NomenclatureDTO<InspectionToggleTypesEnum>({
                value: InspectionToggleTypesEnum.N,
                displayName: translate.getValue('inspections.toggle-no'),
            }),
            new NomenclatureDTO<InspectionToggleTypesEnum>({
                value: InspectionToggleTypesEnum.X,
                displayName: translate.getValue('inspections.toggle-not-available'),
            }),
        ];
    }
}