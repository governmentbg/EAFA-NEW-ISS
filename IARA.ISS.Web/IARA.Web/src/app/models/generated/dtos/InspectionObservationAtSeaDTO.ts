

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { InspectionEditDTO } from './InspectionEditDTO';
import { VesselDuringInspectionDTO } from './VesselDuringInspectionDTO';
import { InspectionObservationToolDTO } from './InspectionObservationToolDTO';
import { InspectionVesselActivityNomenclatureDTO } from './InspectionVesselActivityNomenclatureDTO';

export class InspectionObservationAtSeaDTO extends InspectionEditDTO {
    public constructor(obj?: Partial<InspectionObservationAtSeaDTO>) {
        if (obj != undefined) {
            super(obj as InspectionEditDTO);
            Object.assign(this, obj);
        }
        else {
            super();
        }
    }

    @StrictlyTyped(VesselDuringInspectionDTO)
    public observedVessel?: VesselDuringInspectionDTO;

    @StrictlyTyped(String)
    public course?: string;

    @StrictlyTyped(Number)
    public speed?: number;

    @StrictlyTyped(InspectionObservationToolDTO)
    public observationTools?: InspectionObservationToolDTO[];

    @StrictlyTyped(Boolean)
    public hasShipContact?: boolean;

    @StrictlyTyped(Boolean)
    public hasShipCommunication?: boolean;

    @StrictlyTyped(String)
    public shipCommunicationDescription?: string;

    @StrictlyTyped(InspectionVesselActivityNomenclatureDTO)
    public observedVesselActivities?: InspectionVesselActivityNomenclatureDTO[];
}