import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { Observable, Subject, combineLatest, map, startWith, switchMap, merge, scan } from 'rxjs';
import { AuthService } from '../../../../core/services/auth.service';
import { BookmarksService } from '../../../../core/services/bookmarks.service';
import { SearchService } from '../../../../core/services/search.service';
import { Bookmark } from '../../../../models/bookmark.model';
import { BookmarksViewComponent } from '../../../../shared/components/bookmarks-view/bookmarks-view.component';
import { filterBookmarks } from '../../../../shared/utils/filter.utils';

@Component({
  selector: 'app-tag-bookmarks',
  standalone: true,
  imports: [CommonModule, BookmarksViewComponent],
  templateUrl: './tag-bookmarks.component.html',
  styleUrls: ['./tag-bookmarks.component.scss'],
})
export class TagBookmarksComponent implements OnInit {
  route = inject(ActivatedRoute);
  private bookmarksService = inject(BookmarksService);
  private searchService = inject(SearchService);
  private authService = inject(AuthService);

  filteredBookmarks$!: Observable<Bookmark[]>;
  private toggledSubject = new Subject<Bookmark>();

  ngOnInit() {
    const search$ = this.searchService.searchQuery$.pipe(startWith(''));

    this.filteredBookmarks$ = this.route.paramMap.pipe(
      switchMap(params => {
        const tagName = params.get('name') || '';
        const bookmarks$ = this.bookmarksService.getBookmarksByTag(this.authService.getUserId(), tagName);
        return combineLatest([
          merge(
            bookmarks$,
            this.toggledSubject
          ).pipe(
            scan((current: Bookmark[], incoming) => {
              if (Array.isArray(incoming)) return incoming;
              return current.map(b => b.id === incoming.id ? incoming : b);
            }, [] as Bookmark[])
          ),
          search$,
        ]).pipe(
          map(([bookmarks, query]) => filterBookmarks(bookmarks, query))
        );
      })
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