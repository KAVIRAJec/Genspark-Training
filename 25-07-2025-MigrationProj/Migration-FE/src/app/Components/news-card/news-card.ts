import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NewsDto } from '../../models/news.model';
import { RouterModule } from '@angular/router';
import { Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'app-news-card',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './news-card.html',
  styleUrl: './news-card.css'
})
export class NewsCardComponent {
  @Input() news!: NewsDto;
  @Output() readMore = new EventEmitter<NewsDto>();
  onReadMore() {
    this.readMore.emit(this.news);
  }
}
