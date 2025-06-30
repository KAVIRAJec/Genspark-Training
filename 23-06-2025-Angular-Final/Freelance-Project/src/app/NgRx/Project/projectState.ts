import { PaginationModel } from "../../Models/PaginationModel";
import { ProjectModel } from "../../Models/Project.model";

export interface ProjectState {
  projects: ProjectModel[];
  loading: boolean;
  error: string | null;
  pagination: PaginationModel | null;
}

export const initialProjectState: ProjectState = {
  projects: [],
  loading: false,
  error: null,
  pagination: null
};
