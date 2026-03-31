import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { Folder } from '../../models/folder.model';
import { AuthService } from './auth.service';

@Injectable({
    providedIn: 'root',
})
export class FolderService {
    private http = inject(HttpClient);
    private authService = inject(AuthService);
    private apiUrl = '/api/Folder';

    private foldersSubject = new BehaviorSubject<Folder[]>([]);
    folders$ = this.foldersSubject.asObservable();

    loadFolders(): void {
        const userId = this.authService.getUserId();
        this.http.get<Folder[]>(`${this.apiUrl}/user/${userId}`).subscribe({
            next: (folders) => this.foldersSubject.next(folders),
            error: (err) => console.error('Failed to load folders', err),
        });
    }

    createFolder(data: { name: string; userId: number }): Observable<Folder> {
        return this.http.post<Folder>(this.apiUrl, data).pipe(
            tap(() => this.loadFolders())
        );
    }

    updateFolder(id: number, data: { name: string; userId: number }): Observable<Folder> {
        return this.http.put<Folder>(`${this.apiUrl}/${id}`, data).pipe(
            tap(() => this.loadFolders())
        );
    }

    deleteFolder(id: number): Observable<any> {
        return this.http.delete(`${this.apiUrl}/${id}`).pipe(
            tap(() => this.loadFolders())
        );
    }
}
