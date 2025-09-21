export interface AssignLicenceDTO {
  id: number; // was Id
  employeeId: number; // was EmployeeId
  licenceId: number; // was LicenceId
  licenceInstanceId?: number;
  employeeName: string; // was EmployeeName
  licenceName: string; // was LicenceName
  assignedOn?: string;
  validTo?: string;
}
