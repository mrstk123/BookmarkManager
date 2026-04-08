import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, switchMap, tap } from 'rxjs';
import { Bookmark } from '../../models/bookmark.model';
import { FolderService } from './folder.service';
import { TagService } from './tag.service';

@Injectable({ providedIn: 'root' })
export class BookmarksService {
  private http = inject(HttpClient);
  private folderService = inject(FolderService);
  private tagService = inject(TagService);
  private apiUrl = '/api/Bookmark';

  private refresh$ = new BehaviorSubject<void>(undefined);

  getBookmarks(): Observable<Bookmark[]> {
    return this.refresh$.pipe(
      switchMap(() => this.http.get<Bookmark[]>(this.apiUrl))
    );
  }

  getFavorites(): Observable<Bookmark[]> {
    return this.refresh$.pipe(
      switchMap(() => this.http.get<Bookmark[]>(`${this.apiUrl}/favorites`))
    );
  }

  getBookmarksByFolder(folderName: string): Observable<Bookmark[]> {
    return this.refresh$.pipe(
      switchMap(() => this.http.get<Bookmark[]>(`${this.apiUrl}?folderName=${encodeURIComponent(folderName)}`))
    );
  }

  getBookmarksByTag(tagName: string): Observable<Bookmark[]> {
    return this.refresh$.pipe(
      switchMap(() => this.http.get<Bookmark[]>(`${this.apiUrl}?tagName=${encodeURIComponent(tagName)}`))
    );
  }

  refresh(): void {
    this.refresh$.next();
  }

  createBookmark(data: { title: string; url: string; folderName?: string; tags: string[]; iconUrl?: string; isFavorite?: boolean }): Observable<Bookmark> {
    return this.http.post<Bookmark>(this.apiUrl, data).pipe(
      tap(() => {
        this.refresh();
        this.folderService.loadFolders();
        this.tagService.loadTags();
      })
    );
  }

  updateBookmark(id: number, data: { title: string; url: string; folderName?: string; tags: string[]; iconUrl?: string; isFavorite?: boolean }): Observable<Bookmark> {
    return this.http.put<Bookmark>(`${this.apiUrl}/${id}`, data).pipe(
      tap(() => {
        this.refresh();
        this.folderService.loadFolders();
        this.tagService.loadTags();
      })
    );
  }

  deleteBookmark(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`).pipe(
      tap(() => this.refresh())
    );
  }

  toggleFavorite(id: number): Observable<any> {
    return this.http.put(`${this.apiUrl}/${id}/toggle-favorite`, null);
  }

  recordVisit(id: number): Observable<any> {
    return this.http.put(`${this.apiUrl}/${id}/visit`, null);
  }
}
