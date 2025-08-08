import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { History } from '../../_models/History';
import { HistoryService } from '../../_services/history.service';

@Component({
  selector: 'app-history',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './history.component.html',
  styleUrls: ['./history.component.css'],
})
export class HistoryComponent implements OnInit {
  history: History[] = [];

  constructor(private historyService: HistoryService) {}

  ngOnInit(): void {
    this.historyService.getHistory().subscribe((r) => (this.history = r));
  }

  trackById(_index: number, item: History): number {
    return item.id;
  }
}
