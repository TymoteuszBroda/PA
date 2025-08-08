import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { Report } from '../../_models/Report';
import { ReportService } from '../../_services/report.service';

@Component({
  selector: 'app-reports',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './reports.component.html',
  styleUrls: ['./reports.component.css'],
})
export class ReportsComponent implements OnInit {
  reports: Report[] = [];

  constructor(private reportService: ReportService) {}

  ngOnInit(): void {
    this.reportService.getReports().subscribe((r) => (this.reports = r));
  }

  trackById(_index: number, item: Report): number {
    return item.id;
  }
}
