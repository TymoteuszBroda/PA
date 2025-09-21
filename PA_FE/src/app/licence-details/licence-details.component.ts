import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { LicenceService } from '../../_services/licence.service';
import { Licence } from '../../_models/Licence';
import { AssignLicenceDTO } from '../../_models/AssignLicenceDTO';
import { LicenceInstance } from '../../_models/LicenceInstance';

@Component({
  selector: 'app-licence-details',
  templateUrl: './licence-details.component.html',
  styleUrls: ['./licence-details.component.css'],
  imports: [CommonModule, RouterModule],
})
export class LicenceDetailsComponent implements OnInit {
  licence: Licence | null = null;
  assignedUsers: AssignLicenceDTO[] = [];
  instances: LicenceInstance[] = [];
  errorMessage = '';

  constructor(
    private licenceService: LicenceService,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      const id = params.get('id');
      if (id) {
        this.loadLicence(+id);
        this.loadAssignedUsers(+id);
        this.loadInstances(+id);
      }
    });
  }

  loadLicence(id: number): void {
    this.licenceService.getLicenceById(id).subscribe({
      next: licence => {
        this.licence = licence;
      },
      error: err => {
        console.error('Error loading licence', err);
        this.errorMessage = 'Failed to load licence details';
      },
    });
  }

  loadAssignedUsers(licenceId: number): void {
    this.licenceService.getEmployeesByLicenceId(licenceId).subscribe({
      next: users => {
        this.assignedUsers = users;
      },
      error: err => {
        console.error('Error loading users', err);
        this.errorMessage = `Failed to load users: ${err.message}`;
      },
    });
  }

  loadInstances(id: number): void {
    this.licenceService.getLicenceInstances(id).subscribe({
      next: instances => {
        this.instances = [...instances].sort((a, b) =>
          new Date(a.validTo).getTime() - new Date(b.validTo).getTime()
        );
      },
      error: err => {
        console.error('Error loading instances', err);
        this.instances = [];
      },
    });
  }

  deleteInstance(instanceId: number | undefined): void {
    if (!instanceId || !this.licence) return;
    this.licenceService.deleteLicenceInstanceById(instanceId).subscribe({
      next: () => {
        this.instances = this.instances.filter(i => i.id !== instanceId);
        this.licence!.availableLicences--;
        this.licence!.quantity--;
      },
      error: err => console.error('Error deleting instance', err)
    });
  }

  isInstanceExpiring(validTo: string | undefined): boolean {
    if (!validTo) {
      return false;
    }
    const now = new Date();
    const expiry = new Date(validTo);
    if (isNaN(expiry.getTime())) {
      return false;
    }
    if (expiry < now) {
      return true;
    }
    const twoWeeksAhead = new Date(now);
    twoWeeksAhead.setDate(twoWeeksAhead.getDate() + 14);
    return expiry <= twoWeeksAhead;
  }

  getInstanceStatus(validTo: string | undefined): string {
    if (!validTo) {
      return 'Unknown';
    }
    const expiry = new Date(validTo);
    if (isNaN(expiry.getTime())) {
      return 'Unknown';
    }
    const now = new Date();
    if (expiry < now) {
      return 'Expired';
    }
    const twoWeeksAhead = new Date(now);
    twoWeeksAhead.setDate(twoWeeksAhead.getDate() + 14);
    if (expiry <= twoWeeksAhead) {
      return 'Expires within two weeks';
    }
    return 'Active';
  }
}
