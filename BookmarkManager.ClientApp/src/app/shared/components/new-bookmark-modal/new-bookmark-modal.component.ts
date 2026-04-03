import { Component, ElementRef, EventEmitter, inject, Input, OnInit, Output, DestroyRef, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormsModule, FormBuilder, Validators } from '@angular/forms';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { AuthService } from '../../../core/services/auth.service';
import { FolderService } from '../../../core/services/folder.service';
import { TagService } from '../../../core/services/tag.service';
import { Bookmark } from '../../../models/bookmark.model';
import { BookmarksService } from '../../../core/services/bookmarks.service';
import { getFaviconUrl } from '../../utils/url.utils';

@Component({
  selector: 'app-new-bookmark-modal',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule],
  templateUrl: './new-bookmark-modal.component.html',
  styleUrls: ['./new-bookmark-modal.component.scss'],
})
export class NewBookmarkModalComponent implements OnInit {
  /** Pass a bookmark to switch to Edit mode. Leave undefined for Create mode. */
  @Input() bookmark?: Bookmark;
  @Input() defaultFolder?: string;
  @Input() defaultTag?: string;
  @Input() defaultFavorite = false;

  @Output() close = new EventEmitter<void>();
  @Output() created = new EventEmitter<Bookmark>();

  private folderService = inject(FolderService);
  private tagService = inject(TagService);
  private authService = inject(AuthService);
  private destroyRef = inject(DestroyRef);
  folders$ = this.folderService.folders$;

  private formBuilder = inject(FormBuilder);
  private bookmarksService = inject(BookmarksService);

  form = this.formBuilder.group({
    url: [''],
    title: ['', Validators.required],
    folder: [''],
    tags: [''],
  });

  tagsList: string[] = [];
  showFolderDropdown = false;
  isCreatingFolder = false;
  newFolderName = '';
  faviconUrl = '';

  existingTags: string[] = [];
  tagSuggestions: string[] = [];
  showTagSuggestions = false;
  tagInputValue = '';
  isFavorite = false;

  @ViewChild('newFolderInput') newFolderInput!: ElementRef<HTMLInputElement>;

  get isEditMode(): boolean {
    return !!this.bookmark;
  }

  get modalTitle(): string {
    return this.isEditMode ? 'Edit Bookmark' : 'New Bookmark';
  }

  get submitLabel(): string {
    return this.isEditMode ? 'Update Bookmark' : 'Save Bookmark';
  }

  ngOnInit(): void {
    if (this.bookmark) {
      this.form.patchValue({
        url: this.bookmark.url ?? '',
        title: this.bookmark.title,
        folder: this.bookmark.folderName,
      });
      this.tagsList = [...this.bookmark.tags];
      this.isFavorite = this.bookmark.isFavorite;
      this.onUrlChange(this.bookmark.url ?? '');
    } else {
      if (this.defaultFolder) {
        this.form.patchValue({ folder: this.defaultFolder });
      }
      if (this.defaultTag) {
        this.tagsList = [this.defaultTag];
      }
      if (this.defaultFavorite) {
        this.isFavorite = true;
      }
    }

    this.loadFolders();
    this.loadExistingTags();
  }

  loadFolders() {
    this.folderService.loadFolders();
  }

  loadExistingTags() {
    this.tagService.tags$
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (tags) => (this.existingTags = tags.map((t) => t.name)),
      });
    this.tagService.loadTags();
  }

  onUrlChange(url: string): void {
    this.faviconUrl = getFaviconUrl(url);
  }

  onTagInput(event: Event): void {
    const input = event.target as HTMLInputElement;
    let value = input.value;
    if (value && !value.startsWith('#')) {
      value = '#' + value;
      input.value = value;
    }
    this.tagInputValue = value;
    const query = value.replace(/^#/, '').toLowerCase();
    if (query) {
      this.tagSuggestions = this.existingTags.filter(
        (t) => t.toLowerCase().includes(query) && !this.tagsList.includes(t)
      );
      this.showTagSuggestions = this.tagSuggestions.length > 0;
    } else {
      this.tagSuggestions = [];
      this.showTagSuggestions = false;
    }
  }

  addTag(event: Event): void {
    event.preventDefault();
    const input = event.target as HTMLInputElement;
    let value = input.value.trim().replace(/,$/, '').replace(/^#/, '');
    if (value && !this.tagsList.includes(value)) {
      this.tagsList.push(value);
    }
    input.value = '';
    this.tagInputValue = '';
    this.tagSuggestions = [];
    this.showTagSuggestions = false;
  }

  selectTagSuggestion(tag: string): void {
    if (!this.tagsList.includes(tag)) {
      this.tagsList.push(tag);
    }
    this.tagInputValue = '';
    this.tagSuggestions = [];
    this.showTagSuggestions = false;
  }

  removeTag(index: number): void {
    this.tagsList.splice(index, 1);
  }

  toggleFolderDropdown(): void {
    this.showFolderDropdown = !this.showFolderDropdown;
  }

  selectFolder(folder: string): void {
    this.form.patchValue({ folder });
    this.showFolderDropdown = false;
  }

  startCreateFolder(): void {
    this.isCreatingFolder = true;
    this.newFolderName = '';
    setTimeout(() => this.newFolderInput?.nativeElement.focus());
  }

  cancelCreateFolder(): void {
    this.isCreatingFolder = false;
    this.newFolderName = '';
  }

  createFolder(): void {
    const name = this.newFolderName.trim();
    if (!name) return;

    this.folderService.createFolder({ name }).subscribe({
      next: (folder) => {
        this.isCreatingFolder = false;
        this.newFolderName = '';
        this.form.patchValue({ folder: folder.name });
        this.showFolderDropdown = false;
      },
      error: (err) => console.error('Failed to create folder', err),
    });
  }

  submit(): void {
    if (this.form.valid) {
      const v = this.form.value as { url?: string; title: string; folder?: string };
      const iconUrl = getFaviconUrl(v.url ?? '');
      const payload = {
        title: v.title,
        url: v.url ?? '',
        folderName: v.folder,
        tags: this.tagsList,
        iconUrl,
        isFavorite: this.isFavorite,
      };

      if (this.isEditMode && this.bookmark) {
        this.bookmarksService
          .updateBookmark(this.bookmark.id, payload)
          .subscribe({
            next: (bm) => {
              this.created.emit(bm);
              this.closeModal();
            },
            error: (err) => console.error('Failed to update bookmark', err),
          });
      } else {
        this.bookmarksService
          .createBookmark(payload)
          .subscribe({
            next: (bm) => {
              this.created.emit(bm);
              this.closeModal();
            },
            error: (err) => console.error('Failed to create bookmark', err),
          });
      }
    }
  }

  closeModal(): void {
    this.close.emit();
  }

  trackByFolderId(index: number, folder: { id: number }): number {
    return folder.id;
  }

  trackByIndex(index: number): number {
    return index;
  }
}
