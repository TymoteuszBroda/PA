import { Component, OnInit } from '@angular/core';
import { Licence } from '../../_models/Licence';
import { LicenceInstance } from '../../_models/LicenceInstance';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { LicenceService } from '../../_services/licence.service';
@Component({
  selector: 'app-licence-table',
  imports: [CommonModule],
  templateUrl: './licence-table.component.html',
  styleUrl: './licence-table.component.css',
})
export class LicenceTableComponent implements OnInit {
  licences: Licence[] = [];
  expiryStatuses: { [key: number]: string } = {};

  constructor(private licenceService: LicenceService, private router: Router) {}

  ngOnInit() {
    this.licenceService.getLicences().subscribe((data: Licence[]) => {
      this.licences = data;
      for (const licence of data) {
        this.licenceService.getLicenceInstances(licence.id).subscribe({
          next: (instances: LicenceInstance[]) => {
            const validToDates = instances.map((inst) => inst.validTo);
            this.expiryStatuses[licence.id] = this.calculateExpiryStatus(
              validToDates
            );
          },
          error: () => {
            this.expiryStatuses[licence.id] = this.calculateExpiryStatus(
              licence.validTo
            );
          },
        });
      }
    });
  }
  deleteLicences(id: number): void {
    this.licenceService.deleteLicences(id).subscribe({
      next: (response) => {
        this.licences = this.licences.filter((e) => e.id !== id);
      },
      error: (err) => {
        console.error('Error deleting licence', err);
      },
    });
  }
  editLicence(id: number): void{
    this.router.navigate(['editLicence/', id]);
  }

  showLicenceDetails(id: number): void {
    this.router.navigate(['licenceDetails/', id]);
  }

  decreaseQuantity(licence: Licence): void {
    if (licence.availableLicences > 0) {
      this.licenceService.deleteLicenceInstance(licence.id).subscribe(() => {
        licence.availableLicences--;
        licence.quantity--;
      });
    }
  }

  private calculateExpiryStatus(dates: string | string[]): string {
    const dateArray = Array.isArray(dates) ? dates : [dates];
    const now = new Date();
    const twoWeeksAhead = new Date(now);
    twoWeeksAhead.setDate(twoWeeksAhead.getDate() + 14);

    const hasExpired = dateArray.some((date) => new Date(date) < now);
    if (hasExpired) {
      return 'expired';
    }

    const expiringSoon = dateArray.some((date) => {
      const expiry = new Date(date);
      return expiry >= now && expiry <= twoWeeksAhead;
    });

    if (expiringSoon) {
      return 'expires within two weeks';
    }

    return '';
  }

  isExpiringSoon(id: number): boolean {
    const status = this.expiryStatuses[id];
    return status === 'expires within two weeks' || status === 'expired';
  }

  getExpiryMessage(id: number): string {
    return this.expiryStatuses[id] || '';
  }
}
