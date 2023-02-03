import { AreaTypes } from '@app/shared/enums/area-type.enum';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { SubmittedByRoleNomenclatureDTO } from '@app/models/generated/dtos/SubmittedByRoleNomenclatureDTO';
import { RequestService } from '@app/shared/services/request.service';
import { CancellationReasonDTO } from '@app/models/generated/dtos/CancellationReasonDTO';
import { PermittedFileTypeDTO } from '@app/models/generated/dtos/PermittedFileTypeDTO';
import { ApplicationDeliveryTypeDTO } from '@app/models/generated/dtos/ApplicationDeliveryTypeDTO';
import { ChangeOfCircumstancesTypeDTO } from '@app/models/generated/dtos/ChangeOfCircumstancesTypeDTO';
import { MunicipalityNomenclatureExtendedDTO } from '@app/models/generated/dtos/MunicipalityNomenclatureExtendedDTO';
import { PopulatedAreaNomenclatureExtendedDTO } from '@app/models/generated/dtos/PopulatedAreaNomenclatureExtendedDTO';
import { FishingGearNomenclatureDTO } from '@app/models/generated/dtos/FishingGearNomenclatureDTO';
import { FishNomenclatureDTO } from '@app/models/generated/dtos/FishNomenclatureDTO';
import { ShipNomenclatureDTO } from '@app/models/generated/dtos/ShipNomenclatureDTO';
import { TariffNomenclatureDTO } from '@app/models/generated/dtos/TariffNomenclatureDTO';
import { InspectionObservationToolNomenclatureDTO } from '@app/models/generated/dtos/InspectionObservationToolNomenclatureDTO';
import { InspectionVesselActivityNomenclatureDTO } from '@app/models/generated/dtos/InspectionVesselActivityNomenclatureDTO';
import { ShipsUtils } from '@app/shared/utils/ships.utils';
import { ShipNomenclatureFlags } from '@app/enums/ship-nomenclature-flags.enum';
import { ShipNomenclatureChangeFlags } from '@app/enums/ship-nomenclature-change-flags.enum';

@Injectable({
    providedIn: 'root'
})
export class CommonNomenclatures {
    protected controller: string = 'Nomenclatures';

    private readonly area: AreaTypes = AreaTypes.Nomenclatures;
    private readonly requestService: RequestService;
    private readonly translate: FuseTranslationLoaderService;

    public constructor(requestService: RequestService, translate: FuseTranslationLoaderService) {
        this.requestService = requestService;
        this.translate = translate;
    }

    public getCountries(): Observable<NomenclatureDTO<number>[]> {
        return this.requestService.get<NomenclatureDTO<number>[]>(this.area, this.controller, 'GetCountries', { responseTypeCtr: NomenclatureDTO })
            .pipe(map((countries: NomenclatureDTO<number>[]) => {
                for (const country of countries) {
                    country.displayName = `${country.code} - ${country.displayName}`;
                }

                return countries;
            }));;
    }

    public getMunicipalities(): Observable<MunicipalityNomenclatureExtendedDTO[]> {
        return this.requestService.get(this.area, this.controller, 'GetMuncipalities', { responseTypeCtr: MunicipalityNomenclatureExtendedDTO });
    }

    public getDistricts(): Observable<NomenclatureDTO<number>[]> {
        return this.requestService.get(this.area, this.controller, 'GetDistricts', { responseTypeCtr: NomenclatureDTO });
    }

    public getPopulatedAreas(): Observable<PopulatedAreaNomenclatureExtendedDTO[]> {
        return this.requestService.get<PopulatedAreaNomenclatureExtendedDTO[]>(this.area, this.controller, 'GetPopulatedAreas', {
            responseTypeCtr: PopulatedAreaNomenclatureExtendedDTO
        }).pipe(map((populatedAreas: PopulatedAreaNomenclatureExtendedDTO[]) => {
            const prefixes = new Map<string, string>([
                ['С', 'с.'],
                ['Г', 'гр.'],
                ['М', 'ман.']
            ]);

            const descriptionMap: Record<string, string> = {
                '{DIS}': 'обл.',
                '{MUN}': 'общ.'
            };

            for (const populatedArea of populatedAreas) {
                populatedArea.displayName = populatedArea.displayName!.replace('{ARE}', prefixes.get(populatedArea.areaType!)!);
                populatedArea.description = populatedArea.description!.replace(/{DIS}|{MUN}/g, (matched: string) => {
                    return descriptionMap[matched];
                });
            }

            populatedAreas = populatedAreas.sort((lhs: PopulatedAreaNomenclatureExtendedDTO, rhs: PopulatedAreaNomenclatureExtendedDTO) => {
                return lhs.displayName!.localeCompare(rhs.displayName!);
            });

            return populatedAreas;
        }));
    }

    public getTerritoryUnits(): Observable<NomenclatureDTO<number>[]> {
        return this.requestService.get<NomenclatureDTO<number>[]>(this.area, this.controller, 'GetTerritoryUnits', {
            responseTypeCtr: NomenclatureDTO
        }).pipe(map((territoryUnits: NomenclatureDTO<number>[]) => {
            for (const territoryUnit of territoryUnits) {
                territoryUnit.displayName = `${territoryUnit.code} - ${territoryUnit.displayName}`;
            }

            return territoryUnits;
        }));
    }

    public getDepartments(): Observable<NomenclatureDTO<number>[]> {
        return this.requestService.get(this.area, this.controller, 'GetDepartments', { responseTypeCtr: NomenclatureDTO });
    }

    public getSectors(): Observable<NomenclatureDTO<number>[]> {
        return this.requestService.get(this.area, this.controller, 'GetSectors', { responseTypeCtr: NomenclatureDTO });
    }

    public getDocumentTypes(): Observable<NomenclatureDTO<number>[]> {
        return this.requestService.get(this.area, this.controller, 'GetDocumentTypes', { responseTypeCtr: NomenclatureDTO });
    }

    public getFishTypes(): Observable<FishNomenclatureDTO[]> {
        return this.requestService.get<FishNomenclatureDTO[]>(this.area, this.controller, 'GetFishTypes', {
            responseTypeCtr: FishNomenclatureDTO
        }).pipe(map((fishes: FishNomenclatureDTO[]) => {
            for (const fish of fishes) {
                fish.displayName = `${fish.code} - ${fish.displayName}`;
            }

            return fishes;
        }));
    }

    public getPermissions(): Observable<NomenclatureDTO<number>[]> {
        return this.requestService.get(this.area, this.controller, 'GetPermissions', { responseTypeCtr: NomenclatureDTO });
    }

    public getUserNames(): Observable<NomenclatureDTO<number>[]> {
        return this.requestService.get(this.area, this.controller, 'GetUserNames', { responseTypeCtr: NomenclatureDTO });
    }

    public getFileTypes(): Observable<NomenclatureDTO<number>[]> {
        return this.requestService.get(this.area, this.controller, 'GetFileTypes', { responseTypeCtr: NomenclatureDTO });
    }

    public getOfflinePaymentTypes(): Observable<NomenclatureDTO<number>[]> {
        return this.requestService.get(this.area, this.controller, 'GetOfflinePaymentTypes', { responseTypeCtr: NomenclatureDTO });
    }

    public getOnlinePaymentTypes(): Observable<NomenclatureDTO<number>[]> {
        return this.requestService.get(this.area, this.controller, 'GetOnlinePaymentTypes', { responseTypeCtr: NomenclatureDTO });
    }

    public getPaymentStatuses(): Observable<NomenclatureDTO<number>[]> {
        return this.requestService.get(this.area, this.controller, 'GetPaymentStatuses', { responseTypeCtr: NomenclatureDTO });
    }

    public getGenders(): Observable<NomenclatureDTO<number>[]> {
        return this.requestService.get(this.area, this.controller, 'GetGenders', { responseTypeCtr: NomenclatureDTO });
    }

    public getSubmittedByRoles(): Observable<SubmittedByRoleNomenclatureDTO[]> {
        return this.requestService.get(this.area, this.controller, 'GetSubmittedByRoles', { responseTypeCtr: SubmittedByRoleNomenclatureDTO });
    }

    public getCancellationReasons(): Observable<CancellationReasonDTO[]> {
        return this.requestService.get(this.area, this.controller, 'GetCancellationReasons', { responseTypeCtr: NomenclatureDTO });
    }

    public getPermittedFileTypes(): Observable<PermittedFileTypeDTO[]> {
        return this.requestService.get(this.area, this.controller, 'GetPermittedFileTypes', { responseTypeCtr: PermittedFileTypeDTO });
    }

    public getDeliveryTypes(): Observable<ApplicationDeliveryTypeDTO[]> {
        return this.requestService.get(this.area, this.controller, 'GetDeliveryTypes', { responseTypeCtr: NomenclatureDTO });
    }

    public getChangeOfCircumstancesTypes(): Observable<ChangeOfCircumstancesTypeDTO[]> {
        return this.requestService.get(this.area, this.controller, 'GetChangeOfCircumstancesTypes', { responseTypeCtr: ChangeOfCircumstancesTypeDTO });
    }

    public getFishingGear(): Observable<FishingGearNomenclatureDTO[]> {
        return this.requestService.get<FishingGearNomenclatureDTO[]>(this.area, this.controller, 'GetFishingGear', {
            responseTypeCtr: FishingGearNomenclatureDTO
        }).pipe(map((fishingGears: FishingGearNomenclatureDTO[]) => {
            for (const fishingGear of fishingGears) {
                fishingGear.displayName = `${fishingGear.code} - ${fishingGear.displayName}`;
            }

            return fishingGears;
        }));
    }

    public getFishingGearMarkStatuses(): Observable<NomenclatureDTO<number>[]> {
        return this.requestService.get(this.area, this.controller, 'GetFishingGearMarkStatuses', { responseTypeCtr: NomenclatureDTO });
    }

    public getFishingGearPingerStatuses(): Observable<NomenclatureDTO<number>[]> {
        return this.requestService.get(this.area, this.controller, 'GetFishingGearPingerStatuses', { responseTypeCtr: NomenclatureDTO });

    }

    public getInstitutions(): Observable<NomenclatureDTO<number>[]> {
        return this.requestService.get(this.area, this.controller, 'GetInstitutions', { responseTypeCtr: NomenclatureDTO });
    }

    public getVesselTypes(): Observable<NomenclatureDTO<number>[]> {
        return this.requestService.get(this.area, this.controller, 'GetVesselTypes', { responseTypeCtr: NomenclatureDTO });
    }

    public getPatrolVehicleTypes(): Observable<NomenclatureDTO<number>[]> {
        return this.requestService.get(this.area, this.controller, 'GetPatrolVehicleTypes', { responseTypeCtr: NomenclatureDTO });
    }

    public getCatchCheckTypes(): Observable<NomenclatureDTO<number>[]> {
        return this.requestService.get(this.area, this.controller, 'GetCatchCheckTypes', { responseTypeCtr: NomenclatureDTO });
    }

    public getObservationTools(): Observable<InspectionObservationToolNomenclatureDTO[]> {
        return this.requestService.get(this.area, this.controller, 'GetObservationTools', { responseTypeCtr: InspectionObservationToolNomenclatureDTO });
    }

    public getPorts(): Observable<NomenclatureDTO<number>[]> {
        return this.requestService.get(this.area, this.controller, 'GetPorts', { responseTypeCtr: NomenclatureDTO });
    }

    public getVesselActivities(): Observable<InspectionVesselActivityNomenclatureDTO[]> {
        return this.requestService.get(this.area, this.controller, 'GetVesselActivities', { responseTypeCtr: InspectionVesselActivityNomenclatureDTO });
    }

    public getUsageDocumentTypes(): Observable<NomenclatureDTO<number>[]> {
        return this.requestService.get(this.area, this.controller, 'GetUsageDocumentTypes', { responseTypeCtr: NomenclatureDTO });
    }

    public getCatchZones(): Observable<NomenclatureDTO<number>[]> {
        return this.requestService.get(this.area, this.controller, 'GetCatchZones', { responseTypeCtr: NomenclatureDTO });
    }

    public getShips(): Observable<ShipNomenclatureDTO[]> {
        return this.requestService.get<ShipNomenclatureDTO[]>(this.area, this.controller, 'GetShips', {
            responseTypeCtr: NomenclatureDTO
        }).pipe(map((ships: ShipNomenclatureDTO[]) => {
            const cfr: string = this.translate.getValue('common.ship-cfr');
            const em: string = this.translate.getValue('common.ship-external-mark');

            for (const ship of ships) {
                if (ship.cfr && ship.cfr.length !== 0) {
                    ship.displayName = `${ship.name} | ${cfr}: ${ship.cfr} | ${em}: ${ship.externalMark}`;
                }
                else {
                    ship.displayName = `${ship.name} | ${em}: ${ship.externalMark}`;
                }

                if (ship.eventData) {
                    const keys: string[] = Object.keys(ship.eventData);

                    for (const key of keys) {
                        const event: ShipNomenclatureDTO = ship.eventData[Number(key)];

                        if (event.changeFlags !== ShipNomenclatureChangeFlags.None) {
                            if (!ShipsUtils.hasChange(event, ShipNomenclatureChangeFlags.Cfr)) {
                                event.cfr = ship.cfr;
                            }

                            if (!ShipsUtils.hasChange(event, ShipNomenclatureChangeFlags.Name)) {
                                event.name = ship.name;
                            }

                            if (!ShipsUtils.hasChange(event, ShipNomenclatureChangeFlags.ExternalMark)) {
                                event.externalMark = ship.externalMark;
                            }

                            if (!ShipsUtils.hasChange(event, ShipNomenclatureChangeFlags.TotalLength)) {
                                event.totalLength = ship.totalLength;
                            }

                            if (!ShipsUtils.hasChange(event, ShipNomenclatureChangeFlags.GrossTonnage)) {
                                event.grossTonnage = ship.grossTonnage;
                            }

                            if (!ShipsUtils.hasChange(event, ShipNomenclatureChangeFlags.MainEnginePower)) {
                                event.mainEnginePower = ship.mainEnginePower;
                            }
                        }

                        if (event.cfr && event.cfr.length !== 0) {
                            event.displayName = `${event.name} | ${cfr}: ${event.cfr} | ${em}: ${event.externalMark}`;
                        }
                        else {
                            event.displayName = `${event.name} | ${em}: ${event.externalMark}`;
                        }

                        event.flags = ship.flags;
                    }
                }
            }

            return ships;
        }));
    }

    public getLogBookTypes(): Observable<NomenclatureDTO<number>[]> {
        return this.requestService.get(this.area, this.controller, 'GetLogBookTypes', { responseTypeCtr: NomenclatureDTO });
    }

    public getLogBookStatuses(): Observable<NomenclatureDTO<number>[]> {
        return this.requestService.get(this.area, this.controller, 'GetLogBookStatuses', { responseTypeCtr: NomenclatureDTO });
    }

    public getInspectionTypes(): Observable<NomenclatureDTO<number>[]> {
        return this.requestService.get(this.area, this.controller, 'GetInspectionTypes', { responseTypeCtr: NomenclatureDTO });
    }

    public getPaymentTariffs(): Observable<TariffNomenclatureDTO[]> {
        return this.requestService.get(this.area, this.controller, 'GetPaymentTariffs', { responseTypeCtr: TariffNomenclatureDTO });
    }

    public getShipAssociations(): Observable<NomenclatureDTO<number>[]> {
        return this.requestService.get(this.area, this.controller, 'GetShipAssociations', { responseTypeCtr: NomenclatureDTO });
    }

    public getCatchInspectionTypes(): Observable<NomenclatureDTO<number>[]> {
        return this.requestService.get(this.area, this.controller, 'GetCatchInspectionTypes', { responseTypeCtr: NomenclatureDTO });
    }

    public getTransportVehicleTypes(): Observable<NomenclatureDTO<number>[]> {
        return this.requestService.get(this.area, this.controller, 'GetTransportVehicleTypes', { responseTypeCtr: NomenclatureDTO });
    }

    public getFishSex(): Observable<NomenclatureDTO<number>[]> {
        return this.requestService.get(this.area, this.controller, 'GetFishSex', { responseTypeCtr: NomenclatureDTO });
    }

    public getTurbotSizeGroups(): Observable<NomenclatureDTO<number>[]> {
        return this.requestService.get(this.area, this.controller, 'GetTurbotSizeGroups', { responseTypeCtr: NomenclatureDTO });
    }

    public getWaterBodyTypes(): Observable<NomenclatureDTO<number>[]> {
        return this.requestService.get(this.area, this.controller, 'GetWaterBodyTypes', { responseTypeCtr: NomenclatureDTO });
    }

    public getCatchPresentations(): Observable<NomenclatureDTO<number>[]> {
        type Result = NomenclatureDTO<number>[];
        return this.requestService.get<Result>(this.area, this.controller, 'GetCatchPresentations', { responseTypeCtr: NomenclatureDTO }).pipe(map(
            (entries: Result) => {
                for (const entry of entries) {
                    entry.displayName = `${entry.code} - ${entry.displayName}`;
                }

                return entries;
            })
        );
    }

    public getPoundNets(): Observable<NomenclatureDTO<number>[]> {
        return this.requestService.get(this.area, this.controller, 'GetPoundNets', { responseTypeCtr: NomenclatureDTO });
    }

    public getMarkReasons(): Observable<NomenclatureDTO<number>[]> {
        return this.requestService.get(this.area, this.controller, 'GetMarkReasons', { responseTypeCtr: NomenclatureDTO });
    }

    public getRemarkReasons(): Observable<NomenclatureDTO<number>[]> {
        return this.requestService.get(this.area, this.controller, 'GetRemarkReasons', { responseTypeCtr: NomenclatureDTO });
    }
}
