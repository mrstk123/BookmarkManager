import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Observable, combineLatest, map, startWith } from 'rxjs';
import { SearchService } from '../../../core/services/search.service';
import { Bookmark } from '../../../models/bookmark.model';
import { BookmarksViewComponent } from '../../../shared/components/bookmarks-view/bookmarks-view.component';
import { BookmarksService } from '../../../core/services/bookmarks.service';
import { filterBookmarks } from '../../../shared/utils/filter.utils';

@Component({
    selector: 'app-favorites-component',
    standalone: true,
    imports: [CommonModule, BookmarksViewComponent],
    templateUrl: './favorites.component.html',
    styleUrls: ['./favorites.component.scss'],
})
export class FavoritesComponent implements OnInit {
    private bookmarksService = inject(BookmarksService);
    private searchService = inject(SearchService);

    filteredBookmarks$!: Observable<Bookmark[]>;

    ngOnInit() {
        const bookmarks$ = this.bookmarksService.getFavorites();

        this.filteredBookmarks$ = combineLatest([
            bookmarks$,
            this.searchService.searchQuery$.pipe(startWith(''))
        ]).pipe(
            map(([bookmarks, query]) => filterBookmarks(bookmarks, query))
        );
    }

    onDelete(id: number): void {
        this.bookmarksService.deleteBookmark(id).subscribe({
            error: (err) => console.error('Failed to delete bookmark', err),
        });
    }

    onCreated(): void { }

    onUnfavorited(bookmark: Bookmark): void {
        this.bookmarksService.refresh();
    }
}