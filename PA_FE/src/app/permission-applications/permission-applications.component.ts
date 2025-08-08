import { Component, OnInit } from '@angular/core';
import { PermissionApplication } from '../../_models/PermissionApplication';
import { PermissionApplicationService } from '../../_services/permission-application.service';
import { EmployeeService } from '../../_services/employee.service';
import { LicenceService } from '../../_services/licence.service';
import { Employee } from '../../_models/Employee';
import { Licence } from '../../_models/Licence';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-permission-applications',
  templateUrl: './permission-applications.component.html',
  styleUrls: ['./permission-applications.component.css'],
  standalone: true,
  imports: [CommonModule, FormsModule],
})
export class PermissionApplicationsComponent implements OnInit {
  applications: PermissionApplication[] = [];
  employees: Employee[] = [];
  licences: Licence[] = [];
  newApp: Partial<PermissionApplication> = { isGrant: true };
  errorMessage = '';

  constructor(
    private appService: PermissionApplicationService,
    private employeeService: EmployeeService,
    private licenceService: LicenceService
  ) {}

  ngOnInit(): void {
    this.loadApplications();
    this.employeeService.getEmployees().subscribe((e) => (this.employees = e));
    this.licenceService.getLicences().subscribe((l) => (this.licences = l));
  }

  loadApplications(): void {
    this.appService
      .getApplications()
      .subscribe((apps) => (this.applications = apps));
  }

  createApplication(): void {
    if (!this.newApp.employeeId || !this.newApp.licenceId) return;

    const request: PermissionApplication = {
      id: 0,
      employeeId: this.newApp.employeeId,
      licenceId: this.newApp.licenceId,
      isGrant: this.newApp.isGrant ?? true,
      employeeName: '',
      licenceName: '',
    };

    this.appService.createApplication(request).subscribe({
      next: (app) => {
        this.applications.push(app);
        this.newApp = { isGrant: true };
        this.errorMessage = '';
      },
      error: (err) => {
        this.errorMessage = err.error;
      },
    });
  }

  close(app: PermissionApplication): void {
    if (!confirm('Close this application?')) return;
    this.appService.closeApplication(app.id).subscribe(() => {
      this.applications = this.applications.filter((a) => a.id !== app.id);
    });
  }

  trackById(_index: number, item: PermissionApplication): number {
    return item.id;
  }
}
