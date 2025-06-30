import { createAction, props } from "@ngrx/store";
import { User } from "../Models/User";

export const loadUsers = createAction('[User] Load Users');
export const loadUsersSuccess = createAction('[User] Load Users Success', props<{ users: User[] }>());
export const loadUsersFailure = createAction('[User] Load Users Failure', props<{ error: string }>());
export const addUser = createAction('[User] Add User', props<{ user: User }>());