import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Employee } from '../../_models/Employee';
import { History } from '../../_models/History';
import { EmployeeService } from '../../_services/employee.service';
import { HistoryService } from '../../_services/history.service';

@Component({
  selector: 'app-history',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './history.component.html',
  styleUrls: ['./history.component.css'],
})
export class HistoryComponent implements OnInit {
  history: History[] = [];
  employees: Employee[] = [];
  selectedEmployeeName = '';
  constructor(
    private historyService: HistoryService,
    private employeeService: EmployeeService
  ) {}

  ngOnInit(): void {
    this.employeeService
      .getEmployees()
      .subscribe((e) => (this.employees = e));
    this.onEmployeeChange();
  }

  onEmployeeChange(): void {
    const name = this.selectedEmployeeName || undefined;
    this.historyService.getHistory(name).subscribe((r) => (this.history = r));
  }

  trackById(_index: number, item: History): number {
    return item.id;
  }
}
