import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';
import { InspectionSubjectAddressDTO } from '@app/models/generated/dtos/InspectionSubjectAddressDTO';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { InspectionToggleTypesEnum } from '../../enums/inspection-toggle-types.enum';
import { AddressRegistrationDTO } from '../../models/generated/dtos/AddressRegistrationDTO';
import { NomenclatureDTO } from '../../models/generated/dtos/GenericNomenclatureDTO';
import { CommonUtils } from './common.utils';

export class InspectionUtils {
    public static buildAddress(
        address: InspectionSubjectAddressDTO | AddressRegistrationDTO | undefined,
        translate: FuseTranslationLoaderService
    ): string | undefined {

        if (address === undefined || address === null) {
            return undefined;
        }

        let populatedArea: string | undefined;

        if (address instanceof InspectionSubjectAddressDTO) {
            populatedArea = address.populatedArea;
        }
        else if (address instanceof AddressRegistrationDTO) {
            populatedArea = address.populatedAreaName;
        }

        return [
            populatedArea,
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
                isActive: true,
            }),
            new NomenclatureDTO<InspectionToggleTypesEnum>({
                value: InspectionToggleTypesEnum.N,
                displayName: translate.getValue('inspections.toggle-no'),
                isActive: true,
            }),
        ];
    }

    public static getToggleTripleOptions(translate: FuseTranslationLoaderService): NomenclatureDTO<InspectionToggleTypesEnum>[] {
        return [
            new NomenclatureDTO<InspectionToggleTypesEnum>({
                value: InspectionToggleTypesEnum.Y,
                displayName: translate.getValue('inspections.toggle-yes'),
                isActive: true,
            }),
            new NomenclatureDTO<InspectionToggleTypesEnum>({
                value: InspectionToggleTypesEnum.N,
                displayName: translate.getValue('inspections.toggle-no'),
                isActive: true,
            }),
            new NomenclatureDTO<InspectionToggleTypesEnum>({
                value: InspectionToggleTypesEnum.X,
                displayName: translate.getValue('inspections.toggle-not-applicable'),
                isActive: true,
            }),
        ];
    }

    public static getToggleCheckOptions(translate: FuseTranslationLoaderService): NomenclatureDTO<InspectionToggleTypesEnum>[] {
        return [
            new NomenclatureDTO<InspectionToggleTypesEnum>({
                value: InspectionToggleTypesEnum.Y,
                displayName: translate.getValue('inspections.toggle-matches'),
                isActive: true,
            }),
            new NomenclatureDTO<InspectionToggleTypesEnum>({
                value: InspectionToggleTypesEnum.N,
                displayName: translate.getValue('inspections.toggle-does-not-match'),
                isActive: true,
            }),
            new NomenclatureDTO<InspectionToggleTypesEnum>({
                value: InspectionToggleTypesEnum.X,
                displayName: translate.getValue('inspections.toggle-not-available'),
                isActive: true,
            }),
        ];
    }

    public static getToggleMatchesOptions(translate: FuseTranslationLoaderService): NomenclatureDTO<InspectionToggleTypesEnum>[] {
        return [
            new NomenclatureDTO<InspectionToggleTypesEnum>({
                value: InspectionToggleTypesEnum.Y,
                displayName: translate.getValue('inspections.toggle-yes'),
                isActive: true,
            }),
            new NomenclatureDTO<InspectionToggleTypesEnum>({
                value: InspectionToggleTypesEnum.N,
                displayName: translate.getValue('inspections.toggle-no'),
                isActive: true,
            }),
            new NomenclatureDTO<InspectionToggleTypesEnum>({
                value: InspectionToggleTypesEnum.X,
                displayName: translate.getValue('inspections.toggle-not-available'),
                isActive: true,
            }),
        ];
    }

    //Validators
    public static atLeastOneCatchValidator(): ValidatorFn {
        return (form: AbstractControl): ValidationErrors | null => {
            const catchesControl = form.get('catchesControl')!;
            
            if (catchesControl === null || catchesControl === undefined) {
                return null;
            }

            if (catchesControl.value !== null && catchesControl.value !== undefined && catchesControl.value.length > 0) {
                return null;
            }

            return { 'atLeastOneCatchError': true };
        }
    }
}