import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-delete-bookmark-modal',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './delete-bookmark-modal.component.html',
  styleUrls: ['./delete-bookmark-modal.component.scss'],
})
export class DeleteBookmarkModalComponent {
  @Input() title: string = '';
  @Output() confirm = new EventEmitter<void>();
  @Output() cancel = new EventEmitter<void>();
}