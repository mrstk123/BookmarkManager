import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth-guard';

export const routes: Routes = [
  { path: '', redirectTo: 'bookmarks', pathMatch: 'full' },
  { path: 'login', loadComponent: () => import('./features/auth/login/login.component').then(m => m.LoginComponent) },
  { path: 'sign-up', loadComponent: () => import('./features/auth/register/register.component').then(m => m.RegisterComponent) },
  { path: 'forgot-password', loadComponent: () => import('./features/auth/forgot-password/forgot-password.component').then(m => m.ForgotPasswordComponent) },
  {
    path: '',
    loadComponent: () => import('./layout/layout.component').then(m => m.LayoutComponent),
    canActivate: [authGuard],
    children: [
      { path: 'bookmarks', loadComponent: () => import('./features/bookmarks/all-bookmarks/bookmarks.component').then(m => m.BookmarksComponent) },
      { path: 'favorites', loadComponent: () => import('./features/bookmarks/favorites/favorites.component').then(m => m.FavoritesComponent) },
      { path: 'profile', loadComponent: () => import('./features/profile/profile.component').then(m => m.ProfileComponent) },
      { path: 'folder/:name', loadComponent: () => import('./features/bookmarks/folder-bookmarks/folder-bookmarks.component').then(m => m.FolderBookmarksComponent) },
      { path: 'tag/:name', loadComponent: () => import('./features/bookmarks/tag-bookmarks/tag-bookmarks.component').then(m => m.TagBookmarksComponent) },
    ]
  },

  // wildcard route for 404
  { path: '**', redirectTo: '/bookmarks' }
];
