import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { Observable, Subject, combineLatest, map, startWith, switchMap, merge, scan } from 'rxjs';
import { BookmarksService } from '../../../../core/services/bookmarks.service';
import { TagService } from '../../../../core/services/tag.service';
import { AuthService } from '../../../../core/services/auth.service';
import { SearchService } from '../../../../core/services/search.service';
import { Bookmark } from '../../../../models/bookmark.model';
import { BookmarksViewComponent } from '../../../../shared/components/bookmarks-view/bookmarks-view.component';

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
  private tagService = inject(TagService);
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
          this.tagService.tags$,
        ]).pipe(
          map(([bookmarks, query]) => {
            if (!query.trim()) return bookmarks;
            const q = query.toLowerCase();
            return bookmarks.filter(b =>
              b.title.toLowerCase().includes(q) ||
              b.url.toLowerCase().includes(q) ||
              b.tags.some((t: any) => t.toLowerCase().includes(q))
            );
          })
        );
      })
    );
  }

  onToggledFavorite(bookmark: Bookmark): void {
    this.toggledSubject.next(bookmark);
  }

  onDelete(id: number): void {
    this.bookmarksService.deleteBookmark(id).subscribe();
  }

  onCreated(): void { }
}