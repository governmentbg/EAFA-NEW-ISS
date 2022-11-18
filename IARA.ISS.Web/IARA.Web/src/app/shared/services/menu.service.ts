import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';

@Injectable({
    providedIn: 'root'
})
export class MenuService {
    public folded: Subject<boolean> = new Subject<boolean>();
    public opened: Subject<boolean> = new Subject<boolean>();
}