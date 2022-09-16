import { Navigation as PublicNavigation } from 'src/app/shared/navigation/tl-navigation.public';
import { Navigation as AdministrativeNavigation } from 'src/app/shared/navigation/tl-navigation.administrative';
import { ITLNavigation } from './base/tl-navigation.interface';


export class Navigation {

    public static Menu: ITLNavigation[];

    public static getMenu(isPublic: boolean): ITLNavigation[] {
        if (isPublic) {
            return PublicNavigation.Menu;
        } else {
            return AdministrativeNavigation.Menu;
        }

    }
}