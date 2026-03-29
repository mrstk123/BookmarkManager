import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { Observable, combineLatest, map, startWith, switchMap, merge } from 'rxjs';
import { AuthService } from '../../../../core/services/auth.service';
import { BookmarksService } from '../../../../core/services/bookmarks.service';
import { FolderService } from '../../../../core/services/folder.service';
import { SearchService } from '../../../../core/services/search.service';
import { Bookmark } from '../../../../models/bookmark.model';
import { BookmarksViewComponent } from '../../../../shared/components/bookmarks-view/bookmarks-view.component';

@Component({
  selector: 'app-folder-bookmarks',
  standalone: true,
  imports: [CommonModule, BookmarksViewComponent],
  templateUrl: './folder-bookmarks.component.html',
  styleUrls: ['./folder-bookmarks.component.scss'],
})
export class FolderBookmarksComponent implements OnInit {
  route = inject(ActivatedRoute);
  private bookmarksService = inject(BookmarksService);
  private folderService = inject(FolderService);
  private searchService = inject(SearchService);
  private authService = inject(AuthService);

  filteredBookmarks$!: Observable<Bookmark[]>;

  ngOnInit() {
    this.filteredBookmarks$ = this.route.paramMap.pipe(
      switchMap(params => {
        const folderName = params.get('name') || '';
        return combineLatest([
          this.bookmarksService.getBookmarksByFolder(this.authService.getUserId(), folderName),
          this.searchService.searchQuery$.pipe(startWith('')),
          this.folderService.folders$,
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

  onDelete(id: number): void {
    this.bookmarksService.deleteBookmark(id).subscribe();
  }

  onCreated(): void { }

  onToggleFavorite(id: number): void {
    this.bookmarksService.toggleFavorite(id).subscribe();
  }
}