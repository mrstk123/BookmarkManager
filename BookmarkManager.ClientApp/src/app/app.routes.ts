import { Routes } from '@angular/router';
import { Component } from '@angular/core';
import { authGuard } from './core/guards/auth-guard';
import { ForgotPasswordComponent } from './features/auth/forgot-password/forgot-password.component';
import { LoginComponent } from './features/auth/login/login.component';
import { RegisterComponent } from './features/auth/register/register.component';
import { BookmarksComponent } from './features/bookmarks/pages/bookmarks/bookmarks.component';
import { FavoritesComponent } from './features/bookmarks/pages/favorites/favorites.component';
import { TagBookmarksComponent } from './features/bookmarks/pages/tag-bookmarks/tag-bookmarks.component';
import { ProfileComponent } from './features/profile/pages/profile/profile.component';
import { Layout } from './layout/layout.component';
import { FolderBookmarksComponent } from './features/bookmarks/pages/folder-bookmarks/folder-bookmarks.component';

@Component({
  template: `<h2>{{title}}</h2>`,
  standalone: true
})
export class DummyComponent {
  title = 'Page Content Placeholder';
}

export const routes: Routes = [
  { path: '', redirectTo: 'bookmarks', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  { path: 'sign-up', component: RegisterComponent },
  { path: 'forgot-password', component: ForgotPasswordComponent },
  {
    path: '',
    component: Layout,
    canActivate: [authGuard],
    children: [
      { path: 'bookmarks', component: BookmarksComponent },
      { path: 'favorites', component: FavoritesComponent },
      { path: 'profile', component: ProfileComponent },
      { path: 'folder/:name', component: FolderBookmarksComponent },
      { path: 'tag/:name', component: TagBookmarksComponent },
      { path: 'work', component: DummyComponent },
      { path: 'design-inspiration', component: DummyComponent },
      { path: 'tags', component: DummyComponent }
    ]
  },

  // wildcard route for 404
  { path: '**', redirectTo: '/bookmarks' }
];
