import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Observable, combineLatest, map, startWith } from 'rxjs';
import { AuthService } from '../../../../core/services/auth.service';
import { SearchService } from '../../../../core/services/search.service';
import { Bookmark } from '../../../../models/bookmark.model';
import { BookmarksService } from '../../../../core/services/bookmarks.service';
import { BookmarksViewComponent } from '../../../../shared/components/bookmarks-view/bookmarks-view.component';

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
    private authService = inject(AuthService);

    filteredBookmarks$!: Observable<Bookmark[]>;

    ngOnInit() {
        const bookmarks$ = this.bookmarksService.getBookmarks(this.authService.getUserId());

        this.filteredBookmarks$ = combineLatest([
            bookmarks$,
            this.searchService.searchQuery$.pipe(startWith(''))
        ]).pipe(
            map(([bookmarks, query]) => {
                if (!query.trim()) return bookmarks;
                const q = query.toLowerCase();
                return bookmarks.filter(b =>
                    b.title.toLowerCase().includes(q) ||
                    b.url.toLowerCase().includes(q) ||
                    (b.folderName?.toLowerCase().includes(q)) ||
                    b.tags.some((t: any) => t.toLowerCase().includes(q))
                );
            })
        );
    }

    onDelete(id: number): void {
        this.bookmarksService.deleteBookmark(id).subscribe();
    }

    onCreated(): void {
        // Observable auto-updates via HTTP re-fetch
    }

    onToggleFavorite(id: number): void {
        this.bookmarksService.toggleFavorite(id).subscribe();
    }
}