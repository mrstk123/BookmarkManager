import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class SearchService {
    private query$ = new BehaviorSubject<string>('');

    get searchQuery$() {
        return this.query$.asObservable();
    }

    get currentQuery(): string {
        return this.query$.value;
    }

    setQuery(query: string): void {
        this.query$.next(query);
    }
}