import { CommonModule } from '@angular/common';
import { Component, OnInit, inject, DestroyRef, ChangeDetectorRef, ElementRef, ViewChild } from '@angular/core';
import { RouterModule } from '@angular/router';
import { Router, NavigationEnd } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { filter, debounceTime, distinctUntilChanged, Subject } from 'rxjs';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { AuthService } from '../core/services/auth.service';
import { FolderService } from '../core/services/folder.service';
import { SearchService } from '../core/services/search.service';
import { TagService } from '../core/services/tag.service';
import { Folder } from '../models/folder.model';
import { Tag } from '../models/tag.model';

const MOBILE_BREAKPOINT = 768;

@Component({
    selector: 'app-layout',
    imports: [RouterModule, CommonModule, FormsModule],
    templateUrl: './layout.component.html',
    styleUrls: ['./layout.component.scss'],
})
export class LayoutComponent implements OnInit {
    isSidebarClosed = false;
    isMobileSidebarOpen = false;
    isDarkMode = false;
    pageTitle: string = 'Dashboard';
    private router = inject(Router);
    private folderService = inject(FolderService);
    private tagService = inject(TagService);
    private searchService = inject(SearchService);
    private authService = inject(AuthService);
    private destroyRef = inject(DestroyRef);
    private cdr = inject(ChangeDetectorRef);

    folders: Folder[] = [];
    isCreatingFolder = false;
    newFolderName = '';
    editingFolderId: number | null = null;
    editingFolderName = '';

    tags: Tag[] = [];
    isCreatingTag = false;
    newTagName = '';
    editingTagId: number | null = null;
    editingTagName = '';

    deleteConfirmFolder: Folder | null = null;
    deleteConfirmTag: Tag | null = null;

    @ViewChild('folderInput') folderInput!: ElementRef<HTMLInputElement>;
    @ViewChild('tagInput') tagInput!: ElementRef<HTMLInputElement>;
    @ViewChild('editFolderInput') editFolderInput!: ElementRef<HTMLInputElement>;
    @ViewChild('editTagInput') editTagInput!: ElementRef<HTMLInputElement>;

    searchQuery = '';
    private searchSubject = new Subject<string>();

    ngOnInit(): void {
        // Initialize dark mode from saved preference
        const savedTheme = localStorage.getItem('theme');
        this.isDarkMode = savedTheme === 'dark';
        document.documentElement.setAttribute('data-theme', this.isDarkMode ? 'dark' : 'light');

        this.router.events
            .pipe(filter((e): e is NavigationEnd => e instanceof NavigationEnd), takeUntilDestroyed(this.destroyRef))
            .subscribe((e: NavigationEnd) => {
                this.setTitleFromUrl(e.urlAfterRedirects);
                this.closeMobileSidebar();
                this.cdr.markForCheck();
            });
        this.setTitleFromUrl(this.router.url);

        this.folderService.folders$
            .pipe(takeUntilDestroyed(this.destroyRef))
            .subscribe(folders => { this.folders = folders; this.cdr.markForCheck(); });
        this.tagService.tags$
            .pipe(takeUntilDestroyed(this.destroyRef))
            .subscribe(tags => { this.tags = tags; this.cdr.markForCheck(); });
        this.folderService.loadFolders();
        this.tagService.loadTags();

        this.searchSubject.pipe(
            debounceTime(300),
            distinctUntilChanged(),
            takeUntilDestroyed(this.destroyRef)
        ).subscribe(query => {
            this.searchService.setQuery(query);
        });
    }

    onSearchInput(query: string): void {
        this.searchSubject.next(query);
    }

    clearSearch(): void {
        this.searchQuery = '';
        this.searchSubject.next('');
    }

    trackByFolderId(index: number, folder: Folder): number {
        return folder.id;
    }

    trackByTagId(index: number, tag: Tag): number {
        return tag.id;
    }

    startCreateFolder(): void {
        this.isCreatingFolder = true;
        this.newFolderName = '';
        setTimeout(() => this.folderInput?.nativeElement.focus());
    }

    cancelCreateFolder(): void {
        this.isCreatingFolder = false;
        this.newFolderName = '';
    }

    createFolder(): void {
        const name = this.newFolderName.trim();
        if (!name) return;

        this.folderService.createFolder({ name }).subscribe({
            next: () => {
                this.isCreatingFolder = false;
                this.newFolderName = '';
                this.cdr.markForCheck();
            },
            error: (err) => console.error('Failed to create folder', err),
        });
    }

    startEditFolder(folder: Folder): void {
        this.editingFolderId = folder.id;
        this.editingFolderName = folder.name;
        setTimeout(() => this.editFolderInput?.nativeElement.focus());
    }

    cancelEditFolder(): void {
        this.editingFolderId = null;
        this.editingFolderName = '';
    }

    saveEditFolder(): void {
        if (!this.editingFolderId) return;
        const name = this.editingFolderName.trim();
        if (!name) return;

        const folder = this.folders.find(f => f.id === this.editingFolderId);
        const oldName = folder?.name;

        this.folderService.updateFolder(this.editingFolderId, { name }).subscribe({
            next: () => {
                this.editingFolderId = null;
                this.editingFolderName = '';
                if (oldName && this.router.url.startsWith(`/folder/${encodeURIComponent(oldName)}`)) {
                    this.router.navigate(['/folder', name]);
                }
                this.cdr.markForCheck();
            },
            error: (err) => console.error('Failed to update folder', err),
        });
    }

    deleteFolder(folder: Folder): void {
        this.deleteConfirmFolder = folder;
    }

    confirmDeleteFolder(): void {
        if (!this.deleteConfirmFolder) return;
        const folder = this.deleteConfirmFolder;
        this.deleteConfirmFolder = null;
        this.folderService.deleteFolder(folder.id).subscribe({
            next: () => {
                if (this.router.url.startsWith(`/folder/${encodeURIComponent(folder.name)}`)) {
                    this.router.navigate(['/bookmarks']);
                }
                this.cdr.markForCheck();
            },
            error: (err) => console.error('Failed to delete folder', err),
        });
    }

    startCreateTag(): void {
        this.isCreatingTag = true;
        this.newTagName = '';
        setTimeout(() => this.tagInput?.nativeElement.focus());
    }

    cancelCreateTag(): void {
        this.isCreatingTag = false;
        this.newTagName = '';
    }

    createTag(): void {
        const name = this.newTagName.trim();
        if (!name) return;

        this.tagService.createTag({ name }).subscribe({
            next: () => {
                this.isCreatingTag = false;
                this.newTagName = '';
                this.cdr.markForCheck();
            },
            error: (err) => console.error('Failed to create tag', err),
        });
    }

    startEditTag(tag: Tag): void {
        this.editingTagId = tag.id;
        this.editingTagName = tag.name;
        setTimeout(() => this.editTagInput?.nativeElement.focus());
    }

    cancelEditTag(): void {
        this.editingTagId = null;
        this.editingTagName = '';
    }

    saveEditTag(): void {
        if (!this.editingTagId) return;
        const name = this.editingTagName.trim();
        if (!name) return;

        const tag = this.tags.find(t => t.id === this.editingTagId);
        const oldName = tag?.name;

        this.tagService.updateTag(this.editingTagId, { name }).subscribe({
            next: () => {
                this.editingTagId = null;
                this.editingTagName = '';
                if (oldName && this.router.url.startsWith(`/tag/${encodeURIComponent(oldName)}`)) {
                    this.router.navigate(['/tag', name]);
                }
                this.cdr.markForCheck();
            },
            error: (err) => console.error('Failed to update tag', err),
        });
    }

    deleteTag(tag: Tag): void {
        this.deleteConfirmTag = tag;
    }

    confirmDeleteTag(): void {
        if (!this.deleteConfirmTag) return;
        const tag = this.deleteConfirmTag;
        this.deleteConfirmTag = null;
        this.tagService.deleteTag(tag.id).subscribe({
            next: () => {
                if (this.router.url.startsWith(`/tag/${encodeURIComponent(tag.name)}`)) {
                    this.router.navigate(['/bookmarks']);
                }
                this.cdr.markForCheck();
            },
            error: (err) => console.error('Failed to delete tag', err),
        });
    }

    cancelDelete(): void {
        this.deleteConfirmFolder = null;
        this.deleteConfirmTag = null;
    }

    setTitleFromUrl(url: string): void {
        if (!url) {
            this.pageTitle = 'Dashboard';
            return;
        }
        if (url.includes('/bookmarks')) {
            this.pageTitle = 'All Bookmarks';
        } else if (url.includes('/login')) {
            this.pageTitle = 'Login';
        } else if (url.includes('/sign-up')) {
            this.pageTitle = 'Sign Up';
        } else if (url.includes('/forgot-password')) {
            this.pageTitle = 'Forgot Password';
        } else if (url.includes('/tags')) {
            this.pageTitle = 'Tags';
        } else if (url.includes('/favorites')) {
            this.pageTitle = 'Favorites';
        } else {
            this.pageTitle = 'Dashboard';
        }
    }

    toggleSidebar() {
        if (window.innerWidth <= MOBILE_BREAKPOINT) {
            this.isMobileSidebarOpen = !this.isMobileSidebarOpen;
        } else {
            this.isSidebarClosed = !this.isSidebarClosed;
        }
    }

    toggleMobileSidebar() {
        this.isMobileSidebarOpen = !this.isMobileSidebarOpen;
    }

    closeMobileSidebar() {
        this.isMobileSidebarOpen = false;
    }

    toggleDarkMode(): void {
        this.isDarkMode = !this.isDarkMode;
        const theme = this.isDarkMode ? 'dark' : 'light';
        document.documentElement.setAttribute('data-theme', theme);
        localStorage.setItem('theme', theme);
    }

    logout() {
        this.authService.logout();
        this.router.navigate(['/login']);
    }
}
