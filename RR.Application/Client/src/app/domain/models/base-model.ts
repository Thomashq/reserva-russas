export interface BaseModel {
  id: string; // Guid no C# corresponde a string no TypeScript
  creationDate?: Date | null;
  updateDate?: Date | null;
  deleteDate?: Date | null;
}
