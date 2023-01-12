import { Injectable } from '@angular/core';

import { CommercialFishingLogBookEditDTO } from '@app/models/generated/dtos/CommercialFishingLogBookEditDTO';

@Injectable()
export class CommercialFishingRegisterCacheService {
    private permitLicenselogBooksMap: Map<number, CommercialFishingLogBookEditDTO[]>;

    public constructor() {
        this.permitLicenselogBooksMap = new Map<number, CommercialFishingLogBookEditDTO[]>();
    }

    public cacheLogBooks(permitLicenseId: number, logBooks: CommercialFishingLogBookEditDTO[]): void {
        this.permitLicenselogBooksMap.set(permitLicenseId, logBooks);
    }

    public getLogBooks(permitLicenseId: number): CommercialFishingLogBookEditDTO[] | undefined {
        return this.permitLicenselogBooksMap.get(permitLicenseId);
    }

    public checkLogBooks(permitLicenseId: number): boolean {
        if (this.permitLicenselogBooksMap.get(permitLicenseId) != undefined) {
            return true;
        }
        else {
            return false;
        }
    }

    public getPermitLicenseLogBooksCacheSize(): number {
        return this.permitLicenselogBooksMap.size;
    }

    public removeLogBooks(permitLicenseId: number): void {
        this.permitLicenselogBooksMap.delete(permitLicenseId);
    }

    public clearPermitLicenseLogBooksCache(): void {
        this.permitLicenselogBooksMap.clear();
    }
}