import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { Tag } from '../../models/tag.model';
import { AuthService } from './auth.service';

@Injectable({
    providedIn: 'root',
})
export class TagService {
    private http = inject(HttpClient);
    private authService = inject(AuthService);
    private apiUrl = '/api/Tag';

    private tagsSubject = new BehaviorSubject<Tag[]>([]);
    tags$ = this.tagsSubject.asObservable();

    loadTags(): void {
        const userId = this.authService.getUserId();
        this.http.get<Tag[]>(`${this.apiUrl}/user/${userId}`).subscribe({
            next: (tags) => this.tagsSubject.next(tags),
        });
    }

    createTag(data: { name: string; userId: number }): Observable<Tag> {
        return this.http.post<Tag>(this.apiUrl, data).pipe(
            tap(() => this.loadTags())
        );
    }

    updateTag(id: number, data: { name: string; userId: number }): Observable<Tag> {
        return this.http.put<Tag>(`${this.apiUrl}/${id}`, data).pipe(
            tap(() => this.loadTags())
        );
    }

    deleteTag(id: number): Observable<any> {
        return this.http.delete(`${this.apiUrl}/${id}`).pipe(
            tap(() => this.loadTags())
        );
    }
}
