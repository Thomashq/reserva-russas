export interface BaseModel {
  id: number; // Guid no C# corresponde a string no TypeScript
  creationDate?: Date | null;
  updateDate?: Date | null;
  deleteDate?: Date | null;
}
