import { EventEmitter, Injectable } from '@angular/core';
import { FishingGearMarkDTO } from '@app/models/generated/dtos/FishingGearMarkDTO';

@Injectable({
    providedIn: 'root'
})
export class FishingGearManipulationService {

    public markAdded: EventEmitter<FishingGearMarkDTO>;

    public constructor() {
        this.markAdded = new EventEmitter();
    }
}
