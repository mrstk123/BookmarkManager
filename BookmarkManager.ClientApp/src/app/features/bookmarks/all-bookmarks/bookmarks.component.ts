import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Observable, Subject, combineLatest, map, startWith, merge, scan } from 'rxjs';
import { SearchService } from '../../../core/services/search.service';
import { Bookmark } from '../../../models/bookmark.model';
import { BookmarksService } from '../../../core/services/bookmarks.service';
import { BookmarksViewComponent } from '../../../shared/components/bookmarks-view/bookmarks-view.component';
import { filterBookmarks } from '../../../shared/utils/filter.utils';

@Component({
    selector: 'app-bookmarks-component',
    standalone: true,
    imports: [CommonModule, BookmarksViewComponent],
    templateUrl: './bookmarks.component.html',
    styleUrls: ['./bookmarks.component.scss'],
})
export class BookmarksComponent implements OnInit {
    private bookmarksService = inject(BookmarksService);
    private searchService = inject(SearchService);

    filteredBookmarks$!: Observable<Bookmark[]>;
    private toggledSubject = new Subject<Bookmark>();

    ngOnInit() {
        const bookmarks$ = this.bookmarksService.getBookmarks();
        const search$ = this.searchService.searchQuery$.pipe(startWith(''));

        this.filteredBookmarks$ = combineLatest([
            merge(
                bookmarks$,
                this.toggledSubject
            ).pipe(
                scan((current: Bookmark[], incoming) => {
                    if (Array.isArray(incoming)) return incoming;
                    return current.map(b => b.id === incoming.id ? incoming : b);
                }, [] as Bookmark[])
            ),
            search$
        ]).pipe(
            map(([bookmarks, query]) => filterBookmarks(bookmarks, query))
        );
    }

    onToggledFavorite(bookmark: Bookmark): void {
        this.toggledSubject.next(bookmark);
    }

    onDelete(id: number): void {
        this.bookmarksService.deleteBookmark(id).subscribe({
            error: (err) => console.error('Failed to delete bookmark', err),
        });
    }

    onCreated(): void { }
}