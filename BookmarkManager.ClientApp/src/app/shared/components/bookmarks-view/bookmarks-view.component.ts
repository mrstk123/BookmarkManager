import { Component, EventEmitter, Input, Output, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { DeleteBookmarkModalComponent } from '../../../features/bookmarks/modals/delete-bookmark-modal/delete-bookmark-modal.component';
import { NewBookmarkModalComponent } from '../../../features/bookmarks/modals/new-bookmark-modal/new-bookmark-modal.component';
import { Bookmark } from '../../../models/bookmark.model';

const VIEW_MODE_KEY = 'bookmarks-view-mode';

@Component({
  selector: 'app-bookmarks-view',
  standalone: true,
  imports: [CommonModule, RouterModule, NewBookmarkModalComponent, DeleteBookmarkModalComponent],
  templateUrl: './bookmarks-view.component.html',
  styleUrls: ['./bookmarks-view.component.scss'],
})
export class BookmarksViewComponent implements OnInit {
  @Input() bookmarks: Bookmark[] = [];
  @Input() showNewButton = false;
  @Input() pageTitle = '';

  @Output() edit = new EventEmitter<Bookmark>();
  @Output() delete = new EventEmitter<number>();
  @Output() toggleFavorite = new EventEmitter<number>();
  @Output() created = new EventEmitter<void>();

  viewMode: 'grid' | 'list' = 'grid';

  showModal = false;
  editingBookmark?: Bookmark;

  showDeleteModal = false;
  bookmarkToDelete?: Bookmark;

  ngOnInit(): void {
    const saved = localStorage.getItem(VIEW_MODE_KEY);
    if (saved === 'grid' || saved === 'list') {
      this.viewMode = saved;
    }
  }

  setViewMode(mode: 'grid' | 'list'): void {
    this.viewMode = mode;
    localStorage.setItem(VIEW_MODE_KEY, mode);
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

  onToggleFavorite(bookmark: Bookmark): void {
    this.toggleFavorite.emit(bookmark.id);
  }

  getIconUrl(b: Bookmark): string {
    if (b.iconUrl) return b.iconUrl;
    const domain = this.extractDomain(b.url);
    return domain ? `https://www.google.com/s2/favicons?domain=${domain}&sz=64` : '';
  }

  private extractDomain(url: string): string {
    if (!url) return '';
    try {
      const u = url.startsWith('http') ? url : `https://${url}`;
      return new URL(u).hostname;
    } catch {
      return '';
    }
  }
}
