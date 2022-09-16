
export class PageUtils {
    public static getPageForIndex(index: number, pageSize: number): number {
        return Math.floor(index / pageSize);
    }
}
