import { CommonModule } from '@angular/common';
import { Component, OnInit, inject } from '@angular/core';
import { RouterModule } from '@angular/router';
import { Router, NavigationEnd } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { filter, debounceTime, distinctUntilChanged, Subject } from 'rxjs';
import { AuthService } from '../core/services/auth.service';
import { FolderService } from '../core/services/folder.service';
import { SearchService } from '../core/services/search.service';
import { TagService } from '../core/services/tag.service';
import { Folder } from '../models/folder.model';
import { Tag } from '../models/tag.model';

@Component({
    selector: 'app-layout',
    imports: [RouterModule, CommonModule, FormsModule],
    templateUrl: './layout.component.html',
    styleUrl: './layout.component.scss',
})
export class Layout implements OnInit {
    isSidebarClosed = false;
    isMobileSidebarOpen = false;
    pageTitle: string = 'Dashboard';
    private router = inject(Router);
    private folderService = inject(FolderService);
    private tagService = inject(TagService);
    private searchService = inject(SearchService);
    private authService = inject(AuthService);

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

    searchQuery = '';
    private searchSubject = new Subject<string>();

    ngOnInit(): void {
        this.router.events
            .pipe(filter((e): e is NavigationEnd => e instanceof NavigationEnd))
            .subscribe((e: NavigationEnd) => {
                this.setTitleFromUrl(e.urlAfterRedirects);
                this.closeMobileSidebar();
            });
        this.setTitleFromUrl(this.router.url);

        this.folderService.folders$.subscribe(folders => this.folders = folders);
        this.tagService.tags$.subscribe(tags => this.tags = tags);
        this.folderService.loadFolders();
        this.tagService.loadTags();

        this.searchSubject.pipe(
            debounceTime(300),
            distinctUntilChanged()
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

    startCreateFolder(): void {
        this.isCreatingFolder = true;
        this.newFolderName = '';
    }

    cancelCreateFolder(): void {
        this.isCreatingFolder = false;
        this.newFolderName = '';
    }

    createFolder(): void {
        const name = this.newFolderName.trim();
        if (!name) return;

        this.folderService.createFolder({ name, userId: this.authService.getUserId() }).subscribe({
            next: () => {
                this.isCreatingFolder = false;
                this.newFolderName = '';
            },
        });
    }

    startEditFolder(folder: Folder): void {
        this.editingFolderId = folder.id;
        this.editingFolderName = folder.name;
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

        this.folderService.updateFolder(this.editingFolderId, { name, userId: this.authService.getUserId() }).subscribe({
            next: () => {
                this.editingFolderId = null;
                this.editingFolderName = '';
                if (oldName && this.router.url.startsWith(`/folder/${encodeURIComponent(oldName)}`)) {
                    this.router.navigate(['/folder', name]);
                }
            }
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
            }
        });
    }

    startCreateTag(): void {
        this.isCreatingTag = true;
        this.newTagName = '';
    }

    cancelCreateTag(): void {
        this.isCreatingTag = false;
        this.newTagName = '';
    }

    createTag(): void {
        const name = this.newTagName.trim();
        if (!name) return;

        this.tagService.createTag({ name, userId: this.authService.getUserId() }).subscribe({
            next: () => {
                this.isCreatingTag = false;
                this.newTagName = '';
            },
        });
    }

    startEditTag(tag: Tag): void {
        this.editingTagId = tag.id;
        this.editingTagName = tag.name;
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

        this.tagService.updateTag(this.editingTagId, { name, userId: this.authService.getUserId() }).subscribe({
            next: () => {
                this.editingTagId = null;
                this.editingTagName = '';
                if (oldName && this.router.url.startsWith(`/tag/${encodeURIComponent(oldName)}`)) {
                    this.router.navigate(['/tag', name]);
                }
            }
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
            }
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
        if (window.innerWidth <= 768) {
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

    logout() {
        this.authService.logout();
        this.router.navigate(['/login']);
    }
}
