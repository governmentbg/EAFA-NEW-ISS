import { Injectable } from '@angular/core';

@Injectable()
export class ShipLogBookPageDataService {
    public get nextNewCatchRecordId(): number {
        this._lastNewCatchRecordId--;

        return this._lastNewCatchRecordId;
    }

    private _lastNewCatchRecordId: number = 0;
}