

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { OriginDeclarationFishDTO } from './OriginDeclarationFishDTO'; 

export class ShipRegisterOriginDeclarationFishDTO extends OriginDeclarationFishDTO {
    public constructor(obj?: Partial<ShipRegisterOriginDeclarationFishDTO>) {
        if (obj != undefined) {
            super(obj as OriginDeclarationFishDTO);
            Object.assign(this, obj);
        } 
        else {
            super();
        }
    }
  
    @StrictlyTyped(String)
    public logBookNum?: string;

    @StrictlyTyped(String)
    public logBookPageNum?: string;

    @StrictlyTyped(Date)
    public date?: Date;

    @StrictlyTyped(String)
    public fishingGearTypeName?: string;

    @StrictlyTyped(Number)
    public unloadTypeId?: number;

    @StrictlyTyped(Number)
    public unloadPortId?: number;

    @StrictlyTyped(Date)
    public unloadDateTime?: Date;
}