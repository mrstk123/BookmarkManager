import { Injectable } from '@angular/core';

/**
 * Abstraction over browser localStorage for testability and SSR compatibility.
 */
@Injectable({
  providedIn: 'root',
})
export class StorageService {
  /**
   * Retrieves a value from storage and parses it as JSON if possible.
   */
  get<T>(key: string): T | null {
    if (typeof window === 'undefined') return null;
    const raw = localStorage.getItem(key);
    if (raw === null) return null;
    try {
      return JSON.parse(raw) as T;
    } catch {
      return raw as unknown as T;
    }
  }

  /**
   * Stores a value in storage as JSON string.
   */
  set<T>(key: string, value: T): void {
    if (typeof window === 'undefined') return;
    localStorage.setItem(key, JSON.stringify(value));
  }

  /**
   * Stores a raw string value in storage.
   */
  setString(key: string, value: string): void {
    if (typeof window === 'undefined') return;
    localStorage.setItem(key, value);
  }

  /**
   * Retrieves a raw string value from storage.
   */
  getString(key: string): string | null {
    if (typeof window === 'undefined') return null;
    return localStorage.getItem(key);
  }

  /**
   * Removes a value from storage.
   */
  remove(key: string): void {
    if (typeof window === 'undefined') return;
    localStorage.removeItem(key);
  }

  /**
   * Clears all values from storage.
   */
  clear(): void {
    if (typeof window === 'undefined') return;
    localStorage.clear();
  }
}