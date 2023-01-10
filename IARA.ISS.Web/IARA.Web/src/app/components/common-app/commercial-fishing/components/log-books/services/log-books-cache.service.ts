import { Injectable } from '@angular/core';

import { ShipLogBookPageRegisterDTO } from '@app/models/generated/dtos/ShipLogBookPageRegisterDTO';
import { AdmissionLogBookPageRegisterDTO } from '@app/models/generated/dtos/AdmissionLogBookPageRegisterDTO';
import { FirstSaleLogBookPageRegisterDTO } from '@app/models/generated/dtos/FirstSaleLogBookPageRegisterDTO';
import { TransportationLogBookPageRegisterDTO } from '@app/models/generated/dtos/TransportationLogBookPageRegisterDTO';
import { AquacultureLogBookPageRegisterDTO } from '@app/models/generated/dtos/AquacultureLogBookPageRegisterDTO';

@Injectable()
export class LogBooksCacheService {
    private shipLogBookPagesMap: Map<string, ShipLogBookPageRegisterDTO[]>;
    private shipAdmissionLogBookPagesMap: Map<string, AdmissionLogBookPageRegisterDTO[]>;
    private shipTransportationLogBookPagesMap: Map<string, TransportationLogBookPageRegisterDTO[]>; 

    private firstSaleLogBookPagesMap: Map<number, FirstSaleLogBookPageRegisterDTO[]>;
    private admissionLogBookPagesMap: Map<number, AdmissionLogBookPageRegisterDTO[]>;
    private transportationLogBookPagesMap: Map<number, TransportationLogBookPageRegisterDTO[]>;
    private aquacultureLogbookPagesMap: Map<number, AquacultureLogBookPageRegisterDTO[]>;

    public constructor() {
        this.shipLogBookPagesMap = new Map<string, ShipLogBookPageRegisterDTO[]>();
        this.shipAdmissionLogBookPagesMap = new Map<string, AdmissionLogBookPageRegisterDTO[]>();
        this.shipTransportationLogBookPagesMap = new Map<string, TransportationLogBookPageRegisterDTO[]>(); 

        this.firstSaleLogBookPagesMap = new Map<number, FirstSaleLogBookPageRegisterDTO[]>();
        this.admissionLogBookPagesMap = new Map<number, AdmissionLogBookPageRegisterDTO[]>();
        this.transportationLogBookPagesMap = new Map<number, TransportationLogBookPageRegisterDTO[]>();
        this.aquacultureLogbookPagesMap = new Map<number, AquacultureLogBookPageRegisterDTO[]>();
    }

    public cacheShipLogBookPages(logBookId: number, permitLicenseId: number, pages: ShipLogBookPageRegisterDTO[]): void {
        const key: string = this.generateMapKey(logBookId, permitLicenseId);
        this.shipLogBookPagesMap.set(key, pages);
    }

    public getShipLogBookPages(logBookId: number, permitLicenseId: number): ShipLogBookPageRegisterDTO[] | undefined {
        const key: string = this.generateMapKey(logBookId, permitLicenseId);
        return this.shipLogBookPagesMap.get(key);
    }

    public cacheShipAdmissionLogBookPages(logBookId: number, permitLicenseId: number, pages: AdmissionLogBookPageRegisterDTO[]): void {
        const key: string = this.generateMapKey(logBookId, permitLicenseId);
        this.shipAdmissionLogBookPagesMap.set(key, pages);
    }

    public getShipAdmissionLogBookPages(logBookId: number, permitLicenseId: number): AdmissionLogBookPageRegisterDTO[] | undefined {
        const key: string = this.generateMapKey(logBookId, permitLicenseId);
        return this.shipAdmissionLogBookPagesMap.get(key);
    }

    public cacheShipTransportationLogBookPages(logBookId: number, permitLicenseId: number, pages: TransportationLogBookPageRegisterDTO[]): void {
        const key: string = this.generateMapKey(logBookId, permitLicenseId);
        this.shipTransportationLogBookPagesMap.set(key, pages);
    }

    public getShipTransportationLogBookPages(logBookId: number, permitLicenseId: number): TransportationLogBookPageRegisterDTO[] | undefined {
        const key: string = this.generateMapKey(logBookId, permitLicenseId);
        return this.shipTransportationLogBookPagesMap.get(key);
    }

    public cacheFirstSaleLogBookPages(logBookId: number, pages: FirstSaleLogBookPageRegisterDTO[]): void {
        this.firstSaleLogBookPagesMap.set(logBookId, pages);
    }

    public getFirstSaleLogBookPages(logBookId: number): FirstSaleLogBookPageRegisterDTO[] | undefined{
        return this.firstSaleLogBookPagesMap.get(logBookId);
    }

    public cacheAdmissionLogBookPages(logBookId: number, pages: AdmissionLogBookPageRegisterDTO[]): void {
        this.admissionLogBookPagesMap.set(logBookId, pages);
    }

    public getAdmissionLogBookPages(logBookId: number): AdmissionLogBookPageRegisterDTO[] | undefined {
        return this.admissionLogBookPagesMap.get(logBookId);
    }

    public cacheTransportationLogBookPages(logBookId: number, pages: TransportationLogBookPageRegisterDTO[]): void {
        this.transportationLogBookPagesMap.set(logBookId, pages);
    }

    public getTransportationLogBookPages(logBookId: number): TransportationLogBookPageRegisterDTO[] | undefined {
        return this.transportationLogBookPagesMap.get(logBookId);
    }

    public cacheAquacultureLogBookPages(logBookId: number, pages: AquacultureLogBookPageRegisterDTO[]): void {
        this.aquacultureLogbookPagesMap.set(logBookId, pages);
    }

    public getAquacultureLogBookPages(logBookId: number): AquacultureLogBookPageRegisterDTO[] | undefined {
        return this.aquacultureLogbookPagesMap.get(logBookId);
    }

    private generateMapKey(logBookId: number, permitLicenseId: number): string {
        return JSON.stringify(new LogBookPermitLicenseIds(logBookId, permitLicenseId));
    }
}

class LogBookPermitLicenseIds {
    public logBookId: number;
    public permitLicenseId: number;

    public constructor(logBookId: number, permitLicenseId: number) {
        this.logBookId = logBookId;
        this.permitLicenseId = permitLicenseId;
    }
}