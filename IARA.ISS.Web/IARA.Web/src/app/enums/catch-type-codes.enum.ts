﻿export enum CatchTypeCodesEnum {
    /**
     * Улов на борда по време на риболовната активност
     * */
    ONBOARD,

    /**
     * Улов, разпределен към квота
     * */
    ALLOCATED_TO_QUOTA,

    /**
     * Улов, взет като проба за оценка на състава на улова
     * */
    SAMPLE,

    /**
     * Улов, съхраняван в мрежа по време на дейността
     * */
    KEPT_IN_NET,

    /**
     * Улов, взет на борда по време на риболовната активност
     * */
    TAKEN_ONBOARD,

    /**
     * Улов или морски животни, освободени по време на дейността (уловът се освобождава, ако мрежата никога не е била затворена
     * след точката на извличане, както е определено в регионалните планове за изхвърляне)
     * */
    RELEASED,

    /**
     * Улов, изхвърлен по време на дейността
     * */
    DISCARDED,

    /**
     * Изхвърлен улов, за който се прилагат изключенията de minimis
     * */
    DEMINIMIS,

    /**
     * Улов, разтоварен (за декларации) или предстои да бъде разтоварен (за уведомления) от кораба или неговото оборудване по време на операцията
     * (напр. разтоварване, трансбордиране, преместване, изхвърляне в морето)
     * */
    UNLOADED,

    /**
     * Улов, натоварен (за декларации) или който ще бъде натоварен (за уведомления) на кораба по време на операцията
     * (напр. трансбордиране, преместване).
     * */
    LOADED,

    /**
     * Случаен улов на водни животни
     * */
    BY_CATCH
}