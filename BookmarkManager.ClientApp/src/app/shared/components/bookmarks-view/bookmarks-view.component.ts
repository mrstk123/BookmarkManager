import { Component, EventEmitter, Input, Output, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { StorageService } from '../../../core/services/storage.service';
import { DeleteBookmarkModalComponent } from '../delete-bookmark-modal/delete-bookmark-modal.component';
import { NewBookmarkModalComponent } from '../new-bookmark-modal/new-bookmark-modal.component';
import { Bookmark } from '../../../models/bookmark.model';
import { BookmarksService } from '../../../core/services/bookmarks.service';
import { getFaviconUrl } from '../../utils/url.utils';

const VIEW_MODE_KEY = 'bookmarks-view-mode';

@Component({
  selector: 'app-bookmarks-view',
  standalone: true,
  imports: [CommonModule, RouterModule, NewBookmarkModalComponent, DeleteBookmarkModalComponent],
  templateUrl: './bookmarks-view.component.html',
  styleUrls: ['./bookmarks-view.component.scss'],
})
export class BookmarksViewComponent implements OnInit {
  private bookmarksService = inject(BookmarksService);
  private storage = inject(StorageService);

  @Input() bookmarks: Bookmark[] = [];
  @Input() showNewButton = false;
  @Input() pageTitle = '';
  @Input() defaultFolder?: string;
  @Input() defaultTag?: string;
  @Input() defaultFavorite = false;

  @Output() delete = new EventEmitter<number>();
  @Output() created = new EventEmitter<void>();
  @Output() unfavorited = new EventEmitter<Bookmark>();
  @Output() toggledFavorite = new EventEmitter<Bookmark>();

  viewMode: 'grid' | 'list' = 'grid';

  showModal = false;
  editingBookmark?: Bookmark;

  showDeleteModal = false;
  bookmarkToDelete?: Bookmark;

  ngOnInit(): void {
    const savedMode = this.storage.getString(VIEW_MODE_KEY) as 'grid' | 'list' | null;
    if (savedMode === 'grid' || savedMode === 'list') {
      this.viewMode = savedMode;
    }
  }

  setViewMode(mode: 'grid' | 'list'): void {
    this.viewMode = mode;
    this.storage.setString(VIEW_MODE_KEY, mode);
  }

  openModal(): void {
    this.editingBookmark = undefined;
    this.showModal = true;
  }

  openEditModal(bookmark: Bookmark): void {
    this.editingBookmark = bookmark;
    this.showModal = true;
  }

  closeModal(): void {
    this.showModal = false;
    this.editingBookmark = undefined;
    this.created.emit();
  }

  openDeleteModal(bookmark: Bookmark): void {
    this.bookmarkToDelete = bookmark;
    this.showDeleteModal = true;
  }

  closeDeleteModal(): void {
    this.showDeleteModal = false;
    this.bookmarkToDelete = undefined;
  }

  confirmDelete(): void {
    if (this.bookmarkToDelete) {
      this.delete.emit(this.bookmarkToDelete.id);
      this.closeDeleteModal();
    }
  }

  /**
   * Toggles favorite and emits the updated bookmark.
   * The parent component handles the actual array update via toggledFavorite event.
   */
  onToggleFavorite(bookmark: Bookmark): void {
    this.bookmarksService.toggleFavorite(bookmark.id).subscribe({
      next: () => {
        const updated: Bookmark = { ...bookmark, isFavorite: !bookmark.isFavorite };
        // Let the parent handle the update - no direct DOM mutation
        this.toggledFavorite.emit(updated);
        if (!updated.isFavorite) {
          this.unfavorited.emit(updated);
        }
      },
      error: (err) => console.error('Failed to toggle favorite', err),
    });
  }

  getIconUrl(b: Bookmark): string {
    return b.iconUrl || getFaviconUrl(b.url);
  }

  trackByBookmarkId(_index: number, b: Bookmark): number {
    return b.id;
  }

  trackByIndex(index: number): number {
    return index;
  }
}