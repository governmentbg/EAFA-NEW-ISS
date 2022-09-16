import { ShipNomenclatureFlags } from '@app/enums/ship-nomenclature-flags.enum';
import { ShipNomenclatureDTO } from '@app/models/generated/dtos/ShipNomenclatureDTO';
import { ShipNomenclatureChangeFlags } from '../../enums/ship-nomenclature-change-flags.enum';

export class ShipsUtils {
    public static isThirdParty(ship: ShipNomenclatureDTO): boolean {
        return ShipsUtils.is(ship, ShipNomenclatureFlags.ThirdPartyShip);
    }

    public static hasFishingCapacity(ship: ShipNomenclatureDTO): boolean {
        return ShipsUtils.is(ship, ShipNomenclatureFlags.FishingCapacity);
    }

    public static isForbiddenForPermits(ship: ShipNomenclatureDTO): boolean {
        return ShipsUtils.is(ship, ShipNomenclatureFlags.Forbidden);
    }

    public static hasBlackSeaPermit(ship: ShipNomenclatureDTO): boolean {
        return ShipsUtils.is(ship, ShipNomenclatureFlags.BlackSeaPermit);
    }

    public static hasDanubePermit(ship: ShipNomenclatureDTO): boolean {
        return ShipsUtils.is(ship, ShipNomenclatureFlags.DanubePermit);
    }

    public static hasBlackSeaPermitAppl(ship: ShipNomenclatureDTO): boolean {
        return ShipsUtils.is(ship, ShipNomenclatureFlags.BlackSeaPermitAppl);
    }

    public static hasDanubePermitAppl(ship: ShipNomenclatureDTO): boolean {
        return ShipsUtils.is(ship, ShipNomenclatureFlags.DanubePermitAppl);
    }

    public static hasPoundNetPermit(ship: ShipNomenclatureDTO): boolean {
        return ShipsUtils.is(ship, ShipNomenclatureFlags.PoundNetPermit);
    }

    public static hasPoundNetPermitAppl(ship: ShipNomenclatureDTO): boolean {
        return ShipsUtils.is(ship, ShipNomenclatureFlags.PoundNetPermitAppl);
    }

    public static hasActiveFishQuota(ship: ShipNomenclatureDTO): boolean {
        return ShipsUtils.is(ship, ShipNomenclatureFlags.ActiveFishQuota);
    }

    public static isDestOrDereg(ship: ShipNomenclatureDTO): boolean {
        return ShipsUtils.is(ship, ShipNomenclatureFlags.DestOrDereg);
    }

    public static is(ship: ShipNomenclatureDTO, flags: ShipNomenclatureFlags): boolean {
        return (ship.flags! & flags) !== 0;
    }

    public static hasChange(ship: ShipNomenclatureDTO, flags: ShipNomenclatureChangeFlags): boolean {
        return (ship.changeFlags! & flags) !== 0;
    }

    public static filter(ships: ShipNomenclatureDTO[], filters: ShipNomenclatureFilters | undefined): ShipNomenclatureDTO[] {
        if (filters !== undefined && filters !== null) {
            if (filters.isThirdPartyShip !== undefined && filters.isThirdPartyShip !== null) {
                ships = ships.filter(x => (ShipsUtils.isThirdParty(x) === filters.isThirdPartyShip && x.isActive === true) || x.isActive === false);
            }

            if (filters.hasFishingCapacity !== undefined && filters.hasFishingCapacity !== null) {
                ships = ships.filter(x => (ShipsUtils.hasFishingCapacity(x) === filters.hasFishingCapacity && x.isActive === true) || x.isActive === false);
            }

            if (filters.isForbiddenForPermits !== undefined && filters.isForbiddenForPermits !== null) {
                ships = ships.filter(x => (ShipsUtils.isForbiddenForPermits(x) === filters.isForbiddenForPermits && x.isActive === true) || x.isActive === false);
            }

            if (filters.hasBlackSeaPermit !== undefined && filters.hasBlackSeaPermit !== null) {
                ships = ships.filter(x => (ShipsUtils.hasBlackSeaPermit(x) === filters.hasBlackSeaPermit && x.isActive === true) || x.isActive === false);
            }

            if (filters.hasDanubePermit !== undefined && filters.hasDanubePermit !== null) {
                ships = ships.filter(x => (ShipsUtils.hasDanubePermit(x) === filters.hasDanubePermit && x.isActive === true) || x.isActive === false);
            }

            if (filters.hasPoundNetPermit !== undefined && filters.hasPoundNetPermit !== null) {
                ships = ships.filter(x => (ShipsUtils.hasPoundNetPermit(x) === filters.hasPoundNetPermit && x.isActive) || x.isActive === false);
            }

            if (filters.hasActiveFishQuota !== undefined && filters.hasActiveFishQuota !== null) {
                ships = ships.filter(x => (ShipsUtils.hasActiveFishQuota(x) === filters.hasActiveFishQuota && x.isActive === true) || x.isActive === false);
            }

            if (filters.isDestOrDereg !== undefined && filters.isDestOrDereg !== null) {
                ships = ships.filter(x => (ShipsUtils.isDestOrDereg(x) === filters.isDestOrDereg && x.isActive === true) || x.isActive === false);
            }
        }

        return ships;
    }

    public static get(ships: ShipNomenclatureDTO[], shipId: number): ShipNomenclatureDTO {
        const result: ShipNomenclatureDTO | undefined = ships.find(x => x.value === shipId);

        if (result !== undefined) {
            return result;
        }

        for (const ship of ships) {
            if (ship.eventData) {
                const keys: string[] = Object.keys(ship.eventData);

                for (const key of keys) {
                    const event: ShipNomenclatureDTO = ship.eventData[Number(key)];

                    if (event.value === shipId) {
                        return event;
                    }
                }
            }
        }

        throw new Error(`Ship with id ${shipId} not found.`);
    }
}

export class ShipNomenclatureFilters {
    public isThirdPartyShip: boolean | undefined;
    public hasFishingCapacity: boolean | undefined;
    public isForbiddenForPermits: boolean | undefined;
    public hasBlackSeaPermit: boolean | undefined;
    public hasDanubePermit: boolean | undefined;
    public hasPoundNetPermit: boolean | undefined;
    public hasActiveFishQuota: boolean | undefined;
    public isDestOrDereg: boolean | undefined;

    public constructor(obj?: Partial<ShipNomenclatureFilters>) {
        Object.assign(this, obj);
    }
}