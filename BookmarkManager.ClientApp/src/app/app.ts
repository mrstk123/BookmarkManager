import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { GlobalLoadingComponent } from './core/components/global-loading/global-loading.component';

@Component({
    selector: 'app-root',
    imports: [RouterOutlet, GlobalLoadingComponent],
    templateUrl: './app.html',
    styleUrls: []
})
export class App {
    protected readonly title = signal('BookmarkManager.ClientApp');
}
