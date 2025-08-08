export interface PermissionApplication {
  id: number;
  uniqueId?: string;
  employeeId: number;
  employeeName: string;
  licenceId: number;
  licenceName: string;
  isGrant: boolean;
}
