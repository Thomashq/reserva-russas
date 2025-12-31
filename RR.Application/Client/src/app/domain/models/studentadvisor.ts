export interface IStudentAdvisor {
  id: number;
  studentId: number;
  servantId: number;
  isActive: boolean;
  createdAt?: string;
  updatedAt?: string;
}
